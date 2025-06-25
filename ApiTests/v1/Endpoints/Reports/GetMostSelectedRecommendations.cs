using AtCommon.Api;
using AtCommon.Dtos.Reports;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ApiTests.v1.Endpoints.Reports
{
    [TestClass]
    [TestCategory("Reports")]
    public class GetMostSelectedRecommendations : BaseV1Test
    {
        [TestMethod]
        [TestCategory("KnownDefect")]
        [TestCategory("CompanyAdmin")]
        public async Task Reports_Get_MostSelectedRecommendations_Success()
        {
            var client = await GetAuthenticatedClient();

            var response = await client.GetAsync<IList<MostSelectedRecommendationResponse>>(RequestUris.ReportsMostSelectedRecommendations());

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status codes do not match");
            Assert.IsTrue(response.Dto.All(dto => dto.Competency != null), "Compentency field is empty");
            Assert.IsTrue(response.Dto.All(dto => dto.CompanyId > 0), "CompanyId field is invalid");
            Assert.IsTrue(response.Dto.All(dto => dto.RecommendationCount > 0), "Recommendation count field is invalid");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Reports_Get_MostSelectedRecommendations_MissingAuthorizationToken()
        {
            var client = GetUnauthenticatedClient();

            var response = await client.GetAsync<IList<MostSelectedRecommendationResponse>>(RequestUris.ReportsMostSelectedRecommendations());

            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status codes do not match");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Reports_Get_MostSelectedRecommendations_UnauthorizedUser()
        {
            var client = await GetAuthenticatedClient();

            var response = await client.GetAsync<IList<MostSelectedRecommendationResponse>>(RequestUris.ReportsMostSelectedRecommendations());

            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status codes do not match");
        }
    }
}
