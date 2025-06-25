using System.Collections.Generic;

namespace AtCommon.Dtos.Analytics
{
    public class AnalyticsRequest : BaseTeamIdentityAnalyticsModel
    {
        public int? RadarType { get; set; }
        public int? BenchmarkType { get; set; }
        public IList<int> Tags { get; set; }
    }
}
