namespace SnakeGame.GameLogic.PowerUpEffects;

public class LuckyCloverEffect : IPowerUpEffect
{
    public PowerUpType PowerUpType => PowerUpType.LuckyClover;

    public void ApplyToGame(SnakeGame game)
    {
        game.BoostPowerUpSpawnChance();
    }

    public void RemoveFromGame(SnakeGame game)
    {
        game.RestoreBasePowerUpSpawnChance();
    }
}
