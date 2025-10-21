namespace SnakeGame.GameLogic.PowerUpEffects;

public class PowerUpEffectFactory
{
    private readonly Dictionary<PowerUpType, IPowerUpEffect> _effectsByType;

    public PowerUpEffectFactory()
    {
        _effectsByType = new Dictionary<PowerUpType, IPowerUpEffect>
        {
            { PowerUpType.SpeedBoost, new SpeedBoostEffect() },
            { PowerUpType.ScoreMultiplier, new ScoreMultiplierEffect() },
            { PowerUpType.SlowMotion, new SlowMotionEffect() },
            { PowerUpType.LuckyClover, new LuckyCloverEffect() },
            { PowerUpType.Invincibility, new InvincibilityEffect() },
            { PowerUpType.GhostMode, new GhostModeEffect() }
        };
    }

    public IPowerUpEffect CreateEffectFor(PowerUpType powerUpType)
    {
        if (!_effectsByType.TryGetValue(powerUpType, out var effect))
        {
            throw new ArgumentException($"Unknown power up type: {powerUpType}", nameof(powerUpType));
        }

        return effect;
    }
}
