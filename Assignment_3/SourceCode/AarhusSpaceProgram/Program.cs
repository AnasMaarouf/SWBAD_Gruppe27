
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// context connectionstring
builder.Services.AddDbContext<MainDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
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
    // OpenAPI document
    app.MapOpenApi();

    // Scalar UI
    app.MapScalarApiReference(options =>
    {
        options.Title = "Space Program API";
    });

    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
