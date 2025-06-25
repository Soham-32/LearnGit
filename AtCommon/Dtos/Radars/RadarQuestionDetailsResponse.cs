using System;
using System.Collections.Generic;

namespace AtCommon.Dtos.Radars
{
    public class RadarQuestionDetailsResponse
    {
        public int SurveyId { get; set; }
        public Guid SurveyUid { get; set; }
        public string Name { get; set; }
        public IEnumerable<RadarDimension> Dimensions { get; set; }
    }

    public class RadarDimension
    {
        public int DimensionId { get; set; }
        public string Name { get; set; }
        public IEnumerable<RadarSubdimension> Subdimensions { get; set; }
    }

    public class RadarSubdimension
    {
        public int SubdimensionId { get; set; }
        public string Name { get; set; }
        public IEnumerable<RadarCompetency> Competencies { get; set; }
    }

    public class RadarCompetency
    {
        public int CompetencyId { get; set; }
        public string Name { get; set; }
        public IEnumerable<RadarQuestion> Questions { get; set; }
    }

    public class RadarQuestion
    {
        public int QuestionId { get; set; }
        public string Text { get; set; }
    }
}