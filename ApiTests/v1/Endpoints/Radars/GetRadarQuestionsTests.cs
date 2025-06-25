using AtCommon.Api;
using AtCommon.Dtos.Radars;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ApiTests.v1.Endpoints.Radars
{
    [TestClass]
    [TestCategory("Radars")]
    public class GetRadarQuestionsTests : BaseV1Test
    {
        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("SiteAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("Member") , TestCategory("PartnerAdmin")]
        public async Task Radar_Question_Get_BySurveyId_Success()
        {
            //given
            var client = await GetAuthenticatedClient();

            //when
            var response = await client.GetAsync<RadarQuestionDetailsResponse>(
                RequestUris.RadarQuestions(Company.Id).AddQueryParameter("surveyId",  SharedConstants.TeamSurveyId));

            //then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status Code does not match.");
            Assert.AreEqual(SharedConstants.TeamSurveyId, response.Dto.SurveyId, "SurveyId doesn't match");
            Assert.IsTrue(response.Dto.SurveyUid.CompareTo(Guid.Empty) != 0, "SurveyUid is empty");
            Assert.IsTrue(!string.IsNullOrEmpty(response.Dto.Name), "Name is empty or null");
            Assert.IsTrue(response.Dto.Dimensions.Any(), "There were no Dimensions in the response");

            //Dimension
            Assert.IsTrue(response.Dto.Dimensions.All(d => !string.IsNullOrEmpty(d.Name)), "Dimension Name is empty or null");
            Assert.IsTrue(response.Dto.Dimensions.All(d => d.DimensionId >= 0), "Dimension id isn't valid");
            Assert.IsTrue(response.Dto.Dimensions.All(d => d.Subdimensions.Any()), "PulseSubdimensions count is 0");

            //Sub Dimension
            Assert.IsTrue(response.Dto.Dimensions.All(d => d.Subdimensions.All(s => s.SubdimensionId > 0)), "SubdimensionId is not valid");
            Assert.IsTrue(response.Dto.Dimensions.All(d => d.Subdimensions.All(s => !string.IsNullOrEmpty(s.Name))), "Subdimension Name is empty or null");
            Assert.IsTrue(response.Dto.Dimensions.All(d => d.Subdimensions.All(s => s.Competencies.Any())), "Competencies count is 0");

            //Competency
            Assert.IsTrue(response.Dto.Dimensions.All(d => d.Subdimensions.All(s => s.Competencies.All(c => c.CompetencyId > 0))), "CompetencyId isn't valid");
            Assert.IsTrue(response.Dto.Dimensions.All(d => d.Subdimensions.All(s => s.Competencies.All(c => !string.IsNullOrEmpty(c.Name)))), "Competency Name is empty or null");
            Assert.IsTrue(response.Dto.Dimensions.Where(x => !x.Name.Equals("Finish")).All(d => d.Subdimensions.All(s => s.Competencies.All(c => c.Questions.Any()))), "PulseQuestions count is 0");

            //Question
            Assert.IsTrue(response.Dto.Dimensions.All(d => d.Subdimensions.All(s => s.Competencies.All(c => c.Questions.All(q => q.QuestionId > 0)))), "QuestionId is not valid");

        }


        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("SiteAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Radar_Question_Get_BySurveyId_Unauthorized()
        {
            //given
            var client = GetUnauthenticatedClient();

            //when
            var response = await client.GetAsync<RadarQuestionDetailsResponse>(
                RequestUris.RadarQuestions(Company.Id).AddQueryParameter("surveyId",  SharedConstants.TeamSurveyId));

            //then
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status Code does not match.");
        }


        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("SiteAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Radar_Question_Get_BySurveyId_BadRequest()
        {
            //given
            var client = await GetAuthenticatedClient();

            //when
            var response = await client.GetAsync(
                RequestUris.RadarQuestions(Company.Id).AddQueryParameter("surveyId",  "0"));

            //then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status Code does not match.");
        }


        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("SiteAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Radar_Question_Get_BySurveyId_NotFound()
        {
            //given
            var client = await GetAuthenticatedClient();

            //when
            var response = await client.GetAsync(
                RequestUris.RadarQuestions(Company.Id).AddQueryParameter("surveyId",  "88888888"));

            //then
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode, "Status Code does not match.");
        }


        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("SiteAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Radar_Question_Get_Without_Any_Parameters_BadRequest()
        {
            //given
            var client = await GetAuthenticatedClient();

            //when
            var response = await client.GetAsync(RequestUris.RadarQuestions(Company.Id));

            //then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status Code does not match.");
        }

    }
}
