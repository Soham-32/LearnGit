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
    public class CreateGrowthPlanItemsTests : BaseV1Test
    {
        private static int _teamId;
        private static int _nonAssignTeamId;
        private static TeamResponse _teamResponse;
        private static AddTeamWithMemberRequest _team;
        private static CompanyHierarchyResponse _allTeamsList;
        private static RadarQuestionDetailsResponse _questionDetailsResponse;
        private static List<int> _competenciesId;
        private static readonly UserConfig SiteAdminUserConfig = new UserConfig("SA");
        private static User SiteAdminUser => SiteAdminUserConfig.GetUserByDescription("user 1");
        private static readonly UserConfig CompanyAdminUserConfig = new UserConfig("CA");
        private static User CompanyAdminUser => CompanyAdminUserConfig.GetUserByDescription("user 1");

        // Non Assign CompanyId and TeamId
        private const int NonAssignCompanyId = 71;
        private const int NonAssignCompanyTeamId = 4773;

        [ClassInitialize]
        public static void GetTeamDetails(TestContext testContext)
        {
            // Create a team
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
        [TestCategory("KnownDefect")]   //Bug: 41997
        public async Task GrowthPlan_Put_Item_With_AssignedTeam_Success()
        {
            // given
            var client = await GetAuthenticatedClient();

            var competenciesNames = _questionDetailsResponse.Dimensions.Select(s => s.Subdimensions).Last()
                .Select(c => c.Competencies).Last().Select(i => i.Name).OrderBy(n => n).ToList();

            var growthItemRequestDetails = GrowthPlanFactory.GrowthItemCreateRequest(Company.Id, _teamId, _competenciesId);

            // when
            var growthItemResponse = await client.PutAsync<GrowthPlanItemIdResponse>(RequestUris.GrowthPlanSave(), growthItemRequestDetails);

            // then
            Assert.AreEqual(HttpStatusCode.Created, growthItemResponse.StatusCode, "Status code doesn't match.");
           
            var getGrowthItemLists = await client.GetAsync<IList<GrowthPlanItemResponse>>(RequestUris.GrowthPlanItems(Company.Id).AddQueryParameter("teamId", _teamId));
            getGrowthItemLists.EnsureSuccess();
            Assert.IsTrue(getGrowthItemLists.Dto.Any(x => x.Id.Equals(growthItemResponse.Dto.GrowthPlanItemId)), "Growth Item Id is not available");

            var actualGrowthItemDetails = getGrowthItemLists.Dto.First(x => x.Id.Equals(growthItemResponse.Dto.GrowthPlanItemId));
            Assert.IsTrue(getGrowthItemLists.Dto.Any(x => x.Title.Equals(growthItemRequestDetails.Title)), "Growth Item is not available");
            Assert.AreEqual(growthItemRequestDetails.CompanyId, actualGrowthItemDetails.CompanyId, "Company Id is not matched");
            Assert.AreEqual(growthItemRequestDetails.TeamId, actualGrowthItemDetails.TeamId, "Team Id is not matched");
            Assert.AreEqual(growthItemRequestDetails.Category, actualGrowthItemDetails.Category, "Category is not matched");
            Assert.AreEqual(growthItemRequestDetails.Description, actualGrowthItemDetails.Description, "Description is not matched");
            Assert.IsTrue(actualGrowthItemDetails.Id != 0, "Id is null or empty");
            Assert.AreEqual(growthItemRequestDetails.Owner, actualGrowthItemDetails.Owner, "Owner is not matched");
            Assert.AreEqual(growthItemRequestDetails.Status, actualGrowthItemDetails.Status, "Status is not matched");
            Assert.AreEqual(growthItemRequestDetails.SurveyId, actualGrowthItemDetails.SurveyId, "Survey Id is not matched");
            Assert.AreEqual(growthItemRequestDetails.Priority, actualGrowthItemDetails.Priority,"Priority is not matched");
            Assert.AreEqual(growthItemRequestDetails.TargetDate?.ToString("MM/dd/yyyy"), actualGrowthItemDetails.TargetDate?.ToString("MM/dd/yyyy"), "TargetDate is not matched");
            Assert.AreEqual(growthItemRequestDetails.Type, actualGrowthItemDetails.Type, "Type is not matched");
            Assert.AreEqual(competenciesNames.ListToString(), actualGrowthItemDetails.CompetencyName, "Competencies are not matched");
            var updatedBy = User.IsOrganizationalLeader() ? "OrgLeader" :
                User.IsBusinessLineAdmin() ? User.Type.ToString().Replace("Admin", "") : User.Type.ToString();
            Assert.AreEqual(updatedBy, actualGrowthItemDetails.UpdatedBy.RemoveWhitespace(), "Updated By field is not matched");
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task GrowthPlan_Put_Item_BadRequest()
        {
            // given
            var client = await GetAuthenticatedClient();

            var growthItemRequestDetails = GrowthPlanFactory.GrowthItemCreateRequest(0, _teamId, _competenciesId);

            // when
            var growthItemResponse = await client.PutAsync(RequestUris.GrowthPlanSave(), growthItemRequestDetails.ToStringContent());
            
            // then
            Assert.AreEqual(HttpStatusCode.BadRequest, growthItemResponse.StatusCode, "Status code doesn't match.");
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task GrowthPlan_Put_Item_Unauthorized()
        {
            // given
            var client = GetUnauthenticatedClient();

            var growthItemRequestDetails = GrowthPlanFactory.GrowthItemCreateRequest(Company.Id, _teamId, new List<int>(2));

            // when
            var growthItemResponse = await client.PutAsync(RequestUris.GrowthPlanSave(), growthItemRequestDetails.ToStringContent());

            // then
            Assert.AreEqual(HttpStatusCode.Unauthorized, growthItemResponse.StatusCode, "Status code doesn't match.");
        }

        [TestMethod]
        [TestCategory("Member")]
        public async Task GrowthPlan_Put_Item_Forbidden()
        {
            // given
            var client = await GetAuthenticatedClient();

            var growthItemRequestDetails = GrowthPlanFactory.GrowthItemCreateRequest(Company.Id, _teamId, _competenciesId);

            // when
            var growthItemResponse = await client.PutAsync(RequestUris.GrowthPlanSave(), growthItemRequestDetails.ToStringContent());

            // then
            Assert.AreEqual(HttpStatusCode.Forbidden, growthItemResponse.StatusCode, "Status code doesn't match.");
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task GrowthPlan_Put_Item_With_NotExist_Company_Forbidden()
        {
            // given
            var client = await GetAuthenticatedClient();

            var growthItemRequestDetails = GrowthPlanFactory.GrowthItemCreateRequest(SharedConstants.FakeCompanyId, SharedConstants.FakeCompanyId, _competenciesId);

            // when
            var growthItemResponse = await client.PutAsync(RequestUris.GrowthPlanSave(), growthItemRequestDetails.ToStringContent());

            // then
            Assert.AreEqual(HttpStatusCode.Forbidden, growthItemResponse.StatusCode, "Status code doesn't match.");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task GrowthPlan_Put_Item_With_NonAssigned_Company_Forbidden()
        {
            // given
            var client = await GetAuthenticatedClient();

            var growthItemRequestDetails = GrowthPlanFactory.GrowthItemCreateRequest(NonAssignCompanyId, NonAssignCompanyTeamId, _competenciesId);

            // when
            var growthItemResponse = await client.PutAsync(RequestUris.GrowthPlanSave(), growthItemRequestDetails.ToStringContent());

            // then
            Assert.AreEqual(HttpStatusCode.Forbidden, growthItemResponse.StatusCode, "Status code doesn't match.");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("BLAdmin"), TestCategory("Member")]
        public async Task GrowthPlan_Put_Item_With_NonAssigned_Team_Forbidden()
        {
            // given
            var client = await GetAuthenticatedClient();

            var growthItemRequestDetails = GrowthPlanFactory.GrowthItemCreateRequest(Company.Id, _nonAssignTeamId, _competenciesId);

            // when
            var growthItemResponse = await client.PutAsync(RequestUris.GrowthPlanSave(), growthItemRequestDetails.ToStringContent());

            // then
            Assert.AreEqual(HttpStatusCode.Forbidden, growthItemResponse.StatusCode, "Status code doesn't match.");
        }
        
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        public async Task GrowthPlan_Put_Item_InternalServerError()
        {
            // given
            var client = await GetAuthenticatedClient();

            var growthItemRequestDetails = GrowthPlanFactory.GrowthItemCreateRequest(Company.Id, 00, new List<int>{0000});

            // when
            var growthItemResponse = await client.PutAsync(RequestUris.GrowthPlanSave(), growthItemRequestDetails.ToStringContent());

            // then
            Assert.AreEqual(HttpStatusCode.InternalServerError, growthItemResponse.StatusCode, "Status code doesn't match.");
        }

    }
}