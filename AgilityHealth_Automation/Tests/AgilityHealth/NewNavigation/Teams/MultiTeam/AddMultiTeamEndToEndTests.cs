using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.Enum.NewNavigation;
using AgilityHealth_Automation.ObjectFactories.NewNavigation.Teams;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.NewNavigation.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.NewNavigation.Teams.Create;
using AgilityHealth_Automation.PageObjects.AgilityHealth.NewNavigation.Teams.Edit;
using AgilityHealth_Automation.Utilities;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.NewNavigation.Teams.MultiTeam
{
    [TestClass]
    [TestCategory("Teams"), TestCategory("Multi-Team"), TestCategory("NewNavigation")]
    public class AddMultiTeamEndToEndTests : NewNavBaseTest
    {
        [TestMethod]
        [TestCategory("KnownDefect")]   //Bug:45959
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public void MultiTeam_Add_EndToEnd()
        {
            var login = new LoginPage(Driver, Log);
            var teamDashboardPage = new TeamDashboardPage(Driver, Log);
            var createMultiTeamStepperPage = new CreateTeamStepperPage(Driver, Log);
            var addMultiTeamSubTeamPage = new AddTeamSubTeamStepperPage(Driver, Log);
            var createLeadersStepperPage = new AddTeamMembersStepperPage(Driver, Log);
            var multiTeamProfileTabPage = new TeamProfileTabPage(Driver, Log);
            var subTeamTabPage = new SubTeamTabPage(Driver, Log);
            var reviewStepperPage = new ReviewStepperPage(Driver, Log);
            var leadersTabPage = new StakeholdersTabPage(Driver, Log);

            var multiTeamInfo = MultiTeamsFactory.GetValidMultiTeamInfo();
            var leadersInfo = TeamsFactory.GetTeamMemberInfo();

            Log.Info("Login to the application and switch to new navigation");
            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(User.Username, User.Password);
            SwitchToNewNav();

            Log.Info("Click on 'Add a New Team' button and select 'Multi-Team' type to create a multi team");
            teamDashboardPage.ClickOnAddNewTeamButtonAndSelectTeamType(TeamType.MultiTeam);

            Log.Info("Enter multi team info and click on 'Continue To Add Sub-Teams' button");
            createMultiTeamStepperPage.EnterTeamInfo(multiTeamInfo);
            createMultiTeamStepperPage.ClickOnContinueToSubTeamOrTeamMemberButton();

            Log.Info("Select sub team and verify the selected team, Then click on 'Continue To Add Leaders' button");
            addMultiTeamSubTeamPage.AssignSubTeam(SharedConstants.Team);
            Assert.That.ListContains(addMultiTeamSubTeamPage.GetAssignedSubTeamList(), SharedConstants.Team, $"List does not contain - {SharedConstants.Team}");
            addMultiTeamSubTeamPage.ClickOnContinueToAddLeadersButton();

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
            var expectedMultiTeamProfileText = reviewStepperPage.GetExpectedMultiTeamProfileText(multiTeamInfo).ReplaceStringData();
            var actualMultiTeamProfileText = reviewStepperPage.GetTeamProfileText().ReplaceStringData();
            Assert.AreEqual(expectedMultiTeamProfileText, actualMultiTeamProfileText, "Team profile text doesn't match");

            Log.Info("Validate the 'Team Tags' on 'Review' stepper");
            var actualMultiTeamTagsText = reviewStepperPage.GetTeamTagsText();
            CollectionAssert.AreEquivalent(multiTeamInfo.Tags, actualMultiTeamTagsText, "Tags are not matching");

            Log.Info("Validate the 'Sub Teams' on 'Review' stepper");
            var expectedTeamTagsList = new List<string> { "Software Delivery", "Kanban" };
            Assert.IsTrue(reviewStepperPage.GetSubTeamsTextList().Contains(SharedConstants.Team), "Failure !! Sub-team: " + SharedConstants.Team + " does not display in Finish and Review page");
            CollectionAssert.AreEqual(expectedTeamTagsList, reviewStepperPage.GetSubTeamsTagList(), "Team Tags are not matching");

            Log.Info("Validate the 'Leaders' info on 'Review' stepper");
            var leaderInfo = reviewStepperPage.GetTeamMemberOrStakeholderOrLeadersInfoFromGrid(leadersInfo.Email);
            Assert.AreEqual(leadersInfo.FirstName, leaderInfo.FirstName, "First name doesn't match");
            Assert.AreEqual(leadersInfo.LastName, leaderInfo.LastName, "Last name doesn't match");
            Assert.AreEqual(leadersInfo.Email, leaderInfo.Email, "Email doesn't match");

            Log.Info("Click on 'Finish' button and validate the multi team info on 'Team Profile' tab");
            reviewStepperPage.ClickOnFinishButton();
            var actualMultiTeamInfo = multiTeamProfileTabPage.GetMultiTeamInfo();
            Assert.AreEqual(multiTeamInfo.TeamName, actualMultiTeamInfo.TeamName, "Team Name doesn't match");
            Assert.AreEqual(multiTeamInfo.WorkType, actualMultiTeamInfo.WorkType, "Work Type doesn't match");
            Assert.AreEqual(multiTeamInfo.DepartmentAndGroup, actualMultiTeamInfo.DepartmentAndGroup, "Department doesn't match");
            Assert.AreEqual(multiTeamInfo.ExternalIdentifier, actualMultiTeamInfo.ExternalIdentifier, "External Identifier doesn't match");
            Assert.AreEqual(multiTeamInfo.TeamBioOrBackground, actualMultiTeamInfo.TeamBioOrBackground, "Team BIO doesn't match");

            Log.Info("Click on 'Sub Teams' tab and validate the added sub teams info");
            subTeamTabPage.ClickOnSubTeamTab();
            Assert.AreEqual(SharedConstants.Team, teamDashboardPage.GetTeamGridCellValue(1, "Team Name"), "Team Name doesn't match");
            Assert.AreEqual(SharedConstants.NewTeamWorkType, teamDashboardPage.GetTeamGridCellValue(1, "Work Type"), "Team Name doesn't match");
            Assert.AreEqual("3", teamDashboardPage.GetTeamGridCellValue(1, "No of Team Members"), "Team Members count doesn't match");
            CollectionAssert.AreEquivalent(expectedTeamTagsList, teamDashboardPage.GetTeamTagsValue(), "Team Tags doesn't match");

            Log.Info("Click on 'Leaders' tab and validate the leader info");
            leadersTabPage.ClickOnLeadersTab();
            var editLeaderInfo = reviewStepperPage.GetTeamMemberOrStakeholderOrLeadersInfoFromGrid(leadersInfo.Email);
            Assert.AreEqual(editLeaderInfo.FirstName, leaderInfo.FirstName, "First name doesn't match");
            Assert.AreEqual(editLeaderInfo.LastName, leaderInfo.LastName, "Last name doesn't match");
            Assert.AreEqual(editLeaderInfo.Email, leaderInfo.Email, "Email doesn't match");

            Log.Info("Navigate to 'Team Dashboard' and search newly created 'Multi Team' then validate team info");
            teamDashboardPage.NavigateToPage(Company.Id);
            teamDashboardPage.SearchTeam(multiTeamInfo.TeamName);
            var expectedTeamTagsValues = multiTeamInfo.Tags.Select(item => item.Value).ToList();
            expectedTeamTagsValues.AddRange(new List<string> { multiTeamInfo.WorkType });
            Assert.AreEqual(multiTeamInfo.TeamName, teamDashboardPage.GetTeamGridCellValue(1, "Team Name"), "Team Name doesn't match");
            Assert.AreEqual(multiTeamInfo.WorkType, teamDashboardPage.GetTeamGridCellValue(1, "Work Type"), "Work Type doesn't match");
            Assert.AreEqual("1", teamDashboardPage.GetTeamGridCellValue(1, "No of Sub-Teams"), "Sub team count doesn't match");
            CollectionAssert.AreEquivalent(expectedTeamTagsValues, teamDashboardPage.GetTeamTagsValue(), "Team Tags doesn't match");
        }
    }
}
