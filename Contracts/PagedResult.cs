namespace ConsoleApp2.Contracts;

public record PagedResult<T>(IReadOnlyList<T> Items, int Page, int PageSize, int TotalCount);