namespace CrmWebApi.DTOs.User;

public record UserResponse(
    int UsrId,
    string? FirstName,
    string? LastName,
    string? Email,
    string? Phone,
    string Login,
    List<string> Policies
);