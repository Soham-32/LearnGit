using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Common.Members;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Team.Edit
{
    public class EditTeamStakeHolderPage : StakeHolderCommon
    {


        public EditTeamStakeHolderPage(IWebDriver driver, ILogger log) : base(driver, log) { }

        private readonly By UpdateButton = By.CssSelector("a.k-grid-update");
        
        public void ClickUpdateButton()
        {
            Log.Step(GetType().Name, "On Edit Stakeholder popup, click Update button");
            Wait.UntilElementClickable(UpdateButton).Click();
            Wait.UntilElementNotExist(UpdateButton);
        }

    }
}
