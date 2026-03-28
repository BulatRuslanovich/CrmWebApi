using System.ComponentModel.DataAnnotations;

namespace CrmWebApi.DTOs.Auth;

public record ResetPasswordRequest(
	[Required, EmailAddress] string Email,
	[Required] string Code,
	[Required, MinLength(6), MaxLength(100)] string NewPassword
);
