namespace SnakeGame.GameLogic.PowerUpEffects;

public class SpeedBoostEffect : IPowerUpEffect
{
    private const double SpeedMultiplier = 1.5;

    public PowerUpType PowerUpType => PowerUpType.SpeedBoost;

    public void ApplyToGame(SnakeGame game)
    {
        game.IncreaseGameSpeedByMultiplier(SpeedMultiplier);
    }

    public void RemoveFromGame(SnakeGame game)
    {
        game.RestoreNormalGameSpeed();
    }
}
