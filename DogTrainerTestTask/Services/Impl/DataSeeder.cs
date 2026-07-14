using DogTrainerTestTask.Data;
using DogTrainerTestTask.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DogTrainerTestTask.Services.Impl;

public class DataSeeder(AppDbContext dbContext, RoleManager<UserRole> roleManager) : IDataSeeder
{
    public void Seed()
    {
        SeedRolesAsync().GetAwaiter().GetResult();
        SeedEntities();
    }

    // RoleManager only exposes asynchronous methods. 
    // Therefore, role creation and existence checks must be performed asynchronously.
    private async Task SeedRolesAsync()
    {
        foreach (var role in Enum.GetNames<UserRoles>())
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new UserRole(role));
            }
        }
    }

    private void SeedEntities()
    {
        if (!dbContext.Users.Any())
        {
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
        }

        if (!dbContext.BreederBenefits.Any())
        {
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
        }

        var random = new Random();

        if (!dbContext.Litters.Any())
        {
            var litters = Enumerable.Range(1, 50)
                .Select(i => new Litter
                {
                    Id = i,
                    BreederId = random.Next(1, 6),
                    Status = (LitterStatus)random.Next(
                        Enum.GetValues<LitterStatus>().Length),
                    CreatedAt = DateTimeOffset.UtcNow.AddDays(-random.Next(0, 365))
                })
                .ToList();

            dbContext.AddRange(litters);
        }

        dbContext.SaveChanges();
    }
}