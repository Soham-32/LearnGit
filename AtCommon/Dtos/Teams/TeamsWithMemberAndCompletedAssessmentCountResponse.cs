using System;
using System.Collections.Generic;

namespace AtCommon.Dtos.Teams
{
    public class TeamsWithMemberAndCompletedAssessmentCountResponse
    {
        public int CompanyId { get; set; }
        public Guid TeamUid { get; set; }
        public int TeamId { get; set; }
        public string Name { get; set; }
        public List<CompletedSurvey> SurveysForCompletedAssessments { get; set; }
        public int TeamMemberCount { get; set; }
        public List<TeamMemberResponse> TeamMembers { get; set; }
    }

    public class CompletedSurvey
    {
        public string Name { get; set; }
        public int Id { get; set; }
    }
}
