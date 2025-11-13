namespace TelegramMonitor;

[ApiDescriptionSettings(Tag = "telegram", Description = "Telegram 控制接口")]
public class TelegramService : IDynamicApiController, ITransient
{
    private readonly TelegramClientManager _clientManager;

    public TelegramService(TelegramClientManager clientManager)
    {
        _clientManager = clientManager;
    }

    [HttpPost("login")]
    public Task<LoginState> Login([FromBody] LoginRequest req)
        => _clientManager.LoginAsync(req.PhoneNumber, req.LoginInfo);

    [HttpPost("proxy")]
    public async Task<LoginState> Proxy([FromBody] ProxyRequest req)
    {
        bool wasMonitoring = _clientManager.IsMonitoring;

        if (wasMonitoring)
        {
            await _clientManager.StopTaskAsync();
        }

        var loginState = await _clientManager.SetProxyAsync(req.Type, req.Url);

        if (loginState == LoginState.LoggedIn && wasMonitoring)
        {
            await _clientManager.StartTaskAsync();
        }

        return loginState;
    }

    [HttpGet("status")]
    public TgStatus Status()
        => new(_clientManager.IsLoggedIn, _clientManager.IsMonitoring);

    [HttpGet("dialogs")]
    public async Task<List<DisplayDialogs>> Dialogs()
    {
        if (!_clientManager.IsLoggedIn)
        {
            throw Oops.Oh("未登录");
        }
        return await _clientManager.DialogsAsync();
    }

    [HttpPost("target")]
    public void Target([FromBody] long id)
    {
        if (!_clientManager.IsLoggedIn) throw Oops.Oh("未登录");
        _clientManager.SetSendChatId(id);
    }

    [HttpPost("start")]
    public Task<MonitorStartResult> Start()
    {
        return _clientManager.StartTaskAsync();
    }

    [HttpPost("stop")]
    public async void Stop()
    {
        if (!_clientManager.IsLoggedIn) throw Oops.Oh("未登录");
        await _clientManager.StopTaskAsync();
    }
}

public record LoginRequest(string PhoneNumber, string LoginInfo);
public record ProxyRequest(ProxyType Type, string Url);
public record TgStatus(bool LoggedIn, bool Monitoring);