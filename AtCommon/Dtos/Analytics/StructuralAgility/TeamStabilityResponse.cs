namespace AtCommon.Dtos.Analytics.StructuralAgility
{
    public class TeamStabilityResponse
    {
        public string WidgetType { get; set; }
        public int TeamId { get; set; }
        public string TeamName { get; set; }
        public string TeamType { get; set; }
        public int TeamCount { get; set; }
        public int OriginalMemberCount { get; set; }
        public int AddedMemberCount { get; set; }
        public int RemovedMemberCount { get; set; }
        public decimal TeamStabilityPercentage { get; set; }
        public int StabilityTarget { get; set; }
    }
}
