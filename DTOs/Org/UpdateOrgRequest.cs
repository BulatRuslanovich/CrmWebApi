using System.ComponentModel.DataAnnotations;

namespace CrmWebApi.DTOs.Org;

public record UpdateOrgRequest(
	int? OrgTypeId,
	[MaxLength(200)] string? OrgName,
	[MaxLength(12)] string? Inn,
	double? Latitude,
	double? Longitude,
	[MaxLength(500)] string? Address
);
