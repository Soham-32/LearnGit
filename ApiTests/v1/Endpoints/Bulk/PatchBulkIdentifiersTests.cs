using System;
using System.Net;
using System.Threading.Tasks;
using AtCommon.Api;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiTests.v1.Endpoints.Bulk
{
    [TestClass]
    [TestCategory("Bulk"), TestCategory("Public")]
    public class PatchBulkIdentifiersTests : BaseV1Test
    {
        // 200-201
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Bulk_Identifiers_Patch_OK()
        {
            // given
            // add a new team without an ExternalIdentifier
            var companyAdmin = new UserConfig("CA").GetUserByDescription("user 1");
            var caClient = await ClientFactory.GetAuthenticatedClient(companyAdmin.Username, 
                companyAdmin.Password, TestEnvironment.EnvironmentName);
            
            var team = TeamFactory.GetNormalTeam("bulk");
            var createTeamResponse = await caClient.PostAsync<TeamResponse>(RequestUris.Teams(), team);
            createTeamResponse.EnsureSuccess();

            var client = await GetAuthenticatedClient();

            // when
            var response = await client.PatchAsync(RequestUris.BulkIdentifiers(Company.Id));

            // then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, 
                "Status code doesn't match.");

            var teamProfileResponse = await caClient.GetAsync<TeamProfileResponse>(RequestUris.TeamDetails(createTeamResponse.Dto.Uid));
            Assert.IsTrue(Guid.TryParse(teamProfileResponse.Dto.ExternalIdentifier, out _), 
                $"ExternalIdentifier <{teamProfileResponse.Dto.ExternalIdentifier}> is not a valid Guid.");
        }
        
        // 401
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Bulk_Identifiers_Patch_Unauthorized()
        {
            // given
            var client = GetUnauthenticatedClient();

            // when
            var response = await client.PatchAsync(RequestUris.BulkIdentifiers(Company.Id));

            // then
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, 
                "Status code doesn't match.");
        }

        // 403
        [TestMethod]
        [TestCategory("BLAdmin"), TestCategory("TeamAdmin"), TestCategory("OrgLeader")]
        [TestCategory("Member")]
        public async Task Bulk_Identifiers_Patch_Role_Forbidden()
        {
            // given
            var client = await GetAuthenticatedClient();

            // when
            var response = await client.PatchAsync(RequestUris.BulkIdentifiers(Company.Id));

            // then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, 
                "Status code doesn't match.");
        }

        [TestMethod]
        [TestCategory("BLAdmin"), TestCategory("TeamAdmin"), TestCategory("OrgLeader")]
        [TestCategory("Member")]
        public async Task Bulk_Identifiers_Patch_CompanyId_Forbidden()
        {
            // given
            var client = await GetAuthenticatedClient();

            // when
            var response = await client.PatchAsync(RequestUris.BulkIdentifiers(2));

            // then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, 
                "Status code doesn't match.");
        }

        // 404
        [TestMethod]
        [TestCategory("SiteAdmin")]
        public async Task Bulk_Identifiers_Patch_NotFound()
        {
            // given
            var client = await GetAuthenticatedClient();

            // when
            var response = await client.PatchAsync(RequestUris.BulkIdentifiers(SharedConstants.FakeCompanyId));

            // then
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode, "Status code doesn't match.");
        }
    }
}