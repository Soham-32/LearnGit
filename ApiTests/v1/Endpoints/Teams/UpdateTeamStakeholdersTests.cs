using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AtCommon.ObjectFactories;
using AtCommon.Api;
using AtCommon.Dtos.Teams;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiTests.v1.Endpoints.Teams
{
    [TestClass]
    [TestCategory("Teams"), TestCategory("Public")]
    public class UpdateTeamStakeholdersTests : BaseV1Test
    {
        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public async Task TeamStakeholders_Put_Success()
        {
            // authenticate
            var client = await GetAuthenticatedClient();
            
            // add team
            var team = TeamFactory.GetValidPostTeam("GetValidPostTeam_");
            var teamResponse = await client.PostAsync<TeamResponse>(RequestUris.Teams(), team);
            teamResponse.EnsureSuccess();

            // add stakeholder
            var stake = MemberFactory.GetValidPostTeamMember();
            var stakeResponse =
                await client.PostAsync<TeamMemberResponse>(RequestUris.TeamStakeholder(teamResponse.Dto.Uid), stake);
            stakeResponse.EnsureSuccess();
            
            // update stakeholder
            var updatedStake = MemberFactory.GetValidPutTeamMember();
            var response =
                await client.PutAsync<TeamMemberResponse>(
                    RequestUris.TeamStakeholderUpdate(teamResponse.Dto.Uid, stakeResponse.Dto.Uid), updatedStake);
            // assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Response Status Code doesn't match");
            Assert.AreEqual(updatedStake.FirstName, response.Dto.FirstName, "FirstName doesn't match");
            Assert.AreEqual(updatedStake.LastName, response.Dto.LastName, "FirstName doesn't match");
            Assert.AreEqual(updatedStake.Email, response.Dto.Email, "FirstName doesn't match");
            Assert.AreEqual(stakeResponse.Dto.Uid, response.Dto.Uid, "Uid doesn't match");
        }

        [TestMethod]
        [TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task TeamStakeholders_Put_NoPermission_Forbidden()
        {
            // authenticate
            var client = await GetAuthenticatedClient();

            // find a team with stakeholders
            var teamsResponse = await client.GetAsync<IList<TeamProfileResponse>>(RequestUris.Teams());
            var firstTeam = teamsResponse.Dto.First(team => team.Name == "Automation Multi Team for Update");

            var stakeUid = new Guid();

            var updatedStake = MemberFactory.GetValidPutTeamMember();
            var response =
                await client.PutAsync(
                    RequestUris.TeamStakeholderUpdate(firstTeam.Uid, stakeUid), updatedStake.ToStringContent());
            // assert
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Response Status Code doesn't match");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task TeamStakeholders_Put_Unauthorized()
        {
            var client = GetUnauthenticatedClient();

            var updatedStake = MemberFactory.GetValidPutTeamMember();
            var response = await client.PutAsync(RequestUris.TeamStakeholderUpdate(Guid.NewGuid(),
                Guid.NewGuid()), updatedStake.ToStringContent());

            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Response Status code doesn't match");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public async Task TeamStakeholders_Put_InvalidUid_BadRequest()
        {
            // authenticate
            var client = await GetAuthenticatedClient();

            // add team
            var team = TeamFactory.GetValidPostTeam("GetValidPostTeam_");
            var teamResponse = await client.PostAsync<TeamResponse>(RequestUris.Teams(), team);
            teamResponse.EnsureSuccess();
            var updatedStake = MemberFactory.GetValidPutTeamMember();

            var response = await client.PutAsync(RequestUris.TeamStakeholderUpdate(teamResponse.Dto.Uid,
                new Guid()), updatedStake.ToStringContent());

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status code doesn't match");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public async Task TeamStakeholders_Put_InvalidBody_BadRequest()
        {
            // authenticate
            var client = await GetAuthenticatedClient();

            // add team
            var team = TeamFactory.GetValidPostTeam("GetValidPostTeam_");
            var teamResponse = await client.PostAsync<TeamResponse>(RequestUris.Teams(), team);
            teamResponse.EnsureSuccess();

            // add stakeholder
            var stake = MemberFactory.GetValidPostTeamMember();
            var stakeResponse =
                await client.PostAsync<TeamMemberResponse>(RequestUris.TeamStakeholder(teamResponse.Dto.Uid), stake);
            stakeResponse.EnsureSuccess();
            // update stakeholder

            var updatedStake = TeamFactory.GetValidPostTeam("GetValidPostTeam_");
            var response = await client.PutAsync(RequestUris.TeamStakeholderUpdate(teamResponse.Dto.Uid,
                stakeResponse.Dto.Uid), updatedStake.ToStringContent());
            // assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status Code doesn't match");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public async Task TeamStakeholders_Put_NotFound()
        {
            // authenticate
            var client = await GetAuthenticatedClient();

            // add team
            var team = TeamFactory.GetValidPostTeam("GetValidPostTeam_");
            var teamResponse = await client.PostAsync<TeamResponse>(RequestUris.Teams(), team);
            teamResponse.EnsureSuccess();

            var stake = MemberFactory.GetValidPostTeamMember();
            var response = await client.PutAsync(RequestUris.TeamStakeholderUpdate(teamResponse.Dto.Uid,
                Guid.NewGuid()), stake.ToStringContent());

            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode, "Response status code doesn't match");
        }
    }
}
