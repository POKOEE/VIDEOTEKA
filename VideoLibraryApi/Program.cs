using Microsoft.EntityFrameworkCore;
using VideoLibraryApi.Data;

var builder = WebApplication.CreateBuilder(args);

// Регистрируем контекст БД (SQLite)
builder.Services.AddDbContext<VideoLibraryContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();