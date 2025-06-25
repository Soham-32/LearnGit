using System;
using System.Collections.Generic;

namespace AtCommon.Dtos.IndividualAssessments
{

    public class CreateTeamMemberTagsRequest 
    {
        public Guid AssessmentUid { get; set; }
        public Guid ParticipantUid { get; set; }
        public List<RoleRequest> ParticipantGroups { get; set; }
    }

}
