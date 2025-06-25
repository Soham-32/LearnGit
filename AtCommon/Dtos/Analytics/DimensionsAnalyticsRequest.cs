namespace AtCommon.Dtos.Analytics
{
    public class DimensionsAnalyticsRequest : AnalyticsRequest
    {
        public WidgetType WidgetType { get; set; }
        public DimensionSortOrder DimensionSortOrder { get; set; }
        public string EndQuarter { get; set; }
    }
}
