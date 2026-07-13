using DogTrainerTestTask.Data.Entities;
using Microsoft.AspNetCore.Identity;

namespace DogTrainerTestTask.Data;

public static class DataConfiguration
{
    public static void ConfigureRoles(this IServiceProvider serviceProvider)
    {
        ConfigureRolesAsync(serviceProvider).Wait();
    }
    
    private static async Task ConfigureRolesAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<UserRole>>();

        foreach (var role in Enum.GetNames<UserRoles>())
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new UserRole(role));
            }
        }
    }
}