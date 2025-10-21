using Microsoft.EntityFrameworkCore;

namespace SnakeGame.Data;

public class SnakeGameDbContext : DbContext
{
    public SnakeGameDbContext(DbContextOptions<SnakeGameDbContext> options) : base(options)
    {
    }

    public DbSet<HighScore> HighScores { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<HighScore>(entity =>
        {
            entity.HasIndex(e => e.Score);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });
    }
}

