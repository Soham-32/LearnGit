using System.Collections.Generic;

namespace AtCommon.Dtos.Radars
{
    public class DimensionDetailResponse
    {
        public int? DimensionId { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public int RadarOrder { get; set; }
        public int? SortOrder { get; set; }
        public string FontSize { get; set; }
        public string Direction { get; set; }
        public string LetterSpacing { get; set; }
        public string Font { get; set; }
        public bool Enabled { get; set; }
        public string Position { get; set; }
        public int LinkedAppId { get; set; }
        public int SourceAppId { get; set; }
        public IEnumerable<SubdimensionDetailResponse> Subdimensions { get; set; }
    }
}