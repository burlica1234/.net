namespace ConsoleApp1;

public static class PatternMatching
{
    public static void ObjectMatching(object obj)
    {
        switch (obj)
        {
            case Book book:
                Console.WriteLine($"[Book] {book.Title} ({book.YearPublished})");
                break;
            case Borrower borrower:
                Console.WriteLine($"[Borrower] {borrower.Name} has {borrower.BorrowedBooks.Count} books");
                break;
            default:
                Console.WriteLine("[Unknown] Type not recognized");
                break;
        }
    }
}