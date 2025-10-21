using System.Text.RegularExpressions;
using DotnetBadWordDetector;
using FluentValidation;

namespace SnakeGame.Validation;

public class PlayerNameValidator : AbstractValidator<string>
{
    private readonly ProfanityDetector _profanityDetector;

    public PlayerNameValidator()
    {
        _profanityDetector = new ProfanityDetector();

        RuleFor(playerName => playerName)
            .NotEmpty()
            .WithMessage("Player name cannot be empty");

        RuleFor(playerName => playerName)
            .MinimumLength(2)
            .WithMessage("Player name must be at least 2 characters");

        RuleFor(playerName => playerName)
            .MaximumLength(50)
            .WithMessage("Player name cannot exceed 50 characters");

        RuleFor(playerName => playerName)
            .Must(ContainOnlyValidCharacters)
            .WithMessage("Player name can only contain letters, numbers, spaces, hyphens, underscores, and periods");

        RuleFor(playerName => playerName)
            .Must(NotContainProfanity)
            .WithMessage("Player name contains inappropriate language");

        RuleFor(playerName => playerName)
            .Must(NotContainSqlInjectionPatterns)
            .WithMessage("Player name contains invalid characters");

        RuleFor(playerName => playerName)
            .Must(NotContainXssPatterns)
            .WithMessage("Player name contains invalid characters");
    }

    private static bool ContainOnlyValidCharacters(string playerName)
    {
        return Regex.IsMatch(playerName, @"^[a-zA-Z0-9\s\-_\.]+$");
    }

    private bool NotContainProfanity(string playerName)
    {
        return !_profanityDetector.IsPhraseProfane(playerName);
    }

    private static bool NotContainSqlInjectionPatterns(string playerName)
    {
        var sqlPatterns = new[]
        {
            "';", "--", "/*", "*/", "xp_", "sp_", "exec", "execute", "select", "insert",
            "update", "delete", "drop", "create", "alter", "union", "script", "javascript"
        };

        var lowerInput = playerName.ToLower();
        return !sqlPatterns.Any(pattern => lowerInput.Contains(pattern));
    }

    private static bool NotContainXssPatterns(string playerName)
    {
        var xssPatterns = new[] { "<", ">", "&lt;", "&gt;", "script", "javascript", "onerror", "onclick" };
        var lowerInput = playerName.ToLower();
        return !xssPatterns.Any(pattern => lowerInput.Contains(pattern));
    }
}
