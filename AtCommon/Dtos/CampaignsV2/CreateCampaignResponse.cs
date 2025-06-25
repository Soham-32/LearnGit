using System;
using System.Collections.Generic;

namespace AtCommon.Dtos.CampaignsV2
{
    public class CreateCampaignResponse
    {
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string MatchMakingStrategy { get; set; }
        public int MaximumFacilitatorTeamAssignments { get; set; }
        public string CreateAssessment { get; set; }
        public int Id { get; set; }
        public int SurveyId { get; set; }
        public string Status { get; set; }
    }

    public class ErrorMessage
    {
        public List<string> EndDate { get; set; }
        public List<string> StartDate { get; set; }
    }
}