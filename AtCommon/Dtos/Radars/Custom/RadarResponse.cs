using System;
using System.Collections.Generic;

namespace AtCommon.Dtos.Radars.Custom
{
    public class RadarResponse
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
        public IEnumerable<RadarSubDimension> SubDimensions { get; set; }
    }

    public class RadarSubDimension
    {
        public int SubDimensionId { get; set; }
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
        public List<QuestionValue> QuestionValue { get; set; }
    }
    public class QuestionValue
    {
        public string Value { get; set; }
    }

    public class AllDimensions
    {
        public int SurveyId { get; set; }
        public string SurveyUid { get; set; }
        public string Name { get; set; }
        public List<RadarDimension> Dimensions { get; set; }
    }
}