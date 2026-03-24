namespace CrmWebApi.DTOs.User;


public record UpdateUserRequest(
    string? FirstName,
    string? LastName,
    string? Email,
    string? Phone
);