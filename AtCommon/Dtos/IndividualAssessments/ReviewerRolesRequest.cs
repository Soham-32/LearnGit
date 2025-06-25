using System;
using System.Collections.Generic;

namespace AtCommon.Dtos.IndividualAssessments
{
    public class ReviewerRolesRequest
    {
        public Guid AssessmentUid { get; set; }
        public Guid ReviewerUid { get; set; }
        public List<RoleRequest> RoleTags { get; set; }
    }
}
