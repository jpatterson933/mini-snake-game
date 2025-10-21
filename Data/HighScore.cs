using System.ComponentModel.DataAnnotations;

namespace SnakeGame.Data;

public class HighScore
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string PlayerName { get; set; } = string.Empty;
    
    [Required]
    public int Score { get; set; }
    
    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

