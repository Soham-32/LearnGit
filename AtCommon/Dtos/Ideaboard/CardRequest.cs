using System.Collections.Generic;

namespace AtCommon.Dtos.Ideaboard
{
    public class CardRequest
    {
        public int ItemId { get; set; }
        public bool IsCardMovedToDifferentColumn { get; set; }
        public int? DimensionId { get; set; }
        public string ColumnName { get;set; }
        public int SortOrder { get; set; }
        public string ItemText { get; set; }
        public int VoteCount { get;set; }
        public Dictionary<string, int> Votes { get; set; }
    }
}