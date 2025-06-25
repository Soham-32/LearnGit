using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Linq;
using AtCommon.Api;
using AtCommon.Dtos.Companies;

namespace ApiTests.v1.Endpoints.Companies
{
    [TestClass]
    [TestCategory("Companies")]
    public class GetCompaniesTests : BaseV1Test
    {
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public async Task Companies_Get_Success()
        {
            //arrange
            var client = await GetAuthenticatedClient();

            //act
            var companyResponse = await client.GetAsync<IList<CompanyResponse>>(RequestUris.Companies());

            //assert
            Assert.AreEqual(HttpStatusCode.OK, companyResponse.StatusCode, "Response Status Code does not match.");
            Assert.IsTrue(companyResponse.Dto.All(dto => !string.IsNullOrWhiteSpace(dto.Name)), "Name is null or empty");
            Assert.IsTrue(companyResponse.Dto.All(dto => dto.Id >= 0), "Id is not greater than or equal to 0");
            Assert.IsTrue(companyResponse.Dto.All(dto => !string.IsNullOrWhiteSpace(dto.Industry)), 
                "Industry is null or empty");
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public async Task Companies_Get_Unauthorized()
        {
            var client = GetUnauthenticatedClient();

            //act
            var companyResponse = await client.GetAsync(RequestUris.Companies());

            //assert
            Assert.AreEqual(HttpStatusCode.Unauthorized, companyResponse.StatusCode, "Response Status Code does not match.");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("Member"), TestCategory("OrgLeader"),TestCategory("TeamAdmin")]
        public async Task Companies_Get_Forbidden()
        {
            //arrange
            var client = await GetAuthenticatedClient();

            //act
            var companyResponse = await client.GetAsync(RequestUris.Companies());

            //assert
            Assert.AreEqual(HttpStatusCode.Forbidden, companyResponse.StatusCode);
        }
    }
}
