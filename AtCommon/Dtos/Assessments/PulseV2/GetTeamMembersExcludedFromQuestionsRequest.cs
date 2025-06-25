using System.Collections.Generic;

namespace AtCommon.Dtos.Assessments.PulseV2
{
    public class GetTeamMembersExcludedFromQuestionsRequest
    {
        public int TeamId { get; set; }
        public int SurveyId { get; set; }
        public List<int> CompetencyIds { get; set; }
    }
}
