using System;
using System.Linq;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.GrowthPlan.GrowthPlanV2.Add;
using AgilityHealth_Automation.PageObjects.AgilityHealth.GrowthPlan.GrowthPlanV2.Dashboard;
using AtCommon.Api;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.GrowthPlan;
using AtCommon.Dtos.GrowthPlan.Custom;
using AtCommon.Dtos.Radars;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.GrowthPlan.GrowthPlanV2.Edit
{
    [TestClass]
    [TestCategory("GrowthPlanV2"), TestCategory("GrowthPlan")]
    public class GrowthPlanEditItemTest1 : BaseTest
    {
        private static bool _classInitFailed;
        private static readonly GrowthItem UpdateGrowthItemInfo = GrowthPlanFactory.GetValidUpdatedGrowthItem();
        private static int _teamId;
        private static TeamResponse _teamResponse;
        private static AddTeamWithMemberRequest _team;
        private static GrowthPlanItemRequest _growthPlanItemRequest;
        private static GrowthPlanItemIdResponse _growthPlanItemResponse;
        private static RadarQuestionDetailsResponse _radarResponse;

        [ClassInitialize]
        public static void ClassSetup(TestContext testContext)
        {
            try
            {
                var setup = new SetupTeardownApi(TestEnvironment);

                _team = TeamFactory.GetNormalTeam("Team");
                _team.Members.Add(new AddMemberRequest
                {
                    FirstName = SharedConstants.TeamMember1.FirstName,
                    LastName = SharedConstants.TeamMember1.LastName,
                    Email = SharedConstants.TeamMember1.Email
                });
                _team.Members.Add(new AddMemberRequest
                {
                    FirstName = SharedConstants.TeamMember2.FirstName,
                    LastName = SharedConstants.TeamMember2.LastName,
                    Email = SharedConstants.TeamMember2.Email
                });
                _teamResponse = setup.CreateTeam(_team).GetAwaiter().GetResult();
                _teamId = setup.GetCompanyHierarchy(Company.Id).GetTeamByName(_teamResponse.Name).TeamId;

                var surveyId = setup.GetRadar(Company.Id, SharedConstants.TeamAssessmentType).Id;
                _radarResponse = setup.GetRadarQuestions(Company.Id, surveyId);
                var competenciesIds = _radarResponse.Dimensions.Select(s => s.Subdimensions).First()
                    .Select(c => c.Competencies).First().Select(i => i.CompetencyId).ToList();

                _growthPlanItemRequest = GrowthPlanFactory.GrowthItemCreateRequest(Company.Id, _teamId, competenciesIds);
                _growthPlanItemResponse = setup.CreateGrowthItem(_growthPlanItemRequest);
            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }

        [TestMethod]
        [TestCategory("Smoke")]
        [TestCategory("CompanyAdmin")]
        public void GPV2_Item_Edit_Successfully()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var growthPlanDashboard = new GrowthPlanDashboardPage(Driver, Log);
            var growthPlanAddItemPage = new GrowthPlanAddItemPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            growthPlanDashboard.NavigateToPage(Company.Id, _teamId);
            growthPlanDashboard.ClickOnColumnValueByGiTitleName(_growthPlanItemRequest.Title, "Title");
            growthPlanAddItemPage.FillForm(UpdateGrowthItemInfo);
            growthPlanAddItemPage.ClickOnSaveButton();

            var allColumn = GrowthPlanDashboardPage.ColumnLocators.Keys.ToList();
            growthPlanDashboard.AddColumns(allColumn);

            var actualTeamTitleNames = growthPlanDashboard.GetAllColumnValues("Title");
            var titleName = UpdateGrowthItemInfo.Title;
            Assert.That.ListContains(actualTeamTitleNames, titleName, $"Growth Item {titleName} is not found");
            Assert.AreEqual(_growthPlanItemResponse.GrowthPlanItemId.ToString(), growthPlanDashboard.GetColumnValueByGiTitleName(titleName, "Id"), "'Id' doesn't matched");
            Assert.AreEqual(UpdateGrowthItemInfo.Title, growthPlanDashboard.GetColumnValueByGiTitleName(titleName, "Title"), "Updated 'Title' doesn't match");
            Assert.AreEqual(UpdateGrowthItemInfo.Owner, growthPlanDashboard.GetColumnValueByGiTitleName(titleName, "Owner(s)"), "Updated 'Owner(s)' doesn't match");
            Assert.AreEqual(UpdateGrowthItemInfo.TargetDate?.ToString("MM/dd/yyyy"), growthPlanDashboard.GetColumnValueByGiTitleName(titleName, "Target Date"), "Updated 'Target Date' doesn't match");
            Assert.AreEqual(UpdateGrowthItemInfo.Status, growthPlanDashboard.GetColumnValueByGiTitleName(titleName, "Status"), "Updated 'Status' doesn't match");
            Assert.AreEqual(UpdateGrowthItemInfo.Priority, growthPlanDashboard.GetColumnValueByGiTitleName(titleName, "Priority"), "Updated 'Priority' doesn't match");
            Assert.AreEqual(UpdateGrowthItemInfo.Category, growthPlanDashboard.GetColumnValueByGiTitleName(titleName, "Category"), "Updated 'Category' doesn't match");
            Assert.AreEqual(User.FullName, growthPlanDashboard.GetColumnValueByGiTitleName(titleName, "Updated By"), "Updated 'Updated By' doesn't match");
            Assert.AreEqual(_team.Name, growthPlanDashboard.GetColumnValueByGiTitleName(titleName, "Origination"), "Updated 'Origination' doesn't match");
            Assert.AreEqual(_team.Name, growthPlanDashboard.GetColumnValueByGiTitleName(titleName, "Team"), "Updated 'Team' doesn't match");
            Assert.IsTrue(growthPlanDashboard.GetColumnValueByGiTitleName(titleName, "Competency Target").Contains(UpdateGrowthItemInfo.CompetencyTargets.ListToString()), "Updated 'Competency Target' doesn't contains on GI dashboard");
            Assert.IsTrue(growthPlanDashboard.GetColumnValueByGiTitleName(titleName, "Type").Contains(UpdateGrowthItemInfo.Type), "Updated 'Type' doesn't contains on GI dashboard");
            Assert.IsTrue(UpdateGrowthItemInfo.RadarType.Contains(growthPlanDashboard.GetColumnValueByGiTitleName(titleName, "Radar Type").RemoveWhitespace()), "Updated 'Radar Type' doesn't contains on GI dashboard");
        }
    }
}
