using System;
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
    public class DeletePublishedCampaignsFacilitatorsTests : BaseV1Test
    {
        private static CreateCampaignRequest _request1;
        private static CreateCampaignRequest _request2;
        private static (CreateCampaignResponse createCampaignResponse, SaveAsDraftResponse saveAsDraftResponse) _campaignResponse1;
        private static (CreateCampaignResponse createCampaignResponse, SaveAsDraftResponse saveAsDraftResponse) _campaignResponse2;
        private static User SiteAdminUser => new UserConfig("SA").GetUserByDescription("user 1");

        private const int TargetRatio = 2;
        private const int NumberOfTeams = 4;
        private const int NumberOfFacilitators = 2;

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            //Create new Campaign and Assessment
            var setupApi = new SetupTeardownApi(TestEnvironment);

            _request1 = CampaignFactory.GetCampaign();
            _campaignResponse1 = setupApi.CreateAndSetupCampaign(Company.Id, _request1, TargetRatio, NumberOfTeams, NumberOfFacilitators, SiteAdminUser);

            _request2 = CampaignFactory.GetCampaign();
            _campaignResponse2 = setupApi.CreateAndSetupCampaign(Company.Id, _request2, TargetRatio, NumberOfTeams, NumberOfFacilitators, SiteAdminUser);
        }

        //200
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public async Task CampaignsV2_Delete_Facilitator_Success()
        {
            //Given
            var client = await GetAuthenticatedClient();

            //When
            var response = await client.DeleteAsync<string>(RequestUris.CampaignsV2DeleteFacilitators(Company.Id, _campaignResponse1.createCampaignResponse.Id).AddQueryParameter("facilitatorIds", CampaignFactory.FacilitatorId));

            //Then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status Code doesn't match");
            Assert.AreEqual("Ok", response.Dto, "Message doesn't matched");

            var getDeletedFacilitator1 = await client.GetAsync<CampaignSetupResponse>(RequestUris.CampaignsV2Setup(Company.Id, _campaignResponse1.createCampaignResponse.Id));

            Assert.That.ListNotContains(getDeletedFacilitator1.Dto.SelectedFacilitators, CampaignFactory.FacilitatorIdsList.Last().ToList(), "Deleted facilitator id is still exist");

        }

        // 200
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public async Task CampaignsV2_Delete_MultipleFacilitators_Success()
        {
            // Given
            var client = await GetAuthenticatedClient();
            var queryString = string.Join("&", CampaignFactory.FacilitatorIdsList.Select(fid => $"facilitatorIds={Uri.EscapeDataString(fid)}"));

            // When
            var response = await client.DeleteAsync<string>($"{RequestUris.CampaignsV2DeleteFacilitators(Company.Id, _campaignResponse2.createCampaignResponse.Id)}?{queryString}");
            response.EnsureSuccess();

            var getDeletedFacilitator = await client.GetAsync<CampaignSetupResponse>(RequestUris.CampaignsV2Setup(Company.Id, _campaignResponse2.createCampaignResponse.Id));

            // Then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status Code doesn't match");
            Assert.AreEqual("Ok", response.Dto, "Message doesn't match");
            Assert.IsNull(getDeletedFacilitator.Dto.SelectedFacilitators, "'FacilitatorsIds' list is not null");
        }

        //400
        [TestMethod]
        [TestCategory("SiteAdmin")]
        public async Task CampaignsV2_Delete_Facilitator_AllEmptyDetails_BadRequest()
        {
            //Given
            var client = await GetAuthenticatedClient();

            var expectedErrorMessage = new List<string>
            {
                "'Company Id' is not valid",
                "'Campaign Id' must not be empty.",
                "CampaignId is not found."
            };

            //When
            var response = await client.DeleteAsync<List<string>>(RequestUris.CampaignsV2DeleteFacilitators(0, 0).AddQueryParameter("facilitatorIds", 0));

            //Then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status Code doesn't match.");
            Assert.That.ListsAreEqual(expectedErrorMessage, response.Dto.ToList(), "Error message doesn't match");
        }

        //400
        [TestMethod]
        [TestCategory("SiteAdmin")]
        public async Task CampaignsV2_Delete_Facilitator_FakeCompanyId_BadRequest()
        {
            //Given
            var client = await GetAuthenticatedClient();

            var expectedErrorMessage = new List<string>
            {
                "CompanyId is not found"
            };

            //When
            var response = await client.DeleteAsync<List<string>>(RequestUris.CampaignsV2DeleteFacilitators(SharedConstants.FakeCompanyId, _campaignResponse1.createCampaignResponse.Id).AddQueryParameter("facilitatorIds", CampaignFactory.FacilitatorId));

            //Then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status Code doesn't match.");
            Assert.That.ListsAreEqual(expectedErrorMessage, response.Dto.ToList(), "Error message doesn't match");
        }

        //400
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public async Task CampaignsV2_Delete_Facilitator_FakeCampaignId_BadRequest()
        {
            //Given
            var client = await GetAuthenticatedClient();

            var expectedErrorMessage = new List<string>
            {
                "CampaignId is not found"
            };

            //When
            var response = await client.DeleteAsync<List<string>>(RequestUris.CampaignsV2DeleteFacilitators(Company.Id, SharedConstants.FakeCampaignId).AddQueryParameter("facilitatorIds", CampaignFactory.FacilitatorId));

            //Then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status Code doesn't match.");
            Assert.That.ListsAreEqual(expectedErrorMessage, response.Dto.ToList(), "Error message doesn't match");
        }

        //400
        [TestMethod]
        [TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public async Task CampaignsV2_Delete_Facilitator_EmptyGuidFacilitatorId_BadRequest()
        {
            //Given
            var client = await GetAuthenticatedClient();

            var expectedErrorMessage = new List<string>
            {
                "Invalid Facilitator."
            };

            //When
            var response = await client.DeleteAsync<List<string>>(RequestUris.CampaignsV2DeleteFacilitators(Company.Id, _campaignResponse1.createCampaignResponse.Id).AddQueryParameter("facilitatorIds", new Guid()));

            //Then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status Code doesn't match.");
            Assert.That.ListsAreEqual(expectedErrorMessage, response.Dto.ToList(), " Error message doesn't match");
        }

        //401
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("TeamAdmin"), TestCategory("Member")]
        public async Task CampaignsV2_Delete_Facilitator_Unauthorized()
        {
            //Given
            var client = GetUnauthenticatedClient();

            //When
            var response = await client.DeleteAsync<string>(RequestUris.CampaignsV2DeleteFacilitators(Company.Id, _campaignResponse1.createCampaignResponse.Id).AddQueryParameter("facilitatorIds", CampaignFactory.FacilitatorId));

            //Then
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status Code doesn't match");

        }

        //403
        [TestMethod]
        [TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("TeamAdmin"), TestCategory("Member")]
        public async Task CampaignsV2_Delete_Facilitator_FakeCompanyId_Forbidden()
        {
            //Given
            var client = await GetAuthenticatedClient();

            //When
            var response = await client.DeleteAsync<string>(RequestUris.CampaignsV2DeleteFacilitators(SharedConstants.FakeCompanyId, _campaignResponse1.createCampaignResponse.Id).AddQueryParameter("facilitatorIds", CampaignFactory.FacilitatorId));

            //Then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status Code doesn't match");
        }

        //403
        [TestMethod]
        [TestCategory("OrgLeader"), TestCategory("TeamAdmin"), TestCategory("Member")]
        public async Task CampaignsV2_Delete_Facilitator_Forbidden()
        {
            //Given
            var client = await GetAuthenticatedClient();

            //When
            var response = await client.DeleteAsync<string>(RequestUris.CampaignsV2DeleteFacilitators(Company.Id, _campaignResponse1.createCampaignResponse.Id).AddQueryParameter("facilitatorIds", CampaignFactory.FacilitatorId));

            //Then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status Code doesn't match");

        }
    }
}
