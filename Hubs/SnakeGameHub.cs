using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using SnakeGame.Configuration;
using SnakeGame.GameLogic;
using SnakeGame.Repositories;
using SnakeGame.Validation;

namespace SnakeGame.Hubs;

public class SnakeGameHub : Hub
{
    private static readonly Dictionary<string, GameLogic.SnakeGame> ActiveGames = new();
    private static readonly Dictionary<string, Timer> GameTimers = new();

    private readonly IHubContext<SnakeGameHub> _hubContext;
    private readonly IHighScoreRepository _highScoreRepository;
    private readonly IValidator<string> _playerNameValidator;
    private readonly PlayerNameSanitizer _playerNameSanitizer;
    private readonly GameConfiguration _gameConfig;
    private readonly IMediator _mediator;
    private readonly ILogger<SnakeGameHub> _logger;

    public SnakeGameHub(
        IHubContext<SnakeGameHub> hubContext,
        IHighScoreRepository highScoreRepository,
        IValidator<string> playerNameValidator,
        PlayerNameSanitizer playerNameSanitizer,
        IOptions<GameConfiguration> gameConfig,
        IMediator mediator,
        ILogger<SnakeGameHub> logger)
    {
        _hubContext = hubContext;
        _highScoreRepository = highScoreRepository;
        _playerNameValidator = playerNameValidator;
        _playerNameSanitizer = playerNameSanitizer;
        _gameConfig = gameConfig.Value;
        _mediator = mediator;
        _logger = logger;
    }

    public async Task StartNewGame()
    {
        var connectionId = Context.ConnectionId;

        _logger.LogInformation("Player {ConnectionId} starting new game", connectionId);

        StopExistingGameIfRunning(connectionId);

        var newGame = new GameLogic.SnakeGame(_gameConfig, _mediator);
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

    public async Task<bool> IsTopFiveScore(int score)
    {
        _logger.LogInformation("Checking if score {Score} is a top five score", score);
        return await _highScoreRepository.IsTopFiveScore(score);
    }

    public async Task<object> SaveHighScore(string playerName, int score)
    {
        _logger.LogInformation("Attempting to save high score for player {PlayerName} with score {Score}", playerName, score);

        var validationResult = await _playerNameValidator.ValidateAsync(playerName);

        if (!validationResult.IsValid)
        {
            var firstError = validationResult.Errors.First().ErrorMessage;
            _logger.LogWarning("Player name validation failed: {ErrorMessage}", firstError);
            return new { success = false, error = firstError };
        }

        if (score <= 0)
        {
            _logger.LogWarning("Invalid score attempted: {Score}", score);
            return new { success = false, error = "Invalid score." };
        }

        var sanitizedName = _playerNameSanitizer.SanitizePlayerName(playerName);

        await _highScoreRepository.SaveHighScore(sanitizedName, score);

        _logger.LogInformation("High score saved successfully for {PlayerName} with score {Score}", sanitizedName, score);

        return new { success = true, message = "High score saved successfully!" };
    }

    public async Task<List<object>> GetTopFiveScores()
    {
        _logger.LogDebug("Fetching top five scores");

        var topScores = await _highScoreRepository.GetTopFiveScores();

        return topScores.Select(h => new
        {
            playerName = h.PlayerName,
            score = h.Score,
            date = h.CreatedAt
        } as object).ToList();
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

        var adjustedInterval = (int)(_gameConfig.BaseGameTickIntervalInMilliseconds / game.GameSpeed);

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

