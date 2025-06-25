using System.Collections.Generic;

namespace AtCommon.Dtos.CampaignsV2
{
     public class MatchmakingResponse
    {
        public int CampaignId { get; set; }
        public Dictionary<string, string> TeamFacilitatorMap { get; set; }
    }

     public class ErrorResponse
     {
         public List<string> CampaignId { get; set; }
         public List<string> TargetRatio { get; set; }
    }
}
