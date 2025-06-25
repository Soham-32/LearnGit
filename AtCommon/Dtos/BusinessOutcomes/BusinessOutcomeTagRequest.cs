using System;
using Newtonsoft.Json;

namespace AtCommon.Dtos.BusinessOutcomes
{
    public class BusinessOutcomeTagRequest
    {
        [JsonProperty("Uid")]
        public Guid Uid { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("CategoryLabelUid")]
        public Guid CategoryLabelUid { get; set; }

        [JsonProperty("CompanyId")]
        public int CompanyId { get; set; }

        [JsonProperty("Order")]
        public int Order { get; set; }

        [JsonProperty("Color")]
        public string Color { get; set; }

        public DateTime CreatedAt { get; set; } 
        public DateTime? DeletedAt { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}