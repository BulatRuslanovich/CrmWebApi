namespace CrmWebApi.Middleware;

public class EmailNotConfirmedException(string email)
	: Exception("Email не подтверждён")
{
	public string Email { get; } = email;
}
