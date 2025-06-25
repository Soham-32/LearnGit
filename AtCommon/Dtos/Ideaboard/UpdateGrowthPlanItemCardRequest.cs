namespace AtCommon.Dtos.Ideaboard
{
    public class UpdateGrowthPlanItemCardRequest
    {
        public int ItemId { get; set; }
        public int? DimensionId { get; set; }
        public int GrowthPlanItemId { get; set; }
        public string GrowthPlanItemCategory { get; set; }
        public string ItemText { get; set; }
    }
}