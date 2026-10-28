namespace VideoLibraryApi.Models;

public class Customer
{
    public int CustomerId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }

     public decimal Balance { get; set; } = 0;

    public List<Rental> Rentals { get; set; } = new();
     public List<Transaction> Transactions { get; set; } = new();
}