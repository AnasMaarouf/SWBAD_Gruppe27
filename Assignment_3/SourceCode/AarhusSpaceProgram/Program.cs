
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, config) =>
{
    config.ReadFrom.Configuration(context.Configuration);
});

// context connectionstring
builder.Services.AddDbContext<MainDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Controllers

builder.Services.AddControllers();

builder.Services.AddOpenApi(options =>
{
    options.AddSchemaTransformer((schema, context, cancellationToken) =>
    {
        if (schema?.Extensions != null)
            schema.Extensions.Clear();
        return Task.CompletedTask;
    });
});

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
