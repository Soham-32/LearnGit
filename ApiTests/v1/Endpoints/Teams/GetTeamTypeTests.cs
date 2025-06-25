using AtCommon.Api;
using AtCommon.Dtos.Teams;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ApiTests.v1.Endpoints.Teams
{
    [TestClass]
    [TestCategory("Teams"), TestCategory("Public")]
    public class GetTeamTypeTests : BaseV1Test
    {
        public int FakeTeamId = 555;
        private static TeamProfileResponse _expectedTeamResponse;
        private static int _teamId;

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            var client = ClientFactory.GetAuthenticatedClient(User.Username, User.Password, TestEnvironment.EnvironmentName).GetAwaiter().GetResult();
            var teamResponse = client.GetAsync<IList<TeamProfileResponse>>(RequestUris.Teams().AddQueryParameter("companyId", Company.Id)).GetAwaiter().GetResult();
            _expectedTeamResponse = teamResponse.Dto.Where(a => a.Name.Equals(SharedConstants.Team)).ToList().FirstOrDefault();
            // ReSharper disable once PossibleNullReferenceException
            _teamId = _expectedTeamResponse.TeamId;
        }

        //200
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("Member")]
        [TestCategory("OrgLeader"), TestCategory("PartnerAdmin")]
        public async Task Teams_Get_TeamType_Success()
        {
            //given
            var client = await GetAuthenticatedClient();
            var queries = new Dictionary<string, object>
            {
                {"companyId",Company.Id},
                {"teamId", _teamId }
            };

            //when
            var response = await client.GetAsync<TeamProfileResponse>(RequestUris.TeamType().AddQueryParameter(queries));

            //then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Response Status Code doesn't match.");
            Assert.AreEqual(_expectedTeamResponse.Name, response.Dto.Name, "Team name doesn't match");
            Assert.AreEqual(_expectedTeamResponse.TeamId, response.Dto.TeamId, "Team Id doesn't match");
            Assert.AreEqual(_expectedTeamResponse.Type, response.Dto.Type, "Team type doesn't match");
            Assert.AreEqual(_expectedTeamResponse.CreatedAt, response.Dto.CreatedAt, "Team creation date doesn't match");
            Assert.AreEqual(_expectedTeamResponse.Uid, response.Dto.Uid, "Team Uid doesn't match");
            Assert.AreEqual(_expectedTeamResponse.TeamArchiveStatus, response.Dto.TeamArchiveStatus, "TeamArchiveStatus doesn't match");
            Assert.AreEqual(_expectedTeamResponse.TeamArchiveStatusId, response.Dto.TeamArchiveStatusId, "TeamArchiveStatusId doesn't match");
            Assert.AreEqual(_expectedTeamResponse.ExternalIdentifier, response.Dto.ExternalIdentifier, "ExternalIdentifier doesn't match");
        }

        //400
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("Member")]
        [TestCategory("OrgLeader"), TestCategory("PartnerAdmin")]
        public async Task Teams_Get_TeamType_NoCompanyId_NoTeamId_BadRequest()
        {
            //given
            var client = await GetAuthenticatedClient();

            //when
            var response = await client.GetAsync<List<string>>(RequestUris.TeamType());

            //then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Response Status Code doesn't match.");
            Assert.That.ListContains(response.Dto, "'CompanyId' must be greater than 0");
            Assert.That.ListContains(response.Dto, "'TeamId' must be greater than 0");
        }

        //400
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("Member")]
        [TestCategory("OrgLeader"), TestCategory("PartnerAdmin")]
        public async Task Teams_Get_TeamType_ValidCompanyId_BadRequest()
        {
            //given
            var client = await GetAuthenticatedClient();

            //when
            var response = await client.GetAsync<List<string>>(RequestUris.TeamType().AddQueryParameter("companyId", Company.Id));

            //then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Response Status Code doesn't match.");
            Assert.AreEqual(response.Dto.FirstOrDefault(), "'TeamId' must be greater than 0");
        }

        //400
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("Member")]
        [TestCategory("OrgLeader"), TestCategory("PartnerAdmin")]
        public async Task Teams_Get_TeamType_ValidTeamId_BadRequest()
        {
            //given
            var client = await GetAuthenticatedClient();

            //when
            var response = await client.GetAsync<List<string>>(RequestUris.TeamType().AddQueryParameter("teamId", _teamId));

            //then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Response Status Code doesn't match.");
            Assert.AreEqual(response.Dto.FirstOrDefault(), "'CompanyId' must be greater than 0");
        }

        //401
        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member")]
        public async Task Teams_Get_TeamType_Unauthorized()
        {
            // arrange
            var client = GetUnauthenticatedClient();

            // act
            var response = await client.GetAsync(RequestUris.Teams());

            // assert
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Response Status Code does not match.");
        }

        //403
        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("Member")]
        [TestCategory("OrgLeader"), TestCategory("PartnerAdmin")]
        public async Task Teams_Get_TeamType_FakeCompanyIdAndTeamId_Forbidden()
        {
            //given
            var client = await GetAuthenticatedClient();
            var queries = new Dictionary<string, object>
            {
                {"companyId",SharedConstants.FakeCompanyId},
                {"teamId",FakeTeamId}
            };

            //when
            var response = await client.GetAsync<TeamProfileResponse>(RequestUris.TeamType().AddQueryParameter(queries));

            //then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Response Status Code doesn't match.");
            Assert.AreEqual(null, response.Dto, "Response Dto is not null");
        }

        //403
        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("Member")]
        [TestCategory("OrgLeader"), TestCategory("PartnerAdmin")]
        public async Task Teams_Get_TeamType_FakeCompanyIdWithValidTeamId_Forbidden()
        {
            //given
            var client = await GetAuthenticatedClient();
            var queries = new Dictionary<string, object>
            {
                {"companyId",SharedConstants.FakeCompanyId},
                {"teamId",_teamId}
            };

            //when
            var response = await client.GetAsync<TeamProfileResponse>(RequestUris.TeamType().AddQueryParameter(queries));

            //then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Response Status Code doesn't match.");
            Assert.AreEqual(null, response.Dto, "Response Dto is not null");
        }

        //404
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("Member")]
        [TestCategory("OrgLeader"), TestCategory("PartnerAdmin")]
        public async Task Teams_Get_TeamType_InvalidTeamId_NotFound()
        {
            //given
            var client = await GetAuthenticatedClient();
            var queries = new Dictionary<string, object>
            {
                {"companyId",Company.Id},
                {"teamId", FakeTeamId }
            };

            //when
            var response = await client.GetAsync<TeamProfileResponse>(RequestUris.TeamType().AddQueryParameter(queries));

            //then
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode, "Response Status Code doesn't match.");
            Assert.AreEqual(null, response.Dto, "Response Dto is not null");
        }
    }
}
