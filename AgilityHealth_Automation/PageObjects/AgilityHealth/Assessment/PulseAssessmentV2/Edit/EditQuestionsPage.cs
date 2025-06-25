using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.PulseAssessmentV2.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.PulseAssessmentV2.Create;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.PulseAssessmentV2.Edit
{
    public class EditQuestionsPage: SelectQuestionsBasePage
    {
        public EditHeaderPage Header { get; set; }
        public EditQuestionsPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
            Header = new EditHeaderPage(driver, log);
        }


        private static readonly By AssessmentType = By.XPath("//div[text()='Assessment Type']/ancestor::label/following-sibling::div");
        private readonly By QuestionSelection = By.XPath("//div[text()='Select Questions']");
        private static By QuestionsByCompetencyAfterPublish(string competencyName) => By.XPath($"//*[text()='{competencyName}']/parent::div//div/following-sibling::div");
        private static readonly By CompetencySection = By.XPath("//div[@id='simple-tabpanel-1']//div[@class='breakHereForQuestion']//parent::span/preceding-sibling::div");

        public string GetAssessmentType()
        {
            Log.Step(nameof(EditQuestionsPage), "Get assessment type");
            return Wait.UntilElementVisible(AssessmentType).GetText();
        }

        public bool IsSelectQuestionsTextDisplayed()
        {
            return Driver.IsElementPresent(QuestionSelection);
        }

        public List<string> GetAllQuestionsByCompetencyAfterPublish(string competencyName)
        {
            Log.Step(nameof(SelectQuestionsPage), $"Get all questions by '{competencyName}' after publish the assessment");
            Driver.JavaScriptScrollToElement(QuestionsByCompetencyAfterPublish(competencyName), false);
            return Wait.UntilAllElementsLocated(QuestionsByCompetencyAfterPublish(competencyName)).Select(e => Driver.JavaScriptScrollToElement(e).GetText().Replace("\r", "").Replace("\n", "").Replace(" ", "")).ToList();
        }

        public bool IsCompetencyDisplayed(string competencyName)
        {
            var competency = Wait.UntilAllElementsLocated(CompetencySection).Select(a => a.GetText().Split(new[] { ":" }, StringSplitOptions.None)[1].Replace("Roles", "Roles ")).ToList();
            return competency.Contains(competencyName);
        }
    }
}