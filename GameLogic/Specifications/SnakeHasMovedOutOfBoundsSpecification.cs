using Ardalis.Specification;
using SnakeGame.Configuration;

namespace SnakeGame.GameLogic.Specifications;

public class SnakeHasMovedOutOfBoundsSpecification : Specification<SnakeGame>
{
    private readonly GameConfiguration _config;

    public SnakeHasMovedOutOfBoundsSpecification(GameConfiguration config)
    {
        _config = config;

        Query.Where(game =>
            game.Trail.Head.X < 0 ||
            game.Trail.Head.X >= _config.GridWidth ||
            game.Trail.Head.Y < 0 ||
            game.Trail.Head.Y >= _config.GridHeight);
    }
}
