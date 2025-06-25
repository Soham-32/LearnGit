using System;
using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Radar;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api.Enums;
using AtCommon.Dtos.Radars.Custom;
using AtCommon.Utilities;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Survey
{
    internal class SurveyPage : BasePage
    {
        public SurveyPage(IWebDriver driver, ILogger log) : base(driver, log) { }

        // Header
        private readonly By GlobeIcon = By.Id("globeIcon");
        private readonly By LanguageDropdown = By.XPath("//span[@aria-owns='Languages_listbox']");
        private static By SelectLanguageDropdownOption(string language) => By.XPath($"//ul[@id='Languages_listbox']//li[@role='option'][normalize-space()='{language}']");

        private readonly By HeaderCompanyName = By.XPath("//div[@id='title_subtitle']//h3");
        private readonly By HeaderTeamName = By.XPath("//div[@id='title_subtitle']//h4");
        private readonly By HeaderRoleDropDown = By.XPath("//span[@aria-owns='selection_listbox']//span[@class='k-select']");
        private static By HeaderRoleDropDownItems(string item) => By.XPath($"//ul[@id='selection_listbox']//li[text()='{item}']");

        private readonly By StartSurveyButton = By.Id("start_survey");
        private readonly By NextButton = By.CssSelector("div.actionBar a.buttonNext");
        private static By SurveySavedText(string text = "Saved") => By.XPath($"//div[contains(@class,'success-saving-msg')]//div[text()='{text}']");
        private readonly By FinishButton = By.CssSelector("div.actionBar a.buttonFinish");
        private readonly By CompleteSurveyOkButton = By.Id("ok-confirming");
        private readonly By ConfirmIdentityTitle = By.ClassName("confirmIdentityTitle");
        private readonly By RolesListbox = By.XPath("//ul[@id = 'Roles_taglist']/..");
        private static By RoleItem(string item) => By.XPath($"//ul[@id = 'Roles_listbox']/li[text() = '{item}']");
        private readonly By ParticipantGroupListBox = By.XPath("//ul[@id = 'ParticipantGroups_taglist']/..");
        private static By ParticipantGroupItem(string item) => By.XPath($"//ul[@id = 'ParticipantGroups_listbox']/li[text() = '{item}']");

        private readonly By SurveyFinishStep = By.Id("finishButton");
        private readonly By ReviewerContinueButton = By.Id("continue");
        private static By NoteTextbox(string section) => By.XPath($"//*[text()='Add any additional thoughts about \"{section}\"']/following-sibling::p[1]/textarea");
        //For TH 2.0
        private readonly By StrengthsNoteArea =
        By.XPath("//p[text()=\"What are the top strengths of this team? What value has this team delivered? Share this team's strengths and success stories you want to celebrate!\"]/following-sibling::p[1]/textarea");
        private readonly By ImprovementsNoteArea =
        By.XPath("//p[text()=\"What are this team's growth opportunities? Think of areas that could significantly improve the overall team's performance if they were addressed.\"]/following-sibling::p[1]/textarea");
        private readonly By ImpedimentsNoteArea =
        By.XPath("//p[text()=\"What are the significant impediments this team has been experiencing? Some may be team related and some may be organizational.\"]/following-sibling::p[1]/textarea");

        //For Agile Coach Health
        private readonly By IaStrengthsNoteArea =
        By.XPath("//p[text()=\"What do you see as my top strengths?\"]/following-sibling::p[1]/textarea");
        private readonly By IaImprovementsNoteArea =
        By.XPath("//p[text()=\"What do you see as my top areas for improvement/growth?\"]/following-sibling::p[1]/textarea");
        private readonly By IaImpedimentsNoteArea =
        By.XPath("//p[text()=\"What do you see as my impediments to growth?\"]/following-sibling::p[1]/textarea");

        //Confirm Identity popup
        private readonly By ContinueButton = By.Id("yesIdentityConfirm");
        private readonly By ThatsNotMeButton = By.Id("noIdentityConfirm");
        private readonly By AllQuestions = By.CssSelector("span.questionText");

        //Sub Dimension footer button        
        private readonly By SaveButton = By.ClassName("buttonSave");
        private readonly By QuestionLabelArrowText = By.ClassName("buttonQuestionLabel");
        private readonly By SectionLabelArrowText = By.ClassName("buttonSectionLabel");

        //Left Navigation
        private readonly By LeftNavDimensionNames = By.CssSelector("span.stepDesc");

        //Survey Questions Body
        private readonly By DimensionBody = By.CssSelector("div[id*=step][style='display: block; left: 0px;']");
        private readonly By AllSubDimensionsHeaderText = By.CssSelector("div[id*=step][style='display: block; left: 0px;'] h3");
        private static By QuestionsBySubDimensionText(string subDimension) => By.XPath($"//div[contains(@id,'step')]//h3[ contains(text(),'{subDimension}')]//following-sibling::ul/li//span[@class='questionText']");
        private static By SurveyQuestionAnswerNaText(string subDimension) => By.XPath($"//div[contains(@id,'step')]//h3[text()='{subDimension}']//following-sibling::ul/li//div[contains(@class,'ui-checkbox')]/label");
        private static By QuestionProgressDescriptionText(string subDimension) => By.XPath($"//div[contains(@id,'step')]//h3[text()='{subDimension}']//following-sibling::ul/li//div[contains(@class,'answers')]/div");
        private static By CompetencyNotesTitleText(string subDimension) => By.XPath($"//div[contains(@id,'step')]//h3[text()='{subDimension}']//following-sibling::ul/following-sibling::p[1]");
        private static By CompetencyDescriptionText(string subDimension) => By.XPath($"//div[contains(@id,'step')]//h3[contains(text(),'{subDimension}')]//following-sibling::p");

        //confirm Identity popup text
        private readonly By PopupTitleText = By.Id("teamMemberConfirmation_wnd_title");
        private readonly By ConfirmIdentityText = By.ClassName("confirmIdentityText");
        private readonly By ConfirmIdentityFooterText = By.ClassName("confirmIdentityFooter");

        //survey welcome Page
        private readonly By WelcomePageMessageText = By.XPath("//div[@id='step-1']/p/span");
        private readonly By WelcomePageStepper = By.Id("welcomeButton");
        private readonly By WelcomePageMessageForSpanishText = By.XPath("//div[@id='step-1']/p[2]");

        private readonly By FinishOpenEndQuestionText = By.XPath("//div[contains(@id,'step-7')]/p[contains(@class,'openEndedQuestion')]");
        private readonly By ProgressOpenEndQuestionText = By.XPath("//div[contains(@id,'step-7')]/p/div[contains(@class,'progressAnswerStyle')]");

        //Confirm Identity finish popup
        private readonly By ConfirmIdentityFinishPopupText = By.Id("confirm-finishing-dialog-msg");
        private readonly By ConfirmIdentityFinishPopupTitleText = By.Id("confirm-finishing-dialog_wnd_title");
        private readonly By ConfirmIdentityFinishCancelButton = By.Id("cancel-confirming");
        private readonly By SurveyFinishMessageText = By.XPath("//div[contains(@id,'step-7')]");

        //Survey Completed text
        private readonly By SurveyAlreadyCompletedText = By.XPath("//p[contains(text(),'You have already completed the survey!')]");

        //Header
        public void SelectLanguageDropdown(string language)
        {
            Log.Step(nameof(SurveyPage), $"Select {language} language  from 'Language' dropdown");
            Wait.UntilElementVisible(GlobeIcon).Click();
            SelectItem(LanguageDropdown, SelectLanguageDropdownOption(language));
            Wait.UntilJavaScriptReady();
        }

        public string GetHeaderCompanyName()
        {
            return Wait.UntilElementVisible(HeaderCompanyName).GetText();
        }
        public string GetHeaderTeamName()
        {
            return Wait.UntilElementVisible(HeaderTeamName).GetText();
        }
        public void SelectHeaderRoleTag(List<string> roleList)
        {
            Log.Step(nameof(SurveyPage), $"Select role <{roleList}> in the dropdown");
            foreach (var role in roleList)
            {
                SelectItem(HeaderRoleDropDown, HeaderRoleDropDownItems(role));
            }
        }

        public void CompleteRandomSurvey()
        {
            Log.Info("Complete the survey and switch to 'Team Dashboard' window");
            ConfirmIdentity();
            ClickStartSurveyButton();
            SubmitRandomSurvey();
            ClickNextButton();
            ClickNextButton();
            ClickNextButton();
            ClickNextButton();
            ClickNextButton();
            ClickFinishButton();

        }

        public void ClickStartSurveyButton()
        {
            Log.Step(nameof(SurveyPage), "Click 'Start Survey' button");
            Wait.UntilElementClickable(StartSurveyButton).Click();
        }

        public void ClickNextButton()
        {
            Log.Step(nameof(SurveyPage), "Click 'Next' button");
            Wait.UntilElementClickable(NextButton).Click();
        }
        public bool WaitUntilSurveySavedTextShown()
        {
            return Driver.IsElementDisplayed(SurveySavedText());
        }

        public void ClickFinishButton(bool english = true)
        {
            Log.Step(nameof(SurveyPage), "Click 'Finish' button");
            Wait.UntilJavaScriptReady();
            Driver.JavaScriptScrollToElement(Wait.UntilElementClickable(FinishButton)).Click();
            Wait.UntilJavaScriptReady();
            Wait.UntilElementClickable(CompleteSurveyOkButton).Click();
            Wait.UntilJavaScriptReady();
            //if (english)
            //{
            //    (new WebDriverWait(Driver, TimeSpan.FromSeconds(60))).Until((d) => d.FindElement(By.CssSelector("div.stepContainer div[style*='left: 0px;'][style*='display: block;']")).GetText().StartsWith("Thank you"));
            //}//While filling assessment, we receive OOPS page but assessment is filled. We will remove one bug is fixed
        }

        public void SubmitRandomSurvey()
        {
            Log.Step(nameof(SurveyPage), "Fill out survey with random answers");
            const string jsScript = "$('.pages').page();" +
            "$('.slider').each(function() {" +
            "var num = Math.floor((Math.random() * 10) + 1);" +
            "$(this).val(num);" +
            "$(this).slider('refresh');});";
            Driver.ExecuteJavaScript(jsScript);
        }

        public void SubmitSurveyNotes(List<string> dimensions, List<DimensionNote> notes)
        {
            Log.Step(nameof(SurveyPage), "Adding a notes for every dimensions");
            foreach (var dimension in dimensions)
            {
                EnterNotesForDimension(dimension, notes);
                ClickNextButton();
            }
        }

        public void SubmitSurvey(int value)
        {
            Log.Step(nameof(SurveyPage), $"Submit fixed value selections = {value}");
            var jsScript = "$('.pages').page();" +
            "$('.slider').each(function() {" +
            "var num = Math.floor(" + value + ");" +
            "$(this).val(num);" +
            "$(this).slider('refresh');});";
            Driver.ExecuteJavaScript(jsScript);
        }

        public void SubmitSurveyPerQuestion(int memberIndex, List<RadarDimension> surveyData)
        {
            foreach (var t in surveyData)
            {
                var subDimensionsList = t.SubDimensions.ToList();
                for (var j = 0; j < subDimensionsList.Count; j++)
                {
                    var competenciesList = t.SubDimensions.ToList().ElementAt(j).Competencies.ToList();
                    for (var k = 0; k < competenciesList.Count; k++)
                    {
                        var sliderValue = "var num = Math.floor(" + competenciesList[k].Questions.ToList()
                            .ElementAt(0).QuestionValue[memberIndex].Value + ");";
                        var questionsList = t.SubDimensions.ToList().ElementAt(j).Competencies.ToList().ElementAt(k)
                            .Questions.ToList();
                        foreach (var jsScript in questionsList.Select(t1 => "$('.pages').page(); " +
                                                                            "$('input#" + ("slider" + t1.QuestionId) +
                                                                            ".slider.ui-slider-bg.ui-shadow-inset.ui-body-inherit.ui-corner-all.ui-slider-input').each(function(){" +
                                                                            sliderValue +
                                                                            "$(this).val(num);" +
                                                                            "$(this).slider('refresh');});"))
                        {
                            Driver.ExecuteJavaScript(jsScript);
                        }
                    }
                }
            }
        }

        public void EnterNote(string noteSection, string content)
        {
            Log.Step(nameof(SurveyPage), $"Enter text <{content}> into note section <{noteSection}>");
            Driver.JavaScriptScrollToElement(NoteTextbox(noteSection)).SetText(content);
        }

        public void EnterNotesForDimension(string dimension, List<DimensionNote> notes)
        {
            notes.Where(n => n.Dimension == dimension).ToList()
            .ForEach(o => EnterNote(o.SurveySection, o.Note));
        }

        public void EnterOpenEndedNote(string noteSection, string note, AssessmentType type)
        {
            var locator = noteSection switch
            {
                "Strengths" => type == AssessmentType.Individual ? IaStrengthsNoteArea : StrengthsNoteArea,
                "Improvements" => type == AssessmentType.Individual ? IaImprovementsNoteArea : ImprovementsNoteArea,
                "Impediments" => type == AssessmentType.Individual ? IaImpedimentsNoteArea : ImpedimentsNoteArea,
                _ => throw new ArgumentException($"<{noteSection}> is not a recognized open ended note section.")
            };

            Log.Step(nameof(SurveyPage), $"Enter text <{note}> into note section '{noteSection}'");
            Wait.UntilElementClickable(locator).SetText(note);
        }

        public void EnterOpenEndedNotes(List<DimensionNote> notes, AssessmentType type = AssessmentType.Team)
        {
            notes.Where(n => n.Dimension == "Open").ToList()
            .ForEach(o => EnterOpenEndedNote(o.SubDimension, o.Note, type));
        }

        public void ConfirmIdentity()
        {
            Log.Step(nameof(SurveyPage), "Click the 'Continue' button on the Confirm Identity popup");
            Wait.UntilElementClickable(ContinueButton).Click();
            Wait.UntilElementInvisible(ContinueButton);
        }

        public void ClickThatsNotMe()
        {
            Log.Step(nameof(SurveyPage), "Click the 'That's Not Me' button on the Confirm Identity popup");
            Wait.UntilJavaScriptReady();
            Wait.UntilElementClickable(ThatsNotMeButton).Click();
            Wait.UntilElementNotExist(ThatsNotMeButton);
        }

        public List<string> GetQuestionIds()
        {
            var elements = Wait.UntilAllElementsLocated(AllQuestions);
            return (from element in elements select element.GetAttribute("id")).ToList();
        }

        public string GetSurveyIdentity()
        {
            return Wait.UntilElementVisible(ConfirmIdentityTitle).GetText();
        }
        public bool IsRolesDropDownDisplayed()
        {
            return Driver.IsElementDisplayed(RolesListbox);
        }
        public bool IsParticipantGroupsDisplayed()
        {
            return Driver.IsElementDisplayed(ParticipantGroupListBox);
        }
        public bool IsSurveyFinishStepDisplayed()
        {
            return Driver.IsElementDisplayed(SurveyFinishStep);
        }
        public void SelectReviewerRole(List<string> role)
        {
            Log.Step(nameof(SurveyPage), $"Select Reviewer role <{role}> in the dropdown");
            foreach (var specificRole in role)
            {
                SelectItem(RolesListbox, RoleItem(specificRole));
            }
            Wait.UntilElementClickable(ReviewerContinueButton).Click();
        }

        public void SelectRoleAndParticipantGroup(List<string> role, List<string> participantGroup)
        {
            Log.Step(nameof(SurveyPage), $"Select Reviewer role <{role}> in the dropdown");
            foreach (var specificRole in role)
            {
                SelectItem(RolesListbox, RoleItem(specificRole));
            }
            foreach (var specificGroup in participantGroup)
            {
                SelectItem(ParticipantGroupListBox, ParticipantGroupItem(specificGroup));
            }
            Wait.UntilElementClickable(ReviewerContinueButton).Click();
        }

        //Left Navigation
        public IList<string> GetLeftNavDimensionsList()
        {
            Log.Step(nameof(SurveyPage), "Get dimensions, sub-dimensions & competency List");
            return Wait.UntilAllElementsLocated(LeftNavDimensionNames).Select(e => e.GetText()).ToList();
        }
        //Survey Questions Body
        public string GetDimensionBodyText()
        {
            Log.Step(nameof(SurveyPage), "Get Body Text");
            return Wait.UntilElementVisible(DimensionBody).GetText();
        }

        public IList<string> GetAllSubDimensionsHeaderTextList()
        {
            Log.Step(nameof(SurveyPage), "Get subDimension header text");
            return Wait.UntilAllElementsLocated(AllSubDimensionsHeaderText).Select(e => e.GetText()).ToList();
        }

        public IList<string> GetAllQuestionsBySubDimensionList(string subDimension)
        {
            Log.Step(nameof(SurveyPage), "Get all questions list");
            return Wait.UntilAllElementsLocated(QuestionsBySubDimensionText(subDimension)).Where(e => e.Displayed).Select(e => e.GetText()).ToList();
        }

        public List<string> GetAllQuestionsByNaTextList(string subDimension)
        {
            Log.Step(nameof(SurveyPage), "Get all N/A text");
            return Wait.UntilAllElementsLocated(SurveyQuestionAnswerNaText(subDimension)).Where(e => e.Displayed).Select(e => e.GetText().ReplaceStringData()).ToList();
        }

        public List<string> GetAllQuestionsByProgressDescriptionList(string subDimension)
        {
            Log.Step(nameof(SurveyPage), "Get all Questions Progress description");
            return Wait.UntilAllElementsLocated(QuestionProgressDescriptionText(subDimension)).Where(e => e.Displayed).Select(e => e.GetText()).ToList();
        }

        public List<string> GetCompetencyNotesList(string subDimension)
        {
            Log.Step(nameof(SurveyPage), "Get all competencies notes list");
            return Wait.UntilAllElementsLocated(CompetencyNotesTitleText(subDimension)).Where(e => e.Displayed).Select(e => e.GetText()).ToList();
        }

        public List<string> GetCompetencyDescriptionList(string subDimension)
        {
            Log.Step(nameof(SurveyPage), "Get all the competencies description list");
            return Wait.UntilAllElementsLocated(CompetencyDescriptionText(subDimension)).Where(e => e.Displayed).Select(e => e.GetText()).ToList();
        }

        public List<string> GetMaturityDescriptionList(string subDimension)
        {
            Log.Step(nameof(SurveyPage), "Get maturity description list");
            return Wait.UntilAllElementsLocated(CompetencyDescriptionText(subDimension)).Where(e => e.Displayed).Select(e => e.GetText()).ToList();
        }

        // Confirm Identity Popup
        public void WaitForConfirmIdentityPopupToLoad()
        {
            Wait.UntilElementVisible(PopupTitleText);
        }
        public string GetConfirmIdentityPopupTitle()
        {
            Log.Step(nameof(SurveyPage), "Get confirm identity popup title");
            return Wait.UntilElementVisible(PopupTitleText).GetText();
        }
        public string GetConfirmIdentityText()
        {
            Log.Step(nameof(SurveyPage), "Get confirm identity text");
            return Wait.UntilElementVisible(ConfirmIdentityText).GetText();
        }

        public string GetConfirmIdentityFooterText()
        {
            Log.Step(nameof(SurveyPage), "Get confirm identity footer text");
            return Wait.UntilElementVisible(ConfirmIdentityFooterText).GetText();
        }

        public string GetConfirmIdentityContinueButtonText()
        {
            Log.Step(nameof(SurveyPage), "Get confirm identity 'Continue' button text");
            return Wait.UntilElementVisible(ContinueButton).GetText();
        }

        public string GetConfirmIdentityThatsNotMeButtonText()
        {
            Log.Step(nameof(SurveyPage), "Get confirm identity 'That's not me' button text");
            return Wait.UntilElementVisible(ThatsNotMeButton).GetText();
        }

        // Confirm welcome page
        public string GetWelcomePageMessage()
        {
            Log.Step(nameof(SurveyPage), "Get welcome page message");
            return Wait.UntilElementVisible(WelcomePageMessageText).GetText();
        }

        public string GetWelcomePageMessageForSpanish()
        {
            Log.Step(nameof(SurveyPage), "Get welcome page message for spanish");
            return Wait.UntilElementVisible(WelcomePageMessageForSpanishText).GetText();
        }

        public string GetWelcomePageSurveyStartButtonText()
        {
            Log.Step(nameof(SurveyPage), "Get welcome page survey 'Start' button text");
            return Wait.UntilElementVisible(StartSurveyButton).GetText();
        }

        public string GetWelcomePageStepperText()
        {
            Log.Step(nameof(SurveyPage), "Get welcome page stepper text");
            return Wait.UntilElementVisible(WelcomePageStepper).GetText();
        }

        public void ClickOnWelcomePageStepper()
        {
            Log.Step(nameof(SurveyPage), "Click on welcome page stepper");
            Wait.UntilElementVisible(WelcomePageStepper).Click();
        }

        //Confirm open ended question
        public List<string> GetFinishOpenEndedQuestionList()
        {
            Log.Step(nameof(SurveyPage), "Get finish openEnded questions text");
            return Wait.UntilAllElementsLocated(FinishOpenEndQuestionText).Where(e => e.Displayed).Select(e => e.GetText()).ToList();
        }

        public List<string> GetFinishOpenEndedQuestionProgressList()
        {
            Log.Step(nameof(SurveyPage), "Get finish openEnded questions progress list");
            return Wait.UntilAllElementsLocated(ProgressOpenEndQuestionText).Where(e => e.Displayed).Select(e => e.GetText()).ToList();
        }

        public string GetSaveButtonText()
        {
            Log.Step(nameof(SurveyPage), "Get 'Save' button text");
            return Wait.UntilElementVisible(SaveButton).GetText();
        }
        //Confirm dimension footer buttons text
        public string GetQuestionLabelArrowText()
        {
            Log.Step(nameof(SurveyPage), "Get questions label arrow text");
            return Wait.UntilElementVisible(QuestionLabelArrowText).GetText();
        }

        public string GetSectionLabelArrowText()
        {
            Log.Step(nameof(SurveyPage), "Get section label arrow text");
            return Wait.UntilElementVisible(SectionLabelArrowText).GetText();
        }

        public string GetFinishButtonText()
        {
            Log.Step(nameof(SurveyPage), "Get 'Finish' button text");
            return Wait.UntilElementVisible(FinishButton).GetText();
        }

        //Confirm survey saved text
        public string GetSurveySavedText(string saveText)
        {
            Log.Step(nameof(SurveyPage), "Get survey 'Saved' text");
            return Wait.UntilElementVisible(SurveySavedText(saveText)).GetText();
        }

        public void ClickSurveyFinishButton()
        {
            Log.Step(nameof(SurveyPage), "Click on the survey 'Finish' button");
            Wait.UntilJavaScriptReady();
            Driver.JavaScriptScrollToElement(Wait.UntilElementClickable(FinishButton)).Click();
        }

        //Confirm confirm Identity finish popup
        public string GetConfirmIdentityFinishHeaderTitle()
        {
            Log.Step(nameof(SurveyPage), "Get confirm identity finish header title text");
            return Wait.UntilElementVisible(ConfirmIdentityFinishPopupTitleText).GetText();
        }
        public string GetConfirmIdentityFinishDescription()
        {
            Log.Step(nameof(SurveyPage), "Get confirm identity finish popup description text");
            return Wait.UntilElementVisible(ConfirmIdentityFinishPopupText).GetText();
        }

        public string GetConfirmIdentityFinishOkButtonText()
        {
            Log.Step(nameof(SurveyPage), "Get confirm identity finish 'Ok' button text");
            return Wait.UntilElementVisible(CompleteSurveyOkButton).GetText();
        }

        public string GetConfirmIdentityFinishCancelButtonText()
        {
            Log.Step(nameof(SurveyPage), "Get confirm identity finish 'Cancel' button text");
            return Wait.UntilElementVisible(ConfirmIdentityFinishCancelButton).GetText();
        }

        public string GetSurveyFinishMessage()
        {
            Log.Step(nameof(SurveyPage), "Get survey finish message text");
            return Wait.UntilElementVisible(SurveyFinishMessageText).GetText();
        }

        public void ClickOnSurveyConfirmIdentityOkButton()
        {
            Log.Step(nameof(SurveyPage), "Click on survey confirm identity 'Ok' button text");
            Wait.UntilElementVisible(CompleteSurveyOkButton).Click();
        }

        // Survey already completed validation text
        public string GetSurveyAlreadyCompletedText()
        {
            Log.Step(nameof(SurveyPage), "Get Survey already completed text");
            return Wait.UntilElementVisible(SurveyAlreadyCompletedText).GetText().Replace("\r\n", string.Empty);
        }

    }
}