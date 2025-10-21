namespace SnakeGame.Configuration;

public class GameConfiguration
{
    public const string SectionName = "Game";

    public int GridWidth { get; init; } = 30;
    public int GridHeight { get; init; } = 30;
    public double BasePowerUpSpawnChance { get; init; } = 0.15;
    public double BoostedPowerUpSpawnChance { get; init; } = 0.50;
    public int TicksBeforeNextPowerUpCanSpawn { get; init; } = 30;
    public double PowerUpExtensionPerFoodInSeconds { get; init; } = 1.0;
    public double BaseGameTickIntervalInSeconds { get; init; } = 0.15;
    public int BaseGameTickIntervalInMilliseconds { get; init; } = 100;
}
