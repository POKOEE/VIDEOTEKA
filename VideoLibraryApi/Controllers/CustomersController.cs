using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VideoLibraryApi.Data;
using VideoLibraryApi.Models;

namespace VideoLibraryApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly VideoLibraryContext _db;
        public CustomersController(VideoLibraryContext db) => _db = db;

        // GET /api/customers
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var customers = await _db.Customers
                .Select(c => new
                {
                    c.CustomerId,
                    c.FirstName,
                    c.LastName,
                    FullName = c.FirstName + " " + c.LastName,
                    c.Email,
                    c.Phone
                })
                .ToListAsync();
            return Ok(customers);
        }

        // POST /api/customers
        [HttpPost]
        public async Task<IActionResult> Create(Customer customer)
        {
            _db.Customers.Add(customer);
            await _db.SaveChangesAsync();
            return Ok(customer);
        }

        // DELETE /api/customers/2
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var c = await _db.Customers.FindAsync(id);
            if (c == null) return NotFound();
            _db.Customers.Remove(c);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        // GET /api/customers/5/history
[HttpGet("{id}/history")]
public async Task<IActionResult> GetHistory(int id)
{
    var history = await _db.Rentals
        .Where(r => r.CustomerId == id)
        .Include(r => r.Movie)
        .OrderByDescending(r => r.RentedAt)
        .Select(r => new
        {
            r.RentalId,
            MovieTitle = r.Movie != null ? r.Movie.Title : null,
            r.RentedAt,
            r.ReturnedAt,
            IsActive = r.ReturnedAt == null
        })
        .ToListAsync();

    return Ok(history);
}
    }
}