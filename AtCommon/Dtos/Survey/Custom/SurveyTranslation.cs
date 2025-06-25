using System.Collections.Generic;
using Newtonsoft.Json;

namespace AtCommon.Dtos.Survey.Custom
{
    public class Competency
    {
        public string CompetencyName { get; set; }
        public string CompetencyAbbreviation { get; set; }
        public int CompetencyId { get; set; }
        public List<Questions> Question { get; set; }
    }

    public class ConfirmFinishingDialogBox
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string OkButton { get; set; }
        public string CancelButton { get; set; }
    }

    public class ConfirmIdentityPopup
    {
        public string TranslatedConfirmIdentityTitle { get; set; }
        public string TranslatedHeaderTitle { get; set; }
        public string TranslatedHeaderTitleSubtext { get; set; }
        public string TranslatedContinueButtonText { get; set; }
        public string TranslatedThatNotMeButtonText { get; set; }
        public string FooterText { get; set; }
    }

    public class Dimension
    {
        public string DimensionName { get; set; }
        public List<SubDimension> SubDimension { get; set; }
        public List<FooterButtonText> FooterButtonText { get; set; }
        public List<Finish> Finish { get; set; }
    }

    public class Finish
    {
        public List<OpenEndQuestionList> OpenEndQuestionList { get; set; }
        public List<FooterButtonText> FooterButtonText { get; set; }
        public List<ConfirmFinishingDialogBox> ConfirmFinishingDialogBox { get; set; }
        public List<LastPageMessage> LastPageMessage { get; set; }
    }

    public class FooterButtonText
    {
        public string SaveButton { get; set; }
        public string QuestionLabel { get; set; }
        public string SectionLabel { get; set; }
        public string SaveMessage { get; set; }
        public string ValidationMessage { get; set; }
        public string FinishButton { get; set; }
    }

    public class SurveyTranslation
    {
        public List<ConfirmIdentityPopup> ConfirmIdentityPopup { get; set; }
        public List<WelcomePage> WelcomePage { get; set; }
        public List<SurveyAssessmentEmailSubject> SurveyAssessmentEmailSubject { get; set; }
        public List<RadarCompetencyPopup> RadarCompetencyPopup { get; set; }
        public List<Dimension> Dimensions { get; set; }
    }

    public class LastPageMessage
    {
        public string SubmitMessage { get; set; }
        public string ThankYouMessage { get; set; }
    }

    public class Note
    {
        public string NotesTitle { get; set; }
    }

    public class OpenEndQuestionList
    {
        public string OpenEndQuestion { get; set; }
        public string OpenEndQuestionDetail { get; set; }
    }

    public class Questions
    {
        public string Question { get; set; }

        [JsonProperty("N/AText")]
        public string NaText { get; set; }
        public string QuestionFooterDetail { get; set; }
        public string QuestionTooltips { get; set; }
        public string ValidationMessage { get; set; }
        public string Title { get; set; }

    }

    public class SubDimension
    {
        public string HeaderName { get; set; }
        public string DescriptionOne { get; set; }
        public string DescriptionTwo { get; set; }
        public List<Competency> Competencies { get; set; }
        public List<Note> Notes { get; set; }
        public string Name { get; set; }
    }

    public class SurveyAssessmentEmailSubject
    {
        public string Subject { get; set; }
    }
    public class WelcomePage
    {
        public string LeftSidePanelText { get; set; }
        public string WelcomeMessage { get; set; }
        public string StartButtonText { get; set; }
    }

    public class RadarCompetencyPopup
    {
        public string CompetencyProfilePopupName { get; set; }
        public string GrowthPortal { get; set; }
        public string AvgResponse { get; set; }
        public List<string> Question { get; set; }
    }
}

