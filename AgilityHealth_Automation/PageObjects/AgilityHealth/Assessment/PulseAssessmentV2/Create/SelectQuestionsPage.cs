using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.PulseAssessmentV2.Base;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.PulseAssessmentV2.Create
{
    public class SelectQuestionsPage : SelectQuestionsBasePage
    {
        public CreateHeaderPage Header { get; set; }

        public SelectQuestionsPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
            Header = new CreateHeaderPage(driver, log);
        }
    }
}
