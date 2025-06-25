using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AtCommon.Api;
using AtCommon.Dtos.Bulk;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiTests.v1.Endpoints.Bulk
{
    [TestClass]
    [TestCategory("Bulk"), TestCategory("Public")]
    public class PutBulkUsersTests : BaseV1Test
    {
        private static readonly List<string> ExpectedErrorList = new List<string>
        {
            "'First Name' must not be empty.",
            "'Last Name' must not be empty.",
            "'Email' must not be empty.",
            "Invalid role"
        };

        // 200-201
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Bulk_Users_Put_OK()
        {
            // given
            var client = await GetAuthenticatedClient();
            var userRequest = new List<AddUser> {UserFactory.GetBlAdminUserForBulkImport()};
            
            // when
            var response = await client.PutAsync(RequestUris.BulkUsers(Company.Id), userRequest.ToStringContent());

            // then
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode, 
                "Status code doesn't match.");
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Bulk_Users_Put_EmptyValues_BadRequest()
        {
            // given
            var client = await GetAuthenticatedClient();
            var userRequest = new List<AddUser>
            {
                new AddUser
                {
                    FirstName = "",
                    LastName = "",
                    Email = "",
                    Role = "Super Guy"
                }
            };
            
            // when
            var response = await client.PutAsync<List<string>>(RequestUris.BulkUsers(Company.Id), userRequest);

            // then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, 
                "Status code doesn't match.");
            Assert.That.ListsAreEqual(ExpectedErrorList, response.Dto);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Bulk_Users_Put_NullValues_BadRequest()
        {
            // given
            var client = await GetAuthenticatedClient();
            var userRequest = new List<AddUser> { new AddUser() };
            
            // when
            var response = await client.PutAsync<List<string>>(RequestUris.BulkUsers(Company.Id), userRequest);

            // then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, 
                "Status code doesn't match.");
            Assert.That.ListsAreEqual(ExpectedErrorList, response.Dto);
        }

        // 401
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Bulk_Users_Put_Unauthorized()
        {
            // given
            var client = GetUnauthenticatedClient();
            var userRequest = new List<AddUser> {UserFactory.GetBlAdminUserForBulkImport()};
            
            // when
            var response = await client.PutAsync(RequestUris.BulkUsers(Company.Id), userRequest.ToStringContent());

            // then
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, 
                "Status code doesn't match.");
        }

        // 403
        [TestMethod]
        [TestCategory("BLAdmin"), TestCategory("TeamAdmin"), TestCategory("OrgLeader")]
        [TestCategory("Member")]
        public async Task Bulk_Users_Put_UserRole_Forbidden()
        {
            // given
            var client = await GetAuthenticatedClient();
            var userRequest = new List<AddUser> {UserFactory.GetBlAdminUserForBulkImport()};
            
            // when
            var response = await client.PutAsync(RequestUris.BulkUsers(Company.Id), 
                userRequest.ToStringContent());

            // then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, 
                "Status code doesn't match.");
        }

        [TestMethod]
        [TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Bulk_Users_Put_Permission_Forbidden()
        {
            // given
            var client = await GetAuthenticatedClient();
            var userRequest = new List<AddUser> {UserFactory.GetBlAdminUserForBulkImport()};
            
            // when
            var response = await client.PutAsync(RequestUris.BulkUsers(2), 
                userRequest.ToStringContent());

            // then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, 
                "Status code doesn't match.");
        }

        // 404
        [TestMethod]
        [TestCategory("SiteAdmin")]
        public async Task Bulk_Users_Put_NotFound()
        {
            // given
            var client = await GetAuthenticatedClient();
            var userRequest = new List<AddUser> {UserFactory.GetBlAdminUserForBulkImport()};
            
            // when
            var response = await client.PutAsync(RequestUris.BulkUsers(SharedConstants.FakeCompanyId), 
                userRequest.ToStringContent());

            // then
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode, 
                "Status code doesn't match.");
        }
    }
}