using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AtCommon.Api;
using AtCommon.Dtos.Teams;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiTests.v1.Endpoints.Teams
{
    [TestClass]
    [TestCategory("Teams"), TestCategory("Public")]
    public class GetTeamStakeholdersTests : BaseV1Test
    {
        // 200
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        public async Task TeamStakeholders_Get_Success()
        {
            // arrange
            var client = await GetAuthenticatedClient();
            var teamsResponse = await client.GetAsync<IList<TeamProfileResponse>>(RequestUris.Teams());
            teamsResponse.EnsureSuccess();
            var firstTeam = teamsResponse.Dto.First(team => team.Name == "Automation Radar Team");
            // act
            var response = await client.GetAsync<IList<TeamMemberResponse>>(RequestUris.TeamStakeholder(firstTeam.Uid));

            // assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Response Status code does not match");
            Assert.IsTrue(response.Dto.Any(dto => !string.IsNullOrWhiteSpace(dto.FirstName)), "FirstName is null or empty");
            Assert.IsTrue(response.Dto.Any(dto => !string.IsNullOrWhiteSpace(dto.LastName)), "LastName is null or empty");
            Assert.IsTrue(response.Dto.Any(dto => !string.IsNullOrWhiteSpace(dto.Email)), "Email is null or empty");
            Assert.IsTrue(response.Dto.Any(dto => !string.IsNullOrWhiteSpace(dto.Uid.ToString())), "Uid is null or empty");

        }

        // 401
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task TeamStakeholders_Get_Unauthorized()
        {
            //arrange
            var client = GetUnauthenticatedClient();
            var newGuid = Guid.NewGuid();

            //act 
            var response = await client.GetAsync(RequestUris.TeamStakeholder(newGuid));

            // assert
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Response Status Code does not match");
        }

        // 403
        [TestMethod]
        [TestCategory("Member")]
        public async Task TeamStakeholders_Get_Role_Forbidden()
        {
            // arrange
            var client = await GetAuthenticatedClient();
            var teamsResponse = await client.GetAsync<IList<TeamProfileResponse>>(RequestUris.Teams());
            teamsResponse.EnsureSuccess();
            const string teamName = "Automation Radar Team";
            var firstTeam = teamsResponse.Dto.FirstOrDefault(team => team.Name == teamName);
            if (firstTeam == null) throw new Exception($"<{teamName}> was not found in the response");

            // act
            var response = await client.GetAsync(RequestUris.TeamStakeholder(firstTeam.Uid));

            // assert
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Response Status code does not match");
        }

        // 403
        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task TeamStakeholders_Get_Forbidden()
        {
            //arrange
            var client = await GetAuthenticatedClient();
            var newGuid = Guid.NewGuid();

            // act
            var response = await client.GetAsync(RequestUris.TeamStakeholder(newGuid));

            // assert
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Response Status Code does not match");
        }

        // 404
        [TestMethod]
        [TestCategory("SiteAdmin")]
        public async Task TeamStakeholders_Get_InvalidTeamUid_NotFound()
        {
            //arrange
            var client = await GetAuthenticatedClient();
            var invalidTeamUid = Guid.NewGuid();

            // act
            var response = await client.GetAsync(RequestUris.TeamStakeholder(invalidTeamUid));

            // assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode, "Response Status Code does not match");
        }
    }
}
