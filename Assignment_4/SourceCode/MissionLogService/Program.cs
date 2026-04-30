using MongoDB.Driver;
using MongoDB.Bson;

var builder = Host.CreateApplicationBuilder(args);

// ---------------------------
// HTTP Client (API calls)
// ---------------------------
builder.Services.AddHttpClient();

// ---------------------------
// MongoDB setup (DI - IMPORTANT)
// ---------------------------
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var connectionString = config["MongoDB:ConnectionString"];

    return new MongoClient(connectionString);
});

builder.Services.AddSingleton(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var client = sp.GetRequiredService<IMongoClient>();

    var database = client.GetDatabase("AarhusSpaceProgramLogDB");

    return database.GetCollection<BsonDocument>("ActiveMissionsLog");
});

// ---------------------------
// Background service
// ---------------------------
builder.Services.AddHostedService<ActiveMissionsBackgroundService>();

var host = builder.Build();
host.Run();