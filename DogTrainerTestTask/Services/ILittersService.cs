using DogTrainerTestTask.Data.Entities;
using DogTrainerTestTask.Dto;

namespace DogTrainerTestTask.Services;

public interface ILittersService
{
    Task PublishLitterAsync(long breederId, long litterId);
    
    Task<PaginationResultDto<LitterDto>> GetLittersByBreederIdAsync(long breederId, GetLittersRequestDto getLittersRequestDto);
}