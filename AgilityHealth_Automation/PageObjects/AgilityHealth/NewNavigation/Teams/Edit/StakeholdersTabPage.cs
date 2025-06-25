using AgilityHealth_Automation.PageObjects.AgilityHealth.NewNavigation.Teams.Base;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.NewNavigation.Teams.Edit
{
    public class StakeholdersTabPage : StakeholdersBasePage
    {
        public StakeholdersTabPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        #region Locators

        #region Stakeholders Tab

        private readonly By StakeholdersTab = By.Id("stakeHoldersTab_click");

        #endregion

        #endregion


        #region Methods

        #region Stakeholer Tab

        public void ClickOnStakeHolderTab()
        {
            Log.Step(nameof(StakeholdersTabPage), "Click on 'Stakeholders' tab");
            Wait.UntilElementClickable(StakeholdersTab).Click();
        }

        #endregion

        #endregion
    }
}