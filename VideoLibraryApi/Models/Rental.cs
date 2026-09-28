namespace VideoLibraryApi.Models;

public class Rental
{
    public int RentalId { get; set; }

    public int CustomerId { get; set; }
    public Customer? Customer { get; set; }

    public int MovieId { get; set; }
    public Movie? Movie { get; set; }

    public DateTime RentedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ReturnedAt { get; set; }

    public bool IsActive => ReturnedAt == null;
}