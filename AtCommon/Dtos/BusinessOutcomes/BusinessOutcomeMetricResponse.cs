using System;

namespace AtCommon.Dtos.BusinessOutcomes
{
    public class BusinessOutcomeMetricResponse
    {
        public DateTime? CreatedAt { get; set; }
        public int Order { get; set; }
        public int Uid { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int TypeId { get; set; }
        
    }
}