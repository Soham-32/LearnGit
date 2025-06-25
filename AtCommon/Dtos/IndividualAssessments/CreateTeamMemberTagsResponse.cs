using System;
using System.Collections.Generic;
using AtCommon.Dtos.Assessments;

namespace AtCommon.Dtos.IndividualAssessments
{
    public class CreateTeamMemberTagsResponse
    {
        public Guid AssessmentUid { get; set; }
        public Guid ParticipantUid { get; set; }
        public List<RoleResponse> ParticipantGroups { get; set; }
    }
}
