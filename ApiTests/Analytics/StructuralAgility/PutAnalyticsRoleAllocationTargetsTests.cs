using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using ApiTests.v1.Endpoints;
using AtCommon.Api;
using AtCommon.Dtos.Analytics.StructuralAgility;
using AtCommon.ObjectFactories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiTests.Analytics.StructuralAgility
{
    [TestClass]
    [TestCategory("Insights")]
    public class PutAnalyticsRoleAllocationTargetsTests : BaseV1Test
    {
        private const int TeamId = 0;
        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_RoleAllocationTargets_Put_Success()
        {
            // given
            var client = await GetInsightsAuthenticatedClient();
            var getResponse = await client.GetAsync<IList<RoleAllocationTargetsResponse>> (
                RequestUris.AnalyticsRoleAllocationTargets(Company.InsightsId, TeamId));
            // when
            var targetsRequest = InsightsFactory.GetRoleAllocationTargetsRequest(getResponse.Dto);

            var response =
                await client.PutAsync<int>(RequestUris.AnalyticsRoleAllocationTargets(Company.InsightsId), targetsRequest);
            // then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status Code does not match.");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_RoleAllocationTargets_Put_Unauthorized()
        {
            // given
            var client = GetUnauthenticatedClient();
            // when
            var targetsRequest = InsightsFactory.GetRoleAllocationTargetsRequest();
            var response =
                await client.PutAsync<string>(RequestUris.AnalyticsRoleAllocationTargets(Company.InsightsId), targetsRequest);
            // then
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status Code does not match.");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_RoleAllocationTargets_Put_Forbidden()
        {
            // given
            var client = await GetInsightsAuthenticatedClient();
            // when
            var targetsRequest = InsightsFactory.GetRoleAllocationTargetsRequest();
            var response =
                await client.PutAsync<string>(RequestUris.AnalyticsRoleAllocationTargets(100), targetsRequest);
            // then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status Code does not match.");
        }
    }
}
