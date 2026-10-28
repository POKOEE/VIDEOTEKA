using Microsoft.EntityFrameworkCore;
using VideoLibraryApi.Models;

namespace VideoLibraryApi.Data;

public class VideoLibraryContext : DbContext
{
    public VideoLibraryContext(DbContextOptions<VideoLibraryContext> options)
        : base(options) { }

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Genre> Genres => Set<Genre>();
    public DbSet<Movie> Movies => Set<Movie>();
    public DbSet<Rental> Rentals => Set<Rental>();
    public DbSet<Transaction> Transactions => Set<Transaction>();  // ← проверьте, что это есть

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ==== Связи ====
        modelBuilder.Entity<Movie>()
            .HasOne(m => m.Genre)
            .WithMany(g => g.Movies)
            .HasForeignKey(m => m.GenreId);

        modelBuilder.Entity<Rental>()
            .HasOne(r => r.Customer)
            .WithMany(c => c.Rentals)
            .HasForeignKey(r => r.CustomerId);

        modelBuilder.Entity<Rental>()
            .HasOne(r => r.Movie)
            .WithMany(m => m.Rentals)
            .HasForeignKey(r => r.MovieId);

        // ==== Точность ====
        modelBuilder.Entity<Movie>()
            .Property(m => m.Price)
            .HasPrecision(10, 2);

        modelBuilder.Entity<Customer>()
            .Property(c => c.Balance)
            .HasPrecision(10, 2);

        modelBuilder.Entity<Transaction>()
            .Property(t => t.Amount)
            .HasPrecision(10, 2);

        modelBuilder.Entity<Rental>()
            .Property(r => r.PaidAmount)
            .HasPrecision(10, 2);
    }
}