using System;

namespace AtCommon.Dtos.BusinessOutcomes.Custom
{
    public class Deliverable : BaseBusinessOutcomesDto
    {
        public string Title { get; set; }
        public bool IsComplete { get; set; }
        public string BusinessOutcomeId { get; set; }
        public DateTime CompletedBy { get; set; }
    }
}