using System.Collections.Generic;

namespace AtCommon.Dtos.Radars.Custom
{
    public class AnalyticsValue
    {
        public string CompetenciesName { get; set; }
        public string CompetenciesValue { get; set; }
    }

    public class AnalyticsSection
    {
        public string AnalyticsHeader { get; set; }
        public List<AnalyticsValue> AnalyticsValues { get; set; }
    }

    public class Analytics
    {
        public List<AnalyticsSection> AnalyticsSections { get; set; }
    }
}