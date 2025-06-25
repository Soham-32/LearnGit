using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AtCommon.Api;
using AtCommon.Dtos.Assessments.PulseV2;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AtCommon.Dtos;
using AtCommon.Dtos.Assessments.Pulse;
using AtCommon.Dtos.Companies;

namespace ApiTests.v1.Endpoints.PulseAssessmentV2
{
    [TestClass]
    [TestCategory("PulseAssessmentsV2")]
    public class PulseAssessmentV2DeletePulseAssessmentTests : PulseApiTestBase
    {
        private static User _user;
        private static int _teamId;
        private static IList<TeamV2Response> _teamWithTeamMemberResponses;
        private static RadarQuestionDetailsV2Response _radarResponse;
        private static readonly UserConfig CompanyAdminUserConfig = new UserConfig("CA");

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            var setupApi = new SetupTeardownApi(TestEnvironment);

            _user = User;
            if (User.IsSiteAdmin() || User.IsPartnerAdmin() || User.IsMember())
            {
                _user = CompanyAdminUserConfig.GetUserByDescription("user 1");
            }

            //Get team profile 
            _teamId = setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(SharedConstants.UpdateTeam).TeamId;
            _teamWithTeamMemberResponses = setupApi.GetTeamWithTeamMemberResponse(_teamId, _user);

            //Get radar details
            _radarResponse = setupApi.GetRadarQuestionDetailsV2(Company.Id, _teamId, SharedConstants.AtTeamHealth3SurveyId, _user);
        }

        // 200
        [TestMethod]
        [TestCategory("KnownDefect")] //Bug Id: 51203
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public async Task PulseAssessmentV2_Delete_PulseAssessment_Success()
        {
            //given
            var client = await GetAuthenticatedClient();
            var pulseAddRequest = PulseV2Factory.PulseAssessmentV2AddRequest(_radarResponse, _teamWithTeamMemberResponses, _teamId);
            var pulseAssessmentResponse = await client.PostAsync<CreatePulseAssessmentResponse>(RequestUris.PulseAssessmentV2SavePulseAssessment().AddQueryParameter("companyId", Company.Id), pulseAddRequest);
            pulseAssessmentResponse.EnsureSuccess();

            var pulseAssessmentId = pulseAssessmentResponse.Dto.PulseAssessmentId;

            // when
            var response = await client.DeleteAsync(RequestUris.PulseAssessmentV2Delete(pulseAssessmentId));

            // then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status code doesn't match.");

            //Get the deleted Pulse
            var getPulseAssessmentResponse = await client.GetAsync<PulseAssessmentV2Response>(RequestUris.PulseAssessmentV2GetPulseAssessment(pulseAssessmentId).AddQueryParameter("companyId", Company.Id).AddQueryParameter("dataRequest", "PulseCheck"));
            Assert.AreEqual(HttpStatusCode.NotFound, getPulseAssessmentResponse.StatusCode, "Status code doesn't match.");
        }

        // 400
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task PulseAssessmentV2_Delete_PulseAssessment_BadRequest()
        {
            // given
            var client = await GetAuthenticatedClient();

            // when
            var response = await client.DeleteAsync<IList<string>>(RequestUris.PulseAssessmentV2Delete(00000));

            // then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status code doesn't match.");
            Assert.AreEqual("'PulseAssessmentId' must be greater than 0", response.Dto[0], "Response message does not match");

        }

        // 401
        [TestMethod]
        [TestCategory("KnownDefect")] //Bug Id: 51203
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        public async Task PulseAssessmentV2_Delete_PulseAssessment_Unauthorized()
        {
            // given
            var client = await GetAuthenticatedClient();

            var pulseAddRequest = PulseV2Factory.PulseAssessmentV2AddRequest(_radarResponse, _teamWithTeamMemberResponses, _teamId);
            var pulseAssessmentResponse = await client.PostAsync<CreatePulseAssessmentResponse>(RequestUris.PulseAssessmentV2SavePulseAssessment().AddQueryParameter("companyId", Company.Id), pulseAddRequest);
            pulseAssessmentResponse.EnsureSuccess();

            var pulseAssessmentId = pulseAssessmentResponse.Dto.PulseAssessmentId;

            client = GetUnauthenticatedClient();

            // when
            var response = await client.DeleteAsync(RequestUris.PulseAssessmentV2Delete(pulseAssessmentId));

            // then
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status code doesn't match.");
        }

