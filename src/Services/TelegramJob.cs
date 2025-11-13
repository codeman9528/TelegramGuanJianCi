using Furion.Shapeless;

namespace TelegramMonitor;

[JobDetail("telegram-job", Description = "Telegram账号活跃检查", GroupName = "monitor", Concurrent = true)]
[PeriodSeconds(60, TriggerId = "telegram-trigger", Description = "每分钟检查一下账号活跃度", RunOnStart = false)]
public class TelegramJob : IJob
{
    private readonly ILogger<TelegramJob> _logger;
    private readonly TelegramClientManager _clientManager;

    public TelegramJob(
        ILogger<TelegramJob> logger,
        TelegramClientManager clientManager)
    {
        _logger = logger;
        _clientManager = clientManager;
    }

    public async Task ExecuteAsync(JobExecutingContext context, CancellationToken stoppingToken)
    {
        try
        {
            if (!_clientManager.IsLoggedIn)
            {
                _logger.LogWarning("Telegram账号未登录或已断开连接，尝试重新连接");

                var phone = _clientManager.GetPhone;

                if (string.IsNullOrEmpty(phone))
                {
                    _logger.LogError("没有保存的电话号码，无法自动重连");
                    return;
                }

                var loginResult = await _clientManager.ConnectAsync(phone);

                if (loginResult == LoginState.LoggedIn)
                {
                    _logger.LogInformation("Telegram账号重新连接成功");

                    if (_clientManager.IsMonitoring)
                    {
                        await _clientManager.StartTaskAsync();
                        _logger.LogInformation("已重新启动监控任务");
                    }
                }
                else
                {
                    _logger.LogError($"Telegram账号重新连接失败，状态：{loginResult}");
                }
            }
            else
            {
                _logger.LogDebug("Telegram账号连接正常");
            }
        }
        catch (Exception e)
        {
            _logger.LogError($"账号检查失败：{e.Message}");
        }
    }
}