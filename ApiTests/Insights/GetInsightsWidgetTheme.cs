using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ApiTests.v1.Endpoints;
using AtCommon.Api;
using AtCommon.Dtos.Insights;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiTests.Insights
{
    [TestClass]
    [TestCategory("Insights")]
    public class GetInsightsWidgetTheme : BaseV1Test
    {
        [TestMethod]
        [TestCategory("Avengers")]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Insights_WidgetTheme_Get_Success()
        {
            //given
            var client = await GetInsightsAuthenticatedClient();

            //when
            var response = await client.GetAsync<WidgetThemeResponse>(RequestUris.InsightsWidgetTheme(1));

            //then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status Code does not match.");
            Assert.IsTrue(response.Dto.Id.ToString().Any(char.IsDigit), "Id is not an int");
            Assert.IsFalse(String.IsNullOrEmpty(response.Dto.Name), "Name is null or empty");
            Assert.IsTrue(response.Dto.ThemeJson.Contains("color1"), "Color1 does not exist in the ThemeJson field.");
            Assert.IsTrue(response.Dto.ThemeJson.Contains("color2"), "Color1 does not exist in the ThemeJson field.");
            Assert.IsTrue(response.Dto.ThemeJson.Contains("color4"), "Color1 does not exist in the ThemeJson field.");
            Assert.IsTrue(response.Dto.ThemeJson.Contains("color5"), "Color1 does not exist in the ThemeJson field.");
            Assert.IsTrue(response.Dto.ThemeJson.Contains("color3"), "Color1 does not exist in the ThemeJson field.");
            Assert.IsTrue(response.Dto.ThemeJson.Contains("color6"), "Color1 does not exist in the ThemeJson field.");
        }

        [TestMethod]
        [TestCategory("Avengers")]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Insights_WidgetTheme_Get_Unauthorized()
        {
            //given
            var client = GetUnauthenticatedClient();

            //when
            var response = await client.GetAsync<WidgetThemeResponse>(RequestUris.InsightsWidgetTheme(1));

            //then
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status Code does not match.");
        }
    }
}
