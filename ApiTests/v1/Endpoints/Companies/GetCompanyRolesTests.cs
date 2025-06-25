using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AtCommon.Api;
using AtCommon.Dtos.Users;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiTests.v1.Endpoints.Companies
{
    [TestClass]
    [TestCategory("Companies")]
    public class GetCompanyRolesTests : BaseV1Test
    {
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Companies_Get_UserRoles_Success()
        {
            //arrange
            var client = await GetAuthenticatedClient();

            var rolesQuery = new Dictionary<string, object>
            {
                { "companyId", Company.Id },
                { "roleType", SharedConstants.RoleIndividual }
            };
            //act
            var response = await client.GetAsync<UserRolesResponse>(RequestUris.CompaniesUsersRoles().AddQueryParameter(rolesQuery));

            //assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status codes are not the same");
            Assert.IsTrue(response.Dto.Roles.All(i => i.Id > 0), "Role id must be greater than 0");
            Assert.IsTrue(response.Dto.Roles.All(i => !string.IsNullOrEmpty(i.Name)), "Name of role cannot be null or empty");
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Companies_Get_UserRoles_Unauthenticated_Unauthorized()
        {
            //arrange
            var client = GetUnauthenticatedClient();

            var rolesQuery = new Dictionary<string, object>
            {
                { "companyId", Company.Id },
                { "roleType", SharedConstants.RoleIndividual }
            };
            //act
            var response = await client.GetAsync(RequestUris.CompaniesUsersRoles().AddQueryParameter(rolesQuery));

            //assert
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status codes are not the same");
        }
        
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Companies_Get_UserRoles_InvalidRoleType_NotFound()
        {
            //arrange
            var client = await GetAuthenticatedClient();

            var rolesQuery = new Dictionary<string, object>
            {
                { "companyId", Company.Id },
                { "roleType", "0" }
            };
            //act
            var response = await client.GetAsync(RequestUris.CompaniesUsersRoles().AddQueryParameter(rolesQuery));

            //assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode, "Status codes are not the same");
        }
        
        [TestMethod]
        [TestCategory("SiteAdmin")]
        public async Task Companies_Get_UserRoles_InvalidCompany_NotFound()
        {
            //arrange
            var client = await GetAuthenticatedClient();

            var rolesQuery = new Dictionary<string, object>
            {
                { "companyId", -1 },
                { "roleType", "Individual" }
            };
            //act
            var response = await client.GetAsync(RequestUris.CompaniesUsersRoles().AddQueryParameter(rolesQuery));

            //assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode, "Status codes are not the same");
        }
        
        [TestMethod]
        [TestCategory("PartnerAdmin")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Companies_Get_UserRoles_InvalidCompany_Forbidden()
        
        {
            //arrange
            var client = await GetAuthenticatedClient();

            var rolesQuery = new Dictionary<string, object>
            {
                { "companyId", -1 },
                { "roleType", "Individual" }
            };
            //act
            var response = await client.GetAsync(RequestUris.CompaniesUsersRoles().AddQueryParameter(rolesQuery));

            //assert
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status codes are not the same");
        }
    }
}