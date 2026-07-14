using DogTrainerTestTask.Data;
using DogTrainerTestTask.Data.Entities;
using DogTrainerTestTask.Dto;
using DogTrainerTestTask.Exceptions;
using DogTrainerTestTask.Services;
using DogTrainerTestTask.Services.Impl;
using JetBrains.Annotations;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace DogTrainerTestTask.Tests;

[TestSubject(typeof(LittersService))]
public class LittersServiceTest
{
    private readonly SqliteConnection connection;
    private readonly AppDbContext dbContext;
    private readonly LittersService service;
    private readonly Mock<INotificationService> notificationService;

    public LittersServiceTest()
    {
        connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;

        dbContext = new AppDbContext(options);
        dbContext.Database.EnsureCreated();

        notificationService = new Mock<INotificationService>();

        service = new LittersService(dbContext, notificationService.Object);
    }

    [Fact]
    public async Task PublishLitterAsync_ShouldThrowNotFoundException_WhenLitterDontBelongToUser()
    {
        await dbContext.Users.AddRangeAsync(
            new User
            {
                Id = 1,
                UserName = "breeder1@test.com",
                Email = "breeder1@test.com"
            },
            new User
            {
                Id = 2,
                UserName = "breeder1@test.com",
                Email = "breeder1@test.com"
            });

        await dbContext.Litters.AddAsync(new Litter
        {
            Id = 1,
            BreederId = 2,
            Status = LitterStatus.Submitted,
            CreatedAt = DateTime.UtcNow,
        },CancellationToken.None);

        await dbContext.BreederBenefits.AddAsync(new BreederBenefit
        {
            BreederId = 1,
            UsedCount = 0,
            FreeLimit = 3,
        },CancellationToken.None);

        await dbContext.SaveChangesAsync(CancellationToken.None);

        await Assert.ThrowsAsync<NotFoundException>(() => service.PublishLitterAsync(1, 1));
    }


    [Fact]
    public async Task PublishLitterAsync_ShouldThrowDomainException_WhenAlreadyPublished()
    {
        await dbContext.Users.AddAsync(new User
        {
            Id = 1,
            UserName = "breeder1@test.com",
            Email = "breeder1@test.com"
        },CancellationToken.None);

        await dbContext.Litters.AddAsync(new Litter
        {
            Id = 1,
            BreederId = 1,
            Status = LitterStatus.Published,
            CreatedAt = DateTime.UtcNow,
        }, CancellationToken.None);

        await dbContext.BreederBenefits.AddAsync(new BreederBenefit
        {
            BreederId = 1,
            UsedCount = 0,
            FreeLimit = 3,
        }, CancellationToken.None);

        await dbContext.SaveChangesAsync(CancellationToken.None);

        await Assert.ThrowsAsync<DomainException>(() => service.PublishLitterAsync(1, 1));
    }

    [Fact]
    public async Task PublishLitterAsync_ShouldThrowDomainException_WhenLitterIsNotApproved()
    {
        await dbContext.Users.AddAsync(new User
        {
            Id = 1,
            UserName = "breeder1@test.com",
            Email = "breeder1@test.com"
        }, CancellationToken.None);

        await dbContext.Litters.AddAsync(new Litter
        {
            Id = 1,
            BreederId = 1,
            Status = LitterStatus.Submitted,
            CreatedAt = DateTime.UtcNow,
        }, CancellationToken.None);

        await dbContext.BreederBenefits.AddAsync(new BreederBenefit
        {
            BreederId = 1,
            UsedCount = 0,
            FreeLimit = 3,
        }, CancellationToken.None);

        await dbContext.SaveChangesAsync(CancellationToken.None);

        await Assert.ThrowsAsync<DomainException>(() => service.PublishLitterAsync(1, 1));
    }

    [Fact]
    public async Task PublishLitterAsync_ShouldThrowDomainExceptionAndLog_WhenFreeLimitExceeded()
    {
        await dbContext.Users.AddAsync(new User
        {
            Id = 1,
            UserName = "breeder1@test.com",
            Email = "breeder1@test.com"
        }, CancellationToken.None);

        await dbContext.Litters.AddAsync(new Litter
        {
            Id = 1,
            BreederId = 1,
            Status = LitterStatus.Approved,
            CreatedAt = DateTime.UtcNow,
        }, CancellationToken.None);

        await dbContext.BreederBenefits.AddAsync(new BreederBenefit
        {
            BreederId = 1,
            UsedCount = 3,
            FreeLimit = 3,
        }, CancellationToken.None);

        await dbContext.SaveChangesAsync(CancellationToken.None);

        await Assert.ThrowsAsync<DomainException>(() => service.PublishLitterAsync(1, 1));

        var auditLog = await dbContext.AuditLogs.FirstOrDefaultAsync(CancellationToken.None);

        Assert.NotNull(auditLog);
    }

