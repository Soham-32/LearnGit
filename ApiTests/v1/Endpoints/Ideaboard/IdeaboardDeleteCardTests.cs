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
    public class IdeaboardDeleteCardTests : BaseV1Test
    {
        public static string ColumnName;
        public static Guid AssessmentUid;
        private static List<CreateCardResponse> _notesCardResponse;
        private static List<CreateCardResponse> _dimensionCardResponse;
        private static readonly UserConfig CompanyAdminUserConfig = new UserConfig("CA");
        private readonly DeleteCardRequest DeleteCardInfo = new DeleteCardRequest
        {
            AssessmentUid = Guid.NewGuid(),
            SignalRGroupName = "Ideaboard",
            ItemId = 1
        };

        [ClassInitialize]
        public static void ClassSetup(TestContext testContext)
        {
            var user = User;
            if (user.IsMember())
            {
                user = CompanyAdminUserConfig.GetUserByDescription("user 1");
            }

            if (!user.IsTeamAdmin() && !user.IsCompanyAdmin() && !user.IsBusinessLineAdmin()) return;
            var setup = new SetupTeardownApi(TestEnvironment);
            var team = TeamFactory.GetValidPostTeamWithMember("IdeaboardCardForAPI_");
            var teamResponse = setup.CreateTeam(team, user).GetAwaiter().GetResult();

            //get radar details
            var radarDetails = setup.GetRadarDetailsBySurveyId(Company.Id, SharedConstants.IndividualAssessmentSurveyId);

            //get up individual assessment members
            var individualAssessment = IndividualAssessmentFactory.GetValidIndividualAssessment(Company.Id, user.CompanyName, radarDetails.SurveyId);
            individualAssessment.Members = new List<IndividualAssessmentMemberRequest>
            {
                teamResponse.Members.FirstOrDefault().CheckForNull("No team members found in the response")
                    .ToAddIndividualMemberRequest()
            };
            individualAssessment.TeamUid = teamResponse.Uid;
            individualAssessment.Published = true;

            //create individual assessment and get radar details
            var assessment = setup.CreateIndividualAssessment(individualAssessment, SharedConstants.IndividualAssessmentType, user).GetAwaiter().GetResult();
            
            //arrange information for card
            AssessmentUid = assessment.AssessmentList[0].AssessmentUid;
           
            //create card
            _notesCardResponse =
                setup.CreateIdeaboardCardForIndividualAssessment(individualAssessment, Company.Id, AssessmentUid, user,
                    true, 3);
            _dimensionCardResponse =
                setup.CreateIdeaboardCardForIndividualAssessment(individualAssessment, Company.Id, AssessmentUid, user,
                    false, 3);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public async Task Ideaboard_DeleteCards_Dimension_Delete_Success()
        {
            var client = await GetAuthenticatedClient();

            //act
            //delete card
            var deleteCardRequest = IdeaboardFactory.DeleteCard(AssessmentUid, _dimensionCardResponse[0].Card.ItemId);
            
            var response = await client.DeleteAsync(RequestUris.IdeaboardCard().AddQueryParameter("companyId", Company.Id), deleteCardRequest);
            
            //assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status codes does not match");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public async Task Ideaboard_DeleteCards_Notes_Delete_Success()
        {
            var client = await GetAuthenticatedClient();

            //act
            //delete card
            var deleteCardRequest = IdeaboardFactory.DeleteCard(AssessmentUid, _notesCardResponse[0].Card.ItemId);

            var response = await client.DeleteAsync(RequestUris.IdeaboardCard().AddQueryParameter("companyId", Company.Id), deleteCardRequest);

            //assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status codes does not match");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public async Task Ideaboard_DeleteCards_Dimension_Delete_WithoutCompanyId_Success()
        {
            var client = await GetAuthenticatedClient();

            //act
            //delete card
            var deleteCardRequest = IdeaboardFactory.DeleteCard(AssessmentUid, _dimensionCardResponse[1].Card.ItemId);
            
            var response = await client.DeleteAsync(RequestUris.IdeaboardCard(), deleteCardRequest);
            
            //assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status codes does not match");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public async Task Ideaboard_DeleteCards_Notes_Delete_WithoutCompanyId_Success()
        {
            var client = await GetAuthenticatedClient();

            //act
            //delete card
            var deleteCardRequest = IdeaboardFactory.DeleteCard(AssessmentUid, _notesCardResponse[1].Card.ItemId);

            var response = await client.DeleteAsync(RequestUris.IdeaboardCard(), deleteCardRequest);

            //assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status codes does not match");
        }

        [TestMethod]
        [TestCategory("SiteAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public async Task Ideaboard_DeleteCards_Delete_BadRequest()
        {
            var client = await GetAuthenticatedClient();

            //act
            //delete card
            var deleteCardRequest = new DeleteCardRequest
            {
                AssessmentUid = new Guid(),
                SignalRGroupName = "",
                SignalRUserId = "",
                ItemId = 0
            };
            
            var response = await client.DeleteAsync<IList<string>>(RequestUris.IdeaboardCard().AddQueryParameter("companyId", Company.Id), deleteCardRequest);

            var errorResponseList = new List<string>
            {
                "'SignalRGroupName' should not be empty",
                "'SignalRUserId' should not be empty",
                "'ItemId' must be greater than 0",
                "'Assessment Uid' is not valid"
            };
            
            //assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status codes does not match");
            Assert.That.ListsAreEqual(errorResponseList, response.Dto.ToList(), "Error lists does not match");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public async Task Ideaboard_DeleteCards_Dimension_Delete_ReDelete_NotFound()
        {
            var client = await GetAuthenticatedClient();

            //act
            //delete card
            var deleteCardRequest = IdeaboardFactory.DeleteCard(AssessmentUid, _dimensionCardResponse[2].Card.ItemId);

            var firstDeleteResponse = await client.DeleteAsync(RequestUris.IdeaboardCard().AddQueryParameter("companyId", Company.Id), deleteCardRequest);
            firstDeleteResponse.EnsureSuccessStatusCode();

            var secondDeleteResponse = await client.DeleteAsync(RequestUris.IdeaboardCard().AddQueryParameter("companyId", Company.Id), deleteCardRequest);

            //assert
            Assert.AreEqual(HttpStatusCode.OK, firstDeleteResponse.StatusCode, "Status codes does not match");
            Assert.AreEqual(HttpStatusCode.NotFound, secondDeleteResponse.StatusCode, "Status codes does not match");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public async Task Ideaboard_DeleteCards_Notes_Delete_ReDelete_NotFound()
        {
            var client = await GetAuthenticatedClient();

            //act
            //delete card
            var deleteCardRequest = IdeaboardFactory.DeleteCard(AssessmentUid, _notesCardResponse[2].Card.ItemId);
            
            var firstDeleteResponse = await client.DeleteAsync(RequestUris.IdeaboardCard().AddQueryParameter("companyId", Company.Id), deleteCardRequest);

            var secondDeleteResponse = await client.DeleteAsync(RequestUris.IdeaboardCard().AddQueryParameter("companyId", Company.Id), deleteCardRequest);

            //assert
            Assert.AreEqual(HttpStatusCode.OK,firstDeleteResponse.StatusCode,"Status codes does not match");
            Assert.AreEqual(HttpStatusCode.NotFound, secondDeleteResponse.StatusCode, "Status codes does not match");
        }

        [TestMethod]
        [TestCategory("SiteAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public async Task Ideaboard_DeleteCards_Delete_Unauthorized()
        {
            var client = GetUnauthenticatedClient();

            //act
            //delete card
            var response = await client.DeleteAsync<IList<string>>(RequestUris.IdeaboardCard().AddQueryParameter("companyId", Company.Id), DeleteCardInfo);

            //assert
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status codes does not match");
        }
        
        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public async Task Ideaboard_DeleteCards_Delete_Forbidden()
        {
            var client = await GetAuthenticatedClient();

            //act
            //delete card
            const int companyId = SharedConstants.FakeCompanyId;
            var response = await client.DeleteAsync<IList<string>>(RequestUris.IdeaboardCard().AddQueryParameter("companyId", companyId), DeleteCardInfo);

            //assert
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status codes does not match");
        }
        
        [TestMethod]
        [TestCategory("SiteAdmin")]
        public async Task Ideaboard_DeleteCards_Delete_NonExistentCompany_BadRequest()
        {
            var client = await GetAuthenticatedClient();

            //act
            //delete card
            const int companyId = SharedConstants.FakeCompanyId;
            var response = await client.DeleteAsync<IList<string>>(RequestUris.IdeaboardCard().AddQueryParameter("companyId", companyId), DeleteCardInfo);

            //assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status codes does not match");
        }
    }
}