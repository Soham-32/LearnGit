using System;

namespace AtCommon.Dtos.Ideaboard
{
    public class SetVotesAllowedRequests : BaseDto
    {
        public Guid AssessmentUid { get; set; }
        public int VotesAllowed { get; set; }
        public int CompanyId { get; set; }
    }
}