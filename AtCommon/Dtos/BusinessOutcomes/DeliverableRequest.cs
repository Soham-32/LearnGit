using System;

namespace AtCommon.Dtos.BusinessOutcomes
{
    public class DeliverableRequest : BaseBusinessOutcomesDto
    {
        public string Title { get; set; }
        public bool IsComplete { get; set; }
        public Guid BusinessOutcomeId { get; set; }
        public DateTime? CompletedBy { get; set; }
    }
}