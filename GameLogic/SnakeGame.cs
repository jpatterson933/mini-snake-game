namespace SnakeGame.GameLogic;

public class SnakeGame
{
    private const int GridWidth = 30;
    private const int GridHeight = 30;

    public SnakeTrail Trail { get; private set; }
    public Food CurrentFood { get; private set; }
    public int Score { get; private set; }
    public bool IsGameOver { get; private set; }

    public SnakeGame()
    {
        var centerOfGrid = new Position(GridWidth / 2, GridHeight / 2);
        Trail = new SnakeTrail(centerOfGrid);
        CurrentFood = CreateFoodAtRandomLocation();
        Score = 0;
        IsGameOver = false;
    }

    public void ChangeSnakeDirection(Direction newDirection)
    {
        Trail.ChangeDirection(newDirection);
    }

    public void UpdateGameState()
    {
        if (IsGameOver)
            return;

        Trail.MoveInCurrentDirection();

        if (SnakeHasMovedOutOfBounds())
        {
            EndGame();
            return;
        }

        if (Trail.HasCollidedWithItself())
        {
            EndGame();
            return;
        }

        if (SnakeHasConsumedFood())
        {
            AddFoodToSnake();
            IncreaseScore();
            SpawnNewFood();
        }
        else
        {
            Trail.RemoveTailSegment();
        }
    }

    private bool SnakeHasMovedOutOfBounds()
    {
        var head = Trail.Head;
        return head.X < 0 || head.X >= GridWidth || head.Y < 0 || head.Y >= GridHeight;
    }

    private bool SnakeHasConsumedFood()
    {
        return CurrentFood.IsAtSameLocationAs(Trail.Head);
    }

    private void AddFoodToSnake()
    {
        Trail.GrowByAddingFood(CurrentFood.Color);
    }

    private void IncreaseScore()
    {
        Score += 10;
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
}

