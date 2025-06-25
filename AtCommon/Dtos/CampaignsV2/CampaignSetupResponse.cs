using System.Collections.Generic;

namespace AtCommon.Dtos.CampaignsV2
{
    public class CampaignSetupResponse
    {
        public int CampaignId { get; set; }
        public List<int> SelectedTeams { get; set; }
        public List<string> SelectedFacilitators { get; set; }
        public Dictionary<string, string> TeamFacilitatorMap { get; set; }
        public string MatchmakingAssignmentsState { get; set; }
        public AssessmentSettings AssessmentSettings { get; set; }
    }
}
