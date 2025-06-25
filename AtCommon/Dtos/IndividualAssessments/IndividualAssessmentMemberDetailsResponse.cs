using System;
using System.Collections.Generic;
using AtCommon.Dtos.Assessments;
using AtCommon.Dtos.Teams;

namespace AtCommon.Dtos.IndividualAssessments
{
    public class IndividualAssessmentMemberDetailsResponse : MemberResponse
    {
        public Guid AssessmentUid { get; set; }
        public string ViewAssessmentResultsUrl { get; set; }
        public string TakeAssessmentUrl { get; set; }
        public DateTime? AssessmentCompleteDateTime { get; set;  }
        public List<TagResponse> Tags { get; set; }
        public List<RoleResponse> RoleTags { get; set; }
        public List<RoleResponse> ParticipantGroups { get; set; }
        public List<ReviewerResponse> Reviewers { get; set; }
    }
}