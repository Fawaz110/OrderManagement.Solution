using Microsoft.AspNetCore.Identity;
using OrderManagement.Entities;

namespace OrderManagement.Helper
{
    public static class ContextSeed
    {
        public async static Task ApplyRolesSeed(RoleManager<IdentityRole> roleManager, ILogger logger)
        {
            if (!roleManager.Roles.Any())
            {
                string[] roleNames = { "Admin", "Customer" };

                foreach (var roleName in roleNames)
                    if (!await roleManager.RoleExistsAsync(roleName))
                    {
                        var result = await roleManager.CreateAsync(new IdentityRole(roleName));

                        if (result.Succeeded)
                            logger.LogInformation("Role Added Successfully <3");
                    }
            }
            else
                logger.LogWarning("No Need For Roles Seeding");
        }

        public async static Task ApplyUserSeeding(UserManager<AppUser> userManager, ILogger logger)
        {
            if (!userManager.Users.Any())
            {
                var admin = new AppUser
                {
                    Name = "Mustafa",
                    Email = "mustafa.admin@store.com",
                };
                admin.UserName = admin.Name + Guid.NewGuid().ToString().Split('-')[0];

                var result = await userManager.CreateAsync(admin, "Mustafa@123");

                if (result.Succeeded)
                {
                    result = await userManager.AddToRoleAsync(admin, "Admin");

                    if(result.Succeeded)
                        logger.LogInformation("Admin added successfully <3");
                }

                var customer = new AppUser
                {
                    Name = "Mustafa",
                    Email = "mustafa.customer@store.com",
                };
                customer.UserName = customer.Name + Guid.NewGuid().ToString().Split('-')[0];

                result = await userManager.CreateAsync(customer, "Mustafa@123");

                if (result.Succeeded)
                {
                    result = await userManager.AddToRoleAsync(customer, "Customer");

                    if (result.Succeeded)
                        logger.LogInformation("Customer added successfully <3");
                }
            }
            else
                logger.LogInformation("No Need For Users Seeding");
        }
    }
}
