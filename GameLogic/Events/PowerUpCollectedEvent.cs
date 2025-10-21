using MediatR;

namespace SnakeGame.GameLogic.Events;

public record PowerUpCollectedEvent(PowerUpType Type, Position Location, string Color, bool IsMassiveExplosion) : INotification;
