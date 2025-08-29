using Microsoft.AspNetCore.Identity;

namespace EngConnect.Api.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static async Task SeedRolesAsync(this IApplicationBuilder app)
        {
            // Create a scope to retrieve scoped services
            using (IServiceScope scope = app.ApplicationServices.CreateScope())
            {
                // Retrieve the RoleManager from DI
                RoleManager<IdentityRole> roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                // Define the roles to seed
                string[] roleNames = { "Admin", "Student", "Tutor" };

                // Loop through each role and create it if it doesn't exist
                foreach (string roleName in roleNames)
                {
                    if (!await roleManager.RoleExistsAsync(roleName))
                    {
                        await roleManager.CreateAsync(new IdentityRole(roleName));
                    }
                }
            }
        }
    }
}
