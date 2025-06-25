using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AtCommon.Api;
using AtCommon.Dtos;
using AtCommon.Dtos.Assessments.PulseV2;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.IndividualAssessments;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiTests.v1.Endpoints.PulseAssessmentV2
{
    [TestClass]
    [TestCategory("PulseAssessmentsV2")]
    public class PulseAssessmentV2SavePulseAssessmentTests : PulseApiTestBase
    {
        private static User _user;
        private static SetupTeardownApi _setupApi;
        private static int _teamId;
        private static IList<TeamV2Response> _teamWithTeamMemberResponses;
        private static RadarQuestionDetailsV2Response _radarResponse;
        private static readonly UserConfig CompanyAdminUserConfig = new UserConfig("CA");

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            _setupApi = new SetupTeardownApi(TestEnvironment);

            _user = User;
            if (User.IsSiteAdmin() || User.IsPartnerAdmin() || User.IsMember())
            {
                _user = CompanyAdminUserConfig.GetUserByDescription("user 1");
            }

            //Get team profile 
            _teamId = _setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(SharedConstants.UpdateTeam).TeamId;
            _teamWithTeamMemberResponses = _setupApi.GetTeamWithTeamMemberResponse(_teamId,_user);
            //Get radar details
            _radarResponse = _setupApi.GetRadarQuestionDetailsV2(Company.Id, _teamId, SharedConstants.AtTeamHealth3SurveyId, _user);
        }

        //200
        [TestMethod]
        [TestCategory("KnownDefect")] //Bug Id: 51203
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        public async Task PulseAssessmentV2_Post_SavePulseAssessment_Published_Success()
        {
            //given
            var client = await GetAuthenticatedClient();
            var roleFilter = new List<RoleRequest>()
            {
                new RoleRequest()
                {
                    Key = "Role",
                    Tags = new List<TagRoleRequest>()
                    {
                        new TagRoleRequest()
                        {
                            Id = 11597,
                            Name = "Scrum Master"
                        }
                    }
                }
            };

            var pulseAddRequest = PulseV2Factory.PulseAssessmentV2AddRequest(_radarResponse, _teamWithTeamMemberResponses, _teamId, true, roleFilter);

            //When
            var pulseAssessmentResponse = await client.PostAsync<CreatePulseAssessmentResponse>(RequestUris.PulseAssessmentV2SavePulseAssessment().AddQueryParameter("companyId", Company.Id), pulseAddRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.OK, pulseAssessmentResponse.StatusCode, "Status code does not match");
            Assert.IsTrue(pulseAssessmentResponse.Dto.PulseAssessmentId > 0, "Pulse assessment Id is 0");
        }

        //200
        [TestMethod]
        [TestCategory("KnownDefect")] //Bug Id: 51203
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        public async Task PulseAssessmentV2_Post_SavePulseAssessment_WithOutRoleFilter_Published_Success()
        {
            //given
            var client = await GetAuthenticatedClient();

            var teams = _setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(SharedConstants.UpdateTeam).TeamId;

            var pulseAddRequest = PulseV2Factory.PulseAssessmentV2AddRequest(_radarResponse, _teamWithTeamMemberResponses, teams);

            //When
            var pulseAssessmentResponse = await client.PostAsync<CreatePulseAssessmentResponse>(RequestUris.PulseAssessmentV2SavePulseAssessment().AddQueryParameter("companyId", Company.Id), pulseAddRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.OK, pulseAssessmentResponse.StatusCode, "Status code does not match");
            Assert.IsTrue(pulseAssessmentResponse.Dto.PulseAssessmentId > 0, "Pulse assessment Id is 0");
        }

        //200
        [TestMethod]
        [TestCategory("KnownDefect")] //Bug Id: 51203
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        public async Task PulseAssessmentV2_Post_SavePulseAssessment_WithoutCompanyId_Published_Success()
        {
            //given
            var client = await GetAuthenticatedClient();

            var pulseAddRequest = PulseV2Factory.PulseAssessmentV2AddRequest(_radarResponse, _teamWithTeamMemberResponses, _teamId);

            //When
            var pulseAssessmentResponse = await client.PostAsync<CreatePulseAssessmentResponse>(RequestUris.PulseAssessmentV2SavePulseAssessment(), pulseAddRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.OK, pulseAssessmentResponse.StatusCode, "Status code does not match");
            Assert.IsTrue(pulseAssessmentResponse.Dto.PulseAssessmentId > 0, "Pulse assessment Id is 0");
        }

        //200
        [TestMethod]
        [TestCategory("KnownDefect")] //Bug Id: 51203
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        public async Task PulseAssessmentV2_Post_SavePulseAssessment_Draft_Success()
        {
            //given
            var client = await GetAuthenticatedClient();

            var pulseAddRequest = PulseV2Factory.PulseAssessmentV2AddRequest(_radarResponse, _teamWithTeamMemberResponses, _teamId, false);

            //When
            var pulseAssessmentResponse = await client.PostAsync<CreatePulseAssessmentResponse>(RequestUris.PulseAssessmentV2SavePulseAssessment().AddQueryParameter("companyId", Company.Id), pulseAddRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.OK, pulseAssessmentResponse.StatusCode, "Status code does not match");
            Assert.IsTrue(pulseAssessmentResponse.Dto.PulseAssessmentId > 0, "Pulse assessment Id is 0");

        }

        //400
        [TestMethod]
        [TestCategory("SiteAdmin")]
        public async Task PulseAssessmentV2_Post_SavePulseAssessment_WithFakeCompanyId_BadRequest()
        {
            //given
            var client = await GetAuthenticatedClient();

            var pulseAddRequest = PulseV2Factory.PulseAssessmentV2AddRequest(_radarResponse, _teamWithTeamMemberResponses, _teamId);

            //When
            var pulseAssessmentResponse = await client.PostAsync<IList<string>>(RequestUris.PulseAssessmentV2SavePulseAssessment().AddQueryParameter("companyId", SharedConstants.FakeCompanyId), pulseAddRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.BadRequest, pulseAssessmentResponse.StatusCode, "Status code does not match");
            Assert.AreEqual("Company with Id 3 is not a valid company", pulseAssessmentResponse.Dto[0], "Response message is not the same");
        }

        //400
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        public async Task PulseAssessmentV2_Post_SavePulseAssessment_SelectedTeams_InValidTeamId_BadRequest()
        {
            //Given
            var client = await GetAuthenticatedClient();
            var teams = _setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(SharedConstants.UpdateTeam).TeamId;
            var pulseAddRequest = PulseV2Factory.PulseAssessmentV2AddRequest(_radarResponse, _teamWithTeamMemberResponses, teams);
            pulseAddRequest.SelectedTeams.First().TeamId = 0;

            //When
            var pulseAssessmentResponse = await client.PostAsync<IList<string>>(RequestUris.PulseAssessmentV2SavePulseAssessment().AddQueryParameter("companyId", Company.Id), pulseAddRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.BadRequest, pulseAssessmentResponse.StatusCode, "Status code does not match");
            Assert.AreEqual("The team id(s) are invalid for 0.", pulseAssessmentResponse.Dto[0], "Response message is not the same");
        }

        //400
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        public async Task PulseAssessmentV2_Post_SavePulseAssessment_InValidTeamId_BadRequest()
        {
            //Given
            var client = await GetAuthenticatedClient();
            var teams = _setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(SharedConstants.UpdateTeam).TeamId;
            var pulseAddRequest = PulseV2Factory.PulseAssessmentV2AddRequest(_radarResponse, _teamWithTeamMemberResponses, teams);
            pulseAddRequest.TeamId = 0;

            //When
            var pulseAssessmentResponse = await client.PostAsync<IList<string>>(RequestUris.PulseAssessmentV2SavePulseAssessment().AddQueryParameter("companyId", Company.Id), pulseAddRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.BadRequest, pulseAssessmentResponse.StatusCode, "Status code does not match");
            Assert.AreEqual("'Team Id' is not valid", pulseAssessmentResponse.Dto[0], "Response message is not the same");
        }

        //401
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("Member")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        public async Task PulseAssessmentV2_Post_SavePulseAssessment_Unauthorized()
        {
            //given
            var client = GetUnauthenticatedClient();
            var pulseAddRequest = PulseV2Factory.PulseAssessmentV2AddRequest(_radarResponse, _teamWithTeamMemberResponses, _teamId);

            //When
            var pulseAssessmentResponse = await client.PostAsync<IList<string>>(RequestUris.PulseAssessmentV2SavePulseAssessment().AddQueryParameter("companyId", Company.Id), pulseAddRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.Unauthorized, pulseAssessmentResponse.StatusCode, "Status code does not match");
        }

        //403
        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task PulseAssessmentV2_Post_SavePulseAssessment_WithFakeCompanyId_Forbidden()
        {
            //given
            var client = await GetAuthenticatedClient();

            var pulseAddRequest = PulseV2Factory.PulseAssessmentV2AddRequest(_radarResponse, _teamWithTeamMemberResponses, _teamId);

            //When
            var pulseAssessmentResponse = await client.PostAsync<IList<string>>(RequestUris.PulseAssessmentV2SavePulseAssessment().AddQueryParameter("companyId", SharedConstants.FakeCompanyId), pulseAddRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.Forbidden, pulseAssessmentResponse.StatusCode, "Status code does not match");
        }

        //403
        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task PulseAssessmentV2_Post_SavePulseAssessment_WithFakeTeamId_Forbidden()
        {
            //given
            var client = await GetAuthenticatedClient();

            var pulseAddRequest = PulseV2Factory.PulseAssessmentV2AddRequest(_radarResponse, _teamWithTeamMemberResponses, _teamId);
            pulseAddRequest.TeamId = 9999;
            
            //When
            var pulseAssessmentResponse = await client.PostAsync<IList<string>>(RequestUris.PulseAssessmentV2SavePulseAssessment().AddQueryParameter("companyId", Company.Id), pulseAddRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.Forbidden, pulseAssessmentResponse.StatusCode, "Status code does not match");
        }

        //403
        [TestMethod]
        [TestCategory("Member")]
        public async Task PulseAssessmentV2_Post_SavePulseAssessment_ByMember_Forbidden()
        {
            //Given
            var client = await GetAuthenticatedClient();
            var pulseAddRequest = PulseV2Factory.PulseAssessmentV2AddRequest(_radarResponse, _teamWithTeamMemberResponses, _teamId);

            //When
            var pulseAssessmentResponse = await client.PostAsync<IList<string>>(RequestUris.PulseAssessmentV2SavePulseAssessment().AddQueryParameter("companyId", Company.Id), pulseAddRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.Forbidden, pulseAssessmentResponse.StatusCode, "Status code does not match");
        }

        //409
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        public async Task PulseAssessmentV2_Post_SavePulseAssessment_Conflicts()
        {
            //given
            var client = await GetAuthenticatedClient();

            var pulseAddRequest = PulseV2Factory.PulseAssessmentV2AddRequest(_radarResponse, _teamWithTeamMemberResponses, _teamId);

            var pulseAssessmentResponse = await client.PostAsync<CreatePulseAssessmentResponse>(RequestUris.PulseAssessmentV2SavePulseAssessment().AddQueryParameter("companyId", Company.Id), pulseAddRequest);
            pulseAssessmentResponse.EnsureSuccess();

            //When
            pulseAssessmentResponse = await client.PostAsync<CreatePulseAssessmentResponse>(RequestUris.PulseAssessmentV2SavePulseAssessment().AddQueryParameter("companyId", Company.Id), pulseAddRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.Conflict, pulseAssessmentResponse.StatusCode, "Status code does not match");
        }
    }
}
