using System;
using System.Collections.Generic;
using AtCommon.Dtos.IndividualAssessments;

namespace AtCommon.Dtos.Assessments.PulseV2
{
    public class SavePulseAssessmentV2Request
    {
        public int TeamId { get; set; }
        public int SurveyId { get; set; }
        public string SurveyName { get; set; }
        public int PulseAssessmentId { get; set; }
        public int TemplatePulseAssessmentId { get; set; }
        public string Name { get; set; }
        public bool IsTemplate { get; set; }
        public List<PulseAssessmentTeamRequest> SelectedTeams { get; set; }
        public PulseAssessmentRoleFilterRequest RoleFilter { get; set; }
        public bool IsPublished { get; set; }
        public DateTime? PublishedDate { get; set; }
        public int PeriodId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int RepeatIntervalId { get; set; }
        public int RepeatEndStrategyId { get; set; }
        public int RepeatOccurrenceNumber { get; set; }
        public List<int?> DimensionIds { get; set; }
        public IEnumerable<int> SubDimensionIds { get; set; }
        public List<PulseCompetencyV2> Competencies { get; set; }
    }

    public class PulseAssessmentTeamRequest
    {
        public Guid TeamUid { get; set; }
        public int TeamId { get; set; }
        public string Name { get; set; }
        public List<PulseAssessmentMemberRequest> SelectedParticipants { get; set; }
    }

    public class PulseAssessmentRoleFilterRequest
    {
        public List<RoleRequest> Tags { get; set; }
    }

    public class PulseCompetencyV2
    {
        public int CompetencyId { get; set; }
        public IEnumerable<int> QuestionIds { get; set; }
    }
}
