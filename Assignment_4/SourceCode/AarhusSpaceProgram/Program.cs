using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Text;
using Scalar.AspNetCore;
using System.IO.Pipelines;
using MongoDB.Driver;
using MongoDB.Bson;

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

// DB Context
builder.Services.AddDbContext<MainDBContext>(options => {
    options.UseSqlServer(
            builder.Configuration.GetConnectionString("DefaultConnection"),
            sql => sql.EnableRetryOnFailure()
        )
        .EnableSensitiveDataLogging()
        .LogTo(Log.Information);
});

// Controllers
builder.Services.AddControllers();

builder.Services.AddOpenApi();



// Identity
builder.Services.AddIdentity<ApiUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
})
.AddEntityFrameworkStores<MainDBContext>();

// JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options => {
    var key = builder.Configuration["JWT:SigningKey"];

    if (string.IsNullOrEmpty(key))
        throw new Exception("JWT:SigningKey is missing in appsettings.json");

    
        options.TokenValidationParameters = new TokenValidationParameters {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["JWT:Issuer"],

            ValidateAudience = true,
            ValidAudience = builder.Configuration["JWT:Audience"],

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(key)
            ),

            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
});



builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    return new MongoClient(config["MongoDB:ConnectionString"]);
});

builder.Services.AddSingleton(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    var db = client.GetDatabase("AarhusSpaceProgramLogDB");
    return db.GetCollection<BsonDocument>("ActiveMissionsLog");
});

var app = builder.Build();


// ----------------------
// Seed data
// ----------------------
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var dbContext = services.GetRequiredService<MainDBContext>();
    dbContext.Database.MigrateAsync();

    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    var retry = 0;
    while (retry < 10)
    {
        try
        {
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            break;
        }
        catch (Exception ex)
        {
            retry++;
            Console.WriteLine($"DB not ready yet, retry {retry}/10: {ex.Message}");
            await Task.Delay(3000);
        }
    }

    string[] roles = { "Admin", "Astronaut", "Scientist", "Manager" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    await SeedingData.SeedUser(services,
        new ApiUser
        {
            UserName = "BobSmith@aarhusspaceprogram.com",
            Email = "BobSmith@aarhusspaceprogram.com",
            FullName = "Bob Smith"
        },
        "BobSmith1!",
        "Manager"
    );

    await SeedingData.SeedUser(services,
        new ApiUser
        {
            UserName = "NeilArmstrong@aarhusspaceprogram.com",
            Email = "NeilArmstrong@aarhusspaceprogram.com",
            FullName = "Neil Armstrong"
        },
        "NeilArmstrong1!",
        "Astronaut"
    );

    await SeedingData.SeedUser(services,
        new ApiUser
        {
            UserName = "CarlSagan@aarhusspaceprogram.com",
            Email = "CarlSagan@aarhusspaceprogram.com",
            FullName = "Carl Sagan"
        },
        "CarlSagan1!",
        "Scientist"
    );
}


// ----------------------
// Middleware
// ----------------------
if (app.Environment.IsDevelopment())
{
    // OpenAPI JSON endpoint
    app.MapOpenApi();

    // Scalar UI
    app.MapScalarApiReference(options =>
    {
        options.Title = "Aarhus Space Program API";
    });

    app.UseDeveloperExceptionPage();
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


// ----------------------
// Run
// ----------------------
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