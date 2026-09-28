namespace VideoLibraryApi.Models;

public class Genre
{
    public int GenreId { get; set; }
    public string Name { get; set; } = string.Empty;

    public List<Movie> Movies { get; set; } = new();
}