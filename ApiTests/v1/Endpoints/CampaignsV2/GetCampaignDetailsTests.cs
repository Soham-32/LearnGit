using System.Collections.Generic;
using System.Globalization;
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
    public class GetCampaignDetailsTests : BaseV1Test
    {
        private static CreateCampaignRequest _campaignRequest;
        private static CreateCampaignResponse _campaignResponse;
        private static User SiteAdminUser => new UserConfig("SA").GetUserByDescription("user 1");

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            _campaignRequest = CampaignFactory.GetCampaign();
            _campaignResponse = new SetupTeardownApi(TestEnvironment).CreateCampaign(Company.Id, _campaignRequest, SiteAdminUser);
        }

        //200
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public async Task CampaignsV2_Get_CampaignDetails_Success()
        {
            //Given
            var client = await GetAuthenticatedClient();

            //When
            var response = await client.GetAsync<CreateCampaignResponse>(RequestUris.CampaignsV2Details(Company.Id, _campaignResponse.Id));

            //Then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status Code doesn't match.");
            Assert.AreEqual(_campaignResponse.Name, response.Dto.Name, "Campaign name does not match.");
            Assert.AreEqual(_campaignResponse.CreateAssessment, response.Dto.CreateAssessment, "Create Assessment value does not match.");
            Assert.AreEqual(_campaignResponse.StartDate.ToString(CultureInfo.InvariantCulture), response.Dto.StartDate.ToString(CultureInfo.InvariantCulture), "Start date does not match.");
            Assert.AreEqual(_campaignResponse.EndDate.ToString(CultureInfo.InvariantCulture), response.Dto.EndDate.ToString(CultureInfo.InvariantCulture), "End date does not match.");
            Assert.AreEqual(_campaignResponse.MatchMakingStrategy, response.Dto.MatchMakingStrategy, "Match Making Strategy does not match.");
            Assert.AreEqual(_campaignResponse.MaximumFacilitatorTeamAssignments, response.Dto.MaximumFacilitatorTeamAssignments, "Maximum Facilitator Team Assignments value does not match.");
            Assert.AreEqual(_campaignResponse.Status, response.Dto.Status, "Status does not match.");
            Assert.AreEqual(_campaignResponse.SurveyId, response.Dto.SurveyId, "Survey Id does not match.");
            Assert.AreEqual(_campaignResponse.Id, response.Dto.Id, "Id is not valid.");
        }


        //400
        [TestMethod]
        [TestCategory("SiteAdmin")]
        public async Task CampaignsV2_Get_CampaignDetails_WithFakeCompanyId_BadRequest()
        {
            //Given
            var client = await GetAuthenticatedClient();

            var errorResponseList = new List<string>
            {
                "CompanyId is not found"
            };

            //When
            var response = await client.GetAsync<IList<string>>(RequestUris.CampaignsV2Details(SharedConstants.FakeCompanyId, _campaignResponse.Id));

            //Then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status Code doesn't match.");
            Assert.That.ListsAreEqual(errorResponseList, response.Dto.ToList(), "Error message does not match");
        }


        //400
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public async Task CampaignsV2_Get_CampaignDetails_WithFakeCampaignId_BadRequest()
        {
            //Given
            var client = await GetAuthenticatedClient();

            var errorResponseList = new List<string>
            {
                "CampaignId is not found"
            };

            //When
            var response = await client.GetAsync<IList<string>>(RequestUris.CampaignsV2Details(Company.Id, SharedConstants.FakeCampaignId));

            //Then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status Code doesn't match.");
            Assert.That.ListsAreEqual(errorResponseList, response.Dto.ToList(), "Error message does not match");
        }

        //401
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        [TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("TeamAdmin"), TestCategory("Member")]
        public async Task CampaignsV2_Get_CampaignDetails_Unauthorized()
        {
            //Given
            var client = GetUnauthenticatedClient();

            //When
            var response = await client.GetAsync<CreateCampaignResponse>(RequestUris.CampaignsV2Details(Company.Id, _campaignResponse.Id));

            //Then
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status Code doesn't match.");
        }

        //403
        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("TeamAdmin"), TestCategory("Member")]
        public async Task CampaignsV2_Get_CampaignDetails_With_FakeCompanyId_Forbidden()
        {
            //Given
            var client = await GetAuthenticatedClient();

            //When
            var response = await client.GetAsync<IList<string>>(RequestUris.CampaignsV2Details(SharedConstants.FakeCompanyId, _campaignResponse.Id));

            //Then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status Code doesn't match.");
        }

        // 403
        [TestMethod]
        [TestCategory("OrgLeader"), TestCategory("TeamAdmin"), TestCategory("Member")]
        public async Task CampaignsV2_Get_CampaignDetails_WithDifferentUsers_Forbidden()
        {
            //Given
            var client = await GetAuthenticatedClient();

            //When
            var response = await client.GetAsync<IList<string>>(RequestUris.CampaignsV2Details(Company.Id, _campaignResponse.Id));

            //Then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status Code doesn't match.");
        }
    }
}
