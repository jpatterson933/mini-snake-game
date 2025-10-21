using MediatR;

namespace SnakeGame.GameLogic.Events;

public record GameOverEvent(Position Location, int FinalScore) : INotification;
