using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AtCommon.Api;
using AtCommon.Dtos;
using AtCommon.Dtos.Ideaboard;
using AtCommon.Dtos.IndividualAssessments;
using AtCommon.Dtos.Reports;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiTests.v1.Endpoints.Ideaboard
{
    [TestClass]
    [TestCategory("Ideaboard")]
    public class IdeaboardCardSortingTests : BaseV1Test
    {
        public static string ColumnName;
        public static Guid AssessmentUid;
        public static string NotesColumn = "Notes";
        private static IndividualAssessmentResponse _assessment;
        private static ApiResponse<IdeaBoardResponse> _createdCards;
        private static readonly UserConfig CompanyAdminUserConfig = new UserConfig("CA");
        private static User CompanyAdmin => CompanyAdminUserConfig.GetUserByDescription("user 1");
        private static readonly UserConfig MemberUserConfig = new UserConfig("M");
        private static User Member => MemberUserConfig.GetUserByDescription("user 1");

        [ClassInitialize]
        public static async Task ClassSetup(TestContext testContext)
        {
            var user = User;
            if (user.IsSiteAdmin() || user.IsMember())
            {
                user = CompanyAdmin;
            }

            var setup = new SetupTeardownApi(TestEnvironment);
            var team = TeamFactory.GetValidPostTeamWithMember("IdeaboardCardForAPI_");
            team.Members.Add(Member.ToAddMemberRequest());
            var teamResponse = setup.CreateTeam(team, user).GetAwaiter().GetResult();

            //Get radar details
            var radarDetails = setup.GetRadarDetailsBySurveyId(Company.Id, SharedConstants.IndividualAssessmentSurveyId);

            //Get up individual assessment members
            var individualAssessment = IndividualAssessmentFactory.GetValidIndividualAssessment(Company.Id, user.CompanyName, radarDetails.SurveyId);
            individualAssessment.Members = new List<IndividualAssessmentMemberRequest>
            {
                teamResponse.Members.FirstOrDefault(d=>d.FirstName==Member.FirstName).CheckForNull("No team members found in the response").ToAddIndividualMemberRequest()
            };
            individualAssessment.TeamUid = teamResponse.Uid;
            individualAssessment.Published = true;

            //Create individual assessment and get radar details
            _assessment = setup.CreateIndividualAssessment(individualAssessment, SharedConstants.IndividualAssessmentType, user).GetAwaiter().GetResult();

            //Arrange information for card
            AssessmentUid = _assessment.AssessmentList[0].AssessmentUid;
            var dimensionId = radarDetails.Dimensions.FirstOrDefault(d => d.Name != "Finish").CheckForNull().DimensionId;
            ColumnName = radarDetails.Dimensions.FirstOrDefault(d => d.DimensionId == dimensionId).CheckForNull().Name;

            // Create card
            setup.CreateIdeaboardCardForIndividualAssessment(individualAssessment, Company.Id, AssessmentUid, user, false, 3);
            setup.CreateIdeaboardCardForIndividualAssessment(individualAssessment, Company.Id, AssessmentUid, user, true, 3);

            //Get ideaboard cards
            _createdCards = await setup.GetIdeaboardCards(Company.Id, AssessmentUid, user);

            //Set votes allowed to the ideaboard
            setup.SetIdeaboardVotesAllowed(Company.Id, AssessmentUid, user);

        }

        [TestMethod]
        [TestCategory("SiteAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public async Task Ideaboard_SortCards_Dimension_Post_Success()
        {
            //Given
            var client = await GetAuthenticatedClient();

            //act: Give votes by admin
            var updateCardRequest = IdeaboardFactory.UpdateBulkCards(_createdCards);
            var dimensionCards = updateCardRequest.Cards.Where(d => d.ColumnName == ColumnName).ToList();
            dimensionCards[0].Votes = new Dictionary<string, int>() { { User.Username, 0 } };
            dimensionCards[1].Votes = new Dictionary<string, int>() { { User.Username, 1 } };
            dimensionCards[2].Votes = new Dictionary<string, int>() { { User.Username, 2 } };

            var updateCardResponseByAdmin = await client.PutAsync<UpdateCardsResponse>(RequestUris.IdeaboardCards().AddQueryParameter("companyId", Company.Id), updateCardRequest);
            updateCardResponseByAdmin.EnsureSuccess();

            var expectedVotesByAdmin = updateCardResponseByAdmin.Dto.Cards.Where(d => d.ColumnName == ColumnName).Select(n => n.Votes.Values.FirstOrDefault()).OrderByDescending(nd => nd).ToList();

            //Give votes by Member on same cards
            var memberClient = ClientFactory.GetAuthenticatedClient(Member.Username, Member.Password, TestEnvironment.EnvironmentName).GetAwaiter().GetResult();

            dimensionCards[0].Votes = new Dictionary<string, int>() { { Member.Username, 0 } };
            dimensionCards[1].Votes = new Dictionary<string, int>() { { Member.Username, 1 } };
            dimensionCards[2].Votes = new Dictionary<string, int>() { { Member.Username, 2 } };

            var updateCardResponseByMember = await memberClient.PutAsync<UpdateCardsResponse>(RequestUris.IdeaboardCards().AddQueryParameter("companyId", Company.Id), updateCardRequest);
            updateCardResponseByMember.EnsureSuccess();

            var expectedVotesByMember = updateCardResponseByMember.Dto.Cards.Where(d => d.ColumnName == ColumnName).Select(n => n.Votes.Values.FirstOrDefault()).OrderByDescending(nd => nd).ToList();

            //When
            //sorting
            var sortCardRequest = IdeaboardFactory.SortCardRequest(_assessment.AssessmentList[0].AssessmentUid);
            var sortCardResponse = await client.PostAsync<SortCardResponse>(RequestUris.IdeaboardSortCards().AddQueryParameter("companyId", Company.Id), sortCardRequest);
            Assert.AreEqual(HttpStatusCode.OK, sortCardResponse.StatusCode, "Status code does not match");

            var actualVotesOfAdmin = sortCardResponse.Dto.Cards.Where(d => d.ColumnName == ColumnName).OrderBy(n => n.SortOrder).Select(d => d.Votes).Select(n => n.Values.FirstOrDefault()).ToList();

            var actualVotesOfMember = await memberClient.GetAsync<IdeaBoardResponse>(RequestUris.IdeaboardGetCardsByAssessmentUid(_assessment.AssessmentList[0].AssessmentUid).AddQueryParameter("companyId", Company.Id));
            actualVotesOfMember.EnsureSuccess();

            var actualVotesByMember = actualVotesOfMember.Dto.Cards.Where(d => d.ColumnName == ColumnName).OrderBy(n => n.SortOrder).Select(d => d.Votes).Select(n => n.Values.FirstOrDefault()).ToList();

            //Then
            Assert.IsTrue(_createdCards.Dto.AssessmentUid == sortCardResponse.Dto.AssessmentUid, "The assessment UIds does not match");
            CollectionAssert.AreEqual(expectedVotesByAdmin, actualVotesOfAdmin, "Card votes does not sorted");
            CollectionAssert.AreEqual(expectedVotesByMember, actualVotesByMember, "Card votes does not sorted");
        }

        [TestMethod]
        [TestCategory("SiteAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public async Task Ideaboard_SortCards_Notes_Post_Success()
        {
            //Given
            var client = await GetAuthenticatedClient();

            //act: Give votes by Admin
            var updateCardRequest = IdeaboardFactory.UpdateBulkCards(_createdCards);
            var notesCards = updateCardRequest.Cards.Where(d => d.ColumnName == NotesColumn).ToList();
            notesCards[0].Votes = new Dictionary<string, int>() { { User.Username, 0 } };
            notesCards[1].Votes = new Dictionary<string, int>() { { User.Username, 3 } };
            notesCards[2].Votes = new Dictionary<string, int>() { { User.Username, 4 } };

            var updateCardResponseByAdmin = await client.PutAsync<UpdateCardsResponse>(RequestUris.IdeaboardCards().AddQueryParameter("companyId", Company.Id), updateCardRequest);
            updateCardResponseByAdmin.EnsureSuccess();

            var expectedVotesByAdmin = updateCardResponseByAdmin.Dto.Cards.Where(d => d.ColumnName == NotesColumn).Select(n => n.Votes.Values.FirstOrDefault()).OrderByDescending(nd => nd).ToList();

            //Give votes by Member on same cards
            var memberClient = ClientFactory.GetAuthenticatedClient(Member.Username, Member.Password, TestEnvironment.EnvironmentName).GetAwaiter().GetResult();

            notesCards[0].Votes = new Dictionary<string, int>() { { Member.Username, 0 } };
            notesCards[1].Votes = new Dictionary<string, int>() { { Member.Username, 3 } };
            notesCards[2].Votes = new Dictionary<string, int>() { { Member.Username, 4 } };

            var updateCardResponseByMember = await memberClient.PutAsync<UpdateCardsResponse>(RequestUris.IdeaboardCards().AddQueryParameter("companyId", Company.Id), updateCardRequest);
            updateCardResponseByMember.EnsureSuccess();

            var expectedVotesByMember = updateCardResponseByMember.Dto.Cards.Where(d => d.ColumnName == NotesColumn).Select(n => n.Votes.Values.FirstOrDefault()).OrderByDescending(nd => nd).ToList();

            //When
            //sorting
            var sortCardRequest = IdeaboardFactory.SortCardRequest(_assessment.AssessmentList[0].AssessmentUid);
            var sortCardResponse = await client.PostAsync<SortCardResponse>(RequestUris.IdeaboardSortCards().AddQueryParameter("companyId", Company.Id), sortCardRequest);
            Assert.AreEqual(HttpStatusCode.OK, sortCardResponse.StatusCode, "Status code does not match");

            var actualVotesOfAdmin = sortCardResponse.Dto.Cards.Where(d => d.ColumnName == NotesColumn).OrderBy(n => n.SortOrder).Select(d => d.Votes).Select(n => n.Values.FirstOrDefault()).ToList();

            var actualVotesOfMember = await memberClient.GetAsync<IdeaBoardResponse>(RequestUris.IdeaboardGetCardsByAssessmentUid(_assessment.AssessmentList[0].AssessmentUid).AddQueryParameter("companyId", Company.Id));
            actualVotesOfMember.EnsureSuccess();

            var actualVotesByMember = actualVotesOfMember.Dto.Cards.Where(d => d.ColumnName == NotesColumn).OrderBy(n => n.SortOrder).Select(d => d.Votes).Select(n => n.Values.FirstOrDefault()).ToList();

            //Then
            Assert.IsTrue(_createdCards.Dto.AssessmentUid == sortCardResponse.Dto.AssessmentUid, "The assessment UIds does not match");
            CollectionAssert.AreEqual(expectedVotesByAdmin, actualVotesOfAdmin, "Card votes does not sorted");
            CollectionAssert.AreEqual(expectedVotesByMember, actualVotesByMember, "Card votes does not sorted");
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("Member")]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public async Task Ideaboard_SortCards_Post_BadRequest()
        {
            //Given
            var client = await GetAuthenticatedClient();

            //sort card
            var sortBadRequest = new UpdateCardRequest()
            {
                AssessmentUid = new Guid(),
                SignalRGroupName = "",
                SignalRUserId = null,
            };

            var errorResponseList = new List<string>
            {
                "'Assessment Uid' is not valid",
                "'SignalRGroupName' should not be empty",
                "'SignalRUserId' should not be empty",
            };

            //When
            //act
            var sortCardResponse = await client.PostAsync<IList<string>>(RequestUris.IdeaboardSortCards().AddQueryParameter("companyId", Company.Id), sortBadRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.BadRequest, sortCardResponse.StatusCode, "The status codes does not match");
            Assert.That.ListsAreEqual(errorResponseList, sortCardResponse.Dto.ToList(), "Error list does not match");
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("Member")]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public async Task Ideaboard_SortCards_Post_Unauthorized()
        {
            //Given
            var client = GetUnauthenticatedClient();

            //When
            //act
            var assessmentUid = new Guid();
            var sortCardResponse = await client.PostAsync(RequestUris.IdeaboardSortCards().AddQueryParameter("companyId", Company.Id), assessmentUid.ToStringContent());

            //Then
            Assert.AreEqual(HttpStatusCode.Unauthorized, sortCardResponse.StatusCode, "Status codes does not match");
        }

        [TestMethod]
        [TestCategory("Member")]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public async Task Ideaboard_SortCards_Post_Forbidden()
        {
            //Given
            var client = await GetAuthenticatedClient();

            //When
            const int companyId = SharedConstants.FakeCompanyId;
            var sortCardRequest = IdeaboardFactory.SortCardRequest(AssessmentUid);
            var sortCardResponse = await client.PostAsync(RequestUris.IdeaboardSortCards().AddQueryParameter("companyId", companyId), sortCardRequest.ToStringContent());

            //Then
            Assert.AreEqual(HttpStatusCode.Forbidden, sortCardResponse.StatusCode, "Status codes does not match");
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("Member")]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public async Task Ideaboard_SortCards_Post_NotFound()
        {
            //Given
            var client = await GetAuthenticatedClient();

            var sortUnauthorized = new UpdateCardRequest()
            {
                AssessmentUid = Guid.NewGuid(),
                SignalRGroupName = "test",
                SignalRUserId = "test"
            };

            //When
            //act
            var sortCardResponse = await client.PostAsync<IList<string>>(RequestUris.IdeaboardSortCards().AddQueryParameter("companyId", Company.Id), sortUnauthorized);

            //Then
            Assert.AreEqual(HttpStatusCode.NotFound, sortCardResponse.StatusCode, "Status codes does not match");
        }
    }
}