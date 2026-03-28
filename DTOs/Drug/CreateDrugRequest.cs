using System.ComponentModel.DataAnnotations;

namespace CrmWebApi.DTOs.Drug;

public record CreateDrugRequest(
	[Required, MaxLength(200)] string DrugName,
	[MaxLength(200)] string? Brand,
	[MaxLength(100)] string? Form,
	[MaxLength(1000)] string? Description
);
