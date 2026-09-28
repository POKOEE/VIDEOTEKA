namespace VideoLibraryApi.Models;

// Фильм
public class Movie
{
    public int MovieId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Year { get; set; }
    public int DurationMinutes { get; set; }
    public decimal Price { get; set; }        

    public int GenreId { get; set; }
    public Genre? Genre { get; set; }

    public List<Rental> Rentals { get; set; } = new();
}