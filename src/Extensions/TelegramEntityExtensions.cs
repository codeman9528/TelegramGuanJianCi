namespace TelegramMonitor;

public static class TelegramEntityExtensions
{
    public static string DisplayName(this User u)
        => $"{u.first_name}{u.last_name}".Trim();

    public static string JoinUsernames(this IEnumerable<string>? names)
        => names?.Any() == true
            ? string.Join(' ', names.Select(n => $"@{n}"))
            : string.Empty;
}