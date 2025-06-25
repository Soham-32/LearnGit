using System.Collections.Generic;

namespace AtCommon.Dtos.CampaignsV2
{
     public class MatchmakingRequest
    {
        public int? CampaignId { get; set; }
        public List<int> TeamIds { get; set; }
        public List<string> FacilitatorIds { get; set; }
        public int? TargetRatio { get; set; }
        public string MatchmakingType { get; set; }
    }
}
