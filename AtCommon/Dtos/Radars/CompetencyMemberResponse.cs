
using System.Collections.Generic;

namespace AtCommon.Dtos.Radars
{
    public class CompetencyMemberResponse
    {
        public int CompetencyId { get; set; }
        public string Name { get; set; }
        public string Exclude { get; set; }
        public int RadarOrder { get; set; }
        public bool IsExcluded { get; set; }
        public bool ExcludeQuestions { get; set; }
        public List<QuestionMemberResponse> Questions { get; set; }
    }
}
