using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AtCommon.Api;
using AtCommon.Dtos.Bulk;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiTests.v1.Endpoints.Bulk
{
    [TestClass]
    [TestCategory("Bulk"), TestCategory("Public")]
    public class PutBulkTeamsTests : BulkBaseTest
    {
        
        // 200-201
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Bulk_Teams_Put_NewTeam_Created()
        {
            // given
            var client = await GetAuthenticatedClient();
            var teams = new List<AddTeam> {TeamFactory.GetTeamForBulkImport()};
            // when
            var response = client.PutAsync(RequestUris.BulkTeams(Company.Id), 
                teams.ToStringContent()).GetAwaiter().GetResult();

            // then
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode, 
                "Status code doesn't match.");

            var caClient = GetCaClient();
            var newTeams = await caClient.GetAsync<IList<TeamProfileResponse>>(
                RequestUris.Teams());
            newTeams.EnsureSuccess();
            Assert.IsTrue(newTeams.Dto.Any(t => t.Name == teams.First().Name));
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Bulk_Teams_Put_ExistingTeam_Created()
        {
            // given
            var caClient = GetCaClient();
            var createTeamRequest = TeamFactory.GetNormalTeam("BulkTeam");
            var createTeamResponse = await caClient.PostAsync<TeamResponse>(RequestUris.Teams(), 
                createTeamRequest);
            createTeamResponse.EnsureSuccess();
            var createMultiTeamResponse = CreateMultiteam();
            var teamsResponse = await caClient.GetAsync<List<TeamProfileResponse>>(RequestUris.Teams());
            teamsResponse.EnsureSuccess();
            var multiTeamExternalId = teamsResponse.Dto.FirstOrDefault(t => t.Uid == createMultiTeamResponse.Uid)
                .CheckForNull($"<{createMultiTeamResponse.Name}> not found in the response.").ExternalIdentifier;

            var client = await GetAuthenticatedClient();
            var updatedTeam = new AddTeam
            {
                Name = createTeamRequest.Name + "Updated",
                Type = createTeamRequest.Type,
                Description = createTeamRequest.Description + "Updated",
                ExternalIdentifier = createTeamRequest.ExternalIdentifier,
                Department = createTeamRequest.Department + "Updated",
                FormationDate = DateTime.Today.AddDays(1),
                AgileAdoptionDate = DateTime.Today.AddDays(1),
                Bio = createTeamRequest.Bio + "Updated",
                Tags = new List<TeamTagRequest>
                {
                    new TeamTagRequest
                    {
                        Category = "Work Type", Tags = new List<string> { SharedConstants.NewTeamWorkType }
                    },
                    new TeamTagRequest
                    {
                        Category = "Methodology", Tags = new List<string> { "Kanban" }
                    }
                },
                ParentExternalIdentifiers = new List<string> { multiTeamExternalId }
            };

            // when
            var response = await client.PutAsync(RequestUris.BulkTeams(Company.Id), 
                new List<AddTeam> {updatedTeam}.ToStringContent());
            
            // then
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode, 
                "Status code doesn't match.");

            var newTeams = await caClient.GetAsync<IList<TeamProfileResponse>>(
                RequestUris.Teams());
            newTeams.EnsureSuccess();

            var actualTeam = newTeams.Dto.FirstOrDefault(t => t.Uid == createTeamResponse.Dto.Uid)
                .CheckForNull($"Team with Uid <{createTeamResponse.Dto.Uid}> not found in the response.");
            Assert.AreEqual(updatedTeam.Name, actualTeam.Name, "Team Name is not updated");
            Assert.AreEqual(updatedTeam.Description, actualTeam.Description, "Team Description is not updated");
            Assert.AreEqual(updatedTeam.Department, actualTeam.Department, "Team Department is not updated");
            Assert.AreEqual(updatedTeam.FormationDate, actualTeam.FormationDate, "Team Formation Date is not updated");
            Assert.AreEqual(updatedTeam.AgileAdoptionDate, actualTeam.AgileAdoptionDate, "Team Agile Adoption Date is not updated");
            Assert.AreEqual(updatedTeam.Bio, actualTeam.Bio, "Team Bio is not updated");
            foreach (var updatedTag in updatedTeam.Tags)
            {
                Assert.IsTrue(actualTeam.TeamTags.Any(tag => tag.Category == updatedTag.Category),
                    $"Tag with category <{updatedTag.Category}> is missing");
                Assert.That.ListsAreEqual(updatedTag.Tags, 
                    actualTeam.TeamTags.First(t => t.Category == updatedTag.Category).Tags.ToList());
            }
            Assert.IsTrue(actualTeam.TeamTags.Any(tag => tag.Category == "Methodology" && tag.Tags.Contains("Kanban")));
            var updatedMultiTeam = newTeams.Dto.FirstOrDefault(t => t.ExternalIdentifier == multiTeamExternalId)
                .CheckForNull($"Multiteam ExternalIdentifier <{multiTeamExternalId}> was not found in the response.");
            Assert.That.ListContains(updatedMultiTeam.Subteams.Select(g => g.ToString("D")).ToList(), 
                createTeamResponse.Dto.Uid.ToString("D"));
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Bulk_Teams_Put_NullValues_BadRequest()
        {
            // given
            var client = await GetAuthenticatedClient();
            var teams = new List<AddTeam> { new AddTeam() };
            
            // when
            var response = await client.PutAsync<List<string>>(RequestUris.BulkTeams(Company.Id), 
                teams);

            // then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, 
                "Status code doesn't match.");
            var expectedErrorList = new List<string>
            {
                "'Name' must not be empty.",
                "'Type' must not be empty.",
                "'External Identifier' must not be empty."
            };
            Assert.That.ListsAreEqual(expectedErrorList, response.Dto);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Bulk_Teams_Put_EmptyValues_BadRequest()
        {
            // given
            var client = await GetAuthenticatedClient();
            var teams = new List<AddTeam> { new AddTeam
            {
                Name = "",
                Type = "",
                ExternalIdentifier = "",
                Tags = new List<TeamTagRequest>
                {
                    new TeamTagRequest { Category = "", Tags = new List<string> { "" }}
                }
            } };
            
            // when
            var response = await client.PutAsync<List<string>>(RequestUris.BulkTeams(Company.Id), 
                teams);

            // then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, 
                "Status code doesn't match.");
            var expectedErrorList = new List<string>
            {
                "'Name' must not be empty.",
                "'Type' must not be empty.",
                "'External Identifier' must not be empty.",
                "'Category' must not be empty.",
                "'Tags' must not be empty."
            };
            Assert.That.ListsAreEqual(expectedErrorList, response.Dto);
        }

        // 401
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Bulk_Teams_Put_Unauthorized()
        {
            // given
            var client = GetUnauthenticatedClient();
            var teams = new List<AddTeam> {TeamFactory.GetTeamForBulkImport()};
            
            // when
            var response = await client.PutAsync(RequestUris.BulkTeams(Company.Id), 
                teams.ToStringContent());

            // then
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, 
                "Status code doesn't match.");
        }

        // 403
        [TestMethod]
        [TestCategory("BLAdmin"), TestCategory("TeamAdmin"), TestCategory("OrgLeader")]
        [TestCategory("Member")]
        public async Task Bulk_Teams_Put_UserRole_Forbidden()
        {
            // given
            var client = await GetAuthenticatedClient();
            var teams = new List<AddTeam> {TeamFactory.GetTeamForBulkImport()};

            // when
            var response = await client.PutAsync(RequestUris.BulkTeams(Company.Id), 
                teams.ToStringContent());

            // then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, 
                "Status code doesn't match.");
        }

        [TestMethod]
        [TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Bulk_Teams_Put_Permission_Forbidden()
        {
            // given
            var client = await GetAuthenticatedClient();
            var teams = new List<AddTeam> {TeamFactory.GetTeamForBulkImport()};

            // when
            var response = await client.PutAsync(RequestUris.BulkTeams(2), 
                teams.ToStringContent());

            // then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, 
                "Status code doesn't match.");
        }

        // 404
        [TestMethod]
        [TestCategory("SiteAdmin")]
        public async Task Bulk_Teams_Put_NotFound()
        {
            // given
            var client = await GetAuthenticatedClient();
            var teams = new List<AddTeam> {TeamFactory.GetTeamForBulkImport()};

            // when
            var response = await client.PutAsync(RequestUris.BulkTeams(SharedConstants.FakeCompanyId), teams.ToStringContent());

            // then
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode, "Status code doesn't match.");
        }
    }
}