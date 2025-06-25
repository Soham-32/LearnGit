using System.Collections.Generic;
using AgilityHealth_Automation.Enum.NewNavigation;
using AgilityHealth_Automation.ObjectFactories.NewNavigation.Teams;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.NewNavigation.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.NewNavigation.Teams.Create;
using AgilityHealth_Automation.PageObjects.AgilityHealth.NewNavigation.Teams.Edit;
using AgilityHealth_Automation.Utilities;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.NewNavigation.Teams.EnterpriseTeam
{
    [TestClass]
    [TestCategory("Teams"), TestCategory("EnterpriseTeam"), TestCategory("NewNavigation")]
    public class AddEnterpriseTeamEndToEndTests : NewNavBaseTest
    {
        [TestMethod]
        [TestCategory("KnownDefect")] //Bug Id: 51046
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public void EnterpriseTeam_Add_EndToEnd()
        {
            var login = new LoginPage(Driver, Log);
            var teamDashboardPage = new TeamDashboardPage(Driver, Log);
            var createEnterpriseTeamStepperPage = new CreateTeamStepperPage(Driver, Log);
            var addEnterpriseTeamSubTeamPage = new AddTeamSubTeamStepperPage(Driver, Log);
            var createLeadersStepperPage = new AddTeamMembersStepperPage(Driver, Log);
            var enterpriseTeamProfileTabPage = new TeamProfileTabPage(Driver, Log);
            var subTeamTabPage = new SubTeamTabPage(Driver, Log);
            var reviewStepperPage = new ReviewStepperPage(Driver, Log);
            var leadersTabPage = new StakeholdersTabPage(Driver, Log);

            var enterpriseTeamInfo = EnterpriseTeamsFactory.GetValidEnterpriseTeamInfo();
            var leadersInfo = TeamsFactory.GetTeamMemberInfo();

            Log.Info("Login to the application and switch to new navigation then switch to grid view on team dashboard");
            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(User.Username, User.Password);
            SwitchToNewNav();
            teamDashboardPage.SwitchToGridView();

            Log.Info("Click on 'Add a New Team' button and select 'Enterprise' type to create a enterprise team");
            teamDashboardPage.ClickOnAddNewTeamButtonAndSelectTeamType(TeamType.EnterpriseTeam);

            Log.Info("Enter enterprise team info and click on 'Continue To Add Sub-Teams' button");
            createEnterpriseTeamStepperPage.EnterTeamInfo(enterpriseTeamInfo);
            createEnterpriseTeamStepperPage.ClickOnContinueToSubTeamOrTeamMemberButton();

            Log.Info("Select sub team and verify the selected team, Then click on 'Continue To Add Leaders' button");
            addEnterpriseTeamSubTeamPage.AssignSubTeam(SharedConstants.PortfolioTeam);
            Assert.That.ListContains(addEnterpriseTeamSubTeamPage.GetAssignedSubTeamList(), SharedConstants.PortfolioTeam, $"List does not contain - {SharedConstants.PortfolioTeam}");
            addEnterpriseTeamSubTeamPage.ClickOnContinueToAddLeadersButton();

            Log.Info("Click on 'Add New Leaders' option, enter leader info and click on 'Create & Close' button as well as validate the newly created leader info from grid");
            createLeadersStepperPage.OpenAddTeamMembersOrLeadersPopup();
            createLeadersStepperPage.PopupBase.EnterTeamMemberOrLeadersInfo(leadersInfo);
            createLeadersStepperPage.PopupBase.ClickOnCreateAndCloseButton();
            var actualLeaders = createLeadersStepperPage.GetTeamMemberOrStakeholderOrLeadersInfoFromGrid(leadersInfo.Email);
            Assert.AreEqual(leadersInfo.FirstName, actualLeaders.FirstName, "First name doesn't match");
            Assert.AreEqual(leadersInfo.LastName, actualLeaders.LastName, "Last name doesn't match");
            Assert.AreEqual(leadersInfo.Email, actualLeaders.Email, "Email doesn't match");

            Log.Info("Click on 'Continue To Review' button and validate team profile info on 'Review' stepper");
            createLeadersStepperPage.ClickOnContinueToReviewButton();
            var expectedEnterpriseTeamProfileText = reviewStepperPage.GetExpectedEnterpriseTeamProfileText(enterpriseTeamInfo).ReplaceStringData();
            var actualEnterpriseTeamProfileText = reviewStepperPage.GetTeamProfileText().ReplaceStringData();
            Assert.AreEqual(expectedEnterpriseTeamProfileText, actualEnterpriseTeamProfileText, "Team profile text doesn't match");

            Log.Info("Validate the 'Sub Teams' on 'Review' stepper");
            var expectedTeamTagsList = new List<string> { "Portfolio Team" };
            Assert.IsTrue(reviewStepperPage.GetSubTeamsTextList().Contains(SharedConstants.PortfolioTeam), "Failure !! Sub-team: " + SharedConstants.PortfolioTeam + " does not display in Finish and Review page");
            CollectionAssert.AreEqual(expectedTeamTagsList, reviewStepperPage.GetSubTeamsTagList(), "Team Tags are not matching");

            Log.Info("Validate the 'Leaders' info on 'Review' stepper");
            var leaderInfo = reviewStepperPage.GetTeamMemberOrStakeholderOrLeadersInfoFromGrid(leadersInfo.Email);
            Assert.AreEqual(leadersInfo.FirstName, leaderInfo.FirstName, "First name doesn't match");
            Assert.AreEqual(leadersInfo.LastName, leaderInfo.LastName, "Last name doesn't match");
            Assert.AreEqual(leadersInfo.Email, leaderInfo.Email, "Email doesn't match");

            Log.Info("Click on 'Finish' button and validate the enterprise team info on 'Team Profile' tab");
            reviewStepperPage.ClickOnFinishButton();
            var actualEnterpriseTeamInfo = enterpriseTeamProfileTabPage.GetEnterpriseTeamInfo();
            Assert.AreEqual(enterpriseTeamInfo.TeamName, actualEnterpriseTeamInfo.TeamName, "Team Name doesn't match");
            Assert.AreEqual(enterpriseTeamInfo.DepartmentAndGroup, actualEnterpriseTeamInfo.DepartmentAndGroup, "Department doesn't match");
            Assert.AreEqual(enterpriseTeamInfo.ExternalIdentifier, actualEnterpriseTeamInfo.ExternalIdentifier, "External Identifier doesn't match");
            Assert.AreEqual(enterpriseTeamInfo.TeamBioOrBackground, actualEnterpriseTeamInfo.TeamBioOrBackground, "Team BIO doesn't match");

            Log.Info("Click on 'Sub Teams' tab and validate the added sub teams info");
            subTeamTabPage.ClickOnSubTeamTab();
            Assert.AreEqual(SharedConstants.PortfolioTeam, teamDashboardPage.GetTeamGridCellValue(1, "Team Name"), "Team Name doesn't match");
            Assert.AreEqual("Portfolio Team", teamDashboardPage.GetTeamGridCellValue(1, "Work Type"), "Team Name doesn't match");
            Assert.AreEqual("2", teamDashboardPage.GetTeamGridCellValue(1, "No of Team Members"), "Team Members count doesn't match");
            CollectionAssert.AreEqual(expectedTeamTagsList, teamDashboardPage.GetTeamTagsValue(), "Team Tag doesn't match");

            Log.Info("Click on 'Leaders' tab and validate the leader info");
            leadersTabPage.ClickOnLeadersTab();
            var editLeaderInfo = reviewStepperPage.GetTeamMemberOrStakeholderOrLeadersInfoFromGrid(leadersInfo.Email);
            Assert.AreEqual(editLeaderInfo.FirstName, leaderInfo.FirstName, "First name doesn't match");
            Assert.AreEqual(editLeaderInfo.LastName, leaderInfo.LastName, "Last name doesn't match");
            Assert.AreEqual(editLeaderInfo.Email, leaderInfo.Email, "Email doesn't match");

            Log.Info("Navigate to 'Team Dashboard' and search newly created 'Enterprise Team' then validate team info");
            teamDashboardPage.NavigateToPage(Company.Id);
            teamDashboardPage.SearchTeam(enterpriseTeamInfo.TeamName);
            Assert.AreEqual(enterpriseTeamInfo.TeamName, teamDashboardPage.GetTeamGridCellValue(1, "Team Name"), "Team Name doesn't match");
            Assert.AreEqual(enterpriseTeamInfo.TeamName, teamDashboardPage.GetTeamGridCellValue(1, "Work Type"), "Work Type doesn't match");
            Assert.AreEqual("1", teamDashboardPage.GetTeamGridCellValue(1, "No of Sub-Teams"), "Sub team count doesn't match");
        }
    }
}
