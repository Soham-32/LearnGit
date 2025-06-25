using System;

namespace AtCommon.Dtos.Analytics
{
    public class OvertimeAnalyticsResponse : BenchmarkingResponse
    {
        public DateTime DateKey { get; set; }
        public string QuarterName { get; set; }
    }
}