    [Fact]
    public async Task PublishLitterAsync_ShouldPublishAndSendEmailAndLog_WhenApproved()
    {
        await dbContext.Users.AddAsync(new User
        {
            Id = 1,
            UserName = "breeder1@test.com",
            Email = "breeder1@test.com"
        }, CancellationToken.None);

        await dbContext.Litters.AddAsync(new Litter
        {
            Id = 1,
            BreederId = 1,
            Status = LitterStatus.Approved,
            CreatedAt = DateTime.UtcNow,
        },CancellationToken.None );

        await dbContext.BreederBenefits.AddAsync(new BreederBenefit
        {
            BreederId = 1,
            UsedCount = 0,
            FreeLimit = 3
        }, CancellationToken.None);

        await dbContext.SaveChangesAsync(CancellationToken.None);

        await service.PublishLitterAsync(1, 1);

        var litter = await dbContext.Litters.FirstOrDefaultAsync(x => x.Id == 1, CancellationToken.None);

        Assert.Equal(LitterStatus.Published, litter!.Status);

        notificationService.Verify(x => x.Notify(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task GetLittersByBreederIdAsync_ShouldReturnLitters_WhenBreederHasLitters()
    {
        await dbContext.Users.AddAsync(new User
        {
            Id = 1,
            UserName = "breeder1@test.com",
            Email = "breeder1@test.com"
        }, CancellationToken.None);

        await dbContext.Users.AddAsync(new User
        {
            Id = 2,
            UserName = "breeder1@test.com",
            Email = "breeder1@test.com"
        }, CancellationToken.None);

        await dbContext.Litters.AddRangeAsync(
            new Litter
            {
                Id = 1,
                BreederId = 1,
                Status = LitterStatus.Approved,
                CreatedAt = DateTime.UtcNow,
            },
            new Litter
            {
                Id = 2,
                BreederId = 2,
                Status = LitterStatus.Approved,
                CreatedAt = DateTime.UtcNow,
            });

        await dbContext.SaveChangesAsync(CancellationToken.None);

        var requestDto = new GetLittersRequestDto(null, 1, 10);

        var result = await service.GetLittersByBreederIdAsync(1, requestDto);

        Assert.Single(result.Items);
        Assert.Equal(1, result.Items[0].Id);
        Assert.Equal(1, result.TotalCount);
    }

    [Fact]
    public async Task GetLittersByBreederIdAsync_ShouldReturnLitters_WhenStatusProvided()
    {
        await dbContext.Users.AddAsync(new User
        {
            Id = 1,
            UserName = "breeder1@test.com",
            Email = "breeder1@test.com"
        }, CancellationToken.None);

        await dbContext.Litters.AddRangeAsync(
            new Litter
            {
                Id = 1,
                BreederId = 1,
                Status = LitterStatus.Approved,
                CreatedAt = DateTime.UtcNow,
            },
            new Litter
            {
                Id = 2,
                BreederId = 1,
                Status = LitterStatus.Published,
                CreatedAt = DateTime.UtcNow,
            });

        await dbContext.SaveChangesAsync(CancellationToken.None);

        var requestDto = new GetLittersRequestDto(LitterStatus.Published, 1, 10);

        var result = await service.GetLittersByBreederIdAsync(1, requestDto);

        Assert.Single(result.Items);
        Assert.Equal(2, result.Items[0].Id);
    }

    [Fact]
    public async Task GetLittersByBreederIdAsync_ShouldPaginateResults()
    {
        await dbContext.Users.AddAsync(new User
        {
            Id = 1,
            UserName = "breeder1@test.com",
            Email = "breeder1@test.com"
        }, CancellationToken.None);

        await dbContext.Litters.AddRangeAsync(
            new Litter
            {
                Id = 1,
                BreederId = 1,
                Status = LitterStatus.Approved,
                CreatedAt = DateTime.UtcNow.AddDays(-1)
            },
            new Litter
            {
                Id = 2,
                BreederId = 1,
                Status = LitterStatus.Approved,
                CreatedAt = DateTime.UtcNow.AddDays(-2),
            },
            new Litter
            {
                Id = 3,
                BreederId = 1,
                Status = LitterStatus.Approved,
                CreatedAt = DateTime.UtcNow.AddDays(-3),
            });

        await dbContext.SaveChangesAsync(CancellationToken.None);

        var requestDto = new GetLittersRequestDto(LitterStatus.Approved, 1, 1);


        var result = await service.GetLittersByBreederIdAsync(1, requestDto);

        Assert.Single(result.Items);
        Assert.Equal(1, result.Items[0].Id);
        Assert.Equal(3, result.TotalCount);
    }
}