namespace CrmWebApi.DTOs.User;

public record CreateUserRequest(
    string? FirstName,
    string? LastName,
    string? Email,
    string? Phone,
    string Login,
    string Password,
    List<int> PolicyIds
);
