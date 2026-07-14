using DogTrainerTestTask.Data.Entities;

namespace DogTrainerTestTask.Dto;

public record LitterDto(long Id, LitterStatus Status, DateTimeOffset CreatedAt);