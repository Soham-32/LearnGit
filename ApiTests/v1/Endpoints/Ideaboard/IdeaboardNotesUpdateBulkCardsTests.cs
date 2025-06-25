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
    public class IdeaboardNotesUpdateBulkCardsTests : BaseV1Test
    {
        public static Guid AssessmentUid;
        private static AddTeamWithMemberRequest _team;
        private static RadarDetailResponse _radarDetails;
        private static TeamResponse _teamResponse;
        private static ApiResponse<IdeaBoardResponse> _createdCards;
        private static readonly UserConfig CompanyAdminUserConfig = new UserConfig("CA");
        private static List<DimensionDetailResponse> _dimension;

        private readonly UpdateCardsRequest UpdateCardsBadRequest = new UpdateCardsRequest
        {
            AssessmentUid = new Guid(),
            SignalRGroupName = "",
            SignalRUserId = "",
            Cards = new List<CardRequest>
            {
                new CardRequest
                {
                    ItemId = 0,
                    DimensionId = 0,
                    ColumnName = "",
                    SortOrder = 0,
                    ItemText = Guid.NewGuid().ToString(),
                    VoteCount = -1
                }

            }
        };

        [ClassInitialize]
        public static async Task ClassSetupAsync(TestContext testContext)
        {
            var user = User;
            if (user.IsMember())
            {
                user = CompanyAdminUserConfig.GetUserByDescription("user 1");
            }

            if (!user.IsCompanyAdmin() && !user.IsBusinessLineAdmin() && !user.IsTeamAdmin()) return;
            var setup = new SetupTeardownApi(TestEnvironment);
            _team = TeamFactory.GetValidPostTeamWithMember("IdeaboardCardForAPI_");
            _teamResponse = setup.CreateTeam(_team, user).GetAwaiter().GetResult();

            //get radar details
            _radarDetails = setup.GetRadarDetailsBySurveyId(Company.Id, SharedConstants.IndividualAssessmentSurveyId);
            _radarDetails.Dimensions[0].Name = "Notes";

            //get up individual assessment members
            var individualAssessment = IndividualAssessmentFactory.GetValidIndividualAssessment(Company.Id, user.CompanyName, _radarDetails.SurveyId);
            individualAssessment.Members = new List<IndividualAssessmentMemberRequest>
            {
                _teamResponse.Members.FirstOrDefault().CheckForNull("No team members found in the response").ToAddIndividualMemberRequest()
            };
            individualAssessment.TeamUid = _teamResponse.Uid;
            individualAssessment.Published = true;

            //create individual assessment and get radar details
             var assessment = setup.CreateIndividualAssessment(individualAssessment, SharedConstants.IndividualAssessmentType, user).GetAwaiter().GetResult();

            // created card
            setup.CreateIdeaboardCardForIndividualAssessment(individualAssessment, Company.Id, assessment.AssessmentList.First().AssessmentUid, User, true, 4);

            //get cards
            _createdCards = await setup.GetIdeaboardCards(Company.Id, assessment.AssessmentList.First().AssessmentUid, user);

        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("Member")]
        public async Task Ideaboard_BulkUpdate_Notes_Put_Success()
        {
            var client = await GetAuthenticatedClient();

            //act: update card
            _dimension = _radarDetails.Dimensions.Where(d => d.Name != "Finish").ToList();
            _dimension[0].DimensionId = null;

            var updateCardsRequest = IdeaboardFactory.UpdateBulkCards(_createdCards, _dimension);

            var response = await client.PutAsync<UpdateCardsResponse>(RequestUris.IdeaboardCards().AddQueryParameter("companyId", Company.Id), updateCardsRequest);

            //assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "The status codes does not match");
            Assert.AreEqual(_createdCards.Dto.AssessmentUid, response.Dto.AssessmentUid, "The assessment Uid does not match");

            for (var i = 0; i < response.Dto.Cards.Count; i++)
            {
                if (response.Dto.Cards[i].ColumnName == _dimension[i].Name)
                {
                    Assert.AreEqual(_dimension[i].Name, response.Dto.Cards[i].ColumnName, "Column names are not the same. The card was moved.");
                    Assert.AreEqual(_dimension[i].DimensionId, response.Dto.Cards[i].DimensionId, "Dimension ids are same. That card was not moved");
                    Assert.AreNotEqual(_createdCards.Dto.Cards[i].ItemText, response.Dto.Cards[i].ItemText, "Card text did not update");
                }
                else
                {
                    Assert.AreNotEqual(_dimension[i].Name, response.Dto.Cards[i].ColumnName, "Column names are the same. The cards did not move");
                    Assert.AreNotEqual(_dimension[i].DimensionId, response.Dto.Cards[i].DimensionId, "Dimension id are the same. The cards did not move.");
                    Assert.AreEqual(_createdCards.Dto.Cards[i].ItemText, response.Dto.Cards[i].ItemText, "Card text updated");
                }
                Assert.AreEqual(_createdCards.Dto.Cards[i].ItemId, response.Dto.Cards[i].ItemId);
            }
            Assert.AreEqual(_createdCards.Dto.Cards.Count, response.Dto.Cards.Count, $"The amount of cards is not the same. {_createdCards.Dto.Cards.Count} were initially created and {response.Dto.Cards.Count} were moved.");
        }

        [TestMethod]
        [TestCategory("SiteAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("Member")]
        public async Task Ideaboard_BulkUpdate_Notes_Put_BadRequest()
        {
            var client = await GetAuthenticatedClient();

            //act: update card
            var response = await client.PutAsync<IList<string>>(RequestUris.IdeaboardCards().AddQueryParameter("companyId", Company.Id), UpdateCardsBadRequest);

            //assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "The status codes does not match");

            var errorResponseList = new List<string>
            {
                "'Assessment Uid' is not valid",
                "'DimensionId' must be greater than 0",
                "'SignalRGroupName' should not be empty",
                "'SortOrder' must be greater than 0",
                "'SignalRUserId' should not be empty",
                "'ItemId' must be greater than 0",
                "'VoteCount' must be equal or greater than 0"
            };

            //assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "The status codes does not match");
            Assert.That.ListsAreEqual(errorResponseList, response.Dto.ToList(), "Error list does not match");
        }

        [TestMethod]
        [TestCategory("SiteAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("Member")]
        public async Task Ideaboard_BulkUpdate_Notes_Put_Unauthorized()
        {
            var client = GetUnauthenticatedClient();

            var response = await client.PutAsync<UpdateCardsResponse>(RequestUris.IdeaboardCards().AddQueryParameter("companyId", Company.Id), UpdateCardsBadRequest);

            //assert
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "The status codes does not match");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("Member")]
        public async Task Ideaboard_BulkUpdate_Notes_Put_Forbidden()
        {
            var client = await GetAuthenticatedClient();

            const int companyId = SharedConstants.FakeCompanyId;

            var response = await client.PutAsync<UpdateCardsResponse>(RequestUris.IdeaboardCards().AddQueryParameter("companyId", companyId), UpdateCardsBadRequest);

            //assert
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "The status codes does not match");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("Member")]
        public async Task Ideaboard_BulkUpdate_Notes_Put_NotFound()
        {
            var client = await GetAuthenticatedClient();

            //act: update card
            var dimensions = _radarDetails.Dimensions.Where(d => d.Name != "Finish").ToList();
            var updateCardsRequest = IdeaboardFactory.UpdateBulkCards(_createdCards, dimensions);
            updateCardsRequest.AssessmentUid = Guid.NewGuid();

            var response = await client.PutAsync<UpdateCardsResponse>(RequestUris.IdeaboardCards().AddQueryParameter("companyId", Company.Id), updateCardsRequest);

            //assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode, "The status codes does not match");
        }
    }
}