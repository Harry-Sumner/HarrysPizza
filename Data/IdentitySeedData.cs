using Microsoft.AspNetCore.Identity;

namespace HarrysPizza.Data
{
    public class IdentitySeedData
    {
        public static async Task Initialize(HarrysPizzaContext context,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            context.Database.EnsureCreated();

            string adminRole = "Admin";
            string customerRole = "Customer";
            string password4all = "P@55word";

            if (await roleManager.FindByNameAsync(adminRole) == null)
            {
                await roleManager.CreateAsync(new IdentityRole(adminRole));
            }

            if (await roleManager.FindByNameAsync(customerRole) == null)
            {
                await roleManager.CreateAsync(new IdentityRole(customerRole));

            }

            if (await userManager.FindByNameAsync("admin@harryspizza.com") == null)
            {
                var user = new IdentityUser
                {
                    UserName = "admin@harryspizza.com",
                    Email = "admin@harryspizza.com",
                    PhoneNumber = "12345678911"
                };
                var result = await userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    await userManager.AddPasswordAsync(user, password4all);
                    await userManager.AddToRoleAsync(user, adminRole);
                }
            }
            if (await userManager.FindByNameAsync("customer@harryspizza.com") == null)
            {
                var user = new IdentityUser
                {
                    UserName = "customer@harryspizza.com",
                    Email = "customer@harryspizza.com",
                    PhoneNumber = "12345678910"
                };
                var result = await userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    await userManager.AddPasswordAsync(user, password4all);
                    await userManager.AddToRoleAsync(user, customerRole);
                }
            }
        }
    }
}

    
