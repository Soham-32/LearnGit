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
    public class GetTeamsByIdTests : BaseV1Test
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
        [TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        [TestCategory("CompanyAdmin"), TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public async Task Teams_Get_TeamById_Success()
        {
            //Given
            var client = await GetAuthenticatedClient();

            //When
            var response = await client.GetAsync<TeamProfileResponse>(RequestUris.Teams(_teamProfileResponse.First().Uid));

            //Then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Response Status Code does not match.");
            Assert.AreEqual(_teamProfileResponse.First().Name, response.Dto.Name, "Team name does not match.");
            Assert.AreEqual(_teamProfileResponse.First().ExternalIdentifier, response.Dto.ExternalIdentifier, "External identifier does not match.");
        }

        //401
        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        [TestCategory("CompanyAdmin"), TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public async Task Teams_Get_TeamById_Unauthorized()
        {
            //Given
            var client = GetUnauthenticatedClient();

            //When
            var response = await client.GetAsync(RequestUris.Teams(_teamProfileResponse.First().Uid));

            //Then
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Response Status Code does not match.");
        }

        //403
        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Teams_Get_TeamById_Forbidden()
        {
            //Given
            var client = await GetAuthenticatedClient();
            var emptyUid = Guid.Empty;

            //When
            var response = await client.GetAsync(RequestUris.Teams(emptyUid));

            //Then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Response Status Code does not match.");
        }

        //403
        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Teams_Get_TeamById_NewUId_Forbidden()
        {
            //Given
            var client = await GetAuthenticatedClient();
            var newUid = Guid.NewGuid();

            //When
            var response = await client.GetAsync(RequestUris.Teams(newUid));

            //Then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Response Status Code does not match.");
        }

        //404
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public async Task Teams_Get_TeamById_NotFound()
        {
            //Given
            var client = await GetAuthenticatedClient();
            var emptyUid = Guid.Empty;

            //When
            var response = await client.GetAsync(RequestUris.Teams(emptyUid));

            //Then
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode, "Response Status Code does not match.");
        }

        //404
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public async Task Teams_Get_TeamById_NewUId_NotFound()
        {
            //Given
            var client = await GetAuthenticatedClient();
            var newUid = Guid.NewGuid();

            //When
            var response = await client.GetAsync(RequestUris.Teams(newUid));

            //Then
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode, "Response Status Code does not match.");
        }
    }
}
