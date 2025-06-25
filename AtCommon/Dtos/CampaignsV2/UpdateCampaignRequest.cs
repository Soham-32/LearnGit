using System;
namespace AtCommon.Dtos.CampaignsV2
{
    public class UpdateCampaignRequest
    {
        public int CampaignId { get; set; }
        public string Name { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string MatchMakingStrategy { get; set; }
        public int MaximumFacilitatorTeamAssignments { get; set; }
        public string CreateAssessment { get; set; }
        public int SurveyId { get; set; }
        public string Status { get; set; }
    }
}
