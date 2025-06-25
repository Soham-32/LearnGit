using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Assessment.Scheduler
{
    public class SchedulerTabPage : AssessmentDashboardBasePage
    {
        public SchedulerTabPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        private readonly By SendBatchAssessmentButton = By.CssSelector("a[onclick='OpenSendBatchPopup();']");
        private readonly By CreateDraftAssessmentButton = By.CssSelector("a[onclick='DraftAnAssessment();']");
        private readonly By AssessmentSchedulerTitle = By.XPath("//h1[text()='Assessment Scheduler'] | //h1//*[text()='Assessment Scheduler']");

        // Create Draft Assessment popup
        private readonly By FindAFacilitatorCheckbox = By.Id("foundFacilitator");
        private readonly By CancelButton = By.CssSelector("a.k-scheduler-cancel");

        public void ClickSendBatchAssessment()
        {
            Log.Step(nameof(SchedulerTabPage), "Click on Send Batch Assessment button");
            Wait.UntilElementClickable(SendBatchAssessmentButton).Click();
        }

        public void ClickCreateDraftAssessment()
        {
            Log.Step(nameof(SchedulerTabPage), "Click on Create Draft Assessment button");
            Wait.UntilElementClickable(CreateDraftAssessmentButton).Click();
        }

        public void ClickCancelButton()
        {
            Log.Step(nameof(SchedulerTabPage), "Click on Cancel button");
            Wait.UntilElementClickable(CancelButton).Click();
            Wait.UntilJavaScriptReady();
        }

        public bool IsFindAFacilitatorCheckboxVisible()
        {
            return Driver.IsElementDisplayed(FindAFacilitatorCheckbox);
        }

        public string GetAssessmentSchedulerTitle()
        {
            return Wait.UntilElementVisible(AssessmentSchedulerTitle).GetText();
        }

        public void NavigateToPage(int companyId)
        {
            NavigateToUrl($"{BaseTest.ApplicationUrl}/company/{companyId}/AssessmentSchedulerDashboard");
        }

        public void NavigateToSchedulerTabForProd(string env, int companyId)
        {
            NavigateToUrl($"https://{env}.agilityinsights.ai/company/{companyId}/AssessmentSchedulerDashboard");
        }

    }
}
