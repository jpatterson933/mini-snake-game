using Microsoft.EntityFrameworkCore;
using SnakeGame.Hubs;
using SnakeGame.Data;
using SnakeGame.Services;

var builder = WebApplication.CreateBuilder(args);

AddSignalRForRealtimeGameCommunication(builder);
AddDatabaseConfiguration(builder);
AddServices(builder);

var app = builder.Build();

await EnsureDatabaseCreated(app);

ConfigureStaticFileServing(app);
ConfigureServerToListenOnRailwayPort(app);
ConfigureHealthCheckEndpoint(app);
ConfigureSnakeGameHub(app);

app.Run();

void AddSignalRForRealtimeGameCommunication(WebApplicationBuilder applicationBuilder)
{
    applicationBuilder.Services.AddSignalR();
}

void AddDatabaseConfiguration(WebApplicationBuilder applicationBuilder)
{
    var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL");
    
    if (string.IsNullOrEmpty(connectionString))
    {
        throw new InvalidOperationException("DATABASE_URL environment variable is not set. Please configure PostgreSQL connection string in Railway.");
    }

    // Railway provides DATABASE_URL in format: postgres://user:password@host:port/database
    // Convert to Npgsql connection string format
    var databaseUri = new Uri(connectionString);
    var userInfo = databaseUri.UserInfo.Split(':');
    
    var npgsqlConnectionString = $"Host={databaseUri.Host};Port={databaseUri.Port};Database={databaseUri.AbsolutePath.Trim('/')};Username={userInfo[0]};Password={userInfo[1]};SSL Mode=Require;Trust Server Certificate=true";

    applicationBuilder.Services.AddDbContext<SnakeGameDbContext>(options =>
        options.UseNpgsql(npgsqlConnectionString));
}

void AddServices(WebApplicationBuilder applicationBuilder)
{
    applicationBuilder.Services.AddScoped<InputValidationService>();
}

async Task EnsureDatabaseCreated(WebApplication application)
{
    using var scope = application.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<SnakeGameDbContext>();
    await dbContext.Database.MigrateAsync();
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

void ConfigureSnakeGameHub(WebApplication application)
{
    application.MapHub<SnakeGameHub>("/gameHub");
}

