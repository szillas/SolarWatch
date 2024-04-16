namespace SolarWatch.Services.Authentication;

public interface IAuthService
{
    Task<AuthResult> RegisterAsync(string email, string userName, string password);
    Task<AuthResult> LoginAsync(string email, string password);
}