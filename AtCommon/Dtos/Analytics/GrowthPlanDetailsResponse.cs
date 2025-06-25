using System;

namespace AtCommon.Dtos.Analytics
{
    public class GrowthPlanDetailsResponse
    {
        public string Title { get; set; }
        public string Type { get; set; }
        public string Team { get; set; }
        public int TeamId { get; set; }
        public string RadarType { get; set; }
        public string Description { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string Priority { get; set; }
        public string CreatedBy { get; set; }
        public string Status { get; set; }
    }
}