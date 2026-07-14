using DogTrainerTestTask.Data;
using DogTrainerTestTask.Data.Entities;
using DogTrainerTestTask.Dto;
using Microsoft.EntityFrameworkCore;

namespace DogTrainerTestTask.Services.Impl;

public class LittersService(AppDbContext dbContext) : ILittersService
{
    public Task PublishLitterAsync(long litterId)
    {
        throw new NotImplementedException();
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

        var totalCount = await dbContext.Litters.CountAsync();
        var items = await query
            .OrderByDescending(x => x.Id)
            .Skip(skipItems)
            .Take(getLittersRequestDto.PageSize)
            .Select(x => x.ToLitterDto())
            .ToListAsync();
        
        return new PaginationResultDto<LitterDto>(items, getLittersRequestDto.PageNumber, getLittersRequestDto.PageSize, totalCount);
    }
}