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
    public class PulseAssessmentV2AddEmailTests : PulseApiTestBase
    {
        private static CreatePulseAssessmentResponse _pulseAssessmentResponse;
        private static SavePulseAssessmentV2Request _pulseAddRequest;
        private static readonly UserConfig CompanyAdminUserConfig = new UserConfig("CA");
        private static Dictionary<string, object> _query;
        private static int _teamId;

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
            var teamWithTeamMemberResponse = setupApi.GetTeamWithTeamMemberResponse(_teamId, user);

            //Get radar details
            var radarResponse = setupApi.GetRadarQuestionDetailsV2(Company.Id, _teamId, SharedConstants.AtTeamHealth3SurveyId,user);

            _pulseAddRequest = PulseV2Factory.PulseAssessmentV2AddRequest(radarResponse, teamWithTeamMemberResponse, _teamId);

            _pulseAssessmentResponse = setupApi.CreatePulseAssessmentV2(_pulseAddRequest, Company.Id, user);

            _query = new Dictionary<string, object>
            {
                { "pulseAssessmentId", _pulseAssessmentResponse.PulseAssessmentId },
                { "teamId", _teamId },
                { "memberUid", _pulseAddRequest.SelectedTeams.First().SelectedParticipants.First().Uid }
            };
        }

        //200
        [TestMethod]
        [TestCategory("KnownDefect")] //Bug Id: 51203
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public async Task PulseAssessmentV2_Post_Email_Success()
        {
            //Given
            var client = await GetAuthenticatedClient();

            //When 
            var response = await client.PostAsync<PulseAssessmentEmailResponse>(RequestUris.PulseAssessmentV2Email().AddQueryParameter("companyId", Company.Id).AddQueryParameter(_query));

            //Then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status Code doesn't match");
            Assert.IsTrue(response.Dto.EmailSentAt.CompareTo(new DateTime()) != 0, "Sent time is null or empty");
            Assert.That.TimeIsClose(response.Dto.EmailSentAt.ToUniversalTime(), DateTime.UtcNow);
        }

        //200
        [TestMethod]
        [TestCategory("KnownDefect")] //Bug Id: 51203
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public async Task PulseAssessmentV2_Post_Email_WithoutCompanyId_Success()
        {
            //Given
            var client = await GetAuthenticatedClient();

            //When 
            var response = await client.PostAsync<PulseAssessmentEmailResponse>(RequestUris.PulseAssessmentV2Email().AddQueryParameter(_query));

            //Then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status Code doesn't match");
            Assert.IsTrue(response.Dto.EmailSentAt.CompareTo(new DateTime()) != 0, "Sent time is null or empty");
            Assert.That.TimeIsClose(response.Dto.EmailSentAt.ToUniversalTime(), DateTime.UtcNow);
        }

        //400
        [TestMethod]
        [TestCategory("KnownDefect")] //Bug Id: 51203
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public async Task PulseAssessmentV2_Post_Email_WithInValidPulseAssessmentId_BadRequest()
        {
            //Given
            var client = await GetAuthenticatedClient();

            var query = new Dictionary<string, object>
            {
                { "companyId", Company.Id },
                { "pulseAssessmentId", 00000 },
                { "teamId", _teamId },
                { "memberUid", _pulseAddRequest.SelectedTeams.First().SelectedParticipants.First().Uid }
            };

            //When 
            var response = await client.PostAsync<IList<string>>(RequestUris.PulseAssessmentV2Email().AddQueryParameter(query));

            //Then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status Code doesn't match");
            Assert.AreEqual("'PulseAssessmentId' must be greater than 0", response.Dto.First(), "Error message doesn't match");
        }

        //401
        [TestMethod]
        [TestCategory("KnownDefect")] //Bug Id: 51203
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("Member")]
        public async Task PulseAssessmentV2_Post_Email_Unauthorized()
        {
            //Given
            var client = GetUnauthenticatedClient();

            //When 
            var response = await client.PostAsync<PulseAssessmentEmailResponse>(RequestUris.PulseAssessmentV2Email().AddQueryParameter("companyId", Company.Id).AddQueryParameter(_query));

            //Then
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status Code doesn't match");
        }

        //403
        [TestMethod]
        [TestCategory("KnownDefect")] //Bug Id: 51203
        [TestCategory("PartnerAdmin")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public async Task PulseAssessmentV2_Post_Email_WithFakeCompanyId_Forbidden()
        {
            //Given
            var client = await GetAuthenticatedClient();

            //When 
            var response = await client.PostAsync<PulseAssessmentEmailResponse>(RequestUris.PulseAssessmentV2Email().AddQueryParameter("companyId", SharedConstants.FakeCompanyId).AddQueryParameter(_query));

            //Then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status Code doesn't match");
        }

        //404
        [TestMethod]
        [TestCategory("KnownDefect")] //Bug Id: 51203
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public async Task PulseAssessmentV2_Post_SendEmail_WithFakePulseAssessmentId_NotFound()
        {
            //Given
            var client = await GetAuthenticatedClient();

            var query = new Dictionary<string, object>
            {
                { "companyId", Company.Id },
                { "pulseAssessmentId", 99999 },
                { "teamId", _teamId },
                { "memberUid", _pulseAddRequest.SelectedTeams.First().SelectedParticipants.First().Uid}
            };

            //When 
            var response = await client.PostAsync<IList<string>>(RequestUris.PulseAssessmentV2Email().AddQueryParameter(query));

            //Then
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode, "Status Code doesn't match");
        }

        //404
        [TestMethod]
        [TestCategory("KnownDefect")] //Bug Id: 51203
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public async Task PulseAssessmentV2_Post_SendEmail_WithFakeTeamId_NotFound()
        {
            //Given
            var client = await GetAuthenticatedClient();

            var query = new Dictionary<string, object>
            {
                { "companyId", Company.Id },
                { "pulseAssessmentId",  _pulseAssessmentResponse.PulseAssessmentId },
                { "teamId", 9999 },
                { "memberUid", _pulseAddRequest.SelectedTeams.First().SelectedParticipants.First().Uid}
            };

            //When 
            var response = await client.PostAsync<IList<string>>(RequestUris.PulseAssessmentV2Email().AddQueryParameter(query));

            //Then
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode, "Status Code doesn't match");
        }

        //404
        [TestMethod]
        [TestCategory("KnownDefect")] //Bug Id: 51203
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public async Task PulseAssessmentV2_Post_Email_RandomGuidMember_NotFound()
        {
            //Given
            var client = await GetAuthenticatedClient();

            var query = new Dictionary<string, object>
            {
                { "companyId", Company.Id },
                { "pulseAssessmentId", _pulseAssessmentResponse.PulseAssessmentId },
                { "teamId", _teamId },
                { "memberUid", Guid.NewGuid() }
            };

            //When 
            var response = await client.PostAsync<IList<string>>(RequestUris.PulseAssessmentV2Email().AddQueryParameter(query));

            //Then
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode, "Status Code doesn't match");
        }
    }
}


