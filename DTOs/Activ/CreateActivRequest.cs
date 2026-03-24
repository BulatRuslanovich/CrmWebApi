namespace CrmWebApi.DTOs.Activ;

public record CreateActivRequest(
    int OrgId,
    int StatusId,
    DateTimeOffset? Start,
    DateTimeOffset? End,
    string? Description,
    string? Result,
    List<int> DrugIds
);
