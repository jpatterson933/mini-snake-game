namespace SnakeGame.GameLogic.PowerUpEffects;

public class ScoreMultiplierEffect : IPowerUpEffect
{
    private const int DoublePoints = 2;

    public PowerUpType PowerUpType => PowerUpType.ScoreMultiplier;

    public void ApplyToGame(SnakeGame game)
    {
        game.DoubleScoreMultiplier();
    }

    public void RemoveFromGame(SnakeGame game)
    {
        game.RestoreBaseScoreMultiplier();
    }
}
