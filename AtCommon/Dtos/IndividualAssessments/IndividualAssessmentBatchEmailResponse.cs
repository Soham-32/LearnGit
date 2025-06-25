using System;

namespace AtCommon.Dtos.IndividualAssessments
{
    public class IndividualAssessmentBatchEmailResponse
    {
        public Guid AssessmentUid { get; set; }
        public Guid MemberUid { get; set; }
        public string EmailType { get; set; }
        public DateTime EmailSentAt { get; set; }
    }
}
