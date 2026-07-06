using Microsoft.AspNetCore.Identity;
using INFM_Web.Constants;

namespace INFM_Web.Data
{
    public class DbSeeder
    {
        public static async Task SeedDefaultData(IServiceProvider service)
        {
            var userMgr = service.GetRequiredService<UserManager<IdentityUser>>();
            var roleMgr = service.GetRequiredService<RoleManager<IdentityRole>>();

            // Ensure every application role exists (idempotent).
            foreach (var role in Enum.GetNames(typeof(Roles)))
            {
                if (!await roleMgr.RoleExistsAsync(role))
                {
                    await roleMgr.CreateAsync(new IdentityRole(role));
                }
            }

            // Ensure a default admin account exists so the back-office is reachable
            // on a fresh database.
            await EnsureUserAsync(userMgr, "admin@techserve.com", "Admin$$9901", Roles.Admin);
            await EnsureUserAsync(userMgr, "worker@techserve.com", "Worker$$9901", Roles.Worker);
            await EnsureUserAsync(userMgr, "user@techserve.com", "User$$9901", Roles.User);
        }

        private static async Task EnsureUserAsync(
            UserManager<IdentityUser> userMgr, string email, string password, Roles role)
        {
            var existing = await userMgr.FindByEmailAsync(email);
            if (existing != null)
            {
                return;
            }

            var user = new IdentityUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
            };

            var result = await userMgr.CreateAsync(user, password);
            if (result.Succeeded)
            {
                await userMgr.AddToRoleAsync(user, role.ToString());
            }
        }
    }
}
