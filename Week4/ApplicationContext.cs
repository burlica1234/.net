using Lab4.Features.Products;
using Microsoft.EntityFrameworkCore;

namespace Lab4;

public class ApplicationContext : DbContext
{
    public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) { }

    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Product>()
            .HasIndex(p => p.SKU)
            .IsUnique();

        b.Entity<Product>()
            .Property(p => p.IsAvailable)
            .HasDefaultValue(false); 
    }
}