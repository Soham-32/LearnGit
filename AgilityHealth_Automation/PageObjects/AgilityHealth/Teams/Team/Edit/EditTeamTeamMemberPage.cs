using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Common.Members;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Team.Edit
{
    public class EditTeamTeamMemberPage : TeamMemberCommon
    {

        public EditTeamTeamMemberPage(IWebDriver driver, ILogger log) : base(driver, log) { }

        public By UpdateButton = By.CssSelector("a.k-grid-update");
        private readonly By TeamMembersLoadIcon = By.XPath("//div[@class='k-loading-image']");

        public void ClickUpdateButton()
        {
            Log.Step(GetType().Name, "On Edit Team Member popup, click Update button");
            Wait.UntilElementClickable(UpdateButton).Click();
            Wait.UntilElementNotExist(UpdateButton);
        }
        public void WaitForTeamMembersPageToLoad()
        {
            Wait.UntilElementNotExist(TeamMembersLoadIcon);
        }
    }
}
