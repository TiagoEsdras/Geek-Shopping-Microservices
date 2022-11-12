using GeekShopping.IdentityServer.Configuration;
using GeekShopping.IdentityServer.Model.Context;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace GeekShopping.IdentityServer.Initializer;

public class DbInitializer : IDbInitializer
{
    private readonly MySQLContext context;
    private readonly UserManager<ApplicationUser> userManager;
    private readonly RoleManager<IdentityRole> roleManager;

    public DbInitializer(
        MySQLContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager
    )
    {
        this.context = context;
        this.userManager = userManager;
        this.roleManager = roleManager;
    }

    public void Initialize()
    {
        if (roleManager.FindByNameAsync(IdentityConfiguration.Admin).Result is not null) return;

        roleManager.CreateAsync(new IdentityRole(IdentityConfiguration.Admin)).GetAwaiter().GetResult();
        roleManager.CreateAsync(new IdentityRole(IdentityConfiguration.Client)).GetAwaiter().GetResult();

        ApplicationUser admin = new()
        {
            UserName = "tiago-admin",
            Email = "tiago-admin@email.com",
            EmailConfirmed = true,
            PhoneNumber = "+55 31 99999-9999",
            FirstName = "Tiago",
            LastName = "Admin"
        };

        userManager.CreateAsync(admin, "Admin@123").GetAwaiter().GetResult();
        userManager.AddToRoleAsync(admin, IdentityConfiguration.Admin).GetAwaiter().GetResult();

        var adminClains = userManager.AddClaimsAsync(admin, new Claim[]
        {
            new Claim(JwtClaimTypes.Name, $"{admin.FirstName} {admin.LastName}"),
            new Claim(JwtClaimTypes.GivenName, admin.FirstName),
            new Claim(JwtClaimTypes.FamilyName, admin.LastName),
            new Claim(JwtClaimTypes.Role, IdentityConfiguration.Admin),
        }).Result;

        ApplicationUser client = new()
        {
            UserName = "tiago-client",
            Email = "tiago-client@email.com",
            EmailConfirmed = true,
            PhoneNumber = "+55 31 99999-9999",
            FirstName = "Tiago",
            LastName = "Client"
        };

        userManager.CreateAsync(client, "Client@123").GetAwaiter().GetResult();
        userManager.AddToRoleAsync(client, IdentityConfiguration.Client).GetAwaiter().GetResult();

        var clientClains = userManager.AddClaimsAsync(client, new Claim[]
        {
            new Claim(JwtClaimTypes.Name, $"{client.FirstName} {client.LastName}"),
            new Claim(JwtClaimTypes.GivenName, client.FirstName),
            new Claim(JwtClaimTypes.FamilyName, client.LastName),
            new Claim(JwtClaimTypes.Role, IdentityConfiguration.Client),
        }).Result;
    }
}