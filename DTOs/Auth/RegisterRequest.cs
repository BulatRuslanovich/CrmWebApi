namespace CrmWebApi.DTOs.Auth;

public record RegisterRequest(
    string? FirstName,
    string? LastName,
    string? Email,
    string? Phone,
    string Login,
    string Password
);
