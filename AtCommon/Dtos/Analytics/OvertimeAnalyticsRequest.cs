namespace AtCommon.Dtos.Analytics
{
    public class OvertimeAnalyticsRequest : AnalyticsRequest
    {
        public WidgetType WidgetType { get; set; }
        public string EndQuarter { get; set; }
        public int? NumberOfQuarters { get; set; }
    }

}
