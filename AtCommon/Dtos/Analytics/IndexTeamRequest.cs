namespace AtCommon.Dtos.Analytics
{
    public class IndexTeamRequest : BaseTeamIdentityAnalyticsPageableModel
    {
        public WidgetType WidgetType { get; set; }
        public Stage Stage { get; set; }
        public int? RadarType { get; set; }
        public string EndQuarter { get; set; }
    }
}
