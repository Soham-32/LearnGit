using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AtCommon.Api;
using AtCommon.Dtos;
using AtCommon.Dtos.CampaignsV2;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiTests.v1.Endpoints.CampaignsV2
{
    [TestClass]
    [TestCategory("CampaignsV2")]
    public class PostTeamIdsCampaignTests : BaseV1Test
    {
        private static CreateCampaignRequest _request;
        private static CreateCampaignResponse _campaignResponse;
        private static MatchmakingResponse _autoMatchmakingResponse;
        private static MatchmakingRequest _autoMatchmakingRequest;
        private static SaveAsDraftRequest _saveAsDraftRequest;
        private static SaveAsDraftResponse _saveAsDraftResponse;
        private static LaunchCampaignRequest _launchCampaignRequest;
        private static GetTeamAssessmentsAllIdsRequest _getCampaignsTeamsIdsRequest;

        public static int NumberOfTeams = 4;
        public static int FacilitatorIndex = 1;
        public static int NumberOfFacilitators = 1;
        public static int TargetRatio = 2;

        private static readonly UserConfig SiteAdminUserConfig = new UserConfig("SA");
        private static User SiteAdminUser => SiteAdminUserConfig.GetUserByDescription("user 1");

        public static GetTeamAssessmentsAllIdsRequest GetTeamAssessmentsAllIdsRequest = new GetTeamAssessmentsAllIdsRequest
        {
            SearchText = "",
            FacilitatorId = "",
            AssessmentStatus = "",
        };


        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            //Create new Campaign and Assessment
            var setupApi = new SetupTeardownApi(TestEnvironment);

            _request = CampaignFactory.GetCampaign();
            _campaignResponse = setupApi.CreateCampaign(Company.Id, _request, SiteAdminUser);

            _autoMatchmakingRequest = CampaignFactory.AutoMatchmakingCampaignData(_campaignResponse.Id, NumberOfTeams, NumberOfFacilitators, TargetRatio, FacilitatorIndex);
            _autoMatchmakingResponse = setupApi.AutoMatchmakingResponse(Company.Id, _campaignResponse.Id,
                _autoMatchmakingRequest, SiteAdminUser);

            _saveAsDraftRequest = CampaignFactory.SaveAsDraftCampaignData(_autoMatchmakingResponse.CampaignId,
                _autoMatchmakingRequest.TeamIds, _autoMatchmakingRequest.FacilitatorIds, _autoMatchmakingResponse.TeamFacilitatorMap);
            _saveAsDraftResponse = setupApi.SetupCampaignResponse(Company.Id, _campaignResponse.Id, _saveAsDraftRequest, SiteAdminUser);

            _launchCampaignRequest = CampaignFactory.LaunchCampaign(_saveAsDraftResponse.CampaignId);
            setupApi.LaunchCampaign(Company.Id, _campaignResponse.Id, _launchCampaignRequest, SiteAdminUser);

            _getCampaignsTeamsIdsRequest = CampaignFactory.GetCampaignTeamsIds();

        }

        //200
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public async Task CampaignsV2_Post_CampaignTeamIds_With_AllValidDetails_Success()
        {
            //Given
            var client = await GetAuthenticatedClient();

            //When
            var response = await client.PostAsync<GetTeamAssessmentsAllIdsResponse>(RequestUris.CampaignsV2CampaignTeamIds(Company.Id, _campaignResponse.Id), _getCampaignsTeamsIdsRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status code doesn't match.");
            Assert.AreEqual(CampaignFactory.GetCompanyTeam().TeamIds.FirstOrDefault(), response.Dto.TeamIds.FirstOrDefault(),"Team Id does not match");
        }

        //200
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public async Task CampaignsV2_Post_CampaignTeamsIds_With_EmptyRequestData_Success()
        { 
            //Given
            var client = await GetAuthenticatedClient();

            //When
            var response = await client.PostAsync<GetTeamAssessmentsAllIdsResponse>(RequestUris.CampaignsV2CampaignTeamIds(Company.Id, _campaignResponse.Id), GetTeamAssessmentsAllIdsRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status code doesn't match.");
            Assert.AreEqual(_saveAsDraftResponse.SelectedTeams.Select(d => d).ToList().ToString(), response.Dto.TeamIds.Select(d => d).ToList().ToString(), "'TeamIds List' does not match");
        }

        //200
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public async Task CampaignsV2_Post_CampaignTeamIds_With_SearchText_Success()
        {
            //Given
            var client = await GetAuthenticatedClient();

            GetTeamAssessmentsAllIdsRequest.SearchText = SharedConstants.RadarTeam;

            //When
            var response = await client.PostAsync<GetTeamAssessmentsAllIdsResponse>(RequestUris.CampaignsV2CampaignTeamIds(Company.Id, _campaignResponse.Id), GetTeamAssessmentsAllIdsRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status code doesn't match.");
            Assert.AreEqual(CampaignFactory.GetCompanyTeam().TeamIds.FirstOrDefault(),response.Dto.TeamIds.FirstOrDefault(),"Team Id does not match");
        }

        //200
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public async Task CampaignsV2_Post_CampaignTeamIds_With_FacilitatorId_Success()
        {
            //Given
            var client = await GetAuthenticatedClient();

            GetTeamAssessmentsAllIdsRequest.FacilitatorId = CampaignFactory.FacilitatorId;

            //When
            var response = await client.PostAsync<GetTeamAssessmentsAllIdsResponse>(RequestUris.CampaignsV2CampaignTeamIds(Company.Id, _campaignResponse.Id), GetTeamAssessmentsAllIdsRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status code doesn't match.");
            Assert.AreEqual(_saveAsDraftResponse.SelectedTeams.Select(d => d).ToList().ToString(), response.Dto.TeamIds.Select(d => d).ToList().ToString(), "'TeamIds List' does not match");
        }

        //200
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public async Task CampaignsV2_Post_CampaignTeamIds_With_AssessmentStatus_Success()
        {
            //Given
            var client = await GetAuthenticatedClient();

            GetTeamAssessmentsAllIdsRequest.AssessmentStatus = CampaignFactory.AssessmentStatusList.First();

            //When
            var response = await client.PostAsync<GetTeamAssessmentsAllIdsResponse>(RequestUris.CampaignsV2CampaignTeamIds(Company.Id, _campaignResponse.Id), GetTeamAssessmentsAllIdsRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status code doesn't match.");
            Assert.AreEqual(_saveAsDraftResponse.SelectedTeams.Select(d => d).ToList().ToString(), response.Dto.TeamIds.Select(d => d).ToList().ToString(), "'TeamIds List' does not match");
        }

        //400
        [TestMethod]
        [TestCategory("SiteAdmin")]
        public async Task CampaignsV2_Post_CampaignTeamIds_FakeCompanyId_BadRequest()
        {
            //Given
            var client = await GetAuthenticatedClient();

            var errorResponseList = new List<string>
            {
                "CompanyId is not found"
            };

            //When
            var response = await client.PostAsync<List<string>>(RequestUris.CampaignsV2CampaignTeamIds(SharedConstants.FakeCompanyId, _campaignResponse.Id), GetTeamAssessmentsAllIdsRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status code doesn't match.");
            Assert.That.ListsAreEqual(errorResponseList, response.Dto.ToList(), "Error message is not matched");
        }

        //400
        [TestMethod]
        [TestCategory("SiteAdmin")]
        public async Task CampaignsV2_Post_CampaignTeamIds_InvalidCompanyId_BadRequest()
        {
            //Given
            var client = await GetAuthenticatedClient();

            var errorResponseList = new List<string>
            {
                "CompanyId cannot be null or empty or less then zero."
            };

            //When
            var response = await client.PostAsync<List<string>>(RequestUris.CampaignsV2CampaignTeamIds(-1, _campaignResponse.Id), GetTeamAssessmentsAllIdsRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status code doesn't match.");
            Assert.That.ListsAreEqual(errorResponseList, response.Dto.ToList(), "Error message is not matched");
        }

        //400
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public async Task CampaignsV2_Post_CampaignTeamIds_FakeCampaignId_BadRequest()
        {
            //Given
            var client = await GetAuthenticatedClient();

            var errorResponseList = new List<string>
            {
                "CampaignId is not found"
            };

            //When
            var response = await client.PostAsync<List<string>>(RequestUris.CampaignsV2CampaignTeamIds(Company.Id, SharedConstants.FakeCampaignId), GetTeamAssessmentsAllIdsRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status code doesn't match.");
            Assert.That.ListsAreEqual(errorResponseList, response.Dto.ToList(), "Error message is not matched");
        }

        //401
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public async Task CampaignsV2_Post_CampaignTeamIds_Unauthorized()
        {
            //Given
            var client =  GetUnauthenticatedClient();

            //When
            var response = await client.PostAsync<GetTeamAssessmentsAllIdsResponse>(RequestUris.CampaignsV2CampaignTeamIds(Company.Id, _campaignResponse.Id), GetTeamAssessmentsAllIdsRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status code doesn't match.");
        }

        //403 
        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("TeamAdmin"), TestCategory("Member")]
        public async Task CampaignsV2_Post_CampaignTeamIds_FakeCompanyId_Forbidden()
        {
            //Given
            var client = await GetAuthenticatedClient();

            //When
            var response = await client.PostAsync<List<string>>(RequestUris.CampaignsV2CampaignTeamIds(SharedConstants.FakeCompanyId, _campaignResponse.Id), GetTeamAssessmentsAllIdsRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status code doesn't match.");
        }

        //403 
        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("TeamAdmin"), TestCategory("Member")]
        public async Task CampaignsV2_Post_CampaignTeamIds_InvalidCompanyId_Forbidden()
        {
            //Given
            var client = await GetAuthenticatedClient();

            //When
            var response = await client.PostAsync<List<string>>(RequestUris.CampaignsV2CampaignTeamIds(-1, _campaignResponse.Id), GetTeamAssessmentsAllIdsRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status code doesn't match.");
        }

        //403
        [TestMethod]
        [TestCategory("OrgLeader"), TestCategory("TeamAdmin"), TestCategory("Member")]
        public async Task CampaignsV2_Post_CampaignTeamIds_FakeCampaignId_Forbidden()
        {
            //Given
            var client = await GetAuthenticatedClient();

            //When
            var response = await client.PostAsync<List<string>>(RequestUris.CampaignsV2CampaignTeamIds(Company.Id, SharedConstants.FakeCampaignId), GetTeamAssessmentsAllIdsRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status code doesn't match.");
        }

        //403
        [TestMethod]
        [TestCategory("OrgLeader"), TestCategory("TeamAdmin"), TestCategory("Member")]
        public async Task CampaignsV2_Post_CampaignTeamIds_WithAllValidDetails_Forbidden()
        {
            //Given
            var client = await GetAuthenticatedClient();

            //When
            var response = await client.PostAsync<GetTeamAssessmentsAllIdsResponse>(RequestUris.CampaignsV2CampaignTeamIds(Company.Id, _campaignResponse.Id), GetTeamAssessmentsAllIdsRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status code doesn't match.");
        }

    }
}
