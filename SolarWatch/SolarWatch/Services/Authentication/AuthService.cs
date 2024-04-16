using Microsoft.AspNetCore.Identity;

namespace SolarWatch.Services.Authentication;

public class AuthService : IAuthService
{
    private readonly UserManager<IdentityUser> _userManager;

    public AuthService(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<AuthResult> RegisterAsync(string email, string userName, string password)
        {
            var result = await _userManager.CreateAsync(
                new IdentityUser { UserName = userName, Email = email }, password);

            if (!result.Succeeded)
            {
                return FailedRegistration(result, email, userName);
            }

            return new AuthResult(true, email, userName, "");
        }
    
    private static AuthResult FailedRegistration(IdentityResult result, string email, string username)
    {
        var authResult = new AuthResult(false, email, username, "");

        foreach (var error in result.Errors)
        {
            authResult.ErrorMessages.Add(error.Code, error.Description);
        }

        return authResult;
    }
}