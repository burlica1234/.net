using System;
using System.Collections.Generic;
using System.Linq;            
using ConsoleApp1;

namespace ConsoleApp1
{
    class Program
    {
        static void Main()
        {
            var books = new List<Book>();
            var librarian = new Librarian("Burlica", "alexburlica898@gmail.com", "Fiction");

            Console.WriteLine("Enter book details (leave Title empty to finish):");
            while (true)
            {
                Console.Write("Book Title: ");
                string? title = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(title)) break;   

                Console.Write("Author: ");
                string? author = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(author)) author = "Unknown";

                Console.Write("Year Published: ");
                string? yearPublished = Console.ReadLine();
                int year;
                // verif daca s a introdus un an valid
                if (!int.TryParse(yearPublished, out year))
                {
                    year = 2000;
                }

                books.Add(new Book(title.Trim(), author.Trim(), year));
                Console.WriteLine("✔ Book added.\n");
            }

            Console.WriteLine("\nAll Entered Books:");
            foreach (var b in books)
                Console.WriteLine($"- {b.Title} by {b.Author} ({b.YearPublished})");

            // filtrare carti dupa 2010
            BookFilter.ShowBooksAfter2010(books);

            // clonare cu with
            if (books.Count >= 2)
            {
                var borrower = new Borrower(1, "Matei Manea", new List<Book> { books[0] });

                var updatedBorrower = borrower with
                {
                    BorrowedBooks = borrower.BorrowedBooks.Append(books[1]).ToList()
                };

                Console.WriteLine($"\nBorrower: {updatedBorrower.Name}");
                Console.WriteLine("Borrowed Books:");
                foreach (var book in updatedBorrower.BorrowedBooks)
                    Console.WriteLine($"- {book.Title} ({book.YearPublished})");

                // Pattern matching
                PatternMatching.ObjectMatching(updatedBorrower);
                PatternMatching.ObjectMatching(books[1]);
                PatternMatching.ObjectMatching(42);
            }
            else
            {
                Console.WriteLine("\nAdd at least 2 books to demonstrate borrower cloning.");
            }

            Console.WriteLine($"\nLibrarian: {librarian.Name} ({librarian.LibrarySection})");
        }
    }
}
