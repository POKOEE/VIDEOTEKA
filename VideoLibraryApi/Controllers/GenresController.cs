using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VideoLibraryApi.Data;
using VideoLibraryApi.Models;

namespace VideoLibraryApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GenresController : ControllerBase
    {
        private readonly VideoLibraryContext _db;
        public GenresController(VideoLibraryContext db) => _db = db;

        // GET /api/genres
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var genres = await _db.Genres.ToListAsync();
            return Ok(genres);
        }

        // POST /api/genres
        [HttpPost]
        public async Task<IActionResult> Create(Genre genre)
        {
            _db.Genres.Add(genre);
            await _db.SaveChangesAsync();
            return Ok(genre);
        }

        // DELETE /api/genres/3
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var g = await _db.Genres.FindAsync(id);
            if (g == null) return NotFound();
            _db.Genres.Remove(g);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}