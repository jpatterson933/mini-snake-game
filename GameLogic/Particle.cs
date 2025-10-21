namespace SnakeGame.GameLogic;

public class Particle
{
    public double X { get; private set; }
    public double Y { get; private set; }
    public double VelocityX { get; private set; }
    public double VelocityY { get; private set; }
    public string Color { get; private set; }
    public double Size { get; private set; }
    public double LifetimeRemaining { get; private set; }
    public double Opacity { get; private set; }

    public Particle(double x, double y, double velocityX, double velocityY, string color, double size, double lifetime)
    {
        X = x;
        Y = y;
        VelocityX = velocityX;
        VelocityY = velocityY;
        Color = color;
        Size = size;
        LifetimeRemaining = lifetime;
        Opacity = 1.0;
    }

    public void UpdatePosition(double deltaTime)
    {
        X += VelocityX * deltaTime;
        Y += VelocityY * deltaTime;
        LifetimeRemaining -= deltaTime;
        Opacity = Math.Max(0, LifetimeRemaining / 1.0);
    }

    public bool IsExpired() => LifetimeRemaining <= 0;
}

