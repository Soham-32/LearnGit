using System.Collections.Generic;

namespace AtCommon.Dtos.Radars.Custom
{
    public class RadarDetails
    {
        public string Language { get; set; }

        public string CompanyName { get; set; }

        public string Type { get; set; }

        public string Name { get; set; }

        public string Scale { get; set; }

        public bool ShowAsAbsolute { get; set; }

        public bool Public { get; set; }

        public bool Show1Response { get; set; }

        public bool IncludeInGlobalBenchmarking { get; set; }

        public bool Active { get; set; }

        public bool TranslatedActive { get; set; }

        public string Logo { get; set; }

        public List<string> LimitedTo { get; set; }

        public List<string> AvailableTo { get; set; }

        public List<string> WorkType { get; set; }

        public string CopyrightText { get; set; }

        public string AssessmentWelcomeMessage { get; set; }

        public string TranslatedAssessmentWelcomeMessage { get; set; }

        public string EmailWelcomeMessage { get; set; }

        public string TranslatedEmailWelcomeMessage { get; set; }

        public bool AppendStandardFooter { get; set; }

        public string ThankYouMessage { get; set; }

        public string TranslatedThankYouMessage { get; set; }

        public string EmailMessageSenderName { get; set; }

        public string EmailMessageSubject { get; set; }

        public string TranslatedEmailMessageSubject { get; set; }

        public List<RadarDimensionResponse> RadarDimensions { get; set; }
    }

    public class RadarDimensionResponse
    {
        public string Name { get; set; }
        public string Color { get; set; }
        public string SortOrder { get; set; }
        public string RadarOrder { get; set; }
        public string Font { get; set; }
        public string FontSize { get; set; }
        public string LetterSpacing { get; set; }
        public string Direction { get; set; }
        public List<RadarSubDimensionResponse> RadarSubDimensions { get; set; }
    }
    public class RadarSubDimensionResponse
    {
        public string Dimension { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public string Abbreviation { get; set; }
        public string Description { get; set; }
        public string Direction { get; set; }
        public string RadarOrder { get; set; }
        public string Font { get; set; }
        public string FontSize { get; set; }
        public string LetterSpacing { get; set; }
        public List<RadarCompetencyResponse> RadarCompetencies { get; set; }
    }
    public class RadarCompetencyResponse
    {
        public string Dimension { get; set; }
        public string SubDimension { get; set; }
        public string Name { get; set; }
        public string Abbreviation { get; set; }
        public string AnalyticsAbbreviation { get; set; }
        public string Exclude { get; set; }
        public string Direction { get; set; }
        public string RadarOrder { get; set; }
        public string Font { get; set; }
        public string FontSize { get; set; }
        public string LetterSpacing { get; set; }
        public List<RadarQuestionResponse> RadarQuestions { get; set; }
    }

    public class RadarQuestionResponse
    {
        public string Dimension { get; set; }
        public string SubDimension { get; set; }
        public string Competency { get; set; }
        public string ScaleOverride { get; set; }
        public bool QuantitativeMetric { get; set; }

        public string QuestionText { get; set; }
        public string QuestionHelp { get; set; }
        public string WorkTypeFilter { get; set; }
        public string WorkType { get; set; }
        public string MethodologyFilter { get; set; }
        public string Methodology { get; set; }
        public string CompanyFilter { get; set; }
        public string Company { get; set; }
        public string QuestionTags { get; set; }
        public string ExcludeRoles { get; set; }
        public string ParticipantTagsFilter { get; set; }
        public string ParticipantTags { get; set; }
        public List<RadarOpenEndedResponse> RadarOpenEnded { get; set; }
    }
    public class RadarOpenEndedResponse
    {
        public string OpenQuestions { get; set; }
        public string Text { get; set; }
        public string Order { get; set; }
        public string Exclude { get; set; }
        public string CompanyFilter { get; set; }
        public string Company { get; set; }
    }
}
