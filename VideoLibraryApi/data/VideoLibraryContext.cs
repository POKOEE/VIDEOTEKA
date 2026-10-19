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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
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

        modelBuilder.Entity<Movie>()
            .Property(m => m.Price)
            .HasPrecision(10, 2);
    }
}