using System.Net.Http.Json;
using MongoDB.Bson;
using MongoDB.Driver;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class ActiveMissionsBackgroundService : BackgroundService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ActiveMissionsBackgroundService> _logger;
    private readonly IMongoCollection<BsonDocument> _logCollection;
    private readonly IConfiguration _config;

    public ActiveMissionsBackgroundService(
        IHttpClientFactory httpClientFactory,
        ILogger<ActiveMissionsBackgroundService> logger,
        IConfiguration config)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _config = config;

        var mongoConnection = _config["MongoDB:ConnectionString"];

        if (string.IsNullOrWhiteSpace(mongoConnection))
            throw new Exception("MongoDB connection string is missing");

        var mongoClient = new MongoClient(mongoConnection);
        var database = mongoClient.GetDatabase(_config["MongoDB:DatabaseName"] ?? "AarhusSpaceProgramLogDB");

        _logCollection = database.GetCollection<BsonDocument>("ActiveMissionsLog");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var client = _httpClientFactory.CreateClient();
        client.BaseAddress = new Uri(_config["WebApi:BaseUrl"]);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var missions = await client.GetFromJsonAsync<List<MissionResponseDTO>>(
                    "/api/missions?status=Active",
                    stoppingToken
                );

                if (missions == null || missions.Count == 0)
                {
                    _logger.LogInformation("No active missions at {Time}", DateTime.UtcNow);
                    await Task.Delay(TimeSpan.FromSeconds(20), stoppingToken);
                    continue;
                }

                _logger.LogInformation("Fetched {Count} active missions", missions.Count);

                foreach (var mission in missions)
                {
                    if (mission == null)
                        continue;

                    var missionId = Convert.ToInt32(mission.Id); // FIX: ensures int consistency

                    var document = new BsonDocument
                    {
                        { "MissionID", mission.Id },
                        { "MissionName", mission.Name },
                        { "Message", "Telemetry check completed" },
                        { "TimeStamp", DateTime.UtcNow },
                        { "CreatedAt", DateTime.UtcNow }
                    };

                    try
                    {
                        await _logCollection.InsertOneAsync(document, cancellationToken: stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex,
                            "Failed to insert log for MissionID {MissionId}",
                            missionId);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Background service crashed while processing missions");
            }

            await Task.Delay(TimeSpan.FromSeconds(20), stoppingToken);
        }
    }
}