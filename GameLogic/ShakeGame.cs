namespace SnakeGame.GameLogic;

public class ShakeGame
{
    private const int GridWidth = 30;
    private const int GridHeight = 30;

    public ShakeTrail Trail { get; private set; }
    public Ingredient CurrentIngredient { get; private set; }
    public int Score { get; private set; }
    public bool IsGameOver { get; private set; }

    public ShakeGame()
    {
        var centerOfGrid = new Position(GridWidth / 2, GridHeight / 2);
        Trail = new ShakeTrail(centerOfGrid);
        CurrentIngredient = CreateIngredientAtRandomLocation();
        Score = 0;
        IsGameOver = false;
    }

    public void ChangeShakeDirection(Direction newDirection)
    {
        Trail.ChangeDirection(newDirection);
    }

    public void UpdateGameState()
    {
        if (IsGameOver)
            return;

        Trail.MoveInCurrentDirection();

        if (ShakeHasMovedOutOfBounds())
        {
            EndGameWithSpilledShake();
            return;
        }

        if (Trail.HasCollidedWithItself())
        {
            EndGameWithSpilledShake();
            return;
        }

        if (ShakeHasConsumedIngredient())
        {
            AddIngredientToShake();
            IncreaseScore();
            SpawnNewIngredient();
        }
        else
        {
            Trail.RemoveTailSegment();
        }
    }

    private bool ShakeHasMovedOutOfBounds()
    {
        var head = Trail.Head;
        return head.X < 0 || head.X >= GridWidth || head.Y < 0 || head.Y >= GridHeight;
    }

    private bool ShakeHasConsumedIngredient()
    {
        return CurrentIngredient.IsAtSameLocationAs(Trail.Head);
    }

    private void AddIngredientToShake()
    {
        Trail.GrowByAddingIngredient(CurrentIngredient.Color);
    }

    private void IncreaseScore()
    {
        Score += 10;
    }

    private void SpawnNewIngredient()
    {
        CurrentIngredient = CreateIngredientAtRandomLocation();
    }

    private void EndGameWithSpilledShake()
    {
        IsGameOver = true;
    }

    private Ingredient CreateIngredientAtRandomLocation()
    {
        Position randomPosition;
        do
        {
            randomPosition = new Position(
                Random.Shared.Next(GridWidth),
                Random.Shared.Next(GridHeight)
            );
        } while (IsPositionOccupiedByShake(randomPosition));

        return new Ingredient(randomPosition);
    }

    private bool IsPositionOccupiedByShake(Position position)
    {
        return Trail.Segments.Any(segment => segment.IsAtSameLocationAs(position));
    }
}

