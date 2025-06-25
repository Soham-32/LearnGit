using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AtCommon.ObjectFactories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AtCommon.Api;
using AtCommon.Dtos.Teams;

namespace ApiTests.v1.Endpoints.Teams
{
    [TestClass]
    [TestCategory("Teams"), TestCategory("Public")]
    public class AddTeamsTests : BaseV1Test
    {
        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member")]
        public async Task Teams_Post_MissingToken_Unauthorized()
        {
            //given
            var client = GetUnauthenticatedClient();

            var team = TeamFactory.GetValidPostTeam("GetValidPostTeam_");

            //when
            var response = await client.PostAsync(RequestUris.Teams(), team.ToStringContent());

            //then
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Response Status Code does not match.");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]

        public async Task Teams_Post_Team_Created()
        {
            //given
            var client = await GetAuthenticatedClient();

            var team = TeamFactory.GetValidPostTeam("GetValidPostTeam_");

            //when
            var response = await client.PostAsync<TeamResponse>(RequestUris.Teams(), team);

            //then
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode, "Response Status Code does not match.");
            Assert.AreEqual(team.Name, response.Dto.Name, "Name does not match");
            Assert.AreEqual("Team", response.Dto.Type, "Type does not match");
            Assert.IsTrue(response.Dto.Subteams != null, "Subteams does not exist");
            Assert.IsTrue(!string.IsNullOrWhiteSpace(response.Dto.Uid.ToString()), "Uid is null or empty");

        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]

        public async Task Teams_Post_MultiTeam_Created()
        {
            //given
            var client = await GetAuthenticatedClient();

            var team = TeamFactory.GetValidPostMultiTeam("GetValidPostMultiTeam_");

            //when
            var response = await client.PostAsync<TeamResponse>(RequestUris.Teams(), team);

            //then
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode, "Response Status Code does not match.");
            Assert.AreEqual(team.Name, response.Dto.Name, "Name does not match");
            Assert.AreEqual("MultiTeam", response.Dto.Type, "Type does not match");
            Assert.IsTrue(!string.IsNullOrWhiteSpace(response.Dto.Uid.ToString()), "Uid is null or empty");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]

