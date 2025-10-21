namespace SnakeGame.GameLogic;

public class SnakeGame
{
    private const int GridWidth = 30;
    private const int GridHeight = 30;
    private const double BasePowerUpSpawnChance = 0.15;
    private const double BoostedPowerUpSpawnChance = 0.50;
    private const int TicksBeforeNextPowerUpCanSpawn = 30;
    private const double PowerUpExtensionPerFoodInSeconds = 1.0;

    public SnakeTrail Trail { get; private set; }
    public Food CurrentFood { get; private set; }
    public PowerUp? CurrentPowerUp { get; private set; }
    public ParticleSystem Particles { get; private set; }
    public int Score { get; private set; }
    public bool IsGameOver { get; private set; }
    public int ScoreMultiplier { get; private set; }
    public double GameSpeed { get; private set; }
    public double CurrentPowerUpSpawnChance { get; private set; }

    private readonly List<ActivePowerUp> activePowerUps = new();
    private readonly List<VisualEffect> activeVisualEffects = new();
    private int ticksSinceLastPowerUpSpawn = 0;

    public IReadOnlyList<ActivePowerUp> ActivePowerUps => activePowerUps.AsReadOnly();
    public IReadOnlyList<VisualEffect> ActiveVisualEffects => activeVisualEffects.AsReadOnly();

    public SnakeGame()
    {
        var centerOfGrid = new Position(GridWidth / 2, GridHeight / 2);
        Trail = new SnakeTrail(centerOfGrid);
        CurrentFood = CreateFoodAtRandomLocation();
        Particles = new ParticleSystem();
        Score = 0;
        IsGameOver = false;
        ScoreMultiplier = 1;
        GameSpeed = 1.0;
        CurrentPowerUpSpawnChance = BasePowerUpSpawnChance;
        CurrentPowerUp = null;
    }

    public void ChangeSnakeDirection(Direction newDirection)
    {
        Trail.ChangeDirection(newDirection);
    }

    public void UpdateGameState(double deltaTime = 0.15)
    {
        if (IsGameOver)
            return;

        UpdateActivePowerUpTimers(deltaTime);
        UpdateVisualEffects(deltaTime);
        Particles.UpdateAllParticles(deltaTime);
        
        Trail.MoveInCurrentDirection();

        if (SnakeHasMovedOutOfBounds())
        {
            if (IsInvincible() || IsGhostModeActive())
            {
                WrapSnakeHeadAroundGrid();
            }
            else
            {
                TriggerGameOverEffects();
                EndGame();
                return;
            }
        }

        if (Trail.HasCollidedWithItself() && !IsInvincible())
        {
            TriggerGameOverEffects();
            EndGame();
            return;
        }

        if (SnakeHasConsumedFood())
        {
            TriggerFoodCollectionEffects();
            AddFoodToSnake();
            IncreaseScoreByFoodValue();
            ExtendActivePowerUpDurations();
            SpawnNewFood();
            ConsiderSpawningPowerUp();
        }
        else
        {
            Trail.RemoveTailSegment();
        }

        if (SnakeHasConsumedPowerUp())
        {
            CollectPowerUp();
        }

        ticksSinceLastPowerUpSpawn++;
    }

    private bool SnakeHasMovedOutOfBounds()
    {
        var head = Trail.Head;
        return head.X < 0 || head.X >= GridWidth || head.Y < 0 || head.Y >= GridHeight;
    }

    private void WrapSnakeHeadAroundGrid()
    {
        var head = Trail.Head;
        var wrappedX = head.X;
        var wrappedY = head.Y;

        if (head.X < 0)
            wrappedX = GridWidth - 1;
        else if (head.X >= GridWidth)
            wrappedX = 0;

        if (head.Y < 0)
            wrappedY = GridHeight - 1;
        else if (head.Y >= GridHeight)
            wrappedY = 0;

        Trail.UpdateHeadPosition(new Position(wrappedX, wrappedY));
    }

    private bool SnakeHasConsumedFood()
    {
        return CurrentFood.IsAtSameLocationAs(Trail.Head);
    }

    private void AddFoodToSnake()
    {
        Trail.GrowByAddingFood(CurrentFood.Color);
    }

    private void IncreaseScoreByFoodValue()
    {
        Score += 10 * ScoreMultiplier;
    }

    private void IncreaseScoreByPowerUpValue(int powerUpPoints)
    {
        Score += powerUpPoints * ScoreMultiplier;
    }

    private void SpawnNewFood()
    {
        CurrentFood = CreateFoodAtRandomLocation();
    }

    private void EndGame()
    {
        IsGameOver = true;
    }

    private Food CreateFoodAtRandomLocation()
    {
        Position randomPosition;
        do
        {
            randomPosition = new Position(
                Random.Shared.Next(GridWidth),
                Random.Shared.Next(GridHeight)
            );
        } while (IsPositionOccupiedBySnake(randomPosition));

        return new Food(randomPosition);
    }

