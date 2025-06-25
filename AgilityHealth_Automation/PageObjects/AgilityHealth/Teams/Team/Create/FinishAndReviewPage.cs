using System;
using System.Collections.Generic;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.DataObjects;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Team.Create
{
    public class FinishAndReviewPage : BasePage

    {
        public FinishAndReviewPage(IWebDriver driver, ILogger log) : base(driver, log) { }

        private readonly By TeamName = By.XPath("//label[@for='TeamName']/following-sibling::span");
        private readonly By WorkType = By.XPath("//label[@for='WorkType']/following-sibling::span");
        private readonly By PreferredLanguage = By.XPath("//label[@for='IsoLanguageName']/following-sibling::span");
        private readonly By ExternalIdentifier = By.XPath("//label[@for='ExternalIdentifier']/following-sibling::span");
        private readonly By Methodology = By.XPath("//label[text()='Methodology']/following-sibling::span");
        private readonly By Department = By.XPath("//label[@for='Department']/following-sibling::span");
        private readonly By DateEstablished = By.XPath("//label[@for='DateEstablished']/following-sibling::span");
        private readonly By AgileAdoptionDate = By.XPath("//label[@for='AgileAdoptionDate']/following-sibling::span");
        private readonly By Description = By.XPath("//label[@for='Description']/following-sibling::span");
        private readonly By Biography = By.XPath("//label[@for='Biography']/following-sibling::span");
        private readonly By TeamImage = By.XPath("//div[@class='team-avatar']/img");
        private readonly By MultiTeamType = By.XPath("//label[text()='MultiTeam Type']/following-sibling::span");
        private readonly By EnterpriseTeamType = By.XPath("//label[text()='Enterprise Team Type']/following-sibling::span");
        private readonly By GoToTeamDashboardButton = By.LinkText("Go To Team Dashboard");
        private readonly By GoToTeamAssessmentDashboardButton = By.LinkText("Go To Assessment Dashboard");

        private readonly By SubTeamTeamNames = By.XPath("//h5[contains(.,'Sub-Teams')]/following-sibling::table[@id='stakeholders']/tbody/tr/td[2]");
        private static By ClickHereLink(string sectionName) => By.XPath($"//div[@class='contents']//p[contains(text(),'{sectionName}')]//a");
        private static By EditLink(string sectionName) => By.XPath($"//h5[contains(text(),'{sectionName}')]//a");

        public TeamMemberInfo GetTeamMemberFromGrid(int rowIndex)
        {
            string firstName = Wait.UntilElementVisible(
                By.XPath($"//table[@id='team_members']/tbody/tr[{rowIndex}]/td[2]")).Text;
            string lastName = Wait.UntilElementVisible(
                By.XPath($"//table[@id='team_members']/tbody/tr[{rowIndex}]/td[3]")).Text;
            string emailRow = Wait.UntilElementVisible(
                By.XPath($"//table[@id='team_members']/tbody/tr[{rowIndex}]/td[4]")).Text;
            string rolesRow = Wait.UntilElementVisible(
                By.XPath($"//table[@id='team_members']/tbody/tr[{rowIndex}]/td[5]")).Text;
            string participantGroupsRow = Wait.UntilElementVisible(
                By.XPath($"//table[@id='team_members']/tbody/tr[{rowIndex}]/td[5]")).Text;

            var teamMemberInfo = new TeamMemberInfo
            {
                FirstName = firstName,
                LastName = lastName,
                Email = emailRow,
                Role = rolesRow,
                ParticipantGroup = participantGroupsRow
            };

            return teamMemberInfo;
        }

        public StakeHolderInfo GetStakeHolderFromGrid(int rowIndex)
        {
            Wait.UntilJavaScriptReady();
            string firstNameRow = Wait.UntilElementVisible(
                By.XPath($"//h5[contains(.,'Stakeholders')]/following-sibling::table[@id='stakeholders']/tbody/tr[{rowIndex}]/td[2]"))
                .Text;
            string lastNameRow = Wait.UntilElementVisible(
                By.XPath($"//h5[contains(.,'Stakeholders')]/following-sibling::table[@id='stakeholders']/tbody/tr[{rowIndex}]/td[3]"))
                .Text;
            string emailRow = Wait.UntilElementVisible(
                By.XPath($"//h5[contains(.,'Stakeholders')]/following-sibling::table[@id='stakeholders']/tbody/tr[{rowIndex}]/td[4]"))
                .Text;
            string rolesRow = Wait.UntilElementVisible(
                By.XPath($"//h5[contains(.,'Stakeholders')]/following-sibling::table[@id='stakeholders']/tbody/tr[{rowIndex}]/td[5]"))
                .Text;

            var stakeHolderInfo = new StakeHolderInfo
            {
                FirstName = firstNameRow,
                LastName = lastNameRow,
                Email = emailRow,
                Role = rolesRow
            };

            return stakeHolderInfo;
        }

        public TeamInfo GetTeamInfo()
        {
            TeamInfo teamInfo = new TeamInfo
            {
                TeamName = Wait.UntilElementExists(TeamName).GetText(),
                WorkType = Wait.UntilElementExists(WorkType).GetText(),
                PreferredLanguage = Wait.UntilElementExists(PreferredLanguage).GetText(),
                Methodology = Wait.UntilElementExists(Methodology).GetText(),
                Department = Wait.UntilElementExists(Department).GetText(),
                DateEstablished = Wait.UntilElementExists(DateEstablished).GetText(),
                AgileAdoptionDate = Wait.UntilElementExists(AgileAdoptionDate).GetText(),
                Description = Wait.UntilElementExists(Description).GetText(),
                TeamBio = Wait.UntilElementExists(Biography).GetText(),
                ImagePath = Wait.UntilElementExists(TeamImage).GetElementAttribute("src")
            };
            return teamInfo;
        }

        public void ClickOnGoToTeamDashboard()
        {
            Log.Step(nameof(FinishAndReviewPage), "On Review & Finish page, click Go to Dashboard button");
            Wait.UntilElementClickable(GoToTeamDashboardButton).Click();
            Wait.HardWait(2000); // Wait till dashboard loads successfully.
        }

        public IList<string> GetSubTeamsText()
        {
            return Driver.GetTextFromAllElements(SubTeamTeamNames);
        }

        internal string GetNoSubTeamText()
        {
            var locator = By.XPath("//h5[contains(.,'Sub-Teams')]/following-sibling::p");
            return Driver.IsElementPresent(locator) ? Wait.UntilElementVisible(locator).GetText() : string.Empty;
        }

        public MultiTeamInfo GetMultiTeamInfo()
        {
            Wait.UntilElementVisible(TeamName);
            var multiTeamInfo = new MultiTeamInfo()
            {
                TeamName = Wait.UntilElementExists(TeamName).GetText(),
                TeamType = Wait.UntilElementExists(MultiTeamType).GetText(),
                AssessmentType = "",
                Department = Wait.UntilElementExists(Department).GetText(),
                DateEstablished = Wait.UntilElementExists(DateEstablished).GetText(),
                AgileAdoptionDate = Wait.UntilElementExists(AgileAdoptionDate).GetText(),
                Description = Wait.UntilElementExists(Description).GetText(),
                TeamBio = Wait.UntilElementExists(Biography).GetText(),
                ImagePath = Wait.UntilElementExists(TeamImage).GetElementAttribute("src")
            };

            return multiTeamInfo;
        }

        public EnterpriseTeamInfo GetEnterpriseTeamInfo()
        {
            Wait.UntilElementVisible(TeamName);
            var enterpriseTeamInfo = new EnterpriseTeamInfo()
            {
                TeamName = Wait.UntilElementExists(TeamName).GetText(),
                TeamType = Wait.UntilElementExists(EnterpriseTeamType).GetText(),
                ExternalIdentifier = "",
                Department = Wait.UntilElementExists(Department).GetText(),
                Description = Wait.UntilElementExists(Description).GetText(),
                TeamBio = Wait.UntilElementExists(Biography).GetText(),
                ImagePath = Wait.UntilElementExists(TeamImage).GetElementAttribute("src")
            };
            //if the dates aren't blank, parse them
            var dateEstablishedText = Wait.UntilElementExists(DateEstablished).GetText();
            var agileAdoptionDateText = Wait.UntilElementExists(AgileAdoptionDate).GetText();
            enterpriseTeamInfo.DateEstablished = (dateEstablishedText != "") ? DateTime.Parse(dateEstablishedText) : new DateTime();
            enterpriseTeamInfo.AgileAdoptionDate = (agileAdoptionDateText != "") ? DateTime.Parse(agileAdoptionDateText) : new DateTime();

            if (BaseTest.User.IsCompanyAdmin())
                enterpriseTeamInfo.ExternalIdentifier = Wait.UntilElementExists(ExternalIdentifier).GetText();

            return enterpriseTeamInfo;
        }

        public void ClickOnGoToAssessmentDashboard()
        {
            Log.Step(GetType().Name, "On Review & Finish page, click Go to Assessment dashboard button");
            Wait.UntilElementClickable(GoToTeamAssessmentDashboardButton).Click();
            Wait.UntilJavaScriptReady();
        }

        public void ClickOnEditLink(string sectionName)
        {
            Log.Step(GetType().Name, $"On Review & Finish page, Click on the 'Edit' link for '{sectionName}'");
            Wait.UntilElementClickable(EditLink(sectionName)).Click();
        }
        public void ClickOnClickHereLink(string sectionName)
        {
            Log.Step(nameof(FinishAndReviewPage), $"Click on 'click here' link of {sectionName}.");
            Wait.UntilElementVisible(ClickHereLink(sectionName)).Click();
        }

        public bool IsClickHereLinkPresent(string sectionName)
        {
            return Driver.IsElementPresent(ClickHereLink(sectionName));
        }

        public void NavigateToPage(string teamId)
        {
            NavigateToUrl($"{BaseTest.ApplicationUrl}/teams/review/{teamId}");
        }
    }
}
