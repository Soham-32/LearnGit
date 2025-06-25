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
    public class IdeaboardGetVotesAllowedTests : BaseV1Test
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

            //create individual assessment & fetching assessment UID
            var assessment = setup.CreateIndividualAssessment(individualAssessment, SharedConstants.IndividualAssessmentType, user).GetAwaiter().GetResult();
            AssessmentUid = assessment.AssessmentList.First().AssessmentUid;

            //create card
            setup.CreateIdeaboardCardForIndividualAssessment(individualAssessment, Company.Id, AssessmentUid,user);

            //set request
            SetVotesAllowedRequest = IdeaboardFactory.SetVotesAllowedRequest(AssessmentUid, Company.Id);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public async Task Ideaboard_VotesAllowed_Get_Success()
        {
            //given
            var client = await GetAuthenticatedClient();

            //set allowed votes
            var setVotesAllowedResponse = await client.PutAsync<IdeaBoardResponse>(RequestUris.IdeaboardVotesAllowed(), SetVotesAllowedRequest);
            setVotesAllowedResponse.EnsureSuccess();

            //when
            //get allowed votes
            var  getVotesAllowedResponse = await client.GetAsync<IdeaBoardResponse>(RequestUris.IdeaboardVotesAllowed().AddQueryParameter("assessmentUid", AssessmentUid));

            //then
            Assert.AreEqual(HttpStatusCode.OK, getVotesAllowedResponse.StatusCode,"Status code does not match");
            Assert.AreEqual(setVotesAllowedResponse.Dto.AssessmentUid,  getVotesAllowedResponse.Dto.AssessmentUid, "AssessmentUid does not match");
            Assert.AreEqual(setVotesAllowedResponse.Dto.Name, getVotesAllowedResponse.Dto.Name,"Radar Name does not match");
            Assert.AreEqual(setVotesAllowedResponse.Dto.VotesAllowed,  getVotesAllowedResponse.Dto.VotesAllowed, "Allowed votes does not match");
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public async Task Ideaboard_VotesAllowed_Get_BadRequest()
        {
            //given
            var client = await GetAuthenticatedClient();

            var errorResponseList = new List<string>
            {
                "'Assessment Uid' must not be empty."
            };

            //when
            //get allowed votes
            var assessmentUid = new Guid();
            var getVotesAllowedResponse = await client.GetAsync<IList<string>>(RequestUris.IdeaboardVotesAllowed().AddQueryParameter("assessmentUid", assessmentUid));

            //then
            Assert.AreEqual(HttpStatusCode.BadRequest, getVotesAllowedResponse.StatusCode, "The status codes does not match");
            Assert.That.ListsAreEqual(errorResponseList, getVotesAllowedResponse.Dto.ToList(), "Error list does not match");
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public async Task Ideaboard_VotesAllowed_Get_Unauthorized()
        {
            //given
            var client = GetUnauthenticatedClient();

            //when
            //get allowed votes
            var getVotesAllowedResponse = await client.GetAsync(RequestUris.IdeaboardVotesAllowed().AddQueryParameter("assessmentUid", AssessmentUid));

            //then
            Assert.AreEqual(HttpStatusCode.Unauthorized, getVotesAllowedResponse.StatusCode, "Status codes does not match");

        }
    }
}