using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AtCommon.ObjectFactories;
using AtCommon.Api;
using AtCommon.Dtos.Teams;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiTests.v1.Endpoints.Teams
{
    [TestClass]
    [TestCategory("Teams"), TestCategory("Public")]
    public class AddTeamStakeholdersTests : BaseV1Test
    {
        // 201
        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public async Task TeamStakeholder_Post_Success()
        {
            var client = await GetAuthenticatedClient();

            var team = TeamFactory.GetValidPostTeam("GetValidPostTeam_");

            var teamResponse = await client.PostAsync<TeamResponse>(RequestUris.Teams(), team);
            teamResponse.EnsureSuccess();
            var teamUid = teamResponse.Dto.Uid;

            var member = MemberFactory.GetValidPostTeamStakeholder();

            var response = await client.PostAsync<StakeholderResponse>(RequestUris.TeamStakeholder(teamUid), member);

            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode, "Response Status code doesn't match");
            Assert.AreEqual(member.FirstName, response.Dto.FirstName, "FirstName doesn't match");
            Assert.AreEqual(member.LastName, response.Dto.LastName, "LastName doesn't match");
            Assert.AreEqual(member.Email, response.Dto.Email, "Email doesn't match");
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.Dto.Uid.ToString()), "Uid is null or empty");
        }

        // 400
        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public async Task TeamStakeholder_Post_InvalidBody_BadRequest()
        {
            var client = await GetAuthenticatedClient();

            var team = TeamFactory.GetValidPostTeam("GetValidPostTeam_");

            var teamResponse = await client.PostAsync<TeamResponse>(RequestUris.Teams(), team);
            teamResponse.EnsureSuccess();
            var teamUid = teamResponse.Dto.Uid;

            var member = TeamFactory.GetValidPostTeam("GetValidPostTeam_");
            var memberJson = member.ToStringContent();

            var response = await client.PostAsync(RequestUris.TeamStakeholder(teamUid), memberJson);

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Response Status code doesn't match");
        }

        // 401
        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("Member"), TestCategory("SiteAdmin")]
        public async Task TeamStakeholder_Post_Unauthorized()
        {
            var client = GetUnauthenticatedClient();
            var teamUid = Guid.NewGuid();

            var member = MemberFactory.GetValidPostTeamStakeholder();
            var memberJson = member.ToStringContent();
            var response = await client.PostAsync(RequestUris.TeamStakeholder(teamUid), memberJson);

            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Response Status code doesn't match");
        }

        // 403
        [TestMethod]
        [TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task TeamStakeholder_Post_NoPermission_Forbidden()
        {
            User.VerifyType(new List<UserType> { UserType.OrganizationalLeader, UserType.Member });

            var client = await GetAuthenticatedClient();

            var teamsResponse = await client.GetAsync<IList<TeamProfileResponse>>(RequestUris.Teams());
            var teamUid = teamsResponse.Dto.First(team => team.Name == "Automation Multi Team for Update").Uid;

            var member = MemberFactory.GetValidPostTeamStakeholder();

            var response = await client.PostAsync(RequestUris.TeamStakeholder(teamUid), member.ToStringContent());

            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Response Status code doesn't match");
        }

        // 403
        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        public async Task TeamStakeholder_Post_Forbidden()
        {
            var client = await GetAuthenticatedClient();
            var teamUid = new Guid();

            var member = MemberFactory.GetValidPostTeamStakeholder();
            var memberJson = member.ToStringContent();
            var response = await client.PostAsync(RequestUris.TeamStakeholder(teamUid), memberJson);

            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Response Status code doesn't match");
        }

        // 404
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public async Task TeamStakeholder_Post_InvalidTeamUid_NotFound()
        {
            var client = await GetAuthenticatedClient();
            var invalidTeamUid = new Guid();

            var member = MemberFactory.GetValidPostTeamStakeholder();
            var memberJson = member.ToStringContent();
            var response = await client.PostAsync(RequestUris.TeamStakeholder(invalidTeamUid), memberJson);

            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode, "Response Status code doesn't match");
        }
    }
}
