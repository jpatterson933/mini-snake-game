namespace SnakeGame.GameLogic;

public record Position(int X, int Y)
{
    public bool IsAtSameLocationAs(Position other) => X == other.X && Y == other.Y;
}

