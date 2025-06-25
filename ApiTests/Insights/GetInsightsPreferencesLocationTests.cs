using System.Net;
using System.Threading.Tasks;
using AtCommon.Api;
using AtCommon.Dtos.Insights;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiTests.Insights
{
    [TestClass]
    [TestCategory("Insights")]
    public class GetInsightsPreferencesLocationTests : v1.Endpoints.BaseV1Test
    {
        [TestMethod]
        [TestCategory("Avengers")]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Insights_PreferencesLocation_Get_Success()
        {
            //given
            var client = await GetInsightsAuthenticatedClient();

            //when
            var response = await client.GetAsync<UserPreferenceResponse>(RequestUris.InsightsPreferences("insightsdashboardfilters"));

            //then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status Code does not match");
            Assert.IsTrue(response.Dto.Location.Contains("insightsdashboardfilters"), "insightsdashboardfilters does not exist as the location.");
            Assert.IsTrue(response.Dto.Preference.Contains("selectedRadar"), "selectRadar does not exist in preferences.");
            Assert.IsTrue(response.Dto.Preference.Contains("selectedBenchmark"), "selectBenchmark does not exist in preferences.");
        }

        [TestMethod]
        [TestCategory("Avengers")]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Insights_PreferencesLocation_Get_Unauthorized()
        {
            //given
            var client = GetUnauthenticatedClient();

            //when
            var response = await client.GetAsync(RequestUris.InsightsPreferences("insightsdashboardfilters"));

            //then
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status Code does not match.");            
        }
    }
}
