using MediatR;
using SnakeGame.GameLogic.Events;

namespace SnakeGame.GameLogic.EventHandlers;

public class FoodConsumedEventHandler : INotificationHandler<FoodConsumedEvent>
{
    public Task Handle(FoodConsumedEvent notification, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
