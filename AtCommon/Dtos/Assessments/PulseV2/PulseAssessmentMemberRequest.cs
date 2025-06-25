using System;
using System.Collections.Generic;
using AtCommon.Dtos.IndividualAssessments;

namespace AtCommon.Dtos.Assessments.PulseV2
{
    public class PulseAssessmentMemberRequest
    {
        public Guid Uid { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public List<RoleRequest> Tags { get; set; }
    }
}
