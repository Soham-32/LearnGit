using AgilityHealth_Automation.PageObjects.AgilityHealth.NewNavigation.Teams.Base;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.NewNavigation.Teams.Create
{
    public class CreateTeamStepperPage : TeamBasePage
    {
        public CreateTeamStepperPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }
        #region Locators

        #endregion



        #region Methods

        #region Create Team stepper

        public void RemoveTeamName()
        {
            Wait.UntilElementClickable(TeamName).Clear();
        }
        #endregion

        #endregion

    }
}
