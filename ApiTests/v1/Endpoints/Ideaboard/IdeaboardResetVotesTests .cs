using AtCommon.Api;
using AtCommon.Dtos.Ideaboard;
using AtCommon.Dtos.IndividualAssessments;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AtCommon.Dtos;

namespace ApiTests.v1.Endpoints.Ideaboard
{
    [TestClass]
    [TestCategory("Ideaboard")]
    public class IdeaboardResetVotesTests : BaseV1Test
    {
        public static Guid AssessmentUid;
        public static ApiResponse<IdeaBoardResponse> CreatedCards;
        private static readonly UserConfig MemberUserConfig = new UserConfig("M");
        private static User Member => MemberUserConfig.GetUserByDescription("user 1");

        [ClassInitialize]
        public static async Task ClassSetup(TestContext testContext)
        {
            if (!User.IsCompanyAdmin() && !User.IsBusinessLineAdmin() && !User.IsTeamAdmin()) return;
            var setup = new SetupTeardownApi(TestEnvironment);
            var client = ClientFactory.GetAuthenticatedClient(User.Username, User.Password, TestEnvironment.EnvironmentName).GetAwaiter().GetResult();

            //Create a team
            var team = TeamFactory.GetValidPostTeamWithMember("IdeaboardCardForAPI_");
            team.Members.Add(Member.ToAddMemberRequest());
            var teamResponse = setup.CreateTeam(team, User).GetAwaiter().GetResult();

            //Get radar details
            var radarDetails = setup.GetRadarDetailsBySurveyId(Company.Id, SharedConstants.IndividualAssessmentSurveyId);

            //get individual assessment members
            var individualAssessment = IndividualAssessmentFactory.GetValidIndividualAssessment(Company.Id, User.CompanyName, radarDetails.SurveyId);
            individualAssessment.Members = new List<IndividualAssessmentMemberRequest>
            {
                teamResponse.Members.FirstOrDefault(d=>d.FirstName==Member.FirstName).CheckForNull("No team members found in the response").ToAddIndividualMemberRequest()
            };
            individualAssessment.TeamUid = teamResponse.Uid;
            individualAssessment.Published = true;

            //Create individual assessment 
            var assessment = setup.CreateIndividualAssessment(individualAssessment, SharedConstants.IndividualAssessmentType).GetAwaiter().GetResult();
            AssessmentUid = assessment.AssessmentList[0].AssessmentUid;

            //Created ideaboard cards
            setup.CreateIdeaboardCardForIndividualAssessment( individualAssessment, Company.Id, AssessmentUid, User,false,3);

            //Get cards
            CreatedCards = await client.GetAsync<IdeaBoardResponse>(RequestUris.IdeaboardGetCardsByAssessmentUid(AssessmentUid).AddQueryParameter("companyId", Company.Id));

            //Set votes allowed to the ideaboard
            setup.SetIdeaboardVotesAllowed(Company.Id, AssessmentUid);
            
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public async Task Ideaboard_ResetVotes_Post_Success()
        {
            //Given
            var client = await GetAuthenticatedClient();

            //act: Give votes by Admin
            var updateCardRequest = IdeaboardFactory.UpdateBulkCards(CreatedCards);
            updateCardRequest.Cards[0].Votes = new Dictionary<string, int>() { { User.Username, 0 } };
            updateCardRequest.Cards[1].Votes = new Dictionary<string, int>() { { User.Username, 3 } };
            updateCardRequest.Cards[2].Votes = new Dictionary<string, int>() { { User.Username, 4 } };

            var updateCardResponseByAdmin = await client.PutAsync<UpdateCardsResponse>(RequestUris.IdeaboardCards().AddQueryParameter("companyId", Company.Id), updateCardRequest);
            updateCardResponseByAdmin.EnsureSuccess();

            //Give votes by Member on same cards
            var memberClient = ClientFactory.GetAuthenticatedClient(Member.Username, Member.Password, TestEnvironment.EnvironmentName).GetAwaiter().GetResult();

            updateCardRequest.Cards[0].Votes = new Dictionary<string, int>() { { Member.Username, 0 } };
            updateCardRequest.Cards[1].Votes = new Dictionary<string, int>() { { Member.Username, 3 } };
            updateCardRequest.Cards[2].Votes = new Dictionary<string, int>() { { Member.Username, 4 } };

            var updateCardResponseByMember = await memberClient.PutAsync<UpdateCardsResponse>(RequestUris.IdeaboardCards().AddQueryParameter("companyId", Company.Id), updateCardRequest);
            updateCardResponseByMember.EnsureSuccess();

            //When
            var resetCardRequests = IdeaboardFactory.ResetCardsRequest(AssessmentUid);
            var resetCardResponse = await client.PostAsync<UpdateCardsResponse>(RequestUris.IdeaboardResetVotes().AddQueryParameter("companyId", Company.Id), resetCardRequests);

            var actualVotesOfMember = await memberClient.GetAsync<IdeaBoardResponse>(RequestUris.IdeaboardGetCardsByAssessmentUid(AssessmentUid).AddQueryParameter("companyId", Company.Id));
            actualVotesOfMember.EnsureSuccess();

            //Then
            Assert.AreEqual(HttpStatusCode.OK, resetCardResponse.StatusCode, "The status codes does not match");
            Assert.AreEqual(resetCardRequests.AssessmentUid, resetCardResponse.Dto.AssessmentUid, "The assessment Uid does not match");
            foreach (var card in resetCardResponse.Dto.Cards)
            {
                Assert.IsTrue(card.Votes == null, "The votes are not null");
            }

            foreach (var memberCard in actualVotesOfMember.Dto.Cards)
            {
                Assert.IsTrue(memberCard.Votes == null, "The votes are not null");
            }
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public async Task Ideaboard_ResetVotes_Post_BadRequest_With_EmptyAssessmentUid()
        {
            //Given
            var client = await GetAuthenticatedClient();
            var resetVotesRequest = IdeaboardFactory.ResetCardsRequest(new Guid());
            var errorResponseList = new List<string>
            {
                "'Assessment Uid' must not be empty."
            };

            //When
            var resetCardResponse = await client.PostAsync<List<string>>(RequestUris.IdeaboardResetVotes().AddQueryParameter("companyId", Company.Id), resetVotesRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.BadRequest, resetCardResponse.StatusCode, "The status codes does not match");
            Assert.That.ListsAreEqual(errorResponseList, resetCardResponse.Dto.ToList(), "Error list does not match");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public async Task Ideaboard_ResetVotes_Post_BadRequest_With_RandomAssessmentUid()
        {
            //Given
            var client = await GetAuthenticatedClient();
            var resetVotesRequest = IdeaboardFactory.ResetCardsRequest(Guid.NewGuid());

            //When
            var resetCardResponse = await client.PostAsync(RequestUris.IdeaboardResetVotes().AddQueryParameter("companyId", Company.Id), resetVotesRequest.ToStringContent());

            //Then
            Assert.AreEqual(HttpStatusCode.BadRequest, resetCardResponse.StatusCode, "The status codes does not match");
        }

        [TestMethod]
        [TestCategory("SiteAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public async Task Ideaboard_ResetVotes_Post_Unauthorized()
        {
            //Given
            var client = GetUnauthenticatedClient();
            var resetVotesRequest = IdeaboardFactory.ResetCardsRequest(AssessmentUid);

            //When
            var resetCardResponse = await client.PostAsync(RequestUris.IdeaboardResetVotes().AddQueryParameter("companyId", Company.Id), resetVotesRequest.ToStringContent());

            //Then
            Assert.AreEqual(HttpStatusCode.Unauthorized, resetCardResponse.StatusCode, "The status codes does not match");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public async Task Ideaboard_ResetVotes_Post_Forbidden()
        {
            //Given
            var client = await GetAuthenticatedClient();
            var resetVotesRequest = IdeaboardFactory.ResetCardsRequest(AssessmentUid);

            //When
            var resetCardResponse = await client.PostAsync(RequestUris.IdeaboardResetVotes().AddQueryParameter("companyId", SharedConstants.FakeCompanyId), resetVotesRequest.ToStringContent());

            //Then
            Assert.AreEqual(HttpStatusCode.Forbidden, resetCardResponse.StatusCode, "The status codes does not match");
        }
    }
}