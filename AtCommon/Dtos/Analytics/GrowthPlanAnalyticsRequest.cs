using System;
using AtCommon.Api.Enums;

namespace AtCommon.Dtos.Analytics
{
    public class GrowthPlanAnalyticsRequest : AnalyticsRequest
    {
        public override bool HydrateTeamIds => true;
        public GrowthItemCategory GrowthItemCategory { get; set; }
        public GrowthItemSegmentType SubDimensionId { get; set; }
        public GrowthItemStatusType GrowthItemStatusType { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

}
