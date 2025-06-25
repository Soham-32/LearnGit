using System.Collections.Generic;
using System;
using System.Text.Json.Serialization;

namespace AtCommon.Dtos.BusinessOutcomes
{
    public class BusinessOutcomeChecklistItemRequest
    {
        public BusinessOutcomeChecklistItemRequest()
        {
            Owners = new List<string>();
        }
        public int Id { get; set; }
        [JsonIgnore]
        public Guid BusinessOutcomeId { get; set; }
        public string ItemText { get; set; }
        public bool IsComplete { get; set; }
        [JsonIgnore]
        public int CompanyId { get; set; }
        public DateTime? TargetDate { get; set; }
        public IEnumerable<string> Owners { get; set; }
        public int SortOrder { get; set; }

        public BusinessOutcomeChecklistItemResponse ToResponse()
        {
            return new BusinessOutcomeChecklistItemResponse
            {
                BusinessOutcomeId = this.BusinessOutcomeId,
                Id = this.Id,
                IsComplete = this.IsComplete,
                ItemText = this.ItemText,
                TargetDate = this.TargetDate

            };
        }
    }
}

    
