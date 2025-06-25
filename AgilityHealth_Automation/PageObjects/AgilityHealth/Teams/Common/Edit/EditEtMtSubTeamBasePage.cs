using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Common.Edit
{
    internal class EditEtMtSubTeamBasePage : BasePage
    {
        public EditEtMtSubTeamBasePage(IWebDriver driver, ILogger log) : base(driver, log) { }

        private readonly By SubTeamsSearchBox = By.XPath("//input[@placeholder='Filter Sub-Teams']");
        private static By SelectSubTeam(string subTeamName) => By.XPath($"//div[@id='SubAllteamsGrid']//td[@class='chkbxAllGrid']//font//font[text()='{subTeamName}'] | //div[@id='SubAllteamsGrid']//td[@class='chkbxAllGrid'][text()='{subTeamName}']");
        private static By SelectedSubTeam(string subTeamName) => By.XPath($"//input[@class='chkbxAllGrid subteams']/../following-sibling::td[@class='chkbxAllGrid'][text()='{subTeamName}']");

        private readonly By SelectedSubTeamList = By.XPath("//div[@id='SubSelectedteamsGrid']/div[@class='k-grid-content']//td[@class='chkbxAllGrid']");
        private readonly By UpdateSubTeamButton = By.Id("btnAddSubTeams");

        public void SelectSubTeamViaSearchBox(string subTeamName)
        {
            Log.Step(nameof(EditEtMtSubTeamBasePage), $"On Edit Team page, Search {subTeamName} and select");
            Wait.UntilElementVisible(SubTeamsSearchBox).SetText(subTeamName);
            Wait.UntilJavaScriptReady();
            Wait.UntilElementExists(SelectSubTeam(subTeamName)).Click();
        }
        public void ClickUpdateSubTeamButton()
        {
            Log.Step(nameof(EditEtMtSubTeamBasePage), "On Edit Team page, Sub-Teams tab, click Update Sub-Teams button");
            Wait.UntilElementClickable(UpdateSubTeamButton).Click();
            Wait.HardWait(3000); // Wait till page load successfully.
        }
        public List<string> GetSelectedSubTeamList()
        {
            return Wait.UntilAllElementsLocated(SelectedSubTeamList).Select(a => a.GetText()).ToList();
        }
        public void RemoveSubTeam(string subTeamName) => Wait.UntilElementClickable(SelectedSubTeam(subTeamName)).Click();

        public void NavigateToPage(string teamType,int teamId)
        {
            Log.Step(GetType().Name, $"Navigate to sub teams edit page, team id {teamId}");
            NavigateToUrl($"{BaseTest.ApplicationUrl}/{teamType}/{teamId}/subteamsedit");
        }

    }
}
