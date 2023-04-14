using GameStore.Models.Identity;
using GameStoreWeb.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GameStore.Utility.Extensions
{
    public static class ApplicationBuilder
    {
        /// <summary>
        /// Seeds related IdentityUsers of type <see cref="GameStore.Models.Identity.ApplicationUser"/> and Roles in DataBase
        /// </summary>
        /// <remarks>Default Accounts: Admin, User. Password = Password123!</remarks>
        /// <param name="app">An instance of <see cref="IApplicationBuilder"/>.</param>
        /// <returns></returns>
        public static async Task SeedIdentityDb(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;

                using (var context = new ApplicationDbContext(
                scope.ServiceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
                {
                    if (!context.Database.GetPendingMigrations().Any())
                        context.Database.Migrate();

                    var adminUser = await EnsureUser(serviceProvider, "Admin", "Password123!");
                    await EnsureRole(serviceProvider, adminUser, "Admin");

                    var normalUser = await EnsureUser(serviceProvider, "User", "Password123!");
                    await EnsureRole(serviceProvider, normalUser, "User");
                }
            }
        }

        private static async Task<string> EnsureUser(
            IServiceProvider serviceProvider,
            string userName, string initPw)
        {
            var userManager = serviceProvider.GetService<UserManager<IdentityUser>>();

            var user = await userManager.FindByNameAsync(userName);

            // Creating new user
            if (user is null)
            {
                user = new ApplicationUser
                {
                    UserName = userName,
                    Email = userName + "@gmail.com",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, initPw);
            }

            if (user is null)
                throw new Exception("User did not get created. Password policy problem?");

            return user.Id;
        }

        private static async Task<IdentityResult> EnsureRole(
            IServiceProvider serviceProvider, string uid, string role)
        {
            var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();

            IdentityResult ir;

            if (!await roleManager.RoleExistsAsync(role))
            {
                ir = await roleManager.CreateAsync(new(role));
            }

            var userManager = serviceProvider.GetService<UserManager<IdentityUser>>();

            var user = await userManager.FindByIdAsync(uid);

            if (user is null)
                throw new Exception("User not existing");

            ir = await userManager.AddToRoleAsync(user, role);

            return ir;
        }
    }
}
