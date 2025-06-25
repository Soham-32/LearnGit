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
    public class IdeaboardGetCardsWithAssessmentUidTests : BaseV1Test
    {
        public static string ColumnName;
        public static Guid AssessmentUid;
        private static List<CreateCardResponse> _notesCardResponse;
        private static List<CreateCardResponse> _dimensionCardResponse;
        private static readonly UserConfig CompanyAdminUserConfig = new UserConfig("CA");

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
             var assessment = setup.CreateIndividualAssessment(individualAssessment, SharedConstants.IndividualAssessmentType, user).GetAwaiter().GetResult();
             AssessmentUid = assessment.AssessmentList[0].AssessmentUid;
            
             //create card
            _dimensionCardResponse = setup.CreateIdeaboardCardForIndividualAssessment( individualAssessment, Company.Id, AssessmentUid,  user, false, 2);
            _notesCardResponse =  setup.CreateIdeaboardCardForIndividualAssessment( individualAssessment, Company.Id, AssessmentUid,  user, true, 2);

        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("Member")]
        public async Task Ideaboard_GetCards_Get_WithCompanyId_Success()
        {
            var client = await GetAuthenticatedClient();
            
            //act
             var response = await client.GetAsync<IdeaBoardResponse>(RequestUris.IdeaboardGetCardsByAssessmentUid(AssessmentUid).AddQueryParameter("companyId", Company.Id));

             //assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status codes does not match");
            Assert.IsTrue(_dimensionCardResponse.All(c => c.Name == response.Dto.Name), "The names does not match");
            Assert.IsTrue(_dimensionCardResponse.All(c => c.AssessmentUid == response.Dto.AssessmentUid), "The assessment Uid does not match");
            Assert.IsTrue(response.Dto.Cards.All(c => c.ItemId > 0), "No card was created");
            Assert.That.ListContains(response.Dto.Cards.Select(a => a.ItemId.ToString()).ToList(), _dimensionCardResponse.Select(d => d.Card.ItemId).First().ToString(), "ItemIds of Dimension cards does not match");
            Assert.That.ListContains(response.Dto.Cards.Select(a => a.ItemId.ToString()).ToList(), _notesCardResponse.Select(d => d.Card.ItemId).First().ToString(), "ItemIds of Notes cards does not match");
            
        }
        
        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("Member")]
        public async Task Ideaboard_GetCards_Get_WithoutCompanyId_Success()
        {
            var client = await GetAuthenticatedClient();

            //act
            var response =
                await client.GetAsync<IdeaBoardResponse>(
                    RequestUris.IdeaboardGetCardsByAssessmentUid(AssessmentUid));
            
            //assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status codes does not match");
            Assert.IsTrue(_dimensionCardResponse.All(c => c.Name == response.Dto.Name), "The names does not match");
            Assert.IsTrue(_dimensionCardResponse.All(c => c.AssessmentUid == response.Dto.AssessmentUid), "The assessment Uid does not match");
            Assert.IsTrue(response.Dto.Cards.All(c => c.ItemId > 0), "No card was created");
            Assert.That.ListContains(response.Dto.Cards.Select(a => a.ItemId.ToString()).ToList(), _dimensionCardResponse.Select(d => d.Card.ItemId).First().ToString(), "ItemIds of Dimension cards does not match");
            Assert.That.ListContains(response.Dto.Cards.Select(a => a.ItemId.ToString()).ToList(), _notesCardResponse.Select(d => d.Card.ItemId).First().ToString(), "ItemIds of Notes Cards does not match");
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("Member")]
        public async Task Ideaboard_GetCards_Get_BadRequest()
        {
            var client = await GetAuthenticatedClient();

            //act
            var assessmentUid = new Guid();
            var response = await client.GetAsync<IList<string>>(RequestUris.IdeaboardGetCardsByAssessmentUid(assessmentUid).AddQueryParameter("companyId", Company.Id));
            
            //assert 
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status codes does not match");
            Assert.AreEqual("'Assessment Uid' is not valid", response.Dto[0], "Response messages are not the same");
        }
        
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("Member")]
        public async Task Ideaboard_GetCards_Get_Unauthorized()
        {
            var client = GetUnauthenticatedClient();

            //act
            var assessmentUid = Guid.NewGuid(); 
            var response = await client.GetAsync<IdeaBoardResponse>(RequestUris.IdeaboardGetCardsByAssessmentUid(assessmentUid).AddQueryParameter("companyId", Company.Id));
            
            //assert
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status codes does not match");
        }
        
        [TestMethod]
        [TestCategory("PartnerAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("Member")]
        public async Task Ideaboard_GetCards_Get_Forbidden()
        {
            var client = await GetAuthenticatedClient();

            //act
            var assessmentUid = Guid.NewGuid();
            const int companyId = SharedConstants.FakeCompanyId;
            var response = await client.GetAsync<IdeaBoardResponse>(RequestUris.IdeaboardGetCardsByAssessmentUid(assessmentUid).AddQueryParameter("companyId", companyId));
            
            //assert
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status codes does not match");
        }
    }
}