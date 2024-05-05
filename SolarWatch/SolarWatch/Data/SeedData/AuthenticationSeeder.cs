using Microsoft.AspNetCore.Identity;

namespace SolarWatch.Data.SeedData;

public class AuthenticationSeeder
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IConfiguration _configuration;

    public AuthenticationSeeder(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager, IConfiguration configuration)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _configuration = configuration;
    }
    
    public void AddRoles()
    {
        var adminRoleName = _configuration["RoleNames:Admin"];
        var userRoleName = _configuration["RoleNames:User"];

        if (adminRoleName != null)
        {
            var tAdmin = CreateRole(_roleManager, adminRoleName);
            tAdmin.Wait();
        }

        if (userRoleName != null)
        {
            var tUser = CreateRole(_roleManager, userRoleName);
            tUser.Wait();
        }
    }

    private async Task CreateRole(RoleManager<IdentityRole> roleManager, string roleName)
    {
        await roleManager.CreateAsync(new IdentityRole(roleName)); 
    }
    
    public void AddAdmin()
    {
        var tAdmin = CreateAdminIfNotExists();
        tAdmin.Wait();
    }

    private async Task CreateAdminIfNotExists()
    {
        var adminInDb = await _userManager.FindByEmailAsync("admin@admin.com");
        if (adminInDb == null)
        {
            var admin = new IdentityUser { UserName = "admin", Email = "admin@admin.com" };
            var adminCreated = await _userManager.CreateAsync(admin, "admin123");

            if (adminCreated.Succeeded)
            {
                await _userManager.AddToRoleAsync(admin, "Admin");
                //await _userManager.AddToRoleAsync(admin, _configuration["RoleNames:Admin"]);
            }
        }
    }
    
}