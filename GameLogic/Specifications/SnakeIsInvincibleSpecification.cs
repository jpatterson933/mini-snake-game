using Ardalis.Specification;

namespace SnakeGame.GameLogic.Specifications;

public class SnakeIsInvincibleSpecification : Specification<SnakeGame>
{
    public SnakeIsInvincibleSpecification()
    {
        Query.Where(game => game.IsInvincible());
    }
}
