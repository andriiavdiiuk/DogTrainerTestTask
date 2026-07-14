using System.Text.Json;
using DogTrainerTestTask.Data;
using DogTrainerTestTask.Data.Entities;
using DogTrainerTestTask.Dto;
using DogTrainerTestTask.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace DogTrainerTestTask.Services.Impl;

public class LittersService(AppDbContext dbContext, INotificationService notificationService) : ILittersService
{
    public async Task PublishLitterAsync(long breederId, long litterId)
    {
        var litter = dbContext.Litters.FirstOrDefault(x => x.BreederId == breederId && x.Id == litterId);
        if (litter is null)
        {
            throw new NotFoundException($"Litter {litterId} not found");
        }

        if (litter.Status == LitterStatus.Published)
        {
            throw new DomainException($"Litter {litterId} is already published");   
        }
        
        if (litter.Status != LitterStatus.Approved)
        {
            throw new DomainException($"Litter {litterId} not approved");   
        }
        
        var breederBenefits = dbContext.BreederBenefits.FirstOrDefault(x => x.BreederId == breederId);
        
        // Breeder benefits must always exist for every user.
        // A missing record indicates an invalid state.
        if (breederBenefits is null)
        {
            throw new InvalidOperationException($"Breeder benefits do not exists for user {breederId}");
        }
        
        bool exceedLimit = breederBenefits.UsedCount + 1 > breederBenefits.FreeLimit;
        if (exceedLimit)
        {
            dbContext.AuditLogs.Add(new AuditLog()
            { 
                Action = "Publish attempt failed - limits exceeded", 
                EntityId = litterId,
                EntityName = litter.GetType().Name,
                ModifiedBy = breederId,
                CreatedAt =  DateTime.UtcNow,
            });
            await dbContext.SaveChangesAsync();
            throw new DomainException($"Free publication limit exceeded");
        }

        await using var transaction = await dbContext.Database.BeginTransactionAsync();
        
        var oldLitter = JsonSerializer.Serialize(litter.ToLitterAuditDto());
        
        breederBenefits.UsedCount++;
        litter.Status = LitterStatus.Published;
        
        dbContext.AuditLogs.Add(new AuditLog()
        { 
            Action = "Published for Free", 
            EntityId = litterId,
            EntityName = litter.GetType().Name,
            ModifiedBy = breederId,
            CreatedAt =  DateTime.UtcNow,
            OldValues = oldLitter,
            NewValues = JsonSerializer.Serialize(litter.ToLitterAuditDto())
        });
        notificationService.Notify("Published litter for free");
        await dbContext.SaveChangesAsync();
        await transaction.CommitAsync();
    }

    public async Task<PaginationResultDto<LitterDto>> GetLittersByBreederIdAsync(
        long breederId, 
        GetLittersRequestDto getLittersRequestDto)
    {
        var query = dbContext.Litters.Where(x => x.BreederId == breederId);

        if (getLittersRequestDto.Status is not null)
        {
            query = query.Where(x => x.Status == getLittersRequestDto.Status);
        }
        
        var skipItems = (getLittersRequestDto.PageNumber - 1) * getLittersRequestDto.PageSize;

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderByDescending(x => x.Id)
            .Skip(skipItems)
            .Take(getLittersRequestDto.PageSize)
            .Select(x => x.ToLitterDto())
            .ToListAsync();
        
        return new PaginationResultDto<LitterDto>(items, getLittersRequestDto.PageNumber, getLittersRequestDto.PageSize, totalCount);
    }
}