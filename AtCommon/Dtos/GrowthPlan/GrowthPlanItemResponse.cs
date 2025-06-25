using System;
using System.Collections.Generic;

namespace AtCommon.Dtos.GrowthPlan
{
    public class GrowthPlanItemResponse
    {
        public int Id { get; set; }
        public int? CompanyId { get; set; }
        public int? TeamId { get; set; }
        public string Title { get; set; }
        public string Priority { get; set; }
        public int PriorityId { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public string Category { get; set; }
        public string PlanType { get; set; }
        public string AssessmentName { get; set; }
        public string TeamName { get; set; }
        public string Origination { get; set; }
        public string CompetencyName { get; set; }
        public string RadarType { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Owner { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? TargetDate { get; set; }
        public DateTime? Completed { get; set; }
        public string Size { get; set; }
        public string AffectedTeamsName { get; set; }
        public string TeamTagName { get; set; }
        public string Description { get; set; }
        public int? SurveyId { get; set; }
        public string SurveyName { get; set; }
        public int? ReportingContainerId { get; set; }
        public string ExternalIdentifier { get; set; }
        public List<GrowthPlanItemStatusResponse> Statuses { get; set; }
    }
}