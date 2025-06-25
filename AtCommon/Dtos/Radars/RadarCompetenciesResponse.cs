using System;

namespace AtCommon.Dtos.Radars
{
    public class RadarCompetenciesResponse
    {
        public int Id { get; set; }
        public int SurveyId { get; set; }
        public string Name { get; set; }
        public int SubDimensionId { get; set; }
        public string SubDimensionName { get; set; }
        public int OriginalCompetencyId { get; set; }
        public Guid UId { get; set; }
        public Guid OriginalCompetencyUId { get; set; }
    }
}
