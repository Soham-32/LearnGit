using System;
using AtCommon.Api.Enums;

namespace AtCommon.Dtos.Analytics
{
    public class GrowthPlanDetailsAnalyticsRequest : BaseTeamIdentityAnalyticsPageableModel
    {
        public override bool HydrateTeamIds => true;
        public int? RadarType { get; set; }
        public int? BenchmarkType { get; set; }
        public GrowthItemCategory GrowthItemCategory { get; set; }
        public int? SubDimensionId { get; set; }
        public GrowthItemStatusType GrowthItemStatusType { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string SelectedName { get; set; }
        public GrowthItemDetailStatusType SelectedStatusId { get; set; }
        public bool NotPaged { get; set; }
    }
}