using System;
using System.Collections.Generic;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.GrowthItems;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.PageObjects.AgilityHealth.GrowthPlan.Add;
using AgilityHealth_Automation.SetUpTearDown;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos.Assessments.Team.Custom;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.GrowthPlan.Custom;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Dashboards.GrowthItems
{
    [TestClass]
    [TestCategory("GrowthItemsDashboard"), TestCategory("Dashboard")]
    public class GiDashboardKanbanEditTests : BaseTest
    {
        private static TeamAssessmentInfo _teamAssessment;
        private static GrowthItem _growthItemInfo;

        [ClassInitialize]
        public static void ClassSetup(TestContext testContext)
        {
            var team = new SetupTeardownApi(TestEnvironment).GetCompanyHierarchy(Company.Id).GetTeamByName(SharedConstants.Team);

            _teamAssessment = new TeamAssessmentInfo
            {
                AssessmentType = SharedConstants.TeamAssessmentType,
                AssessmentName = RandomDataUtil.GetAssessmentName(), 
                TeamMembers = new List<string> { Constants.TeamMemberName1 },
                StakeHolders = new List<string> { Constants.StakeholderName1 }
            };

            _growthItemInfo = new GrowthItem
            {
                Category = "Team",
               Type = "Agile Enablement",
                Title = RandomDataUtil.GetGrowthPlanTitle(),
                Status = "Not Started",
                TargetDate = DateTime.Now,
                CompetencyTargets = new List<string> {Constants.TeamHealth2CompentenciesLableForMember[0]},
                Priority = "Low",
                Size = "2",
                Description = RandomDataUtil.GetGrowthPlanDescription()
            };

            var setup = new SetUpMethods(testContext, TestEnvironment);
            setup.AddTeamAssessmentAndGi(team.TeamId, _teamAssessment, new List<GrowthItem> { _growthItemInfo });
        }


        [TestMethod]
        [TestCategory("KnownDefect")] //Bug Id: 51227
        [ TestCategory("Critical")]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public void GrowthItemDashboard_Kanban_EditGrowthItem()
        {
            var login = new LoginPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var addGrowthItemPopup = new AddGrowthItemPopupPage(Driver, Log);
            var growthItemDashBoard = new GrowthItemsDashboardPage(Driver, Log);
            var giDashboardKanbanView = new GiDashboardKanbanWidgetPage(Driver, Log);

            Driver.NavigateToPage(ApplicationUrl);

            login.LoginToApplication(User.Username, User.Password);

            dashBoardPage.GridTeamView();

            dashBoardPage.ClickGrowthItemDashBoard();

            growthItemDashBoard.ChangeViewWidget(GrowthItemWidget.Kanban);
            giDashboardKanbanView.FilterBySurveyType("All");

            giDashboardKanbanView.ClickEditKanbanGrowthItem(_growthItemInfo.Title, _growthItemInfo.Status);
            
            _growthItemInfo = new GrowthItem
            {
                Category = "Individual",
                Type = "Product & Program",
                Title = RandomDataUtil.GetGrowthPlanTitle(),
                Status = "In Progress",
                TargetDate = DateTime.Now.AddDays(1),
                CompetencyTargets = new List<string> { Constants.TeamHealth2CompentenciesLableForMember[0] },
                Priority = "High",
                Size = "3",
                Color = "#ffa365",
                Description = RandomDataUtil.GetGrowthPlanDescription()
            };

            addGrowthItemPopup.EnterGrowthItemInfo(_growthItemInfo);
            addGrowthItemPopup.ClickSaveButton();

            Driver.RefreshPage();
            growthItemDashBoard.ChangeViewWidget(GrowthItemWidget.Kanban);
            Assert.IsTrue(giDashboardKanbanView.DoesKanbanGiExist(_growthItemInfo.Title, "In Progress"),
                "Growth Item should be edited successfully");
        }
    }
}