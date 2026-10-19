using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VideoLibraryApi.Data;
using VideoLibraryApi.Models;

namespace VideoLibraryApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly VideoLibraryContext _db;
        public CartController(VideoLibraryContext db) => _db = db;

        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout([FromBody] CheckoutRequest request)
        {
            var customer = await _db.Customers.FindAsync(request.CustomerId);
            if (customer == null) return BadRequest("Клиент не найден");
            if (request.MovieIds == null || request.MovieIds.Count == 0)
                return BadRequest("Корзина пуста");

            var movies = await _db.Movies
                .Where(m => request.MovieIds.Contains(m.MovieId))
                .ToListAsync();
            if (movies.Count != request.MovieIds.Count)
                return BadRequest("Некоторые фильмы не найдены");

            var busy = await _db.Rentals
                .Where(r => r.ReturnedAt == null && request.MovieIds.Contains(r.MovieId))
                .Select(r => r.MovieId)
                .ToListAsync();
            if (busy.Any())
                return BadRequest($"Фильмы недоступны: {string.Join(", ", busy)}");

            decimal total = movies.Sum(m => m.Price);

            if (request.PaymentMethod == PaymentMethod.Balance && customer.Balance < total)
                return BadRequest($"Недостаточно средств. Нужно {total} ₽, у вас {customer.Balance} ₽");

            if (request.PaymentMethod == PaymentMethod.Balance)
            {
                customer.Balance -= total;
                _db.Transactions.Add(new Transaction
                {
                    CustomerId = customer.CustomerId,
                    Amount = -total,
                    Type = TransactionType.Payment,
                    Method = PaymentMethod.Balance,
                    Description = $"Оплата аренды: {movies.Count} фильм(ов)"
                });
            }

            foreach (var movie in movies)
            {
                _db.Rentals.Add(new Rental
                {
                    CustomerId = customer.CustomerId,
                    MovieId = movie.MovieId,
                    RentedAt = DateTime.UtcNow,
                    PaymentMethod = request.PaymentMethod,
                    PaidAmount = movie.Price
                });
            }

            await _db.SaveChangesAsync();

            return Ok(new
            {
                message = "Аренда оформлена",
                total,
                balance = customer.Balance
            });
        }
    }

    public class CheckoutRequest
    {
        public int CustomerId { get; set; }
        public List<int> MovieIds { get; set; } = new();
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Balance;
    }
}