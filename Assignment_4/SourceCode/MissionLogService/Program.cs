var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHttpClient();
builder.Services.AddHostedService<ActiveMissionsBackgroundService>();

var host = builder.Build();
host.Run();