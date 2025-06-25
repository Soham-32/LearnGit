using System.Collections.Generic;
namespace AtCommon.Dtos.Radars
{
    public class SubdimensionMemberResponse
    {
        public int SubdimensionId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Abbreviation { get; set; }
        public string Color { get; set; }
        public bool Enabled { get; set; }
        public int RadarOrder { get; set; }
        public bool IsExcluded { get; set; }
        public bool IsFinal { get; set; }
        public List<CompetencyMemberResponse> Competencies { get; set; }
    }
}
