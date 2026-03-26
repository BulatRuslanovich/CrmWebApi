namespace CrmWebApi.DTOs.Activ;

public record ActivResponse(
	int ActivId,
	int UsrId,
	string UsrLogin,
	int OrgId,
	string OrgName,
	int StatusId,
	string StatusName,
	DateTimeOffset? Start,
	DateTimeOffset? End,
	string? Description,
	string? Result,
	List<string> Drugs
);
