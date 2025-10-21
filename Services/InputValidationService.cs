using System.Text.RegularExpressions;
using DotnetBadWordDetector;

namespace SnakeGame.Services;

public class InputValidationService
{
    private readonly ProfanityDetector _profanityDetector;

    public InputValidationService()
    {
        _profanityDetector = new ProfanityDetector();
    }

    public ValidationResult ValidatePlayerName(string playerName)
    {
        if (string.IsNullOrWhiteSpace(playerName))
        {
            return new ValidationResult(false, "Player name cannot be empty.");
        }

        if (playerName.Length > 50)
        {
            return new ValidationResult(false, "Player name cannot exceed 50 characters.");
        }

        if (playerName.Length < 2)
        {
            return new ValidationResult(false, "Player name must be at least 2 characters.");
        }

        // Allow only alphanumeric characters, spaces, and basic punctuation
        if (!Regex.IsMatch(playerName, @"^[a-zA-Z0-9\s\-_\.]+$"))
        {
            return new ValidationResult(false, "Player name can only contain letters, numbers, spaces, hyphens, underscores, and periods.");
        }

        // Check for offensive words using ML-based profanity detector
        if (_profanityDetector.IsPhraseProfane(playerName))
        {
            return new ValidationResult(false, "Player name contains inappropriate language.");
        }

        // Prevent SQL injection patterns (though EF Core already parameterizes queries)
        if (ContainsSqlInjectionPatterns(playerName))
        {
            return new ValidationResult(false, "Player name contains invalid characters.");
        }

        // Prevent XSS patterns
        if (ContainsXssPatterns(playerName))
        {
            return new ValidationResult(false, "Player name contains invalid characters.");
        }

        return new ValidationResult(true, SanitizeInput(playerName));
    }

    private static bool ContainsSqlInjectionPatterns(string input)
    {
        var sqlPatterns = new[]
        {
            "';", "--", "/*", "*/", "xp_", "sp_", "exec", "execute", "select", "insert",
            "update", "delete", "drop", "create", "alter", "union", "script", "javascript"
        };

        var lowerInput = input.ToLower();
        return sqlPatterns.Any(pattern => lowerInput.Contains(pattern));
    }

    private static bool ContainsXssPatterns(string input)
    {
        var xssPatterns = new[] { "<", ">", "&lt;", "&gt;", "script", "javascript", "onerror", "onclick" };
        var lowerInput = input.ToLower();
        return xssPatterns.Any(pattern => lowerInput.Contains(pattern));
    }

    private static string SanitizeInput(string input)
    {
        // Trim whitespace and normalize
        var sanitized = input.Trim();
        
        // Remove any control characters
        sanitized = Regex.Replace(sanitized, @"[\x00-\x1F\x7F]", "");
        
        // Collapse multiple spaces into single space
        sanitized = Regex.Replace(sanitized, @"\s+", " ");
        
        return sanitized;
    }
}

public class ValidationResult
{
    public bool IsValid { get; }
    public string Message { get; }

    public ValidationResult(bool isValid, string message)
    {
        IsValid = isValid;
        Message = message;
    }
}

