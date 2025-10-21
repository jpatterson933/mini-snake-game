namespace SnakeGame.GameLogic.PowerUpEffects;

public class InvincibilityEffect : IPowerUpEffect
{
    public PowerUpType PowerUpType => PowerUpType.Invincibility;

    public void ApplyToGame(SnakeGame game)
    {
    }

    public void RemoveFromGame(SnakeGame game)
    {
    }
}
