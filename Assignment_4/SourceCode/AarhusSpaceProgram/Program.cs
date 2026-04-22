using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
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

// adds background services.
builder.Services.AddHttpClient();
builder.Services.AddHostedService<ActiveMissionsBackgroundService>();

// adds OpenAPI
builder.Services.AddOpenApi();

// adds user services
builder.Services.AddIdentity<ApiUser, IdentityRole>(options => {
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
}).AddEntityFrameworkStores<MainDBContext>();

builder.Services.AddAuthentication(options => {
    options.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options => {
    options.TokenValidationParameters = new TokenValidationParameters {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JWT:Audience"],
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["JWT:SigningKey"]))
    };
});

var app = builder.Build();

using (var scope = app.Services.CreateScope()) {
     var services = scope.ServiceProvider;

    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = services.GetRequiredService<UserManager<ApiUser>>();

    // Creating user roles
    string[] roles = { "Admin", "Astronaut", "Scientist", "Manager" };

    foreach (var role in roles) {
        if (!await roleManager.RoleExistsAsync(role)) {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }


    // Seeding users
    // Manager
    await SeedingData.SeedUser( services,
        new ApiUser {
            Email = "BobSmith@aarhusspaceprogram.com",
            FullName = "Bob Smith"
        },
        "BobSmith1!",
        "Manager"
    );

    // Astronaut
    await SeedingData.SeedUser( services,
        new ApiUser{
            Email = "NeilArmstrong@aarhusspaceprogram.com",
            FullName = "Neil Armstrong"
        },
        "NeilArmstrong1!",
        "Astronaut"
    );

    // Scientist
    await SeedingData.SeedUser(
        services,
        new ApiUser
        {
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

try {
    Log.Information("Starting up application");
    app.Run();
}
catch (Exception ex) {
    Log.Fatal(ex, "Application failed to start");
}
finally {
    Log.CloseAndFlush();
}