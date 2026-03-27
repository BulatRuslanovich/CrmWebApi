using CrmWebApi.DTOs.Auth;
using CrmWebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CrmWebApi.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService service) : ControllerBase
{
	[HttpPost("register")]
	[AllowAnonymous]
	public async Task<IActionResult> Register([FromBody] RegisterRequest req)
	{
		var result = await service.RegisterAsync(req);
		return Accepted(result);
	}

	[HttpPost("confirm-email")]
	[AllowAnonymous]
	public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailRequest req)
	{
		var result = await service.ConfirmEmailAsync(req);
		return Ok(result);
	}

	[HttpPost("resend-confirmation")]
	[AllowAnonymous]
	public async Task<IActionResult> ResendConfirmation([FromBody] string email)
	{
		await service.ResendConfirmationAsync(email);
		return NoContent();
	}

	[HttpPost("forgot-password")]
	[AllowAnonymous]
	public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest req)
	{
		await service.ForgotPasswordAsync(req.Email);
		return NoContent();
	}

	[HttpPost("reset-password")]
	[AllowAnonymous]
	public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest req)
	{
		await service.ResetPasswordAsync(req);
		return NoContent();
	}

	[HttpPost("login")]
	[AllowAnonymous]
	public async Task<IActionResult> Login([FromBody] LoginRequest req)
	{
		var result = await service.LoginAsync(req);
		return Ok(result);
	}

	[HttpPost("refresh")]
	[AllowAnonymous]
	public async Task<IActionResult> Refresh([FromBody] string refreshToken)
	{
		var result = await service.RefreshAsync(refreshToken);
		return Ok(result);
	}

	[HttpPost("logout")]
	[Authorize]
	public async Task<IActionResult> Logout([FromBody] string refreshToken)
	{
		await service.LogoutAsync(refreshToken);
		return NoContent();
	}
}
