using System;

namespace AtCommon.Dtos.Insights
{
    public class UpdateUserWidgetPreferenceRequest
    {
        public Guid DashboardWidgetUid { get; set; }
        public string Preference { get; set; }
    }
}
