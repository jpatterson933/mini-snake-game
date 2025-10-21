var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

ConfigureStaticFileServing(app);
ConfigureServerToListenOnRailwayPort(app);
ConfigureHealthCheckEndpoint(app);

app.Run();

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

