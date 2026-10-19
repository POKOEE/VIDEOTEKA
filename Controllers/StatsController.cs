using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VideoLibraryApi.Data;

namespace VideoLibraryApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatsController : ControllerBase
    {
        private readonly VideoLibraryContext _db;
        public StatsController(VideoLibraryContext db) => _db = db;

        // GET /api/stats
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(new
            {
                Movies = await _db.Movies.CountAsync(),
                Genres = await _db.Genres.CountAsync(),
                Customers = await _db.Customers.CountAsync(),
                RentalsTotal = await _db.Rentals.CountAsync(),
                RentalsActive = await _db.Rentals.CountAsync(r => r.ReturnedAt == null)
            });
        }
    }
}