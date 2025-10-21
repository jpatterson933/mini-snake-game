namespace SnakeGame.GameLogic;

public class ActivePowerUp
{
    public PowerUpType Type { get; private set; }
    public double RemainingTimeInSeconds { get; private set; }
    public double TotalDurationInSeconds { get; private set; }

    public ActivePowerUp(PowerUpType type, int durationInSeconds)
    {
        Type = type;
        RemainingTimeInSeconds = durationInSeconds;
        TotalDurationInSeconds = durationInSeconds;
    }

    public void DecrementTimeByGameTick(double tickDurationInSeconds)
    {
        RemainingTimeInSeconds -= tickDurationInSeconds;
    }

    public bool HasExpired() => RemainingTimeInSeconds <= 0;

    public double GetProgressPercentage() => RemainingTimeInSeconds / TotalDurationInSeconds;
}

