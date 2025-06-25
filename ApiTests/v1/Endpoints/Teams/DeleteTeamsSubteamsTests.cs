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
    public class DeleteTeamsSubteamsTests : TeamSubteamsBase
    {
        // 200-201
        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("TeamAdmin")]
        public async Task Teams_Subteams_Delete_Created()
        {
            // given
            var client = await GetAuthenticatedClient();
            var multiTeamResponse = await CreateMultiteam(client);
            var subteamUids = await GetTeamUidsForSubteams(2, client);
            var addSubteamResponse = await client.PatchAsync(
                RequestUris.TeamSubteams(multiTeamResponse.Uid), subteamUids);
            addSubteamResponse.EnsureSuccessStatusCode();

            // when
            var response = await client.DeleteAsync(
                RequestUris.TeamSubteams(multiTeamResponse.Uid),
                new List<string> { subteamUids.First() });

            // then
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode, "Status code doesn't match.");
            subteamUids.RemoveAt(0);
            var updatedResponse =
                await client.GetAsync<TeamProfileResponse>(RequestUris.TeamDetails(multiTeamResponse.Uid));
            updatedResponse.EnsureSuccess();
            Assert.That.ListsAreEqual(subteamUids,
                updatedResponse.Dto.Subteams.Select(t => t.ToString("D")).ToList());
        }

        // 401
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("TeamAdmin"), TestCategory("Member"), TestCategory("OrgLeader")]
        public async Task Teams_Subteams_Delete_Unauthorized()
        {
            // given
            var client = GetUnauthenticatedClient();

            // when
            var response = await client.DeleteAsync(
                RequestUris.TeamSubteams(Guid.NewGuid()),
                new List<string>());

            // then
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status code doesn't match.");
        }

        // 403
        [TestMethod]
        [TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Teams_Subteams_Delete_UserRole_Forbidden()
        {
            // given
            var multiTeamResponse = await CreateMultiteam();
            var client = await GetAuthenticatedClient();

            // when
            var response = await client.DeleteAsync(
                RequestUris.TeamSubteams(multiTeamResponse.Uid), new List<string>());

            // then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status code doesn't match.");
        }

        // 403
        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("TeamAdmin"), TestCategory("Member"), TestCategory("OrgLeader")]
        public async Task Teams_Subteams_Delete_TeamId_Forbidden()
        {
            // given
            var client = await GetAuthenticatedClient();

            // when
            var response = await client.DeleteAsync(
                RequestUris.TeamSubteams(Guid.NewGuid()), new List<string>());

            // then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status code doesn't match.");
        }

        // 404
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public async Task Teams_Subteams_Delete_InvalidTeamUid_NotFound()
        {
            // given
            var client = await GetAuthenticatedClient();
            var invalidTeamUid = Guid.NewGuid();

            // when
            var response = await client.DeleteAsync(
                RequestUris.TeamSubteams(invalidTeamUid), new List<string>());

            // then
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode, "Status code doesn't match.");
        }
    }
}