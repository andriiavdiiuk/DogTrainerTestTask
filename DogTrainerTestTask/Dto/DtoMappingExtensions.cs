using DogTrainerTestTask.Data.Entities;
using Riok.Mapperly.Abstractions;

namespace DogTrainerTestTask.Dto;


[Mapper(AutoUserMappings = false, RequiredMappingStrategy = RequiredMappingStrategy.None)]
public static partial class DtoMappingExtensions
{
    [MapProperty(nameof(Litter.Id), nameof(LitterDto.Id))]
    [MapProperty(nameof(Litter.Status), nameof(LitterDto.Status))]
    [MapProperty(nameof(Litter.CreatedAt), nameof(LitterDto.CreatedAt))]
    public static partial LitterDto ToLitterDto(this Litter source);
    
    [MapProperty(nameof(Litter.Id), nameof(LitterAuditDto.Id))]
    [MapProperty(nameof(Litter.BreederId), nameof(LitterAuditDto.BreederId))]
    [MapProperty(nameof(Litter.Status), nameof(LitterAuditDto.Status))]
    [MapProperty(nameof(Litter.CreatedAt), nameof(LitterAuditDto.CreatedAt))]
    public static partial LitterAuditDto ToLitterAuditDto(this Litter source);
}