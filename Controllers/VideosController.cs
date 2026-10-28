using Microsoft.AspNetCore.Mvc;
using VideoLibraryApi.Models;

namespace VideoLibraryApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VideosController : ControllerBase
    {
        private static List<Video> _videos = new List<Video>
        {
            new Video { Id = 1, Title = "Интерстеллар", Genre = "Фантастика", Year = 2014 },
            new Video { Id = 2, Title = "Матрица", Genre = "Фантастика", Year = 1999 },
            new Video { Id = 3, Title = "Форрест Гамп", Genre = "Драма", Year = 1994 }
        };

        [HttpGet]
        public ActionResult<IEnumerable<Video>> GetAll()
        {
            return Ok(_videos);
        }

        [HttpGet("{id}")]
        public ActionResult<Video> GetById(int id)
        {
            var video = _videos.FirstOrDefault(v => v.Id == id);
            if (video == null) return NotFound();
            return Ok(video);
        }

        [HttpPost]
        public ActionResult<Video> Create(Video video)
        {
            video.Id = _videos.Max(v => v.Id) + 1;
            _videos.Add(video);
            return CreatedAtAction(nameof(GetById), new { id = video.Id }, video);
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            var video = _videos.FirstOrDefault(v => v.Id == id);
            if (video == null) return NotFound();
            _videos.Remove(video);
            return NoContent();
        }
    }
}