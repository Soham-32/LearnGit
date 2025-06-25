using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AtCommon.Api;
using AtCommon.Dtos.Companies;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace ApiTests.v1.Endpoints.Bulk
{
    [TestClass]
    [TestCategory("Bulk"), TestCategory("Public")]
    public class GetBulkTests : BaseV1Test
    {
        // 200-201
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"),TestCategory("PartnerAdmin")]
        public async Task Bulk_Get_OK()
        {
            // given
            var client = await GetAuthenticatedClient();

            // when
            var response = await client.GetAsync<CompanyExportResponse>(
                RequestUris.Bulk(Company.Id));

            // then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode,
                "Status code doesn't match.");
            Assert.IsTrue(response.Dto.Teams.HasValues, "There are no Teams in the response.");
            Assert.IsTrue(response.Dto.Members.HasValues, "There are no Members in the response.");
            Assert.IsTrue(response.Dto.Users.HasValues, "There are no Users in the response.");

            // Verify team's data
            var jTokenListOfTeams = response.Dto.Teams.Children().ToList();
            foreach (var teamObject in jTokenListOfTeams.Select(team => JObject.Parse(team.ToString())))
            {
                Assert.IsTrue(teamObject["TeamUId"] != null, "TeamUid is null");
                Assert.IsTrue(teamObject["TeamId"] != null, "TeamId is null");
                Assert.IsTrue(teamObject["Name"] != null, "Team name is null");
            }

            // Verify member's data
            var jTokenListOfMembers = response.Dto.Members.Children().ToList();
            foreach (var memberObject in jTokenListOfMembers.Select(member => JObject.Parse(member.ToString())))
            {
                Assert.IsTrue(memberObject["TeamUId"] != null, "TeamUid is null");
                Assert.IsTrue(memberObject["Name"] != null, "Team Name is null");
                Assert.IsTrue(memberObject["Email"] != null, "Email of member is null");
            }

            // Verify user's data
            var jTokenListOfUsers = response.Dto.Users.Children().ToList();
            foreach (var memberObject in jTokenListOfUsers.Select(user => JObject.Parse(user.ToString())))
            {
                Assert.IsTrue(memberObject["FirstName"] != null, "FirstName of user is null");
                Assert.IsTrue(memberObject["LastName"] != null, "LastName of user is null");
                Assert.IsTrue(memberObject["Email"] != null, "Email of user is null");
            }
        }

        // 401
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        public async Task Bulk_Get_Unauthorized()
        {
            // given
            var client = GetUnauthenticatedClient();

            // when
            var response = await client.GetAsync<IList<string>>(RequestUris.Bulk(Company.Id));

            // then
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status code doesn't match.");
        }

        // 403
        [TestMethod]
        [TestCategory("BLAdmin"), TestCategory("TeamAdmin"), TestCategory("OrgLeader")]
        [TestCategory("Member")]
        public async Task Bulk_Get_UserRole_Forbidden()
        {
            // given
            var client = await GetAuthenticatedClient();

            // when
            var response = await client.GetAsync<IList<string>>(RequestUris.Bulk(Company.Id));

            // then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status code doesn't match.");
        }

        // 403
        [TestMethod]
        [TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Bulk_Get_Forbidden()
        {
            // given
            var client = await GetAuthenticatedClient();

            // when
            var response = await client.GetAsync(RequestUris.Bulk(2));

            // then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status code doesn't match.");
        }

        // 404
        [TestMethod]
        [TestCategory("SiteAdmin")]
        public async Task Bulk_Get_NotFound()
        {
            // given
            var client = await GetAuthenticatedClient();

            // when
            var response = await client.GetAsync<IList<string>>(RequestUris.Bulk(SharedConstants.FakeCompanyId));

            // then
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode, "Status code doesn't match.");
        }
    }
}