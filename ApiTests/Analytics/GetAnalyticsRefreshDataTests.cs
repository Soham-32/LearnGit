using System;
using System.Net;
using System.Threading.Tasks;
using ApiTests.v1.Endpoints;
using AtCommon.Api;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiTests.Analytics
{
    //[TestClass]
    [TestCategory("Insights")]
    public class GetAnalyticsRefreshDataTests : BaseV1Test
    {
        // 200-201
        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_RefreshData_Get_OK()
        {
            // given
            var client = await GetInsightsAuthenticatedClient();

            // when
            var response = await client.GetAsync<string>(RequestUris.AnalyticsRefreshData(Company.InsightsId));

            // then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status code doesn't match.");
            Assert.IsTrue(DateTime.TryParse(response.Dto, out var result), 
                $"Response <{response.Dto}> could not be parsed to a DateTime");
            Assert.That.TimeIsClose(DateTime.Now, result);
        }

        // 401
        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_RefreshData_Get_Unauthorized()
        {
            // given
            var client = GetUnauthenticatedClient();

            // when
            var response = await client.GetAsync<string>(RequestUris.AnalyticsRefreshData(Company.InsightsId));

            // then
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status code doesn't match.");
        }

        // 403
        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_RefreshData_Get_Forbidden()
        {
            // given
            var client = await GetInsightsAuthenticatedClient();

            // when
            var response = await client.GetAsync<string>(RequestUris.AnalyticsRefreshData(SharedConstants.FakeCompanyId));

            // then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status code doesn't match.");
        }

        // 404
        [TestMethod]
        [TestCategory("SiteAdmin")]
        public async Task Analytics_RefreshData_Get_NotFound()
        {
            // given
            var client = await GetInsightsAuthenticatedClient();

            // when
            var response = await client.GetAsync<string>(RequestUris.AnalyticsRefreshData(SharedConstants.FakeCompanyId));

            // then
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode, "Status code doesn't match.");
        }
    }
}