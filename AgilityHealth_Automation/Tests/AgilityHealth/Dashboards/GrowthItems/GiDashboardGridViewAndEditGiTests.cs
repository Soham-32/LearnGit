using System;
using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.GrowthItems;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.PageObjects.AgilityHealth.GrowthPlan.Add;
using AgilityHealth_Automation.PageObjects.AgilityHealth.GrowthPlan.Radar;
using AgilityHealth_Automation.SetUpTearDown;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos.Assessments.Team.Custom;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.GrowthPlan.Custom;
using AtCommon.Dtos.Radars;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Dashboards.GrowthItems
{

    [TestClass]
    [TestCategory("GrowthItemsDashboard"), TestCategory("Dashboard")]
    public class GiDashboardGridViewAndEditGiTests : BaseTest

    {
        public static bool ClassInitFailed;
        private static TeamAssessmentInfo _teamAssessment;
        private static TeamHierarchyResponse _team;
        private static readonly GrowthItem GrowthItemInfo = GrowthPlanFactory.GetValidGrowthItem();
        private static readonly GrowthItem GrowthItemInfo2 = GrowthPlanFactory.GetValidGrowthItem();
        private static RadarQuestionDetailsResponse _radarResponse;


        [ClassInitialize]
        public static void ClassSetup(TestContext testContext)
        {
            try
            {
                var setup = new SetupTeardownApi(TestEnvironment);
                var setupUi = new SetUpMethods(testContext, TestEnvironment);

                // Get team
                _team = setup.GetCompanyHierarchy(Company.Id).GetTeamByName(SharedConstants.Team);

                // Create a team assessment and add two growth items to the team assessment
                _teamAssessment = new TeamAssessmentInfo
                {
                    AssessmentType = SharedConstants.TeamAssessmentType,
                    AssessmentName = $"Team_Assessment211{Guid.NewGuid()}",
                    TeamMembers = new List<string> { SharedConstants.TeamMember1.FullName(), SharedConstants.TeamMember2.FullName() }
                };
                GrowthItemInfo2.Category = GrowthPlanFactory.GetMultiTeamGrowthPlanCategories().Last();
                GrowthItemInfo2.CompetencyTargets = new List<string> { SharedConstants.DimensionValueDelivered, SharedConstants.SurveyCompetency };
                setupUi.AddTeamAssessmentAndGi(_team.TeamId, _teamAssessment, new List<GrowthItem> { GrowthItemInfo, GrowthItemInfo2 });

                //get radar response
                var surveyId = setup.GetRadar(Company.Id, SharedConstants.TeamAssessmentType).Id;
                _radarResponse = setup.GetRadarQuestions(Company.Id, surveyId);
            }

            catch (Exception)
            {
                ClassInitFailed = true;
                throw;
            }
        }

        [TestMethod]
        [TestCategory("Smoke")]
        [TestCategory("Critical")]
        [TestCategory("KnownDefect")] //Bug Id: 53219
        [TestCategory("CompanyAdmin")]
        public void GrowthItemDashboard_Grid_ViewAndEdit_GrowthItem()
        {
            VerifySetup(ClassInitFailed);

            var login = new LoginPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var giDashboardGridView = new GiDashboardGridWidgetPage(Driver, Log);
            var growthItemGridView = new GrowthItemGridViewWidget(Driver, Log);
            var addGrowthItemPopup = new AddGrowthItemPopupPage(Driver, Log);

            var expectedDimensionListForFirstGi = _radarResponse.Dimensions.Where(dimensions => dimensions.Subdimensions.Any(subDimensions => subDimensions.Competencies.Any(competencies => competencies.Name.Equals(GrowthItemInfo.CompetencyTargets.First())))).Select(dimension => dimension.Name).ToList();
            var expectedSubDimensionListForFirstGi = _radarResponse.Dimensions.SelectMany(dimensions => dimensions.Subdimensions.Where(subDimensions => subDimensions.Competencies.Any(competencies => competencies.Name.Equals(GrowthItemInfo.CompetencyTargets.First())))).Select(subDimension => subDimension.Name).ToList();

            var expectedDimensionListForSecondGi = _radarResponse.Dimensions.Where(dimensions => dimensions.Subdimensions.Any(subDimensions => subDimensions.Competencies.Any(competencies => competencies.Name.Equals(GrowthItemInfo2.CompetencyTargets.First())))).Select(dimension => dimension.Name).ToList();
            var expectedSubDimensionListForSecondGi = _radarResponse.Dimensions.SelectMany(dimensions => dimensions.Subdimensions.Where(subDimensions => subDimensions.Competencies.Any(competencies => competencies.Name.Equals(GrowthItemInfo2.CompetencyTargets.First())))).Select(subDimension => subDimension.Name).ToList();

            Log.Info($"Log in as {User.FullName} and click on the 'Growth Item' dashboard");
            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(User.Username, User.Password);
            dashBoardPage.ClickGrowthItemDashBoard();
            growthItemGridView.SwitchToGridView();

            Log.Info("Added all columns and verify that all GI details should be matched");
            var expectedColumns = GrowthPlanFactory.GetGrowthPlanColumnNameList();
            expectedColumns.AddRange(new List<string>() { "Dimension", "Sub-Dimension", "External ID", "Target Date", "Competency Target" });
            giDashboardGridView.ClearFilter();
            giDashboardGridView.AddSelectedColumns(expectedColumns);

            Log.Info($"Get {GrowthItemInfo.Title} growth Item info and verify every details");
            var actualFirstGrowthItem = growthItemGridView.GetGrowthItemFromGrid(GrowthItemInfo.Title, true);
            Assert.IsTrue(growthItemGridView.IsGiPresent(GrowthItemInfo.Title), $"{GrowthItemInfo.Title} Gi is not present");
            Assert.AreEqual(GrowthItemInfo.Type, actualFirstGrowthItem.Type, "Type doesn't match");
            Assert.AreEqual(GrowthItemInfo.Title, actualFirstGrowthItem.Title, "Title doesn't match");
            Assert.AreEqual(GrowthItemInfo.Category, actualFirstGrowthItem.Category, "Category doesn't match");
            Assert.AreEqual(GrowthItemInfo.Status, actualFirstGrowthItem.Status, "Status doesn't match");
            Assert.AreEqual(GrowthItemInfo.Priority, actualFirstGrowthItem.Priority, "Priority doesn't match");
            Assert.AreEqual(GrowthItemInfo.TargetDate?.Date, actualFirstGrowthItem.TargetDate?.Date, "TargetDate doesn't match");
            Assert.AreEqual(GrowthItemInfo.Size, actualFirstGrowthItem.Size, "Size doesn't match");
            Assert.AreEqual(GrowthItemInfo.Description, actualFirstGrowthItem.Description, "Description doesn't match");
            Assert.AreEqual(expectedDimensionListForFirstGi.First(), actualFirstGrowthItem.Dimension, "Dimension doesn't match");
            Assert.AreEqual(expectedSubDimensionListForFirstGi.First(), actualFirstGrowthItem.SubDimension, "SubDimension doesn't match");
            Assert.AreEqual(GrowthItemInfo.Color, actualFirstGrowthItem.Color, "Color doesn't match");

            Log.Info($"Get {GrowthItemInfo2.Title} growth Item info and verify every details");
            var actualSecondGrowthItem = growthItemGridView.GetGrowthItemFromGrid(GrowthItemInfo2.Title, true);
            Assert.IsTrue(growthItemGridView.IsGiPresent(GrowthItemInfo2.Title), $"{GrowthItemInfo.Title} Gi is not present");
            Assert.AreEqual(GrowthItemInfo2.Type, actualSecondGrowthItem.Type, "Type doesn't match");
            Assert.AreEqual(GrowthItemInfo2.Title, actualSecondGrowthItem.Title, "Title doesn't match");
            Assert.AreEqual(GrowthItemInfo2.Category, actualSecondGrowthItem.Category, "Category doesn't match");
            Assert.AreEqual(GrowthItemInfo2.Status, actualSecondGrowthItem.Status, "Status doesn't match");
            Assert.AreEqual(GrowthItemInfo2.Priority, actualSecondGrowthItem.Priority, "Priority doesn't match");
            Assert.AreEqual(GrowthItemInfo2.TargetDate?.Date, actualSecondGrowthItem.TargetDate?.Date, "TargetDate doesn't match");
            Assert.AreEqual(GrowthItemInfo2.Size, actualSecondGrowthItem.Size, "Size doesn't match");
            Assert.AreEqual(GrowthItemInfo2.Description, actualSecondGrowthItem.Description, "Description doesn't match");
            Assert.AreNotEqual(expectedDimensionListForSecondGi.First(), actualSecondGrowthItem.Dimension, "Dimension is matched");
            Assert.AreNotEqual(expectedSubDimensionListForSecondGi.First(), actualSecondGrowthItem.SubDimension, "SubDimension is matched");
            Assert.AreEqual(GrowthItemInfo2.Color, actualSecondGrowthItem.Color, "Color doesn't match");

            Log.Info($"Update {GrowthItemInfo.Title} on growth item dashboard and verify every details");
            var updateGrowthItemInfo = GrowthPlanFactory.GetValidUpdatedGrowthItem();
            updateGrowthItemInfo.Category = GrowthPlanFactory.GetMultiTeamGrowthPlanCategories().First();
            updateGrowthItemInfo.CompetencyTargets = new List<string> { "Technical  Expertise" };
            var updatedDimensionListForFirstGi = _radarResponse.Dimensions.Where(dimensions => dimensions.Subdimensions.Any(subDimensions => subDimensions.Competencies.Any(competencies => competencies.Name.Equals(updateGrowthItemInfo.CompetencyTargets.First())))).Select(dimension => dimension.Name).ToList();
            var updatedSubDimensionListForFirstGi = _radarResponse.Dimensions.SelectMany(dimensions => dimensions.Subdimensions.Where(subDimensions => subDimensions.Competencies.Any(competencies => competencies.Name.Equals(updateGrowthItemInfo.CompetencyTargets.First())))).Select(subDimension => subDimension.Name).ToList();

            giDashboardGridView.ClickGrowthItemDashboardEditButton(GrowthItemInfo.Title);
            addGrowthItemPopup.EnterGrowthItemInfo(updateGrowthItemInfo);
            addGrowthItemPopup.ClickSaveButton();
            Assert.IsTrue(growthItemGridView.IsGiPresent(updateGrowthItemInfo.Title), $"{updateGrowthItemInfo.Title} is not present on 'Growth Item Dashboard' page");
            Assert.IsFalse(growthItemGridView.IsGiPresent(GrowthItemInfo.Title), $"{GrowthItemInfo.Title} is present on 'Growth Item Dashboard' page");

            var editedGrowthItem = growthItemGridView.GetGrowthItemFromGrid(updateGrowthItemInfo.Title, true);
            Assert.AreEqual(updateGrowthItemInfo.Category, editedGrowthItem.Category, "Category doesn't match");
            Assert.AreEqual(updateGrowthItemInfo.Type, editedGrowthItem.Type, "Type doesn't match");
            Assert.AreEqual(updateGrowthItemInfo.Title, editedGrowthItem.Title, "Title doesn't match");
            Assert.AreEqual(updateGrowthItemInfo.Status, editedGrowthItem.Status, "Status doesn't match");
            Assert.AreEqual(updateGrowthItemInfo.Priority, editedGrowthItem.Priority, "Priority doesn't match");
            Assert.AreEqual(updateGrowthItemInfo.TargetDate?.Date, editedGrowthItem.TargetDate?.Date, "TargetDate doesn't match");
            Assert.AreEqual(updateGrowthItemInfo.Size, editedGrowthItem.Size, "Size doesn't match");
            Assert.AreEqual(updateGrowthItemInfo.Description, editedGrowthItem.Description, "Description doesn't match");
            Assert.AreEqual(updatedDimensionListForFirstGi.First(), editedGrowthItem.Dimension, "Dimension doesn't match");
            Assert.AreEqual(updatedSubDimensionListForFirstGi.First(), editedGrowthItem.SubDimension, "SubDimension doesn't match");
            Assert.AreEqual(updateGrowthItemInfo.Color, editedGrowthItem.Color, "Color doesn't match");
        }
    }
}