using System;
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
    public class PulseAssessmentV2GetPulseAssessmentTests : PulseApiTestBase
    {
        private static int _teamId;
        private static IList<TeamV2Response> _teamWithTeamMemberResponses;
        private static CreatePulseAssessmentResponse _pulseAssessmentResponse;
        private static RadarQuestionDetailsV2Response _radarDetailResponse;
        private static SavePulseAssessmentV2Request _pulseAssessmentV2Request;
        private static List<RoleRequest> _roleRequests;
        private static readonly UserConfig CompanyAdminUserConfig = new UserConfig("CA");
        private static readonly GetPulseAssessmentV2Request GetPulseAssessmentRequest = PulseV2Factory.GetPulseAssessmentRequest("PulseCheck");

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            var setupApi = new SetupTeardownApi(TestEnvironment);

            var user = User;
            if (User.IsSiteAdmin() || User.IsPartnerAdmin() || User.IsMember())
            {
                user = CompanyAdminUserConfig.GetUserByDescription("user 1");
            }

            //Get team profile 
            _teamId = setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(SharedConstants.UpdateTeam).TeamId;
            _teamWithTeamMemberResponses = setupApi.GetTeamWithTeamMemberResponse(_teamId, user);

            //Get radar details
            _radarDetailResponse = setupApi.GetRadarQuestionDetailsV2(Company.Id, _teamId, SharedConstants.TeamSurveyId, user);
            _roleRequests = new List<RoleRequest>()
            {
                new RoleRequest()
                    { Key = "Role", Tags = new List<TagRoleRequest>() { new TagRoleRequest() { Id = 11597, Name = "Scrum Master" } } }
            };

            //Create PulseAssessment
            _pulseAssessmentV2Request = PulseV2Factory.PulseAssessmentV2AddRequest(_radarDetailResponse, _teamWithTeamMemberResponses, _teamId, true, _roleRequests);
            _pulseAssessmentResponse = setupApi.CreatePulseAssessmentV2(_pulseAssessmentV2Request, Company.Id, user);

        }

        //200
        [TestMethod]
        [TestCategory("KnownDefect")] //Bug Id: 51203
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        public async Task PulseAssessmentV2_Get_PulseAssessment_WithDataRequest_PulseCheck_Published_Success()
        {
            //given
            var client = await GetAuthenticatedClient();
            
            //When
            var getPulseAssessmentResponse =
                await client.PostAsync<PulseAssessmentV2Response>(
                    RequestUris.PulseAssessmentV2GetPulseAssessment(_pulseAssessmentResponse.PulseAssessmentId)
                        .AddQueryParameter("companyId", Company.Id), GetPulseAssessmentRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.OK, getPulseAssessmentResponse.StatusCode, "Status code does not match");

            ResponseVerificationWithDataRequest(_pulseAssessmentV2Request, getPulseAssessmentResponse, "PulseCheck");
        }

        //200
        [TestMethod]
        [TestCategory("KnownDefect")] //Bug Id: 51203
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        public async Task PulseAssessmentV2_Get_PulseAssessment_WithDataRequest_Questions_Published_Success()
        {
            //given
            var client = await GetAuthenticatedClient();
            var getPulseAssessmentRequest = PulseV2Factory.GetPulseAssessmentRequest("Questions");

            //When
            var getPulseAssessmentResponse =
                await client.PostAsync<PulseAssessmentV2Response>(
                    RequestUris.PulseAssessmentV2GetPulseAssessment(_pulseAssessmentResponse.PulseAssessmentId)
                        .AddQueryParameter("companyId", Company.Id), getPulseAssessmentRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.OK, getPulseAssessmentResponse.StatusCode, "Status code does not match");

            ResponseVerificationWithDataRequest(_pulseAssessmentV2Request, getPulseAssessmentResponse, "Questions");
        }

        //200
        [TestMethod]
        [TestCategory("KnownDefect")] //Bug Id: 51203
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        public async Task PulseAssessmentV2_Get_PulseAssessment_WithDataRequest_Teams_Published_Success()
        {
            //given
            var client = await GetAuthenticatedClient();
            var getPulseAssessmentRequest = PulseV2Factory.GetPulseAssessmentRequest("Teams");
            
            //When
            var getPulseAssessmentResponse =
                await client.PostAsync<PulseAssessmentV2Response>(RequestUris.PulseAssessmentV2GetPulseAssessment(_pulseAssessmentResponse.PulseAssessmentId).AddQueryParameter("companyId",Company.Id), getPulseAssessmentRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.OK, getPulseAssessmentResponse.StatusCode, "Status code does not match");

            ResponseVerificationWithDataRequest(_pulseAssessmentV2Request, getPulseAssessmentResponse);
        }

        //200
        [TestMethod]
        [TestCategory("KnownDefect")] //Bug Id: 51203
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        public async Task PulseAssessmentV2_Get_PulseAssessment_Published_WithoutRoleFilter_Success()
        {
            //given
            var client = await GetAuthenticatedClient();

            var getPulseAssessmentRequest = PulseV2Factory.GetPulseAssessmentRequest("Questions");
            var pulseAssessmentV2AddRequest = PulseV2Factory.PulseAssessmentV2AddRequest(_radarDetailResponse, _teamWithTeamMemberResponses, _teamId);
            var pulseAssessmentResponse = client.PostAsync<CreatePulseAssessmentResponse>(
                RequestUris.PulseAssessmentV2SavePulseAssessment().AddQueryParameter("companyId",Company.Id),
                pulseAssessmentV2AddRequest).GetAwaiter().GetResult();
            pulseAssessmentResponse.EnsureSuccess();

            //When
            var getPulseAssessmentResponse =
                await client.PostAsync<PulseAssessmentV2Response>(
                    RequestUris.PulseAssessmentV2GetPulseAssessment(_pulseAssessmentResponse.PulseAssessmentId)
                        .AddQueryParameter("companyId", Company.Id), getPulseAssessmentRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.OK, getPulseAssessmentResponse.StatusCode, "Status code does not match");

            ResponseVerificationWithDataRequest(_pulseAssessmentV2Request, getPulseAssessmentResponse, "Questions");
        }

        //200
        [TestMethod]
        [TestCategory("KnownDefect")] //Bug Id: 51203
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        public async Task PulseAssessmentV2_Get_PulseAssessment_Draft_Success()
        {
            //given
            var client = await GetAuthenticatedClient();
            
            var pulseAssessmentV2Request = PulseV2Factory.PulseAssessmentV2AddRequest(_radarDetailResponse, _teamWithTeamMemberResponses, _teamId, false);
            var pulseAssessmentResponse = client.PostAsync<CreatePulseAssessmentResponse>(
                RequestUris.PulseAssessmentV2SavePulseAssessment().AddQueryParameter("companyId",Company.Id),
                    pulseAssessmentV2Request).GetAwaiter().GetResult();
            pulseAssessmentResponse.EnsureSuccess();

            //When
            var getPulseAssessmentResponse =
                await client.PostAsync<PulseAssessmentV2Response>(
                    RequestUris.PulseAssessmentV2GetPulseAssessment(pulseAssessmentResponse.Dto.PulseAssessmentId)
                        .AddQueryParameter("companyId", Company.Id), GetPulseAssessmentRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.Created, pulseAssessmentResponse.StatusCode, "Status code does not match");
            Assert.AreEqual(pulseAssessmentV2Request.Name, getPulseAssessmentResponse.Dto.Name, "Pulse Assessment 'Name' is not matched");
            Assert.AreEqual(pulseAssessmentV2Request.TeamId, getPulseAssessmentResponse.Dto.TeamId, "'TeamId' is not matched");
            Assert.AreEqual(pulseAssessmentV2Request.SurveyId, getPulseAssessmentResponse.Dto.SurveyId, "'SurveyId' is not matched");
            Assert.AreEqual(pulseAssessmentV2Request.SurveyName, getPulseAssessmentResponse.Dto.SurveyName, "Survey does not match");
            Assert.IsNotNull(getPulseAssessmentResponse.Dto.PulseAssessmentUId, "'PulseAssessmentUid' is null");
            Assert.IsFalse(getPulseAssessmentResponse.Dto.IsPublished, "Pulse Assessment is not published");
            Assert.IsFalse(getPulseAssessmentResponse.Dto.IsPulseAssessmentClosed, "Pulse Assessment is closed");
            Assert.IsTrue(getPulseAssessmentResponse.Dto.SelectedTeams.Count == 0, "'Selected Teams' are available");
            Assert.IsTrue(!getPulseAssessmentResponse.Dto.PulseComponents.Any(), "'Pulse components' are available");
            Assert.AreEqual(pulseAssessmentV2Request.PeriodId, getPulseAssessmentResponse.Dto.PeriodId, "'PeriodId' is not matched");
            Assert.IsNull(getPulseAssessmentResponse.Dto.StartDate, "StartDate does not match");
            Assert.IsNull(getPulseAssessmentResponse.Dto.EndDate, "EndDate does not match");
            Assert.IsNull(getPulseAssessmentResponse.Dto.PublishedDate, "PublishDate does not match");
            Assert.IsNull(getPulseAssessmentResponse.Dto.RoleFilter.Tags, "'Role filters' are available");
        }

        //400
        [TestMethod]
        [TestCategory("KnownDefect")] //Bug Id: 51203
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task PulseAssessmentV2_Get_PulseAssessment_WithRandomDataRequest_Published_BadRequest()
        {
            //Given
            var client = await GetAuthenticatedClient();
            var getPulseAssessmentRequest = PulseV2Factory.GetPulseAssessmentRequest("abcd");

            //When
            var pulseAssessmentResponse =
                await client.PostAsync<IList<string>>(
                    RequestUris.PulseAssessmentV2GetPulseAssessment(_pulseAssessmentResponse.PulseAssessmentId)
                        .AddQueryParameter("companyId", Company.Id), getPulseAssessmentRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.BadRequest, pulseAssessmentResponse.StatusCode, "Status code does not match");
            Assert.AreEqual("'DataRequest' options are PulseCheck, Questions or Teams", pulseAssessmentResponse.Dto[0], "Response messages are not the same");
        }

        //400
        [TestMethod]
        [TestCategory("KnownDefect")] //Bug Id: 51203
        [TestCategory("SiteAdmin")]
        public async Task PulseAssessmentV2_Get_PulseAssessment_WithoutCompanyId_BadRequest()
        {
            //Given
            var client = await GetAuthenticatedClient();
            
            //When
            var pulseAssessmentResponse =
                await client.PostAsync<IList<string>>(RequestUris.PulseAssessmentV2GetPulseAssessment(_pulseAssessmentResponse.PulseAssessmentId), GetPulseAssessmentRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.BadRequest, pulseAssessmentResponse.StatusCode, "Status code does not match");
            Assert.AreEqual("'Company Id' is not valid", pulseAssessmentResponse.Dto[0], "Response messages are not the same");
        }

        //400
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task PulseAssessmentV2_Get_PulseAssessment_Invalid_PulseAssessmentId_BadRequest()
        {
            //Given
            var client = await GetAuthenticatedClient();
            
            //When
            var pulseAssessmentResponse = await client.PostAsync<IList<string>>(
                RequestUris.PulseAssessmentV2GetPulseAssessment(0000).AddQueryParameter("companyId", Company.Id),
                GetPulseAssessmentRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.BadRequest, pulseAssessmentResponse.StatusCode, "Status code does not match");
            Assert.AreEqual("'Pulse Assessment Id' is not valid", pulseAssessmentResponse.Dto[0], "Response messages are not the same");
        }

        //401
        [TestMethod]
        [TestCategory("KnownDefect")] //Bug Id: 51203
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task PulseAssessmentV2_Get_PulseAssessment_Unauthorized()
        {
            //given
            var client = GetUnauthenticatedClient();
            
            //When
            var pulseAssessmentResponse =
                await client.PostAsync<IList<string>>(
                    RequestUris.PulseAssessmentV2GetPulseAssessment(_pulseAssessmentResponse.PulseAssessmentId)
                        .AddQueryParameter("companyId", Company.Id), GetPulseAssessmentRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.Unauthorized, pulseAssessmentResponse.StatusCode, "Status code does not match");
        }

        //403
        [TestMethod]
        [TestCategory("KnownDefect")] //Bug Id: 51203
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task PulseAssessmentV2_Get_PulseAssessment_Published_WithoutCompanyId_Forbidden()
        {
            //given
            var client = await GetAuthenticatedClient();
            
            //When
            var getPulseAssessmentResponse = await client.PostAsync<IList<string>>(
                RequestUris.PulseAssessmentV2GetPulseAssessment(_pulseAssessmentResponse.PulseAssessmentId),
                GetPulseAssessmentRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.Forbidden, getPulseAssessmentResponse.StatusCode, "Status code does not match");
        }

        //403
        [TestMethod]
        [TestCategory("KnownDefect")] //Bug Id: 51203
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task PulseAssessmentV2_Get_PulseAssessment_WithFakeCompanyId_Forbidden()
        {
            //given
            var client = await GetAuthenticatedClient();
            
            //When
            var pulseAssessmentResponse = await client.PostAsync<IList<string>>(
                RequestUris.PulseAssessmentV2GetPulseAssessment(_pulseAssessmentResponse.PulseAssessmentId)
                    .AddQueryParameter("companyId", SharedConstants.FakeCompanyId), GetPulseAssessmentRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.Forbidden, pulseAssessmentResponse.StatusCode, "Status code does not match");
        }

        //403
        [TestMethod]
        [TestCategory("KnownDefect")] //Bug Id: 51203
        [TestCategory("Member")]
        public async Task PulseAssessmentV2_Get_PulseAssessment_ByMember_Forbidden()
        {
            //Given
            var client = await GetAuthenticatedClient();
            
            //When
            var pulseAssessmentResponse = await client.PostAsync<IList<string>>(
                RequestUris.PulseAssessmentV2GetPulseAssessment(_pulseAssessmentResponse.PulseAssessmentId)
                    .AddQueryParameter("companyId", Company.Id), GetPulseAssessmentRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.Forbidden, pulseAssessmentResponse.StatusCode, "Status code does not match");
        }

        //404
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task PulseAssessmentV2_Get_PulseAssessment_NotFound()
        {
            //given
            var client = await GetAuthenticatedClient();
            
            //When
            var pulseAssessmentResponse =
                await client.PostAsync<IList<string>>(
                    RequestUris.PulseAssessmentV2GetPulseAssessment(1111).AddQueryParameter("companyId", Company.Id),
                    GetPulseAssessmentRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.NotFound, pulseAssessmentResponse.StatusCode, "Status code does not match");
        }

        public static void ResponseVerificationWithDataRequest(SavePulseAssessmentV2Request pulseAssessmentV2Request,
            ApiResponse<PulseAssessmentV2Response> getPulseAssessmentResponse, string dataRequest = "Teams")
        {
            switch (dataRequest)
            {
                case "PulseCheck":
                    Assert.AreEqual(pulseAssessmentV2Request.Name, getPulseAssessmentResponse.Dto.Name, "Pulse Assessment 'Name' is not matched");
                    Assert.AreEqual(pulseAssessmentV2Request.TeamId, getPulseAssessmentResponse.Dto.TeamId, "'TeamId' is not matched");
                    Assert.AreEqual(pulseAssessmentV2Request.SurveyId, getPulseAssessmentResponse.Dto.SurveyId, "'SurveyId' is not matched");
                    Assert.AreEqual(pulseAssessmentV2Request.SurveyName, getPulseAssessmentResponse.Dto.SurveyName, "Survey does not match");
                    Assert.AreEqual(pulseAssessmentV2Request.RepeatEndStrategyId, getPulseAssessmentResponse.Dto.RepeatEndStrategyId, "'Repeat End StrategyId' is not matched");
                    Assert.AreEqual(pulseAssessmentV2Request.PeriodId, getPulseAssessmentResponse.Dto.PeriodId, "'PeriodId' is not matched");
                    Assert.AreEqual(pulseAssessmentV2Request.RepeatIntervalId, getPulseAssessmentResponse.Dto.RepeatIntervalId, "'RepeatIntervalId' is not matched");
                    Assert.That.TimeIsClose(pulseAssessmentV2Request.StartDate!, (DateTime)getPulseAssessmentResponse.Dto.StartDate!, 5);
                    Assert.IsTrue(getPulseAssessmentResponse.Dto.EndDate==null,"End date is not null for 'Does Not Repeat'");
                    Assert.That.TimeIsClose((DateTime)pulseAssessmentV2Request.PublishedDate!, (DateTime)getPulseAssessmentResponse.Dto.PublishedDate!, 3);
                    Assert.IsNotNull(getPulseAssessmentResponse.Dto.PulseAssessmentUId, "'PulseAssessmentUid' is null");
                    Assert.IsTrue(getPulseAssessmentResponse.Dto.IsPublished, "Pulse Assessment is not published");
                    Assert.IsFalse(getPulseAssessmentResponse.Dto.IsPulseAssessmentClosed, "Pulse Assessment is closed");
                    Assert.IsTrue(getPulseAssessmentResponse.Dto.SelectedTeams.Count == 0, "'Selected Teams' are available");
                    Assert.IsTrue(!getPulseAssessmentResponse.Dto.PulseComponents.Any(), "'Pulse components' are available");
                    Assert.IsNull(getPulseAssessmentResponse.Dto.RoleFilter.Tags, "'Role filters' are available");
                    break;

                case "Questions":
                    //PulseComponents
                    //Dimension
                    Assert.That.ListsAreEqual(pulseAssessmentV2Request.DimensionIds.Select(a => a.ToString()).ToList(), getPulseAssessmentResponse.Dto.PulseComponents.Select(d => d.DimensionId.ToString()).ToList(), "DimensionIds doesn't match");
                    Assert.That.ListsAreEqual(_radarDetailResponse.Dimensions.Where(a => a.Name != "Finish").Select(d => d.Name).ToList(), getPulseAssessmentResponse.Dto.PulseComponents.Select(d => d.Name).ToList(), "Dimension Names doesn't match");
                    Assert.IsTrue(getPulseAssessmentResponse.Dto.PulseComponents.All(d => d.Selected), "All dimensions are not selected");

                    //Sub Dimension
                    Assert.That.ListsAreEqual(pulseAssessmentV2Request.SubDimensionIds.Select(a => a.ToString()).ToList(), getPulseAssessmentResponse.Dto.PulseComponents.SelectMany(d => d.SubDimensions).Select(s => s.SubDimensionId.ToString()).ToList(), "Sub-DimensionIds doesn't match");
                    Assert.That.ListsAreEqual(_radarDetailResponse.Dimensions.Where(a => a.Name != "Finish").SelectMany(d => d.SubDimensions).Select(s => s.Name).ToList(), getPulseAssessmentResponse.Dto.PulseComponents.SelectMany(d => d.SubDimensions).Select(s => s.Name).ToList(), "Sub-Dimension Names doesn't match");
                    Assert.IsTrue(getPulseAssessmentResponse.Dto.PulseComponents.SelectMany(d => d.SubDimensions).All(d => d.Selected), "All sub-dimensions are not selected");

                    //Competencies
                    Assert.That.ListsAreEqual(pulseAssessmentV2Request.Competencies.Select(c => c.CompetencyId.ToString()).ToList(), getPulseAssessmentResponse.Dto.PulseComponents.SelectMany(d => d.SubDimensions).SelectMany(s => s.Competencies).Select(c => c.CompetencyId.ToString()).ToList(), "CompetencyIds doesn't match");
                    Assert.That.ListsAreEqual(_radarDetailResponse.Dimensions.Where(a => a.Name != "Finish").SelectMany(d => d.SubDimensions).SelectMany(s => s.Competencies).Select(c => c.Name).ToList(), getPulseAssessmentResponse.Dto.PulseComponents.SelectMany(d => d.SubDimensions).SelectMany(s => s.Competencies).Select(c => c.Name).ToList(), "Competency Names doesn't match");
                    Assert.IsTrue(getPulseAssessmentResponse.Dto.PulseComponents.SelectMany(d => d.SubDimensions).SelectMany(d => d.Competencies).All(d => d.Selected), "All competencies are not selected");

                    //Questions
                    Assert.That.ListsAreEqual(pulseAssessmentV2Request.Competencies.SelectMany(c => c.QuestionIds.Select(a => a.ToString())).ToList(), getPulseAssessmentResponse.Dto.PulseComponents.SelectMany(d => d.SubDimensions).SelectMany(s => s.Competencies).SelectMany(c => c.Questions).Select(q => q.QuestionId.ToString()).ToList(), "Question Ids doesn't match");
                    Assert.IsTrue(getPulseAssessmentResponse.Dto.PulseComponents.SelectMany(d => d.SubDimensions).SelectMany(d => d.Competencies).SelectMany(c => c.Questions).All(d => d.Selected), "All Questions are not selected");

                    var questionIds = pulseAssessmentV2Request.Competencies.SelectMany(e => e.QuestionIds).ToList();
                    foreach (var id in questionIds)
                    {
                        var expected = _radarDetailResponse.Dimensions.Where(a => a.Name != "Finish").SelectMany(d => d.SubDimensions).SelectMany(s => s.Competencies).SelectMany(c => c.Questions).First(q => q.QuestionId.Equals(id));
                        var actual = getPulseAssessmentResponse.Dto.PulseComponents.SelectMany(d => d.SubDimensions).SelectMany(s => s.Competencies).SelectMany(c => c.Questions).First(q => q.QuestionId.Equals(id));

                        Assert.AreEqual(expected.Text.FormatSurveyQuestions(), actual.Text.FormatSurveyQuestions(), "Question Text does not match.");
                    }

                    Assert.AreEqual(_pulseAssessmentV2Request.RoleFilter.Tags.First().Key, getPulseAssessmentResponse.Dto.RoleFilter.Tags.First().Key, "Key Does not match");
                    Assert.AreEqual(_pulseAssessmentV2Request.RoleFilter.Tags.First().Tags.First().Id, getPulseAssessmentResponse.Dto.RoleFilter.Tags.First().Tags.First().Id, "Role Id does not match");
                    Assert.AreEqual(_pulseAssessmentV2Request.RoleFilter.Tags.First().Tags.First().Name, getPulseAssessmentResponse.Dto.RoleFilter.Tags.First().Tags.First().Name, "Role Name does not match");

                    break;

                default:
                    var ignoreProperties = new List<string>() {"TeamType","SelectedParticipants" };

                    var teams = PulseV2Factory.GetTeams(pulseAssessmentV2Request);
                    Assert.That.ResponseAreEqual(teams, getPulseAssessmentResponse.Dto.SelectedTeams, ignoreProperties);

                    break;
            }
        }
    }
}
