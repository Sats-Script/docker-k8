using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Logging
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .CreateLogger();
builder.Host.UseSerilog();

// Configuration
// var pgHost = Environment.GetEnvironmentVariable("PGHOST") ?? "postgres";
// var pgPort = Environment.GetEnvironmentVariable("PGPORT") ?? "5432";
// var pgUser = Environment.GetEnvironmentVariable("PGUSER") ?? "devuser";
// var pgPwd  = Environment.GetEnvironmentVariable("PGPASSWORD") ?? "secret123";
// var pgDb   = Environment.GetEnvironmentVariable("PGDATABASE") ?? "devdb";
// var connString = $"Host={pgHost};Port={pgPort};Username={pgUser};Password={pgPwd};Database={pgDb}";
var pgHost = Environment.GetEnvironmentVariable("PGHOST");
var pgPort = Environment.GetEnvironmentVariable("PGPORT");
var pgUser = Environment.GetEnvironmentVariable("PGUSER");
var pgPwd  = Environment.GetEnvironmentVariable("PGPASSWORD");
var pgDb   = Environment.GetEnvironmentVariable("PGDATABASE");

// Optional: throw exception if any variable is missing
if (string.IsNullOrEmpty(pgHost) ||
    string.IsNullOrEmpty(pgPort) ||
    string.IsNullOrEmpty(pgUser) ||
    string.IsNullOrEmpty(pgPwd) ||
    string.IsNullOrEmpty(pgDb))
{
    throw new Exception("One or more required DB environment variables are not set!");
}

var connString = $"Host={pgHost};Port={pgPort};Username={pgUser};Password={pgPwd};Database={pgDb}";


builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseNpgsql(connString));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "DevOpsSample API", Version = "v1" });
});

var app = builder.Build();

// Auto-migrate
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/healthz", () => Results.Ok(new { status = "ok" }));
app.MapGet("/api/products", async (AppDbContext db) => await db.Products.AsNoTracking().ToListAsync());
app.MapPost("/api/products", async (AppDbContext db, Product p) =>
{
    db.Products.Add(p);
    await db.SaveChangesAsync();
    return Results.Created($"/api/products/{p.Id}", p);
});
app.MapPut("/api/products/{id:int}", async (AppDbContext db, int id, Product input) =>
{
    var p = await db.Products.FindAsync(id);
    if (p is null) return Results.NotFound();
    p.Name = input.Name;
    p.Price = input.Price;
    p.InStock = input.InStock;
    await db.SaveChangesAsync();
    return Results.NoContent();
});
app.MapDelete("/api/products/{id:int}", async (AppDbContext db, int id) =>
{
    var p = await db.Products.FindAsync(id);
    if (p is null) return Results.NotFound();
    db.Products.Remove(p);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();

// Models & DbContext
public record Product
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public decimal Price { get; set; }
    public bool InStock { get; set; } = true;
}

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Product> Products => Set<Product>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>().Property(p => p.Price).HasColumnType("numeric(10,2)");
        base.OnModelCreating(modelBuilder);
    }
}
