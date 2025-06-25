using System;
using System.Linq;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.GrowthPlan.GrowthPlanV2.Add;
using AgilityHealth_Automation.PageObjects.AgilityHealth.GrowthPlan.GrowthPlanV2.Dashboard;
using AtCommon.Api;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.GrowthPlan;
using AtCommon.Dtos.Radars;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.GrowthPlan.GrowthPlanV2.Edit
{
    [TestClass]
    [TestCategory("GrowthPlanV2"), TestCategory("GrowthPlan")]
    public class GrowthPlanEditItemTest2 : BaseTest
    {
        private static bool _classInitFailed;
        private static int _teamId;
        private static TeamResponse _teamResponse;
        private static AddTeamWithMemberRequest _team;
        private static GrowthPlanItemRequest _growthPlanItemRequest;
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
                setup.CreateGrowthItem(_growthPlanItemRequest);
            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void GPV2_Item_Edit_Verify_SaveButton_Disabled_When_MandatoryField_NotFilled()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var growthPlanDashboard = new GrowthPlanDashboardPage(Driver, Log);
            var growthPlanAddItemPage = new GrowthPlanAddItemPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            growthPlanDashboard.NavigateToPage(Company.Id, _teamId);
            growthPlanDashboard.ClickOnColumnValueByGiTitleName(_growthPlanItemRequest.Title, "Title");
            
            growthPlanAddItemPage.EnterGrowthPlanTitle("");
            Assert.IsFalse(growthPlanAddItemPage.IsSaveButtonEnabled(), "'Save' button enabled");

            growthPlanAddItemPage.EnterGrowthPlanTitle(_growthPlanItemRequest.Title);
            Assert.IsTrue(growthPlanAddItemPage.IsSaveButtonEnabled(), "'Save' button disabled");
        }
    }
}
