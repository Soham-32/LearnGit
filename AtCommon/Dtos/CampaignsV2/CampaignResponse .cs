using System;

namespace AtCommon.Dtos.CampaignsV2
{
    public class CampaignResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CompanyId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

    }
}
