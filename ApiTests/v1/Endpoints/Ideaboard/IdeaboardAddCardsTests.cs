using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AtCommon.Api;
using AtCommon.Dtos.Ideaboard;
using AtCommon.Dtos.IndividualAssessments;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiTests.v1.Endpoints.Ideaboard
{
    [TestClass]
    [TestCategory("Ideaboard")]
    public class IdeaboardAddCardsTests : BaseV1Test
    {
        public static int? DimensionId;
        public static string ColumnName;
        public static string RadarName;
        public static readonly int? NotesId = null;
        public static string NotesColumn = "Notes";
        public static Guid AssessmentUid;
        private static AddTeamWithMemberRequest _team;
        private static TeamResponse _teamResponse;
        private static readonly UserConfig CompanyAdminUserConfig = new UserConfig("CA");

        [ClassInitialize]
        public static void ClassSetup(TestContext testContext)
        {
            var user = User;
            if (user.IsMember())
            {
                user = CompanyAdminUserConfig.GetUserByDescription("user 1");
            }

            if (!user.IsBusinessLineAdmin() && !user.IsTeamAdmin() && !user.IsCompanyAdmin()) return;
            var setup = new SetupTeardownApi(TestEnvironment);
            _team = TeamFactory.GetValidPostTeamWithMember("IdeaboardCardForAPI_");
            _teamResponse = setup.CreateTeam(_team, user).GetAwaiter().GetResult();

            //get radar details
            var radarDetails = setup.GetRadarDetailsBySurveyId(Company.Id, SharedConstants.IndividualAssessmentSurveyId);

            //get up individual assessment members
            var individualAssessment = IndividualAssessmentFactory.GetValidIndividualAssessment(Company.Id, user.CompanyName, radarDetails.SurveyId);
            individualAssessment.Members = new List<IndividualAssessmentMemberRequest>
            {
                _teamResponse.Members.FirstOrDefault().CheckForNull("No team members found in the response").ToAddIndividualMemberRequest()
            };
            individualAssessment.TeamUid = _teamResponse.Uid;
            individualAssessment.Published = true;

            //create individual assessment and get RadarDetails
            var assessment = setup.CreateIndividualAssessment(individualAssessment, SharedConstants.IndividualAssessmentType,user).GetAwaiter().GetResult();

            //arrange information for card
            AssessmentUid = assessment.AssessmentList[0].AssessmentUid;
            DimensionId = radarDetails.Dimensions.FirstOrDefault(d => d.Name != "Finish").CheckForNull().DimensionId;
            ColumnName = radarDetails.Dimensions.FirstOrDefault(d => d.DimensionId == DimensionId).CheckForNull().Name;
            RadarName = radarDetails.Name;
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("Member")]
        public async Task Ideaboard_AddCards_Dimension_Post_Created()
        {
            var client = await GetAuthenticatedClient();

            //create card
            var cardRequestList = IdeaboardFactory.GetCard(AssessmentUid, DimensionId, ColumnName, RadarName, 3);
            var cardResponseList = new List<CreateCardResponse>();
            
            //act
            foreach (var card in cardRequestList)
            {
                var response = await client.PostAsync<CreateCardResponse>(RequestUris.IdeaboardCard().AddQueryParameter("companyId", Company.Id), card);
                response.EnsureSuccess();
                cardResponseList.Add(response.Dto);
                Assert.AreEqual(HttpStatusCode.Created, response.StatusCode, "Status codes does not match");
            }
            
            //assert
            foreach (var response in cardResponseList)
            {
                Assert.IsTrue(cardRequestList.All(c => c.Name == response.Name), "The names does not match");
                Assert.IsTrue(cardRequestList.All(c => c.AssessmentUid == response.AssessmentUid), "The assessment UIds does not match");
                Assert.IsTrue(response.IdeaBoardId > 0, "An id number has not been assigned to the card");
                Assert.IsTrue(cardRequestList.All(c => c.Card.DimensionId == response.Card.DimensionId), "The dimension ids does not match");
                Assert.IsTrue(cardRequestList.All(c => c.Card.ColumnName == response.Card.ColumnName), "The column names does not match");
                Assert.IsTrue(cardRequestList.All(c => c.Card.SortOrder == response.Card.SortOrder), "The columns are not in the same order");
                Assert.IsTrue(cardRequestList.Exists(c => c.Card.ItemText == response.Card.ItemText), "The text in the cards does not match");
                Assert.IsTrue(response.Card.VoteCount >= 0, "Vote count is not valid");
            }
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("Member")]
        public async Task Ideaboard_AddCards_Notes_Post_Created()
        {
            var client = await GetAuthenticatedClient();

            //create card
            var cardRequestList = IdeaboardFactory.GetCard(AssessmentUid, NotesId, NotesColumn, RadarName, 3);
            var cardResponseList = new List<CreateCardResponse>();

            //act
            foreach (var card in cardRequestList)
            {
                var response = await client.PostAsync<CreateCardResponse>(RequestUris.IdeaboardCard().AddQueryParameter("companyId", Company.Id), card);
                response.EnsureSuccess();
                cardResponseList.Add(response.Dto);
                Assert.AreEqual(HttpStatusCode.Created, response.StatusCode, "Status codes does not match");
            }

            //assert
            foreach (var response in cardResponseList)
            {
                Assert.IsTrue(cardRequestList.All(c => c.Name == response.Name), "The names does not match");
                Assert.IsTrue(cardRequestList.All(c => c.AssessmentUid == response.AssessmentUid), "The assessment UId does not match");
                Assert.IsTrue(response.IdeaBoardId > 0, "An id number has not been assigned to the card");
                Assert.IsTrue(cardRequestList.All(c => c.Card.DimensionId == response.Card.DimensionId), "The dimension ids does not match");
                Assert.IsTrue(cardRequestList.All(c => c.Card.ColumnName == response.Card.ColumnName), "The column names does not match");
                Assert.IsTrue(cardRequestList.All(c => c.Card.SortOrder == response.Card.SortOrder), "The columns are not in the same order");
                Assert.IsTrue(cardRequestList.Exists(c => c.Card.ItemText == response.Card.ItemText), "The text in the cards does not match");
                Assert.IsTrue(response.Card.VoteCount >= 0, "Vote count is not valid");
            }
        }

        [TestMethod]
        [TestCategory("SiteAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("Member")]
        public async Task Ideaboard_AddCards_Post_BadRequest()
        {
            var client = await GetAuthenticatedClient();

           //create card
            var cardRequest = new CreateCardRequest
            {
                AssessmentUid = new Guid(),
                SignalRGroupName = "",
                SignalRUserId = null,
                Name = "",
                Card = new CardRequest
                {
                    DimensionId = 0,
                    ColumnName = "",
                    SortOrder = 0,
                    VoteCount = -1
                }
            };
            
            var errorResponseList = new List<string>
            {
                "'Assessment Uid' is not valid",
                "'DimensionId' must be greater than 0",
                "'SignalRGroupName' should not be empty",
                "'SignalRUserId' should not be empty",
                "'Name' should not be empty",
                "'SortOrder' must be greater than 0",
                "'VoteCount' must be equal or greater than 0"
            };

            //act
            var response = await client.PostAsync<IList<string>>(RequestUris.IdeaboardCard().AddQueryParameter("companyId", Company.Id), cardRequest);
            
            //assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "The status codes does not match");
            Assert.That.ListsAreEqual(errorResponseList, response.Dto.ToList(), "Error list does not match");
        }
        
        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("Member")]
        public async Task Ideaboard_AddCards_VoteCount_Post_BadRequest()
        {
            var client = await GetAuthenticatedClient();

            //create card
            var cardRequest = IdeaboardFactory.GetCard(AssessmentUid, DimensionId, ColumnName, RadarName);
            cardRequest.FirstOrDefault().CheckForNull().Card.VoteCount = -1;
            //act
            var response = await client.PostAsync<IList<string>>(RequestUris.IdeaboardCard().AddQueryParameter("companyId", Company.Id), cardRequest.FirstOrDefault());

            //assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status codes does not match");
            Assert.AreEqual("'VoteCount' must be equal or greater than 0", response.Dto[0], "Response messages are not the same");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("Member")]
        public async Task Ideaboard_AddCards_Dimension_EmptyItemText_Post_Created()
        {
            var client = await GetAuthenticatedClient();

            //create card
            var cardRequest = IdeaboardFactory.GetCard(AssessmentUid, DimensionId, ColumnName, RadarName);
            cardRequest.FirstOrDefault().CheckForNull().Card.ItemText = "";

            //act
            var response = await client.PostAsync<CreateCardResponse>(RequestUris.IdeaboardCard().AddQueryParameter("companyId", Company.Id), cardRequest.FirstOrDefault());
            
            //assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode, "Status codes does not match"); 
            Assert.AreEqual("", response.Dto.Card.ItemText, "Card text is not empty");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("Member")]
        public async Task Ideaboard_AddCards_Notes_EmptyItemText_Post_Created()
        {
            var client = await GetAuthenticatedClient();

            //create card
            var cardRequest = IdeaboardFactory.GetCard(AssessmentUid, NotesId, NotesColumn, RadarName);
            cardRequest.FirstOrDefault().CheckForNull().Card.ItemText = "";

            //act
            var response = await client.PostAsync<CreateCardResponse>(RequestUris.IdeaboardCard().AddQueryParameter("companyId", Company.Id), cardRequest.FirstOrDefault());

            //assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode, "Status codes does not match");
            Assert.AreEqual("", response.Dto.Card.ItemText, "Card text is not empty");
        }

        [TestMethod]
        [TestCategory("SiteAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("Member")]
        public async Task Ideaboard_AddCards_Post_Unauthorized()
        {
            var client = GetUnauthenticatedClient();
            
            //create card
            var cardRequest = IdeaboardFactory.GetCard(AssessmentUid, NotesId, NotesColumn, RadarName);

            //act
            var response = await client.PostAsync<CreateCardResponse>(RequestUris.IdeaboardCard().AddQueryParameter("companyId", Company.Id), cardRequest);
            
            //assert
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status codes does not match");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("Member")]
        public async Task Ideaboard_AddCards_Post_Forbidden()
        {
             var client = await GetAuthenticatedClient();

           //create card
            var cardRequest = IdeaboardFactory.GetCard(AssessmentUid, DimensionId, ColumnName, RadarName);

            //act
            const int companyId = SharedConstants.FakeCompanyId;
            var response = await client.PostAsync<CreateCardResponse>(RequestUris.IdeaboardCard().AddQueryParameter("companyId", companyId), cardRequest.FirstOrDefault());
            
            //assert
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status codes does not match");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("Member")]
        public async Task Ideaboard_AddCards_Post_NotFound()
        {
            var client = await GetAuthenticatedClient();

            //create card
            var assessmentUid = Guid.NewGuid();
            var cardRequest = IdeaboardFactory.GetCard(assessmentUid, DimensionId, ColumnName, RadarName);

            //act
            var response = await client.PostAsync<CreateCardResponse>(RequestUris.IdeaboardCard().AddQueryParameter("companyId", Company.Id), cardRequest.FirstOrDefault());
            
            //assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode, "Status codes does not match");
        }
    }
}