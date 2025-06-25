using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AtCommon.Api;
using AtCommon.Dtos;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.GrowthPlan;
using AtCommon.Dtos.Radars;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiTests.v1.Endpoints.GrowthPlan
{
    [TestClass]
    [TestCategory("GrowthPlan")]
    public class DeleteGrowthPlanItemsTests : BaseV1Test
    {
        private static int _teamId;
        private static int _nonAssignTeamId;
        private static TeamResponse _teamResponse;
        private static AddTeamWithMemberRequest _team;
        private static CompanyHierarchyResponse _allTeamsList;
        private static RadarQuestionDetailsResponse _questionDetailsResponse;
        private static GrowthPlanItemRequest _growthPlanItemRequest;
        private static List<int> _competenciesId;
        private static readonly UserConfig SiteAdminUserConfig = new UserConfig("SA");
        private static User SiteAdminUser => SiteAdminUserConfig.GetUserByDescription("user 1");
        private static readonly UserConfig CompanyAdminUserConfig = new UserConfig("CA");
        private static User CompanyAdminUser => CompanyAdminUserConfig.GetUserByDescription("user 1");

        [ClassInitialize]
        public static void GetTeamDetails(TestContext testContext)
        {
            //Create a team
            _team = TeamFactory.GetNormalTeam("Team");
            _team.Tags.RemoveAll(a => a.Category.Equals("Business Lines"));
            _teamResponse = new SetupTeardownApi(TestEnvironment).CreateTeam(_team, CompanyAdminUser).GetAwaiter().GetResult();

            //Get Team Info
            _allTeamsList = new SetupTeardownApi(TestEnvironment).GetCompanyHierarchy(SiteAdminUserConfig.GetCompanyByEnvironment(TestEnvironment.EnvironmentName).Id, SiteAdminUser);
            _nonAssignTeamId = new SetupTeardownApi(TestEnvironment).GetCompanyHierarchy(_allTeamsList.CompanyId).GetTeamByName(_teamResponse.Name).TeamId;
            _teamId = new SetupTeardownApi(TestEnvironment).GetCompanyHierarchy(_allTeamsList.CompanyId).GetTeamByName(SharedConstants.Team).TeamId;

            //Getting Survey info
            var surveyId = new SetupTeardownApi(TestEnvironment)
                .GetRadar(_allTeamsList.CompanyId, SharedConstants.TeamAssessmentType).Id;
            _questionDetailsResponse =
                new SetupTeardownApi(TestEnvironment).GetRadarQuestions(_allTeamsList.CompanyId, surveyId);
            _competenciesId = _questionDetailsResponse.Dimensions.Select(s => s.Subdimensions).Last()
                .Select(c => c.Competencies).Last().Select(i => i.CompetencyId).ToList();
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        public async Task GrowthPlan_Delete_AssignedTeam_Item_Success()
        {
            // given
            var client = await GetAuthenticatedClient();

            var growthItemRequestDetails = GrowthPlanFactory.GrowthItemCreateRequest(Company.Id, _teamId, _competenciesId);

            var growthItemResponse = await client.PutAsync<GrowthPlanItemIdResponse>(RequestUris.GrowthPlanSave(), growthItemRequestDetails);
            growthItemResponse.EnsureSuccess();

            //when
            var growthItemDeletedResponse = await client.DeleteAsync(RequestUris.GrowthPlanDelete(growthItemResponse.Dto.GrowthPlanItemId));

            //then
            Assert.IsTrue(growthItemDeletedResponse.IsSuccessStatusCode, "Success code is not matched");

            var getGrowthItemListsResponse = await client.GetAsync<IList<GrowthPlanItemResponse>>(RequestUris.GrowthPlanItems(Company.Id));
            getGrowthItemListsResponse.EnsureSuccess();

            Assert.That.ListNotContains(getGrowthItemListsResponse.Dto.Select(a => a.Id.ToString()).ToList(), growthItemRequestDetails.Id, $"Growth Item {growthItemRequestDetails.Id} is still exist");
            Assert.That.ListNotContains(getGrowthItemListsResponse.Dto.Select(a => a.Title).ToList(), growthItemRequestDetails.Title, $"Growth Item {growthItemRequestDetails.Title} is still exist");
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin"),
         TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task GrowthPlan_Delete_Item_BadRequest()
        {

            // given
            var client = await GetAuthenticatedClient();

            //when
            var growthItemDeletedResponse = await client.DeleteAsync(RequestUris.GrowthPlanDelete(000000));

            //then
            Assert.AreEqual(HttpStatusCode.BadRequest, growthItemDeletedResponse.StatusCode,
                "Status code doesn't match.");

        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task GrowthPlan_Delete_Item_Unauthorized()
        {
            // given
            var client = GetUnauthenticatedClient();

            //when
            var growthItemDeletedResponse = await client.DeleteAsync(RequestUris.GrowthPlanDelete(SharedConstants.FakeCompanyId));

            //then
            Assert.AreEqual(HttpStatusCode.Unauthorized, growthItemDeletedResponse.StatusCode, "Status code doesn't match.");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task GrowthPlan_Delete_Item_NonAssigned_Company_Forbidden()
        {
            // given
            var client = await GetAuthenticatedClient();

            _growthPlanItemRequest = GrowthPlanFactory.GrowthItemCreateRequest(71, 4773, _competenciesId);
            var growthPlanResponse = new SetupTeardownApi(TestEnvironment).CreateGrowthItem(_growthPlanItemRequest, SiteAdminUser);

            //when
            var growthItemDeletedResponse = await client.DeleteAsync(RequestUris.GrowthPlanDelete(growthPlanResponse.GrowthPlanItemId));

            //then
            Assert.AreEqual(HttpStatusCode.Forbidden, growthItemDeletedResponse.StatusCode, "Status code doesn't match.");

        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task GrowthPlan_Delete_Item_NonAssignedTeam_Forbidden()
        {
            // given
            var client = await GetAuthenticatedClient();

            _growthPlanItemRequest =
                    GrowthPlanFactory.GrowthItemCreateRequest(_allTeamsList.CompanyId, _nonAssignTeamId, _competenciesId);

            var growthPlanResponse = new SetupTeardownApi(TestEnvironment).CreateGrowthItem(_growthPlanItemRequest, SiteAdminUser);

            //when
            var growthItemDeletedResponse = await client.DeleteAsync(RequestUris.GrowthPlanDelete(growthPlanResponse.GrowthPlanItemId));

            //then
            Assert.AreEqual(HttpStatusCode.Forbidden, growthItemDeletedResponse.StatusCode, "Status code doesn't match.");
        }

        [TestMethod]
        [TestCategory("Member")]
        public async Task GrowthPlan_Delete_Item_Forbidden()
        {
            // given
            var client = await GetAuthenticatedClient();

            _growthPlanItemRequest =
                GrowthPlanFactory.GrowthItemCreateRequest(_allTeamsList.CompanyId, _teamId, _competenciesId);

            var growthPlanResponse = new SetupTeardownApi(TestEnvironment).CreateGrowthItem(_growthPlanItemRequest, SiteAdminUser);

            //when
            var growthItemDeletedResponse = await client.DeleteAsync(RequestUris.GrowthPlanDelete(growthPlanResponse.GrowthPlanItemId));

            //then
            Assert.AreEqual(HttpStatusCode.Forbidden, growthItemDeletedResponse.StatusCode, "Status code doesn't match.");
        }
    }
}