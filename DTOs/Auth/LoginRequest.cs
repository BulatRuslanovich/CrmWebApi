using System.ComponentModel.DataAnnotations;

namespace CrmWebApi.DTOs.Auth;

public record LoginRequest(
	[Required] string Login,
	[Required] string Password
);
