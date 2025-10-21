namespace SnakeGame.GameLogic.PowerUpEffects;

public interface IPowerUpEffect
{
    PowerUpType PowerUpType { get; }
    void ApplyToGame(SnakeGame game);
    void RemoveFromGame(SnakeGame game);
}
