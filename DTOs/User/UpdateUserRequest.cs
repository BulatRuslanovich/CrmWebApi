using System.ComponentModel.DataAnnotations;

namespace CrmWebApi.DTOs.User;

public record UpdateUserRequest(
	[MaxLength(100)] string? FirstName,
	[MaxLength(100)] string? LastName,
	[EmailAddress, MaxLength(200)] string? Email,
	[MaxLength(20)] string? Phone
);
