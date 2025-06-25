using System.Collections.Generic;

namespace AtCommon.Dtos.CampaignsV2
{
    public class AddFacilitatorsToCampaignRequest
    {
        public int CampaignId { get; set; }
        public List<string> FacilitatorIds { get; set; }
    }
}