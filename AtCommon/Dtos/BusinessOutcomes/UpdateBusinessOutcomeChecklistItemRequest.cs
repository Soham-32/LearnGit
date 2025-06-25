using System;
using System.Collections.Generic;

namespace AtCommon.Dtos.BusinessOutcomes
{

    public class UpdateBusinessOutcomeChecklistItemRequest : BaseBusinessOutcomesDto
    {
        public int Id { get; set; }
        public Guid BusinessOutcomeId { get; set; }
        public string ItemText { get; set; }
        public bool IsComplete { get; set; }
        public int CompanyId { get; set; }
        public DateTime TargetDate { get; set; }
        public List<string> Owners { get; set; }
        public int SortOrder { get; set; }
    }
}
