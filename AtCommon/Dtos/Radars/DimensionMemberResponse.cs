using System.Collections.Generic;
namespace AtCommon.Dtos.Radars
{
    public class DimensionMemberResponse
    {
        public int DimensionId { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public int RadarOrder { get; set; }
        public int SortOrder { get; set; }
        public string FontSize { get; set; }
        public bool IsExcluded { get; set; }
        public bool SkipForTestDrive { get; set; }
        public bool IsFinish { get; set; }
        public List<SubdimensionMemberResponse> Subdimensions { get; set; }
    }
}
