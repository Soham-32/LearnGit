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
    public class DeleteCampaignTests : BaseV1Test
    {
        private static CreateCampaignRequest _campaignRequest1;
        private static CreateCampaignRequest _campaignRequest2;
        private static CreateCampaignResponse _campaignResponse1;
        private static CreateCampaignResponse _campaignResponse2;
        private static User SiteAdminUser => new UserConfig("SA").GetUserByDescription("user 1");

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            var setUpApi = new SetupTeardownApi(TestEnvironment);
            _campaignRequest1 = CampaignFactory.GetCampaign();
            _campaignRequest2 = CampaignFactory.GetCampaign();
            _campaignResponse1 = setUpApi.CreateCampaign(Company.Id, _campaignRequest1, SiteAdminUser);
            _campaignResponse2 = setUpApi.CreateCampaign(Company.Id, _campaignRequest2, SiteAdminUser);
        }

        //200
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public async Task CampaignsV2_Delete_Campaign_Success()
        {
            //Given
            var client = await GetAuthenticatedClient();

            //When
            var response = await client.DeleteAsync(RequestUris.CampaignsV2Delete(Company.Id, _campaignResponse2.Id));

            //Then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status Code doesn't match.");
        }

        //400
        [TestMethod]
        [TestCategory("SiteAdmin")]
        public async Task CampaignsV2_Delete_Campaign_WithFakeCompanyId_BadRequest()
        {
            //Given
            var client = await GetAuthenticatedClient();

            var errorResponseList = new List<string>
            {
                "CompanyId is not found"
            };

            //When
            var response = await client.DeleteAsync<IList<string>>(RequestUris.CampaignsV2Delete(SharedConstants.FakeCompanyId, _campaignResponse1.Id));

            //Then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status Code doesn't match.");
            Assert.That.ListsAreEqual(errorResponseList, response.Dto.ToList(), "Response error message doesn't match");
        }

        // 401
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        [TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("TeamAdmin"), TestCategory("Member")]
        public async Task CampaignsV2_Delete_Campaign_Unauthorized()
        {
            //Given
            var client = GetUnauthenticatedClient();

            //When
            var response = await client.DeleteAsync(RequestUris.CampaignsV2Delete(Company.Id, _campaignResponse1.Id));

            //Then
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status Code doesn't match.");
        }

        // 403
        [TestMethod]
        [TestCategory("OrgLeader"), TestCategory("TeamAdmin"), TestCategory("Member")]
        public async Task CampaignsV2_Delete_Campaign_WithDifferentUsers_Forbidden()
        {
            //Given
            var client = await GetAuthenticatedClient();

            //When
            var response = await client.DeleteAsync(RequestUris.CampaignsV2Delete(Company.Id, _campaignResponse1.Id));

            //Then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status Code doesn't match.");
        }

        // 403
        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("TeamAdmin"), TestCategory("Member")]
        public async Task CampaignsV2_Delete_Campaign_WithFakeCompanyId_Forbidden()
        {
            //Given
            var client = await GetAuthenticatedClient();

            //When
            var response = await client.DeleteAsync(RequestUris.CampaignsV2Delete(SharedConstants.FakeCompanyId, _campaignResponse1.Id));

            //Then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status Code doesn't match.");
        }
    }
}
