using System.Net;
using System.Threading.Tasks;
using AtCommon.Api;
using AtCommon.Dtos.Companies;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiTests.v1.Endpoints.Companies
{
    [TestClass]
    [TestCategory("Companies")]
    public class GetCompaniesCurrentUsageTests : BaseV1Test
    {
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public async Task Companies_CurrentUsage_Get_OK()
        {
            var client = await GetAuthenticatedClient();

            var response = await client.GetAsync<CurrentUsageResponse>(
                RequestUris.CompaniesCurrentUsage(Company.Id));

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status Code does not match");
            Assert.AreEqual(Company.Id, response.Dto.Id, "Id doesn't match");
            Assert.AreEqual(User.CompanyName, response.Dto.CompanyName, "CompanyName does not match");
        }

        // 401
        [TestMethod]
        [TestCategory("Rocket"), TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public async Task Companies_CurrentUsage_Get_Unauthorized()
        {
            // arrange
            var client = GetUnauthenticatedClient();
            
            // act
            var response = await client.GetAsync(RequestUris.CompaniesCurrentUsage(Company.Id));

            // assert
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status code does not match");
        }

        // 403
        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("Member"), TestCategory("OrgLeader"), TestCategory("TeamAdmin")]
        public async Task Companies_CurrentUsage_Get_Forbidden()
        {
            // arrange
            var client = await GetAuthenticatedClient();
            
            // act
            var response = await client.GetAsync(RequestUris.CompaniesCurrentUsage(Company.Id));

            // assert
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status code does not match");
        }

        [TestMethod]
        [TestCategory("PartnerAdmin")]
        public async Task Companies_CurrentUsage_Get_Forbidden_PA_Invalid_Company()
        {
            // arrange
            var client = await GetAuthenticatedClient();

            // act
            var response = await client.GetAsync(RequestUris.CompaniesCurrentUsage(20000));

            // assert
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status code does not match");
        }

        // 404
        [TestMethod]
        [TestCategory("SiteAdmin")]
        public async Task Companies_CurrentUsage_Get_NotFound()
        {
            // arrange
            var client = await GetAuthenticatedClient();
            
            // act
            var response = await client.GetAsync(RequestUris.CompaniesCurrentUsage(999999999));

            // assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode, "Status code does not match");
        }
    }
}
