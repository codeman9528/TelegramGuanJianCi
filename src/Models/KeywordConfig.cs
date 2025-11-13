namespace TelegramMonitor
{
    [SugarTable("KeywordConfig")]
    public class KeywordConfig
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        [Description("关键词配置ID")]
        public int Id { get; set; }

        [SugarColumn(ColumnName = "KeywordContent")]
        [Description("关键词内容")]
        public string KeywordContent { get; set; } = string.Empty;

        [SugarColumn(ColumnName = "KeywordType")]
        [Description("关键词匹配类型")]
        public KeywordType KeywordType { get; set; } = KeywordType.Contains;

        [SugarColumn(ColumnName = "KeywordAction")]
        [Description("关键词执行动作")]
        public KeywordAction KeywordAction { get; set; } = KeywordAction.Monitor;

        [SugarColumn(ColumnName = "IsCaseSensitive")]
        [Description("是否区分大小写")]
        public bool IsCaseSensitive { get; set; } = false;

        [SugarColumn(ColumnName = "IsBold")]
        [Description("是否粗体")]
        public bool IsBold { get; set; } = false;

        [SugarColumn(ColumnName = "IsItalic")]
        [Description("是否斜体")]
        public bool IsItalic { get; set; } = false;

        [SugarColumn(ColumnName = "IsUnderline")]
        [Description("是否下划线")]
        public bool IsUnderline { get; set; } = false;

        [SugarColumn(ColumnName = "IsStrikeThrough")]
        [Description("是否删除线")]
        public bool IsStrikeThrough { get; set; } = false;

        [SugarColumn(ColumnName = "IsQuote")]
        [Description("是否引用样式")]
        public bool IsQuote { get; set; } = false;

        [SugarColumn(ColumnName = "IsMonospace")]
        [Description("是否等宽字体")]
        public bool IsMonospace { get; set; } = false;

        [SugarColumn(ColumnName = "IsSpoiler")]
        [Description("是否剧透内容")]
        public bool IsSpoiler { get; set; } = false;
    }

    public enum KeywordType
    {
        [Description("全字匹配")]
        FullWord = 0,

        [Description("包含指定文本")]
        Contains = 1,

        [Description("使用正则表达式匹配")]
        Regex = 2,

        [Description("模糊匹配多个关键词(以?分隔)")]
        Fuzzy = 3,

        [Description("匹配特定用户")]
        User = 4
    }

    public enum KeywordAction
    {
        [Description("排除")]
        Exclude = 0,

        [Description("监控")]
        Monitor = 1
    }
}