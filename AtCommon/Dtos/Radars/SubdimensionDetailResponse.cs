using System.Collections.Generic;
namespace AtCommon.Dtos.Radars
{
    public class SubdimensionDetailResponse
    {
        public int SubdimensionId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Abbreviation { get; set; }
        public string Color { get; set; }
        public bool Enabled { get; set; }
        public string FontSize { get; set; }
        public string Font { get; set; }
        public string LetterSpacing { get; set; }
        public int RadarOrder { get; set; }
        public int LinkedAppId { get; set; }
        public string Direction { get; set; }
        public int SourceAppId { get; set; }
        public IEnumerable<CompetencyDetailResponse> Competencies { get; set; }

    }
}