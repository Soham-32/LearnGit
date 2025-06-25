using System;
using System.Net;
using System.Threading.Tasks;
using AtCommon.Api;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiTests.Analytics
{
    //[TestClass]
    [TestCategory("Insights")]
    public class GetAnalyticsSyncDateTimeTests : v1.Endpoints.BaseV1Test
    {
        [TestMethod]
        [TestCategory("Avengers")]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_SyncDateTime_Get_Success()
        {
            // given
            var client = await GetInsightsAuthenticatedClient();

            // when
            var response = await client.GetAsync<string>(RequestUris.AnalyticsSyncDateTime(Company.InsightsId));

            // then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status Code does not match.");
            Assert.IsTrue(DateTime.TryParse(response.Dto, out _), "DateTime is invalid.");
        }

        [TestMethod]
        [TestCategory("Avengers")]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_SyncDateTime_Get_Unauthorized()
        {
            //given
            var client = GetUnauthenticatedClient();

            //when
            var response = await client.GetAsync(RequestUris.AnalyticsSyncDateTime(Company.InsightsId));

            //then
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status Code does not match.");

        }
    }
}
