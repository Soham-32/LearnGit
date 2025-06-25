using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.PulseAssessmentV2.Base;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.PulseAssessmentV2.Edit
{
    public class EditPulseCheckPage : PulseCheckBasePage
    {
        public EditHeaderPage Header { get; set; }

        public EditPulseCheckPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
            Header = new EditHeaderPage(driver, log);
        }


        private readonly By AssessmentTypeBox = AutomationId.Equals("assessmentType", "input");


        public string GetAssessmentType()
        {
            Log.Step(GetType().Name, "Get 'AssessmentType'");
            return Wait.UntilElementExists(AssessmentTypeBox).GetAttribute("value");
        }
    }
}
