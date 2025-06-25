using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AtCommon.Api;
using AtCommon.Dtos.IndividualAssessments;
using AtCommon.Dtos.Reports;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiTests.v1.Endpoints.IndividualAssessments
{
    [TestClass]
    [TestCategory("TalentDevelopment")]
    public class GetIndividualAssessmentTests : BaseV1Test
    {
        private static readonly Guid RandomAssessmentId = Guid.NewGuid();

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public async Task Assessments_Individual_Get_Success()
        {
            var client = await GetAuthenticatedClient();

            //Create an individual assessment
            var team = TeamFactory.GetValidPostTeamWithMember("GetValidPostTeamWithMember_");
            var teamCreateResponse = await client.PostAsync<TeamResponse>(RequestUris.Teams(), team);
            teamCreateResponse.EnsureSuccess();
            var teamUid = teamCreateResponse.Dto.Uid;

            //get survey id
            var surveyId = GetIndividualSurveyId();
            
            //get up individual assessment members
            var individualAssessment = IndividualAssessmentFactory.GetValidIndividualAssessment(Company.Id, User.CompanyName, surveyId);
            individualAssessment.Members = new List<IndividualAssessmentMemberRequest>
            {
                teamCreateResponse.Dto.Members.FirstOrDefault().CheckForNull("No team members found in the response").ToAddIndividualMemberRequest()
            };
            individualAssessment.CompanyId = Company.Id;
            individualAssessment.TeamUid = teamUid;

            var createIaResponse = await client.PostAsync<IndividualAssessmentResponse>(RequestUris.IndividualAssessments(), individualAssessment);
            createIaResponse.EnsureSuccess();
            individualAssessment.BatchId = createIaResponse.Dto.BatchId;
            var createIaResponse2 = await client.PostAsync<IndividualAssessmentResponse>(RequestUris.IndividualAssessments(), individualAssessment);
            createIaResponse2.EnsureSuccess();
            await client.PostAsync<string>(RequestUris.IndividualAssessmentsEmailsAndClaims(), individualAssessment);
            var assessmentId = createIaResponse2.Dto.AssessmentList.First().AssessmentUid;

            var response = await client.GetAsync<IndividualAssessmentMemberResponse>(RequestUris.AssessmentGetIndividual(assessmentId));

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, $"Response status code does not match. Received a {response.StatusCode}.");
            Assert.AreEqual(createIaResponse2.Dto.BatchId, response.Dto.BatchId, "Batch ids do not match");
            Assert.IsTrue(response.Dto.AssessmentName.Contains(createIaResponse2.Dto.AssessmentName), $"Assessment name does not contain assessment name {createIaResponse2.Dto.AssessmentName}");
            Assert.AreEqual(individualAssessment.Members[0].FirstName, response.Dto.Members[0].FirstName, "First names do not match");
            Assert.AreEqual(individualAssessment.Members[0].LastName, response.Dto.Members[0].LastName, "Last names do not match");
            Assert.AreEqual(individualAssessment.SurveyTypeId, response.Dto.SurveyTypeId, "Survey types do not match");
            Assert.IsTrue(response.Dto.Members[0].ViewAssessmentResultsUrl != null, "There is no assessment results url");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Assessments_Individual_Get_Unauthorized()
        {
            var client = GetUnauthenticatedClient();

            var response = await client.GetAsync<IndividualAssessmentMemberResponse>(RequestUris.AssessmentGetIndividual(RandomAssessmentId));

            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, $"Response status code does not match. Received a {response.StatusCode}.");
        }

        [TestMethod]
        [TestCategory("KnownDefect")] // Bug: 36471
        [TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Assessments_Individual_Get_Forbidden()
        {
            var client = await GetAuthenticatedClient();

            var assessmentId = new Guid("21ff4218-4ec4-e911-bcd0-0004ffd34370");

            var response = await client.GetAsync<IndividualAssessmentMemberResponse>(RequestUris.AssessmentGetIndividual(assessmentId));

            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, $"Response status code does not match. Received a {response.StatusCode}.");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        public async Task Assessments_Individual_Get_NotFound()
        {
            var client = await GetAuthenticatedClient();

            var response = await client.GetAsync<IndividualAssessmentMemberResponse>(RequestUris.AssessmentGetIndividual(RandomAssessmentId));

            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode, $"Response status code does not match. Received a {response.StatusCode}.");
        }
    }
}
