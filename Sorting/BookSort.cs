
using System.Linq.Expressions;
using ConsoleApp2.Domain;
namespace ConsoleApp2.Sorting;

public enum SortBy { Title, Year }         
public enum SortDir { Asc, Desc }

public static class BookSort
{
    private static readonly Dictionary<SortBy, Expression<Func<Book, object>>> Map =
        new()
        {
            [SortBy.Title] = b => b.Title,
            [SortBy.Year]  = b => b.Year
        };

    public static IOrderedQueryable<Book> Apply(IQueryable<Book> query, SortBy by, SortDir dir)
    {
        var key = Map[by];
        return dir == SortDir.Asc
            ? query.OrderBy(key)
            : query.OrderByDescending(key);
    }
}