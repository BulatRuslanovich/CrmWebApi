using CrmWebApi.DTOs.Auth;

namespace CrmWebApi.Services;

public interface IAuthService
{
	public Task<PendingConfirmationResponse> RegisterAsync(RegisterRequest req);
	public Task<AuthResponse> ConfirmEmailAsync(ConfirmEmailRequest req);
	public Task ResendConfirmationAsync(string email);
	public Task<AuthResponse> LoginAsync(LoginRequest req);
	public Task<AuthResponse> RefreshAsync(string refreshToken);
	public Task LogoutAsync(string refreshToken);
	public Task ForgotPasswordAsync(string email);
	public Task ResetPasswordAsync(ResetPasswordRequest req);
}
