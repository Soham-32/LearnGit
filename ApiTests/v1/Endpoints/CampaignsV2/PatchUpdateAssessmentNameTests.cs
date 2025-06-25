using System.Collections.Generic;
using System.Linq;
using AtCommon.Api;
using AtCommon.Dtos.CampaignsV2;
using AtCommon.Dtos;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.Threading.Tasks;
using System;

namespace ApiTests.v1.Endpoints.CampaignsV2
{
    [TestClass]
    [TestCategory("CampaignsV2")]
    public class PatchUpdateAssessmentNameTests : BaseV1Test
    {
        private static CreateCampaignRequest _request;
        private static (CreateCampaignResponse createCampaignResponse, SaveAsDraftResponse saveAsDraftResponse) _campaignResponse;
        private static UpdateAssessmentNameRequest _updateAssessmentNameRequest;
        public static int TargetRatio = 2;
        public static int NumberOfTeams = 4;
        public static int NumberOfFacilitators = 2;
        private static User SiteAdminUser => new UserConfig("SA").GetUserByDescription("user 1");
        public static UpdateAssessmentNameRequest UpdateAssessmentNameEmptyRequest = new UpdateAssessmentNameRequest
        {
            AssessmentName = "",
            TeamIds = new List<int>()
        };

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            //Create new Campaign and Publish it.
            var setupApi = new SetupTeardownApi(TestEnvironment);

            _request = CampaignFactory.GetCampaign();
            _campaignResponse = setupApi.CreateAndSetupCampaign(Company.Id, _request, TargetRatio, NumberOfTeams, NumberOfFacilitators, SiteAdminUser);

            _updateAssessmentNameRequest = CampaignFactory.UpdateAssessmentName(
                _campaignResponse.createCampaignResponse.Name, _campaignResponse.saveAsDraftResponse.SelectedTeams);
        }

