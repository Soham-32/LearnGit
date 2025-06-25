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
    public class AddTeamMemberTests : BaseV1Test
    {
        // 201
        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public async Task TeamMembers_Post_Created()
        {
            // arrange
            var client = await GetAuthenticatedClient();

            var team = TeamFactory.GetValidPostTeam("GetValidPostTeam_");

            var teamResponse = await client.PostAsync<TeamResponse>(RequestUris.Teams(), team);
            teamResponse.EnsureSuccess();
            var teamUid = teamResponse.Dto.Uid;

            var member = MemberFactory.GetValidPostTeamMember();

            // act
            var memberResponse = await client.PostAsync<TeamMemberResponse>(RequestUris.TeamMembers(teamUid), member);

            // assert
            Assert.AreEqual(HttpStatusCode.Created, memberResponse.StatusCode, "Status Code doesn't match");
            Assert.IsTrue(memberResponse.LocationHeader.ToString().Contains(teamUid.ToString()),
                $"Expected: <{teamUid}>. Actual: <{memberResponse.LocationHeader}>");
            Assert.AreEqual(member.FirstName, memberResponse.Dto.FirstName, "FirstName doesn't match");
            Assert.AreEqual(member.LastName, memberResponse.Dto.LastName, "LastName doesn't match");
            Assert.AreEqual(member.Email, memberResponse.Dto.Email, "Email doesn't match");
            Assert.IsTrue(!string.IsNullOrEmpty(memberResponse.Dto.Uid.ToString()), "Uid is null");
        }


        // 400
        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public async Task TeamMembers_Post_Enterprise_BadRequest()
        {
            // arrange
            var client = await GetAuthenticatedClient();

            var teamsResponse = await client.GetAsync<IList<TeamProfileResponse>>(RequestUris.Teams());
            teamsResponse.EnsureSuccess();
            var enterpriseTeam = teamsResponse.Dto.First(t => t.Type == "Enterprise");

            var member = MemberFactory.GetValidPostTeamMember();

            // act
            var response = await client.PostAsync<IList<string>>(RequestUris.TeamMembers(enterpriseTeam.Uid), member);

            // assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Response Status Code does not match.");
            Assert.AreEqual("Member can not be added to multi-team or enterprise team.", response.Dto[0]);
        }

        // 400
        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public async Task TeamMembers_Post_MultiTeam_BadRequest()
        {
            // arrange
            var client = await GetAuthenticatedClient();

            var teamsResponse = await client.GetAsync<IList<TeamProfileResponse>>(RequestUris.Teams());
            teamsResponse.EnsureSuccess();
            var multiTeam = teamsResponse.Dto.First(t => t.Type == "MultiTeam");

            var member = MemberFactory.GetValidPostTeamMember();

            // act
            var response = await client.PostAsync<IList<string>>(RequestUris.TeamMembers(multiTeam.Uid), member);

            // assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Response Status Code does not match.");
            Assert.AreEqual("Member can not be added to multi-team or enterprise team.", response.Dto[0]);
        }

        // 400
        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("SiteAdmin")]
        public async Task TeamMembers_Post_MissingFirstName_BadRequest()
        {
            // arrange
            var client = await GetAuthenticatedClient();

            var teamsResponse = await client.GetAsync<IList<TeamProfileResponse>>(RequestUris.Teams());
            teamsResponse.EnsureSuccess();
            var team = teamsResponse.Dto.First(t => t.Type == "Team");

            var member = MemberFactory.GetValidPostTeamMember();
            member.FirstName = null;

            // act
            var response = await client.PostAsync<IList<string>>(RequestUris.TeamMembers(team.Uid), member);

            // assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Response Status Code does not match.");
            Assert.AreEqual("'FirstName' should not be empty", response.Dto[0]);
        }

        // 400
        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public async Task TeamMembers_Post_MissingLastName_BadRequest()
        {
            // arrange
            var client = await GetAuthenticatedClient();

            var teamsResponse = await client.GetAsync<IList<TeamProfileResponse>>(RequestUris.Teams());
            teamsResponse.EnsureSuccess();
            var team = teamsResponse.Dto.First(t => t.Type == "Team");

            var member = MemberFactory.GetValidPostTeamMember();
            member.LastName = "";

            // act
            var response = await client.PostAsync<IList<string>>(RequestUris.TeamMembers(team.Uid), member);

            // assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Response Status Code does not match.");
            Assert.AreEqual("'LastName' should not be empty", response.Dto[0]);
        }

        // 400
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public async Task TeamMembers_Post_MissingEmail_BadRequest()
        {
            // arrange
            var client = await GetAuthenticatedClient();

            var teamsResponse = await client.GetAsync<IList<TeamProfileResponse>>(RequestUris.Teams());
            teamsResponse.EnsureSuccess();
            var team = teamsResponse.Dto.First(t => t.Type == "Team");

            var member = MemberFactory.GetValidPostTeamMember();
            member.Email = null;

            // act
            var response = await client.PostAsync<IList<string>>(RequestUris.TeamMembers(team.Uid), member);

            // assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Response Status Code does not match.");
            Assert.AreEqual("'Email' should not be empty.", response.Dto[0]);
        }

        // 400
        [TestMethod]
        [TestCategory("KnownDefect")] // Bug: 38984
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public async Task TeamMembers_Post_InvalidTeamUid()
        {
            // arrange
            var client = await GetAuthenticatedClient();

            var invalidTeamUid = Guid.NewGuid();
            var member = MemberFactory.GetValidPostTeamMember();

            // act
            var response = await client.PostAsync(RequestUris.TeamMembers(invalidTeamUid), member.Uid.ToStringContent());

            // assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Response Status Code does not match.");
        }

        // 401
        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member")]
        public async Task TeamMembers_Post_Unauthorized()
        {
            // arrange
            var client = GetUnauthenticatedClient();

            var member = MemberFactory.GetValidPostTeamMember();
            var unique = Guid.NewGuid();

            // act
            var response = await client.PostAsync(RequestUris.TeamMembers(unique), member.ToStringContent());

            // assert
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Response Status Code does not match.");
        }

        // 403
        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task TeamMembers_Post_InvalidTeamId_Forbidden()
        {
            // arrange
            var client = await GetAuthenticatedClient();
            var invalidTeamId = Guid.NewGuid();
            var member = MemberFactory.GetValidPostTeamMember();

            // act
            var response = await client.PostAsync(RequestUris.TeamMembers(invalidTeamId), member.ToStringContent());

            // assert
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Response Status Code does not match.");
        }

        // 404
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public async Task TeamMembers_Post_InvalidTeamId_NotFound()
        {
            // arrange
            var client = await GetAuthenticatedClient();

            var unique = Guid.NewGuid();
            var member = MemberFactory.GetValidPostTeamMember();

            // act
            var response = await client.PostAsync(RequestUris.TeamMembers(unique), member.ToStringContent());

            // assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode, "Response Status Code does not match.");
        }
    }
}