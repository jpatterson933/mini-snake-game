using Microsoft.EntityFrameworkCore;
using SnakeGame.Data;

namespace SnakeGame.Repositories;

public class HighScoreRepository : IHighScoreRepository
{
    private readonly SnakeGameDbContext _dbContext;

    public HighScoreRepository(SnakeGameDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<HighScore>> GetTopFiveScores()
    {
        return await _dbContext.HighScores
            .OrderByDescending(highScore => highScore.Score)
            .Take(5)
            .ToListAsync();
    }

    public async Task<bool> IsTopFiveScore(int score)
    {
        if (score <= 0)
            return false;

        var topFiveScores = await GetTopFiveScores();

        if (HasFewerThanFiveScores(topFiveScores))
            return true;

        return ScoreExceedsLowestTopFiveScore(score, topFiveScores);
    }

    public async Task<HighScore> SaveHighScore(string playerName, int score)
    {
        var highScore = CreateHighScore(playerName, score);

        _dbContext.HighScores.Add(highScore);
        await _dbContext.SaveChangesAsync();

        return highScore;
    }

    private static bool HasFewerThanFiveScores(List<HighScore> topFiveScores)
    {
        return topFiveScores.Count < 5;
    }

    private static bool ScoreExceedsLowestTopFiveScore(int score, List<HighScore> topFiveScores)
    {
        var lowestTopFiveScore = topFiveScores.Min(highScore => highScore.Score);
        return score > lowestTopFiveScore;
    }

    private static HighScore CreateHighScore(string playerName, int score)
    {
        return new HighScore
        {
            PlayerName = playerName,
            Score = score,
            CreatedAt = DateTime.UtcNow
        };
    }
}
