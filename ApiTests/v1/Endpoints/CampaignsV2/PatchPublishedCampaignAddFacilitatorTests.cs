using System.Net;
using System.Linq;
using AtCommon.Api;
using AtCommon.Dtos;
using AtCommon.Utilities;
using System.Threading.Tasks;
using AtCommon.ObjectFactories;
using AtCommon.Dtos.CampaignsV2;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiTests.v1.Endpoints.CampaignsV2
{
    [TestClass]
    [TestCategory("CampaignsV2")]
    public class PatchPublishedCampaignAddFacilitatorTests : BaseV1Test
    {
        public static bool ClassInitFailed;

        private static CreateCampaignRequest _request;
        private static (CreateCampaignResponse createCampaignResponse, SaveAsDraftResponse saveAsDraftResponse) _campaignResponse;
        private static AddFacilitatorsToCampaignRequest _addFacilitatorsToCampaignRequest;

        private const int TargetRatio = 2;
        private const int NumberOfTeams = 4;
        private const int NumberOfFacilitators = 1;

        private static readonly UserConfig SiteAdminUserConfig = new UserConfig("SA");
        private static User SiteAdminUser => SiteAdminUserConfig.GetUserByDescription("user 1");

        public static AddFacilitatorsToCampaignRequest AddFacilitatorsToCampaignRequest = new AddFacilitatorsToCampaignRequest
        {
            CampaignId = 0,
            FacilitatorIds = new List<string>(),
        };

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            var setupApi = new SetupTeardownApi(TestEnvironment);
            _request = CampaignFactory.GetCampaign();
            _campaignResponse = setupApi.CreateAndSetupCampaign(Company.Id, _request, TargetRatio, NumberOfTeams, NumberOfFacilitators, SiteAdminUser);

            _addFacilitatorsToCampaignRequest = CampaignFactory.AddFacilitatorsToPublishedCampaign(_campaignResponse.createCampaignResponse.Id);
        }

        //200
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin"), TestCategory("BLAdmin")]
        public async Task CampaignsV2_Patch_Facilitators_With_AllRequestData_Success()
        {
            //Given
            var client = await GetAuthenticatedClient();

            //When
            var response = await client.PatchAsync<string>(RequestUris.CampaignsV2PatchFacilitators(Company.Id, _campaignResponse.createCampaignResponse.Id), _addFacilitatorsToCampaignRequest);
            response.EnsureSuccess();

            var getAddedFacilitator = await client.GetAsync<CampaignSetupResponse>(RequestUris.CampaignsV2Setup(Company.Id, _campaignResponse.createCampaignResponse.Id));

            //Then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status code doesn't match");
            Assert.AreEqual("Ok", response.Dto, "Message doesn't match");
            Assert.That.ListsAreEqual(CampaignFactory.FacilitatorIdsList, getAddedFacilitator.Dto.SelectedFacilitators, "Facilitators list doesn't match");
        }

        //200
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin"), TestCategory("BLAdmin")]
        public async Task CampaignsV2_Patch_Facilitator_With_FacilitatorId_Success()
        {
            //Given
            var client = await GetAuthenticatedClient();

            var facilitator = CampaignFactory.FacilitatorIdsList.GetRange(1, 1);
            AddFacilitatorsToCampaignRequest.FacilitatorIds = facilitator;

            //When
            var response = await client.PatchAsync<string>(RequestUris.CampaignsV2PatchFacilitators(Company.Id, _campaignResponse.createCampaignResponse.Id), AddFacilitatorsToCampaignRequest);

            var getAddedFacilitator = await client.GetAsync<CampaignSetupResponse>(RequestUris.CampaignsV2Setup(Company.Id, _campaignResponse.createCampaignResponse.Id));

            //Then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status code doesn't match");
            Assert.AreEqual("Ok", response.Dto, "Message doesn't match");
            Assert.That.ListsAreEqual(CampaignFactory.FacilitatorIdsList, getAddedFacilitator.Dto.SelectedFacilitators, "Facilitators list doesn't match");
        }

        //400
        [TestMethod]
        [TestCategory("SiteAdmin")]
        public async Task CampaignsV2_Patch_Facilitator_With_FakeCompanyId_BadRequest()
        {
            //Given
            var client = await GetAuthenticatedClient();

            var expectedErrorMessage = new List<string>
            {
                "CompanyId is not found"
            };

            //When
            var response = await client.PatchAsync<IList<string>>(RequestUris.CampaignsV2PatchFacilitators(SharedConstants.FakeCompanyId, _campaignResponse.createCampaignResponse.Id), _addFacilitatorsToCampaignRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status code doesn't match");
            Assert.That.ListsAreEqual(expectedErrorMessage, response.Dto.ToList(), "Error message doesn't match");
        }

        //400
        [TestMethod]
        [TestCategory("SiteAdmin")]
        public async Task CampaignsV2_Patch_Facilitator_With_InvalidCompanyId_BadRequest()
        {
            //Given
            var client = await GetAuthenticatedClient();

            var expectedErrorMessage = new List<string>
            {
                "'Company Id' is not valid"
            };

            //When
            var response = await client.PatchAsync<List<string>>(RequestUris.CampaignsV2PatchFacilitators(-1, _campaignResponse.createCampaignResponse.Id), _addFacilitatorsToCampaignRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status code doesn't match");
            Assert.That.ListsAreEqual(expectedErrorMessage, response.Dto.ToList(), "Error messages doesn't match");
        }

        //400
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin"), TestCategory("BLAdmin")]
        public async Task CampaignsV2_Patch_Facilitator_With_EmptyRequest_BadRequest()
        {
            //Given
            var client = await GetAuthenticatedClient();

            var expectedErrorMessage = new List<string>
            {
                "Facilitator IDs cannot be null or empty."
            };

            //When
            var response = await client.PatchAsync<List<string>>(RequestUris.CampaignsV2PatchFacilitators(Company.Id, _campaignResponse.createCampaignResponse.Id), AddFacilitatorsToCampaignRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status code doesn't match");
            Assert.AreEqual(expectedErrorMessage.ToString(), response.Dto.ToList().ToString(), "Error messages doesn't match");
        }

        //400
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin"), TestCategory("BLAdmin")]
        public async Task CampaignsV2_Patch_Facilitator_With_FakeCampaignId_BadRequest()
        {
            //Given
            var client = await GetAuthenticatedClient();

            var expectedErrorMessage = new List<string>
            {
                "CampaignId is not found"
            };

            //When
            var response = await client.PatchAsync<List<string>>(RequestUris.CampaignsV2PatchFacilitators(Company.Id, SharedConstants.FakeCampaignId), _addFacilitatorsToCampaignRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status code doesn't match");
            Assert.That.ListsAreEqual(expectedErrorMessage, response.Dto.ToList(), "Error message doesn't match");
        }

        //400
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin"), TestCategory("BLAdmin")]
        public async Task CampaignsV2_Patch_Facilitator_With_InvalidCampaignId_BadRequest()
        {
            //Given
            var client = await GetAuthenticatedClient();


            var expectedErrorMessage = new List<string>
            {
                "CampaignId is not found."
            };

            //When
            var response = await client.PatchAsync<List<string>>(RequestUris.CampaignsV2PatchFacilitators(Company.Id, -1), _addFacilitatorsToCampaignRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status code doesn't match");
            Assert.That.ListsAreEqual(expectedErrorMessage, response.Dto.ToList(), "Error message doesn't match");
        }

        //401
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("TeamAdmin"), TestCategory("Member")]
        public async Task CampaignsV2_Patch_Facilitator_With_AllRequestData_Unauthorized()
        {
            //Given
            var client = GetUnauthenticatedClient();

            //When
            var response = await client.PatchAsync<string>(RequestUris.CampaignsV2PatchFacilitators(Company.Id, _campaignResponse.createCampaignResponse.Id), _addFacilitatorsToCampaignRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status code doesn't match");
        }

        //403
        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("TeamAdmin"), TestCategory("Member")]
        public async Task CampaignsV2_Patch_Facilitator_With_FakeCompanyId_Forbidden()
        {
            //Given
            var client = await GetAuthenticatedClient();

            //When
            var response = await client.PatchAsync<string>(RequestUris.CampaignsV2PatchFacilitators(SharedConstants.FakeCompanyId, _campaignResponse.createCampaignResponse.Id), _addFacilitatorsToCampaignRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status code doesn't match");
        }

        //403
        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("TeamAdmin"), TestCategory("Member")]
        public async Task CampaignsV2_Patch_Facilitator_With_InvalidCompanyId_Forbidden()
        {
            //Given
            var client = await GetAuthenticatedClient();

            //When
            var response = await client.PatchAsync<string>(RequestUris.CampaignsV2PatchFacilitators(-1, _campaignResponse.createCampaignResponse.Id), _addFacilitatorsToCampaignRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status code doesn't match");
        }

        //403
        [TestMethod]
        [TestCategory("OrgLeader"), TestCategory("TeamAdmin"), TestCategory("Member")]
        public async Task CampaignsV2_Patch_Facilitator_With_FakeCampaignId_Forbidden()
        {
            //Given
            var client = await GetAuthenticatedClient();

            //When
            var response = await client.PatchAsync<List<string>>(RequestUris.CampaignsV2PatchFacilitators(Company.Id, SharedConstants.FakeCampaignId), _addFacilitatorsToCampaignRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status code doesn't match");
        }

        //403
        [TestMethod]
        [TestCategory("OrgLeader"), TestCategory("TeamAdmin"), TestCategory("Member")]
        public async Task CampaignsV2_Patch_Facilitator_With_AllRequestData_Forbidden()
        {
            //Given
            var client = await GetAuthenticatedClient();

            //When
            var response = await client.PatchAsync<string>(RequestUris.CampaignsV2PatchFacilitators(Company.Id, _campaignResponse.createCampaignResponse.Id), _addFacilitatorsToCampaignRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status code doesn't match");
        }
    }
}
