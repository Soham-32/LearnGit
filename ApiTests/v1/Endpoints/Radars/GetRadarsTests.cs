using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AtCommon.Api;
using AtCommon.Dtos.Radars;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiTests.v1.Endpoints.Radars
{
    [TestClass]
    [TestCategory("Radars")]
    public class GetRadarsTests : BaseV1Test
    {
        // 200
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Radars_Get_Success()
        {
            // Given
            var client = await GetAuthenticatedClient();

            // When
            var response = await client.GetAsync<IList<RadarResponse>>(RequestUris.Radars());

            // Then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status Code does not match.");
            Assert.IsTrue(response.Dto.All(radar => radar.Id > 0), "At least one of the radars has an invalid Id.");
            Assert.IsTrue(response.Dto.All(radar => !string.IsNullOrEmpty(radar.Name)), "At least one of the radars has a null or empty Name.");
            Assert.IsTrue(response.Dto.All(radar => radar.OriginalVersion > 0), "At least one of the radars has an invalid OriginalVersion.");
            Assert.IsTrue(response.Dto.All(radar => !string.IsNullOrEmpty(radar.SurveyType)), "At least one of the radars has a null or empty SurveyType.");
        }

        // 401
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public async Task Radars_Get_Unauthorized()
        {
            // Given
            var client = GetUnauthenticatedClient();

            // When
            var response = await client.GetAsync(RequestUris.Radars());

            // Then
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status Code does not match.");
            
        }

    }
}
