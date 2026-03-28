using System.ComponentModel.DataAnnotations;

namespace CrmWebApi.DTOs.User;

public record ChangePasswordRequest(
	[Required] string OldPassword,
	[Required, MinLength(6), MaxLength(100)] string NewPassword
);
