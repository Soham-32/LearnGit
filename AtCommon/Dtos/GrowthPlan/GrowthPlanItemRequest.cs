using System;
using System.Collections.Generic;

namespace AtCommon.Dtos.GrowthPlan
{
    public class GrowthPlanItemRequest 
    {
        public int CompanyId { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public int Id { get; set; }
        public string Owner { get; set; }
        public string Status { get; set; }
        public int StatusId { get; set; }
        public int SurveyId { get; set; }
        public string Priority { get; set; }
        public DateTime? TargetDate { get; set; }
        public int TeamId { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public IEnumerable<int> CompetencyTargets { get; set; }
    }
}