using Microsoft.AspNetCore.Identity;

class SeedingData
{
    public static async Task SeedUser(
        IServiceProvider services,
        ApiUser seededUser,
        string password,
        string role)
    {
        var userManager = services.GetRequiredService<UserManager<ApiUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        // Check if user exists
        var user = await userManager.FindByEmailAsync(seededUser.Email);

        if (user == null)
        {
            var newUser = new ApiUser
            {
                UserName = seededUser.Email,
                Email = seededUser.Email,
                FullName = seededUser.FullName,
                EmailConfirmed = true
            };

            var createResult = await userManager.CreateAsync(newUser, password);

            if (!createResult.Succeeded)
            {
                var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                throw new Exception($"User creation failed: {errors}");
            }

            user = newUser;
        }

        // Ensure role exists
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }

        // Assign role
        if (!await userManager.IsInRoleAsync(user, role))
        {
            var roleResult = await userManager.AddToRoleAsync(user, role);

            if (!roleResult.Succeeded)
            {
                var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                throw new Exception($"Role assignment failed: {errors}");
            }
        }
    }
}