using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.Tabs
{
    public class InitiativesTabPage : BaseTabPage
    {
        public InitiativesTabPage(IWebDriver driver, ILogger log) : base(driver, log) { }


        #region InitiativesTab

        private readonly By InitiativesTabButton = By.XPath("//button[text()='InitiativesTab']");
        #endregion



        #region InitiativesTab
        public void ClickOnInitiativesTab()
        {
            Log.Step(nameof(InitiativesTabPage), "Switch to Initiative tab");
            Wait.UntilElementExists(InitiativesTabButton).Click();
        }

        #endregion
    }
}
