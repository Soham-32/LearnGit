using System;

namespace AtCommon.Dtos.BusinessOutcomes
{
    public class BusinessOutcomesLinkRequest : BaseBusinessOutcomesDto
    {
        public Guid BusinessOutcomeUId { get; set; }
        public bool IsActive { get; set; }
        public string ExternalUrl { get; set; }
        public Guid LinkType { get; set; }
        public string Title { get; set; }
    }
}