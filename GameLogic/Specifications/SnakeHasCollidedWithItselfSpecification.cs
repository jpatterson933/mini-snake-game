using Ardalis.Specification;

namespace SnakeGame.GameLogic.Specifications;

public class SnakeHasCollidedWithItselfSpecification : Specification<SnakeGame>
{
    public SnakeHasCollidedWithItselfSpecification()
    {
        Query.Where(game => game.Trail.HasCollidedWithItself());
    }
}
