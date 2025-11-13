namespace TelegramMonitor;

public static class TelegramSetup
{
    public static IServiceCollection AddTelegram(this IServiceCollection services)
    {
        var logDirectory = Path.Combine(AppContext.BaseDirectory, "logs");
        Directory.CreateDirectory(logDirectory);

        if (!Directory.Exists(TelegramMonitorConstants.SessionPath))
            Directory.CreateDirectory(TelegramMonitorConstants.SessionPath);

        var logLock = new object();
        DateTime currentDate = DateTime.Today;
        StreamWriter telegramLogs = CreateWriterForDate(currentDate);

        WTelegram.Helpers.Log = (lvl, str) =>
        {
            var now = DateTime.Now;
            lock (logLock)
            {
                if (now.Date != currentDate)
                {
                    telegramLogs.Dispose();
                    currentDate = now.Date;
                    telegramLogs = CreateWriterForDate(currentDate);
                }

                telegramLogs.WriteLine($"{now:yyyy-MM-dd HH:mm:ss} [{"TDIWE!"[lvl]}] {str}");
            }
        };

        services.AddSingleton<TelegramClientManager>();

        return services;

        static StreamWriter CreateWriterForDate(DateTime date)
        {
            string logPath = Path.Combine(AppContext.BaseDirectory, "logs", $"{date:yyyy-MM-dd}_Telegram.log");
            return new StreamWriter(logPath, true, Encoding.UTF8) { AutoFlush = true };
        }
    }
}