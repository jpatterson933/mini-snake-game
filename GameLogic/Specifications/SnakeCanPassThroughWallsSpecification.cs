using Ardalis.Specification;

namespace SnakeGame.GameLogic.Specifications;

public class SnakeCanPassThroughWallsSpecification : Specification<SnakeGame>
{
    public SnakeCanPassThroughWallsSpecification()
    {
        Query.Where(game =>
            game.IsInvincible() || game.IsGhostModeActive());
    }
}
