namespace SnakeGame.GameLogic.PowerUpEffects;

public class SlowMotionEffect : IPowerUpEffect
{
    private const double SpeedMultiplier = 0.6;

    public PowerUpType PowerUpType => PowerUpType.SlowMotion;

    public void ApplyToGame(SnakeGame game)
    {
        game.DecreaseGameSpeedByMultiplier(SpeedMultiplier);
    }

    public void RemoveFromGame(SnakeGame game)
    {
        game.RestoreNormalGameSpeed();
    }
}
