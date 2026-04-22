using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
class SeedingData {
    
    public static async Task SeedUser(IServiceProvider services, ApiUser seededUser, string Password, string Role) {
        var userManager = services.GetRequiredService<UserManager<ApiUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();


        var User = await userManager.FindByEmailAsync(seededUser.Email);

        if (User == null) {
            var user = new ApiUser {
                Email = seededUser.Email,
                FullName = seededUser.FullName,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, Password);

            if (result.Succeeded) {
                if (!await roleManager.RoleExistsAsync(Role)){
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new Exception($"Failed to assign user to role: {Role}");
                }
                await userManager.AddToRoleAsync(user, Role);
            }
        }
    }
}