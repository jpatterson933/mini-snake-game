namespace SnakeGame.GameLogic;

public class ParticleSystem
{
    private readonly List<Particle> particles = new();
    public IReadOnlyList<Particle> ActiveParticles => particles.AsReadOnly();

    public void EmitParticleBurstAt(Position position, string color, int particleCount)
    {
        var centerX = position.X + 0.5;
        var centerY = position.Y + 0.5;

        for (int i = 0; i < particleCount; i++)
        {
            var angle = Random.Shared.NextDouble() * Math.PI * 2;
            var speed = Random.Shared.NextDouble() * 3 + 1;
            var velocityX = Math.Cos(angle) * speed;
            var velocityY = Math.Sin(angle) * speed;
            var size = Random.Shared.NextDouble() * 0.3 + 0.1;
            var lifetime = Random.Shared.NextDouble() * 0.5 + 0.5;

            particles.Add(new Particle(centerX, centerY, velocityX, velocityY, color, size, lifetime));
        }
    }

    public void EmitMassiveExplosionAt(Position position, string color, int particleCount)
    {
        var centerX = position.X + 0.5;
        var centerY = position.Y + 0.5;

        for (int i = 0; i < particleCount; i++)
        {
            var angle = Random.Shared.NextDouble() * Math.PI * 2;
            var speed = Random.Shared.NextDouble() * 8 + 3;
            var velocityX = Math.Cos(angle) * speed;
            var velocityY = Math.Sin(angle) * speed;
            var size = Random.Shared.NextDouble() * 0.6 + 0.3;
            var lifetime = Random.Shared.NextDouble() * 1.5 + 1.0;

            particles.Add(new Particle(centerX, centerY, velocityX, velocityY, color, size, lifetime));
        }
    }

    public void UpdateAllParticles(double deltaTime)
    {
        foreach (var particle in particles)
        {
            particle.UpdatePosition(deltaTime);
        }

        RemoveExpiredParticles();
    }

    private void RemoveExpiredParticles()
    {
        particles.RemoveAll(p => p.IsExpired());
    }

    public void ClearAllParticles()
    {
        particles.Clear();
    }
}

