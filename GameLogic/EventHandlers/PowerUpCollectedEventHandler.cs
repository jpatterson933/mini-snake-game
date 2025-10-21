using MediatR;
using SnakeGame.GameLogic.Events;

namespace SnakeGame.GameLogic.EventHandlers;

public class PowerUpCollectedEventHandler : INotificationHandler<PowerUpCollectedEvent>
{
    public Task Handle(PowerUpCollectedEvent notification, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
