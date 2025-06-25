using System;

namespace AtCommon.Dtos.BusinessOutcomes
{
    public class BusinessOutcomeLinkResponse
    {
        public Guid BusinessOutcomeUId { get; set; }
        public string ExternalLink { get; set; }
        public bool IsActive { get; set; }
        public Guid LinkType { get; set; }
        public Guid Uid { get; set; }
        public string Title { get; set; }
    }
}