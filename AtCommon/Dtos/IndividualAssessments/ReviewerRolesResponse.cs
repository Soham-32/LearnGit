using System;
using System.Collections.Generic;
using AtCommon.Dtos.Assessments;

namespace AtCommon.Dtos.IndividualAssessments
{
    public class ReviewerRolesResponse
    {
        public Guid AssessmentUid { get; set; }
        public Guid ReviewerUid { get; set; }
        public List<RoleResponse> RoleTags { get; set; }
    }
}
