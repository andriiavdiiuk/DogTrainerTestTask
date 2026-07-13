using DogTrainerTestTask.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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

    public static void SeedEntities(DbContext context, bool _)
    {
        if (context is not AppDbContext dbContext)
        {
            throw new InvalidOperationException($"Expected {nameof(AppDbContext)} but got {context.GetType().Name}");
        }

        dbContext.Users.AddRange(
            new User
            {
                Id = 1,
                UserName = "breeder1@test.com",
                Email = "breeder1@test.com"
            },
            new User
            {
                Id = 2,
                UserName = "breeder2@test.com",
                Email = "breeder2@test.com"
            },
            new User
            {
                Id = 3,
                UserName = "breeder3@test.com",
                Email = "breeder3@test.com"
            }
        );

        dbContext.BreederBenefits.AddRange(
            new BreederBenefit
            {
                BreederId = 1,
                FreeLimit = 5,
                UsedCount = 1
            },
            new BreederBenefit
            {
                BreederId = 2,
                FreeLimit = 10,
                UsedCount = 3
            },
            new BreederBenefit
            {
                BreederId = 3,
                FreeLimit = 2,
                UsedCount = 0
            }
        );
        dbContext.SaveChanges();
    }
}