using System;

namespace AtCommon.Dtos.IndividualAssessments
{
    public class AssessmentList
    {
        public int IndividualAssessmentId { get; set; }
        public Guid AssessmentUid { get; set; }
        public int AssessmentId { get; set; }
        public Guid MemberId { get; set; }
    }
}
