using AgilityHealth_Automation.DataObjects.NewNavigation.Teams;
using AgilityHealth_Automation.PageObjects.AgilityHealth.NewNavigation.Teams.Base;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.NewNavigation.Teams.Create
{
    public class ReviewStepperPage : ReviewBasePage
    {
        public ReviewStepperPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }
        #region Locators


        #region Team Members
        private readonly By TeamMembersEditButton = By.XPath("//div[@id='headingTwo']//a");
        private readonly By TeamMembersSection = By.XPath("//div[@id='headingTwo']/h4");
        #endregion

        #region Stakeholder
        private readonly By EditStakeholderButton = By.XPath("//div[@aria-label='Stakeholders']//a[text()='Edit']");
        private readonly By StakeholderSection = By.XPath("//div[@id='headingSix']/h4");
        #endregion

        #region Review Stepper
        private readonly By FinishButton = By.XPath("//span/a[text()='Finish']");
        #endregion

        #endregion



        #region Methods

        #region Team Profile
        public string GetExpectedTeamProfileText(Team teamInfo)
        {
            return $"Team Profile\r\nEdit\r\n\r\n\r\n\r\nTeam Name:\r\nWork Type:\r\n{teamInfo.TeamName}\r\n{teamInfo.WorkType}\r\nPreferred Language:\r\nMethodology\r\n{teamInfo.PreferredLanguage}\r\n{teamInfo.Methodology}\r\nExternal Identifier:\r\nDepartment / Group:\r\n{teamInfo.ExternalIdentifier}\r\n{teamInfo.DepartmentAndGroup}\r\nDate Established:\r\n{teamInfo.DateEstablished}\r\nTeam BIO / Background:\r\n{teamInfo.TeamBioOrBackground}";
        }
        public string GetExpectedMultiTeamProfileText(Team multiTeamInfo)
        {
            return $"Team Profile\r\nEdit\r\n\r\n\r\n\r\nTeam Name:\r\nMulti-team Work Type:\r\n{multiTeamInfo.TeamName}\r\n{multiTeamInfo.WorkType}\r\n{multiTeamInfo.PreferredLanguage}\r\n{multiTeamInfo.Methodology}\r\nExternal Identifier:\r\nDepartment / Group:\r\n{multiTeamInfo.ExternalIdentifier}\r\n{multiTeamInfo.DepartmentAndGroup}\r\n{multiTeamInfo.DateEstablished}\r\nTeam BIO / Background:\r\n{multiTeamInfo.TeamBioOrBackground}";
        }
        public string GetExpectedPortfolioTeamProfileText(Team portfolioTeamInfo)
        {
            return $"Team Profile\r\nEdit\r\n\r\n\r\n\r\nTeam Name:\r\nPortfolioTeamWorkType:\r\n{portfolioTeamInfo.TeamName}\r\n{portfolioTeamInfo.WorkType}\r\n{portfolioTeamInfo.PreferredLanguage}\r\n{portfolioTeamInfo.Methodology}\r\nExternal Identifier:\r\nDepartment / Group:\r\n{portfolioTeamInfo.ExternalIdentifier}\r\n{portfolioTeamInfo.DepartmentAndGroup}\r\n{portfolioTeamInfo.DateEstablished}\r\nTeam BIO / Background:\r\n{portfolioTeamInfo.TeamBioOrBackground}";
        }
        public string GetExpectedEnterpriseTeamProfileText(Team enterpriseTeamInfo)
        {
            return $"Team Profile\r\nEdit\r\n\r\n\r\n\r\nTeam Name:\r\nTeam Type:\r\n{enterpriseTeamInfo.TeamName}\r\n{enterpriseTeamInfo.WorkType}\r\n{enterpriseTeamInfo.PreferredLanguage}\r\n{enterpriseTeamInfo.Methodology}\r\nExternal Identifier:\r\nDepartment / Group:\r\n{enterpriseTeamInfo.ExternalIdentifier}\r\n{enterpriseTeamInfo.DepartmentAndGroup}\r\n{enterpriseTeamInfo.DateEstablished}\r\nTeam BIO / Background:\r\n{enterpriseTeamInfo.TeamBioOrBackground}";
        }
        #endregion

        #region Team Members
        public void ClickOnTeamMembersEditButton()
        {
            Log.Step(nameof(ReviewStepperPage), "Click on 'Edit Team Member' button");
            Driver.JavaScriptScrollToElement(TeamMembersEditButton);
            Wait.UntilJavaScriptReady();
            Wait.UntilElementClickable(TeamMembersEditButton).Click();
        }
        public void ClickOnTeamMembersSection()
        {
            Log.Step(nameof(ReviewStepperPage), "Click on 'Team Members' header section");
            Driver.JavaScriptScrollToElement(TeamMembersSection);
            Wait.HardWait(3000);
            Wait.UntilElementClickable(TeamMembersSection).Click();
            Wait.UntilJavaScriptReady();
        }

        public bool IsTeamMembersSectionExpanded()
        {
            return bool.Parse(Wait.UntilElementExists(TeamMembersSection).GetAttribute("aria-expanded"));
        }
        #endregion

        #region Stakeholder
        public void ClickOnEditStakeholdersButton()
        {
            Log.Step(nameof(ReviewStepperPage), "Click on 'Edit Stakeholder' button");
            Driver.JavaScriptScrollToElement(EditStakeholderButton);
            Wait.UntilJavaScriptReady();
            Wait.UntilElementClickable(EditStakeholderButton).Click();
            Wait.UntilJavaScriptReady();
        }
        public void ClickOnStakeholdersSection()
        {
            Log.Step(nameof(ReviewStepperPage), "Click on 'Stakeholder' header section");
            Driver.JavaScriptScrollToElement(StakeholderSection);
            Wait.HardWait(3000); // Wait till page scrolling
            Wait.UntilElementClickable(StakeholderSection).Click();
            Wait.HardWait(3000); // Wait till stakeholder section is opened
        }

        public bool IsStakeholdersSectionExpanded()
        {
            return bool.Parse(Wait.UntilElementExists(StakeholderSection).GetAttribute("aria-expanded"));
        }
        #endregion

        #region Review Stepper

        public void ClickOnFinishButton()
        {
            Log.Step(nameof(ReviewStepperPage), "Click on 'Finish' button");
            Driver.JavaScriptScrollToElement(FinishButton);
            Wait.HardWait(3000); //Need to wait until finish button located
            Wait.UntilElementClickable(FinishButton).Click();
            Wait.HardWait(2000); //Need to wait until profile tab is loaded
        }

        #endregion


        #endregion
    }
}
