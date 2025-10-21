using System.Text.RegularExpressions;

namespace SnakeGame.Validation;

public class PlayerNameSanitizer
{
    public string SanitizePlayerName(string playerName)
    {
        var sanitized = TrimWhitespace(playerName);
        sanitized = RemoveControlCharacters(sanitized);
        sanitized = CollapseMultipleSpacesIntoSingleSpace(sanitized);

        return sanitized;
    }

    private static string TrimWhitespace(string input)
    {
        return input.Trim();
    }

    private static string RemoveControlCharacters(string input)
    {
        return Regex.Replace(input, @"[\x00-\x1F\x7F]", "");
    }

    private static string CollapseMultipleSpacesIntoSingleSpace(string input)
    {
        return Regex.Replace(input, @"\s+", " ");
    }
}
