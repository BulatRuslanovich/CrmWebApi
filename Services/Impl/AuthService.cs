using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using CrmWebApi.Data.Entities;
using CrmWebApi.DTOs.Auth;
using CrmWebApi.DTOs.User;
using CrmWebApi.Middleware;
using CrmWebApi.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace CrmWebApi.Services.Impl;

public class AuthService(
	IUserRepository userRepo,
	IRefreshRepository refreshRepo,
	IEmailTokenRepository emailTokenRepo,
	IEmailService emailService,
	IConfiguration config,
	ILogger<AuthService> logger) : IAuthService
{
	private const int TokenTypeConfirmation = 0;
	private const int TokenTypePasswordReset = 1;

	public async Task<PendingConfirmationResponse> RegisterAsync(RegisterRequest req)
	{
		if (req.Email is null or "")
			throw new ArgumentException("Email обязателен для регистрации");

		if (await userRepo.ExistsAsync(u => u.UsrLogin == req.Login))
			throw new InvalidOperationException("Логин уже занят");

		if (await userRepo.ExistsAsync(u => u.UsrEmail == req.Email))
			throw new InvalidOperationException("Пользователь с таким email уже существует");

		var user = new Usr
		{
			UsrFirstname = req.FirstName,
			UsrLastname = req.LastName,
			UsrEmail = req.Email,
			UsrPhone = req.Phone,
			UsrLogin = req.Login,
			UsrPasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password),
			IsEmailConfirmed = false
		};
		await userRepo.AddAsync(user);

		await SendOtpAsync(user, TokenTypeConfirmation, expiryHours: 24);

		var displayName = $"{req.FirstName ?? ""} {req.LastName ?? ""}".Trim();
		if (displayName == "") displayName = req.Login;
		try
		{
			await emailService.SendEmailConfirmationAsync(req.Email, displayName, await GetLatestOtpAsync(user.UsrId, TokenTypeConfirmation));
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Не удалось отправить письмо подтверждения на {Email}", req.Email);
		}

		return new PendingConfirmationResponse(req.Email);
	}

	public async Task<AuthResponse> ConfirmEmailAsync(ConfirmEmailRequest req)
	{
		var user = await userRepo.QueryActive().FirstOrDefaultAsync(u => u.UsrEmail == req.Email)
			?? throw new KeyNotFoundException("Пользователь не найден");

		if (user.IsEmailConfirmed)
			throw new ArgumentException("Email уже подтверждён");

		await VerifyOtpAsync(user.UsrId, req.Code, TokenTypeConfirmation);

		user.IsEmailConfirmed = true;
		await userRepo.UpdateAsync(user);
		await emailTokenRepo.DeleteAllForUserAsync(user.UsrId, TokenTypeConfirmation);

		var fullUser = await userRepo.QueryActive().FirstAsync(u => u.UsrId == user.UsrId);
		return await IssueTokensAsync(fullUser);
	}

	public async Task ResendConfirmationAsync(string email)
	{
		var user = await userRepo.QueryActive().FirstOrDefaultAsync(u => u.UsrEmail == email)
			?? throw new KeyNotFoundException("Пользователь не найден");

		if (user.IsEmailConfirmed)
			throw new ArgumentException("Email уже подтверждён");

		await emailTokenRepo.DeleteAllForUserAsync(user.UsrId, TokenTypeConfirmation);
		await SendOtpAsync(user, TokenTypeConfirmation, expiryHours: 24);

		var name = $"{user.UsrFirstname ?? ""} {user.UsrLastname ?? ""}".Trim();
		if (name == "") name = user.UsrLogin;
		var code = await GetLatestOtpAsync(user.UsrId, TokenTypeConfirmation);
		try
		{
			await emailService.SendEmailConfirmationAsync(email, name, code);
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Не удалось отправить письмо подтверждения на {Email}", email);
			throw new InvalidOperationException("Не удалось отправить письмо. Попробуйте позже.");
		}
	}

	public async Task<AuthResponse> LoginAsync(LoginRequest req)
	{
		var user = await userRepo.QueryActive()
			.FirstOrDefaultAsync(u => u.UsrLogin == req.Login)
			?? throw new UnauthorizedAccessException("Неверный логин или пароль");

		if (!BCrypt.Net.BCrypt.Verify(req.Password, user.UsrPasswordHash))
			throw new UnauthorizedAccessException("Неверный логин или пароль");

		if (!user.IsEmailConfirmed)
			throw new EmailNotConfirmedException(user.UsrEmail ?? "");

		return await IssueTokensAsync(user);
	}

	public async Task<AuthResponse> RefreshAsync(string refreshToken)
	{
		var hash = HashToken(refreshToken);
		var stored = await refreshRepo.GetByTokenHashAsync(hash)
			?? throw new UnauthorizedAccessException("Refresh token не найден или уже использован");

		if (stored.RefreshExpiresAt < DateTime.UtcNow)
		{
			await refreshRepo.DeleteAsync(stored);
			throw new UnauthorizedAccessException("Refresh token истёк");
		}

		await refreshRepo.DeleteAsync(stored);

		var user = await userRepo.QueryActive().FirstOrDefaultAsync(u => u.UsrId == stored.UsrId)
			?? throw new UnauthorizedAccessException("Пользователь не найден или удалён");

		return await IssueTokensAsync(user);
	}

	public async Task LogoutAsync(string refreshToken)
	{
		var hash = HashToken(refreshToken);
		var stored = await refreshRepo.GetByTokenHashAsync(hash);
		if (stored is not null)
			await refreshRepo.DeleteAsync(stored);
	}

	public async Task ForgotPasswordAsync(string email)
	{
		var user = await userRepo.QueryActive().FirstOrDefaultAsync(u => u.UsrEmail == email);
		// Silently succeed even if user not found to avoid enumeration
		if (user is null) return;

		await emailTokenRepo.DeleteAllForUserAsync(user.UsrId, TokenTypePasswordReset);
		await SendOtpAsync(user, TokenTypePasswordReset, expiryHours: 1);

		var name = $"{user.UsrFirstname ?? ""} {user.UsrLastname ?? ""}".Trim();
		if (name == "") name = user.UsrLogin;
		var code = await GetLatestOtpAsync(user.UsrId, TokenTypePasswordReset);
		try
		{
			await emailService.SendPasswordResetAsync(email, name, code);
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Не удалось отправить письмо сброса пароля на {Email}", email);
			throw new InvalidOperationException("Не удалось отправить письмо. Попробуйте позже.");
		}
	}

	public async Task ResetPasswordAsync(ResetPasswordRequest req)
	{
		if (req.NewPassword.Length < 6)
			throw new ArgumentException("Минимум 6 символов");

		var user = await userRepo.QueryActive().FirstOrDefaultAsync(u => u.UsrEmail == req.Email)
			?? throw new KeyNotFoundException("Пользователь не найден");

		await VerifyOtpAsync(user.UsrId, req.Code, TokenTypePasswordReset);

		user.UsrPasswordHash = BCrypt.Net.BCrypt.HashPassword(req.NewPassword);
		await userRepo.UpdateAsync(user);
		await emailTokenRepo.DeleteAllForUserAsync(user.UsrId, TokenTypePasswordReset);
		await refreshRepo.RevokeAllForUserAsync(user.UsrId);
	}

	// ── helpers ─────────────────────────────────────────────────────────────

	private async Task SendOtpAsync(Usr user, int tokenType, int expiryHours)
	{
		var code = Random.Shared.Next(100_000, 1_000_000).ToString();
		var stored = new EmailToken
		{
			UsrId = user.UsrId,
			TokenHash = HashToken(code),
			TokenType = tokenType,
			ExpiresAt = DateTime.UtcNow.AddHours(expiryHours)
		};
		await emailTokenRepo.AddAsync(stored);
		// Store raw code temporarily in memory so caller can retrieve it
		_pendingCodes[(user.UsrId, tokenType)] = code;
	}

	// Tiny in-process cache: lives only during this request scope
	private readonly Dictionary<(int, int), string> _pendingCodes = new();

	private Task<string> GetLatestOtpAsync(int usrId, int tokenType)
	{
		if (_pendingCodes.TryGetValue((usrId, tokenType), out var code))
			return Task.FromResult(code);
		throw new InvalidOperationException("OTP не был создан в этом запросе");
	}

	private async Task VerifyOtpAsync(int usrId, string code, int tokenType)
	{
		var hash = HashToken(code.Trim());
		var token = await emailTokenRepo.GetValidTokenAsync(hash, tokenType)
			?? throw new ArgumentException("Неверный или истёкший код");

		if (token.UsrId != usrId)
			throw new ArgumentException("Неверный или истёкший код");

		if (token.ExpiresAt < DateTime.UtcNow)
		{
			await emailTokenRepo.DeleteAsync(token);
			throw new ArgumentException("Код истёк");
		}
	}

	private async Task<AuthResponse> IssueTokensAsync(Usr user)
	{
		var accessToken = GenerateAccessToken(user);
		var (raw, stored) = GenerateRefreshToken(user.UsrId);
		await refreshRepo.AddAsync(stored);
		return new AuthResponse(accessToken, raw, MapToResponse(user));
	}

	private string GenerateAccessToken(Usr user)
	{
		var secret = config["Jwt:Secret"]!;
		var issuer = config["Jwt:Issuer"]!;
		var audience = config["Jwt:Audience"]!;
		var ttl = int.Parse(config["Jwt:AccessTokenTtlMinutes"] ?? "15");

		var claims = new List<Claim>
		{
			new(ClaimTypes.NameIdentifier, user.UsrId.ToString()),
			new(ClaimTypes.Name, user.UsrLogin)
		};
		claims.AddRange(user.UsrPolicies
			.Where(p => !p.Policy.IsDeleted)
			.Select(p => new Claim(ClaimTypes.Role, p.Policy.PolicyName)));

		var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
		var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
		var token = new JwtSecurityToken(issuer, audience, claims,
			expires: DateTime.UtcNow.AddMinutes(ttl),
			signingCredentials: creds);

		return new JwtSecurityTokenHandler().WriteToken(token);
	}

	private (string raw, Refresh stored) GenerateRefreshToken(int usrId)
	{
		var ttlDays = int.Parse(config["Jwt:RefreshTokenTtlDays"] ?? "7");
		var raw = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
		var stored = new Refresh
		{
			UsrId = usrId,
			RefreshTokenHash = HashToken(raw),
			RefreshExpiresAt = DateTime.UtcNow.AddDays(ttlDays)
		};
		return (raw, stored);
	}

	private static string HashToken(string token)
	{
		var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
		return Convert.ToHexString(bytes).ToLowerInvariant();
	}

	private static UserResponse MapToResponse(Usr u) => new(
		u.UsrId,
		u.UsrFirstname,
		u.UsrLastname,
		u.UsrEmail,
		u.UsrPhone,
		u.UsrLogin,
		[.. u.UsrPolicies.Where(p => !p.Policy.IsDeleted).Select(p => p.Policy.PolicyName)]
	);
}
