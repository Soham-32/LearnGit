using System.Collections.Generic;

namespace AtCommon.Dtos.Ideaboard
{
    public class CardResponse
    {
        public int ItemId { get; set; }
        public int? DimensionId { get; set; }
        public string ColumnName { get; set; } 
        public int? SortOrder { get; set; }
        public string ItemText {get; set; }
        public int VoteCount { get; set; }
        public int? GrowthPlanItemId { get; set; }
        public string GrowthPlanItemCategory { get;set; }
        public Dictionary<string, int> Votes { get; set; }
    }
}