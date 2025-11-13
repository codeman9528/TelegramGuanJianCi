namespace TelegramMonitor;

public static class KeywordMatchExtensions
{
    public static List<KeywordConfig> MatchUser(
        long userId,
        IReadOnlyCollection<string> userNames,
        IEnumerable<KeywordConfig> allKeywords)
    {
        if (allKeywords == null) return new();
        return allKeywords
            .Where(k => k.KeywordType == KeywordType.User)
            .Where(k => IsUserMatch(userId, userNames, k.KeywordContent))
            .ToList();
    }

    public static List<KeywordConfig> MatchText(
        string message,
        IEnumerable<KeywordConfig> allKeywords)
    {
        if (string.IsNullOrWhiteSpace(message) || allKeywords == null) return new();
        return allKeywords
            .Where(k => k.KeywordType != KeywordType.User)
            .Where(k => IsKeywordMatch(k, message))
            .ToList();
    }

    private static bool IsUserMatch(long userId, IReadOnlyCollection<string> names, string keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword))
            return false;

        var normalizedKeyword = keyword.StartsWith("@") ? keyword[1..] : keyword;

        if (userId.ToString() == normalizedKeyword)
            return true;

        return names.Any(name =>
        {
            var normalizedName = name.StartsWith("@") ? name[1..] : name;
            return string.Equals(normalizedName, normalizedKeyword, StringComparison.OrdinalIgnoreCase);
        });
    }

    private static bool IsKeywordMatch(KeywordConfig cfg, string message) =>
        !string.IsNullOrWhiteSpace(cfg.KeywordContent) && cfg.KeywordType switch
        {
            KeywordType.Contains => ContainsMatch(cfg.KeywordContent, message, cfg.IsCaseSensitive),
            KeywordType.Regex => RegexMatch(cfg.KeywordContent, message, cfg.IsCaseSensitive),
            KeywordType.Fuzzy => FuzzyMatch(cfg.KeywordContent, message, cfg.IsCaseSensitive),
            KeywordType.FullWord => FullWordMatch(cfg.KeywordContent, message, cfg.IsCaseSensitive),
            _ => false
        };

    private static bool ContainsMatch(string kw, string msg, bool cs) =>
        cs ? msg.Contains(kw) : msg.Contains(kw, StringComparison.OrdinalIgnoreCase);

    private static bool RegexMatch(string pattern, string msg, bool cs)
    {
        try
        {
            var opt = cs ? RegexOptions.None : RegexOptions.IgnoreCase;
            return Regex.IsMatch(msg, pattern, opt);
        }
        catch (ArgumentException) { return false; }
    }

    private static bool FuzzyMatch(string kw, string msg, bool cs)
    {
        var parts = kw.Split('?', StringSplitOptions.RemoveEmptyEntries)
                      .Select(p => p.Trim())
                      .Where(p => p.Length > 0)
                      .ToArray();
        if (parts.Length == 0) return false;

        if (!cs) msg = msg.ToLowerInvariant();

        return parts.All(p =>
        {
            var target = cs ? p : p.ToLowerInvariant();
            return msg.Contains(target);
        });
    }

    private static bool FullWordMatch(string kw, string msg, bool cs) =>
        cs ? msg == kw : string.Equals(msg, kw, StringComparison.OrdinalIgnoreCase);
}