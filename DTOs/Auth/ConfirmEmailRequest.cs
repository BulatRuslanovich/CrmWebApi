using System.ComponentModel.DataAnnotations;

namespace CrmWebApi.DTOs.Auth;

public record ConfirmEmailRequest(
	[Required, EmailAddress] string Email,
	[Required] string Code
);
