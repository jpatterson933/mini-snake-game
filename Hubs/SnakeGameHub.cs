using Microsoft.AspNetCore.SignalR;
using SnakeGame.GameLogic;

namespace SnakeGame.Hubs;

public class SnakeGameHub : Hub
{
    private static readonly Dictionary<string, GameLogic.SnakeGame> ActiveGames = new();
    private static readonly Dictionary<string, Timer> GameTimers = new();
    private readonly IHubContext<SnakeGameHub> _hubContext;

    public SnakeGameHub(IHubContext<SnakeGameHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task StartNewGame()
    {
        var connectionId = Context.ConnectionId;
        
        StopExistingGameIfRunning(connectionId);
        
        var newGame = new GameLogic.SnakeGame();
        ActiveGames[connectionId] = newGame;
        
        await SendCurrentGameStateToClient(connectionId, newGame);
        StartGameLoop(connectionId);
    }

    public Task ChangeDirection(string direction)
    {
        var connectionId = Context.ConnectionId;
        
        if (!ActiveGames.TryGetValue(connectionId, out var game))
            return Task.CompletedTask;

        var parsedDirection = ParseDirectionFromString(direction);
        game.ChangeSnakeDirection(parsedDirection);
        
        return Task.CompletedTask;
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        StopExistingGameIfRunning(Context.ConnectionId);
        return base.OnDisconnectedAsync(exception);
    }

    private void StartGameLoop(string connectionId)
    {
        CreateAndStartGameTimer(connectionId);
    }

    private void CreateAndStartGameTimer(string connectionId)
    {
        if (!ActiveGames.TryGetValue(connectionId, out var game))
            return;

        StopExistingTimerIfRunning(connectionId);

        var baseIntervalInMilliseconds = 100;
        var adjustedInterval = (int)(baseIntervalInMilliseconds / game.GameSpeed);
        
        var timer = new Timer(async _ => await GameLoopTick(connectionId), null, adjustedInterval, adjustedInterval);
        GameTimers[connectionId] = timer;
    }

    private async Task GameLoopTick(string connectionId)
    {
        if (!ActiveGames.TryGetValue(connectionId, out var game))
            return;

        var previousGameSpeed = game.GameSpeed;
        game.UpdateGameState(0.1);
        
        if (HasGameSpeedChanged(game.GameSpeed, previousGameSpeed))
        {
            CreateAndStartGameTimer(connectionId);
        }

        await SendCurrentGameStateToClient(connectionId, game);

        if (game.IsGameOver)
        {
            StopExistingGameIfRunning(connectionId);
        }
    }

    private bool HasGameSpeedChanged(double currentSpeed, double previousSpeed)
    {
        return Math.Abs(currentSpeed - previousSpeed) > 0.01;
    }

    private void StopExistingTimerIfRunning(string connectionId)
    {
        if (GameTimers.TryGetValue(connectionId, out var existingTimer))
        {
            existingTimer.Dispose();
            GameTimers.Remove(connectionId);
        }
    }

    private async Task SendCurrentGameStateToClient(string connectionId, GameLogic.SnakeGame game)
    {
        await _hubContext.Clients.Client(connectionId).SendAsync("UpdateGameState", new
        {
            trail = game.Trail.Segments,
            colors = game.Trail.Colors,
            food = new
            {
                position = game.CurrentFood.Location,
                color = game.CurrentFood.Color,
                name = game.CurrentFood.Name
            },
            powerUp = game.CurrentPowerUp != null ? new
            {
                position = game.CurrentPowerUp.Location,
                type = game.CurrentPowerUp.Type.ToString(),
                name = game.CurrentPowerUp.Name,
                color = game.CurrentPowerUp.Color,
                icon = game.CurrentPowerUp.Icon,
                rotation = game.CurrentPowerUp.RotationAngle,
                pulse = game.CurrentPowerUp.PulseIntensity
            } : null,
            particles = game.Particles.ActiveParticles.Select(p => new
            {
                x = p.X,
                y = p.Y,
                color = p.Color,
                size = p.Size,
                opacity = p.Opacity
            }).ToList(),
            activePowerUps = game.ActivePowerUps.Select(p => new
            {
                type = p.Type.ToString(),
                progress = p.GetProgressPercentage(),
                remaining = p.RemainingTimeInSeconds
            }).ToList(),
            visualEffects = game.ActiveVisualEffects.Select(e => new
            {
                type = e.EffectType,
                intensity = e.Intensity,
                duration = e.DurationRemaining
            }).ToList(),
            score = game.Score,
            scoreMultiplier = game.ScoreMultiplier,
            gameSpeed = game.GameSpeed,
            isGameOver = game.IsGameOver
        });
    }

    private void StopExistingGameIfRunning(string connectionId)
    {
        if (GameTimers.TryGetValue(connectionId, out var timer))
        {
            timer.Dispose();
            GameTimers.Remove(connectionId);
        }
        ActiveGames.Remove(connectionId);
    }

    private static Direction ParseDirectionFromString(string direction)
    {
        return direction.ToLower() switch
        {
            "up" => Direction.Up,
            "down" => Direction.Down,
            "left" => Direction.Left,
            "right" => Direction.Right,
            _ => Direction.Right
        };
    }
}

