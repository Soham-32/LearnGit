namespace AtCommon.Dtos.Analytics
{
    public class IndexAnalyticsRequest : AnalyticsRequest
    {
        public WidgetType WidgetType { get; set; }
        public string EndQuarter { get; set; }
    }
    public enum WidgetType
    {
        Maturity = 1,
        Performance = 2,
        Agility = 3,
        FalseTest = 4
    }
    public enum Stage
    {
        PreCrawl = 1,
        Crawl = 2,
        Walk = 3,
        Run = 4,
        Fly = 5,
        FalseTest = 6
    }
}