        //200
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public async Task CampaignsV2_Patch_Update_AssessmentName_Success()
        {
            //Given
            var client = await GetAuthenticatedClient();

            //When
            var response = await client.PatchAsync<string>(RequestUris.CampaignsV2PatchUpdateAssessmentName(Company.Id, _campaignResponse.createCampaignResponse.Id), _updateAssessmentNameRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status Code doesn't match");
            Assert.AreEqual("OK", response.Dto, "Message doesn't matched");
        }

        //200
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public async Task CampaignsV2_Patch_Update_Single_Team_AssessmentName_Success()
        {
            //Given
            var client = await GetAuthenticatedClient();
            var random = new Random();
            var updateAssessmentNameRequest = CampaignFactory.UpdateAssessmentName(
                _campaignResponse.createCampaignResponse.Name, _campaignResponse.saveAsDraftResponse.SelectedTeams.OrderBy(x => random.Next()).Take(1).ToList());

            //When
            var response = await client.PatchAsync<string>(RequestUris.CampaignsV2PatchUpdateAssessmentName(Company.Id, _campaignResponse.createCampaignResponse.Id), updateAssessmentNameRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status Code doesn't match");
            Assert.AreEqual("OK", response.Dto, "Message doesn't matched");
        }

        //400
        [TestMethod]
        [TestCategory("SiteAdmin")]
        public async Task CampaignsV2_Patch_Update_AssessmentName_FakeCompanyId_BadRequest()
        {
            //Given
            var client = await GetAuthenticatedClient();

            var expectedErrorMessage = new List<string>
            {
                "CompanyId is not found"
            };

            //When
            var response = await client.PatchAsync<List<string>>(RequestUris.CampaignsV2PatchUpdateAssessmentName(SharedConstants.FakeCompanyId, _campaignResponse.createCampaignResponse.Id), _updateAssessmentNameRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status code doesn't match");
            Assert.That.ListsAreEqual(expectedErrorMessage, response.Dto.ToList(), "Error message doesn't match");
        }

        //400
        [TestMethod]
        [TestCategory("SiteAdmin")]
        public async Task CampaignsV2_Patch_Update_AssessmentName_InvalidCompanyId_BadRequest()
        {
            //Given
            var client = await GetAuthenticatedClient();

            var expectedErrorMessage = new List<string>
            {
                "CompanyId is not found"
            };

            //When
            var response = await client.PatchAsync<List<string>>(RequestUris.CampaignsV2PatchUpdateAssessmentName(-1, _campaignResponse.createCampaignResponse.Id), _updateAssessmentNameRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status code doesn't match");
            Assert.That.ListsAreEqual(expectedErrorMessage, response.Dto.ToList(), "Error message doesn't match");
        }

        //400
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin"), TestCategory("BLAdmin")]
        public async Task CampaignsV2_Patch_Update_AssessmentName_With_EmptyRequest_BadRequest()
        {
            //Given
            var client = await GetAuthenticatedClient();

            var expectedErrorMessage = new List<string>
            {
                "'Assessment Name' should not be empty."
            };

            //When
            var response = await client.PatchAsync<List<string>>(RequestUris.CampaignsV2PatchUpdateAssessmentName(Company.Id, _campaignResponse.createCampaignResponse.Id), UpdateAssessmentNameEmptyRequest);


            //Then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status code doesn't match");
            Assert.AreEqual(expectedErrorMessage.ToString(), response.Dto.ToList().ToString(), "Error messages doesn't match");
        }

        //400
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin"), TestCategory("BLAdmin")]
        public async Task CampaignsV2_Patch_Update_AssessmentName_With_InvalidTeamID_BadRequest()
        {
            //Given
            var client = await GetAuthenticatedClient();

            var updateAssessmentNameWithInvalidTeamIdRequest = UpdateAssessmentNameEmptyRequest;
            updateAssessmentNameWithInvalidTeamIdRequest.AssessmentName=$"Updated_{_campaignResponse.createCampaignResponse.Name}";
            updateAssessmentNameWithInvalidTeamIdRequest.TeamIds = new List<int>{-1};

            var expectedErrorMessage = new List<string>
            {
                "Invalid teamId -1"
            };

            //When
            var response = await client.PatchAsync<List<string>>(RequestUris.CampaignsV2PatchUpdateAssessmentName(Company.Id, _campaignResponse.createCampaignResponse.Id), UpdateAssessmentNameEmptyRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status code doesn't match");
            Assert.AreEqual(expectedErrorMessage.ToString(), response.Dto.ToList().ToString(), "Error messages doesn't match");
        }

        //400
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin"), TestCategory("BLAdmin")]
        public async Task CampaignsV2_Patch_Update_AssessmentName_FakeCampaignId_BadRequest()
        {
            //Given
            var client = await GetAuthenticatedClient();

            var expectedErrorMessage = new List<string>
            {
                "CampaignId is not found"
            };

            //When
            var response = await client.PatchAsync<List<string>>(RequestUris.CampaignsV2PatchUpdateAssessmentName(Company.Id, SharedConstants.FakeCampaignId), _updateAssessmentNameRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status code doesn't match");
            Assert.That.ListsAreEqual(expectedErrorMessage, response.Dto.ToList(), "Error message doesn't match");
        }

        //400
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin"), TestCategory("BLAdmin")]
        public async Task CampaignsV2_Patch_Update_AssessmentName_InvalidCampaignId_BadRequest()
        {
            //Given
            var client = await GetAuthenticatedClient();

            var expectedErrorMessage = new List<string>
            {
                "'Campaign Id' must be greater than '0'."
            };

            //When
            var response = await client.PatchAsync<List<string>>(RequestUris.CampaignsV2PatchUpdateAssessmentName(Company.Id, -1), _updateAssessmentNameRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status code doesn't match");
            Assert.That.ListsAreEqual(expectedErrorMessage, response.Dto.ToList(), "Error message doesn't match");
        }

        //401
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("TeamAdmin"), TestCategory("Member")]
        public async Task CampaignsV2_Patch_Update_AssessmentName_With_AllRequestData_Unauthorized()
        {
            //Given
            var client = GetUnauthenticatedClient();

            //When
            var response = await client.PatchAsync<string>(RequestUris.CampaignsV2PatchUpdateAssessmentName(Company.Id, _campaignResponse.createCampaignResponse.Id), _updateAssessmentNameRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status code doesn't match");
        }

        //403
        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("TeamAdmin"), TestCategory("Member")]
        public async Task CampaignsV2_Patch_Update_AssessmentName_With_FakeCompanyId_Forbidden()
        {
            //Given
            var client = await GetAuthenticatedClient();

            //When
            var response = await client.PatchAsync<string>(RequestUris.CampaignsV2PatchUpdateAssessmentName(SharedConstants.FakeCompanyId, _campaignResponse.createCampaignResponse.Id), _updateAssessmentNameRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status code doesn't match");
        }

        //403
        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("TeamAdmin"), TestCategory("Member")]
        public async Task CampaignsV2_Patch_Update_AssessmentName_With_InvalidCompanyId_Forbidden()
        {
            //Given
            var client = await GetAuthenticatedClient();

            //When
            var response = await client.PatchAsync<string>(RequestUris.CampaignsV2PatchUpdateAssessmentName(-1, _campaignResponse.createCampaignResponse.Id), _updateAssessmentNameRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status code doesn't match");
        }

        //403
        [TestMethod]
        [TestCategory("OrgLeader"), TestCategory("TeamAdmin"), TestCategory("Member")]
        public async Task CampaignsV2_Patch_Update_AssessmentName_With_FakeCampaignId_Forbidden()
        {
            //Given
            var client = await GetAuthenticatedClient();

            //When
            var response = await client.PatchAsync<string>(RequestUris.CampaignsV2PatchUpdateAssessmentName(Company.Id, SharedConstants.FakeCampaignId), _updateAssessmentNameRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status code doesn't match");
        }

        //403
        [TestMethod]
        [TestCategory("OrgLeader"), TestCategory("TeamAdmin"), TestCategory("Member")]
        public async Task CampaignsV2_Patch_Update_AssessmentName_Forbidden()
        {
            //Given
            var client = await GetAuthenticatedClient();

            //When
            var response = await client.PatchAsync<string>(RequestUris.CampaignsV2PatchUpdateAssessmentName(Company.Id, _campaignResponse.createCampaignResponse.Id), _updateAssessmentNameRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status code doesn't match");
        }
    }
}
