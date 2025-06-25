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
    public class GetCompaniesPartnersTests : BaseV1Test
    {
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public async Task Companies_Partners_Get_OK()
        {
            var client = await GetAuthenticatedClient();

            var response = await client.GetAsync<List<CompanyListResponse>>(RequestUris.CompaniesPartners());

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status code does not match");
            Assert.IsTrue(response.Dto.All(p => p.Type == "Partner"), "Type does not equal <Partner>");
            Assert.IsTrue(response.Dto.All(p => !string.IsNullOrWhiteSpace(p.Name)), "Name is null or empty");

        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public async Task Companies_Partners_Get_Unauthorized()
        {
            var client = GetUnauthenticatedClient();

            var response = await client.GetAsync<List<CompanyListResponse>>(RequestUris.CompaniesPartners());

            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status code does not match");
            
        }

    }
}
