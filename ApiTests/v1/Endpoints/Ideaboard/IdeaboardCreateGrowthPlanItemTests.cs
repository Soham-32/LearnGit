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
    public class IdeaboardCreateGrowthPlanItemTests : BaseV1Test
    {
        public static int? DimensionId;
        public static string ColumnName;
        public static Guid AssessmentUid;
        private static Guid _unique = Guid.NewGuid();
        private static List<CreateCardResponse> _notesCardResponse;
        private static List<CreateCardResponse> _dimensionCardResponse;
        private static readonly UserConfig CompanyAdminUserConfig = new UserConfig("CA");
        private readonly CreateGrowthPlanItemRequest GiRequest = new CreateGrowthPlanItemRequest
        {
            AssessmentUid = _unique,
            SignalRGroupName = "CreateGrowthPlanItem",
            SignalRUserId = "42",
            Card = new GrowthPlanItemCardRequest
            {
                ItemId = 1,
                DimensionId = 1,
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
            var team = TeamFactory.GetValidPostTeamWithMember("IdeaboardCardForAPI_");
            var teamResponse = setup.CreateTeam(team, user).GetAwaiter().GetResult();

            //get radar details
            var radarDetails = setup.GetRadarDetailsBySurveyId(Company.Id, SharedConstants.IndividualAssessmentSurveyId);

            //get up individual assessment members
            var individualAssessment = IndividualAssessmentFactory.GetValidIndividualAssessment(Company.Id, user.CompanyName, radarDetails.SurveyId);
            individualAssessment.Members = new List<IndividualAssessmentMemberRequest>
            {
                teamResponse.Members.FirstOrDefault().CheckForNull("No team members found in the response").ToAddIndividualMemberRequest()
            };
            individualAssessment.TeamUid = teamResponse.Uid;
            individualAssessment.Published = true;

            //create individual assessment and get radar details
            var assessment = setup.CreateIndividualAssessment(individualAssessment, SharedConstants.IndividualAssessmentType,user).GetAwaiter().GetResult();

            //arrange information for card
            AssessmentUid = assessment.AssessmentList[0].AssessmentUid;
            DimensionId = radarDetails.Dimensions.FirstOrDefault(d => d.Name != "Finish").CheckForNull().DimensionId;

            _notesCardResponse = setup.CreateIdeaboardCardForIndividualAssessment(individualAssessment, Company.Id,
                AssessmentUid, user, true, 2);
            _dimensionCardResponse = setup.CreateIdeaboardCardForIndividualAssessment(individualAssessment, Company.Id,
                AssessmentUid, user, false, 2);

        }

        [TestMethod]
        [TestCategory("KnownDefect")]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("Member")]
        public async Task Ideaboard_AddGrowthItem_Dimension_Post_Created()
        {
            var client = await GetAuthenticatedClient();

            //create growth plan item
            var gIRequest = IdeaboardFactory.CreateGrowthPlanItem(AssessmentUid, DimensionId,
                SharedConstants.IdeaboardIndividualGpi, _dimensionCardResponse.First().Card.ItemId);

            //act
            var response = await client.PostAsync<CreateGrowthPlanItemResponse>(RequestUris.IdeaboardGrowthPlanItem(), gIRequest);

            //assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode, "Status codes does not match");
            Assert.IsTrue(response.Dto.Card.GrowthPlanItemId > 0, $"Growth plan item id of {response.Dto.Card.GrowthPlanItemId} is invalid");
            Assert.AreEqual(AssessmentUid, response.Dto.AssessmentUid, "Assessment UID does not match");
            Assert.AreEqual(gIRequest.Card.GrowthPlanItemCategory, response.Dto.Card.GrowthPlanItemCategory, "Categories does not match");
        }

        [TestMethod]
        [TestCategory("KnownDefect")]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("Member")]
        public async Task Ideaboard_AddGrowthItem_Notes_Post_Created()
        {
            var client = await GetAuthenticatedClient();

            //create growth plan item
            var gIRequest = IdeaboardFactory.CreateGrowthPlanItem(AssessmentUid, DimensionId,
                SharedConstants.IdeaboardManagementGpi, _notesCardResponse.First().Card.ItemId);

            //act
            var response = await client.PostAsync<CreateGrowthPlanItemResponse>(RequestUris.IdeaboardGrowthPlanItem(), gIRequest);

            //assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode, "Status codes does not match");
            Assert.IsTrue(response.Dto.Card.GrowthPlanItemId > 0, $"Growth plan item id of {response.Dto.Card.GrowthPlanItemId} is invalid");
            Assert.AreEqual(AssessmentUid, response.Dto.AssessmentUid, "Assessment UID does not match");
            Assert.AreEqual(gIRequest.Card.GrowthPlanItemCategory, response.Dto.Card.GrowthPlanItemCategory, "Categories does not match");
        }

        [TestMethod]
        [TestCategory("KnownDefect")]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("Member")]
        public async Task Ideaboard_AddGrowthItem_Dimension_Post_WithCompanyId_Created()
        {
            var client = await GetAuthenticatedClient();

            //create growth plan item
            var gIRequest = IdeaboardFactory.CreateGrowthPlanItem(AssessmentUid, DimensionId,
                SharedConstants.IdeaboardManagementGpi, _dimensionCardResponse.Last().Card.ItemId);
            //act
            var response = await client.PostAsync<CreateGrowthPlanItemResponse>(RequestUris.IdeaboardGrowthPlanItem().AddQueryParameter("companyId", Company.Id), gIRequest);

            //assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode, "Status codes does not match");
            Assert.IsTrue(response.Dto.Card.GrowthPlanItemId > 0, $"Growth plan item id of {response.Dto.Card.GrowthPlanItemId} is invalid");
            Assert.AreEqual(AssessmentUid, response.Dto.AssessmentUid, "Assessment UID does not match");
            Assert.AreEqual(gIRequest.Card.GrowthPlanItemCategory, response.Dto.Card.GrowthPlanItemCategory, "Categories does not match");
        }

        [TestMethod]
        [TestCategory("KnownDefect")]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("Member")]
        public async Task Ideaboard_AddGrowthItem_Notes_Post_WithCompanyId_Created()
        {
            var client = await GetAuthenticatedClient();

            //create growth plan item
            var gIRequest = IdeaboardFactory.CreateGrowthPlanItem(AssessmentUid, DimensionId,
                SharedConstants.IdeaboardIndividualGpi, _notesCardResponse.Last().Card.ItemId);
            //act
            var response = await client.PostAsync<CreateGrowthPlanItemResponse>(RequestUris.IdeaboardGrowthPlanItem().AddQueryParameter("companyId", Company.Id), gIRequest);

            //assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode, "Status codes does not match");
            Assert.IsTrue(response.Dto.Card.GrowthPlanItemId > 0, $"Growth plan item id of {response.Dto.Card.GrowthPlanItemId} is invalid");
            Assert.AreEqual(AssessmentUid, response.Dto.AssessmentUid, "Assessment UID does not match");
            Assert.AreEqual(gIRequest.Card.GrowthPlanItemCategory, response.Dto.Card.GrowthPlanItemCategory, "Categories does not match");
        }

        [TestMethod]
        [TestCategory("SiteAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("Member")]
        public async Task Ideaboard_AddGrowthItem_Post_BadRequest()
        {
            var client = await GetAuthenticatedClient();

            //create growth plan item
            var gIBadRequest = new CreateGrowthPlanItemRequest
            {
                AssessmentUid = new Guid(),
                SignalRGroupName = "",
                SignalRUserId = "",
                Card = new GrowthPlanItemCardRequest
                {
                    ItemId = -1,
                    DimensionId = -1,
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
                "'GrowthPlanItemCategory' options are Individual, Team, Organizational, or Management",
                "'ItemText' should not be empty"
            };

            //act
            var response = await client.PostAsync<IList<string>>(RequestUris.IdeaboardGrowthPlanItem(), gIBadRequest);

            //assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status codes does not match");
            Assert.That.ListsAreEqual(gIErrorList, response.Dto.ToList(), "Error lists does not match");
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("Member")]
        public async Task Ideaboard_AddGrowthItem_Post_Unauthorized()
        {
            var client = GetUnauthenticatedClient();

            //act
            var response = await client.PostAsync<CreateGrowthPlanItemResponse>(RequestUris.IdeaboardGrowthPlanItem(), GiRequest);

            //assert
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status codes does not match");
        }

        [TestMethod]
        [TestCategory("PartnerAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("Member")]
        public async Task Ideaboard_AddGrowthItem_Post_Forbidden()
        {
            var client = await GetAuthenticatedClient();
            const int companyId = SharedConstants.FakeCompanyId;

            //act
            var response = await client.PostAsync<CreateGrowthPlanItemResponse>(RequestUris.IdeaboardGrowthPlanItem().AddQueryParameter("companyId", companyId), GiRequest);

            //assert
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status codes does not match");
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("Member")]
        public async Task Ideaboard_AddGrowthItem_Post_NotFound()
        {
            var client = await GetAuthenticatedClient();

            //act
            var response = await client.PostAsync<CreateGrowthPlanItemResponse>(RequestUris.IdeaboardGrowthPlanItem(), GiRequest);

            //assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode, "Status codes does not match");
        }
    }
}