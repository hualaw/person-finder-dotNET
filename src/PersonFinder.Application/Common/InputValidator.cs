namespace PersonFinder.Application.Common;

public static class InputValidator
{
    private static readonly HashSet<string> ForbiddenKeywords =
        new(StringComparer.OrdinalIgnoreCase)
        {
            "ignore", "instruction", "command", "disregard", "system"
        };

    public static bool IsInputSafe(string jobTitle, IEnumerable<string> hobbies) =>
        !ContainsForbiddenKeyword(jobTitle) && !hobbies.Any(h => ContainsForbiddenKeyword(h));

    public static bool IsLocationValid(double latitude, double longitude, double? radius = null)
    {
        if (latitude is < -90 or > 90 || longitude is < -180 or > 180)
        {
            return false;
        }

        if (radius.HasValue && radius.Value <= 0)
        {
            return false;
        }

        return true;
    }

    private static bool ContainsForbiddenKeyword(string text) =>
        ForbiddenKeywords.Any(kw => text.Contains(kw, StringComparison.OrdinalIgnoreCase));
}
