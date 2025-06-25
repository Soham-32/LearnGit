namespace AtCommon.Dtos.Analytics.StructuralAgility
{
    public class PeopleByRoleRequest : BaseTeamIdentityAnalyticsModel
    {
        public override bool HydrateTeamIds => true;
        public StructuralAgilityWidgetType WidgetType { get; set; }
        public int WorkTypeId { get; set; }
    }
}
