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
    public class UpdateCampaignTests : BaseV1Test
    {
        private static CreateCampaignRequest _campaignRequest1;
        private static CreateCampaignResponse _campaignResponse1;
        private static readonly List<int> UpdatedCampaignId = new List<int>();
        private static User SiteAdminUser => new UserConfig("SA").GetUserByDescription("user 1");



        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            _campaignRequest1 = CampaignFactory.GetCampaign();
            _campaignResponse1 = new SetupTeardownApi(TestEnvironment).CreateCampaign(Company.Id, _campaignRequest1, SiteAdminUser);
        }

        //201
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public async Task CampaignsV2_Patch_Campaign_Created()
        {
            //Given
            var client = await GetAuthenticatedClient();
             var updatedCampaignRequest = CampaignFactory.GetUpdatedCampaign(_campaignResponse1.Id);

            //When
            var updatedCampaignResponse = await client.PutAsync<CreateCampaignResponse>(RequestUris.CampaignsV2Details(Company.Id,_campaignResponse1.Id), updatedCampaignRequest);

            //Storing value that can be use for cleanup
            UpdatedCampaignId.Add(updatedCampaignResponse.Dto.Id);

            //Then
            Assert.AreEqual(HttpStatusCode.Created, updatedCampaignResponse.StatusCode, "Status Code doesn't match.");
            Assert.AreEqual(updatedCampaignRequest.Name, updatedCampaignResponse.Dto.Name, "Campaign name does not match.");
            Assert.AreEqual(updatedCampaignRequest.CreateAssessment, updatedCampaignResponse.Dto.CreateAssessment,
                "Create Assessment value does not match.");
            Assert.AreEqual(updatedCampaignRequest.StartDate, updatedCampaignResponse.Dto.StartDate, "Start date does not match.");
            Assert.AreEqual(updatedCampaignRequest.EndDate, updatedCampaignResponse.Dto.EndDate, "End date does not match.");
            Assert.AreEqual(updatedCampaignRequest.MatchMakingStrategy, updatedCampaignResponse.Dto.MatchMakingStrategy, "Match Making Strategy does not match.");
            Assert.AreEqual(updatedCampaignRequest.MaximumFacilitatorTeamAssignments, updatedCampaignResponse.Dto.MaximumFacilitatorTeamAssignments, "Maximum Facilitator Team Assignments value does not match.");
            Assert.AreEqual(updatedCampaignRequest.Status, updatedCampaignResponse.Dto.Status, "Status does not match.");
            Assert.AreEqual(updatedCampaignRequest.SurveyId, updatedCampaignResponse.Dto.SurveyId, "Survey Id does not match.");
            Assert.AreEqual(updatedCampaignRequest.CampaignId, updatedCampaignResponse.Dto.Id, "Survey Id does not match.");
        }

        //400
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        [TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("TeamAdmin"), TestCategory("Member")]
        public async Task CampaignsV2_Patch_With_InvalidCampaignId_And_RequestParameter_BadRequest()
        {
            //Given
            var client = await GetAuthenticatedClient();
            var updateCampaignRequest = CampaignFactory.GetUpdatedCampaign(_campaignResponse1.Id);

            updateCampaignRequest.Name = "";
            updateCampaignRequest.SurveyId = 0;
            updateCampaignRequest.MatchMakingStrategy = "";
            updateCampaignRequest.MaximumFacilitatorTeamAssignments = 5;
            updateCampaignRequest.CreateAssessment = "";
            updateCampaignRequest.Status = "";
            updateCampaignRequest.CampaignId = 0;

            var errorResponseList = new List<string>
            {
                "'Campaign Id' must be greater than '0'.",
                "'Campaign Name' should not be empty.",
                "Maximum Facilitator TeamAssignments should be less than 5",
                "'Survey Id' must be greater than '0'.",
                "MatchMaking strategy is not valid",
                "Create Assessment is not valid",
            };

            //When
            var updatedResponse = await client.PutAsync<IList<string>>(RequestUris.CampaignsV2Details(Company.Id,0), updateCampaignRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.BadRequest, updatedResponse.StatusCode, "Status Code doesn't match.");
            Assert.That.ListsAreEqual(errorResponseList, updatedResponse.Dto.ToList(), "Error message does not match");
        }

        //400
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        [TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("TeamAdmin"), TestCategory("Member")]
        public async Task CampaignsV2_Patch_CampaignId_InValidRequestParameter_BadRequest()
        {
            //Given
            var client = await GetAuthenticatedClient();
            var updateCampaignRequest = CampaignFactory.GetUpdatedCampaign(_campaignResponse1.Id);

            updateCampaignRequest.Name = "";
            updateCampaignRequest.SurveyId = 0;
            updateCampaignRequest.MatchMakingStrategy = "";
            updateCampaignRequest.MaximumFacilitatorTeamAssignments = 5;
            updateCampaignRequest.CreateAssessment = "";
            updateCampaignRequest.Status = "";
            updateCampaignRequest.CampaignId = 0;

            //When
            var updatedCampaignResponse = await client.PutAsync<string>(RequestUris.CampaignsV2Details(Company.Id, _campaignResponse1.Id), updateCampaignRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.BadRequest, updatedCampaignResponse.StatusCode, "Status Code doesn't match.");
            Assert.AreEqual("Invalid Request", updatedCampaignResponse.Dto,"Invalid message doesn't match");
        }

        // 400
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        [TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("TeamAdmin"), TestCategory("Member")]
        public async Task CampaignsV2_Patch_CampaignId_BadRequest()
        {
            //Given
            var client = await GetAuthenticatedClient();
            var updateCampaignRequest = CampaignFactory.GetUpdatedCampaign(_campaignResponse1.Id);

            updateCampaignRequest.Name = "";
            updateCampaignRequest.SurveyId = 0;
            updateCampaignRequest.MatchMakingStrategy = "";
            updateCampaignRequest.MaximumFacilitatorTeamAssignments = 5;
            updateCampaignRequest.CreateAssessment = "";
            updateCampaignRequest.Status = "";
            updateCampaignRequest.CampaignId = _campaignResponse1.Id;

            //When
            var updatedCampaignResponse = await client.PutAsync<string>(RequestUris.CampaignsV2Details(Company.Id,0), updateCampaignRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.BadRequest, updatedCampaignResponse.StatusCode, "Status Code doesn't match.");
            Assert.AreEqual("Invalid Request", updatedCampaignResponse.Dto, "Invalid message doesn't match");
        }

        //400
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        [TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("TeamAdmin"), TestCategory("Member")]
        public async Task CampaignsV2_Patch_Campaign_WithStartAndEndDate_Null_BadRequest()
        {
            const string errorMessage = "The input was not valid.";
            var updateCampaignRequest = CampaignFactory.GetUpdatedCampaign(_campaignResponse1.Id);

            //Given
            var client = await GetAuthenticatedClient();

            updateCampaignRequest.StartDate = null;
            updateCampaignRequest.EndDate = null;

            //When
            var updatedCampaignResponse = await client.PutAsync<ErrorMessage>(RequestUris.CampaignsV2Details(Company.Id, _campaignResponse1.Id), updateCampaignRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.BadRequest, updatedCampaignResponse.StatusCode, "Status Code doesn't match.");
            Assert.AreEqual(errorMessage, updatedCampaignResponse.Dto.StartDate.FirstOrDefault(), "Start Date parameter validation message doesn't match");
            Assert.AreEqual(errorMessage, updatedCampaignResponse.Dto.EndDate.FirstOrDefault(), "Start Date parameter validation message doesn't match");
        }

        //400
        [TestMethod]
        [TestCategory("SiteAdmin")] 
        public async Task CampaignsV2_Patch_Campaign_WithFakeCompanyId_BadRequest()
        {
            //Given
            var client = await GetAuthenticatedClient();
            var updatedCampaignRequest = CampaignFactory.GetUpdatedCampaign(_campaignResponse1.Id);

            var errorResponseList = new List<string>
            {
                "CompanyId is not found"
            };

            //When
            var updatedCampaignResponse = await client.PutAsync<IList<string>>(RequestUris.CampaignsV2Details(SharedConstants.FakeCompanyId,_campaignResponse1.Id), updatedCampaignRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.BadRequest, updatedCampaignResponse.StatusCode, "Status Code doesn't match.");
            Assert.That.ListsAreEqual(errorResponseList, updatedCampaignResponse.Dto.ToList(), "Error message does not match");
        }

        //401
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        [TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("TeamAdmin"), TestCategory("Member")]
        public async Task CampaignsV2_Patch_Campaign_Unauthorized()
        {
            //Given
            var client = GetUnauthenticatedClient();
            var updatedCampaignRequest = CampaignFactory.GetUpdatedCampaign(_campaignResponse1.Id);

            //When
            var updatedCampaignResponse = await client.PutAsync<CreateCampaignResponse>(RequestUris.CampaignsV2Details(Company.Id,_campaignResponse1.Id), updatedCampaignRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.Unauthorized, updatedCampaignResponse.StatusCode, "Status Code doesn't match.");
        }

        //403
        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("TeamAdmin"), TestCategory("Member")]
        public async Task CampaignsV2_Patch_Campaign_WithFakeCompanyId_Forbidden()
        {
            //Given
            var client = await GetAuthenticatedClient(); 
            var updatedCampaignRequest = CampaignFactory.GetUpdatedCampaign(_campaignResponse1.Id);


            //When
            var updatedCampaignResponse = await client.PutAsync<IList<string>>(RequestUris.CampaignsV2Details(SharedConstants.FakeCompanyId,_campaignResponse1.Id), updatedCampaignRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.Forbidden, updatedCampaignResponse.StatusCode, "Status Code doesn't match.");
        }

        // 403
        [TestMethod]
        [TestCategory("OrgLeader"), TestCategory("TeamAdmin"), TestCategory("Member")]
        public async Task CampaignsV2_Patch_Campaign_WithDifferentUsers_Forbidden()
        {
            //Given
            var client = await GetAuthenticatedClient();
            var updatedCampaignRequest = CampaignFactory.GetUpdatedCampaign(_campaignResponse1.Id);


            //When
            var updatedCampaignResponse = await client.PutAsync<IList<string>>(RequestUris.CampaignsV2Details(Company.Id,_campaignResponse1.Id), updatedCampaignRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.Forbidden, updatedCampaignResponse.StatusCode, "Status Code doesn't match.");
        }

        [ClassCleanup]
        public static void ClassTearDown()
        {
            if (!User.IsSiteAdmin() && !User.IsPartnerAdmin() && !User.IsCompanyAdmin()) return;
            var setupApi = new SetupTeardownApi(TestEnvironment);
            setupApi.DeleteCampaign(Company.Id, UpdatedCampaignId).GetAwaiter().GetResult();
        }
    }
}
