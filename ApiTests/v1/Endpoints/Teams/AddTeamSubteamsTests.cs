using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AtCommon.Api;
using AtCommon.Dtos.Teams;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiTests.v1.Endpoints.Teams
{
    [TestClass]
    [TestCategory("Teams"), TestCategory("Public")]
    public class AddTeamSubteamsTests : TeamSubteamsBase
    {
        
        // 200-201
        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("TeamAdmin")]
        public async Task Teams_Subteams_Patch_Created()
        {
            // given
            var client = await GetAuthenticatedClient();
            // create multiteam
            var multiTeamResponse = await CreateMultiteam(client);
            // Get team uids
            var teamUids = await GetTeamUidsForSubteams(2, client);
            
            // when
            var response = await client.PatchAsync(
                RequestUris.TeamSubteams(multiTeamResponse.Uid), teamUids);

            // then
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode, "Status code doesn't match.");
            var updatedResponse =
                await client.GetAsync<TeamProfileResponse>(RequestUris.TeamDetails(multiTeamResponse.Uid));
            updatedResponse.EnsureSuccess();
            Assert.That.ListsAreEqual(teamUids, updatedResponse.Dto.Subteams.Select(s => s.ToString("D")).ToList());
        }

        // 400
        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("TeamAdmin")]
        public async Task Teams_Subteams_Patch_BadRequest()
        {
            // given
            var client = await GetAuthenticatedClient();
            var multiTeamResponse = await CreateMultiteam(client);
            // when
            var response = await client.PatchAsync(
                RequestUris.TeamSubteams(multiTeamResponse.Uid), "new List<string>()");

            // then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status code doesn't match.");
        }

        // 401
        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("TeamAdmin")]
        public async Task Teams_Subteams_Patch_Unauthorized()
        {
            // given
            var client = GetUnauthenticatedClient();

            // when
            var response = await client.PatchAsync(
                RequestUris.TeamSubteams(Guid.NewGuid()), new List<string>());

            // then
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status code doesn't match.");
        }

        // 403
        [TestMethod]
        [TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Teams_Subteams_Patch_UserRole_Forbidden()
        {
            // given
            var multiTeamResponse = await CreateMultiteam();
            var client = await GetAuthenticatedClient();

            // when
            var response = await client.PatchAsync(
                RequestUris.TeamSubteams(multiTeamResponse.Uid), new List<string>());

            // then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status code doesn't match.");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("TeamAdmin")]
        public async Task Teams_Subteams_Patch_TeamId_Forbidden()
        {
            // given
            var client = await GetAuthenticatedClient();

            // when
            var response = await client.PatchAsync(
                RequestUris.TeamSubteams(Guid.NewGuid()), new List<string>());

            // then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status code doesn't match.");
        }

        // 404
        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("TeamAdmin")]
        public async Task Teams_Subteams_Patch_NotFound()
        {
            // given
            var client = await GetAuthenticatedClient();
            var multiTeamResponse = await CreateMultiteam(client);

            // when
            var response = await client.PatchAsync(
                RequestUris.TeamSubteams(multiTeamResponse.Uid), new List<string> { Guid.NewGuid().ToString("D")});

            // then
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode, "Status code doesn't match.");
        }
    }
}