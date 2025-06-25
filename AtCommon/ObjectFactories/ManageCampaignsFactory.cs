using AtCommon.Dtos.Settings;
using AtCommon.Utilities;
using System;

namespace AtCommon.ObjectFactories
{
    public static class ManageCampaignsFactory
    {
        public static Campaign GetCompaignData()
        {
            return new Campaign
            {
                Name = "Campaign" + RandomDataUtil.GetAssessmentName(),
                Description = "Test Description",
                SatrtDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1)
            };
        }
    }
}

