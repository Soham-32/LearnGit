using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AtCommon.Api;
using AtCommon.Dtos.Teams;

namespace ApiTests.v1.Endpoints.Teams
{
    [TestClass]
    [TestCategory("Teams"), TestCategory("Public")]
    public class GetTeamMembersTests : BaseV1Test
    {

        // 200
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("Member"), TestCategory("OrgLeader")]
        public async Task TeamMembers_Get_Success()
        {
            // arrange
            var client = await GetAuthenticatedClient();

            var teamsResponse = await client.GetAsync<IList<TeamProfileResponse>>(RequestUris.Teams());
            teamsResponse.EnsureSuccess();
            var team = teamsResponse.Dto.First(t => t.Type == "Team");

            // act
            var response = await client.GetAsync<IList<TeamMemberResponse>>(RequestUris.TeamMembers(team.Uid));

            // assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Response Status Code does not match.");
            Assert.IsTrue(response.Dto.Any(dto => !string.IsNullOrWhiteSpace(dto.FirstName)), "FirstName is null or empty");
            Assert.IsTrue(response.Dto.Any(dto => !string.IsNullOrWhiteSpace(dto.LastName)), "LastName is null or empty");
            Assert.IsTrue(response.Dto.Any(dto => !string.IsNullOrWhiteSpace(dto.Email)), "Email is null or empty");
        }

        // 200 > Bug : Deleted team members should not be in response
        [TestMethod]
        [TestCategory("KnownDefect")] // Bug Id : 38946
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("Member")]
        public async Task TeamMembers_Get_NotIncludeDeletedTeams_Success()
        {
            // arrange
            var client = await GetAuthenticatedClient();

            var teamsResponse = await client.GetAsync<IList<TeamProfileResponse>>(RequestUris.Teams());
            teamsResponse.EnsureSuccess();
            var team = teamsResponse.Dto.First(t => t.Type == "Team");

            // act
            var response = await client.GetAsync<IList<TeamMemberResponse>>(RequestUris.TeamMembers(team.Uid).AddQueryParameter("includeDeleted", "false"));
            // assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Response Status Code does not match.");
            Assert.IsTrue(response.Dto.Any(dto => !string.IsNullOrWhiteSpace(dto.FirstName)), "FirstName is null or empty");
            Assert.IsTrue(response.Dto.Any(dto => !string.IsNullOrWhiteSpace(dto.LastName)), "LastName is null or empty");
            Assert.IsTrue(response.Dto.Any(dto => !string.IsNullOrWhiteSpace(dto.Email)), "Email is null or empty");
            Assert.IsTrue(response.Dto.All(dto => dto.DeletedAt == null), "DeletedAt isn't null or empty");
        }

        // 401
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member")]
        public async Task TeamMembers_Get_MissingToken_Unauthorized()
        {
            // arrange
            var client = GetUnauthenticatedClient();
            var invalidUid = Guid.NewGuid();

            // act
            var response = await client.GetAsync(RequestUris.TeamMembers(invalidUid));

            // assert
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Response Status Code does not match.");
        }

        // 403
        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task TeamMembers_Get_InvalidUid_Forbidden()
        {
            // arrange
            var client = await GetAuthenticatedClient();

            var invalidUid = Guid.NewGuid();

            // act
            var response = await client.GetAsync(RequestUris.TeamMembers(invalidUid));

            // assert
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Response Status Code does not match.");
        }

        // 404
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public async Task TeamMembers_Get_InvalidTeamUid_NotFound()
        {
            // arrange
            var client = await GetAuthenticatedClient();
            var invalidTeamUid = Guid.NewGuid();
            // act
            var response = await client.GetAsync(RequestUris.TeamMembers(invalidTeamUid));

            // assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode, "Response Status Code does not match.");
        }
    }
}