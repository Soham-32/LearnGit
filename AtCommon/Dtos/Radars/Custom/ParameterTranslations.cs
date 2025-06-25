using System.Collections.Generic;

namespace AtCommon.Dtos.Radars.Custom
{
    public class Competency
    {
        public string Name { get; set; }
        public string Abbreviation { get; set; }
        public string AnalyticsAbbreviation { get; set; }
        public Question Question { get; set; }
    }
    public class Dimension
    {
        public string Name { get; set; }
        public SubDimension SubDimension { get; set; }
    }

    public class Root
    {
        public string Language { get; set; }
        public List<Dimension> Dimension { get; set; }
    }

    public class OpenEnded
    {
        public string Text { get; set; }
    }

    public class Question
    {
        public string QuestionText { get; set; }
        public string QuestionHelp { get; set; }
        public OpenEnded OpenEnded { get; set; }
    }

    public class ParameterTranslations
    {
        public List<Root> Languages { get; set; }
    }

    public class SubDimension
    {
        public string Name { get; set; }
        public string Abbreviation { get; set; }
        public string Description { get; set; }
        public Competency Competency { get; set; }
    }
}