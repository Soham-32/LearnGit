using System;
using System.Collections.Generic;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Edit;
using AgilityHealth_Automation.PageObjects.AgilityHealth.GrowthPlan.Add;
using AgilityHealth_Automation.PageObjects.AgilityHealth.GrowthPlan.Radar;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Common;
using AgilityHealth_Automation.SetUpTearDown;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos.Assessments.Team.Custom;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.GrowthPlan.Custom;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.GrowthPlan.TeamAssessment
{
    [TestClass]
    [TestCategory("GrowthPlan")]
    public class GrowthItemsTabPulledTests : BaseTest
    {
        private static bool _classInitFailed;
        private static readonly TeamAssessmentInfo TeamAssessment = new TeamAssessmentInfo
        {
            AssessmentType = SharedConstants.TeamAssessmentType,
            AssessmentName = RandomDataUtil.GetAssessmentName(),
        };

        private static readonly TeamAssessmentInfo TeamAssessment1 = new TeamAssessmentInfo
        {
            AssessmentType = SharedConstants.TeamAssessmentType,
            AssessmentName = RandomDataUtil.GetAssessmentName(),
        };

        private static TeamHierarchyResponse _team;
        private static TeamHierarchyResponse _multiTeam;

        [ClassInitialize]
        public static void ClassSetUp(TestContext testContext)
        {
            try
            {
                // get the teamIds
                var teams = new SetupTeardownApi(TestEnvironment).GetCompanyHierarchy(Company.Id);
                _team = teams.GetTeamByName(Constants.TeamForGiTab);
                _multiTeam = teams.GetTeamByName(Constants.MultiTeamForGiTab);

                // add assessments
                var setup = new SetUpMethods(testContext, TestEnvironment);
                TeamAssessment.TeamMembers =
                new List<string> { Constants.TeamMemberName1, Constants.TeamMemberName2 };
                TeamAssessment1.TeamMembers =
                new List<string> { Constants.TeamMemberName1, Constants.TeamMemberName2 };
                setup.AddTeamAssessment(_team.TeamId, TeamAssessment);
                setup.AddTeamAssessment(_team.TeamId, TeamAssessment1);
            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public void GrowthItemTab_TeamAssessmentLevel_DeleteAssessmentHasPullGI()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var addGrowthItemPopup = new AddGrowthItemPopupPage(Driver, Log);
            var growthItemGridView = new GrowthItemGridViewWidget(Driver, Log);
            var growthItemsPage = new GrowthItemsPage(Driver, Log);
            var taEditPage = new TeamAssessmentEditPage(Driver, Log);
            var mtetDashboardPage = new MtEtDashboardPage(Driver, Log);

            Driver.NavigateToPage(ApplicationUrl);

            login.LoginToApplication(User.Username, User.Password);

            teamAssessmentDashboard.NavigateToPage(_team.TeamId);

            teamAssessmentDashboard.ClickOnRadar(TeamAssessment.AssessmentName);

            growthItemGridView.SwitchToGridView();
            growthItemGridView.ClickAddNewGrowthItem();
            var growthItemInfo = new GrowthItem
            {
                Category = "Organizational",
                Type = "Agile Enablement",
                Title = "GI" + RandomDataUtil.GetGrowthPlanTitle(),
                Status = "Not Started",
                Owner = "",
                TargetDate = DateTime.Now,
                CompetencyTargets = new List<string> { Constants.TeamHealth2CompentenciesLableForMember[0] },
                Priority = "Low",
                Size = "2",
                Description = "test description"
            };
            addGrowthItemPopup.EnterGrowthItemInfo(growthItemInfo);
            addGrowthItemPopup.ClickSaveButton();
            var growthItemInfoId = growthItemGridView.GetGrowthItemId(growthItemInfo.Title);

            mtetDashboardPage.NavigateToPage(_multiTeam.TeamId);
            mtetDashboardPage.ClickOnRadar(SharedConstants.TeamAssessmentType);
            growthItemGridView.ClickPullItemFromSubTeam();

            growthItemGridView.SelectAllColumnInPushPullDialogBox();
            var actualSubTeams = growthItemGridView.GetGiInfoFromPushPullDialogBox(growthItemInfo.Title);
            Assert.AreEqual(growthItemInfoId, actualSubTeams.Id, "Growth Plan Id doesn't match");
            Assert.AreEqual(growthItemInfo.Category, actualSubTeams.Category, "Growth Plan Category doesn't match");
            Assert.AreEqual(growthItemInfo.Type, actualSubTeams.Type, "Growth Plan Type doesn't match");
            Assert.AreEqual(growthItemInfo.Title, actualSubTeams.Title, "Growth Plan Title doesn't match");
            Assert.AreEqual(growthItemInfo.Owner, actualSubTeams.Owner, "Growth Plan Owner doesn't match");
            Assert.AreEqual(growthItemInfo.Status, actualSubTeams.Status, "Growth Plan Status doesn't match");
            Assert.AreEqual(growthItemInfo.Priority, actualSubTeams.Priority, "Growth Plan Priority doesn't match");
            Assert.AreEqual(growthItemInfo.TargetDate?.Date, actualSubTeams.TargetDate?.Date,
                "Growth Plan TargetDate doesn't match");
            Assert.AreEqual(growthItemInfo.Size, actualSubTeams.Size, "Growth Plan Size doesn't match");
            Assert.AreEqual(growthItemInfo.Description, actualSubTeams.Description,
                "Growth Plan Description doesn't match");
            Assert.AreEqual(Constants.TeamForGiTab, actualSubTeams.AffectedTeams,
                "Growth Plan Description doesn't match");


            growthItemGridView.PullItemFromSubTeam(growthItemInfo.Title);
            teamAssessmentDashboard.NavigateToPage(_team.TeamId);

            teamAssessmentDashboard.SelectRadarLink(TeamAssessment.AssessmentName, "Edit");
            taEditPage.ClickOnDeleteAssessmentButtonAndChooseRemoveOption();

            teamAssessmentDashboard.SelectGrowthItemsTab();

            var actualGrowthItem = growthItemsPage.GetGrowthItemFromGrid(growthItemInfo.Title);
            Assert.AreEqual(growthItemInfo.Category, actualGrowthItem.Category, "Category doesn't match");
            Assert.AreEqual(growthItemInfo.Type, actualGrowthItem.Type, "Type doesn't match");
            Assert.AreEqual(growthItemInfo.Title, actualGrowthItem.Title, "Title doesn't match");
            Assert.AreEqual(growthItemInfo.Status, actualGrowthItem.Status, "Status doesn't match");
            Assert.AreEqual(growthItemInfo.Priority, actualGrowthItem.Priority, "Priority doesn't match");
            Assert.AreEqual(growthItemInfo.TargetDate?.Date, actualGrowthItem.TargetDate?.Date,
                    "TargetDate doesn't match");
            Assert.AreEqual(growthItemInfo.Size, actualGrowthItem.Size, "Size doesn't match");
            Assert.AreEqual(growthItemInfo.Description, actualGrowthItem.Description, "Description doesn't match");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public void GrowthItemTab_DeletePulledGIInMT()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var addGrowthItemPopup = new AddGrowthItemPopupPage(Driver, Log);
            var growthItemGridView = new GrowthItemGridViewWidget(Driver, Log);
            var growthItemsPage = new GrowthItemsPage(Driver, Log);
            var mtetDashboardPage = new MtEtDashboardPage(Driver, Log);

            Driver.NavigateToPage(ApplicationUrl);

            login.LoginToApplication(User.Username, User.Password);

            teamAssessmentDashboard.NavigateToPage(_team.TeamId);

            teamAssessmentDashboard.SelectGrowthItemsTab();

            growthItemsPage.ClickAddNewItemButton();

            var growthItemInfo = new GrowthItem
            {
                Category = "Organizational",
                Type = "Agile Enablement",
                Title = "GI" + RandomDataUtil.GetGrowthPlanTitle(),
                Status = "Not Started",
                TargetDate = DateTime.Now,
                Priority = "Low",
                Size = "2",
                RadarType = SharedConstants.TeamAssessmentType,
                CompetencyTargets = new List<string> { Constants.TeamHealth2CompentenciesLableForMember[0] },
                Description = "test description"
            };
            addGrowthItemPopup.EnterGrowthItemInfo(growthItemInfo);
            addGrowthItemPopup.ClickSaveButton();

            mtetDashboardPage.NavigateToPage(_multiTeam.TeamId);
            mtetDashboardPage.ClickOnRadar(SharedConstants.TeamAssessmentType);

            growthItemGridView.ClickPullItemFromSubTeam();

            growthItemGridView.PullItemFromSubTeam(growthItemInfo.Title);
            growthItemGridView.ClickClosePullDialog();

            growthItemGridView.UnpullGrowthItem(growthItemInfo.Title);

            teamAssessmentDashboard.NavigateToPage(_team.TeamId);

            teamAssessmentDashboard.SelectGrowthItemsTab();

            var actualGrowthItem = growthItemsPage.GetGrowthItemFromGrid(growthItemInfo.Title);
            Assert.AreEqual(growthItemInfo.Category, actualGrowthItem.Category, "Category doesn't match");
            Assert.AreEqual(growthItemInfo.Type, actualGrowthItem.Type, "Type doesn't match");
            Assert.AreEqual(growthItemInfo.Title, actualGrowthItem.Title, "Title doesn't match");
            Assert.AreEqual(growthItemInfo.Status, actualGrowthItem.Status, "Status doesn't match");
            Assert.AreEqual(growthItemInfo.Priority, actualGrowthItem.Priority, "Priority doesn't match");
            Assert.AreEqual(growthItemInfo.TargetDate?.Date, actualGrowthItem.TargetDate?.Date,
                    "TargetDate doesn't match");
            Assert.AreEqual(growthItemInfo.Size, actualGrowthItem.Size, "Size doesn't match");
            Assert.AreEqual(growthItemInfo.Description, actualGrowthItem.Description, "Description doesn't match");
            Assert.AreEqual(growthItemsPage.GetPulledValue(growthItemInfo.Title), "false", "GI pulled status should display as false");
        }
    }
}
