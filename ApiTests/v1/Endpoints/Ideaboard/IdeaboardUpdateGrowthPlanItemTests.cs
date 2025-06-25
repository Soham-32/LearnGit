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

namespace ApiTests.v1.Endpoints.Ideaboard
{
    [TestClass]
    [TestCategory("Ideaboard")]
    public class IdeaboardUpdateGrowthPlanItemTests : BaseV1Test
    {
        public static int? DimensionId;
        public static Guid AssessmentUid;
        private static AddTeamWithMemberRequest _team;
        private static TeamResponse _teamResponse;
        private static Guid _unique = Guid.NewGuid();
        public static List<CreateCardResponse> DimensionCardResponse;
        public static List<CreateCardResponse> NotesCardResponse;
        private static readonly UserConfig CompanyAdminUserConfig = new UserConfig("CA");
        private readonly UpdateGrowthPlanItemRequest GiRequest = new UpdateGrowthPlanItemRequest
        {
            AssessmentUid = _unique,
            SignalRGroupName = "CreateGrowthPlanItem",
            Card = new UpdateGrowthPlanItemCardRequest
            {
                ItemId = 1,
                DimensionId = 1,
                GrowthPlanItemId = 1,
                GrowthPlanItemCategory = SharedConstants.IdeaboardIndividualGpi,
                ItemText = _unique.ToString()
            }
        };

        [ClassInitialize]
        public static void ClassSetup(TestContext testContext)
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
            var radarDetails = setup.GetRadarDetailsBySurveyId(Company.Id, SharedConstants.IndividualAssessmentSurveyId);

            //get up individual assessment members
            var individualAssessment = IndividualAssessmentFactory.GetValidIndividualAssessment(Company.Id, user.CompanyName, radarDetails.SurveyId);
            individualAssessment.Members = new List<IndividualAssessmentMemberRequest>
            {
                _teamResponse.Members.FirstOrDefault().CheckForNull("No team members found in the response").ToAddIndividualMemberRequest()
            };
            individualAssessment.TeamUid = _teamResponse.Uid;
            individualAssessment.Published = true;

            //create individual assessment and get radar details
            var assessment = setup.CreateIndividualAssessment(individualAssessment, SharedConstants.IndividualAssessmentType, user).GetAwaiter().GetResult();

            //arrange information for card
            AssessmentUid = assessment.AssessmentList[0].AssessmentUid;
            DimensionId = radarDetails.Dimensions.FirstOrDefault(d => d.Name != "Finish").CheckForNull().DimensionId;

            NotesCardResponse = setup.CreateIdeaboardCardForIndividualAssessment(individualAssessment, Company.Id, assessment.AssessmentList.First().AssessmentUid, user, true,2);
            DimensionCardResponse = setup.CreateIdeaboardCardForIndividualAssessment(individualAssessment, Company.Id, assessment.AssessmentList.First().AssessmentUid, user, false, 2);

        }

