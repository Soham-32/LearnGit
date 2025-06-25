using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AtCommon.ObjectFactories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AtCommon.Api;
using AtCommon.Api.Enums;
using AtCommon.Dtos.Teams;

namespace ApiTests.v1.Endpoints.Teams
{
    [TestClass]
    [TestCategory("Teams"), TestCategory("Public")]
    public class GetTeamDetailsTests : BaseV1Test
    {
        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task TeamDetails_Get_MissingToken_Unauthorized()
        {
            // arrange
            var client = GetUnauthenticatedClient();
            var invalidUid = Guid.NewGuid();

            // act
            var response = await client.GetAsync(RequestUris.TeamDetails(invalidUid));

            // assert
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode,
                "Response Status Code does not match.");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task TeamDetails_Get_OK()
        {
            // arrange
            var client = await GetAuthenticatedClient();

            var teamsResponse = await client.GetAsync<IList<TeamProfileResponse>>(RequestUris.Teams());
            teamsResponse.EnsureSuccess();

            var firstTeam = teamsResponse.Dto.First();

            // act
            var response = await client.GetAsync<TeamProfileResponse>(
                RequestUris.TeamDetails(firstTeam.Uid));

            // assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode,
                "Response Status Code does not match.");
            Assert.AreEqual(firstTeam.Uid, response.Dto.Uid, "Uid does not match");
            Assert.AreEqual(firstTeam.Name, response.Dto.Name, "Name does not match");
            Assert.AreEqual(firstTeam.Type, response.Dto.Type, "Type does not match");
            Assert.AreEqual(firstTeam.TeamArchiveStatusId, (int)ArchiveStatus.Active,
                "Team Archive Status ID should be 1");
            Assert.AreEqual(firstTeam.TeamArchiveStatus, "Active",
                "Team Archive Status should be Active");
            Assert.AreEqual(firstTeam.CreatedAt, response.Dto.CreatedAt,
                "Created At is null or empty");
            Assert.IsTrue(firstTeam.DeletedAt == null, "Deleted At is not null or empty");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("PartnerAdmin")]
        public async Task TeamDetails_Get_MultiTeam_OK()
        {
            // arrange
            var client = await GetAuthenticatedClient();
            var team = TeamFactory.GetValidPostMultiTeam("GetValidPostMultiTeam_");

            //create team
            var teamResponse = await client.PostAsync<TeamResponse>(RequestUris.Teams(), team);
            teamResponse.EnsureSuccess();

            //get team
            var teamDetail = await client.GetAsync<TeamProfileResponse>(RequestUris.TeamDetails(teamResponse.Dto.Uid));
            teamDetail.EnsureSuccess();

            // assert
            Assert.AreEqual(HttpStatusCode.OK, teamDetail.StatusCode,
                "Response Status Code does not match.");
            Assert.AreEqual(teamResponse.Dto.Uid, teamDetail.Dto.Uid, "Uid does not match");
            Assert.AreEqual(teamResponse.Dto.Name, teamDetail.Dto.Name, "Name does not match");
            Assert.AreEqual(teamResponse.Dto.Type, teamDetail.Dto.Type, "Type does not match");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("PartnerAdmin")]
        public async Task TeamDetails_Get_EnterpriseTeam_OK()
        {
            // arrange
            var client = await GetAuthenticatedClient();
            var team = TeamFactory.GetValidPostEnterpriseTeam("GetValidPostEnterpriseTeam_");

            //create team
            var teamResponse = await client.PostAsync<TeamResponse>(RequestUris.Teams(), team);
            teamResponse.EnsureSuccess();

            //get team
            var teamDetail = await client.GetAsync<TeamProfileResponse>(RequestUris.TeamDetails(teamResponse.Dto.Uid));
            teamDetail.EnsureSuccess();

            // assert
            Assert.AreEqual(HttpStatusCode.OK, teamDetail.StatusCode,
                "Response Status Code does not match.");
            Assert.AreEqual(teamResponse.Dto.Uid, teamDetail.Dto.Uid, "Uid does not match");
            Assert.AreEqual(teamResponse.Dto.Name, teamDetail.Dto.Name, "Name does not match");
            Assert.AreEqual(teamResponse.Dto.Type, teamDetail.Dto.Type, "Type does not match");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("PartnerAdmin")]
        public async Task TeamDetails_Get_ByArchiveStatus_OK()
        {
            const int archiveStatusId = (int)ArchiveStatus.TeamDisbanded;
            // arrange
            var client = await GetAuthenticatedClient();
            var team = TeamFactory.GetValidPostTeam("GetValidPostTeam_");

            //create team
            var teamResponse = await client.PostAsync<TeamResponse>(RequestUris.Teams(), team);
            teamResponse.EnsureSuccess();

            //delete team
            var teamDeleteResponse = await client.DeleteAsync<TeamResponse>(
                RequestUris.DeleteTeam(teamResponse.Dto.Uid).AddQueryParameter("archiveStatus", archiveStatusId));
            teamDeleteResponse.EnsureSuccess();


            // act
            var response = await client.GetAsync<TeamProfileResponse>(
                RequestUris.TeamDetails(teamResponse.Dto.Uid));

            // assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode,
                "Response Status Code does not match.");
            Assert.AreEqual(response.Dto.Name, teamResponse.Dto.Name, "Name does not match");
            Assert.AreEqual(response.Dto.Type, teamResponse.Dto.Type, "Type does not match");
            Assert.AreEqual(response.Dto.TeamArchiveStatusId, archiveStatusId,
                "TeamArchiveStatusId does not match");
            Assert.AreEqual(response.Dto.TeamArchiveStatus, "Archive - Team Disbanded",
                "Team Archive Status does not match");
            Assert.AreEqual(response.Dto.Uid, teamResponse.Dto.Uid, "Uid does not match");
            Assert.IsNotNull(response.Dto.CreatedAt, "CreatedAt is null or empty");
            Assert.IsTrue(response.Dto.DeletedAt != null, "DeletedAt is null or empty");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task TeamDetails_Get_InvalidUid_Forbidden()
        {
            // arrange
            var client = await GetAuthenticatedClient();

            var invalidUid = Guid.NewGuid();

            // act
            var response = await client.GetAsync(RequestUris.TeamDetails(invalidUid));

            // assert
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode,
                "Response Status Code does not match.");
        }
 
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public async Task TeamDetails_Get_InvalidUid_NotFound()
        {
            // arrange
            var client = await GetAuthenticatedClient();

            // act
            var response = await client.GetAsync(RequestUris.TeamDetails(Guid.NewGuid()));

            // assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode,
                "Response Status Code does not match.");
        }
    }
}