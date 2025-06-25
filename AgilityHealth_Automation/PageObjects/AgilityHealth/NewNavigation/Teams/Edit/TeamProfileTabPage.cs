using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.DataObjects.NewNavigation.Teams;
using AgilityHealth_Automation.PageObjects.AgilityHealth.NewNavigation.Teams.Base;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.NewNavigation.Teams.Edit
{
    public class TeamProfileTabPage : TeamBasePage
    {
        public TeamProfileTabPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        #region Locators

        #region Team Profile Tab

        private readonly By ProfilePreferredLanguageDropDown = By.XPath("//span[@aria-owns='IsoLanguageCode_listbox']/span/span[1]");
        private readonly By ProfileMethodologyDropDown = By.XPath("//span[@aria-owns='MethodologyId_listbox']/span/span[1]");
        private readonly By TeamProfileTab = By.XPath("//a[text()='Team Profile']");

        #endregion

        #endregion


        #region Methods

        #region Team Profile Tab

        public Team GetTeamInfo()
        {
            var teamInfo = new Team
            {
                TeamName = Wait.UntilElementVisible(TeamName).GetElementAttribute("value"),
                WorkType = Wait.UntilElementExists(ProfileWorkTypeDropDown).GetText(),
                PreferredLanguage = Wait.UntilElementExists(ProfilePreferredLanguageDropDown).GetText(),
                Methodology = Wait.UntilElementExists(ProfileMethodologyDropDown).GetText(),
                DepartmentAndGroup = Wait.UntilElementExists(DepartmentOrGroupTextbox).GetElementAttribute("value"),
                DateEstablished = Wait.UntilElementExists(DateEstablishedTextbox).GetElementAttribute("value"),
                TeamBioOrBackground = Wait.UntilElementExists(TeamBioOrBackgroundTextArea).GetElementAttribute("value"),
                ExternalIdentifier = Wait.UntilElementExists(ExternalIdentifierTextBox).GetElementAttribute("value"),
            };

            return teamInfo;
        }
        public new Team GetMultiTeamInfo()
        {
            var teamInfo = new Team
            {
                TeamName = Wait.UntilElementVisible(TeamName).GetElementAttribute("value"),
                WorkType = Wait.UntilElementExists(ProfileWorkTypeDropDown).GetText(),
                DepartmentAndGroup = Wait.UntilElementExists(DepartmentOrGroupTextbox).GetElementAttribute("value"),
                TeamBioOrBackground = Wait.UntilElementExists(TeamBioOrBackgroundTextArea).GetElementAttribute("value"),
                ExternalIdentifier = Wait.UntilElementExists(ExternalIdentifierTextBox).GetElementAttribute("value"),
            };

            return teamInfo;
        }
        public Team GetEnterpriseTeamInfo()
        {
            var teamInfo = new Team
            {
                TeamName = Wait.UntilElementVisible(TeamName).GetElementAttribute("value"),
                DepartmentAndGroup = Wait.UntilElementExists(DepartmentOrGroupTextbox).GetElementAttribute("value"),
                TeamBioOrBackground = Wait.UntilElementExists(TeamBioOrBackgroundTextArea).GetElementAttribute("value"),
                ExternalIdentifier = Wait.UntilElementExists(ExternalIdentifierTextBox).GetElementAttribute("value"),
            };

            return teamInfo;
        }

        #endregion
        public void NavigateToPage(int companyId, int teamId)
        {
            var newNavUrl = $"{BaseTest.ApplicationUrl}/teams/editv2/${teamId}?parentIds=[{companyId},{teamId}]";
            NavigateToUrl(newNavUrl);
            Wait.UntilJavaScriptReady();

        }
        public void ClickOnTeamProfileTab()
        {
            Log.Step(nameof(SubTeamTabPage), "Click on 'Sub Teams' tab");
            Wait.UntilElementClickable(TeamProfileTab).Click();
        }

        #endregion
    }
}