using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Common.Edit;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.N_Tier
{
    public class CreateNTierPage : BasePage
    {
        public CreateNTierPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        private readonly By NTierTeamNameField = By.Id("TeamName");
        private readonly By NTierDoneButton = By.Id("done");
        private readonly By GridValues = By.XPath("//td[@class='chkbxAllGrid']");

        public void InputNTierTeamName(string teamName)
        {
            Log.Step(nameof(CreateNTierPage), "Input N-Tier team name");
            Wait.UntilElementClickable(NTierTeamNameField).ClearTextbox();
            Wait.UntilElementClickable(NTierTeamNameField).SetText(teamName);
        }

        public void ClickCreateButton()
        {
            Log.Step(nameof(CreateNTierPage), "Click 'Create Team & Add Sub-Teams' button to move to the next page");
            Wait.UntilElementClickable(NTierDoneButton).Click();
        }

        internal List<string> GetListOfSubteam()
        {
            Log.Step(nameof(CreateNTierPage), "Get list of subteams");
            var displayedElements = Wait.UntilAllElementsLocated(GridValues).Where(e => e.Displayed);

            return displayedElements.Select(e => e.GetText()).ToList();
        }

        public void CreateNTierTeamWithSubTeam(string teamName)
        {
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var createNTierPage = new CreateNTierPage(Driver, Log);
            var editEtMtSubTeamBasePage = new EditEtMtSubTeamBasePage(Driver, Log);

            Log.Step(nameof(CreateNTierPage), "Create N-Tier team & Add sub team");
            dashBoardPage.ClickAddATeamButton();
            dashBoardPage.SelectTeamType(TeamType.NTier);
            dashBoardPage.ClickAddTeamButton();
            createNTierPage.InputNTierTeamName(teamName);
            createNTierPage.ClickCreateButton();
            editEtMtSubTeamBasePage.ClickUpdateSubTeamButton();
        }
    }
}