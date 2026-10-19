
using Microsoft.EntityFrameworkCore;
using VideoLibraryApi.Data;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<VideoLibraryContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();


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
    // Получаем путь, который видит сервер
    var filePath = Path.Combine(app.Environment.WebRootPath, "sitemap.xml");
    
    // Выводим этот путь в терминал VS Code
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
        // Если файла нет, выводим в терминал, какие файлы реально есть в папке
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