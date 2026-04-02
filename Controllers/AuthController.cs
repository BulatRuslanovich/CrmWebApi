using CrmWebApi.DTOs.Auth;
using CrmWebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CrmWebApi.Controllers;

[Route("api/auth")]
public class AuthController(IAuthService service) : ApiController
{
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterRequest req)
    {
        var result = await service.RegisterAsync(req);
        if (!result.IsSuccess) return MapError(result.Error!);
        return Accepted(result.Value);
    }

    [HttpPost("confirm-email")]
    [AllowAnonymous]
    public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailRequest req) =>
        FromResult(await service.ConfirmEmailAsync(req));

    [HttpPost("resend-confirmation")]
    [AllowAnonymous]
    public async Task<IActionResult> ResendConfirmation([FromBody] string email) =>
        FromResult(await service.ResendConfirmationAsync(email));

    [HttpPost("forgot-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest req) =>
        FromResult(await service.ForgotPasswordAsync(req.Email));

    [HttpPost("reset-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest req) =>
        FromResult(await service.ResetPasswordAsync(req));

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest req) =>
        FromResult(await service.LoginAsync(req));

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> Refresh([FromBody] string refreshToken) =>
        FromResult(await service.RefreshAsync(refreshToken));

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout([FromBody] string refreshToken) =>
        FromResult(await service.LogoutAsync(refreshToken));
}
