using System;
using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.PulseAssessmentV2.Create;
using AgilityHealth_Automation.Utilities;
using AtCommon.Dtos.Assessments.PulseV2.Custom;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.PulseAssessmentV2.Base
{
    public class SelectQuestionsBasePage : BasePage
    {
        public SelectQuestionsBasePage(IWebDriver driver, ILogger log) : base(driver, log) { }

        //Locators
        //Headers
        private readonly By SelectQuestionPageTitle = By.XPath("//button[@automation-id='stepNext']//parent::div//parent::div//preceding-sibling::div//p[contains(normalize-space(),'Select Questions')]");

        //Questions locators
        private static By QuestionCheckbox(string questionName) => By.XPath($"//div[@id='panel3d-header']//span[text()='{questionName}']//parent::div/span/input");
        private static By QuestionExpandCollapseIcon(string questionName) => By.XPath($"//span[contains(normalize-space(),'{questionName}')]/parent::div/parent::div/parent::div/following-sibling::div//*[local-name()='svg' and @data-testid='ExpandMoreIcon']");
        private static By QuestionSection(string questionName) => By.XPath($"//*[text()='Questions Selected']/following-sibling::div//div[contains(normalize-space(),'{questionName}')]");

        private static By QuestionsByCompetency(string competency) => By.XPath($"//*[text()='Questions Selected']/following-sibling::div//div[text()='{competency}']/parent::div");
        private static readonly By AllQuestionsCount = By.ClassName("breakHereForQuestion");

        private static readonly By SelectQuestionsTag = By.XPath("//div//label//span[contains(text(),'Select Questions')] | //div//label//span//font//font[contains(text(),'Select Questions')]");

        //Methods
        public bool IsSelectQuestionsTitleDisplayed()
        {
            return Driver.IsElementDisplayed(SelectQuestionPageTitle);
        }

        public bool IsQuestionSectionDisplayed(string questionName)
        {
            return Driver.IsElementDisplayed(QuestionSection(questionName));
        }

        public bool IsQuestionCheckboxSelected(string questionName)
        {
            Driver.JavaScriptScrollToElement(QuestionCheckbox(questionName), false);
            return Driver.IsElementSelected(QuestionCheckbox(questionName));
        }

        public void ExpandCollapseQuestion(string questionName)
        {
            Log.Step(nameof(SelectQuestionsPage), $"Click On '{questionName}'");
            Wait.UntilElementClickable(QuestionExpandCollapseIcon(questionName)).Click();
            Wait.UntilJavaScriptReady();
        }

        public void CheckUncheckQuestionCheckbox(string questionName, bool check = true)
        {
            Log.Step(nameof(SelectQuestionsPage), $"Select '{questionName}'");
            Driver.JavaScriptScrollToElement(SelectQuestionsTag);
            Wait.HardWait(2000); //Wait till element found
            Wait.UntilElementExists(QuestionCheckbox(questionName)).Check(check);
        }

        public List<string> GetAllQuestionsByCompetency(string competency)
        {
            Log.Step(nameof(SelectQuestionsPage), $"Get all questions by '{competency}'");
            return Wait.UntilAllElementsLocated(QuestionsByCompetency(competency)).Select(e => Driver.JavaScriptScrollToElement(e).GetText().Replace("\r", "").Replace("\n", "").Replace(" ", "")).ToList();
        }
        public int GetTotalCountOfQuestions()
        {
            Log.Step(nameof(SelectQuestionsPage), "Get total number of questions");
            return Wait.UntilAllElementsLocated(AllQuestionsCount).Count;
        }

        public void FillForm(AtCommon.Dtos.Assessments.PulseV2.Custom.PulseAssessmentV2 assessmentInfo)
        {
            Log.Step(nameof(SelectQuestionsPage), "Select Questions 'Dimension', 'SubDimension' and 'Competency'");
            if (assessmentInfo.Questions == null) return;
            foreach (var question in assessmentInfo.Questions)
            {
                switch (question.QuestionSelectionPref)
                {
                    case QuestionSelectionPreferences.Dimension:
                        CheckUncheckQuestionCheckbox(question.DimensionName);
                        break;
                    case QuestionSelectionPreferences.SubDimension:
                        ExpandCollapseQuestion(question.DimensionName);
                        CheckUncheckQuestionCheckbox(question.SubDimensionName);
                        break;
                    case QuestionSelectionPreferences.Competency:
                        ExpandCollapseQuestion(question.DimensionName);
                        ExpandCollapseQuestion(question.SubDimensionName);
                        CheckUncheckQuestionCheckbox(question.CompetencyName);
                        break;
                    case QuestionSelectionPreferences.All:
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

        }
    }
}
