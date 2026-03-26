using CrmWebApi.DTOs.Auth;

namespace CrmWebApi.Services;

public interface IAuthService
{
	public Task<AuthResponse> RegisterAsync(RegisterRequest req);
	public Task<AuthResponse> LoginAsync(LoginRequest req);
	public Task<AuthResponse> RefreshAsync(string refreshToken);
	public Task LogoutAsync(string refreshToken);
}
