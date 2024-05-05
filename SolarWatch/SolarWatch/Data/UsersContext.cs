using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace SolarWatch.Data;

public class UsersContext : IdentityDbContext<IdentityUser, IdentityRole, string>
{
    private readonly IConfiguration _configuration;
    public UsersContext(DbContextOptions<UsersContext> options, IConfiguration configuration) : base(options)
    {
        _configuration = configuration;
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        var adminRoleName = _configuration["RoleNames:Admin"];
        var userRoleName = _configuration["RoleNames:User"];
        var adminEmail = _configuration["SolarWatch:AdminEmail"];
        var adminPassword = _configuration["SolarWatch:AdminPassword"];
        
        // Seed roles
        if (adminRoleName != null)
        {
            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole { Id = "1", Name = adminRoleName, NormalizedName = adminRoleName.ToUpper() }
            );
        }
        if (userRoleName != null)
        {
            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole { Id = "2", Name = userRoleName, NormalizedName = userRoleName.ToUpper() }
            );
        }
        
        // Seed admin user
        if (adminEmail is not null && adminPassword is not null)
        {
            var adminUser = new IdentityUser
            {
                Id = "1",
                UserName = "admin",
                NormalizedUserName = "ADMIN",
                Email = adminEmail,
                NormalizedEmail = adminEmail.ToUpper(),
                EmailConfirmed = true
            };
            var passwordHasher = new PasswordHasher<IdentityUser>();
            adminUser.PasswordHash = passwordHasher.HashPassword(adminUser, adminPassword);

            modelBuilder.Entity<IdentityUser>().HasData(adminUser);

            // Assign admin role to admin user
            modelBuilder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string> { UserId = adminUser.Id, RoleId = "1" }
            );
        }
    }
    
   

}