    private bool IsPositionOccupiedBySnake(Position position)
    {
        return Trail.Segments.Any(segment => segment.IsAtSameLocationAs(position));
    }

    private void ConsiderSpawningPowerUp()
    {
        if (CurrentPowerUp != null)
            return;

        if (ticksSinceLastPowerUpSpawn < TicksBeforeNextPowerUpCanSpawn)
            return;

        if (Random.Shared.NextDouble() > CurrentPowerUpSpawnChance)
            return;

        CurrentPowerUp = CreatePowerUpAtRandomLocation();
        ticksSinceLastPowerUpSpawn = 0;
    }

    private PowerUp CreatePowerUpAtRandomLocation()
    {
        Position randomPosition;
        do
        {
            randomPosition = new Position(
                Random.Shared.Next(GridWidth),
                Random.Shared.Next(GridHeight)
            );
        } while (IsPositionOccupiedBySnakeOrFood(randomPosition));

        return PowerUp.CreateRandomPowerUpAt(randomPosition);
    }

    private bool IsPositionOccupiedBySnakeOrFood(Position position)
    {
        return IsPositionOccupiedBySnake(position) || CurrentFood.IsAtSameLocationAs(position);
    }

    private bool SnakeHasConsumedPowerUp()
    {
        return CurrentPowerUp != null && CurrentPowerUp.IsAtSameLocationAs(Trail.Head);
    }

    private void CollectPowerUp()
    {
        if (CurrentPowerUp == null)
            return;

        TriggerPowerUpCollectionEffects(CurrentPowerUp);
        ActivatePowerUpEffect(CurrentPowerUp);
        IncreaseScoreByPowerUpValue(CurrentPowerUp.PointValue);
        CurrentPowerUp = null;
    }

    private void ActivatePowerUpEffect(PowerUp powerUp)
    {
        var activePowerUp = new ActivePowerUp(powerUp.Type, powerUp.DurationInSeconds);
        activePowerUps.Add(activePowerUp);
        ApplyPowerUpEffect(powerUp.Type);
    }

    private void ApplyPowerUpEffect(PowerUpType type)
    {
        switch (type)
        {
            case PowerUpType.SpeedBoost:
                GameSpeed = 1.5;
                break;
            case PowerUpType.ScoreMultiplier:
                ScoreMultiplier = 2;
                break;
            case PowerUpType.SlowMotion:
                GameSpeed = 0.6;
                break;
            case PowerUpType.LuckyClover:
                CurrentPowerUpSpawnChance = BoostedPowerUpSpawnChance;
                break;
        }
    }

    private void RemovePowerUpEffect(PowerUpType type)
    {
        switch (type)
        {
            case PowerUpType.SpeedBoost:
            case PowerUpType.SlowMotion:
                GameSpeed = 1.0;
                break;
            case PowerUpType.ScoreMultiplier:
                ScoreMultiplier = 1;
                break;
            case PowerUpType.LuckyClover:
                CurrentPowerUpSpawnChance = BasePowerUpSpawnChance;
                break;
        }
    }

    private void UpdateActivePowerUpTimers(double deltaTime)
    {
        foreach (var powerUp in activePowerUps.ToList())
        {
            powerUp.DecrementTimeByGameTick(deltaTime);
            
            if (powerUp.HasExpired())
            {
                RemovePowerUpEffect(powerUp.Type);
                activePowerUps.Remove(powerUp);
            }
        }
    }

    private void ExtendActivePowerUpDurations()
    {
        foreach (var powerUp in activePowerUps)
        {
            powerUp.ExtendTimeByFoodConsumption(PowerUpExtensionPerFoodInSeconds);
        }
    }

    private void UpdateVisualEffects(double deltaTime)
    {
        foreach (var effect in activeVisualEffects.ToList())
        {
            effect.UpdateDuration(deltaTime);
            
            if (effect.HasExpired())
            {
                activeVisualEffects.Remove(effect);
            }
        }
    }

    private bool IsInvincible()
    {
        return activePowerUps.Any(p => p.Type == PowerUpType.Invincibility);
    }

    private bool IsGhostModeActive()
    {
        return activePowerUps.Any(p => p.Type == PowerUpType.GhostMode);
    }

    private void TriggerFoodCollectionEffects()
    {
        Particles.EmitParticleBurstAt(Trail.Head, CurrentFood.Color, 15);
    }

    private void TriggerPowerUpCollectionEffects(PowerUp powerUp)
    {
        if (powerUp.Type == PowerUpType.LuckyClover)
        {
            Particles.EmitMassiveExplosionAt(Trail.Head, powerUp.Color, 80);
        }
        else
        {
            Particles.EmitParticleBurstAt(Trail.Head, powerUp.Color, 30);
        }
    }

    private void TriggerGameOverEffects()
    {
        Particles.EmitParticleBurstAt(Trail.Head, "#ff0000", 50);
        activeVisualEffects.Add(VisualEffect.CreateScreenShake(15, 0.5));
    }
}

