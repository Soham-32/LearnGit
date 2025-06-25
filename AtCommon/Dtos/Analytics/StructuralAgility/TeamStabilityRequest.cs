namespace AtCommon.Dtos.Analytics.StructuralAgility
{
    public class TeamStabilityRequest : BaseTeamIdentityAnalyticsModel
    {
        public override bool HydrateTeamIds => true;
        public StructuralAgilityWidgetType WidgetType { get; set; }
    }
}
