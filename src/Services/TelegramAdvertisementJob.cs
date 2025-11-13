using Furion.Shapeless;

namespace TelegramMonitor;

[JobDetail("telegram-advertisement-job", Description = "telegram-advertisement-job", GroupName = "monitor", Concurrent = true)]
[PeriodMinutes(30, TriggerId = "telegram-ad-monitor-trigger", Description = "每30分钟执行一次的任务", RunOnStart = true)]
public class TelegramAdvertisementJob : IJob
{
    private readonly ILogger<TelegramAdvertisementJob> _logger;
    private readonly IHttpRemoteService _httpRemoteService;
    private readonly SystemCacheServices _systemCacheServices;

    public TelegramAdvertisementJob(ILogger<TelegramAdvertisementJob> logger, IHttpRemoteService httpRemoteService, SystemCacheServices systemCacheServices)
    {
        _logger = logger;
        _httpRemoteService = httpRemoteService;
        _systemCacheServices = systemCacheServices;
    }

    public async Task ExecuteAsync(JobExecutingContext context, CancellationToken stoppingToken)
    {
        try
        {
            var content = await _httpRemoteService.GetAsAsync<string>(TelegramMonitorConstants.MonitorApi);
            var lines = content
                .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(line => line.Trim())
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .ToList();
            _systemCacheServices.SetAdvertisement(lines);
        }
        catch (Exception e)
        {
            _logger.LogError($"获取广告数据失败：{e.Message}");
        }
    }
}