using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AtCommon.Api;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiTests.v1.Endpoints.Teams
{
    [TestClass]
    [TestCategory("Teams"), TestCategory("Public")]
    public class UpdateTeamMemberTests : BaseV1Test
    {
        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public async Task TeamMembers_Put_Success()
        {
            // arrange
            var client = await GetAuthenticatedClient();

            // add a new team
            var team = TeamFactory.GetValidPostTeam("GetValidPostTeam_");

            var teamResponse = await client.PostAsync<TeamResponse>(RequestUris.Teams(), team);
            teamResponse.EnsureSuccess();
            var teamUid = teamResponse.Dto.Uid;

            // add a new member to the team
            var member = MemberFactory.GetValidPostTeamMember();
            var memberResponse = await client.PostAsync<TeamMemberResponse>(RequestUris.TeamMembers(teamUid), member);
            memberResponse.EnsureSuccess();
            var updatedMember = MemberFactory.GetValidPutTeamMember();
            // act
            var response =
                await client.PutAsync<TeamMemberResponse>(RequestUris.TeamMemberUpdate(teamUid, memberResponse.Dto.Uid), updatedMember);

            // assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Response Status Code doesn't match");
            Assert.AreEqual(updatedMember.FirstName, response.Dto.FirstName, "FirstName doesn't match");
            Assert.AreEqual(updatedMember.LastName, response.Dto.LastName, "LastName doesn't match");
            Assert.AreEqual(updatedMember.Email, response.Dto.Email, "EmailName doesn't match");
            Assert.AreEqual(updatedMember.ExternalIdentifier, response.Dto.ExternalIdentifier, "ExternalIdentifier doesn't match");

        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public async Task TeamMembers_Put_Null_Tags_Success()
        {
            // arrange
            var client = await GetAuthenticatedClient();

            // add a new team
            var team = TeamFactory.GetValidPostTeam("GetValidPostTeam_");

            var teamResponse = await client.PostAsync<TeamResponse>(RequestUris.Teams(), team);
            teamResponse.EnsureSuccess();
            var teamUid = teamResponse.Dto.Uid;

            // add a new member to the team
            var member = MemberFactory.GetValidPostTeamMember();
            var memberResponse = await client.PostAsync<TeamMemberResponse>(RequestUris.TeamMembers(teamUid), member);
            memberResponse.EnsureSuccess();
            var updatedMember = MemberFactory.GetValidPutTeamMember();
            updatedMember.Tags = null;

            // act
            var response =
                await client.PutAsync<TeamMemberResponse>(RequestUris.TeamMemberUpdate(teamUid, memberResponse.Dto.Uid), updatedMember);

            // assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Response Status Code doesn't match");
            Assert.AreEqual(updatedMember.FirstName, response.Dto.FirstName, "FirstName doesn't match");
            Assert.AreEqual(updatedMember.LastName, response.Dto.LastName, "LastName doesn't match");
            Assert.AreEqual(updatedMember.Email, response.Dto.Email, "EmailName doesn't match");
            Assert.AreEqual(updatedMember.ExternalIdentifier, response.Dto.ExternalIdentifier, "ExternalIdentifier doesn't match");
            Assert.AreNotEqual(null, response.Dto.Tags, "Tag is null and is not supposed to be");

        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public async Task TeamMembers_Put_Empty_Array_Success()
        {
            // arrange
            var client = await GetAuthenticatedClient();

            // add a new team
            var team = TeamFactory.GetValidPostTeam("GetValidPostTeam_");

            var teamResponse = await client.PostAsync<TeamResponse>(RequestUris.Teams(), team);
            teamResponse.EnsureSuccess();
            var teamUid = teamResponse.Dto.Uid;

            // add a new member to the team
            var member = MemberFactory.GetValidPostTeamMember();
            var memberResponse = await client.PostAsync<TeamMemberResponse>(RequestUris.TeamMembers(teamUid), member);
            memberResponse.EnsureSuccess();
            var updatedMember = MemberFactory.GetValidPutTeamMember();
            updatedMember.Tags.Find(x => x.Category == "Role").Tags = new List<string>();
            // act
            var response =
                await client.PutAsync<TeamMemberResponse>(RequestUris.TeamMemberUpdate(teamUid, memberResponse.Dto.Uid), updatedMember);

            // assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Response Status Code doesn't match");
            Assert.AreEqual(updatedMember.FirstName, response.Dto.FirstName, "FirstName doesn't match");
            Assert.AreEqual(updatedMember.LastName, response.Dto.LastName, "LastName doesn't match");
            Assert.AreEqual(updatedMember.Email, response.Dto.Email, "EmailName doesn't match");
            Assert.AreEqual(updatedMember.ExternalIdentifier, response.Dto.ExternalIdentifier, "ExternalIdentifier doesn't match");
            Assert.AreEqual(updatedMember.Tags.Count - 1, response.Dto.Tags.Count, "Empty array did not remove a tag");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public async Task TeamMembers_Put_Update_Tags_Success()
        {
            // arrange
            var client = await GetAuthenticatedClient();

            // add a new team
            var team = TeamFactory.GetValidPostTeam("GetValidPostTeam_");

            var teamResponse = await client.PostAsync<TeamResponse>(RequestUris.Teams(), team);
            teamResponse.EnsureSuccess();
            var teamUid = teamResponse.Dto.Uid;

            // add a new member to the team
            var member = MemberFactory.GetValidPostTeamMember();
            var memberResponse = await client.PostAsync<TeamMemberResponse>(RequestUris.TeamMembers(teamUid), member);
            memberResponse.EnsureSuccess();
            var updatedMember = MemberFactory.GetValidPutTeamMember_Update_Tags();
            // act
            var response =
                await client.PutAsync<TeamMemberResponse>(RequestUris.TeamMemberUpdate(teamUid, memberResponse.Dto.Uid), updatedMember);

            // assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Response Status Code doesn't match");
            Assert.AreEqual(updatedMember.FirstName, response.Dto.FirstName, "FirstName doesn't match");
            Assert.AreEqual(updatedMember.LastName, response.Dto.LastName, "LastName doesn't match");
            Assert.AreEqual(updatedMember.Email, response.Dto.Email, "EmailName doesn't match");
            Assert.AreEqual(updatedMember.ExternalIdentifier, response.Dto.ExternalIdentifier, "ExternalIdentifier doesn't match");
            Assert.AreEqual(updatedMember.Tags.Find(x => x.Category == "Participant Group").Tags[0], response.Dto.Tags.Find(x => x.Key == "Participant Group").Value[0], "Tag did not updated");
        }

        [TestMethod]
        [TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task TeamMembers_Put_NoPermission_Forbidden()
        {
            // arrange
            const string teamName = "Automation Multi Team for Update";
            const string memberName = "Update_Mem";
            var client = await GetAuthenticatedClient();
            var teamsResponse = await client.GetAsync<IList<TeamProfileResponse>>(RequestUris.Teams());
            teamsResponse.EnsureSuccess();
            var firstTeam = teamsResponse.Dto.FirstOrDefault(team => team.Name == teamName);
            if (firstTeam == null) throw new Exception($"<{teamName}> was not found for this user.");
            var teamUid = firstTeam.Uid;
            var memberResponse = await client.GetAsync<IList<TeamMemberResponse>>(RequestUris.TeamMembers(teamUid));
            memberResponse.EnsureSuccess();
            var member = memberResponse.Dto.FirstOrDefault(m => m.FirstName == memberName);
            if (member == null) 
                throw new Exception($"Team member was not found with first name <{memberName}> on the {teamName}");
            var updatedTeamMember = MemberFactory.GetValidPutTeamMember();
            // act
            var updatedTeamResponse = await client.PutAsync<IList<string>>(RequestUris.TeamMemberUpdate(teamUid, member.Uid), updatedTeamMember);
            // assert
            Assert.AreEqual(HttpStatusCode.Forbidden, updatedTeamResponse.StatusCode, "Response Status Code doesn't match");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member")]
        public async Task TeamMembers_Put_MissingToken_Unauthorized()
        {
            //arrange
            var client = GetUnauthenticatedClient();
            var teamUid = Guid.NewGuid();
            var memberUid = Guid.NewGuid();
            var updatedMember = MemberFactory.GetValidPutTeamMember();

            //act
            var response =
                await client.PutAsync(RequestUris.TeamMemberUpdate(teamUid, memberUid), updatedMember.ToStringContent());

            //assert
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Response Status Code doesn't match");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public async Task TeamMembers_Put_InvalidUid_BadRequest()
        {
            // arrange
            var client = await GetAuthenticatedClient();
            var team = TeamFactory.GetValidPostTeam("GetValidPostTeam_");

            var teamResponse = await client.PostAsync<TeamResponse>(RequestUris.Teams(), team);
            teamResponse.EnsureSuccess();
            var teamUid = teamResponse.Dto.Uid;

            var member = MemberFactory.GetValidPostTeamMember();
            var memberResponse = await client.PostAsync<TeamMemberResponse>(RequestUris.TeamMembers(teamUid), member);
            memberResponse.EnsureSuccess();
            var updatedMember = MemberFactory.GetValidPutTeamMember();
            // act
            var response =
                await client.PutAsync(RequestUris.TeamMemberUpdate(teamUid, new Guid()), updatedMember.ToStringContent());

            // assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Response Status Code doesn't match");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public async Task TeamMembers_Put_InvalidBody_BadRequest()
        {
            // arrange
            var client = await GetAuthenticatedClient();
            var team = TeamFactory.GetValidPostTeam("GetValidPostTeam_");

            var teamResponse = await client.PostAsync<TeamResponse>(RequestUris.Teams(), team);
            teamResponse.EnsureSuccess();
            var teamUid = teamResponse.Dto.Uid;

            var member = MemberFactory.GetValidPostTeamMember();
            var memberResponse = await client.PostAsync<TeamMemberResponse>(RequestUris.TeamMembers(teamUid), member);
            memberResponse.EnsureSuccess();

            var updatedMember = TeamFactory.GetValidPutTeam("GetValidPutTeam_");

            // act
            var response =
                await client.PutAsync(RequestUris.TeamMemberUpdate(teamUid, memberResponse.Dto.Uid), updatedMember.ToStringContent());

            // assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Response Status Code doesn't match");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public async Task TeamMembers_Put_NotFound()
        {
            // arrange
            var client = await GetAuthenticatedClient();
            var team = TeamFactory.GetValidPostTeam("GetValidPostTeam_");

            var teamResponse = await client.PostAsync<TeamResponse>(RequestUris.Teams(), team);
            teamResponse.EnsureSuccess();
            var teamUid = teamResponse.Dto.Uid;

            var member = MemberFactory.GetValidPostTeamMember();
            var memberResponse = await client.PostAsync<TeamMemberResponse>(RequestUris.TeamMembers(teamUid), member);
            memberResponse.EnsureSuccess();

            var updatedMember = MemberFactory.GetValidPutTeamMember();
            // act
            var response =
                await client.PutAsync(RequestUris.TeamMemberUpdate(teamUid, Guid.NewGuid()), updatedMember.ToStringContent());

            // assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode, "Response Status Code doesn't match");
        }
    }
}
