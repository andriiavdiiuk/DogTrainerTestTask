namespace DogTrainerTestTask.Dto;

public record PaginationResultDto<TItem>(IReadOnlyList<TItem> Item, int PageNumber, int PageSize, int TotalCount);