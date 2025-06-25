using System;
using System.Collections.Generic;

namespace AtCommon.Dtos.Assessments.PulseV2
{

    public class PulseAssessmentV2Response
    {
        public Guid PulseAssessmentUId { get; set; }
        public int TeamId { get; set; }
        public int SurveyId { get; set; }
        public string SurveyName { get; set; }
        public string Name { get; set; }
        public List<TeamV2Response> SelectedTeams{ get; set; }
        public PulseAssessmentRoleFilterRequest RoleFilter { get; set; }
        public bool IsPublished { get; set; }
        public bool IsPulseAssessmentClosed { get; set; }
        public DateTime? PublishedDate { get; set; }
        public int PeriodId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int RepeatIntervalId { get; set; }
        public int RepeatEndStrategyId { get; set; }
        public int RepeatOccurrenceNumber { get; set; }
        public bool IsStop { get; set; }
        public IEnumerable<PulseDimensionV2Response> PulseComponents { get; set; }
    }

    public class PulseDimensionV2Response
    {
        public bool Selected { get; set; }
        public int DimensionId { get; set; }
        public string Name { get; set; }
        public IEnumerable<PulseSubDimensionV2Response> SubDimensions { get; set; }
    }

    public class PulseSubDimensionV2Response
    {
        public bool Selected { get; set; }
        public int SubDimensionId { get; set; }
        public string Name { get; set; }
        public IEnumerable<PulseCompetencyV2Response> Competencies { get; set; }
    }

    public class PulseCompetencyV2Response
    {
        public bool Selected { get; set; }
        public int CompetencyId { get; set; }
        public string Name { get; set; }
        public IEnumerable<PulseQuestionV2Response> Questions { get; set; }
    }

    public class PulseQuestionV2Response
    {
        public bool Selected { get; set; }
        public int QuestionId { get; set; }
        public string Text { get; set; }
    }
}
