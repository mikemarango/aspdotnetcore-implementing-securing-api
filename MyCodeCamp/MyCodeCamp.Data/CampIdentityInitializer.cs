using Microsoft.AspNetCore.Identity;
using MyCodeCamp.Data.Entities;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MyCodeCamp.Data
{
    public class CampIdentityInitializer
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<CampUser> userManager;

        public CampIdentityInitializer(UserManager<CampUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        public async Task Seed()
        {
            var user = await userManager.FindByNameAsync("shawnwildermuth");

            // Add User
            if (user == null)
            {
                if (!await roleManager.RoleExistsAsync("Admin"))
                {
                    var role = new IdentityRole("Admin");
                    // TODO: FIX THIS
                    //role.Claims.Add(new IdentityRoleClaim<string> { ClaimType = "IsAdmin", ClaimValue = "True" });
                    await roleManager.CreateAsync(role);
                }

                user = new CampUser()
                {
                    UserName = "shawnwildermuth",
                    FirstName = "Shawn",
                    LastName = "Wildermuth",
                    Email = "shawn@wildermuth.com"
                };

                var userResult = await userManager.CreateAsync(user, "P@ssw0rd!");
                var roleResult = await userManager.AddToRoleAsync(user, "Admin");
                var claimResult = await userManager.AddClaimAsync(user, new Claim("SuperUser", "True"));

                if (!userResult.Succeeded || !roleResult.Succeeded || !claimResult.Succeeded)
                {
                    throw new InvalidOperationException("Failed to build user and roles");
                }
            }
        }
    }
}
