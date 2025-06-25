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
    public class PostAutoMatchmakingCampaignTests : BaseV1Test
    {
        private static CreateCampaignRequest _campaignRequest;
        private static CreateCampaignResponse _campaignResponse;
        public static MatchmakingRequest AutoMatchmakingRequest;
        public static int NumberOfTeams = 4;
        public static int NumberOfFacilitators = 2;
        public static int TargetRatio = 2;
        private static User SiteAdminUser => new UserConfig("SA").GetUserByDescription("user 1");

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            _campaignRequest = CampaignFactory.GetCampaign();
            _campaignResponse = new SetupTeardownApi(TestEnvironment).CreateCampaign(Company.Id, _campaignRequest, SiteAdminUser);
            AutoMatchmakingRequest = CampaignFactory.AutoMatchmakingCampaignData(_campaignResponse.Id, NumberOfTeams, NumberOfFacilitators, TargetRatio);
        }

        //200
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public async Task CampaignsV2_Post_AutoMatchmakingCampaign_Success()
        {
            //Given
            var client = await GetAuthenticatedClient();

            //When
            var autoMatchmakingResponse = await client.PostAsync<MatchmakingResponse>(RequestUris.CampaignsV2AutoMatchmaking(Company.Id, _campaignResponse.Id), AutoMatchmakingRequest);

            var expectedTeamIdList = AutoMatchmakingRequest.TeamIds.Select(x => x.ToString()).OrderBy(y => y).ToList();
            var actualTeamIdList = autoMatchmakingResponse.Dto.TeamFacilitatorMap.Keys.OrderBy(z => z).ToList();
            var expectedFacilitatorIdList = AutoMatchmakingRequest.FacilitatorIds.Select(x => x).OrderBy(y => y).ToList();
            var actualFacilitatorIdList = autoMatchmakingResponse.Dto.TeamFacilitatorMap.Values.OrderBy(z => z).Distinct().ToList();

            //Then
            Assert.AreEqual(HttpStatusCode.OK, autoMatchmakingResponse.StatusCode, "Status Code doesn't match.");
            Assert.AreEqual(_campaignResponse.Id, autoMatchmakingResponse.Dto.CampaignId, "Campaign Id does not match.");
            Assert.That.ListsAreEqual(expectedTeamIdList, actualTeamIdList, "Team Id list doesn't match");
            Assert.That.ListsAreEqual(expectedFacilitatorIdList, actualFacilitatorIdList, "Facilitator Id list doesn't match");
            foreach (var facilitatorCount in expectedFacilitatorIdList.Select(facilitator => autoMatchmakingResponse.Dto.TeamFacilitatorMap.Count(k => k.Value == facilitator)))
            {
                Assert.AreEqual(2, facilitatorCount, "Facilitator count doesn't match");
            }
        }

        // 400
        [TestMethod]
        [TestCategory("SiteAdmin")]
        public async Task CampaignsV2_Post_AutoMatchmakingCampaign_WithFakeCompanyId_BadRequest()
        {
            //Given
            var client = await GetAuthenticatedClient();
            var errorResponseList = new List<string>
            {
                "CompanyId is not found"
            };

            //When
            var autoMatchmakingResponse = await client.PostAsync<IList<string>>(RequestUris.CampaignsV2AutoMatchmaking(SharedConstants.FakeCompanyId, _campaignResponse.Id), AutoMatchmakingRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.BadRequest, autoMatchmakingResponse.StatusCode, "Status Code doesn't match.");
            Assert.That.ListsAreEqual(errorResponseList, autoMatchmakingResponse.Dto.ToList(), "Error message does not match");
        }

        // 400
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public async Task CampaignsV2_Post_AutoMatchmakingCampaign_TargetRatioAndCampaignIdNull_BadRequest()
        {
            //Given
            var client = await GetAuthenticatedClient();
            var autoMatchmakingRequest = CampaignFactory.AutoMatchmakingCampaignData(_campaignResponse.Id, NumberOfTeams, NumberOfFacilitators, TargetRatio);
            autoMatchmakingRequest.TargetRatio = null;
            autoMatchmakingRequest.CampaignId = null;

            var invalidMessage = new List<string>()
            {
                "The input was not valid."
            };

            //When
            var autoMatchmakingResponse = await client.PostAsync<ErrorResponse>(RequestUris.CampaignsV2AutoMatchmaking(Company.Id, _campaignResponse.Id), autoMatchmakingRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.BadRequest, autoMatchmakingResponse.StatusCode, "Status Code doesn't match.");
            Assert.That.ListsAreEqual(invalidMessage, autoMatchmakingResponse.Dto.CampaignId, "CampaignId invalid message doesn't match");
            Assert.That.ListsAreEqual(invalidMessage, autoMatchmakingResponse.Dto.TargetRatio, "CampaignId invalid message doesn't match");
        }


        // 400
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        [TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("TeamAdmin"), TestCategory("Member")]
        public async Task CampaignsV2_Post_AutoMatchmakingCampaign_TeamIdAndFacilitatorIdEmpty_BadRequest()
        {
            //Given
            var client = await GetAuthenticatedClient();
            var autoMatchmakingRequest = CampaignFactory.AutoMatchmakingCampaignData(_campaignResponse.Id, NumberOfTeams, NumberOfFacilitators, TargetRatio);
            autoMatchmakingRequest.TeamIds = new List<int>();
            autoMatchmakingRequest.FacilitatorIds = new List<string>();

            var errorResponseList = new List<string>
            {
                "'Team Ids' must not be empty.",
                "'Facilitator Ids' must not be empty.",
            };

            //When
            var autoMatchmakingResponse = await client.PostAsync<IList<string>>(RequestUris.CampaignsV2AutoMatchmaking(Company.Id, _campaignResponse.Id), autoMatchmakingRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.BadRequest, autoMatchmakingResponse.StatusCode, "Status Code doesn't match.");
            Assert.That.ListsAreEqual(errorResponseList, autoMatchmakingResponse.Dto.ToList(), "Error message does not match");
        }

        //401
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        [TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("TeamAdmin"), TestCategory("Member")]
        public async Task CampaignsV2_Post_AutoMatchmakingCampaign_Unauthorized()
        {
            //Given
            var client = GetUnauthenticatedClient();

            //When
            var autoMatchmakingResponse = await client.PostAsync<MatchmakingResponse>(RequestUris.CampaignsV2AutoMatchmaking(Company.Id, _campaignResponse.Id), AutoMatchmakingRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.Unauthorized, autoMatchmakingResponse.StatusCode, "Status Code doesn't match.");
        }

        // 403   
        [TestMethod]
        [TestCategory("OrgLeader"), TestCategory("TeamAdmin"), TestCategory("Member")]
        public async Task CampaignsV2_Post_AutoMatchmakingCampaign_WithDifferentUsers_Forbidden()
        {
            //Given
            var client = await GetAuthenticatedClient();

            //When
            var autoMatchmakingResponse = await client.PostAsync<MatchmakingResponse>(RequestUris.CampaignsV2AutoMatchmaking(Company.Id, _campaignResponse.Id), AutoMatchmakingRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.Forbidden, autoMatchmakingResponse.StatusCode, "Status Code doesn't match.");
        }

        // 403
        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("TeamAdmin"), TestCategory("Member")]
        public async Task CampaignsV2_Post_AutoMatchmakingCampaign_WithFakeCompanyId_Forbidden()
        {
            //Given
            var client = await GetAuthenticatedClient();

            //When
            var autoMatchmakingResponse = await client.PostAsync<MatchmakingResponse>(RequestUris.CampaignsV2AutoMatchmaking(SharedConstants.FakeCompanyId, _campaignResponse.Id), AutoMatchmakingRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.Forbidden, autoMatchmakingResponse.StatusCode, "Status Code doesn't match.");
        }

        // 403   
        // Campaign Feature Off for Automation_2FA (DO NOT USE) - 1869
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        [TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("TeamAdmin"), TestCategory("Member")]
        public async Task CampaignsV2_Post_AutoMatchmakingCampaign_Forbidden()
        {
            //Given
            var client = await GetAuthenticatedClient();

            //When
            var autoMatchmakingResponse = await client.PostAsync<MatchmakingResponse>(RequestUris.CampaignsV2AutoMatchmaking(SharedConstants.Automation2FaDoNotUseCompanyId, _campaignResponse.Id), AutoMatchmakingRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.Forbidden, autoMatchmakingResponse.StatusCode, "Status Code doesn't match.");
        }
    }
}
