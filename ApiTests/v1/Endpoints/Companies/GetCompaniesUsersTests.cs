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
    public class GetCompaniesUsersTests : BaseV1Test
    {
        private const string Query = "ah_automation";
        
        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("OrgLeader"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("Member"), TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public async Task Companies_Users_NoQuery_Success()
        {
            var client = await GetAuthenticatedClient();

            var response = await client.GetAsync<IList<UserRequest>>(RequestUris.CompaniesUsers(Company.Id));
            
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status codes do not match");
            Assert.IsTrue(response.Dto.All(dto => !string.IsNullOrEmpty(dto.FirstName)), "There is an empty or null first name data point");
            Assert.IsTrue(response.Dto.All(dto => !string.IsNullOrEmpty(dto.LastName)), "There is an empty or null last name data point");
            Assert.IsTrue(response.Dto.All(dto => !string.IsNullOrEmpty(dto.Email)), "There is an empty or null email data point");
        }
        
        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("OrgLeader"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("Member"), TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public async Task Companies_Users_Query_Success()
        {
            var client = await GetAuthenticatedClient();

            var response = await client.GetAsync<IList<UserRequest>>(RequestUris.CompaniesUsers(Company.Id).AddQueryParameter("query", Query));
            
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status codes do not match");
            Assert.IsTrue(response.Dto.All(dto => dto.Email.Contains(Query)), $"There are emails in the list that do not match the query of {Query}");
        }
        
        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("OrgLeader"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("Member"), TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public async Task Companies_Users_Unauthorized()
        {
            var client = GetUnauthenticatedClient();

            var response = await client.GetAsync<IList<UserRequest>>(RequestUris.CompaniesUsers(Company.Id));

            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status codes do not match");
        }
        
        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("OrgLeader"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Companies_Users_Forbidden()
        {
            var client = await GetAuthenticatedClient();

            var response = await client.GetAsync<IList<UserRequest>>(RequestUris.CompaniesUsers(-1).AddQueryParameter("query", Query));

            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status codes do not match");
        }
    }
}