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
        var timer = new Timer(async _ => await GameLoopTick(connectionId), null, 150, 150);
        GameTimers[connectionId] = timer;
    }

    private async Task GameLoopTick(string connectionId)
    {
        if (!ActiveGames.TryGetValue(connectionId, out var game))
            return;

        game.UpdateGameState();
        await SendCurrentGameStateToClient(connectionId, game);

        if (game.IsGameOver)
        {
            StopExistingGameIfRunning(connectionId);
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
            score = game.Score,
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

