using System.Security.Claims;
using Domain.Enums;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Initializers
{
    public static class SystemAdministratorInitializer
    {
        public static async ValueTask InitializeSystemAdministratorAsync(
        IServiceProvider serviceProvider,
        IConfiguration config)
        {
            using var scope = serviceProvider.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var httpContextAccessor = scope.ServiceProvider.GetRequiredService<IHttpContextAccessor>();

            var email = config["Logging:SystemAdminSettings:Email"]!;
            var userName = config["Logging:SystemAdminSettings:UserName"]!;
            var password = config["Logging:SystemAdminSettings:Password"]!;
            var firstName = config["Logging:SystemAdminSettings:FirstName"]!;
            var lastName = config["Logging:SystemAdminSettings:LastName"]!;

            var systemAdminRole = await roleManager.FindByNameAsync(Role.SystemAdministrator.ToString());
            if (systemAdminRole == null)
            {
                throw new NullReferenceException("System Administration role creation failed.");
            }

            var existingAdminUser = await userManager.FindByNameAsync(userName!);
            if (existingAdminUser != null)
            {
                return;
            }

            var adminUserGuid = Guid.NewGuid();

            var adminUser = new User
            {
                Id = adminUserGuid,
                Email = email,
                UserName = userName,
                FirstName = firstName,
                LastName = lastName ,
                Age = 77,
            };

            httpContextAccessor.HttpContext = new DefaultHttpContext();
            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Sid, adminUser.Id.ToString()));
            httpContextAccessor.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(claims));

            await userManager.CreateAsync(adminUser, password);
            await userManager.AddToRoleAsync(adminUser, Role.SystemAdministrator.ToString());
        }
    }
}