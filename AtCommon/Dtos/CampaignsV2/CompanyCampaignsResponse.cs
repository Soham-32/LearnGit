using System.Collections.Generic;

namespace AtCommon.Dtos.CampaignsV2
{
    public class CompanyCampaignsResponse
    {
        //public CompanyCampaignsResponse()
        //{
        //    Campaigns = new List<CampaignResponse>();
        //}
        public int CompanyId { get; set; }
        public ICollection<CampaignResponse> Campaigns { get; set; }
    }
}