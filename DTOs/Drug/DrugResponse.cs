namespace CrmWebApi.DTOs.Drug;

public record DrugResponse(
	int DrugId,
	string DrugName,
	string? Brand,
	string? Form,
	string? Description
);
