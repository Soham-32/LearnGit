using System;
using AtCommon.Api.Enums;

namespace AtCommon.Dtos.Analytics.Custom
{
    public class GrowthItem
    {
        public string Type { get; set; }
        public GrowthItemCategory Category { get; set; }
        public string Status { get; set; }
        public string Title { get; set; }
        public string Priority { get; set; }
        public string Description { get; set; }
        public string Team { get; set; }
        public string CompetencyTarget { get; set; }
        public string RadarType { get; set; }
        public DateTime Created { get; set; }
    }
}