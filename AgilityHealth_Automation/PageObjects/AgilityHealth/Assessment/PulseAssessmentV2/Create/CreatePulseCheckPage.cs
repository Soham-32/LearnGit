using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.PulseAssessmentV2.Base;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.PulseAssessmentV2.Create
{
    public class CreatePulseCheckPage : PulseCheckBasePage

    {
        public CreateHeaderPage Header { get; set; }

        public CreatePulseCheckPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
            Header = new CreateHeaderPage(driver, log);
        }

        //Locators
        private readonly By AssessmentTypeListBox = AutomationId.Equals("assessmentType", "div");
        private static By AssessmentTypeListItem(string item) => By.XPath($"//div[@id = 'menu-assessmentType']//*[text() = '{item}']");

        //Methods
        public void SelectAssessmentType(string assessmentType)
        {
            Log.Step(GetType().Name, $"Select {assessmentType}");
            SelectItem(AssessmentTypeListBox, AssessmentTypeListItem(assessmentType));
        }
        public string GetAssessmentType()
        {
            Log.Step(GetType().Name, "Get 'AssessmentType'");
            return Wait.UntilElementVisible(AssessmentTypeListBox).GetText();
        }

    }
}