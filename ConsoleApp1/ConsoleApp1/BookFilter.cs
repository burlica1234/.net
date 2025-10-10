namespace ConsoleApp1;

public static class BookFilter
{
    public static void ShowBooksAfter2010(List<Book> books)
    {
        var booksAfter2010 = books.Where(b => b.YearPublished > 2010).ToList();
        foreach (var book in booksAfter2010)
            Console.WriteLine($"{book.Title} ({book.YearPublished})");
    }
}