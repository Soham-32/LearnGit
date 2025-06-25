using AgilityHealth_Automation.PageObjects.AgilityHealth.NewNavigation.Teams.Base;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.NewNavigation.Teams.Edit
{
    public class SubTeamTabPage : SubTeamBasePage
    {
        public SubTeamTabPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }
        #region Locators
        #region Sub Teams Tab
        private readonly By SubTeamTab = By.XPath("//button[text()='Sub-Teams']");
        private readonly By EditSubTeamsLink = By.Id("EditSubTeams");
        private readonly By UpdateSubTeamsButton = By.Id("btnAddSubTeams");
        #endregion

        #region Sub Teams grid

        private static By TeamNameFromGrid(string teamName) =>
            By.XPath($"//td/a[text()='{teamName}']");
        #endregion

        #endregion


        #region Methods

        #region Sub Teams Tab
        public void ClickOnSubTeamTab()
        {
            Log.Step(nameof(SubTeamTabPage), "Click on 'Sub Teams' tab");
            Wait.UntilElementClickable(SubTeamTab).Click();
        }

        public void ClickOnEditSubTeams()
        {
            Log.Step(nameof(SubTeamTabPage), "Click on Edit Sub-Teams");
            Wait.UntilElementVisible(EditSubTeamsLink).Click();
        }

        public void ClickOnUpdateSubTeam()
        {
            Log.Step(nameof(SubTeamTabPage), "Click on Update Sub Team button");
            Wait.UntilElementVisible(UpdateSubTeamsButton).Click();
            Wait.UntilJavaScriptReady();
        }
        #endregion

        #region Sub Teams grid

        public bool IsSubTeamDisplayed(string teamName)
        {
            return Driver.IsElementDisplayed(TeamNameFromGrid(teamName));
        }
        #endregion

        #endregion
    }
}