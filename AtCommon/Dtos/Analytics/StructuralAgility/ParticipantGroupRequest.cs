namespace AtCommon.Dtos.Analytics.StructuralAgility
{
    public class ParticipantGroupRequest : AnalyticsRequest
    {
        public override bool HydrateTeamIds => true;
        public StructuralAgilityWidgetType WidgetType { get; set; }
        public int Filter { get; set; }
    }
}
