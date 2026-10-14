// PR test
using Microsoft.EntityFrameworkCore;
using VideoLibraryApi.Data;

var builder = WebApplication.CreateBuilder(args);

// Регистрируем контекст БД
builder.Services.AddDbContext<VideoLibraryContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Swagger в любом окружении — чтобы работал в Docker
app.UseSwagger();
app.UseSwaggerUI();

app.UseReDoc(c =>
{
    c.RoutePrefix = "redoc";
    c.SpecUrl = "/swagger/v1/swagger.json";
    c.DocumentTitle = "VideoLibraryApi";
});

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseAuthorization();
app.MapControllers();

app.Run();