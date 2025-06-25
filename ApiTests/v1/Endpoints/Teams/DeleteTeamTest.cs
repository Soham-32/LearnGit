using AtCommon.Api;
using AtCommon.Dtos.Teams;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AtCommon.Api.Enums;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;

namespace ApiTests.v1.Endpoints.Teams
{
    [TestClass]
    [TestCategory("Teams")]
    public class DeleteTeamTest : BaseV1Test
    {
        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("PartnerAdmin")]
        public async Task Team_Delete_PermanentDelete_Success()
        {
            //arrange
            var client = await GetAuthenticatedClient();
            var team = TeamFactory.GetValidPostTeam("GetValidPostTeam_");

            //create team
            var teamResponse = await client.PostAsync<TeamResponse>(RequestUris.Teams(), team);
            teamResponse.EnsureSuccess();

            var teamUId = teamResponse.Dto.Uid;

            //delete team
            var response = await client.DeleteAsync<TeamResponse>(
                RequestUris.DeleteTeam(teamUId).AddQueryParameter("archiveStatus",
                ArchiveStatus.PermanentlyDelete.ToString("D")));

            //get all teams
            var listOfTeams = await client.GetAsync<IList<TeamProfileResponse>>(RequestUris.Teams());

            //assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status code does not match");
            Assert.IsFalse(listOfTeams.Dto.Any(dto => dto.Uid == teamUId), $"Team {teamUId} was not deleted");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("PartnerAdmin")]
        public async Task Team_Delete_ProjectCompleted_Archive_Success()
        {
            //arrange
            var client = await GetAuthenticatedClient();
            var team = TeamFactory.GetValidPostTeam("GetValidPostTeam_");

            //create team
            var teamResponse = await client.PostAsync<TeamResponse>(RequestUris.Teams(), team);
            teamResponse.EnsureSuccess();

            var teamUId = teamResponse.Dto.Uid;

            //delete team
            var response = await client.DeleteAsync<TeamResponse>(
                RequestUris.DeleteTeam(teamUId).AddQueryParameter("archiveStatus", ArchiveStatus.ProjectCompleted.ToString("D")));

            //get all teams
            var listOfTeams = await client.GetAsync<IList<TeamProfileResponse>>(RequestUris.Teams());

            //assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status code does not match");
            Assert.IsFalse(listOfTeams.Dto.Any(dto => dto.Uid == teamUId), $"Team {teamUId} was not archived");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("PartnerAdmin")]
        public async Task Team_Delete_TeamDisbanded_Archive_Success()
        {
            //arrange
            var client = await GetAuthenticatedClient();
            var team = TeamFactory.GetValidPostTeam("GetValidPostTeam_");

            //create team
            var teamResponse = await client.PostAsync<TeamResponse>(RequestUris.Teams(), team);
            teamResponse.EnsureSuccess();

            var teamUId = teamResponse.Dto.Uid;

            //delete team
            var response = await client.DeleteAsync<TeamResponse>(
                RequestUris.DeleteTeam(teamUId).AddQueryParameter("archiveStatus", ArchiveStatus.TeamDisbanded.ToString("D")));

            //get all teams
            var listOfTeams = await client.GetAsync<IList<TeamProfileResponse>>(RequestUris.Teams());

            //assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status code does not match");
            Assert.IsFalse(listOfTeams.Dto.Any(dto => dto.Uid == teamUId), $"Team {teamUId} was not archived");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("PartnerAdmin")]
        public async Task Team_Delete_NoBudget_Archive_Success()
        {
            //arrange
            var client = await GetAuthenticatedClient();
            var team = TeamFactory.GetValidPostTeam("GetValidPostTeam_");

            //create team
            var teamResponse = await client.PostAsync<TeamResponse>(RequestUris.Teams(), team);
            teamResponse.EnsureSuccess();

            var teamUId = teamResponse.Dto.Uid;

            //delete team
            var response = await client.DeleteAsync<TeamResponse>(RequestUris.DeleteTeam(teamUId)
                .AddQueryParameter("archiveStatus", ArchiveStatus.NoBudget.ToString("D")));

            //get all teams
            var listOfTeams = await client.GetAsync<IList<TeamProfileResponse>>(RequestUris.Teams());

            //assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status code does not match");
            Assert.IsFalse(listOfTeams.Dto.Any(dto => dto.Uid == teamUId), $"Team {teamUId} was not archived");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("PartnerAdmin")]
        public async Task Team_Delete_NoLongerUsingAgilityHealth_Archive_Success()
        {
            //arrange
            var client = await GetAuthenticatedClient();
            var team = TeamFactory.GetValidPostTeam("GetValidPostTeam_");

            //create team
            var teamResponse = await client.PostAsync<TeamResponse>(RequestUris.Teams(), team);
            teamResponse.EnsureSuccess();

            var teamUId = teamResponse.Dto.Uid;

            //delete team
            var response = await client.DeleteAsync<TeamResponse>(RequestUris.DeleteTeam(teamUId)
                .AddQueryParameter("archiveStatus",
                    ArchiveStatus.NoLongerUsingAgilityHealth.ToString("D")));

            //get all teams
            var listOfTeams = await client.GetAsync<IList<TeamProfileResponse>>(RequestUris.Teams());

            //assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status code does not match");
            Assert.IsFalse(listOfTeams.Dto.Any(dto => dto.Uid == teamUId), $"Team {teamUId} was not archived");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("PartnerAdmin")]
        public async Task Team_Delete_Other_Archive_Success()
        {
            //arrange
            var client = await GetAuthenticatedClient();
            var team = TeamFactory.GetValidPostTeam("GetValidPostTeam_");

            //create team
            var teamResponse = await client.PostAsync<TeamResponse>(RequestUris.Teams(), team);
            teamResponse.EnsureSuccess();

            var teamUId = teamResponse.Dto.Uid;

            //delete team
            var response = await client.DeleteAsync<TeamResponse>(RequestUris.DeleteTeam(teamUId)
                .AddQueryParameter("archiveStatus", ArchiveStatus.Other.ToString("D")));

            //get all teams
            var listOfTeams = await client.GetAsync<IList<TeamProfileResponse>>(RequestUris.Teams());

            //assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status code does not match");
            Assert.IsFalse(listOfTeams.Dto.Any(dto => dto.Uid == teamUId), $"Team {teamUId} was not archived");
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("Member"), TestCategory("OrgLeader"), TestCategory("PartnerAdmin")]
        public async Task Team_Delete_MissingToken_Unauthorized()
        {
            //arrange
            var client = GetUnauthenticatedClient();
            var uid = Guid.NewGuid();

            //delete team
            var response = await client.DeleteAsync<TeamResponse>(RequestUris.DeleteTeam(uid)
                .AddQueryParameter("archiveStatus", ArchiveStatus.PermanentlyDelete.ToString("D")));

            //assert
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode,
                "Status code does not match");
        }

        [TestMethod]
        [TestCategory("Member"), TestCategory("OrgLeader")]
        public async Task Team_Delete_NoPermission_Forbidden()
        {
            User.VerifyType(new List<UserType> { UserType.OrganizationalLeader, UserType.Member });

            //arrange
            var client = await GetAuthenticatedClient();
            var teamUid = Guid.NewGuid();

            //delete team
            var response = await client.DeleteAsync<TeamResponse>(RequestUris.DeleteTeam(teamUid)
                .AddQueryParameter("archiveStatus", ArchiveStatus.PermanentlyDelete.ToString("D")));

            //assert
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode,
                $"Status code does not match. Team {teamUid} was deleted");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("PartnerAdmin")]
        public async Task Team_Delete_NotFound()
        {
            //arrange
            var client = await GetAuthenticatedClient();
            var team = TeamFactory.GetValidPostTeam("GetValidPostTeam_");

            //create team
            var teamResponse = await client.PostAsync<TeamResponse>(RequestUris.Teams(), team);
            teamResponse.EnsureSuccess();

            var teamUId = teamResponse.Dto.Uid;

            //delete team
            var queryString = new Dictionary<string, object>
            {
                {"archiveStatus", ArchiveStatus.PermanentlyDelete.ToString("D")}
            };

            var firstDelete = await client.DeleteAsync<TeamResponse>(
                RequestUris.DeleteTeam(teamUId).AddQueryParameter(queryString));
            firstDelete.EnsureSuccess();

            //try to delete same team
            var secondDelete = await client.DeleteAsync<TeamResponse>(
                RequestUris.DeleteTeam(teamUId).AddQueryParameter(queryString));

            //get all teams
            var listOfTeams = await client.GetAsync<IList<TeamProfileResponse>>(RequestUris.Teams());

            //assert
            Assert.AreEqual(HttpStatusCode.NotFound, secondDelete.StatusCode,
                "Status code does not match");
            Assert.IsFalse(listOfTeams.Dto.Any(dto => dto.Uid == teamUId),
                $"Team {teamUId} was not archived");

        }

        //400
        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("PartnerAdmin")]
        public async Task Team_Delete_BadRequest()
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
            var response = await client.DeleteAsync<IList<string>>(RequestUris.TeamUpdate(teamUid), updatedTeam);

            // assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Response Status Code doesn't match");
            Assert.AreEqual("'TeamArchiveStatus' is not valid", response.Dto[0], "Error message doesn't match");
        }
    }
}