        public async Task Teams_Post_EnterpriseTeam_Created()
        {
            //given
            var client = await GetAuthenticatedClient();

            var team = TeamFactory.GetValidPostEnterpriseTeam("GetValidPostEnterpriseTeam");

            //when
            var response = await client.PostAsync<TeamResponse>(RequestUris.Teams(), team);

            //then
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode, "Response Status Code does not match.");
            Assert.AreEqual(team.Name, response.Dto.Name, "Name does not match");
            Assert.AreEqual("Enterprise", response.Dto.Type, "Type does not match");
            Assert.IsTrue(!string.IsNullOrWhiteSpace(response.Dto.Uid.ToString()), "Uid is null or empty");
        }

        [TestMethod]
        [TestCategory("OrgLeader"), TestCategory("Member")]

        public async Task Teams_Post_NoPermission_Forbidden()
        {
            //given
            var client = await GetAuthenticatedClient();

            var team = TeamFactory.GetValidPostTeam("GetValidPostTeam_");

            //when
            var response = await client.PostAsync(RequestUris.Teams(), team.ToStringContent());

            //then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Response Status Code does not match.");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("SiteAdmin")]
        public async Task Teams_Post_MissingName_BadRequest()
        {
            //given
            var client = await GetAuthenticatedClient();

            var team = TeamFactory.GetValidPostTeam("GetValidPostTeam_");
            team.Name = null;

            //when
            var response = await client.PostAsync<IList<string>>(RequestUris.Teams(), team);

            //then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Response Status Code does not match.");
            Assert.IsTrue(response.Dto.Contains("'Team Name' should not be empty."));
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public async Task Teams_Post_TeamWithMember_Created()
        {
            //given
            var client = await GetAuthenticatedClient();

            var teamWithMember = TeamFactory.GetValidPostTeamWithMember("GetValidPostTeamWithMember_");

            //when
            var response = await client.PostAsync<TeamResponse>(RequestUris.Teams(), teamWithMember);

            //then
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode, "Response status code does not match.");
            Assert.AreEqual(teamWithMember.Name, response.Dto.Name, "Team Name does not match");
            Assert.AreEqual("Team", response.Dto.Type, "Team Type does not match");
            Assert.IsTrue(response.Dto.Subteams != null, "Subteams does not exist");
            Assert.IsTrue(teamWithMember.Members.Any() , "There is no information for member");
            Assert.IsTrue(teamWithMember.Stakeholders.Any(), "There is no information for stakeholder");
            Assert.IsTrue(response.Dto.Uid.CompareTo(Guid.Empty) != 0, "Uid is empty");
            Assert.AreEqual(teamWithMember.Members.First().FirstName, response.Dto.Members.First().FirstName, "Member first name does not match");
            Assert.AreEqual(teamWithMember.Members.First().LastName, response.Dto.Members.First().LastName, "Member last name does not match");
            Assert.AreEqual(teamWithMember.Members.First().Email, response.Dto.Members.First().Email, "Member email does not match");
            Assert.IsTrue(!string.IsNullOrWhiteSpace(response.Dto.Members.First().Hash), "Hash is empty");
            Assert.IsTrue(response.Dto.Members.First().Uid.CompareTo(Guid.Empty) != 0, "Uid is empty");
            Assert.AreEqual(teamWithMember.Stakeholders.First().FirstName, response.Dto.Stakeholders.First().FirstName, "Stakeholders first name does not match");
            Assert.AreEqual(teamWithMember.Stakeholders.First().LastName, response.Dto.Stakeholders.First().LastName, "Stakeholders last name does not match");
            Assert.AreEqual(teamWithMember.Stakeholders.First().Email, response.Dto.Stakeholders.First().Email, "Stakeholders email does not match");
            Assert.IsTrue(!string.IsNullOrWhiteSpace(response.Dto.Stakeholders.First().Hash), "Hash is empty");
            Assert.IsTrue(response.Dto.Stakeholders.First().Uid.CompareTo(Guid.Empty) != 0, "Uid is empty");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("SiteAdmin")]
        public async Task Teams_Post_TeamWithMember_MissingMemberFirstName_BadRequest()
        {
            //given
            var client = await GetAuthenticatedClient();

            var teamWithMember = TeamFactory.GetValidPostTeamWithMember("GetValidPostTeamWithMember_");
            teamWithMember.Members.First().FirstName = null;

            //when
            var response = await client.PostAsync<IList<string>>(RequestUris.Teams(), teamWithMember);

            //then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Response status code does not match.");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("SiteAdmin")]
        public async Task Teams_Post_TeamWithMember_MissingStakeholderEmail_BadRequest()
        {
            //given
            var client = await GetAuthenticatedClient();

            var teamWithMember = TeamFactory.GetValidPostTeamWithMember("GetValidPostTeamWithMember_");
            teamWithMember.Stakeholders.First().Email = null;

            //when
            var response = await client.PostAsync<IList<string>>(RequestUris.Teams(), teamWithMember);

            //then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Response status code does not match.");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("SiteAdmin")]
        public async Task Teams_Post_TeamWithMember_Unauthorized()
        {
            //given
            var client = GetUnauthenticatedClient();

            var teamWithMember = TeamFactory.GetValidPostTeamWithMember("GetValidPostTeamWithMember_");

            //when
            var response = await client.PostAsync(RequestUris.Teams(), teamWithMember.ToStringContent());

            //then
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Response status code does not match.");
        }

        [TestMethod]
        [TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Teams_Post_TeamWithMember_Forbidden()
        {
            //given
            var client = await GetAuthenticatedClient();
            var teamWithMember = TeamFactory.GetValidPostTeamWithMember("GetValidPostTeamWithMember_");

            //when
            var response = await client.PostAsync<IList<string>>(RequestUris.Teams(), teamWithMember);

            //then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Response status code does not match.");
           
        }
    }
}