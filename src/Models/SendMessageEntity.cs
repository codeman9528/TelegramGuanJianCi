namespace TelegramMonitor;

public class SendMessageEntity
{
    public long SendId { get; set; }
    public string SendTitle { get; set; }
    public IEnumerable<string> SendUserNames { get; set; }
    public long FromId { get; set; }
    public string FromTitle { get; set; }
    public IEnumerable<string> FromUserNames { get; set; }
    public string FromMainUserName { get; set; }
}