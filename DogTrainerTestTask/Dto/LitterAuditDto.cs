using DogTrainerTestTask.Data.Entities;

namespace DogTrainerTestTask.Dto;

public record LitterAuditDto(long Id, long BreederId, LitterStatus Status, DateTimeOffset CreatedAt);