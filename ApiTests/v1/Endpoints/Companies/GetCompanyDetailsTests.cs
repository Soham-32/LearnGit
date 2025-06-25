using AtCommon.Api;
using AtCommon.Dtos.Companies;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ApiTests.v1.Endpoints.Companies
{
    [TestClass]
    [TestCategory("Companies")]
    public class GetCompanyDetailsTests : BaseV1Test
    {
        // 200
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public async Task Companies_Details_Get_Success()
        {
            var client = await GetAuthenticatedClient();

            //act
            var companyResponse = await client.GetAsync<IList<BaseCompanyResponse>>(RequestUris.CompaniesListCompanies());
            companyResponse.EnsureSuccess();
            var companyId = companyResponse.Dto.First().Id;
            var companyName = companyResponse.Dto.First().Name;

            var response = await client.GetAsync<CompanyResponse>(RequestUris.CompanyDetails(companyId));

            //assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Response Status Code does not match.");
            Assert.AreEqual(companyId, response.Dto.Id, "Id does not match");
            Assert.AreEqual(companyName, response.Dto.Name, "Company Name does not match");
        }

        // 401
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public async Task Companies_Details_Get_Unauthorized()
        {
            var client = GetUnauthenticatedClient();

            //act
            var companyResponse = await client.GetAsync(RequestUris.CompanyDetails(2));

            //assert
            Assert.AreEqual(HttpStatusCode.Unauthorized, companyResponse.StatusCode, "Response Status Code does not match.");
        }

        // 403
        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("Member"), TestCategory("OrgLeader"),TestCategory("TeamAdmin")]
        public async Task Companies_Details_Get_Forbidden()
        {
            var client = await GetAuthenticatedClient();

            //act
            var companyResponse = await client.GetAsync<IList<BaseCompanyResponse>>(RequestUris.CompaniesListCompanies());
            companyResponse.EnsureSuccess();
            var companyId = companyResponse.Dto.First().Id;

            var response = await client.GetAsync(RequestUris.CompanyDetails(companyId));

            //assert
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Response Status Code does not match.");

        }

        // 404
        [TestMethod]
        [TestCategory("SiteAdmin")]
        public async Task Companies_Details_Get_NotFound()
        {
            // arrange
            var client = await GetAuthenticatedClient();

            //act
            var response = await client.GetAsync(RequestUris.CompanyDetails(10000));

            //assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode, "Response Status Code does not match.");
        }
    }
}
