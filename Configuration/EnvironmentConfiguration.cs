namespace SnakeGame.Configuration;

public class EnvironmentConfiguration
{
    public string DatabaseUrl { get; private set; } = string.Empty;
    public string BaseUrl { get; private set; } = string.Empty;
    public string Port { get; private set; } = "8080";
    public string Environment { get; private set; } = "Development";

    public static EnvironmentConfiguration LoadFromEnvironment()
    {
        return new EnvironmentConfiguration
        {
            DatabaseUrl = System.Environment.GetEnvironmentVariable("DATABASE_URL") ?? string.Empty,
            BaseUrl = System.Environment.GetEnvironmentVariable("BASE_URL") ?? string.Empty,
            Port = System.Environment.GetEnvironmentVariable("PORT") ?? "8080",
            Environment = System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"
        };
    }

    public bool HasDatabaseUrl() => !string.IsNullOrEmpty(DatabaseUrl);
    
    public bool HasBaseUrl() => !string.IsNullOrEmpty(BaseUrl);

    public bool IsProduction() => Environment.Equals("Production", StringComparison.OrdinalIgnoreCase);

    public string GetServerUrlForDisplay()
    {
        if (HasBaseUrl())
        {
            return BaseUrl;
        }

        return $"http://localhost:{Port}";
    }

    public string GetListeningAddress()
    {
        var port = HasBaseUrl() ? new Uri(BaseUrl).Port.ToString() : Port;
        return $"http://0.0.0.0:{port}";
    }
}

