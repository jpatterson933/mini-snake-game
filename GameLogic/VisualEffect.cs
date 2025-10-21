namespace SnakeGame.GameLogic;

public class VisualEffect
{
    public string EffectType { get; private set; }
    public double Intensity { get; private set; }
    public double DurationRemaining { get; private set; }

    public VisualEffect(string effectType, double intensity, double duration)
    {
        EffectType = effectType;
        Intensity = intensity;
        DurationRemaining = duration;
    }

    public void UpdateDuration(double deltaTime)
    {
        DurationRemaining -= deltaTime;
    }

    public bool HasExpired() => DurationRemaining <= 0;

    public static VisualEffect CreateScreenShake(double intensity = 10.0, double duration = 0.3)
    {
        return new VisualEffect("ScreenShake", intensity, duration);
    }
}

