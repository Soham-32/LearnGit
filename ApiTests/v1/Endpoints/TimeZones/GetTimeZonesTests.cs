using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AtCommon.Api;
using AtCommon.Dtos.TimeZones;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiTests.v1.Endpoints.TimeZones
{
    [TestClass]
    [TestCategory("TimeZones")]
    public class GetTimeZonesTests : BaseV1Test
    {
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("TeamAdmin"), TestCategory("Member")]
        public async Task TimeZones_Get_Success()
        {
            // Given
            var client = await GetAuthenticatedClient();

            // When
            var response = await client.GetAsync<IList<TimeZoneResponse>>(RequestUris.TimeZones());

            //Then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Response status code does not match.");
            Assert.IsTrue(response.Dto.All(t => !string.IsNullOrEmpty(t.Id)), "At least one of the 'Id' fields is empty or null.");
            Assert.IsTrue(response.Dto.All(t => !string.IsNullOrEmpty(t.StandardName)), "At least one of the 'StandardName' fields is empty or null.");
            Assert.IsTrue(response.Dto.All(t => !string.IsNullOrEmpty(t.DisplayName)), "At least one of the 'DisplayName' fields is empty or null.");
            Assert.IsTrue(response.Dto.All(t => !string.IsNullOrEmpty(t.DaylightName)), "At least one of the 'DaylightName' fields is empty or null.");

        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public async Task TimeZones_Get_Unauthorized()
        {
            // Given
            var client = GetUnauthenticatedClient();

            // When
            var response = await client.GetAsync(RequestUris.TimeZones());

            // Then
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Response status code does not match.");
        }

    }
}
