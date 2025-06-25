using Newtonsoft.Json;
using System;

namespace AtCommon.Dtos.BusinessOutcomes
{
    public class BusinessOutcomeTagResponse
    {
        public Guid Uid { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public Guid CategoryLabelUid { get; set; }
        public string CategoryLabelName { get; set; }
        public string Color { get; set; }
    }

}