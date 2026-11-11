
using Microsoft.EntityFrameworkCore;
using VideoLibraryApi.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. Подключение к базе данных
builder.Services.AddDbContext<VideoLibraryContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 2. Настройка портов для работы в Docker (Nginx будет обращаться к 8080)
builder.WebHost.UseUrls("http://0.0.0.0:8080");

var app = builder.Build();

// 3. АВТОМАТИЧЕСКОЕ ПРИМЕНЕНИЕ МИГРАЦИЙ ПРИ СТАРТЕ
// Это критически важно, так как мы исключили файл .db из Docker-образа.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<VideoLibraryContext>();
    // Создаст базу данных, если её нет, и применит все миграции
    db.Database.Migrate(); 
}

// 4. Swagger (Документация)
app.UseSwagger();
app.UseSwaggerUI();
app.UseReDoc(c =>
{
    c.RoutePrefix = "redoc";
    c.SpecUrl = "/swagger/v1/swagger.json";
    c.DocumentTitle = "VideoLibraryApi";
});

// 5. Статические файлы и sitemap
app.UseDefaultFiles();
app.UseStaticFiles();

// 6. SEO Заголовки для API
app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/api"))
    {
        context.Response.Headers.Append("X-Robots-Tag", "noindex, nofollow");
    }
    await next(); 
});

app.UseAuthorization();
app.MapControllers();

app.MapGet("/sitemap.xml", async (HttpContext context) =>
{
    var filePath = Path.Combine(app.Environment.WebRootPath, "sitemap.xml");
    
    Console.WriteLine($"=== ПОПЫТКА ОТКРЫТЬ SITEMAP ===");
    Console.WriteLine($"Ищу файл по пути: {filePath}");
    Console.WriteLine($"Файл существует: {System.IO.File.Exists(filePath)}");
    
    if (System.IO.File.Exists(filePath))
    {
        context.Response.ContentType = "application/xml";
        await context.Response.SendFileAsync(filePath);
    }
    else
    {
        var dirPath = app.Environment.WebRootPath;
        if (Directory.Exists(dirPath))
        {
            var files = Directory.GetFiles(dirPath);
            Console.WriteLine($"Файлы в папке {dirPath}:");
            foreach (var f in files) Console.WriteLine($" - {f}");
        }
        else
        {
            Console.WriteLine($"Папка {dirPath} НЕ СУЩЕСТВУЕТ!");
        }

        context.Response.StatusCode = 404;
        await context.Response.WriteAsync("Sitemap not found on server");
    }
});

app.Run();