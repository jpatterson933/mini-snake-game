using SnakeGame.Hubs;

var builder = WebApplication.CreateBuilder(args);

AddSignalRForRealtimeGameCommunication(builder);

var app = builder.Build();

ConfigureStaticFileServing(app);
ConfigureServerToListenOnRailwayPort(app);
ConfigureHealthCheckEndpoint(app);
ConfigureShakeGameHub(app);

app.Run();

void AddSignalRForRealtimeGameCommunication(WebApplicationBuilder applicationBuilder)
{
    applicationBuilder.Services.AddSignalR();
}

void ConfigureStaticFileServing(WebApplication application)
{
    application.UseDefaultFiles();
    application.UseStaticFiles();
}

void ConfigureServerToListenOnRailwayPort(WebApplication application)
{
    var railwayProvidedPort = Environment.GetEnvironmentVariable("PORT") ?? "8080";
    application.Urls.Add($"http://0.0.0.0:{railwayProvidedPort}");
}

void ConfigureHealthCheckEndpoint(WebApplication application)
{
    application.MapGet("/health", () => "OK");
}

void ConfigureShakeGameHub(WebApplication application)
{
    application.MapHub<ShakeGameHub>("/gameHub");
}

