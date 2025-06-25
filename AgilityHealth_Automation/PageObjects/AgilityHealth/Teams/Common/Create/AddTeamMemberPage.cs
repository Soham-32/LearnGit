using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Common.Members;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Common.Create
{

    public class AddTeamMemberPage : TeamMemberCommon
    {

        public AddTeamMemberPage(IWebDriver driver, ILogger log) : base(driver, log) { }


        private readonly By ContinueToStakeHolderBtn = By.Id("add_stakeholder");
       
        public void ClickContinueToAddStakeHolder()
        {
            Log.Step(nameof(AddTeamMemberPage), "On Add Team Member page, click 'Continue to add Stake Holder' button");
            Wait.UntilJavaScriptReady();
            Wait.UntilElementVisible(ContinueToStakeHolderBtn);
            Wait.UntilElementClickable(ContinueToStakeHolderBtn).Click();
        }

        public bool IsAddTeamMemberPageDisplayed()
        {
            return Driver.IsElementDisplayed(ContinueToStakeHolderBtn);
        }

        public void NavigateToTeamPage(int teamId)
        {
            NavigateToUrl($"{BaseTest.ApplicationUrl}/teammembers/{teamId}");
        }
    }
}
