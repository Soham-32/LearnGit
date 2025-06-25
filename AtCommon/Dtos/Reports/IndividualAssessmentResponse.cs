using AtCommon.Dtos.IndividualAssessments;
using System.Collections.Generic;

namespace AtCommon.Dtos.Reports
{
    public class IndividualAssessmentResponse
    {
        public int BatchId { get; set; }
        public string AssessmentName { get; set; } 
        public string TeamUid { get; set; }
        public bool Published { get; set; }
        public List<IndividualAssessmentMemberRequest> Participants { get; set; }
        public List<AssessmentList> AssessmentList { get; set; }
    }
}
