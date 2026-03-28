using System.ComponentModel.DataAnnotations;

namespace CrmWebApi.DTOs.Auth;

public record ForgotPasswordRequest(
	[Required, EmailAddress] string Email
);
