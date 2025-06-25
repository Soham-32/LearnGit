using System.Collections.Generic;
namespace AtCommon.Dtos.Radars
{
    public class CompetencyDetailResponse
    {
        public int CompetencyId { get; set; }
        public string Name { get; set; }
        public string Abbreviation { get; set; }
        public string Exclude { get; set; }
        public bool External { get; set; }
        public string BiAbbreviation { get; set; }
        public string FontSize { get; set; }
        public string Font { get; set; }
        public string LetterSpacing { get; set; }
        public int RadarOrder { get; set; }
        public int LinkedAppId { get; set; }
        public string Direction { get; set; }
        public int SourceAppId { get; set; }
        public IEnumerable<QuestionDetailResponse> Questions { get; set; }

    }
}