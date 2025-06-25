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
    public class DeleteTeamMemberTests
    {
        [TestClass]
        [TestCategory("Teams"), TestCategory("Public")]
        public class DeleteTeamMemberTest : BaseV1Test
        {
            //200
            [TestMethod]
            [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
            public async Task TeamMembers_Delete_Success()
            {
                //arrange
                var client = await GetAuthenticatedClient();
                var team = TeamFactory.GetValidPostTeam("GetValidPostTeam_");

                //create team
                var teamResponse = await client.PostAsync<TeamResponse>(RequestUris.Teams(), team);
                teamResponse.EnsureSuccess();
                var teamUId = teamResponse.Dto.Uid;

                //add team member to team
                var teamMember = MemberFactory.GetValidPostTeamMember();
                var memberResponse = await client.PostAsync<TeamMemberResponse>(RequestUris.TeamMembers(teamUId), teamMember);
                var teamMemberId = memberResponse.Dto.Uid;

                //delete team member
                var response = await client.DeleteAsync<TeamMemberResponse>(RequestUris.DeleteTeamMember(teamUId, teamMemberId));

                //get created team
                var specificTeam = await client.GetAsync<TeamResponse>(RequestUris.TeamDetails(teamUId));

                //assert
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status code does not match");
                Assert.IsFalse(specificTeam.Dto.Members.Any(dto => dto.Uid == teamMemberId), $"Team {teamMemberId} was not deleted");
            }


            //200
            [TestMethod]
            [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
            public async Task TeamMembers_Delete_ReAdd_Success()
            {
                //arrange
                var client = await GetAuthenticatedClient();
                var team = TeamFactory.GetValidPostTeam("GetValidPostTeam_");

                //create team
                var teamResponse = await client.PostAsync<TeamResponse>(RequestUris.Teams(), team);
                teamResponse.EnsureSuccess();
                var teamUId = teamResponse.Dto.Uid;

                //add team member to team
                var teamMember = MemberFactory.GetValidPostTeamMember();
                var memberResponse = await client.PostAsync<TeamMemberResponse>(RequestUris.TeamMembers(teamUId), teamMember);
                var teamMemberId = memberResponse.Dto.Uid;

                //delete team member
                var response = await client.DeleteAsync<TeamMemberResponse>(RequestUris.DeleteTeamMember(teamUId, teamMemberId));
                response.EnsureSuccess();

                //read team member
                var reAddTeamMember = MemberFactory.GetValidPostTeamMember();
                reAddTeamMember.FirstName = teamMember.FirstName;
                reAddTeamMember.LastName = teamMember.LastName;

                var reAddMemberResponse = await client.PostAsync<TeamMemberResponse>(RequestUris.TeamMembers(teamUId), reAddTeamMember);

                //assert
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status code does not match");
                Assert.AreEqual(reAddMemberResponse.Dto.FirstName, teamMember.FirstName, "FirstName doesn't match");
                Assert.AreEqual(reAddMemberResponse.Dto.LastName, teamMember.LastName, "LastName doesn't match");
                Assert.IsTrue(!string.IsNullOrEmpty(reAddMemberResponse.Dto.Uid.ToString()), "Uid is null");
                Assert.AreEqual(reAddMemberResponse.Dto.Tags.Count(), teamMember.Tags.Count(), "The amount of tags do not match");

                //assert initial member and readded have same tags
                for (var key = 0; key < teamMember.Tags.Count; key++)
                {
                    Assert.AreEqual(teamMember.Tags[key].Category, reAddMemberResponse.Dto.Tags[key].Key);
                    for (var value = 0; value < teamMember.Tags[key].Tags.Count(); value++)
                    {
                        Assert.AreEqual(teamMember.Tags[key].Tags[value], reAddMemberResponse.Dto.Tags[key].Value[value]);
                    }
                }
            }

            //400
            [TestMethod]
            [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
            public async Task TeamMembers_Delete_InvalidMemberUid_BadRequest()
            {
                // arrange
                var client = await GetAuthenticatedClient();
                var team = TeamFactory.GetValidPostTeam("GetValidPostTeam_");

                var teamResponse = await client.PostAsync<TeamResponse>(RequestUris.Teams(), team);
                teamResponse.EnsureSuccess();
                var teamUid = teamResponse.Dto.Uid;
                var invalidMemberUid = new Guid();

                // act
                var response =
                    await client.DeleteAsync(RequestUris.DeleteTeamMember(teamUid, invalidMemberUid));

                // assert
                Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Response Status Code doesn't match");
            }

            //401
            [TestMethod]
            [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("Member"), TestCategory("OrgLeader")]
            public async Task TeamMembers_Delete_MissingToken_Unauthorized()
            {
                //arrange
                var client = GetUnauthenticatedClient();
                var teamUid = Guid.NewGuid();
                var teamMemberUid = Guid.NewGuid();

                //delete team member
                var response = await client.DeleteAsync<TeamMemberResponse>(RequestUris.DeleteTeamMember(teamUid, teamMemberUid));

                //assert
                Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status code does not match");
            }

            //403
            [TestMethod]
            [TestCategory("Member"), TestCategory("OrgLeader")]
            public async Task TeamMembers_Delete_NoPermission_Forbidden()
            {
                //arrange
                var client = await GetAuthenticatedClient();
                var teamsResponse = await client.GetAsync<IList<TeamProfileResponse>>(RequestUris.Teams());
                teamsResponse.EnsureSuccess();
                var firstTeam = teamsResponse.Dto.First();
                var teamUId = firstTeam.Uid;

                //get team member
                var memberResponse = await client.GetAsync<IList<TeamMemberResponse>>(RequestUris.TeamMembers(teamUId));
                var teamMemberUid = memberResponse.Dto.First().Uid;

                //delete team member
                var response = await client.DeleteAsync<TeamMemberResponse>(RequestUris.DeleteTeamMember(teamUId, teamMemberUid));

                //get team and member
                var specificTeamResponse = await client.GetAsync<IList<TeamMemberResponse>>(RequestUris.TeamMembers(teamUId));

                //assert
                Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status code does not match");
                Assert.IsTrue(specificTeamResponse.Dto.Any(dto => dto.Uid == teamMemberUid), $"Team member {teamMemberUid} was deleted");
            }

            //404
            [TestMethod]
            [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
            public async Task TeamMembers_Delete_NotFound()
            {
                //arrange
                var client = await GetAuthenticatedClient();
                var team = TeamFactory.GetValidPostTeam("GetValidPostTeam_");

                //create team
                var teamResponse = await client.PostAsync<TeamResponse>(RequestUris.Teams(), team);
                teamResponse.EnsureSuccess();
                var teamUId = teamResponse.Dto.Uid;

                //add team member to team
                var teamMember = MemberFactory.GetValidPostTeamMember();
                var memberResponse = await client.PostAsync<TeamMemberResponse>(RequestUris.TeamMembers(teamUId), teamMember);
                var teamMemberId = memberResponse.Dto.Uid;

                //delete team member
                var firstDelete = await client.DeleteAsync<TeamMemberResponse>(RequestUris.DeleteTeamMember(teamUId, teamMemberId));
                firstDelete.EnsureSuccess();

                //attempt to delete nonexistent team member 
                var secondDelete = await client.DeleteAsync<TeamMemberResponse>(RequestUris.DeleteTeamMember(teamUId, teamMemberId));

                //get created team
                var specificTeam = await client.GetAsync<TeamResponse>(RequestUris.TeamDetails(teamUId));

                //assert
                Assert.AreEqual(HttpStatusCode.NotFound, secondDelete.StatusCode, "Status code does not match");
                Assert.IsFalse(specificTeam.Dto.Members.Any(dto => dto.Uid == teamMemberId), $"Team {teamMemberId} was not deleted");
            }
        }
    }
}
