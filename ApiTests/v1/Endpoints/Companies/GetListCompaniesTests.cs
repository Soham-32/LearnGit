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
    public class GetListCompaniesTests : BaseV1Test
    {
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Companies_ListCompanies_Get_Success()
        {
            //arrange
            var client = await GetAuthenticatedClient();

            //act
            var companyResponse = await client.GetAsync<IList<BaseCompanyResponse>>(RequestUris.CompaniesListCompanies());

            //assert
            Assert.AreEqual(HttpStatusCode.OK, companyResponse.StatusCode, "Response Status Code does not match.");
            Assert.IsTrue(companyResponse.Dto.All(dto => !string.IsNullOrWhiteSpace(dto.Name)), "Name is null or empty");
            Assert.IsTrue(companyResponse.Dto.All(dto => dto.Id >= 0), "Id is not greater than or equal to 0");
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Companies_ListCompanies_Get_Unauthorized()
        {
            var client = GetUnauthenticatedClient();

            //act
            var companyResponse = await client.GetAsync(RequestUris.CompaniesListCompanies());

            //assert
            Assert.AreEqual(HttpStatusCode.Unauthorized, companyResponse.StatusCode, "Response Status Code does not match.");
        }
    }
}
