using System;
using System.Collections.Generic;

namespace AtCommon.Dtos.BusinessOutcomes
{
    public class BusinessOutcomeChecklistItemResponse : BaseBusinessOutcomesDto
    {
        public int Id { get; set; }
        public Guid BusinessOutcomeId { get; set; }
        public string ItemText { get; set; }
        public bool IsComplete { get; set; }
        public IEnumerable<BusinessOutcomeChecklistOwnerResponse> Owners { get; set; }
        public DateTime? TargetDate { get; set; }
        public int SortOrder { get; set; }
    }

    public class BusinessOutcomeChecklistOwnerResponse
    {
        public string DisplayName { get; set; }
        public string UserId { get; set; }
    }
}
