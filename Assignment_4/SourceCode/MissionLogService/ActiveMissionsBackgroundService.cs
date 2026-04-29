using System.Net.Http.Json;
using MongoDB.Bson;
using MongoDB.Driver;

public class ActiveMissionsBackgroundService : BackgroundService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ActiveMissionsBackgroundService> _logger;
    private readonly IMongoCollection<BsonDocument> _logCollection;

    public ActiveMissionsBackgroundService( IHttpClientFactory httpClientFactory, ILogger<ActiveMissionsBackgroundService> logger) {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        var MongoDB_client = new MongoClient("mongodb://localhost:27017");
        var database = MongoDB_client.GetDatabase("AarhusSpaceProgramLogDB");
        _logCollection = database.GetCollection<BsonDocument>("ActiveMissionsLog");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {

        var AarhusSpaceProgramDB_client = _httpClientFactory.CreateClient();
        AarhusSpaceProgramDB_client.BaseAddress = new Uri("http://localhost:5062");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var missions = await AarhusSpaceProgramDB_client.GetFromJsonAsync<List<MissionResponseDTO>>(
                    "/api/missions?status=Active",
                    stoppingToken
                );

                if (missions == null || missions.Count == 0)
                {
                    _logger.LogWarning("No active missions found at {Time}", DateTime.UtcNow);
                    await Task.Delay(TimeSpan.FromSeconds(20), stoppingToken);
                    continue;
                }

                _logger.LogInformation("Fetched {Count} missions at {Time}",
                    missions.Count, DateTime.UtcNow);

                foreach (var mission in missions) {
                    var document = new BsonDocument
                    {
                        { "MissionID", mission.Id },
                        { "MissionName", mission.Name },
                        { "Message", "Telemetry check completed" },
                        { "TimeStamp", DateTime.UtcNow },
                        { "CreatedAt", DateTime.UtcNow }
                    };

                    try {
                        await _logCollection.InsertOneAsync(document, cancellationToken: stoppingToken);
                    } catch {
                        _logger.LogWarning(
                            "Failed to create log for mission {MissionId}. Status: {StatusCode}",
                            mission.Id,
                            500
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Background service error while processing missions");
            }

            await Task.Delay(TimeSpan.FromSeconds(20), stoppingToken);
        }
    }
}