using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VideoLibraryApi.Data;
using VideoLibraryApi.Models;

namespace VideoLibraryApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RentalsController : ControllerBase
    {
        private readonly VideoLibraryContext _db;
        public RentalsController(VideoLibraryContext db) => _db = db;

        // GET /api/rentals
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var rentals = await _db.Rentals
                .Include(r => r.Movie)
                .Include(r => r.Customer)
                .OrderByDescending(r => r.RentedAt)
                .Select(r => new
                {
                    r.RentalId,
                    MovieTitle = r.Movie != null ? r.Movie.Title : null,
                    CustomerName = r.Customer != null
                        ? r.Customer.FirstName + " " + r.Customer.LastName
                        : null,
                    r.RentedAt,
                    r.ReturnedAt,
                    IsActive = r.ReturnedAt == null
                })
                .ToListAsync();
            return Ok(rentals);
        }

        // POST /api/rentals — выдать фильм
        [HttpPost]
        public async Task<IActionResult> Rent(Rental rental)
        {
            var movie = await _db.Movies.FindAsync(rental.MovieId);
            var customer = await _db.Customers.FindAsync(rental.CustomerId);

            if (movie == null) return BadRequest("Фильм не найден");
            if (customer == null) return BadRequest("Клиент не найден");

            var alreadyActive = await _db.Rentals.AnyAsync(r =>
                r.MovieId == rental.MovieId && r.ReturnedAt == null);
            if (alreadyActive) return BadRequest("Фильм уже выдан");

            rental.RentedAt = DateTime.UtcNow;
            rental.ReturnedAt = null;

            _db.Rentals.Add(rental);
            await _db.SaveChangesAsync();
            return Ok(rental);
        }

        // POST /api/rentals/5/return — вернуть фильм
        [HttpPost("{id}/return")]
        public async Task<IActionResult> Return(int id)
        {
            var rental = await _db.Rentals.FindAsync(id);
            if (rental == null) return NotFound();
            if (rental.ReturnedAt.HasValue) return BadRequest("Уже возвращено");

            rental.ReturnedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return Ok(rental);
        }

        // GET /api/rentals/overdue — просроченные
[HttpGet("overdue")]
public async Task<IActionResult> Overdue()
{
    var threshold = DateTime.UtcNow.AddDays(-7);

    var overdue = await _db.Rentals
        .Where(r => r.ReturnedAt == null && r.RentedAt < threshold)
        .Include(r => r.Movie)
        .Include(r => r.Customer)
        .Select(r => new
        {
            r.RentalId,
            MovieTitle = r.Movie != null ? r.Movie.Title : null,
            CustomerName = r.Customer != null
                ? r.Customer.FirstName + " " + r.Customer.LastName
                : null,
            r.RentedAt,
            DaysOverdue = (DateTime.UtcNow - r.RentedAt).Days - 7
        })
        .ToListAsync();

    return Ok(overdue);
}
    }
}