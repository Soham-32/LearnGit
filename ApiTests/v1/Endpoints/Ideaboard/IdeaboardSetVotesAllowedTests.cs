using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AtCommon.Api;
using AtCommon.Dtos.Ideaboard;
using AtCommon.Dtos.IndividualAssessments;
using AtCommon.Dtos.Radars;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiTests.v1.Endpoints.Ideaboard
{
    [TestClass]
    [TestCategory("Ideaboard")]
    public class IdeaboardSetVotesAllowedTests : BaseV1Test
    {
        public static Guid AssessmentUid;
        public static RadarDetailResponse RadarDetailResponse;
        public static SetVotesAllowedRequests SetVotesAllowedRequest;
        private static readonly UserConfig CompanyAdminUserConfig = new UserConfig("CA");

        [ClassInitialize]
        public static void ClassSetup(TestContext testContext)
        {
            var user = User;
            if (user.IsSiteAdmin())
            {
                user = CompanyAdminUserConfig.GetUserByDescription("user 1");
            }

            var setup = new SetupTeardownApi(TestEnvironment);
            var team = TeamFactory.GetValidPostTeamWithMember("IdeaboardCardForAPI_");
            var teamResponse = setup.CreateTeam(team, user).GetAwaiter().GetResult();

            //get radar details
            RadarDetailResponse = setup.GetRadarDetailsBySurveyId(Company.Id, SharedConstants.IndividualAssessmentSurveyId);

            //get up individual assessment members
            var individualAssessment = IndividualAssessmentFactory.GetValidIndividualAssessment(Company.Id, user.CompanyName, RadarDetailResponse.SurveyId);
            individualAssessment.Members = new List<IndividualAssessmentMemberRequest>
            {
                teamResponse.Members.FirstOrDefault().CheckForNull("No team members found in the response").ToAddIndividualMemberRequest()
            };
            individualAssessment.TeamUid = teamResponse.Uid;
            individualAssessment.Published = true;

            //create individual assessment & fetching Assessment UID
            var assessment = setup.CreateIndividualAssessment(individualAssessment, SharedConstants.IndividualAssessmentType, user).GetAwaiter().GetResult();
            AssessmentUid = assessment.AssessmentList.First().AssessmentUid;

            //create card
            setup.CreateIdeaboardCardForIndividualAssessment(individualAssessment, Company.Id,AssessmentUid, user);

            // set request
            SetVotesAllowedRequest = IdeaboardFactory.SetVotesAllowedRequest(AssessmentUid, Company.Id);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public async Task Ideaboard_VotesAllowed_Put_Success()
        {
            //given
            var client = await GetAuthenticatedClient();

            //when
            //set allowed votes
            var setVotesAllowedResponse = await client.PutAsync<IdeaBoardResponse>(RequestUris.IdeaboardVotesAllowed(), SetVotesAllowedRequest);

            //then
            Assert.AreEqual(HttpStatusCode.OK, setVotesAllowedResponse.StatusCode, "Status code does not match");
            Assert.AreEqual(RadarDetailResponse.Name, setVotesAllowedResponse.Dto.Name, "Radar name does not match");
            Assert.AreEqual(SetVotesAllowedRequest.AssessmentUid, setVotesAllowedResponse.Dto.AssessmentUid, "AssessmentUid does not match");
            Assert.AreEqual(SetVotesAllowedRequest.VotesAllowed, setVotesAllowedResponse.Dto.VotesAllowed, "Allowed votes does not match");
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public async Task Ideaboard_VotesAllowed_Put_BadRequest()
        {
            //given
            var client = await GetAuthenticatedClient();

            var setVotesAllowedRequest = new SetVotesAllowedRequests
            {
                AssessmentUid = new Guid(),
                VotesAllowed = 0,
                CompanyId = 0

            };
            var errorResponseList = new List<string>
            {
                "'Assessment Uid' must not be empty.",
                "'Votes Allowed' must not be empty.",
                "'Company Id' must not be empty.",
            };

            //when
            //set allowed votes
            var setVotesAllowedResponse = await client.PutAsync<IList<string>>(RequestUris.IdeaboardVotesAllowed(), setVotesAllowedRequest);

            //then
            Assert.AreEqual(HttpStatusCode.BadRequest, setVotesAllowedResponse.StatusCode, "Status codes does not match");
            Assert.That.ListsAreEqual(errorResponseList, setVotesAllowedResponse.Dto.ToList(), "Error list does not match");
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public async Task Ideaboard_VotesAllowed_Put_Unauthorized()
        {
            //given
            var client = GetUnauthenticatedClient();

            //when
            //set allowed votes
            var setVotesAllowedResponse = await client.PutAsync(RequestUris.IdeaboardVotesAllowed(), SetVotesAllowedRequest.ToStringContent());

            //then
            Assert.AreEqual(HttpStatusCode.Unauthorized, setVotesAllowedResponse.StatusCode, "Status codes does not match");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public async Task Ideaboard_VotesAllowed_Put_Forbidden()
        {
            //given
            var client = await GetAuthenticatedClient();

            //when
            //set allowed votes
            var setVoteAllowedRequest = IdeaboardFactory.SetVotesAllowedRequest(AssessmentUid, SharedConstants.FakeCompanyId);
            var setVoteAllowedResponse = await client.PutAsync(RequestUris.IdeaboardVotesAllowed(), setVoteAllowedRequest.ToStringContent());

            //then
            Assert.AreEqual(HttpStatusCode.Forbidden, setVoteAllowedResponse.StatusCode, "Status codes does not match");
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public async Task Ideaboard_VotesAllowed_Put_NotFound()
        {
            //given
            var client = await GetAuthenticatedClient();

            //when
            //set allowed votes
            var setVoteAllowedRequest = IdeaboardFactory.SetVotesAllowedRequest(Guid.NewGuid(), Company.Id);
            var setVoteAllowedResponse = await client.PutAsync(RequestUris.IdeaboardVotesAllowed(), setVoteAllowedRequest.ToStringContent());

            //then
            Assert.AreEqual(HttpStatusCode.NotFound, setVoteAllowedResponse.StatusCode, "Status codes does not match");
        }
    }
}