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
    public class IdeaboardUpdateCardTests : BaseV1Test
    {
        public static int? DimensionId;
        public static string ColumnName;
        public static Guid AssessmentUid;
        public static string NotesColumn = "Notes";
        public static List<CreateCardResponse> DimensionCardResponse;
        public static List<CreateCardResponse> NotesCardResponse;
        private static readonly UserConfig MemberUserConfig = new UserConfig("M");
        private static User Member1 => MemberUserConfig.GetUserByDescription("user 1");
        private static User Member2 => MemberUserConfig.GetUserByDescription("user 2");
        private const int VoteCount = 2;
        private readonly UpdateCardRequest UpdateCard = new UpdateCardRequest
        {
            AssessmentUid = Guid.NewGuid(),
            SignalRGroupName = "Ideaboard",
            SignalRUserId = "Guid.NewGuid()",
            Card = new CardRequest
            {
                ItemId = 1,
                DimensionId = 1,
                ColumnName = "ColumnName",
                SortOrder = 1,
                ItemText = Guid.NewGuid().ToString()
            }
        };

        [ClassInitialize]
        public static void ClassSetup(TestContext testContext)
        {
            if (!User.IsCompanyAdmin() && !User.IsBusinessLineAdmin() && !User.IsTeamAdmin()) return;
            var setup = new SetupTeardownApi(TestEnvironment);
            var team = TeamFactory.GetValidPostTeamWithMember("IdeaboardCardForAPI_");
            team.Members.Add(Member1.ToAddMemberRequest());
            team.Members.Add(Member2.ToAddMemberRequest());
            var teamResponse = setup.CreateTeam(team, User).GetAwaiter().GetResult();

            //get radar details
            var radarDetails = setup.GetRadarDetailsBySurveyId(Company.Id, SharedConstants.IndividualAssessmentSurveyId);

            //get up individual assessment members
            var individualAssessment = IndividualAssessmentFactory.GetValidIndividualAssessment(Company.Id, User.CompanyName, radarDetails.SurveyId);
            individualAssessment.Members = new List<IndividualAssessmentMemberRequest>
            {
                teamResponse.Members.FirstOrDefault(d=>d.FirstName==Member1.FirstName).CheckForNull("No team members found in the response").ToAddIndividualMemberRequest(),
                teamResponse.Members.FirstOrDefault(d=>d.LastName==Member2.LastName).CheckForNull("No team members found in the response").ToAddIndividualMemberRequest()
            };
            individualAssessment.TeamUid = teamResponse.Uid;
            individualAssessment.Published = true;

            //create individual assessment and get radar details
            var assessment = setup.CreateIndividualAssessment(individualAssessment, SharedConstants.IndividualAssessmentType).GetAwaiter().GetResult();

            //arrange information for card
            AssessmentUid = assessment.AssessmentList[0].AssessmentUid;
            DimensionId = radarDetails.Dimensions.FirstOrDefault(d => d.Name != "Finish").CheckForNull().DimensionId;
            ColumnName = radarDetails.Dimensions.FirstOrDefault(d => d.DimensionId == DimensionId).CheckForNull().Name;

            NotesCardResponse = setup.CreateIdeaboardCardForIndividualAssessment(individualAssessment, Company.Id, assessment.AssessmentList.First().AssessmentUid, User, true);
            DimensionCardResponse = setup.CreateIdeaboardCardForIndividualAssessment(individualAssessment, Company.Id, assessment.AssessmentList.First().AssessmentUid, User);

            setup.SetIdeaboardVotesAllowed(Company.Id, AssessmentUid);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public async Task Ideaboard_UpdateCards_Dimension_Put_Success()
        {
            //Given
            var client = await GetAuthenticatedClient();

            var updateCardRequest = IdeaboardFactory.UpdateCard(DimensionCardResponse);

            //When
            var memberClient = ClientFactory.GetAuthenticatedClient(Member1.Username, Member1.Password, TestEnvironment.EnvironmentName).GetAwaiter().GetResult();

            //act: update card  with first Member
            updateCardRequest.Card.Votes = new Dictionary<string, int>() { { Member1.Username, VoteCount } };
            var responseOfMember1 = await memberClient.PutAsync<UpdateCardResponse>(RequestUris.IdeaboardCard().AddQueryParameter("companyId", Company.Id), updateCardRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.OK, responseOfMember1.StatusCode, "The status codes does not match");
            Assert.AreNotEqual(DimensionCardResponse.First().Card.ItemText, responseOfMember1.Dto.Card.ItemText, "The text on the card was not updated");
            Assert.AreEqual(DimensionCardResponse.First().Card.DimensionId, responseOfMember1.Dto.Card.DimensionId, "The dimension ids does not match");
            Assert.AreEqual(DimensionCardResponse.First().AssessmentUid, responseOfMember1.Dto.AssessmentUid, "The assessment Uid does not match");
            Assert.AreEqual(DimensionCardResponse.First().Card.ItemId, responseOfMember1.Dto.Card.ItemId, "The item ids does not match");
            Assert.AreEqual(DimensionCardResponse.First().Card.ColumnName, responseOfMember1.Dto.Card.ColumnName, "The column names does not match");
            Assert.AreEqual(DimensionCardResponse.First().Card.SortOrder, responseOfMember1.Dto.Card.SortOrder, "The sort order does not match");
            Assert.AreEqual(Member1.Username, responseOfMember1.Dto.Card.Votes.First().Key, "Member's name does not match");
            Assert.AreEqual(VoteCount, responseOfMember1.Dto.Card.Votes.First().Value, "Vote count is not valid.");

            //When
            memberClient = ClientFactory.GetAuthenticatedClient(Member2.Username, Member2.Password, TestEnvironment.EnvironmentName).GetAwaiter().GetResult();

            //act: update card with second Member
            updateCardRequest.Card.Votes = new Dictionary<string, int>() { { Member2.Username, VoteCount } };
            var responseOfMember2 = await memberClient.PutAsync<UpdateCardResponse>(RequestUris.IdeaboardCard().AddQueryParameter("companyId", Company.Id), updateCardRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.OK, responseOfMember2.StatusCode, "The status codes does not match");
            Assert.AreNotEqual(DimensionCardResponse.First().Card.ItemText, responseOfMember2.Dto.Card.ItemText, "The text on the card was not updated");
            Assert.AreEqual(DimensionCardResponse.First().Card.DimensionId, responseOfMember2.Dto.Card.DimensionId, "The dimension ids does not match");
            Assert.AreEqual(DimensionCardResponse.First().AssessmentUid, responseOfMember2.Dto.AssessmentUid, "The assessment Uid does not match");
            Assert.AreEqual(DimensionCardResponse.First().Card.ItemId, responseOfMember2.Dto.Card.ItemId, "The item ids does not match");
            Assert.AreEqual(DimensionCardResponse.First().Card.ColumnName, responseOfMember2.Dto.Card.ColumnName, "The column names does not match");
            Assert.AreEqual(DimensionCardResponse.First().Card.SortOrder, responseOfMember2.Dto.Card.SortOrder, "The sort order does not match");
            Assert.AreEqual(Member1.Username, responseOfMember2.Dto.Card.Votes.First().Key, "Member's name does not match");
            Assert.AreEqual(VoteCount, responseOfMember2.Dto.Card.Votes.First().Value, "Vote count is not valid.");
            Assert.AreEqual(Member2.Username, responseOfMember2.Dto.Card.Votes.Last().Key, "Member's name does not match");
            Assert.AreEqual(VoteCount, responseOfMember2.Dto.Card.Votes.Last().Value, "Vote count is not valid.");

            // get cards 
            var getCards = await client.GetAsync<IdeaBoardResponse>(RequestUris.IdeaboardGetCardsByAssessmentUid(AssessmentUid).AddQueryParameter("companyId", Company.Id));

            //Then
            Assert.AreEqual(HttpStatusCode.OK, responseOfMember2.StatusCode, "The status codes does not match");
            Assert.AreEqual(getCards.Dto.AssessmentUid, DimensionCardResponse.First().AssessmentUid, "The assessment Uid does not match");
            Assert.That.ListContains(getCards.Dto.Cards.Select(d => d.ItemText.ToString()).ToList(), responseOfMember2.Dto.Card.ItemText, "The Item Text does not match");
            Assert.That.ListContains(getCards.Dto.Cards.Select(d => d.DimensionId.ToString()).ToList(), DimensionCardResponse.First().Card.DimensionId.ToString(), "The dimension ids does not match");
            Assert.That.ListContains(getCards.Dto.Cards.Select(d => d.ItemId.ToString()).ToList(), DimensionCardResponse.First().Card.ItemId.ToString(), "The item ids does not match");
            Assert.That.ListContains(getCards.Dto.Cards.Select(d => d.ColumnName.ToString()).ToList(), DimensionCardResponse.First().Card.ColumnName, "The column names does not match");
            Assert.That.ListContains(getCards.Dto.Cards.Where(d => d.ColumnName == ColumnName).Select(d => d.Votes.First().Key).ToList(), Member1.Username, "Member does not match");
            Assert.That.ListContains(getCards.Dto.Cards.Where(d => d.ColumnName == ColumnName).Select(d => d.Votes.First().Value.ToString()).ToList(), VoteCount.ToString(), "Vote count is not valid.");
            Assert.That.ListContains(getCards.Dto.Cards.Where(d => d.ColumnName == ColumnName).Select(d => d.Votes.Last().Key).ToList(), Member2.Username, "Member does not match");
            Assert.That.ListContains(getCards.Dto.Cards.Where(d => d.ColumnName == ColumnName).Select(d => d.Votes.Last().Value.ToString()).ToList(), VoteCount.ToString(), "Vote count is not valid.");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public async Task Ideaboard_UpdateCards_Notes_Put_Success()
        {
            //Given
            var client = await GetAuthenticatedClient();

            var updateCardRequest = IdeaboardFactory.UpdateCard(NotesCardResponse);

            //When
            var memberClient = ClientFactory.GetAuthenticatedClient(Member1.Username, Member1.Password, TestEnvironment.EnvironmentName).GetAwaiter().GetResult();

            //act: update card  with first Member
            updateCardRequest.Card.Votes = new Dictionary<string, int>() { { Member1.Username, VoteCount } };
            var responseOfMember1 = await memberClient.PutAsync<UpdateCardResponse>(RequestUris.IdeaboardCard().AddQueryParameter("companyId", Company.Id), updateCardRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.OK, responseOfMember1.StatusCode, "The status codes does not match");
            Assert.AreNotEqual(NotesCardResponse.First().Card.ItemText, responseOfMember1.Dto.Card.ItemText, "The text on the card was not updated");
            Assert.AreEqual(NotesCardResponse.First().Card.DimensionId, responseOfMember1.Dto.Card.DimensionId, "The dimension ids does not match");
            Assert.AreEqual(NotesCardResponse.First().AssessmentUid, responseOfMember1.Dto.AssessmentUid, "The assessment Uid does not match");
            Assert.AreEqual(NotesCardResponse.First().Card.ItemId, responseOfMember1.Dto.Card.ItemId, "The item ids does not match");
            Assert.AreEqual(NotesCardResponse.First().Card.ColumnName, responseOfMember1.Dto.Card.ColumnName, "The column names does not match");
            Assert.AreEqual(NotesCardResponse.First().Card.SortOrder, responseOfMember1.Dto.Card.SortOrder, "The sort order does not match");
            Assert.AreEqual(Member1.Username, responseOfMember1.Dto.Card.Votes.First().Key, "Member's name does not match");
            Assert.AreEqual(VoteCount, responseOfMember1.Dto.Card.Votes.First().Value, "Vote count is not valid.");

            //When
            memberClient = ClientFactory.GetAuthenticatedClient(Member2.Username, Member2.Password, TestEnvironment.EnvironmentName).GetAwaiter().GetResult();

            //act: update card with second Member
            updateCardRequest.Card.Votes = new Dictionary<string, int>() { { Member2.Username, VoteCount } };
            var responseOfMember2 = await memberClient.PutAsync<UpdateCardResponse>(RequestUris.IdeaboardCard().AddQueryParameter("companyId", Company.Id), updateCardRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.OK, responseOfMember2.StatusCode, "The status codes does not match");
            Assert.AreNotEqual(NotesCardResponse.First().Card.ItemText, responseOfMember2.Dto.Card.ItemText, "The text on the card was not updated");
            Assert.AreEqual(NotesCardResponse.First().Card.DimensionId, responseOfMember2.Dto.Card.DimensionId, "The dimension ids does not match");
            Assert.AreEqual(NotesCardResponse.First().AssessmentUid, responseOfMember2.Dto.AssessmentUid, "The assessment Uid does not match");
            Assert.AreEqual(NotesCardResponse.First().Card.ItemId, responseOfMember2.Dto.Card.ItemId, "The item ids does not match");
            Assert.AreEqual(NotesCardResponse.First().Card.ColumnName, responseOfMember2.Dto.Card.ColumnName, "The column names does not match");
            Assert.AreEqual(NotesCardResponse.First().Card.SortOrder, responseOfMember2.Dto.Card.SortOrder, "The sort order does not match");
            Assert.AreEqual(Member1.Username, responseOfMember2.Dto.Card.Votes.First().Key, "Member's name does not match");
            Assert.AreEqual(VoteCount, responseOfMember2.Dto.Card.Votes.First().Value, "Vote count is not valid.");
            Assert.AreEqual(Member2.Username, responseOfMember2.Dto.Card.Votes.Last().Key, "Member's name does not match");
            Assert.AreEqual(VoteCount, responseOfMember2.Dto.Card.Votes.Last().Value, "Vote count is not valid.");

            //When
            // get cards 
            var getCards = await client.GetAsync<IdeaBoardResponse>(RequestUris.IdeaboardGetCardsByAssessmentUid(AssessmentUid).AddQueryParameter("companyId", Company.Id));

            //Then
            Assert.AreEqual(HttpStatusCode.OK, responseOfMember2.StatusCode, "The status codes does not match");
            Assert.AreEqual(getCards.Dto.AssessmentUid, NotesCardResponse.First().AssessmentUid, "The assessment Uid does not match");
            Assert.That.ListContains(getCards.Dto.Cards.Select(d => d.ItemText.ToString()).ToList(), responseOfMember2.Dto.Card.ItemText, "The Item Text does not match");
            Assert.That.ListContains(getCards.Dto.Cards.Select(d => d.DimensionId.ToString()).ToList(), NotesCardResponse.First().Card.DimensionId.ToString(), "The dimension ids does not match");
            Assert.That.ListContains(getCards.Dto.Cards.Select(d => d.ItemId.ToString()).ToList(), NotesCardResponse.First().Card.ItemId.ToString(), "The item ids does not match");
            Assert.That.ListContains(getCards.Dto.Cards.Select(d => d.ColumnName.ToString()).ToList(), NotesCardResponse.First().Card.ColumnName, "The column names does not match");
            Assert.That.ListContains(getCards.Dto.Cards.Where(d => d.ColumnName == NotesColumn).Select(d => d.Votes.First().Key).ToList(), Member1.Username, "Member does not match");
            Assert.That.ListContains(getCards.Dto.Cards.Where(d => d.ColumnName == NotesColumn).Select(d => d.Votes.First().Value.ToString()).ToList(), VoteCount.ToString(), "Vote count is not valid.");
            Assert.That.ListContains(getCards.Dto.Cards.Where(d => d.ColumnName == NotesColumn).Select(d => d.Votes.Last().Key).ToList(), Member2.Username, "Member does not match");
            Assert.That.ListContains(getCards.Dto.Cards.Where(d => d.ColumnName == NotesColumn).Select(d => d.Votes.Last().Value.ToString()).ToList(), VoteCount.ToString(), "Vote count is not valid.");
        }

        [TestMethod]
        [TestCategory("SiteAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public async Task Ideaboard_UpdateCards_Put_BadRequest()
        {
            //Given
            var client = await GetAuthenticatedClient();

            //act: update card
            UpdateCard.AssessmentUid = new Guid();
            UpdateCard.Card.DimensionId = -1;
            UpdateCard.SignalRGroupName = "";
            UpdateCard.SignalRUserId = null;
            UpdateCard.Card.SortOrder = -1;
            UpdateCard.Card.ItemId = -1;
            UpdateCard.Card.ItemText = "";
            UpdateCard.Card.VoteCount = -1;

            //When
            var response = await client.PutAsync<IList<string>>(RequestUris.IdeaboardCard().AddQueryParameter("companyId", Company.Id), UpdateCard);

            var errorResponseList = new List<string>
            {
                "'Assessment Uid' is not valid",
                "'DimensionId' must be greater than 0",
                "'SignalRGroupName' should not be empty",
                "'SignalRUserId' should not be empty",
                "'SortOrder' must be greater than 0",
                "'ItemId' must be greater than 0",
                "'VoteCount' must be equal or greater than 0"
            };

            //Then
            //assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "The status codes does not match");
            Assert.That.ListsAreEqual(errorResponseList, response.Dto.ToList(), "Error list does not match");
        }

        [TestMethod]
        [TestCategory("SiteAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public async Task Ideaboard_UpdateCards_Put_Unauthorized()
        {
            //Given
            var client = GetUnauthenticatedClient();

            //When
            var response = await client.PutAsync(RequestUris.IdeaboardCard().AddQueryParameter("companyId", Company.Id), UpdateCard.ToStringContent());

            //Then
            //assert
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "The status codes does not match");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public async Task Ideaboard_UpdateCards_Put_Forbidden()
        {
            //Given
            var client = await GetAuthenticatedClient();
            const int companyId = SharedConstants.FakeCompanyId;

            //When
            var response = await client.PutAsync(RequestUris.IdeaboardCard().AddQueryParameter("companyId", companyId), UpdateCard.ToStringContent());

            //Then
            //assert
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "The status codes does not match");
        }

        [TestMethod]
        [TestCategory("SiteAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public async Task Ideaboard_UpdateCards_Put_NotFound()
        {
            //Given
            var client = await GetAuthenticatedClient();

            //When
            var response = await client.PutAsync(RequestUris.IdeaboardCard().AddQueryParameter("companyId", Company.Id), UpdateCard.ToStringContent());

            //Then
            //assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode, "The status codes does not match");
        }
    }
}