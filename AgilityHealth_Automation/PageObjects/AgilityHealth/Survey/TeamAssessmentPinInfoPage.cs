using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.Utilities;
using AtCommon.Dtos.Assessments.Team.Custom;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Survey
{
    internal class TeamAssessmentPinInfoPage : BasePage
    {
        public TeamAssessmentPinInfoPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }
        private readonly By PinTextBox = By.Id("AssessmentPin");
        private readonly By EmailTextBox = By.Id("EmailAddress");
        private readonly By TakeAssessmentButton = By.XPath("//input[@type='submit']");

        public void FillAssessmentAccessDetails(AssessmentPinRequest request)
        {
            Log.Step(nameof(TeamAssessmentPinInfoPage), "Fill data for Assessment Access");
            Wait.UntilElementClickable(PinTextBox).SetText(request.Pin);
            Wait.UntilElementClickable(EmailTextBox).SetText(request.Email);
            Wait.UntilElementClickable(TakeAssessmentButton).Click();
        }
    }
}