using System;
using System.Collections.Generic;

namespace AtCommon.Dtos.Teams
{
    public class TeamsWithMembersAndSurveysResponse
    {
        public int CompanyId { get; set; }
        public Guid TeamUid { get; set; }
        public int TeamId { get; set; }
        public string Name { get; set; }
        public List<Survey> Surveys { get; set; }
        public List<TeamMemberResponse> TeamMembers { get; set; }
    }

    public class Survey
    {
        public string Name { get; set; }
        public int SurveyId { get; set; }
    }
}
