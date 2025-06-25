using AtCommon.Api;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ApiTests.v1.Endpoints.Teams
{
    [TestClass]
    [TestCategory("Teams")]
    public class UpdateTeamTests : BaseV1Test
    {
        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("PartnerAdmin")]
        public async Task Team_Put_Success()
        {
            // arrange
            var client = await GetAuthenticatedClient();

            // add a new team
            var team = TeamFactory.GetValidPostTeam("GetValidPostTeam_");

            var teamResponse = await client.PostAsync<TeamResponse>(RequestUris.Teams(), team);
            teamResponse.EnsureSuccess();
            var teamUid = teamResponse.Dto.Uid;

            var updatedTeam = TeamFactory.GetValidPutTeam("GetValidPostTeam_");

            // act
            var updatedTeamResponse = await client.PutAsync<TeamProfileResponse>(RequestUris.TeamUpdate(teamUid), updatedTeam);

            // assert
            Assert.AreEqual(HttpStatusCode.OK, updatedTeamResponse.StatusCode, "Response Status Code doesn't match");
            Assert.AreNotEqual(team.Name, updatedTeam.Name, "Name change not reflected");
        }

        [TestMethod]
        [TestCategory("Member")]
        public async Task Team_Put_NoPermission_Forbidden()
        {
            // arrange
            var client = await GetAuthenticatedClient();
            var teamsResponse = await client.GetAsync<IList<TeamProfileResponse>>(RequestUris.Teams());
            teamsResponse.EnsureSuccess();
            const string teamName = "Automation Multi Team for Update";
            var firstTeam = teamsResponse.Dto.FirstOrDefault(t => t.Name == teamName);
            if (firstTeam == null) throw new Exception($"<{teamName}> was not found in the response.");
            var updatedTeam = TeamFactory.GetValidPutTeam("GetValidPostTeam_");
            // act
            var updatedTeamResponse = await client.PutAsync<IList<string>>(
                RequestUris.TeamUpdate(firstTeam.Uid), updatedTeam);

            // assert
            Assert.AreEqual(HttpStatusCode.Forbidden, updatedTeamResponse.StatusCode,
                "Response Status Code doesn't match");
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public async Task Team_Put_NotFound()
        {
            // arrange
            var client = await GetAuthenticatedClient();

            // get a new team request
            var teamRequest = TeamFactory.GetValidPutTeam("GetValidPostTeam_");

            // act
            var response = await client.PutAsync<IList<string>>(
                RequestUris.TeamUpdate(Guid.NewGuid()), teamRequest);

            // assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode,
                "Response Status Code doesn't match");
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("Member"), TestCategory("OrgLeader"), TestCategory("PartnerAdmin")]
        public async Task Team_Put_MissingToken_Unauthorized()
        {
            // arrange
            var client = GetUnauthenticatedClient();

            // add a new team
            var team = TeamFactory.GetValidPostTeam("GetValidPostTeam_");

            var teamResponse = await client.PutAsync<TeamResponse>(
                RequestUris.TeamDetails(Guid.NewGuid()), team);

            // assert
            Assert.AreEqual(HttpStatusCode.Unauthorized, teamResponse.StatusCode, "Response Status Code doesn't match");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Team_Put_Forbidden()
        {
            // arrange
            var client = await GetAuthenticatedClient();

            var teamUid = Guid.NewGuid();

            var updatedTeam = TeamFactory.GetValidPutTeam("GetValidPostTeam_");

            // act
            var updatedTeamResponse = await client.PutAsync<IList<string>>(RequestUris.TeamUpdate(teamUid), updatedTeam);

            // assert
            Assert.AreEqual(HttpStatusCode.Forbidden, updatedTeamResponse.StatusCode, "Response Status Code doesn't match");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("PartnerAdmin")]
        public async Task Team_Put_BadRequest()
        {
            // arrange
            var client = await GetAuthenticatedClient();

            // add a new team
            var team = TeamFactory.GetValidPostTeam("GetValidPostTeam_");

            var teamResponse = await client.PostAsync<TeamResponse>(RequestUris.Teams(), team);
            teamResponse.EnsureSuccess();
            var teamUid = teamResponse.Dto.Uid;

            var updatedTeam = TeamFactory.GetInvalidPutTeam("GetInvalidPutTeam_");

            // act
            var updatedTeamResponse = await client.PutAsync<IList<string>>(RequestUris.TeamUpdate(teamUid), updatedTeam);

            // assert
            Assert.AreEqual(HttpStatusCode.BadRequest, updatedTeamResponse.StatusCode, "Response Status Code doesn't match");
            Assert.AreEqual("'Description' should not be empty", updatedTeamResponse.Dto[0]);
        }
    }
}
