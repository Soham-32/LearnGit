using System;
using System.Collections.Generic;

namespace AtCommon.Dtos.Insights
{
    public class DashboardResponse
    {
        public DashboardResponse()
        {
            Widgets = new List<WidgetResponse>();
        }

        public Guid Uid { get; set; }
        public string Title { get; set; }
        public IList<WidgetResponse> Widgets { get; set; }
    }
}
