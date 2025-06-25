
namespace AtCommon.Dtos.Analytics.StructuralAgility
{
    public class AgileNonAgileTeamsRequest : BaseTeamIdentityAnalyticsModel
    {
        public StructuralAgilityWidgetType WidgetType { get; set; }
        public string SelectedTeamCategoryName { get; set; }
    }
}
