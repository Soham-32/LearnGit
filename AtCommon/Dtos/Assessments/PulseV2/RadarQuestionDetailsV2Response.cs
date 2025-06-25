using System;
using System.Collections.Generic;
using System.Linq;
using AtCommon.Dtos.Assessments.PulseV2.Custom;
using AtCommon.Utilities;

namespace AtCommon.Dtos.Assessments.PulseV2
{
    public class RadarQuestionDetailsV2Response
    {
        public int SurveyId { get; set; }
        public Guid SurveyUid { get; set; }
        public string Name { get; set; }
        public IEnumerable<RadarDimensionV2> Dimensions { get; set; }
    }

    public class RadarDimensionV2
    {
        public int DimensionId { get; set; }
        public string Name { get; set; }
        public IEnumerable<RadarSubDimensionV2> SubDimensions { get; set; }
    }

    public class RadarSubDimensionV2
    {
        public int SubDimensionId { get; set; }
        public string Name { get; set; }
        public IEnumerable<RadarCompetencyV2> Competencies { get; set; }
    }

    public class RadarCompetencyV2
    {
        public int CompetencyId { get; set; }
        public string Name { get; set; }
        public IEnumerable<RadarQuestionV2> Questions { get; set; }
    }

    public class RadarQuestionV2
    {
        public int QuestionId { get; set; }
        public string Text { get; set; }
    }

    public static class RadarQuestions
    {
        public static RadarQuestionDetailsV2Response FilterQuestions(this RadarQuestionDetailsV2Response response,
            QuestionSelectionPreferences questionPref)
        {
            if (questionPref == QuestionSelectionPreferences.Dimension ||
                questionPref == QuestionSelectionPreferences.SubDimension ||
                questionPref == QuestionSelectionPreferences.Competency)
            {
                response.Dimensions =
                    response.Dimensions.Where(d => d.Name == SharedConstants.SurveyDimension).ToList();
            }

            if (questionPref == QuestionSelectionPreferences.SubDimension ||
                questionPref == QuestionSelectionPreferences.Competency)
            {
                response.Dimensions.First().SubDimensions = response.Dimensions.First().SubDimensions
                    .Where(s => s.Name == SharedConstants.SurveySubDimension);
            }

            if (questionPref == QuestionSelectionPreferences.Competency)
            {
                response.Dimensions.First().SubDimensions.First().Competencies = response.Dimensions
                    .First().SubDimensions.First().Competencies
                    .Where(c => c.Name == SharedConstants.SurveyCompetency);
            }

            return response;
        }
    }
}
