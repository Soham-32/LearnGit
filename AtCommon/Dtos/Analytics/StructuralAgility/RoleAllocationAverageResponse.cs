namespace AtCommon.Dtos.Analytics.StructuralAgility
{
    public class RoleAllocationAverageResponse
    {
        public string WorkType { get; set; }
        public string MemberRole { get; set; }
        public string MemberName { get; set; }
        public int TeamsSupportedCount { get; set; }
        public int TeamsSupportedTarget { get; set; }
    }
}