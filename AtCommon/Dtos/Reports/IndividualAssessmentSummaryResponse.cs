using System.Collections.Generic;

namespace AtCommon.Dtos.Reports
{
    public class IndividualAssessmentSummaryResponse
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int RevieweesInvitedCount { get; set; }
        public int ReviewersInvitedCount { get; set; }
        public int RevieweesCompletedCount { get; set; }
        public int ReviewersCompletedCount { get; set; }
        public int InvitedTotal { get; set; }
        public int CompletedTotal { get; set; }
        public int PercentComplete { get; set; }
        public bool HasGrowthPlanItems { get; set; }
        public int GrowthPlanItemsCount { get; set; }
        public List<string> TopThreeHighestCompetencies { get; set; }
        public List<string> TopThreeLowestCompetencies { get; set; }

    }
}
