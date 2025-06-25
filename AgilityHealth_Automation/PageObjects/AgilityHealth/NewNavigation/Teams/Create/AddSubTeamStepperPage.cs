using AgilityHealth_Automation.PageObjects.AgilityHealth.NewNavigation.Teams.Base;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.NewNavigation.Teams.Create
{
    public class AddTeamSubTeamStepperPage : SubTeamBasePage
    {
        public AddTeamSubTeamStepperPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }
        #region Locators

        #region Sub Team Stepper
        // 'Continue' button
        private readonly By ContinueToAddLeadersButton = By.XPath("//div//input[@value='Continue to Add Leaders']");
        #endregion

        #endregion


        #region Methods

        #region Sub Team Stepper
        //'Continue' button
        public void ClickOnContinueToAddLeadersButton()
        {
            Log.Step(nameof(AddTeamSubTeamStepperPage), "Click on 'Continue to Add Leaders' button");
            Wait.UntilElementClickable(ContinueToAddLeadersButton).Click();
            Wait.HardWait(1000); //Need to wait until popup get closed
        }
        #endregion

        #endregion
    }
}