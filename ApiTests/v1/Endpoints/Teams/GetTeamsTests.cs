using AtCommon.Api;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AtCommon.Api.Enums;

namespace ApiTests.v1.Endpoints.Teams
{
    [TestClass]
    [TestCategory("Teams"), TestCategory("Public")]
    public class GetTeamsTests : BaseV1Test
    {
        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("Member"), TestCategory("OrgLeader")]
        public async Task Teams_Get_OK()
        {
            // arrange
            var client = await GetAuthenticatedClient();

            // act
            var response = await client.GetAsync<IList<TeamProfileResponse>>(RequestUris.Teams());

            // assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode,
                "Response Status Code does not match.");
            Assert.IsTrue(response.Dto.Any(dto => !string.IsNullOrWhiteSpace(dto.Name)),
                "Name is null or empty");
            Assert.IsTrue(response.Dto.Any(dto => !string.IsNullOrWhiteSpace(dto.Type)),
                "Type is null or empty");
            Assert.IsTrue(response.Dto.All(dto => dto.TeamArchiveStatusId == (int)ArchiveStatus.Active),
                "TeamArchiveStatusId should be 1");
            Assert.IsTrue(response.Dto.All(dto => dto.TeamArchiveStatus.Equals("Active")),
                "TeamArchiveStatus should be Active");
            Assert.IsTrue(response.Dto.All(dto => dto.CreatedAt.CompareTo(new DateTime()) != 0),
                "CreatedAt is null or empty");
            Assert.IsTrue(response.Dto.All(dto => dto.DeletedAt == null),
                "DeletedAt should be null or empty");
            Assert.IsTrue(response.Dto.Any(dto => !string.IsNullOrWhiteSpace(dto.Uid.ToString())),
                "Uid is null or empty");
            Assert.IsTrue(response.Dto.All(dto => dto.TeamId > 0), "Team Id is 0 or null ");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public async Task Teams_Get_ByArchivedStatus_OK()
        {
            // arrange
            var client = await GetAuthenticatedClient();
            var team = TeamFactory.GetValidPostTeam("GetValidPostTeam_");

            //create team
            var teamResponse = await client.PostAsync<TeamResponse>(RequestUris.Teams(), team);
            teamResponse.EnsureSuccess();

            //delete team
            var teamDeleteResponse = await client.DeleteAsync<TeamResponse>(
                RequestUris.DeleteTeam(teamResponse.Dto.Uid)
                    .AddQueryParameter("archiveStatus", ArchiveStatus.NoBudget.ToString("D")));
            teamDeleteResponse.EnsureSuccess();

            // act
            var response = await client.GetAsync<IList<TeamProfileResponse>>(
                RequestUris.Teams().AddQueryParameter("includeArchived", "true"));
            var archivedTeam = response.Dto.FirstOrDefault(dto => dto.Uid == teamResponse.Dto.Uid);

            // assert
            Assert.IsTrue(archivedTeam != null,
                $"The archived team with name <{teamResponse.Dto.Name}> and Uid <{teamResponse.Dto.Uid}> was not found in the response.");
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode,
                "Response Status Code does not match.");
            Assert.AreEqual(archivedTeam.Name, teamResponse.Dto.Name, "Name does not match");
            Assert.AreEqual(archivedTeam.Type, teamResponse.Dto.Type, "Type does not match");
            Assert.AreEqual(archivedTeam.TeamArchiveStatusId, (int)ArchiveStatus.NoBudget,
                "TeamArchiveStatusId does not match");
            Assert.AreEqual(archivedTeam.TeamArchiveStatus, "Archive - No Budget",
                "TeamArchiveStatus does not match");
            Assert.AreEqual(archivedTeam.Uid, teamResponse.Dto.Uid, "Uid does not match");
            Assert.IsTrue(DateTime.Compare(archivedTeam.CreatedAt, new DateTime()) != 0,"CreatedAt is null or empty");
            Assert.IsTrue(archivedTeam.DeletedAt != null, "DeletedAt is null or empty");
            Assert.IsTrue(response.Dto.All(dto => dto.TeamId > 0), "Team Id is 0 or null ");
        }

        [TestMethod]
        [TestCategory("Member"), TestCategory("OrgLeader")]
        public async Task Teams_Get_ByArchivedStatus_NoOlOrM_OK()
        {
            // arrange
            var client = await GetAuthenticatedClient();

            // act
            var response = await client.GetAsync<IList<TeamProfileResponse>>(
                RequestUris.Teams().AddQueryParameter("includeArchived", "true"));

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode,
                "Response status code does not match");
            var archivedTeam = response.Dto.Any(dto => dto.TeamArchiveStatusId != (int)ArchiveStatus.Active);
            Assert.IsFalse(archivedTeam, "OL/M is able to see archived team");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("Member"), TestCategory("OrgLeader")]
        public async Task Teams_Get_MissingToken_Unauthorized()
        {
            // arrange
            var client = GetUnauthenticatedClient();

            // act
            var response = await client.GetAsync(RequestUris.Teams());

            // assert
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode,
                "Response Status Code does not match.");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("Member")]
        public async Task Teams_Get_ByFullTeamName_OK()
        {
            // arrange
            var client = await GetAuthenticatedClient();
            var getAllResponse = await client.GetAsync<IList<TeamProfileResponse>>(RequestUris.Teams());
            var team = getAllResponse.Dto.First().Name;

            // act
            var response = await client.GetAsync<IList<TeamProfileResponse>>(
                RequestUris.Teams().AddQueryParameter("teamName", team));

            // assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode,
                "Response Status Code does not match.");
            Assert.IsTrue(response.Dto.All(dto => dto.Name.Contains(team)),
                "Name does not match searched team");
            Assert.IsTrue(response.Dto.Count >= 1,
                "Search result is empty when it should have results");
            Assert.IsTrue(response.Dto.All(dto => dto.TeamId > 0), "Team Id is 0 or null ");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("Member")]
        public async Task Teams_Get_ByPartialTeamName_OK()
        {
            // arrange
            var client = await GetAuthenticatedClient();
            var getAllResponse = await client.GetAsync<IList<TeamProfileResponse>>(RequestUris.Teams());
            var team = getAllResponse.Dto.First().Name;
            var partialTeam = team.Substring(0, 5);

            // act
            var response = await client.GetAsync<IList<TeamProfileResponse>>(
                RequestUris.Teams().AddQueryParameter("teamName", partialTeam));

            // assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode,
                "Response Status Code does not match.");
            Assert.IsTrue(response.Dto.All(dto => dto.Name.Contains(partialTeam)),
                "Name does not contain the partial search term");
            Assert.IsTrue(response.Dto.Any(dto => dto.Name == team),
                $"Search result missing {team}");
            Assert.IsTrue(response.Dto.Count > 0, $"There were no results for {partialTeam}");
            Assert.IsTrue(response.Dto.All(dto => dto.TeamId > 0), "Team Id is 0 or null ");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader")]
        public async Task Teams_Get_ByPartialMultiTeamName_OK()
        {
            // arrange
            var client = await GetAuthenticatedClient();
            var getAllResponse = await client.GetAsync<IList<TeamProfileResponse>>(RequestUris.Teams());
            var team = getAllResponse.Dto.First().Name;
            var partialTeam = team.Substring(10, 3);

            // act
            var response = await client.GetAsync<IList<TeamProfileResponse>>(
                RequestUris.Teams().AddQueryParameter("teamName", partialTeam));

            // assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode,
                "Response Status Code does not match.");
            Assert.IsTrue(response.Dto.All(dto => dto.Name.Contains(partialTeam)),
                "Name does not contain the partial search term");
            Assert.IsTrue(response.Dto.Any(dto => dto.Name == team),
                $"Search result missing {team}");
            Assert.IsTrue(response.Dto.All(dto => dto.TeamId > 0), "Team Id is 0 or null ");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("Member"), TestCategory("OrgLeader")]
        public async Task Teams_Get_ByTeamName_MissingToken_Unauthorized()
        {
            // arrange
            var client = GetUnauthenticatedClient();

            // act
            var response = await client.GetAsync(
                RequestUris.Teams().AddQueryParameter("teamName", "Automation"));

            // assert
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode,
                "Response Status Code does not match.");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("Member"), TestCategory("OrgLeader")]
        public async Task Teams_Get_ByTeamName_NonexistentTeam_OK()
        {
            // arrange
            var client = await GetAuthenticatedClient();

            //get random searchTerm
            var path = Path.GetRandomFileName();
            var searchTeam = path.Replace(".", "");

            // act
            var response = await client.GetAsync<IList<TeamProfileResponse>>(
                RequestUris.Teams().AddQueryParameter("teamName", searchTeam));

            // assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode,
                "Response Status Code does not match.");
            Assert.IsTrue(response.Dto.Count == 0,
                "Search brought back results when it was not supposed to");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("Member"), TestCategory("OrgLeader")]
        public async Task Teams_Get_ByCompanyId_OK()
        {
            // arrange
            var client = await GetAuthenticatedClient();

            // act
            var response = await client.GetAsync<IList<TeamProfileResponse>>(
                RequestUris.Teams().AddQueryParameter("companyId", Company.Id));

            // assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, $"Response Status Code does not match. Received a {response.StatusCode}");
            Assert.IsTrue(response.Dto.All(dto => dto.CompanyId == Company.Id), "List of teams do not match company id");
            Assert.IsTrue(response.Dto.All(dto => dto.TeamId > 0), "Team Id is 0 or null ");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("Member"), TestCategory("OrgLeader")]
        public async Task Teams_Get_ByCompanyId_Forbidden()
        {
            // arrange
            var client = await GetAuthenticatedClient();

            // act
            var response = await client.GetAsync(RequestUris.Teams().AddQueryParameter("companyId", "999"));

            // assert
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode,
                $"Response Status Code does not match.");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("Member"), TestCategory("OrgLeader")]
        public async Task Teams_Get_ByCompanyName_OK()
        {
            // arrange
            var client = await GetAuthenticatedClient();
            var companyName = User.CompanyName;

            // act
            var response = await client.GetAsync<IList<TeamProfileResponse>>(
                RequestUris.Teams().AddQueryParameter("companyName", companyName));

            // assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, $"Response Status Code does not match. Received a {response.StatusCode}");
            Assert.IsTrue(response.Dto.All(dto => dto.CompanyId == Company.Id), "List of teams do not match company id");
            Assert.IsTrue(response.Dto.All(dto => dto.TeamId > 0), "Team Id is 0 or null ");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("Member"), TestCategory("OrgLeader")]
        public async Task Teams_Get_ByCompanyName_NonExistent_OK()
        {
            // arrange
            var client = await GetAuthenticatedClient();

            // act
            var response = await client.GetAsync<IList<TeamProfileResponse>>(
                RequestUris.Teams().AddQueryParameter("companyName", "No Company"));

            // assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode,
                "Response Status Code does not match.");
            Assert.IsTrue(response.Dto.Count == 0, "Response did not come back empty");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("Member"), TestCategory("OrgLeader")]
        public async Task Teams_Get_TeamId_OK()
        {
            // arrange
            var client = await GetAuthenticatedClient();

            // act
            var response = await client.GetAsync<IList<TeamProfileResponse>>(
                RequestUris.Teams().AddQueryParameter("companyName", Company.TeamId1));

            // assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, $"Response Status Code does not match. Received a {response.StatusCode}");
            Assert.IsTrue(response.Dto.All(dto => dto.CompanyId == Company.Id), "List of teams do not match company id");
            Assert.IsTrue(response.Dto.All(dto => dto.TeamId > 0), "Team Id is 0 or null ");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("Member"), TestCategory("OrgLeader")]
        public async Task Teams_Get_TeamId_NonExistent_OK()
        {
            // arrange
            var client = await GetAuthenticatedClient();

            // act
            var response = await client.GetAsync<IList<TeamProfileResponse>>(
                RequestUris.Teams().AddQueryParameter("companyName", "7"));

            // assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode,
                "Response Status Code does not match.");
            Assert.IsTrue(response.Dto.Count == 0, "Response did not come back empty");
        }

    }
}