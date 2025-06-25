using System;

namespace AtCommon.Dtos.Companies
{
    public class TeamAssessmentResponse
    {
        public string AssessmentName { get; set; }
        public int TeamId { get; set; }
        public string TeamName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? FacilitationDate { get; set; }
        public bool Active { get; set; }
        public string SurveyType { get; set; }
        public string SurveyName { get; set; }
        public int AssessmentId { get; set; }
        public int SurveyId { get; set; }
        public Guid Uid { get; set; }
        public int TeamMemberPercentageCompleted { get; set; }
        public int StakeholderPercentageCompleted { get; set; }
        public int? TeamMembersCount { get; set; }
        public int? StakeholdersCount { get; set; }
        public int? TeamMemberRespondents { get; set; }
        public int? StakeholderRespondents { get; set; }
        public double? AgilityHealthIndex { get; set; }
    }
}