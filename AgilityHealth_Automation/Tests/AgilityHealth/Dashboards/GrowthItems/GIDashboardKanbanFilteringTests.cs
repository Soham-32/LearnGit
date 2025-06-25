using System;
using System.Collections.Generic;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.GrowthItems;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.PageObjects.AgilityHealth.GrowthPlan.Add;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.GrowthPlan.Custom;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Dashboards.GrowthItems
{
    [TestClass]
    [TestCategory("GrowthItemsDashboard"), TestCategory("Dashboard")]
    public class GiDashboardKanbanFilteringTests : BaseTest
    {
        private readonly List<GrowthItem> GiList = new List<GrowthItem>
            {
                new GrowthItem
                {
                    Category = "Team",
                   Type = "Agile Enablement",
                    Title = "GrowthItem1",
                    Status = "Not Started",
                    RadarType = SharedConstants.TeamAssessmentType,
                    CompetencyTargets = new List<string> {Constants.TeamHealth2CompentenciesLableForMember[0]},
                    Priority = "Low",
                    Size = "2",
                    Description = "test description"
                },
                new GrowthItem
                {
                    Category = "Organizational",
                   Type = "Agile Enablement",
                    Title = "GrowthItem2",
                    Status = "In Progress",
                    RadarType = SharedConstants.TeamAssessmentType,
                    CompetencyTargets = new List<string> {Constants.TeamHealth2CompentenciesLableForMember[0]},
                    Priority = "Low",
                    Size = "2",
                    Description = "test description"
                },
                new GrowthItem
                {
                    Category = "Team",
                   Type = "Agile Enablement",
                    Title = "GrowthItem3",
                    Status = "Done",
                    RadarType = SharedConstants.TeamAssessmentType,
                    CompetencyTargets = new List<string> {Constants.TeamHealth2CompentenciesLableForMember[0]},
                    Priority = "Low",
                    Size = "2",
                    Description = "test description"
                },
                new GrowthItem
                {
                    Category = "Team",
                   Type = "Agile Enablement",
                    Title = "GrowthItem4",
                    Status = "On Hold",
                    RadarType = SharedConstants.TeamAssessmentType,
                    CompetencyTargets = new List<string> {Constants.TeamHealth2CompentenciesLableForMember[0]},
                    Priority = "Low",
                    Size = "2",
                    Description = "test description"
                },
                new GrowthItem
                {
                    Category = "Team",
                   Type = "Agile Enablement",
                    Title = "GrowthItem5",
                    Status = "Cancelled",
                    RadarType = SharedConstants.TeamAssessmentType,
                    CompetencyTargets = new List<string> {Constants.TeamHealth2CompentenciesLableForMember[0]},
                    Priority = "Low",
                    Size = "2",
                    Description = "test description"
                }
            };
        private static bool _classInitFailed;
        private static int _teamId;
        [ClassInitialize]
        public static void ClassSetUp(TestContext testContext)
        {
            try
            {
                var teams = new SetupTeardownApi(TestEnvironment);
                _teamId = teams.GetCompanyHierarchy(Company.Id).GetTeamByName(Constants.TeamForGiTab).TeamId;

            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }
        [TestMethod]
        [TestCategory("KnownDefect")] //Bug Id: 51227
        [TestCategory("Critical")]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public void GrowthItemDashboard_Kanban_Filtering()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var addGrowthItemPopup = new AddGrowthItemPopupPage(Driver, Log);
            var growthItemsPage = new GrowthItemsPage(Driver, Log);
            var growthItemDashBoard = new GrowthItemsDashboardPage(Driver, Log);
            var giDashboardKanbanView = new GiDashboardKanbanWidgetPage(Driver, Log);

            Driver.NavigateToPage(ApplicationUrl);

            login.LoginToApplication(User.Username, User.Password);

            teamAssessmentDashboard.NavigateToPage(_teamId);

            teamAssessmentDashboard.SelectGrowthItemsTab();

            foreach (var growthItem in GiList)
            {
                growthItemsPage.ClickAddNewItemButton();
                addGrowthItemPopup.EnterGrowthItemInfo(growthItem);
                addGrowthItemPopup.ClickSaveButton();
            }
            Driver.NavigateToPage(ApplicationUrl);

            Log.Info("Go to GrowthItem dashboard and use the filters to check growth items");
            dashBoardPage.ClickGrowthItemDashBoard();

            growthItemDashBoard.ChangeViewWidget(GrowthItemWidget.Kanban);

            giDashboardKanbanView.FilterByCategory("All");

            giDashboardKanbanView.FilterBySurveyType("All");

            foreach (var gi in GiList)
            {
                Assert.IsTrue(giDashboardKanbanView.DoesKanbanGiExist(gi.Title, gi.Status),
                    $"Growth Item '{gi.Title}' with status '{gi.Status}' should show with filter 'All'.");
            }

            giDashboardKanbanView.FilterByCategory("Organizational");

            Assert.IsFalse(giDashboardKanbanView.DoesKanbanGiExist(GiList[0].Title, GiList[0].Status),
                $"Growth Item '{GiList[0].Title}' with status '{GiList[0].Status}' shouldn't show with filter 'Organizational'.");
            Assert.IsTrue(giDashboardKanbanView.DoesKanbanGiExist(GiList[1].Title, GiList[1].Status),
                $"Growth Item '{GiList[1].Title}' with status '{GiList[1].Status}' should show with filter 'Organizational'.");
            Assert.IsFalse(giDashboardKanbanView.DoesKanbanGiExist(GiList[2].Title, GiList[2].Status),
                $"Growth Item '{GiList[2].Title}' with status '{GiList[2].Status}' shouldn't show with filter 'Organizational'.");
            Assert.IsFalse(giDashboardKanbanView.DoesKanbanGiExist(GiList[3].Title, GiList[3].Status),
                $"Growth Item '{GiList[3].Title}' with status '{GiList[3].Status}' shouldn't show with filter 'Organizational'.");
            Assert.IsFalse(giDashboardKanbanView.DoesKanbanGiExist(GiList[4].Title, GiList[4].Status),
                $"Growth Item '{GiList[4].Title}' with status '{GiList[4].Status}' shouldn't show with filter 'Organizational'.");

            giDashboardKanbanView.FilterByCategory("All"); 

            giDashboardKanbanView.FilterBySurveyType("DevOps Health");

            foreach (var gi in GiList)
            {
                Assert.IsFalse(giDashboardKanbanView.DoesKanbanGiExist(gi.Title, gi.Status),
                    $"Growth Item '{gi.Title}' with status '{gi.Status}' shouldn't show with filter 'DevOps Health'.");
            }

            giDashboardKanbanView.FilterBySurveyType(SharedConstants.TeamAssessmentType);

            foreach (var gi in GiList)
            {
                Assert.IsTrue(giDashboardKanbanView.DoesKanbanGiExist(gi.Title, gi.Status),
                    $"Growth Item '{gi.Title}' with status '{gi.Status}' should show with filter 'Team Health'.");
            } 
            giDashboardKanbanView.FilterBySurveyType("All");
        }
    }
}