        [TestMethod]
        [TestCategory("KnownDefect")]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("Member")]
        public async Task Ideaboard_UpdateGrowthItem_Dimension_Put_Success()
        {
            var client = await GetAuthenticatedClient();

            //create growth plan item
            var gIRequest = IdeaboardFactory.CreateGrowthPlanItem(AssessmentUid, DimensionId, SharedConstants.IdeaboardManagementGpi, DimensionCardResponse.First().Card.ItemId);
            var gIResponse = await client.PostAsync<CreateGrowthPlanItemResponse>(RequestUris.IdeaboardGrowthPlanItem(), gIRequest);
            gIResponse.EnsureSuccess();

            //update growth plan item
            var updateGiRequest = IdeaboardFactory.UpdateGrowthPlanItem(AssessmentUid, DimensionId, SharedConstants.IdeaboardIndividualGpi, DimensionCardResponse.First().Card.ItemId);

            //act
            var response = await client.PutAsync<UpdateGrowthPlanItemRequest>(RequestUris.IdeaboardGrowthPlanItem().AddQueryParameter("companyId", Company.Id), updateGiRequest);

            //assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status codes does not match");
            Assert.AreEqual(gIResponse.Dto.AssessmentUid, response.Dto.AssessmentUid, "Assessment Ids does not match");
            Assert.AreEqual(SharedConstants.IdeaboardIndividualGpi, response.Dto.Card.GrowthPlanItemCategory, "The growth plan item category did not change");
            Assert.AreNotEqual(gIResponse.Dto.Card.GrowthPlanItemCategory, response.Dto.Card.GrowthPlanItemCategory, "The growth plan item category does change");
        }

        [TestMethod]
        [TestCategory("KnownDefect")]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("Member")]
        public async Task Ideaboard_UpdateGrowthItem_Notes_Put_Success()
        {
            var client = await GetAuthenticatedClient();

            //create growth plan item
            var gIRequest = IdeaboardFactory.CreateGrowthPlanItem(AssessmentUid, DimensionId, SharedConstants.IdeaboardIndividualGpi, NotesCardResponse.First().Card.ItemId);
            var gIResponse = await client.PostAsync<CreateGrowthPlanItemResponse>(RequestUris.IdeaboardGrowthPlanItem(), gIRequest);
            gIResponse.EnsureSuccess();

            //update growth plan item
            var updateGiRequest = IdeaboardFactory.UpdateGrowthPlanItem(AssessmentUid, DimensionId, SharedConstants.IdeaboardManagementGpi, NotesCardResponse.First().Card.ItemId);

            //act
            var response = await client.PutAsync<UpdateGrowthPlanItemRequest>(RequestUris.IdeaboardGrowthPlanItem().AddQueryParameter("companyId", Company.Id), updateGiRequest);

            //assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status codes does not match");
            Assert.AreEqual(gIResponse.Dto.AssessmentUid, response.Dto.AssessmentUid, "Assessment Ids does not match");
            Assert.AreEqual(SharedConstants.IdeaboardManagementGpi, response.Dto.Card.GrowthPlanItemCategory, "The growth plan item category did not change");
            Assert.AreNotEqual(gIResponse.Dto.Card.GrowthPlanItemCategory, response.Dto.Card.GrowthPlanItemCategory, "The growth plan item category does change");
        }

        [TestMethod]
        [TestCategory("KnownDefect")]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("Member")]
        public async Task Ideaboard_UpdateGrowthItem_Dimension_Put_WithOutCompanyId_Success()
        {
            var client = await GetAuthenticatedClient();

            //create growth plan item
            var gIRequest = IdeaboardFactory.CreateGrowthPlanItem(AssessmentUid, DimensionId, SharedConstants.IdeaboardIndividualGpi, DimensionCardResponse.Last().Card.ItemId);
            var gIResponse = await client.PostAsync<CreateGrowthPlanItemResponse>(RequestUris.IdeaboardGrowthPlanItem(), gIRequest);
            gIResponse.EnsureSuccess();

            //update growth plan item
            var updateGiRequest = IdeaboardFactory.UpdateGrowthPlanItem(AssessmentUid, DimensionId, SharedConstants.IdeaboardManagementGpi, DimensionCardResponse.Last().Card.ItemId);

            //act
            var response =
                await client.PutAsync<UpdateGrowthPlanItemRequest>(RequestUris.IdeaboardGrowthPlanItem(), updateGiRequest);

            //assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status codes does not match");
            Assert.AreEqual(gIResponse.Dto.AssessmentUid, response.Dto.AssessmentUid, "Assessment Ids does not match");
            Assert.AreEqual(SharedConstants.IdeaboardManagementGpi, response.Dto.Card.GrowthPlanItemCategory, "The growth plan item category did not change");
            Assert.AreNotEqual(gIResponse.Dto.Card.GrowthPlanItemCategory, response.Dto.Card.GrowthPlanItemCategory, "The growth plan item category does change");
        }

        [TestMethod]
        [TestCategory("KnownDefect")]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("Member")]
        public async Task Ideaboard_UpdateGrowthItem_Notes_Put_WithOutCompanyId_Success()
        {
            var client = await GetAuthenticatedClient();

            //create growth plan item
            var gIRequest = IdeaboardFactory.CreateGrowthPlanItem(AssessmentUid, DimensionId, SharedConstants.IdeaboardManagementGpi, NotesCardResponse.Last().Card.ItemId);
            var gIResponse = await client.PostAsync<CreateGrowthPlanItemResponse>(RequestUris.IdeaboardGrowthPlanItem(), gIRequest);
            gIResponse.EnsureSuccess();

            //update growth plan item
            var updateGiRequest = IdeaboardFactory.UpdateGrowthPlanItem(AssessmentUid, DimensionId, SharedConstants.IdeaboardIndividualGpi, NotesCardResponse.Last().Card.ItemId);

            //act
            var response =
                await client.PutAsync<UpdateGrowthPlanItemRequest>(RequestUris.IdeaboardGrowthPlanItem(), updateGiRequest);

            //assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status codes does not match");
            Assert.AreEqual(gIResponse.Dto.AssessmentUid, response.Dto.AssessmentUid, "Assessment Ids does not match");
            Assert.AreEqual(SharedConstants.IdeaboardIndividualGpi, response.Dto.Card.GrowthPlanItemCategory, "The growth plan item category did not change");
            Assert.AreNotEqual(gIResponse.Dto.Card.GrowthPlanItemCategory, response.Dto.Card.GrowthPlanItemCategory, "The growth plan item category does change");
        }

        [TestMethod]
        [TestCategory("SiteAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("Member")]
        public async Task Ideaboard_UpdateGrowthItem_Put_Unauthorized()
        {
            //arrange
            var client = GetUnauthenticatedClient();
            var growthPlanItem = IdeaboardFactory.UpdateGrowthPlanItem(GiRequest.AssessmentUid, GiRequest.Card.DimensionId, GiRequest.Card.GrowthPlanItemCategory, GiRequest.Card.ItemId);

            //act
            var response = await client.PutAsync<UpdateGrowthPlanItemRequest>(RequestUris.IdeaboardGrowthPlanItem(), growthPlanItem);

            //assert
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status codes does not match");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("Member")]
        public async Task Ideaboard_UpdateGrowthItem_Put_Forbidden()
        {
            //arrange
            var client = await GetAuthenticatedClient();
            var growthPlanItem = IdeaboardFactory.UpdateGrowthPlanItem(GiRequest.AssessmentUid, GiRequest.Card.DimensionId, GiRequest.Card.GrowthPlanItemCategory, GiRequest.Card.ItemId);
            const int companyId = SharedConstants.FakeCompanyId;

            //act
            var response = await client.PutAsync<UpdateGrowthPlanItemRequest>(RequestUris.IdeaboardGrowthPlanItem().AddQueryParameter("companyId", companyId), growthPlanItem);

            //assert
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status codes does not match");
        }

        [TestMethod]
        [TestCategory("SiteAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("Member")]
        public async Task Ideaboard_UpdateGrowthItem_Put_NotFound()
        {
            //arrange
            var client = await GetAuthenticatedClient();
            var growthPlanItem = IdeaboardFactory.UpdateGrowthPlanItem(GiRequest.AssessmentUid, GiRequest.Card.DimensionId, GiRequest.Card.GrowthPlanItemCategory, GiRequest.Card.ItemId);

            //act
            var response = await client.PutAsync<UpdateGrowthPlanItemRequest>(RequestUris.IdeaboardGrowthPlanItem(), growthPlanItem);

            //assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode, "Status codes does not match");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("Member")]
        public async Task Ideaboard_UpdateGrowthItem_Put_BadRequest()
        {
            var client = await GetAuthenticatedClient();

            //create update growth plan item
            var gIUpdateBadRequest = new UpdateGrowthPlanItemRequest()
            {
                AssessmentUid = new Guid(),
                SignalRGroupName = "",
                SignalRUserId = "",
                Card = new UpdateGrowthPlanItemCardRequest()
                {
                    ItemId = -1,
                    DimensionId = -1,
                    GrowthPlanItemId = 0,
                    GrowthPlanItemCategory = "",
                    ItemText = ""
                }
            };

            var gIErrorList = new List<string>
            {
                "'Assessment Uid' is not valid",
                "'SignalRGroupName' should not be empty",
                "'SignalRUserId' should not be empty",
                "'ItemId' must be greater than 0",
                "'DimensionId' must be greater than 0",
                "'GrowthPlanItemId' must be greater than 0",
                "'GrowthPlanItemCategory' options are Individual, Team, Organizational, or Management",
                "'ItemText' should not be empty"
            };

            //act
            var response = await client.PutAsync<IList<string>>(RequestUris.IdeaboardGrowthPlanItem().AddQueryParameter("companyId", Company.Id), gIUpdateBadRequest);

            //assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status codes does not match");
            Assert.That.ListsAreEqual(gIErrorList, response.Dto.ToList(), "Error lists does not match");
        }
    }
}