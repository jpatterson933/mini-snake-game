namespace SnakeGame.GameLogic;

public class PowerUp
{
    public Position Location { get; private set; }
    public PowerUpType Type { get; private set; }
    public string Name { get; private set; }
    public string Color { get; private set; }
    public string Icon { get; private set; }
    public double RotationAngle { get; private set; }
    public double PulseIntensity { get; private set; }
    public int DurationInSeconds { get; private set; }
    public int PointValue { get; private set; }

    private static readonly Dictionary<PowerUpType, (string Name, string Color, string Icon, int Duration, int Points)> PowerUpDefinitions = new()
    {
        { PowerUpType.SpeedBoost, ("Speed Boost", "#FFD700", "⚡", 5, 50) },
        { PowerUpType.ScoreMultiplier, ("Score Multiplier", "#FF1493", "💎", 8, 75) },
        { PowerUpType.Invincibility, ("Invincibility", "#00BFFF", "🛡️", 6, 100) },
        { PowerUpType.SlowMotion, ("Slow Motion", "#00FFFF", "⏰", 7, 60) },
        { PowerUpType.GhostMode, ("Ghost Mode", "#9370DB", "👻", 5, 80) }
    };

    public PowerUp(Position location, PowerUpType type)
    {
        Location = location;
        Type = type;
        
        var definition = PowerUpDefinitions[type];
        Name = definition.Name;
        Color = definition.Color;
        Icon = definition.Icon;
        DurationInSeconds = definition.Duration;
        PointValue = definition.Points;
        
        RotationAngle = 0;
        PulseIntensity = 1.0;
    }

    public static PowerUp CreateRandomPowerUpAt(Position location)
    {
        var allTypes = Enum.GetValues<PowerUpType>();
        var randomType = allTypes[Random.Shared.Next(allTypes.Length)];
        return new PowerUp(location, randomType);
    }

    public void UpdateAnimationState(double deltaTime)
    {
        RotationAngle = (RotationAngle + deltaTime * 180) % 360;
        PulseIntensity = 1.0 + Math.Sin(deltaTime * 5) * 0.3;
    }

    public bool IsAtSameLocationAs(Position position) => Location.IsAtSameLocationAs(position);
}

