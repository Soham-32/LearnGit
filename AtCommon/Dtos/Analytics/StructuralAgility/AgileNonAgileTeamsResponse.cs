namespace AtCommon.Dtos.Analytics.StructuralAgility
{
    public class AgileNonAgileTeamsResponse
    {
        public string WidgetType { get; set; }
        public int? ChildTeamId { get; set; }
        public string ChildTeamName { get; set; }
        public string TeamCategorySelection { get; set; }
        public int? TeamCount { get; set; }
        public int CategoryRank { get; set; }
    }
}
