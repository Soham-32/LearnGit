using AtCommon.Dtos.CampaignsV2;
using AtCommon.Dtos;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AtCommon.Api;
using AtCommon.ObjectFactories;
using System.Net;

namespace ApiTests.v1.Endpoints.CampaignsV2
{
    [TestClass]
    [TestCategory("CampaignsV2")]
    public class PostLaunchCampaignTests : BaseV1Test
    {
        private static CreateCampaignRequest _request;
        private static CreateCampaignResponse _campaignResponse;
        private static MatchmakingResponse _autoMatchmakingResponse;
        private static MatchmakingRequest _autoMatchmakingRequest;
        private static SaveAsDraftRequest _saveAsDraftRequest;
        private static SaveAsDraftResponse _saveAsDraftResponse;
        private static LaunchCampaignRequest _launchCampaignRequest1;
        private static LaunchCampaignRequest _launchCampaignRequest2;
        public static int NumberOfTeams = 4;
        public static int NumberOfFacilitators = 2;
        public static int TargetRatio = 2;
        private static readonly UserConfig SiteAdminUserConfig = new UserConfig("SA");
        private static User SiteAdminUser => SiteAdminUserConfig.GetUserByDescription("user 1");

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            //Create new Campaign and Assessment
            var setupApi = new SetupTeardownApi(TestEnvironment);

            _request = CampaignFactory.GetCampaign();
            _campaignResponse = setupApi.CreateCampaign(Company.Id, _request, SiteAdminUser);

            _autoMatchmakingRequest = CampaignFactory.AutoMatchmakingCampaignData(_campaignResponse.Id, NumberOfTeams, NumberOfFacilitators, TargetRatio);
            _autoMatchmakingResponse = setupApi.AutoMatchmakingResponse(Company.Id, _campaignResponse.Id,
                _autoMatchmakingRequest, SiteAdminUser);

            _saveAsDraftRequest = CampaignFactory.SaveAsDraftCampaignData(_autoMatchmakingResponse.CampaignId,
                _autoMatchmakingRequest.TeamIds, _autoMatchmakingRequest.FacilitatorIds, _autoMatchmakingResponse.TeamFacilitatorMap);
            _saveAsDraftResponse = setupApi.SetupCampaignResponse(Company.Id, _campaignResponse.Id, _saveAsDraftRequest, SiteAdminUser);
            _launchCampaignRequest1 = CampaignFactory.LaunchCampaign(_saveAsDraftResponse.CampaignId);
            _launchCampaignRequest2 = CampaignFactory.LaunchCampaign(_saveAsDraftResponse.CampaignId);
        }

        //201
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public async Task CampaignsV2_Post_Launch_Success()
        {
            //Given
            var client = await GetAuthenticatedClient();
            const string expectedResponse = "Sucessfully Published";

            //When
            var response = await client.PostAsync<string>(RequestUris.CampaignsV2Launch(Company.Id, _saveAsDraftResponse.CampaignId), _launchCampaignRequest2);

            //Then
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode, "Status Code doesn't match.");
            Assert.AreEqual(expectedResponse, response.Dto, "Campaign was not launched");
        }

        //400
        [TestMethod]
        [TestCategory("SiteAdmin")]
        public async Task CampaignsV2_Post_Launch_WithFakeCompanyId_BadRequest()
        {
            //Given
            var client = await GetAuthenticatedClient();
            var errorResponseList = new List<string>
            {
                "CompanyId is not found"
            };

            //When
            var response = await client.PostAsync<IList<string>>(RequestUris.CampaignsV2Launch(SharedConstants.FakeCompanyId, _saveAsDraftResponse.CampaignId), _launchCampaignRequest1);

            //Then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status Code doesn't match.");
            Assert.That.ListsAreEqual(errorResponseList, response.Dto.ToList(), "Error list does not match");
        }

        //400
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin"), TestCategory("BLAdmin")]
        public async Task CampaignsV2_Post_Launch_WithFakeCampaignId_BadRequest()
        {
            //Given
            var client = await GetAuthenticatedClient();

            var errorResponseList = new List<string>
            {
                "CampaignId is not found"
            };
            var launchRequest = _launchCampaignRequest2;
            launchRequest.CampaignId = SharedConstants.FakeCampaignId;

            //When
            var response = await client.PostAsync<IList<string>>(RequestUris.CampaignsV2Launch(Company.Id, SharedConstants.FakeCampaignId), launchRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status Code doesn't match.");
            Assert.That.ListsAreEqual(errorResponseList, response.Dto.ToList(), "Error list does not match");
        }

        //400
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin"), TestCategory("BLAdmin")]
        public async Task CampaignsV2_Post_Launch_With_ValidAndFake_CampaignID_BadRequest()
        {
            //Given
            var client = await GetAuthenticatedClient();

            var launchRequest = _launchCampaignRequest2;
            launchRequest.CampaignId = SharedConstants.FakeCampaignId;

            //When
            var response = await client.PostAsync<string>(RequestUris.CampaignsV2Launch(Company.Id, _saveAsDraftResponse.CampaignId), launchRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status Code doesn't match.");
            Assert.AreEqual("Invalid Request", response.Dto, "Error list does not match");
        }

        //400
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin"), TestCategory("BLAdmin")]
        public async Task CampaignsV2_Post_Launch_With_FakeAndValid_CampaignID_BadRequest()
        {
            //Given
            var client = await GetAuthenticatedClient();

            //When
            var response = await client.PostAsync<string>(RequestUris.CampaignsV2Launch(Company.Id, SharedConstants.FakeCampaignId), _launchCampaignRequest1);

            //Then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status Code doesn't match.");
            Assert.AreEqual("Invalid Request", response.Dto, "Error list does not match");
        }

        //401
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        [TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("TeamAdmin"), TestCategory("Member")]
        public async Task CampaignsV2_Post_Launch_Unauthorized()
        {
            //Given
            var client = GetUnauthenticatedClient();

            //When
            var response = await client.PostAsync<string>(RequestUris.CampaignsV2Launch(Company.Id, _saveAsDraftResponse.CampaignId), _launchCampaignRequest2);

            //Then
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status Code doesn't match.");
        }

        //403
        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("TeamAdmin"), TestCategory("Member")]
        public async Task CampaignsV2_Post_Launch_With_FakeCompanyId_Forbidden()
        {
            //Given
            var client = await GetAuthenticatedClient();

            //When
            var response = await client.PostAsync<string>(RequestUris.CampaignsV2Launch(SharedConstants.FakeCompanyId, _saveAsDraftResponse.CampaignId), _launchCampaignRequest1);

            //Then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status Code doesn't match.");
        }

        //403
        [TestMethod]
        [TestCategory("OrgLeader"), TestCategory("TeamAdmin"), TestCategory("Member")]
        public async Task CampaignsV2_Get_CampaignSetUp_WithDifferentUsers_Forbidden()
        {
            //Given
            var client = await GetAuthenticatedClient();

            //When
            var response = await client.PostAsync<string>(RequestUris.CampaignsV2Launch(Company.Id, _saveAsDraftResponse.CampaignId), _launchCampaignRequest2);

            //Then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status Code doesn't match.");
        }
    }
}
