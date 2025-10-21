using SnakeGame.Data;

namespace SnakeGame.Repositories;

public interface IHighScoreRepository
{
    Task<List<HighScore>> GetTopFiveScores();
    Task<bool> IsTopFiveScore(int score);
    Task<HighScore> SaveHighScore(string playerName, int score);
}
