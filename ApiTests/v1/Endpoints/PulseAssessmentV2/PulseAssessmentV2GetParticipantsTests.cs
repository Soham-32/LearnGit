using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AtCommon.Api;
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
    public class PulseAssessmentV2GetParticipantsTests : PulseApiTestBase
    {
        private static CreatePulseAssessmentResponse _pulseAssessmentResponse;
        private static SavePulseAssessmentV2Request _pulseAddRequest;
        private static TeamHierarchyResponse _teams;
        private static TeamV2Response _expectedTeamResponse;
        private static readonly UserConfig CompanyAdminUserConfig = new UserConfig("CA");
        private static Dictionary<string, object> _query;
        private readonly GetParticipantsRequest RoleFilterRequest = PulseV2Factory.GetParticipantsRequest();

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            var setupApi = new SetupTeardownApi(TestEnvironment);

            var user = User;
            if (User.IsSiteAdmin() || User.IsPartnerAdmin())
            {
                user = CompanyAdminUserConfig.GetUserByDescription("user 1");
            }

            //Get team profile 
            _teams = setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(SharedConstants.UpdateTeam);
            var teamWithTeamMemberResponse = setupApi.GetTeamWithTeamMemberResponse(_teams.TeamId, user);
            _expectedTeamResponse = new TeamV2Response
            {
                TeamId = _teams.TeamId,
                Name = teamWithTeamMemberResponse.First().Name,
                SelectedParticipants = teamWithTeamMemberResponse.First().SelectedParticipants,
                Uid = teamWithTeamMemberResponse.First().Uid
            };

            //Get radar details
            var radarResponse = setupApi.GetRadarQuestionDetailsV2(Company.Id, _teams.TeamId, SharedConstants.AtTeamHealth3SurveyId, user);

            var dimension = new RadarQuestionDetailsV2Response()
            {
                Dimensions = new[] { radarResponse.Dimensions.First() },
                SurveyId = radarResponse.SurveyId,
                Name = radarResponse.Name
            };

            _pulseAddRequest = PulseV2Factory.PulseAssessmentV2AddRequest(dimension, teamWithTeamMemberResponse, _teams.TeamId);
            _pulseAssessmentResponse = setupApi.CreatePulseAssessmentV2(_pulseAddRequest, Company.Id, user);

            _query = new Dictionary<string, object>
            {
                { "companyId", Company.Id },
                { "pulseAssessmentId", _pulseAssessmentResponse.PulseAssessmentId },
                { "teamId", _teams.TeamId }
            };
        }

        //200
        [TestMethod]
        [TestCategory("KnownDefect")] //Bug Id: 51203
        [TestCategory("OrgLeader"), TestCategory("PartnerAdmin")]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public async Task PulseAssessmentV2_Get_Participants_With_ExcludeMember_Success()
        {
            //given
            var client = await GetAuthenticatedClient();
            var roleFilterRequest = PulseV2Factory.GetParticipantsRequest();
            roleFilterRequest.IsSelectedCompetencies = false;
            //When
            var response =
                await client.PostAsync<SelectedParticipantsResponse>(
                    RequestUris.PulseAssessmentV2GetParticipants().AddQueryParameter(_query), roleFilterRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status code does not match");
            Assert.AreEqual(_pulseAssessmentResponse.PulseAssessmentId, response.Dto.PulseAssessmentId, "'PulseAssessmentId' does not match");
            Assert.AreEqual(_teams.TeamId, response.Dto.TeamId, "'TeamId' does not match");

            Assert.IsTrue(response.Dto.SelectedParticipants.First().IsExcludedForQuestions, "Member is not Disabled");
            Assert.IsFalse(response.Dto.SelectedParticipants.Last().IsExcludedForQuestions, "Member is Disabled");
            ResponseVerification(_expectedTeamResponse.SelectedParticipants, response.Dto.SelectedParticipants);
        }

        //200
        [TestMethod]
        [TestCategory("KnownDefect")] //Bug Id: 51203
        [TestCategory("OrgLeader"), TestCategory("PartnerAdmin")]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public async Task PulseAssessmentV2_Get_Participants_Without_RoleRequest_Success()
        {
            //given
            var client = await GetAuthenticatedClient();
            
            //When
            var response =
                await client.PostAsync<SelectedParticipantsResponse>(RequestUris.PulseAssessmentV2GetParticipants().AddQueryParameter(_query), RoleFilterRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status code does not match");
            Assert.AreEqual(_pulseAssessmentResponse.PulseAssessmentId, response.Dto.PulseAssessmentId, "'PulseAssessmentId' does not match");
            Assert.AreEqual(_teams.TeamId, response.Dto.TeamId, "'TeamId' does not match");

            Assert.IsFalse(response.Dto.SelectedParticipants.First().IsFilteredByRole,"Member is filtered with role");
            ResponseVerification(_expectedTeamResponse.SelectedParticipants, response.Dto.SelectedParticipants);

        }

        //200
        [TestMethod]
        [TestCategory("KnownDefect")] //Bug Id: 51203
        [TestCategory("OrgLeader"), TestCategory("PartnerAdmin")]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public async Task PulseAssessmentV2_Get_Participants_With_RoleRequest_Success()
        {
            //given
            var client = await GetAuthenticatedClient();
            var roleRequest = new PulseAssessmentRoleFilterRequest()
            {
                Tags = new List<RoleRequest>()
                {
                    new RoleRequest()
                    {
                        Key = "Role",
                        Tags = new List<TagRoleRequest>()
                        {
                            new TagRoleRequest()
                            {
                                Name = _expectedTeamResponse.SelectedParticipants.First().Tags.First().Tags.First()
                                    .Name,
                                Id = _expectedTeamResponse.SelectedParticipants.First().Tags.First().Tags.First().Id
                            }
                        }
                    }
                }
            };

            var roleFilterRequest = PulseV2Factory.GetParticipantsRequest(roleRequest);
            
            //When
            var response =
                await client.PostAsync<SelectedParticipantsResponse>(RequestUris.PulseAssessmentV2GetParticipants().AddQueryParameter(_query), roleFilterRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status code does not match");
            Assert.AreEqual(_pulseAssessmentResponse.PulseAssessmentId, response.Dto.PulseAssessmentId, "'PulseAssessmentId' does not match");
            Assert.AreEqual(_teams.TeamId, response.Dto.TeamId, "'TeamId' does not match");

            Assert.IsTrue(response.Dto.SelectedParticipants.First().IsFilteredByRole, "Member is not filtered with role");
            ResponseVerification(_expectedTeamResponse.SelectedParticipants, response.Dto.SelectedParticipants);

        }

        //400
        [TestMethod]
        [TestCategory("KnownDefect")] //Bug Id: 51203
        [TestCategory("OrgLeader"), TestCategory("PartnerAdmin")]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public async Task PulseAssessmentV2_Get_Participants_InvalidPulseAssessmentId_BadRequest()
        {
            //given
            var client = await GetAuthenticatedClient();
            
            var query = new Dictionary<string, object>
            {
                { "companyId", Company.Id },
                { "pulseAssessmentId", 0000},
                { "teamId", _teams.TeamId}
            };

            //When
            var response =
                await client.PostAsync<IList<string>>(RequestUris.PulseAssessmentV2GetParticipants().AddQueryParameter(query), RoleFilterRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status code does not match");
            Assert.AreEqual("'PulseAssessmentId' must be greater than 0", response.Dto.First(), "Validation message are not same");
        }

        //400
        [TestMethod]
        [TestCategory("KnownDefect")] //Bug Id: 51203
        [TestCategory("OrgLeader"), TestCategory("PartnerAdmin")]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public async Task PulseAssessmentV2_Get_Participants_InvalidTeamId_BadRequest()
        {
            //given
            var client = await GetAuthenticatedClient();
            
            var query = new Dictionary<string, object>
            {
                { "companyId", Company.Id },
                { "pulseAssessmentId", _pulseAssessmentResponse.PulseAssessmentId},
                { "teamId", 0000}
            };

            //When
            var response =
                await client.PostAsync<IList<string>>(RequestUris.PulseAssessmentV2GetParticipants().AddQueryParameter(query), RoleFilterRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status code does not match");
            Assert.AreEqual("'TeamId' must be greater than 0", response.Dto.First(), "Validation message are not same");
        }

        //400
        [TestMethod]
        [TestCategory("KnownDefect")] //Bug Id: 51203
        [TestCategory("SiteAdmin")]
        public async Task PulseAssessmentV2_Get_Participants_FakeCompanyId_BadRequest()
        {
            //given
            var client = await GetAuthenticatedClient();
            
            var query = new Dictionary<string, object>
            {
                { "companyId", 0 },
                { "pulseAssessmentId", _pulseAssessmentResponse.PulseAssessmentId},
                { "teamId", _teams.TeamId}
            };

            //When
            var response =
                await client.PostAsync<IList<string>>(RequestUris.PulseAssessmentV2GetParticipants().AddQueryParameter(query), RoleFilterRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status code does not match");
            Assert.AreEqual("'CompanyId' must be greater than 0", response.Dto.First(), "Validation message are not same");

        }

        //401
        [TestMethod]
        [TestCategory("KnownDefect")] //Bug Id: 51203
        [TestCategory("OrgLeader"), TestCategory("PartnerAdmin")]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public async Task PulseAssessmentV2_Get_Participants_Unauthorized()
        {
            //given
            var client = GetUnauthenticatedClient();
            
            //When
            var response =
                await client.PostAsync<SelectedParticipantsResponse>(RequestUris.PulseAssessmentV2GetParticipants().AddQueryParameter(_query), RoleFilterRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status code does not match");
        }

        //403
        [TestMethod]
        [TestCategory("KnownDefect")] //Bug Id: 51203
        [TestCategory("OrgLeader"), TestCategory("PartnerAdmin")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public async Task PulseAssessmentV2_Get_Participants_FakeCompanyId_Forbidden()
        {
            //given
            var client = await GetAuthenticatedClient();
            
            var query = new Dictionary<string, object>
            {
                { "companyId", SharedConstants.FakeCompanyId },
                { "pulseAssessmentId", _pulseAssessmentResponse.PulseAssessmentId},
                { "teamId", _teams.TeamId}
            };

            //When
            var response =
                await client.PostAsync<SelectedParticipantsResponse>(RequestUris.PulseAssessmentV2GetParticipants().AddQueryParameter(query), RoleFilterRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status code does not match");

        }

        //Verify Participant's details
        public static void ResponseVerification(List<TeamMemberV2Response> memberRequest, List<TeamMemberV2Response> memberResponse)
        {
            var ignoreProperties = new List<string>() { "DeletedAt", "IsFilteredByRole" , "IsExcludedForQuestions" };
            Assert.That.ResponseAreEqual(memberRequest, memberResponse, ignoreProperties);
        }
    }
}
