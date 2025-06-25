using AtCommon.Api;
using AtCommon.Dtos.Teams;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ApiTests.v1.Endpoints.Teams
{
    [TestClass]
    [TestCategory("Teams")]
    public class GetTeamExportTests : BaseV1Test
    {
        private static IList<TeamProfileResponse> _teamProfileResponse;
        private static readonly UserConfig CompanyAdminUserConfig = new UserConfig("CA");


        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            var user = User;
            if (User.IsSiteAdmin() || User.IsPartnerAdmin())
            {
                user = CompanyAdminUserConfig.GetUserByDescription("user 1");
            }

            var setupApi = new SetupTeardownApi(TestEnvironment);
            _teamProfileResponse = setupApi.GetTeamProfileResponse(SharedConstants.RadarTeam, user);
        }

        //200
        [TestMethod]
        [TestCategory("KnownDefect")] // Bug Id : 43969
        [TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        [TestCategory("CompanyAdmin"), TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public async Task Teams_Get_Export_Success()
        {
            //Given
            var client = await GetAuthenticatedClient();

            //When
            var response = await client.GetAsync<TeamExportResponse>(RequestUris.TeamExport(_teamProfileResponse.First().Uid));

            //Then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Response Status Code does not match.");
            Assert.AreEqual(_teamProfileResponse.First().Name, response.Dto.Name, "Team name does not match.");
            Assert.AreEqual(_teamProfileResponse.First().ExternalIdentifier, response.Dto.ExternalIdentifier, "External identifier does not match.");
            Assert.AreEqual(_teamProfileResponse.First().Description, response.Dto.Description, "Description does not match.");
            Assert.AreEqual(_teamProfileResponse.First().AgileAdoptionDate, response.Dto.AgileAdoptionDate, "Agile adoption date does not match.");
            Assert.AreEqual(_teamProfileResponse.First().FormationDate, response.Dto.FormationDate, "Formation date does not match.");
            Assert.AreEqual(_teamProfileResponse.First().Department, response.Dto.Department, "Department does not match.");
            Assert.AreEqual(_teamProfileResponse.First().Bio, response.Dto.Bio, "Team Bio does not match.");
        }

        //400
        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        [TestCategory("CompanyAdmin"), TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("Member")]
        public async Task Teams_Get_Export_BadRequest()
        {
            //Given
            var client = await GetAuthenticatedClient();
            var emptyUid = Guid.Empty;

            //When
            var response = await client.GetAsync(RequestUris.TeamExport(emptyUid));

            //Then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Response Status Code does not match.");
        }

        //401
        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        [TestCategory("CompanyAdmin"), TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public async Task Teams_Get_Export_Unauthorized()
        {
            //Given
            var client = GetUnauthenticatedClient();

            //When
            var response = await client.GetAsync(RequestUris.TeamExport(_teamProfileResponse.First().Uid));

            //Then
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Response Status Code does not match.");
        }

        //403
        [TestMethod]
        [TestCategory("KnownDefect")] // Bug Id : 43218
        [TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("CompanyAdmin"), TestCategory("Member")]
        public async Task Teams_Get_Export_Forbidden()
        {
            //Given
            var client = await GetAuthenticatedClient();
            var validUid = new Guid("15f48d39-88a2-e911-bcd1-00155d77ae8a");

            //When
            var response = await client.GetAsync(RequestUris.TeamExport(validUid));

            //Then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Response Status Code does not match.");
        }

        //404
        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        [TestCategory("CompanyAdmin"), TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("Member")]
        public async Task Teams_Get_Export_NotFound()
        {
            //Given
            var client = await GetAuthenticatedClient();
            var newUid = Guid.NewGuid();

            //When
            var response = await client.GetAsync(RequestUris.TeamExport(newUid));

            //Then
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode, "Response Status Code does not match.");
        }
    }
}
