using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;
using SnakeGame.Configuration;
using SnakeGame.Data;
using SnakeGame.Hubs;
using SnakeGame.Repositories;
using SnakeGame.Validation;
using System.Reflection;

DotNetEnv.Env.Load();

var environmentConfig = EnvironmentConfiguration.LoadFromEnvironment();
var builder = WebApplication.CreateBuilder(args);

ConfigureSerilogLogging(builder);
AddConfigurationOptions(builder);
AddSignalRForRealtimeGameCommunication(builder);
AddDatabaseConfiguration(builder, environmentConfig);
AddRepositories(builder);
AddValidators(builder);
AddMediatRForDomainEvents(builder);

var app = builder.Build();

ConfigureStaticFileServing(app);
var serverUrl = ConfigureServerListeningAddress(app, environmentConfig);
ConfigureHealthCheckEndpoint(app);
ConfigureSnakeGameHub(app);

LogServerStartupInformation(serverUrl);

app.Run();

void ConfigureSerilogLogging(WebApplicationBuilder applicationBuilder)
{
    Log.Logger = new LoggerConfiguration()
        .ReadFrom.Configuration(applicationBuilder.Configuration)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .CreateLogger();

    applicationBuilder.Host.UseSerilog();
    applicationBuilder.Logging.ClearProviders();
}

void AddConfigurationOptions(WebApplicationBuilder applicationBuilder)
{
    applicationBuilder.Services.Configure<GameConfiguration>(
        applicationBuilder.Configuration.GetSection(GameConfiguration.SectionName));
}

void AddSignalRForRealtimeGameCommunication(WebApplicationBuilder applicationBuilder)
{
    applicationBuilder.Services.AddSignalR();
}

void AddDatabaseConfiguration(WebApplicationBuilder applicationBuilder, EnvironmentConfiguration environmentConfig)
{
    if (!environmentConfig.HasDatabaseUrl())
    {
        throw new InvalidOperationException("DATABASE_URL environment variable is not set. Please configure PostgreSQL connection string.");
    }

    var databaseUri = new Uri(environmentConfig.DatabaseUrl);
    var userInfo = databaseUri.UserInfo.Split(':');

    var username = userInfo[0];
    var password = userInfo.Length > 1 ? userInfo[1] : string.Empty;
    var passwordPart = !string.IsNullOrEmpty(password) ? $"Password={password};" : "";
    var sslMode = environmentConfig.IsProduction() ? "SSL Mode=Require;Trust Server Certificate=true;" : "SSL Mode=Disable;";

    var npgsqlConnectionString = $"Host={databaseUri.Host};Port={databaseUri.Port};Database={databaseUri.AbsolutePath.Trim('/')};Username={username};{passwordPart}{sslMode}";

    applicationBuilder.Services.AddDbContext<SnakeGameDbContext>(options =>
        options.UseNpgsql(npgsqlConnectionString));
}

void AddRepositories(WebApplicationBuilder applicationBuilder)
{
    applicationBuilder.Services.AddScoped<IHighScoreRepository, HighScoreRepository>();
}

void AddValidators(WebApplicationBuilder applicationBuilder)
{
    applicationBuilder.Services.AddScoped<IValidator<string>, PlayerNameValidator>();
    applicationBuilder.Services.AddScoped<PlayerNameSanitizer>();
}

void AddMediatRForDomainEvents(WebApplicationBuilder applicationBuilder)
{
    applicationBuilder.Services.AddMediatR(cfg =>
        cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
}

void ConfigureStaticFileServing(WebApplication application)
{
    application.UseDefaultFiles();
    application.UseStaticFiles();
}

string ConfigureServerListeningAddress(WebApplication application, EnvironmentConfiguration environmentConfig)
{
    application.Urls.Add(environmentConfig.GetListeningAddress());
    return environmentConfig.GetServerUrlForDisplay();
}

void ConfigureHealthCheckEndpoint(WebApplication application)
{
    application.MapGet("/health", () => "OK");
}

void ConfigureSnakeGameHub(WebApplication application)
{
    application.MapHub<SnakeGameHub>("/gameHub");
}

void LogServerStartupInformation(string serverUrl)
{
    Console.WriteLine();
    Console.WriteLine("========================================");
    Console.WriteLine("🎮  Snake Game Server Started");
    Console.WriteLine("========================================");
    Console.WriteLine($"  Server:       {serverUrl}");
    Console.WriteLine($"  Health Check: {serverUrl}/health");
    Console.WriteLine($"  Game Hub:     {serverUrl}/gameHub");
    Console.WriteLine("========================================");
    Console.WriteLine();
}

