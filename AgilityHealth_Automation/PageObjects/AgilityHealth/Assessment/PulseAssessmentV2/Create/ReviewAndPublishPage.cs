using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.Utilities;
using AngleSharp.Common;
using AtCommon.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.PulseAssessmentV2.Create
{
    public class ReviewAndPublishPage : BasePage
    {
        public CreateHeaderPage Header { get; set; }

        public ReviewAndPublishPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
            Header = new CreateHeaderPage(driver, log);
        }

        //Locators
        private readonly By PublishButton = By.XPath("//button[contains(normalize-space(),'PUBLISH')]");
        private readonly By SaveAsDraftButtonOnReviewAndPublishPage = AutomationId.Equals("saveAsDraftBtnTop");

        //Assessment Details Section
        private readonly By AssessmentDetailsPencilIcon = By.XPath("//div[text()='Assessment Details']//*[@data-icon='pen']");
        private readonly By AssessmentDetailsSectionNameText = By.XPath("//div[text()='Name']/following-sibling::div | //div//font[text()='Name']/ancestor::div/following-sibling::div/font/font");
        private readonly By AssessmentDetailsSectionTypeText = By.XPath("//div[text()='Type']/following-sibling::div | //div//font[text()='Type']/ancestor::div/following-sibling::div/font/font");
        private readonly By AssessmentDetailsSectionStartDateAndTimeText = By.XPath("//div[text()='Start Date and Time']/following-sibling::div | //div//font[text()='Start Date and Time']/parent::font/parent::div/following-sibling::div");
        private readonly By AssessmentDetailsSectionFrequencyText = By.XPath("//div[text()='Frequency']/following-sibling::div | //div//font[text()='Frequency']/ancestor::div/following-sibling::div/font/font");
        private readonly By AssessmentDetailsSectionAssessmentPeriodText = By.XPath("//div[text()='Assessment Period']/following-sibling::div | //div//font[text()='Assessment Period']/ancestor::div/following-sibling::div/font/font");

        //Selected Questions Section
        private readonly By QuestionsSelectedPencilIcon = By.XPath("//div[contains(text(),'Questions Selected')]//*[@data-icon='pen'] | //div//font[contains(text(),'Questions Selected')]/../parent::div//*[@data-icon='pen']");
        private readonly By CountOfSelectedQuestions = By.XPath("//div[contains(text(),'Questions Selected')] | //div//font[contains(text(), 'Questions Selected')]/../../font");
        private readonly By SelectedQuestionsList = By.XPath("//div[contains(text(),'Selected')]//following-sibling::div/div | //div//font[contains(text(),'Selected')]/../parent::div//following-sibling::div/div");

        //Team Section
        private readonly By TeamSectionTeamCount = By.XPath("//span[contains(text(),'Team')]");
        private readonly By TeamSectionExpandCollapseIcon = By.XPath(" //span[contains(normalize-space(),'Team')]//following-sibling::button//*[local-name()='svg']");
        private readonly By ListOfTeams = By.XPath("//div[@role='region']//div[@role='button']//button//parent::span");
        private readonly By TeamSectionPencilIcon = By.XPath("//span[contains(normalize-space(),'Team')]//parent::span//..//button//*[local-name()='svg'and @data-icon='pen']");
        public static By TeamExpandCollapseIcon(string teamName) => By.XPath($"//span[contains(normalize-space(),'{teamName}')]");
        public static By ListOfTeamMembersEmailByTeam(string teamName) => By.XPath(
            $"//span[contains(normalize-space(),'{teamName}')]//ancestor::div[@role='button']//following-sibling::div//div[@title][1]");


        //Methods
        public void ClickOnPublishButton()
        {
            Log.Step(nameof(CreateHeaderPage), "Click on 'Publish' button");
            Driver.JavaScriptScrollToElement(Wait.UntilAllElementsLocated(PublishButton).GetItemByIndex(1)).Click();
            Wait.HardWait(2000);//Its taking some time to load on 'TeamAssessment dashboard'
        }

        public void ClickOnSaveAsDraftButton()
        {
            Log.Step(nameof(CreateHeaderPage), "Click on 'Save as Draft' button from 'Review & Publish' page");
            Wait.UntilElementClickable(SaveAsDraftButtonOnReviewAndPublishPage).Click();
            Wait.HardWait(2000);//Its taking some to load on 'TeamAssessment dashboard'.
        }

        public bool IsPublishButtonDisplayed()
        {
            return Driver.IsElementDisplayed(PublishButton);
        }

        public bool IsSaveAsDraftButtonDisplayed()
        {
            return Driver.IsElementDisplayed(SaveAsDraftButtonOnReviewAndPublishPage);
        }

        //Assessment Details
        public void ClickOnAssessmentDetailsPencilIcon()
        {
            Log.Step(GetType().Name, "Click on 'AssessmentDetails' Pencil button");
            Wait.UntilElementClickable(AssessmentDetailsPencilIcon).Click();
        }

        public string GetAssessmentName()
        {
            Log.Step(GetType().Name, "Get assessment 'Name'");
            return Wait.UntilElementExists(AssessmentDetailsSectionNameText).GetText();
        }

        public string GetAssessmentType()
        {
            Log.Step(GetType().Name, "Get assessment 'Type'");
            return Wait.UntilElementExists(AssessmentDetailsSectionTypeText).GetText();
        }

        public string GetAssessmentStartDate()
        {
            Log.Step(GetType().Name, "Get assessment 'Start and End date'");
            return Wait.UntilElementExists(AssessmentDetailsSectionStartDateAndTimeText).GetText();
        }

        public string GetAssessmentFrequency()
        {
            Log.Step(GetType().Name, "Get assessment 'Frequency'");
            return Wait.UntilElementExists(AssessmentDetailsSectionFrequencyText).GetText();
        }

        public string GetAssessmentPeriod()
        {
            Log.Step(GetType().Name, "Get assessment 'Period'");
            return Wait.UntilElementExists(AssessmentDetailsSectionAssessmentPeriodText).GetText();
        }

        //Questions selected
        public void ClickOnQuestionsSelectedPencilIcon()
        {
            Log.Step(GetType().Name, "Click on 'QuestionsSelected' Pencil button");
            Wait.UntilElementClickable(QuestionsSelectedPencilIcon).Click();
        }

        public string GetTheCountOfSelectedQuestions()
        {
            Log.Step(GetType().Name, "Get count of 'QuestionsSelected'");

            if (Wait.UntilAllElementsLocated(CountOfSelectedQuestions).Count <= 1)
                return Wait.UntilElementExists(CountOfSelectedQuestions).GetText().Split('(')[1].Replace(")", "");
            var a = Wait.UntilAllElementsLocated(CountOfSelectedQuestions).Select(a => a.GetText()).ToList();
            return a[1];
            
        }

        public List<string> GetTheSelectedQuestions()
        {
            Log.Step(GetType().Name, "Get list of selected questions");
            return Wait.UntilAllElementsLocated(SelectedQuestionsList).Select(a => a.GetText().Replace("\r", "").Replace("\n", "")).ToList();
        }


        //Teams
        public void ClickOnTeamPencilIcon()
        {
            Log.Step(GetType().Name, "Click on pencil icon of 'Team' Section");
            Driver.JavaScriptScrollToElement(TeamSectionPencilIcon).Click();
        }

        public string GetCountOfTeams()
        {
            Log.Step(GetType().Name, "Get count of 'Teams'");
            return Wait.UntilElementExists(TeamSectionTeamCount).GetText().Split('(')[1].RemoveWhitespace().Replace(")TeamMembers", "");
        }

        public string GetCountOfTeamMembers()
        {
            Log.Step(GetType().Name, "Get count of 'TeamMembers'");
            return Wait.UntilElementExists(TeamSectionTeamCount).GetText().Split('(')[2].RemoveWhitespace().Replace(")", "");
        }

        public void ClickOnTeam()
        {
            Log.Step(GetType().Name, "Click on 'Teams'");
            Driver.JavaScriptScrollToElementCenter(TeamSectionExpandCollapseIcon).Click();
        }

        public List<string> GetListOfTeams()
        {
            Log.Step(GetType().Name, "Get List of 'Teams'");
            if (Wait.UntilElementExists(TeamSectionExpandCollapseIcon).GetAttribute("data-testid") == "KeyboardArrowDownIcon")
            {
                Wait.UntilElementClickable(TeamSectionExpandCollapseIcon).Click();
            }

            return Wait.UntilAllElementsLocated(ListOfTeams).Select(a => a.GetText()).ToList();
        }

        public void ClickOnTeamExpandButton(string teamName)
        {
            Log.Step(GetType().Name, "Click on 'TeamsExpandButton'");
            Wait.UntilElementClickable(TeamExpandCollapseIcon(teamName)).Click();
        }

        public List<string> GetListOfTeamMembersEmail(string teamName)
        {
            Log.Step(GetType().Name, "Get list of 'TeamMember's email'");
            ClickOnTeamExpandButton(teamName);
            return Wait.UntilAllElementsLocated(ListOfTeamMembersEmailByTeam(teamName)).Select(a => a.GetAttribute("title")).ToList();
        }

        public bool IsMemberDisplayed(string teamName)
        {
            return Driver.IsElementDisplayed(ListOfTeamMembersEmailByTeam(teamName));
        }

    }
}