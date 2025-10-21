namespace SnakeGame.GameLogic.PowerUpEffects;

public class GhostModeEffect : IPowerUpEffect
{
    public PowerUpType PowerUpType => PowerUpType.GhostMode;

    public void ApplyToGame(SnakeGame game)
    {
    }

    public void RemoveFromGame(SnakeGame game)
    {
    }
}
