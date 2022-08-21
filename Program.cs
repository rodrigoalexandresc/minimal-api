
using Microsoft.EntityFrameworkCore;

Console.WriteLine("Hello, World!");

var getDrivers1998 = () => new List<string> {
    "Mika Hakkinen", "David Coulthard", "Michael Schumacher", "Jacques Villeneuve"
};

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<F1DbContext>(opt => opt.UseInMemoryDatabase("F1"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseSwagger();
app.UseSwaggerUI();
app.MapGet("/", o => Task.Run(() => o.Response.Redirect("/swagger")));
//app.MapGet("/", () => getDrivers1998());

app.MapGet("/f1/drivers", async (F1DbContext db) => {
    return await db.Drivers.ToListAsync();    
});

app.MapGet("/f1/drivers/{id}", async (int id, F1DbContext db) => 
    await db.FindAsync<Driver>(id));

app.MapPost("/f1/drivers", async (Driver driver, F1DbContext db) => {
    db.Drivers.Add(driver);
    await db.SaveChangesAsync();
    return Results.Created($"/f1/drivers/{driver.Id}", driver);
});

app.Run();

class Driver
{
    public int Id { get; set; }
    public string? Name { get; set; }
}

class F1DbContext : DbContext 
{
    public F1DbContext(DbContextOptions<F1DbContext> options) : base(options)
    {        
    }
    public DbSet<Driver> Drivers { get; set; }
}