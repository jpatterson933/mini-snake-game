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

var builder = WebApplication.CreateBuilder(args);

ConfigureSerilogLogging(builder);
AddConfigurationOptions(builder);
AddSignalRForRealtimeGameCommunication(builder);
AddDatabaseConfiguration(builder);
AddRepositories(builder);
AddValidators(builder);
AddMediatRForDomainEvents(builder);

var app = builder.Build();

ConfigureStaticFileServing(app);
ConfigureServerToListenOnRailwayPort(app);
ConfigureHealthCheckEndpoint(app);
ConfigureSnakeGameHub(app);

app.Run();

void ConfigureSerilogLogging(WebApplicationBuilder applicationBuilder)
{
    Log.Logger = new LoggerConfiguration()
        .ReadFrom.Configuration(applicationBuilder.Configuration)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .CreateLogger();

    applicationBuilder.Host.UseSerilog();
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

void AddDatabaseConfiguration(WebApplicationBuilder applicationBuilder)
{
    var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL");

    if (string.IsNullOrEmpty(connectionString))
    {
        throw new InvalidOperationException("DATABASE_URL environment variable is not set. Please configure PostgreSQL connection string in Railway.");
    }

    var databaseUri = new Uri(connectionString);
    var userInfo = databaseUri.UserInfo.Split(':');

    var npgsqlConnectionString = $"Host={databaseUri.Host};Port={databaseUri.Port};Database={databaseUri.AbsolutePath.Trim('/')};Username={userInfo[0]};Password={userInfo[1]};SSL Mode=Require;Trust Server Certificate=true";

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

