public class ActiveMissionsBackgroundService : BackgroundService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ActiveMissionsBackgroundService> _logger;

    public ActiveMissionsBackgroundService(
        IHttpClientFactory httpClientFactory,
        ILogger<ActiveMissionsBackgroundService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var client = _httpClientFactory.CreateClient();
        client.BaseAddress = new Uri("http://localhost:5062"); // safer for dev

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
                    _logger.LogWarning("No active missions found at {Time}", DateTime.UtcNow);
                    await Task.Delay(TimeSpan.FromSeconds(20), stoppingToken);
                    continue;
                }

                _logger.LogInformation("Fetched {Count} missions at {Time}",
                    missions.Count, DateTime.UtcNow);

                foreach (var mission in missions)
                {
                    var logEntry = new
                    {
                        MissionID = mission.Id,
                        MissionName = mission.Name,
                        Message = $"Telemetry check completed",
                        TimeStamp = DateTime.UtcNow,
                        CreatedAt = DateTime.UtcNow
                    };

                    var response = await client.PostAsJsonAsync(
                        $"/api/missions/{mission.Id}/logs",
                        logEntry,
                        stoppingToken
                    );

                    if (!response.IsSuccessStatusCode)
                    {
                        _logger.LogWarning(
                            "Failed to create log for mission {MissionId}. Status: {StatusCode}",
                            mission.Id,
                            response.StatusCode
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