using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AtCommon.Api;
using AtCommon.Dtos.CampaignsV2;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiTests.v1.Endpoints.CampaignsV2
{
    [TestClass]
    [TestCategory("CampaignsV2")]
    public class AddCampaignTests : BaseV1Test
    {
        private static CreateCampaignRequest _campaignRequest;
        private static readonly List<int> CampaignId = new List<int>();

        //201
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public async Task CampaignsV2_Post_Campaign_Created()
        {
            //Given
            var client = await GetAuthenticatedClient();
             _campaignRequest = CampaignFactory.GetCampaign();

            //When
            var response = await client.PostAsync<CreateCampaignResponse>(RequestUris.CampaignsV2(Company.Id) , _campaignRequest);

            //Storing value that can be use for cleanup
            CampaignId.Add(response.Dto.Id);

            //Then
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode, "Status Code doesn't match.");
            Assert.AreEqual(_campaignRequest.Name, response.Dto.Name, "Campaign name does not match.");
            Assert.AreEqual(_campaignRequest.CreateAssessment, response.Dto.CreateAssessment, "Create Assessment value does not match.");
            Assert.AreEqual(_campaignRequest.StartDate, response.Dto.StartDate, "Start date does not match.");
            Assert.AreEqual(_campaignRequest.EndDate, response.Dto.EndDate, "End date does not match.");
            Assert.AreEqual(_campaignRequest.MatchMakingStrategy, response.Dto.MatchMakingStrategy, "Match Making Strategy does not match.");
            Assert.AreEqual(_campaignRequest.MaximumFacilitatorTeamAssignments, response.Dto.MaximumFacilitatorTeamAssignments, "Maximum Facilitator Team Assignments value does not match.");
            Assert.AreEqual(_campaignRequest.Status, response.Dto.Status, "Status does not match.");
            Assert.AreEqual(_campaignRequest.SurveyId, response.Dto.SurveyId, "Survey Id does not match.");
            Assert.IsTrue(response.Dto.Id > 0, "Id is not valid.");
        }

        //400
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        [TestCategory("BLAdmin")]
        public async Task CampaignsV2_Post_Campaign_InvalidDate_BadRequest() 
        {
            const string errorMessage = "The input was not valid.";
            //Given
            var client = await GetAuthenticatedClient();

            var request = CampaignFactory.GetCampaign();
            request.StartDate = null;
            request.EndDate = null;

            //When
            var response = await client.PostAsync<ErrorMessage>(RequestUris.CampaignsV2(Company.Id), request);

            //Then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status Code doesn't match.");
            Assert.AreEqual(errorMessage, response.Dto.StartDate.FirstOrDefault(), "Start Date parameter validation message doesn't match");
            Assert.AreEqual(errorMessage, response.Dto.EndDate.FirstOrDefault(), "End Date parameter validation message doesn't match");
        }

        // 400
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        [TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("TeamAdmin"), TestCategory("Member")]
        public async Task CampaignsV2_Post_Campaign_BadRequest()
        {
            //Given
            var client = await GetAuthenticatedClient();

            var addCampaignRequest = new CreateCampaignRequest
            {
                Name = "",
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(7),
                SurveyId = 0, // not existed 0
                MatchMakingStrategy = "",
                MaximumFacilitatorTeamAssignments = 5,
                CreateAssessment = "",
                Status = ""     //It's in progress
            };

            var errorResponseList = new List<string>
            {
                "'Campaign Name' should not be empty.",
                "Maximum Facilitator TeamAssignments should be less than 5",
                "'Survey Id' must be greater than '0'.",
                "MatchMaking strategy is not valid",
                "Create Assessment is not valid",
            };

            //When
            var response = await client.PostAsync<IList<string>>(RequestUris.CampaignsV2(Company.Id), addCampaignRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status Code doesn't match.");
            Assert.That.ListsAreEqual(errorResponseList, response.Dto.ToList(), "Error message does not match");
        }

        // 400
        [TestMethod]
        [TestCategory("SiteAdmin")]
        public async Task CampaignsV2_Post_Campaign_WithFakeCompanyId_BadRequest()
        {
            //Given
            var client = await GetAuthenticatedClient();
            var request = CampaignFactory.GetCampaign();

            var errorResponseList = new List<string>
            {
                "CompanyId is not found"
            };

            //When
            var response = await client.PostAsync<IList<string>>(RequestUris.CampaignsV2(SharedConstants.FakeCompanyId), request);

            //Then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status Code doesn't match.");
            Assert.That.ListsAreEqual(errorResponseList, response.Dto.ToList(), "Error message does not match");
        }

        //401
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        [TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("TeamAdmin"), TestCategory("Member")]
        public async Task CampaignsV2_Post_Campaign_Unauthorized()
        {
            //Given
            var client = GetUnauthenticatedClient();
            var request = CampaignFactory.GetCampaign();

            //When
            var response = await client.PostAsync<CreateCampaignResponse>(RequestUris.CampaignsV2(Company.Id), request);

            //Then
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status Code doesn't match.");
        }

        //403
        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("TeamAdmin"), TestCategory("Member")]
        public async Task CampaignsV2_Post_Campaign_WithFakeCompany_Forbidden()
        {
            //Given
            var client = await GetAuthenticatedClient();
            var request = CampaignFactory.GetCampaign();

            //When
            var response = await client.PostAsync<IList<string>>(RequestUris.CampaignsV2(SharedConstants.FakeCompanyId), request);

            //Then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status Code doesn't match.");
        }

        // 403
        [TestMethod]
        [TestCategory("OrgLeader"), TestCategory("TeamAdmin"), TestCategory("Member")]
        public async Task CampaignsV2_Post_Campaign_WithDifferentUsers_Forbidden()
        {
            //Given
            var client = await GetAuthenticatedClient();

            var request = CampaignFactory.GetCampaign();

            //When
            var response = await client.PostAsync<CreateCampaignResponse>(RequestUris.CampaignsV2(Company.Id), request);

            //Then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status Code doesn't match.");
        }

        // 403   
        // Campaign Feature Off for Automation_2FA (DO NOT USE) - 1869
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        [TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("TeamAdmin"), TestCategory("Member")]
        public async Task CampaignsV2_Post_Campaign_WithDifferentCompanyFeatureOff_Forbidden()
        {
            //Given
            var client = await GetAuthenticatedClient();
            var request = CampaignFactory.GetCampaign();

            //When
            var response = await client.PostAsync<CreateCampaignResponse>(RequestUris.CampaignsV2(SharedConstants.Automation2FaDoNotUseCompanyId), request);

            //Then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status Code doesn't match.");
        }


        [ClassCleanup]
        public static void ClassTearDown()
        {
            if (!User.IsSiteAdmin() && !User.IsPartnerAdmin() && !User.IsCompanyAdmin() && CampaignId.Count==0) return;
            var setupApi = new SetupTeardownApi(TestEnvironment);
            setupApi.DeleteCampaign(Company.Id, CampaignId).GetAwaiter().GetResult();
        }
    }
}
