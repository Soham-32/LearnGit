using System.Collections.Generic;

namespace AtCommon.Dtos.IndividualAssessments
{
    public class EditIndividualAssessmentRequest
    {
        public List<IndividualAssessmentMemberRequest> Members { get; set; }
    }
}
