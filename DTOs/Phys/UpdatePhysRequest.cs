using System.ComponentModel.DataAnnotations;

namespace CrmWebApi.DTOs.Phys;

public record UpdatePhysRequest(
	int? SpecId,
	[MaxLength(100)] string? FirstName,
	[MaxLength(100)] string? LastName,
	[MaxLength(100)] string? MiddleName,
	[MaxLength(20)] string? Phone,
	[EmailAddress, MaxLength(200)] string? Email,
	[MaxLength(200)] string? Position
);
