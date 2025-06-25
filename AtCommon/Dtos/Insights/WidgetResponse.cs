using System;

namespace AtCommon.Dtos.Insights
{
    public class WidgetResponse
    {
        public Guid Uid { get; set; }
        public Guid WidgetUid { get; set; }
        public string Type { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string Tooltip { get; set; }
        public string LearnMoreUri { get; set; }
        public short DefaultWidth { get; set; }
        public short DefaultHeight { get; set; }
        public short DefaultRow { get; set; }
        public short DefaultColumn { get; set; }
        public string Preferences { get; set; }
    }
}
