using MediatR;

namespace SnakeGame.GameLogic.Events;

public record FoodConsumedEvent(Position Location, string Color, int Score) : INotification;
