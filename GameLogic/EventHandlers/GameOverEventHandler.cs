using MediatR;
using SnakeGame.GameLogic.Events;

namespace SnakeGame.GameLogic.EventHandlers;

public class GameOverEventHandler : INotificationHandler<GameOverEvent>
{
    public Task Handle(GameOverEvent notification, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
