using ConsoleApp2.Domain;
using Microsoft.EntityFrameworkCore;

namespace ConsoleApp2.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Book> Books => Set<Book>();
}