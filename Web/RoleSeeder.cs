using Microsoft.AspNetCore.Identity;

namespace Web
{
    /// <summary>
    /// Provides functionality to seed predefined roles into the identity system.
    /// </summary>
    /// <remarks>This class is designed to ensure that essential roles, such as "Admin" and "User",  are
    /// created in the identity system if they do not already exist. It is typically used  during application
    /// initialization or database seeding.</remarks>
    public static class RoleSeeder
    {
        public static async Task SeedRolesAsync(RoleManager<IdentityRole<Guid>> roleManager)
        {
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>("Admin"));
            }

            if (!await roleManager.RoleExistsAsync("User"))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>("User"));
            }
        }
    }
}
