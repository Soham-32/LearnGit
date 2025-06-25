using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AtCommon.Api;
using AtCommon.Dtos.Assessments.PulseV2;
using AtCommon.Dtos.Companies;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiTests.v1.Endpoints.PulseAssessmentV2
{
    [TestClass]
    [TestCategory("PulseAssessmentsV2")]
    public class PulseAssessmentV2SendBatchEmailTests : PulseApiTestBase
    {
        private static CreatePulseAssessmentResponse _pulseAssessmentResponse;
        private static SavePulseAssessmentV2Request _pulseAddRequest;
        private static IList<TeamV2Response> _teamV2Responses;
        private static Dictionary<string, object> _query;
        private static TeamHierarchyResponse _teams;
        private static readonly UserConfig CompanyAdminUserConfig = new UserConfig("CA");

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
            _teams = setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(SharedConstants.MultiTeam);
            _teamV2Responses = setupApi.GetTeamWithTeamMemberResponse(_teams.TeamId, user);

            //Get radar details
            var radarResponse = setupApi.GetRadarQuestionDetailsV2(Company.Id, _teams.TeamId, SharedConstants.AtTeamHealth3SurveyId, user);

            _pulseAddRequest = PulseV2Factory.PulseAssessmentV2AddRequest(radarResponse, _teamV2Responses, _teams.TeamId);
            _pulseAssessmentResponse = setupApi.CreatePulseAssessmentV2(_pulseAddRequest, Company.Id, user);

            _query = new Dictionary<string, object>
            {
                { "companyId", Company.Id },
                { "pulseAssessmentId", _pulseAssessmentResponse.PulseAssessmentId },
                { "teamId", _teamV2Responses.Last().TeamId}
            };
        }

        //200
        [TestMethod]
        [TestCategory("KnownDefect")] //Bug Id: 51203
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public async Task PulseAssessmentV2_Post_BatchEmail_Success()
        {
            //Given
            var client = await GetAuthenticatedClient();

            //When 
            var response = await client.PostAsync<PulseAssessmentBatchEmailResponse>(RequestUris.PulseAssessmentV2BatchEmail().AddQueryParameter(_query));

            //Then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status Code doesn't match");
            Assert.IsTrue(GmailUtil.DoesMemberEmailExist(SharedConstants.PulseEmailSubject(_teamV2Responses.Last().Name),
                _teamV2Responses.Last().SelectedParticipants.First().Email, "Inbox"),
                $"Could not find email with subject <{SharedConstants.PulseEmailSubject(_teamV2Responses.Last().Name)}> sent to {_teamV2Responses.Last().SelectedParticipants.First().Email}");

            Assert.IsFalse(GmailUtil.DoesMemberEmailExist(SharedConstants.PulseEmailSubject(_teamV2Responses.First().Name),
                    _teamV2Responses.First().SelectedParticipants.First().Email, "Inbox","UNREAD",5),
                $"Could not find email with subject <{SharedConstants.PulseEmailSubject(_teamV2Responses.First().Name)}> sent to {_teamV2Responses.First().SelectedParticipants.First().Email}");

            ResponseVerification(response);
        }

        //200
        [TestMethod]
        [TestCategory("KnownDefect")] //Bug Id: 51203
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public async Task PulseAssessmentV2_Post_BatchEmail_WithoutTeamId_Success()
        {
            //Given
            var client = await GetAuthenticatedClient();

            var query = new Dictionary<string, object>
            {
                { "companyId", Company.Id },
                { "pulseAssessmentId", _pulseAssessmentResponse.PulseAssessmentId }
            };

            //When 
            var response = await client.PostAsync<PulseAssessmentBatchEmailResponse>(RequestUris.PulseAssessmentV2BatchEmail().AddQueryParameter(query));

            //Then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status Code doesn't match");
            Assert.AreEqual(_teamV2Responses.SelectMany(a => a.SelectedParticipants).Count(), response.Dto.TotalEmailsSent, "Email sent count does not match");
            Assert.IsTrue(response.Dto.EmailSentAt.CompareTo(new DateTime()) != 0, "Sent time is null or empty");
            Assert.That.TimeIsClose(DateTime.UtcNow, response.Dto.EmailSentAt, 5, "Email time does not match");
        }

        //200
        [TestMethod]
        [TestCategory("KnownDefect")] //Bug Id: 51203
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public async Task PulseAssessmentV2_Post_BatchEmail_WithoutCompanyId_Success()
        {
            //Given
            var client = await GetAuthenticatedClient();

            var query = new Dictionary<string, object>
            {
                { "pulseAssessmentId", _pulseAssessmentResponse.PulseAssessmentId },
                { "teamId", _teamV2Responses.Last().TeamId}
            };

            //When 
            var response = await client.PostAsync<PulseAssessmentBatchEmailResponse>(RequestUris.PulseAssessmentV2BatchEmail().AddQueryParameter(query));

            //Then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status Code doesn't match");
            ResponseVerification(response);
        }

        //200
        [TestMethod]
        [TestCategory("KnownDefect")] //Bug Id: 51203
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public async Task PulseAssessmentV2_Post_BatchEmail_Resend_Success()
        {
            //Given
            var client = await GetAuthenticatedClient();

            //When 
            var response1 = await client.PostAsync<PulseAssessmentBatchEmailResponse>(RequestUris.PulseAssessmentV2BatchEmail().AddQueryParameter(_query));
            response1.EnsureSuccess();

            Assert.AreEqual(HttpStatusCode.OK, response1.StatusCode, "Status Code doesn't match");

            _teamV2Responses.Last().SelectedParticipants.ForEach(member => Assert.IsTrue(GmailUtil.DoesMemberEmailExist(SharedConstants.PulseEmailSubject(_teamV2Responses.Last().Name),
                    member.Email, "Inbox"),
                $"Could not find email with subject <{SharedConstants.PulseEmailSubject(_teamV2Responses.Last().Name)}> sent to {member.Email}"));
            ResponseVerification(response1);

            //Resend Reminders again
            var response2 = await client.PostAsync<PulseAssessmentBatchEmailResponse>(RequestUris.PulseAssessmentV2BatchEmail().AddQueryParameter(_query));

            //Then
            Assert.AreEqual(HttpStatusCode.OK, response2.StatusCode, "Status Code doesn't match");

            _teamV2Responses.Last().SelectedParticipants.ForEach(member => Assert.IsTrue(GmailUtil.DoesMemberEmailExist(SharedConstants.PulseEmailSubject(_teamV2Responses.Last().Name),
                    member.Email, "Inbox"),
                $"Could not find email with subject <{SharedConstants.PulseEmailSubject(_teamV2Responses.Last().Name)}> sent to {member.Email}"));
            ResponseVerification(response2);
        }

        //400
        [TestMethod]
        [TestCategory("KnownDefect")] //Bug Id: 51203
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public async Task PulseAssessmentV2_Post_BatchEmail_InvalidPulseId_BadRequest()
        {
            //Given
            var client = await GetAuthenticatedClient();

            //When 
            var response = await client.PostAsync<IList<string>>(RequestUris.PulseAssessmentV2BatchEmail().AddQueryParameter("companyId", Company.Id).AddQueryParameter("pulseAssessmentId", 00000));

            //Then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status Code doesn't match");
            Assert.AreEqual("'PulseAssessmentId' must be greater than 0", response.Dto.First(), "Error message doesn't match");
        }

        //400
        [TestMethod]
        [TestCategory("KnownDefect")] //Bug Id: 51203
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public async Task PulseAssessmentV2_Post_BatchEmail_InvalidTeamId_BadRequest()
        {
            //Given
            var client = await GetAuthenticatedClient();

            var query = new Dictionary<string, object>
            {
                { "companyId", Company.Id },
                { "pulseAssessmentId", _pulseAssessmentResponse.PulseAssessmentId },
                { "TeamId", 9999 }
            };

            //When 
            var response =
                await client.PostAsync<IList<string>>(RequestUris.PulseAssessmentV2BatchEmail().AddQueryParameter(query));

            //Then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status Code doesn't match");
            Assert.AreEqual("The batch emails were not sent either due to an error or no participants existed for an email to be sent.", response.Dto.First(), "Error message does not match");
        }

        //400
        [TestMethod]
        [TestCategory("KnownDefect")] //Bug Id: 51203
        [TestCategory("SiteAdmin")]
        public async Task PulseAssessmentV2_Post_BatchEmail_WithoutCompanyId_BadRequest()
        {
            //Given
            var client = await GetAuthenticatedClient();

            //When 
            var response = await client.PostAsync<IList<string>>(RequestUris.PulseAssessmentV2BatchEmail().AddQueryParameter("pulseAssessmentId", _pulseAssessmentResponse.PulseAssessmentId));

            //Then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status Code doesn't match");
            Assert.AreEqual("'Company Id' is not valid", response.Dto.First(), "Error message doesn't match");
        }

        //401
        [TestMethod]
        [TestCategory("KnownDefect")] //Bug Id: 51203
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public async Task PulseAssessmentV2_Post_BatchEmail_Unauthorized()
        {
            //Given
            var client = GetUnauthenticatedClient();

            //When 
            var response = await client.PostAsync<PulseAssessmentBatchEmailResponse>(RequestUris.PulseAssessmentV2BatchEmail().AddQueryParameter(_query));

            //Then
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status Code doesn't match");
        }

        //403
        [TestMethod]
        [TestCategory("KnownDefect")] //Bug Id: 51203
        [TestCategory("SiteAdmin")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public async Task PulseAssessmentV2_Post_BatchEmail_FakeCompanyId_Forbidden()
        {
            //Given
            var client = await GetAuthenticatedClient();
            var query = new Dictionary<string, object>
            {
                { "companyId", SharedConstants.FakeCompanyId },
                { "pulseAssessmentId", _pulseAssessmentResponse.PulseAssessmentId },
                { "teamId", _teams.TeamId }
            };

            //When 
            var response = await client.PostAsync<string>(RequestUris.PulseAssessmentV2BatchEmail().AddQueryParameter(query));

            //Then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status Code doesn't match");
        }

        //404
        [TestMethod]
        [TestCategory("KnownDefect")] //Bug Id: 51203
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public async Task PulseAssessmentV2_Post_BatchEmail_NotFound()
        {
            //Given
            var client = await GetAuthenticatedClient();

            //When 
            var response = await client.PostAsync<IList<string>>(RequestUris.PulseAssessmentV2BatchEmail().AddQueryParameter("companyId", Company.Id).AddQueryParameter("pulseAssessmentId", 99999));

            //Then
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode, "Status Code doesn't match");
        }

        //Assert
        public static void ResponseVerification(ApiResponse<PulseAssessmentBatchEmailResponse> response)
        {
            Assert.AreEqual(_teamV2Responses.Last().SelectedParticipants.Count, response.Dto.TotalEmailsSent, "Email sent count does not match");
            Assert.IsTrue(response.Dto.EmailSentAt.CompareTo(new DateTime()) != 0, "Sent time is null or empty");
            Assert.That.TimeIsClose(DateTime.UtcNow, response.Dto.EmailSentAt, 5, "Email time does not match");
        }
    }
}


