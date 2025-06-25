namespace AtCommon.Dtos.Ideaboard
{
    public class GrowthPlanItemCardRequest
    {
        public int ItemId { get; set; }
        public int? DimensionId { get; set; }
        public string GrowthPlanItemCategory { get; set; }
        public string ItemText { get; set; }
    }
}