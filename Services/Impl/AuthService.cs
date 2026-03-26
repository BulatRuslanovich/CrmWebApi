using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using CrmWebApi.Data.Entities;
using CrmWebApi.DTOs.Auth;
using CrmWebApi.DTOs.User;
using CrmWebApi.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace CrmWebApi.Services.Impl;

public class AuthService(
	IUserRepository userRepo,
	IRefreshRepository refreshRepo,
	IConfiguration config) : IAuthService
{
	public async Task<AuthResponse> RegisterAsync(RegisterRequest req)
	{
		if (await userRepo.ExistsAsync(u => u.UsrLogin == req.Login))
			throw new InvalidOperationException("Логин уже занят");

		if (req.Email != null && await userRepo.ExistsAsync(u => u.UsrEmail == req.Email))
			throw new InvalidOperationException("Пользователь с таким email уже существует");

		var user = new Usr
		{
			UsrFirstname = req.FirstName,
			UsrLastname = req.LastName,
			UsrEmail = req.Email,
			UsrPhone = req.Phone,
			UsrLogin = req.Login,
			UsrPasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password)
		};
		await userRepo.AddAsync(user);

		var fullUser = await userRepo.QueryActive().FirstAsync(u => u.UsrId == user.UsrId);
		return await IssueTokensAsync(fullUser);
	}

	public async Task<AuthResponse> LoginAsync(LoginRequest req)
	{
		var user = await userRepo.QueryActive()
			.FirstOrDefaultAsync(u => u.UsrLogin == req.Login)
			?? throw new UnauthorizedAccessException("Неверный логин или пароль");

		if (!BCrypt.Net.BCrypt.Verify(req.Password, user.UsrPasswordHash))
			throw new UnauthorizedAccessException("Неверный логин или пароль");

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
