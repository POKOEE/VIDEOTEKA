using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VideoLibraryApi.Data;
using VideoLibraryApi.Models;

namespace VideoLibraryApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MoviesController : ControllerBase
    {
        private readonly VideoLibraryContext _db;
        public MoviesController(VideoLibraryContext db) => _db = db;

        // GET /api/movies — с фильтрами и сортировкой
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? search,
            [FromQuery] int? genreId,
            [FromQuery] decimal? priceMin,
            [FromQuery] decimal? priceMax,
            [FromQuery] bool? availableOnly,
            [FromQuery] string? sortBy)
        {
            var q = _db.Movies.Include(m => m.Genre).AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                q = q.Where(m => m.Title.Contains(search));

            if (genreId.HasValue)
                q = q.Where(m => m.GenreId == genreId.Value);

            if (priceMin.HasValue)
                q = q.Where(m => m.Price >= priceMin.Value);

            if (priceMax.HasValue)
                q = q.Where(m => m.Price <= priceMax.Value);

            if (availableOnly == true)
                q = q.Where(m => !m.Rentals.Any(r => r.ReturnedAt == null));

            q = sortBy switch
            {
                "year"       => q.OrderByDescending(m => m.Year),
                "price"      => q.OrderBy(m => m.Price),
                "price_desc" => q.OrderByDescending(m => m.Price),
                "title"      => q.OrderBy(m => m.Title),
                _            => q.OrderBy(m => m.Title)
            };

            var result = await q.Select(m => new
            {
                m.MovieId,
                m.Title,
                m.Year,
                m.DurationMinutes,
                m.Price,
                m.PosterUrl,             
                m.Description,            
                m.GenreId,
                GenreName = m.Genre != null ? m.Genre.Name : null,
                IsAvailable = !m.Rentals.Any(r => r.ReturnedAt == null)
            }).ToListAsync();

            return Ok(result);
        }

        // GET /api/movies/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var m = await _db.Movies.Include(x => x.Genre)
                                    .FirstOrDefaultAsync(x => x.MovieId == id);
            return m == null ? NotFound() : Ok(m);
        }

        // POST /api/movies
        [HttpPost]
        public async Task<IActionResult> Create(Movie movie)
        {
            _db.Movies.Add(movie);
            await _db.SaveChangesAsync();
            return Ok(movie);
        }

        // PUT /api/movies/5 — редактирование
        [HttpPut("{id}")]
public async Task<IActionResult> Update(int id, Movie movie)
{
    if (id != movie.MovieId) return BadRequest();

    var existing = await _db.Movies.FindAsync(id);
    if (existing == null) return NotFound();

    existing.Title = movie.Title;
    existing.Year = movie.Year;
    existing.DurationMinutes = movie.DurationMinutes;
    existing.Price = movie.Price;
    existing.GenreId = movie.GenreId;
    existing.PosterUrl = movie.PosterUrl;          // ← ЭТА строка должна быть
    existing.Description = movie.Description;      // ← и эта

    await _db.SaveChangesAsync();
    return Ok(existing);
}

        // DELETE /api/movies/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var m = await _db.Movies.FindAsync(id);
            if (m == null) return NotFound();

            var rentals = _db.Rentals.Where(r => r.MovieId == id);
            _db.Rentals.RemoveRange(rentals);

            _db.Movies.Remove(m);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        // GET /api/movies/top — топ популярных
        [HttpGet("top")]
        public async Task<IActionResult> Top([FromQuery] int count = 5)
        {
            var top = await _db.Movies
                .Select(m => new
                {
                    m.MovieId,
                    m.Title,
                    m.Year,
                    m.Price,
                    m.PosterUrl,
                    GenreName = m.Genre != null ? m.Genre.Name : null,
                    RentalsCount = m.Rentals.Count
                })
                .OrderByDescending(x => x.RentalsCount)
                .Take(count)
                .ToListAsync();

            return Ok(top);
        }

        // GET /api/movies/available — доступные (не выданные)
        [HttpGet("available")]
        public async Task<IActionResult> Available()
        {
            var result = await _db.Movies
                .Where(m => !m.Rentals.Any(r => r.ReturnedAt == null))
                .Select(m => new
                {
                    m.MovieId,
                    m.Title,
                    m.Year,
                    m.Price,
                    m.PosterUrl,
                    GenreName = m.Genre != null ? m.Genre.Name : null
                })
                .ToListAsync();

            return Ok(result);
        }
    }
}