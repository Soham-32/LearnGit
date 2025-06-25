using System;

namespace AtCommon.Dtos.Reports
{
    public class IndividualAssessmentGrowthPlanItemsSummaryResponse
    {
        public string TeamName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }   
        public string IndividualAssessmentId { get; set; }
        public int SurveyOriginalVersion { get; set; }
        public DateTime AssessmentEndDate { get; set; }
        public int CompanyId { get; set; }
        public int NumberOfRevieweesInvited { get; set; }
        public int NumberOfReviewersInvited { get; set; }
        public int NumberOfRevieweesCompleted { get; set; }
        public int NumberOfReviewersCompleted { get; set; }
        public int TotalNumberOfInvited { get; set; }
        public int TotalNumberOfCompleted { get; set; }
        public double PercentComplete { get; set; }
        public int HasGrowthPlanItems { get; set; }
        public int NumberOfGrowthPlanItems { get; set; }
        public string TopThreeHighestCompetencies { get; set; }
        public string TopThreeLowestCompetencies { get; set; }
    }
}
