using DogTrainerTestTask.Data.Entities;

namespace DogTrainerTestTask.Dto;

public record GetLittersRequestDto(LitterStatus? Status, int PageNumber, int PageSize);