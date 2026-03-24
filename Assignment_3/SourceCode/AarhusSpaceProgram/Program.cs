using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// ----------------------
// Serilog configuration
// ----------------------
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithThreadId()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// ----------------------
// Services
// ----------------------

// context connectionstring
builder.Services.AddDbContext<MainDBContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"))
        .EnableSensitiveDataLogging()   // optional (dev only!)
        .LogTo(Log.Information)         // logs EF Core queries via Serilog
);

// Controllers
builder.Services.AddControllers();

// adds OpenAPI
builder.Services.AddOpenApi();

var app = builder.Build();

// ----------------------
// Middleware
// ----------------------

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.MapScalarApiReference(options =>
    {
        options.Title = "Space Program API";
    });

    app.UseDeveloperExceptionPage();
}

// Add Serilog request logging middleware
app.UseSerilogRequestLogging();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

try
{
    Log.Information("Starting up application");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application failed to start");
}
finally
{
    Log.CloseAndFlush();
}