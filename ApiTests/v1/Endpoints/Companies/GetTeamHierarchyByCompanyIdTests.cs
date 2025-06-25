using System.Net;
using System.Threading.Tasks;
using AtCommon.Api;
using AtCommon.Dtos.Companies;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiTests.v1.Endpoints.Companies
{
    [TestClass]
    [TestCategory("Companies")]
    public class GetTeamHierarchyByCompanyIdTests : BaseV1Test
    {
        // 200
        [TestMethod]
        [TestCategory("BLAdmin"), TestCategory("SiteAdmin"), TestCategory("CompanyAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Companies_GetTeamHierarchyByCompanyId_Get_Success()
        {
            // given
            var client = await GetAuthenticatedClient();

            var companyId = Company.Id;

            // when
            var response =
                await client.GetAsync<CompanyHierarchyResponse>(
                    RequestUris.CompaniesGetTeamHierarchyByCompanyId(companyId));

            // then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status code doesn't match.");
            Assert.AreEqual(companyId, response.Dto.CompanyId, "CompanyId does not match.");
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.Dto.Name), "Name is null or empty.");
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.Dto.Type), "Type is null or empty.");
            Assert.IsTrue(response.Dto.Children.Count > 0, "Children is null or empty.");
        }

        // 401
        [TestMethod]
        [TestCategory("BLAdmin"), TestCategory("SiteAdmin"), TestCategory("CompanyAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Companies_GetTeamHierarchyByCompanyId_Get_Unauthorized()
        {
            // given
            var client = GetUnauthenticatedClient();
            const int companyId = 2;

            // when
            var response =await client.GetAsync(
                    RequestUris.CompaniesGetTeamHierarchyByCompanyId(companyId));

            // then
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status code doesn't match.");
        }

        // 403
        [TestMethod]
        [TestCategory("BLAdmin"), TestCategory("CompanyAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Companies_GetTeamHierarchyByCompanyId_Get_Forbidden()
        {
            // given
            var client = await GetAuthenticatedClient();
            const int companyId = 2;

            // when
            var response =await client.GetAsync(
                RequestUris.CompaniesGetTeamHierarchyByCompanyId(companyId));

            // then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status code doesn't match.");
        }
    }
}
