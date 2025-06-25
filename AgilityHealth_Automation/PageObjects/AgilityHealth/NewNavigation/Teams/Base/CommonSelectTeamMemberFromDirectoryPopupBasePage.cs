using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;
using System.Collections.Generic;
using System.Threading;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.NewNavigation.Teams.Base
{
    public class CommonSelectTeamMemberFromDirectoryPopupBasePage : TeamBasePage
    {

        public CommonSelectTeamMemberFromDirectoryPopupBasePage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        #region Locators

        #region Select Team Member(s) Popup
        private readonly By TeamsTab = By.XPath("//a[text()='Teams']");
        private static By MemberOrTeamCheckBox(string name) => By.XPath($"//td[text()='{name}']//preceding-sibling::td//input");
        #endregion

        #region Members Tab
        private readonly By AddToTeamMembersButton = By.XPath("//button[contains(@id,'btnAddMembertm') or contains(@id,'btnAddMembersh')]");
        private readonly By MembersTabSearchTextbox = By.XPath("//input[contains(@id,'filterMemberstm') or contains(@id,'filterMemberssh')]");
        #endregion

        #region Teams Tab
        private readonly By AddToTeamButton = By.XPath("//button[contains(@id,'btnAddTeamtm') or contains(@id,'btnAddTeamsh')]");
        private readonly By TeamsTabSearchTextbox = By.XPath("//input[contains(@id,'filterTeamstm') or contains(@id,'filterTeamssh')]");
        #endregion

        #endregion


        #region Methods

        #region Select Team Member(s) Popup
        public void SelectMemberOrTeam(string memberEmailOrTeamName)
        {
            Log.Step(nameof(CommonSelectTeamMemberFromDirectoryPopupBasePage), $"Select member or team : {memberEmailOrTeamName}");
            Wait.UntilElementVisible(MemberOrTeamCheckBox(memberEmailOrTeamName));
            Thread.Sleep(500);//Wait till Member's or Team's checkbox is displayed
            Wait.UntilElementClickable(MemberOrTeamCheckBox(memberEmailOrTeamName)).Check();
        }
        public void ClickOnTeamsTab()
        {
            Log.Step(nameof(CommonSelectTeamMemberFromDirectoryPopupBasePage), "Click on teams tab");
            Wait.UntilElementClickable(TeamsTab).Click();
        }
        #endregion

        #region Members Tab
        public void SearchMembers(string email)
        {
            Log.Step(nameof(CommonSelectTeamMemberFromDirectoryPopupBasePage), $"Search {email} Member");
            Wait.UntilElementClickable(MembersTabSearchTextbox).SetText(email);
        }
        public void ClickOnAddSelectedTeamMembersButton()
        {
            Log.Step(nameof(CommonSelectTeamMemberFromDirectoryPopupBasePage), "Click on 'Add Selected Team Members' button for members");
            Wait.UntilElementClickable(AddToTeamMembersButton).Click();
            Wait.UntilJavaScriptReady();
        }
        public void AddMembersFromDirectory(List<string> emailAddresses)
        {
            Log.Step(nameof(CommonSelectTeamMemberFromDirectoryPopupBasePage), "Add members from directory");
            foreach (var email in emailAddresses)
            {
                SearchMembers(email);
                SelectMemberOrTeam(email);
            }
            ClickOnAddSelectedTeamMembersButton();
        }

        #endregion

        #region Teams Tab
        public void AddTeamsFromDirectory(List<string> teamNameList)
        {
            Log.Step(nameof(CommonSelectTeamMemberFromDirectoryPopupBasePage), "Click on add from directory, Click on teams tab, Select teams and click on Add");
            ClickOnTeamsTab();
            foreach (var teamName in teamNameList)
            {
                ClickOnSearchTeams(teamName);
                SelectMemberOrTeam(teamName);
            }
            ClickOnAddSelectedTeamsButton();
        }
        public void ClickOnSearchTeams(string teamName)
        {
            Log.Step(nameof(CommonSelectTeamMemberFromDirectoryPopupBasePage), $"Search {teamName} teams");
            Wait.UntilElementClickable(TeamsTabSearchTextbox).SetText(teamName);
        }
        public void ClickOnAddSelectedTeamsButton()
        {
            Log.Step(nameof(CommonSelectTeamMemberFromDirectoryPopupBasePage), $"Click on 'Add Selected Team Members' button for teams");
            Wait.UntilElementClickable(AddToTeamButton).Click();
            Wait.UntilJavaScriptReady();
        }
        #endregion

        #endregion

    }
}