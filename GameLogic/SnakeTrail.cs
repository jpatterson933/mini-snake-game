namespace SnakeGame.GameLogic;

public class SnakeTrail
{
    private readonly List<Position> segments = new();
    private readonly List<string> colors = new();
    
    public IReadOnlyList<Position> Segments => segments.AsReadOnly();
    public IReadOnlyList<string> Colors => colors.AsReadOnly();
    public Position Head => segments[0];
    public Direction CurrentDirection { get; private set; }

    public SnakeTrail(Position startingPosition)
    {
        segments.Add(startingPosition);
        colors.Add("#ffffff");
        CurrentDirection = Direction.Right;
    }

    public void ChangeDirection(Direction newDirection)
    {
        if (IsOppositeDirection(newDirection))
            return;

        CurrentDirection = newDirection;
    }

    public void MoveInCurrentDirection()
    {
        var newHead = CalculateNewHeadPosition();
        segments.Insert(0, newHead);
    }

    public void UpdateHeadPosition(Position newPosition)
    {
        segments[0] = newPosition;
    }

    public void GrowByAddingFood(string foodColor)
    {
        colors.Add(foodColor);
    }

    public void RemoveTailSegment()
    {
        segments.RemoveAt(segments.Count - 1);
    }

    public bool HasCollidedWithItself()
    {
        for (int i = 1; i < segments.Count; i++)
        {
            if (Head.IsAtSameLocationAs(segments[i]))
                return true;
        }
        return false;
    }

    private Position CalculateNewHeadPosition()
    {
        return CurrentDirection switch
        {
            Direction.Up => new Position(Head.X, Head.Y - 1),
            Direction.Down => new Position(Head.X, Head.Y + 1),
            Direction.Left => new Position(Head.X - 1, Head.Y),
            Direction.Right => new Position(Head.X + 1, Head.Y),
            _ => Head
        };
    }

    private bool IsOppositeDirection(Direction newDirection)
    {
        return (CurrentDirection == Direction.Up && newDirection == Direction.Down) ||
               (CurrentDirection == Direction.Down && newDirection == Direction.Up) ||
               (CurrentDirection == Direction.Left && newDirection == Direction.Right) ||
               (CurrentDirection == Direction.Right && newDirection == Direction.Left);
    }
}