        // 403
        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task PulseAssessmentV2_Delete_PulseAssessment_FromOtherCompany_Forbidden()
        {
            // given
            //login with SA
            const int companyId = 2;
            var siteAdmin = GetAdminUser();
            var saClient = await ClientFactory.GetAuthenticatedClient(siteAdmin.Username, siteAdmin.Password, TestEnvironment.EnvironmentName);
            var namesResponse = await saClient.GetAsync<IList<PulseAssessmentNameResponse>>(RequestUris.PulseAssessmentsNames.AddQueryParameter("companyId", companyId).AddQueryParameter("dataRequest","PulseCheck"));
            namesResponse.EnsureSuccess();

            var pulseAssessmentId = namesResponse.Dto.First().PulseAssessmentId;

            var client = await GetAuthenticatedClient();

            // when
            var response = await client.DeleteAsync(RequestUris.PulseAssessmentV2Delete(pulseAssessmentId));

            // then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status code doesn't match.");
        }

        // 403
        [TestMethod]
        [TestCategory("KnownDefect")] //Bug Id: 51203
        [TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task PulseAssessmentV2_Delete_PulseAssessment_Forbidden()
        {
            //Create Pulse with CA
            var client = await ClientFactory.GetAuthenticatedClient(_user.Username, _user.Password, TestEnvironment.EnvironmentName);
            var pulseAddRequest = PulseV2Factory.PulseAssessmentV2AddRequest(_radarResponse, _teamWithTeamMemberResponses, _teamId);
            var pulseAssessmentResponse = await client.PostAsync<CreatePulseAssessmentResponse>(RequestUris.PulseAssessmentV2SavePulseAssessment().AddQueryParameter("companyId", Company.Id), pulseAddRequest);
            pulseAssessmentResponse.EnsureSuccess();

            var pulseAssessmentId = pulseAssessmentResponse.Dto.PulseAssessmentId;

            client = await GetAuthenticatedClient();

            // when
            var response = await client.DeleteAsync(RequestUris.PulseAssessmentV2Delete(pulseAssessmentId));

            // then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status code doesn't match.");
        }

        // 404
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task PulseAssessmentV2_Delete_PulseAssessment_NotFound()
        {
            // given
            var client = await GetAuthenticatedClient();

            // when
            var response = await client.DeleteAsync(RequestUris.PulseAssessmentV2Delete(99999));

            // then
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode, "Status code doesn't match.");
        }

        // 404
        [TestMethod]
        [TestCategory("KnownDefect")] //Bug Id: 51203
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public async Task PulseAssessmentV2_ReDelete_PulseAssessment_NotFound()
        {
            //given
            var client = await GetAuthenticatedClient();
            var pulseAddRequest = PulseV2Factory.PulseAssessmentV2AddRequest(_radarResponse, _teamWithTeamMemberResponses, _teamId);
            var pulseAssessmentResponse = await client.PostAsync<CreatePulseAssessmentResponse>(RequestUris.PulseAssessmentV2SavePulseAssessment().AddQueryParameter("companyId", Company.Id), pulseAddRequest);
            pulseAssessmentResponse.EnsureSuccess();

            var pulseAssessmentId = pulseAssessmentResponse.Dto.PulseAssessmentId;

            //Delete Pulse
            var response = await client.DeleteAsync(RequestUris.PulseAssessmentV2Delete(pulseAssessmentId));

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status code doesn't match.");

            //ReDelete Pulse
            // when
            response = await client.DeleteAsync(RequestUris.PulseAssessmentV2Delete(pulseAssessmentId));

            // then
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode, "Status code doesn't match.");
        }
    }
}