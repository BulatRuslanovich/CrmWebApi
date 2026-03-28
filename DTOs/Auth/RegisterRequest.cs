using System.ComponentModel.DataAnnotations;

namespace CrmWebApi.DTOs.Auth;

public record RegisterRequest(
	[MaxLength(100)] string? FirstName,
	[MaxLength(100)] string? LastName,
	[EmailAddress, MaxLength(200)] string? Email,
	[MaxLength(20)] string? Phone,
	[Required, MaxLength(100)] string Login,
	[Required, MinLength(6), MaxLength(100)] string Password
);
