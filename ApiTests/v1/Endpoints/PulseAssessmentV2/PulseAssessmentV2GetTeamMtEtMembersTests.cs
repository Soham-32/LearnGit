using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AtCommon.Api;
using AtCommon.Dtos.Assessments.PulseV2;
using AtCommon.Dtos.Companies;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiTests.v1.Endpoints.PulseAssessmentV2
{
    [TestClass]
    [TestCategory("PulseAssessmentsV2")]
    public class PulseAssessmentV2GetTeamMtEtMembersTests : PulseApiTestBase
    {
        private static int _teamId;
        private static int _updateTeamId;
        private static int _multiTeamId;
        private static int _enterpriseTeamId;
        private static TeamV2Response _expectedTeamResponse;
        private static TeamV2Response _expectedUpdatedTeamResponse;
        private static readonly UserConfig CompanyAdminUserConfig = new UserConfig("CA");
        private static readonly List<string> IgnoredProperties = new List<string>()
        {
            "ExternalIdentifier",
            "DeletedAt"
        };

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            var user = User;
            if (User.IsSiteAdmin() || User.IsPartnerAdmin())
            {
                user = CompanyAdminUserConfig.GetUserByDescription("user 1");
            }

            var setupApi = new SetupTeardownApi(TestEnvironment);

            //TeamResponse1
            _teamId = setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(SharedConstants.Team).TeamId;
            var teamWithTeamMemberResponses = setupApi.GetTeamWithTeamMemberResponse(_teamId,user);
            _expectedTeamResponse = new TeamV2Response
            {
                TeamId = _teamId,
                Name = teamWithTeamMemberResponses.First().Name,
                SelectedParticipants = teamWithTeamMemberResponses.First().SelectedParticipants,
                Uid = teamWithTeamMemberResponses.First().Uid,
                TeamType = teamWithTeamMemberResponses.First().TeamType
            };

            //TeamResponse2
            _updateTeamId = setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(SharedConstants.UpdateTeam).TeamId;
            teamWithTeamMemberResponses = setupApi.GetTeamWithTeamMemberResponse(_updateTeamId,user);
            _expectedUpdatedTeamResponse = new TeamV2Response
            {
                TeamId =_updateTeamId,
                Name = teamWithTeamMemberResponses.First().Name,
                SelectedParticipants = teamWithTeamMemberResponses.First().SelectedParticipants,
                Uid = teamWithTeamMemberResponses.First().Uid,
                TeamType = teamWithTeamMemberResponses.First().TeamType
            };

            //MultiTeamResponse
            _multiTeamId = setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(SharedConstants.MultiTeam).TeamId;

            //EnterpriseTeamResponse
            _enterpriseTeamId = setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(SharedConstants.EnterpriseTeam).TeamId;
        }

        //200
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public async Task PulseAssessmentV2_Get_TeamMembers_Success()
        {
            //Given
            var client = await GetAuthenticatedClient();

            //When
            var teamMemberResponse = await client.GetAsync<IList<TeamV2Response>>(RequestUris.PulseAssessmentV2TeamMtEtMembers(_teamId).AddQueryParameter("companyId", Company.Id));

            //then
            Assert.AreEqual(HttpStatusCode.OK, teamMemberResponse.StatusCode, "Status Code doesn't match");

            Assert.That.ResponseAreEqual(_expectedTeamResponse, teamMemberResponse.Dto.First(), IgnoredProperties);
        }

        //200
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public async Task PulseAssessmentV2_Get_MultiTeamMembers_Success()
        {
            //Given
            var client = await GetAuthenticatedClient();
            var expectedTeamMemberResponse = new List<TeamV2Response>()
            {
                _expectedTeamResponse,
                _expectedUpdatedTeamResponse
            };
            
            //When
            var multiTeamMemberResponse = await client.GetAsync<IList<TeamV2Response>>(RequestUris.PulseAssessmentV2TeamMtEtMembers(_multiTeamId).AddQueryParameter("companyId", Company.Id));

            //Then
            Assert.AreEqual(HttpStatusCode.OK, multiTeamMemberResponse.StatusCode, "Status Code doesn't match");
            Assert.That.ResponseAreEqual(expectedTeamMemberResponse, multiTeamMemberResponse.Dto, IgnoredProperties);
        }

        //200
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public async Task PulseAssessmentV2_Get_EnterpriseTeamMembers_Success()
        {
            //Given
            var client = await GetAuthenticatedClient();

            //When
            var enterpriseTeamMemberResponse = await client.GetAsync<IList<TeamV2Response>>(RequestUris.PulseAssessmentV2TeamMtEtMembers(_enterpriseTeamId).AddQueryParameter("companyId", Company.Id));

            //Then
            Assert.AreEqual(HttpStatusCode.OK, enterpriseTeamMemberResponse.StatusCode, "Status Code doesn't match");

            Assert.That.ResponseAreEqual(_expectedUpdatedTeamResponse, enterpriseTeamMemberResponse.Dto.First(a => a.TeamId == _updateTeamId), IgnoredProperties);
            Assert.That.ResponseAreEqual(_expectedTeamResponse, enterpriseTeamMemberResponse.Dto.First(a => a.TeamId == _teamId), IgnoredProperties);
        }

        //400
        [TestMethod]
        [TestCategory("SiteAdmin")]
        public async Task PulseAssessmentV2_Get_TeamMtEtMembers_WithInvalidCompanyId_BadRequest()
        {
            //Given
            var client = await GetAuthenticatedClient();

            //When
            var response = await client.GetAsync<IList<string>>(RequestUris.PulseAssessmentV2TeamMtEtMembers(_teamId).AddQueryParameter("companyId", 0000));

            //Then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status Code doesn't match");
            Assert.AreEqual("'Company Id' is not valid", response.Dto.First(), "Error message doesn't match");
        }

        //400
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public async Task PulseAssessmentV2_Get_TeamMtEtMembers_WithInvalidTeamId_BadRequest()
        {
            //Given
            var client = await GetAuthenticatedClient();

            //When
            var response = await client.GetAsync<IList<string>>(RequestUris.PulseAssessmentV2TeamMtEtMembers(0000).AddQueryParameter("companyId", Company.Id));

            //Then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status Code doesn't match");
            Assert.AreEqual("'Team Id' is not valid", response.Dto.First(), "Error message doesn't match");
        }

        //401
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public async Task PulseAssessmentV2_Get_TeamMtEtMembers_Unauthorized()
        {
            //Given
            var client = GetUnauthenticatedClient();

            //When
            var response = await client.GetAsync<IList<string>>(RequestUris.PulseAssessmentV2TeamMtEtMembers(_teamId).AddQueryParameter("companyId", Company.Id));

            //Then
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status Code doesn't match");
        }

        //403
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public async Task PulseAssessmentV2_Get_TeamMtEtMembers_WithFakeCompanyId_Forbidden()
        {
            //Given
            var client = await GetAuthenticatedClient();

            //When
            var response = await client.GetAsync<IList<string>>(RequestUris.PulseAssessmentV2TeamMtEtMembers(_teamId).AddQueryParameter("companyId", 3));

            //Then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status Code doesn't match");
        }

        //403
        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public async Task PulseAssessmentV2_Get_TeamMtEtMembers_With_InvalidTeamId_Forbidden()
        {
            //Given
            var client = await GetAuthenticatedClient();

            //When
            var response = await client.GetAsync<IList<string>>(RequestUris.PulseAssessmentV2TeamMtEtMembers(9999).AddQueryParameter("companyId", Company.Id));

            //Then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status Code doesn't match");
        }

    }
}


