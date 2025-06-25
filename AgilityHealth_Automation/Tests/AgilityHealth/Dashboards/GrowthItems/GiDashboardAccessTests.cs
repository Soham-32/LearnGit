using System;
using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.IndividualAssessment;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Common;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.GrowthItems;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.PageObjects.AgilityHealth.GrowthPlan.Add;
using AgilityHealth_Automation.PageObjects.AgilityHealth.GrowthPlan.Radar;
using AgilityHealth_Automation.Tests.AgilityHealth.Assessments.IndividualAssessment;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos;
using AtCommon.Dtos.GrowthPlan.Custom;
using AtCommon.Dtos.IndividualAssessments;
using AtCommon.Dtos.Reports;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Dashboards.GrowthItems
{

    [TestClass]
    [TestCategory("GrowthItemsDashboard"), TestCategory("Dashboard")]
    public class GiDashboardAccessTests : IndividualAssessmentBaseTest
    {
        private static bool _classInitFailed;
        private static SetupTeardownApi _setup;
        private static TeamResponse _teamResponse;
        private static CreateIndividualAssessmentRequest _assessmentRequest;
        private static IndividualAssessmentResponse _assessment;

        private static User TeamAdminUser => TestEnvironment.UserConfig.GetUserByDescription("growth item user");
        private static readonly GrowthItem GrowthItemInfo = new GrowthItem
        {
            Rank = "1",
            Category = "Individual",
            Type = "Agile Enablement",
            Title = RandomDataUtil.GetGrowthPlanTitle(),
            Status = "Not Started",
            TargetDate = DateTime.Now,
            CompetencyTargets = new List<string> { Constants.CompetencyLabelForAgileCoachHealth[0] },
            Priority = "Low",
            Size = "2",
            Color = "#dfff82",
            Description = RandomDataUtil.GetGrowthPlanDescription()
        };
        private static readonly GrowthItem GrowthItemInfo1 = new GrowthItem
        {
            Rank = "1",
            Category = "Management",
            Type = "Agile Enablement",
            Title = RandomDataUtil.GetGrowthPlanTitle(),
            Status = "Not Started",
            TargetDate = DateTime.Now,
            CompetencyTargets = new List<string> { Constants.CompetencyLabelForAgileCoachHealth[0] },
            Priority = "Low",
            Size = "2",
            Color = "#dfff82",
            Description = RandomDataUtil.GetGrowthPlanDescription()
        };

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            try
            {
                _setup = new SetupTeardownApi(TestEnvironment);
                _teamResponse = _setup.GetTeamResponse(SharedConstants.GoiTeam);

                _assessmentRequest = IndividualAssessmentFactory.GetPublishedIndividualAssessment(Company.Id, User.CompanyName, _teamResponse.Uid);
                _assessmentRequest.Members = _teamResponse.Members.Where(a => a.FullName() == Constants.TeamMemberName1).Select(m => m.ToAddIndividualMemberRequest()).ToList();

                _assessment = _setup.CreateIndividualAssessment(_assessmentRequest, SharedConstants.IndividualAssessmentType)
                    .GetAwaiter().GetResult();
            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }

        [TestMethod]
        [TestCategory("KnownDefect")] // Bug : 45370
        [TestCategory("CompanyAdmin")]
        public void GrowthItemDashboard_Grid_IA_GrowthItems_Access()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var iAssessmentDashboardPage = new IndividualAssessmentDashboardPage(Driver, Log);
            var growthItemKanbanViewWidget = new GrowthItemKanbanViewWidget(Driver, Log);
            var addGrowthItemPopupPage = new AddGrowthItemPopupPage(Driver, Log);
            var growthItemsDashboardPage = new GrowthItemsDashboardPage(Driver, Log);
            var giDashboardGridWidgetPage = new GiDashboardGridWidgetPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);
            var dashboardPage = new TeamDashboardPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            Log.Info($"Go to radar of the {_assessment.AssessmentName} assessment");
            var teamId = dashboardPage.GetTeamIdFromLink(SharedConstants.GoiTeam).ToInt();
            teamAssessmentDashboard.NavigateToPage(teamId);
            teamAssessmentDashboard.SwitchToIndividualAssessmentView();
            iAssessmentDashboardPage.ClickOnRadar($"{_assessmentRequest.AssessmentName} - {_assessmentRequest.Members.First().FirstName} {_assessmentRequest.Members.First().LastName}");

            Log.Info($"Create Individual Growth Item (IGI) for the radar {_assessmentRequest.AssessmentName}");
            growthItemKanbanViewWidget.ClickKanbanAddNewGrowthItem();
            addGrowthItemPopupPage.EnterGrowthItemInfo(GrowthItemInfo);
            addGrowthItemPopupPage.ClickSaveButton();

            Log.Info($"Create Management Growth Item (MGI) for the radar {_assessmentRequest.AssessmentName}");
            growthItemKanbanViewWidget.ClickKanbanAddNewGrowthItem();
            addGrowthItemPopupPage.EnterGrowthItemInfo(GrowthItemInfo1);
            addGrowthItemPopupPage.ClickSaveButton();

            Log.Info($"Navigate to 'Growth Items' dashboard and verify that Growth Items {GrowthItemInfo.Title} - IGI is present and {GrowthItemInfo1.Title} - MGI is not present, When we filter category by 'Individual'");
            growthItemsDashboardPage.NavigateToPage(Company.Id);
            growthItemsDashboardPage.ChangeAssessmentTypeView(AssessmentWidget.Individual);
            giDashboardGridWidgetPage.ClearFilter();
            giDashboardGridWidgetPage.FilterByCategory("Individual");
            var igiListFromGiDashboard = giDashboardGridWidgetPage.GetColumnValues("Title");
            Assert.That.ListContains(igiListFromGiDashboard, GrowthItemInfo.Title, $"{GrowthItemInfo.Title} IGI is not present ");
            Assert.That.ListNotContains(igiListFromGiDashboard, GrowthItemInfo1.Title, $"{GrowthItemInfo1.Title} MGI is present");

            Log.Info($"Verify that Growth Items {GrowthItemInfo1.Title} - MGI is present and {GrowthItemInfo.Title} - IGI is not present when we filter category by 'Management'");
            giDashboardGridWidgetPage.FilterByCategory("Management");
            var mgiListFromGiDashboard = giDashboardGridWidgetPage.GetColumnValues("Title");
            Assert.That.ListNotContains(mgiListFromGiDashboard, GrowthItemInfo.Title, $"{GrowthItemInfo.Title} IGI is present");
            Assert.That.ListContains(mgiListFromGiDashboard, GrowthItemInfo1.Title, $"{GrowthItemInfo1.Title} MGI is not present");

            Log.Info($"Verify that Growth Items {GrowthItemInfo.Title} - IGI and {GrowthItemInfo1.Title} - MGI are present when we filter category by 'All'");
            giDashboardGridWidgetPage.FilterByCategory("All");
            var allGrowthItemListFromGiDashboard = giDashboardGridWidgetPage.GetColumnValues("Title");
            Assert.That.ListContains(allGrowthItemListFromGiDashboard, GrowthItemInfo.Title, $"{GrowthItemInfo.Title} IGI is not present");
            Assert.That.ListContains(allGrowthItemListFromGiDashboard, GrowthItemInfo1.Title, $"{GrowthItemInfo1.Title} MGI is not present");

            Log.Info($"Logout as a Company Admin - {User.FullName} and Login as a Team Admin User - {TeamAdminUser.FullName}");
            topNav.LogOut();
            login.LoginToApplication(TeamAdminUser.Username, TeamAdminUser.Password);

            Log.Info("Navigate To 'Growth Items' Dashboard and shift view toggle button to 'Individual' ");
            growthItemsDashboardPage.NavigateToPage(Company.Id);
            growthItemsDashboardPage.ChangeAssessmentTypeView(AssessmentWidget.Individual);

            Log.Info($"Verify that Growth Items {GrowthItemInfo.Title} and {GrowthItemInfo1.Title} are present when we filter category by 'All'");
            giDashboardGridWidgetPage.ClearFilter();
            var allGrowthItemListFromGiDashboardOfTa = giDashboardGridWidgetPage.GetColumnValues("Title");
            Assert.That.ListContains(allGrowthItemListFromGiDashboardOfTa, GrowthItemInfo.Title, $"{GrowthItemInfo.Title} IGI is not present");
            Assert.That.ListContains(allGrowthItemListFromGiDashboardOfTa, GrowthItemInfo1.Title, $"{GrowthItemInfo1.Title} MGI is not present");

            Log.Info($"Verify that Growth Items {GrowthItemInfo.Title} is present and {GrowthItemInfo1.Title} is not present when we filter categories by 'Individual'");
            giDashboardGridWidgetPage.FilterByCategory("Individual");
            var igiListFromGiDashboardOfTa = giDashboardGridWidgetPage.GetColumnValues("Title");
            Assert.That.ListContains(igiListFromGiDashboardOfTa, GrowthItemInfo.Title, $"{GrowthItemInfo.Title} IGI is not present");
            Assert.That.ListNotContains(igiListFromGiDashboardOfTa, GrowthItemInfo1.Title, $"{GrowthItemInfo1.Title} MGI is present");

            Log.Info($"Verify that Growth Items {GrowthItemInfo1.Title} is present and {GrowthItemInfo.Title} is not present when we filter categories by 'Management'");
            giDashboardGridWidgetPage.FilterByCategory("Management");
            var mgiListFromGiDashboardOfTa = giDashboardGridWidgetPage.GetColumnValues("Title");
            Assert.That.ListNotContains(mgiListFromGiDashboardOfTa, GrowthItemInfo.Title, $"{GrowthItemInfo.Title} IGI is present");
            Assert.That.ListContains(mgiListFromGiDashboardOfTa, GrowthItemInfo1.Title, $"{GrowthItemInfo1.Title} MGI is not present");
        }
    }
}