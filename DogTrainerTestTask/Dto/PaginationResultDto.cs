namespace DogTrainerTestTask.Dto;

public record PaginationResultDto<TItem>(IReadOnlyList<TItem> Items, int PageNumber, int PageSize, int TotalCount);