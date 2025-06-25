using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AtCommon.Api;
using AtCommon.Dtos.Companies;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiTests.v1.Endpoints.Companies
{
    [TestClass]
    [TestCategory("Companies")]
    public class GetCompaniesIndustries : BaseV1Test
    {
        // 200
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("Member"), TestCategory("OrgLeader"), TestCategory("TeamAdmin")]
        public async Task Companies_Industries_Get_Success()
        {
            // Given
            var client = await GetAuthenticatedClient();

            // When
            var response = await client.GetAsync<IList<IndustryResponse>>(RequestUris.CompaniesIndustries());

            // Then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status Code does not match.");
            Assert.IsTrue(response.Dto.All(dto => !string.IsNullOrWhiteSpace(dto.Name)), "Name is null or empty");

        }

        // 401
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public async Task Companies_Industries_Get_Unauthorized()
        {
            // Given
            var client = GetUnauthenticatedClient();

            // When
            var response = await client.GetAsync(RequestUris.CompaniesIndustries());

            // Then
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status Code does not match.");

        }

    }
}
