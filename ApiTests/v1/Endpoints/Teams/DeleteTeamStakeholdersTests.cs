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
    [TestCategory("Teams"), TestCategory("Public")]
    public class DeleteTeamStakeholdersTests : BaseV1Test
    {
        // 200
        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public async Task TeamStakeholders_Delete_Success()
        {
            //arrange
            var client = await GetAuthenticatedClient();

            //create team
            var team = TeamFactory.GetValidPostTeam("GetValidPostTeam_");
            var teamResponse = await client.PostAsync<TeamResponse>(RequestUris.Teams(), team);
            teamResponse.EnsureSuccess();

            var teamUId = teamResponse.Dto.Uid;

            //add stakeholder
            var stakeholder = MemberFactory.GetValidPostTeamStakeholder();
            var stakeholderResponse = await client.PostAsync<TeamMemberResponse>(RequestUris.TeamStakeholder(teamUId), stakeholder);
            stakeholderResponse.EnsureSuccess();

            var stakeholderUId = stakeholderResponse.Dto.Uid;

            //delete stakeholder
            var response = await client.DeleteAsync<string>(RequestUris.DeleteTeamStakeholder(teamUId, stakeholderUId));

            //get all stakeholders in team
            var listOfStakeholders = await client.GetAsync<IList<TeamMemberResponse>>(RequestUris.TeamStakeholder(teamUId));

            //assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status code does not match");
            Assert.IsFalse(listOfStakeholders.Dto.Any(dto => dto.Uid == stakeholderUId), $"Stakeholder {stakeholderUId} was not deleted");
        }

        // 200
        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public async Task TeamStakeholders_TeamWithMembers_Delete_Success()
        {
            //arrange
            var client = await GetAuthenticatedClient();

            //create team
            var team = TeamFactory.GetValidPostTeamWithMember("GetValidPostTeamWithMember_");
            var teamResponse = await client.PostAsync<TeamResponse>(RequestUris.Teams(), team);
            teamResponse.EnsureSuccess();

            var teamUId = teamResponse.Dto.Uid;
            var stakeholderUId = teamResponse.Dto.Stakeholders.First().Uid;

            //delete stakeholder
            var response = await client.DeleteAsync<string>(RequestUris.DeleteTeamStakeholder(teamUId, stakeholderUId));

            //get all stakeholders in team
            var listOfStakeholders = await client.GetAsync<IList<TeamMemberResponse>>(RequestUris.TeamStakeholder(teamUId));

            //assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status code does not match");
            Assert.IsFalse(listOfStakeholders.Dto.Any(dto => dto.Uid == stakeholderUId), $"Stakeholder {stakeholderUId} was not deleted");
        }


        // 400
        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public async Task TeamStakeholders_Delete_InvalidMemberUid_BadRequest()
        {
            // arrange
            var client = await GetAuthenticatedClient();

            var team = TeamFactory.GetValidPostTeam("GetValidPostTeam_");
            var teamResponse = await client.PostAsync<TeamResponse>(RequestUris.Teams(), team);
            teamResponse.EnsureSuccess();
            var teamUid = teamResponse.Dto.Uid;
            var invalidMemberUid = new Guid();

            // act
            var response = await client.DeleteAsync(RequestUris.DeleteTeamStakeholder(teamUid, invalidMemberUid));

            // assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Response Status Code doesn't match");
        }

        // 401
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("Member"), TestCategory("OrgLeader")]
        public async Task TeamStakeholders_Delete_NotAuthorized()
        {
            //arrange
            var client = GetUnauthenticatedClient();
            var teamUId = Guid.NewGuid();
            var stakeholderUId = Guid.NewGuid();

            //delete stakeholder
            var response = await client.DeleteAsync<string>(RequestUris.DeleteTeamStakeholder(teamUId, stakeholderUId));

            //assert
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status code does not match");
        }

        // 403
        [TestMethod]
        [TestCategory("Member"), TestCategory("OrgLeader"), TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public async Task TeamStakeholders_Delete_NoPermission_Forbidden()
        {
            //arrange
            var client = await GetAuthenticatedClient();
            var teamUId = Guid.NewGuid();
            var stakeholderUId = Guid.NewGuid();

            //delete stakeholder
            var response = await client.DeleteAsync<string>(RequestUris.DeleteTeamStakeholder(teamUId, stakeholderUId));

            //assert
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status code does not match");
        }

        // 404
        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public async Task TeamStakeholders_Delete_NotFound()
        {
            //arrange
            var client = await GetAuthenticatedClient();

            //create team
            var team = TeamFactory.GetValidPostTeam("GetValidPostTeam_");
            var teamResponse = await client.PostAsync<TeamResponse>(RequestUris.Teams(), team);
            teamResponse.EnsureSuccess();

            var teamUId = teamResponse.Dto.Uid;

            //add stakeholder
            var stakeholderUId = Guid.NewGuid();

            //delete stakeholder
            var response = await client.DeleteAsync<string>(RequestUris.DeleteTeamStakeholder(teamUId, stakeholderUId));

            //assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode, "Status code does not match");
        }
    }
}
