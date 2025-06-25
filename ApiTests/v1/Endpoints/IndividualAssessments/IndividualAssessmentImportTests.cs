using System;
using System.Collections.Generic;
using System.IO;
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
    public class IndividualAssessmentImportTests : BaseV1Test
    {
        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("TeamAdmin")]
        public async Task Assessments_Individual_Import_Post_Success()
        {
            var client = await GetAuthenticatedClient();

            //create team with member
            var team = TeamFactory.GetValidPostTeamWithMember("GetValidPostTeamWithMember_");
            var teamCreateResponse = await client.PostAsync<TeamResponse>(RequestUris.Teams(), team);
            teamCreateResponse.EnsureSuccess();

            //get survey id
            var surveyId = GetIndividualSurveyId();
            
            //create an individual assessment
            var individualAssessment = IndividualAssessmentFactory.GetValidIndividualAssessment(Company.Id, User.CompanyName, surveyId);
            individualAssessment.Members = new List<IndividualAssessmentMemberRequest>
            {
                teamCreateResponse.Dto.Members.FirstOrDefault().CheckForNull("No team members found in the response").ToAddIndividualMemberRequest()
            };
            individualAssessment.CompanyId = Company.Id;
            individualAssessment.TeamUid = teamCreateResponse.Dto.Uid;

            var individualAssessmentResponse = await client.PostAsync<IndividualAssessmentResponse>(RequestUris.IndividualAssessments(), individualAssessment);
            individualAssessmentResponse.EnsureSuccess();
            individualAssessment.BatchId = individualAssessmentResponse.Dto.BatchId;
            var individualAssessmentResponse2 = await client.PostAsync<IndividualAssessmentResponse>(RequestUris.IndividualAssessments(), individualAssessment);
            individualAssessmentResponse2.EnsureSuccess();
            await client.PostAsync<string>(RequestUris.IndividualAssessmentsEmailsAndClaims(), individualAssessment);

            //import a file of participant and reviewer
            var queryString = new Dictionary<string, object>
            {
                { "companyId", Company.Id },
                { "batchId", individualAssessmentResponse2.Dto.BatchId },
                { "assessmentName", individualAssessmentResponse2.Dto.AssessmentName },
                { "pointOfContact", individualAssessment.PointOfContact },
                { "pointOfContactEmail", individualAssessment.PointOfContactEmail },
                { "surveyTypeId", surveyId }
            };
            var response = await client.PostUploadAsync<BatchImportIndividualAssessmentResponse>(RequestUris.AssessmentIndividualImport()
                .AddQueryParameter(queryString), Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                "Resources\\TestData\\TalentDevImport.xlsx"));

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Response status code does not match");
            Assert.AreEqual(2, response.Dto.Batch.Members.Count, "Total participant does not match");
            Assert.AreEqual(1, response.Dto.Batch.Members.LastOrDefault()!.Reviewers.Count, "Total reviewer for imported participant does not match");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("TeamAdmin")]
        public async Task Assessments_Individual_Import_Post_Unauthorized()
        {
            var client = await GetAuthenticatedClient();

            //create team with member
            var team = TeamFactory.GetValidPostTeamWithMember("GetValidPostTeamWithMember_");
            var teamCreateResponse = await client.PostAsync<TeamResponse>(RequestUris.Teams(), team);
            teamCreateResponse.EnsureSuccess();

            //get survey id
            var surveyId = GetIndividualSurveyId();

            //create an individual assessment
            var individualAssessment = IndividualAssessmentFactory.GetValidIndividualAssessment(Company.Id, User.CompanyName, surveyId);
            individualAssessment.Members = new List<IndividualAssessmentMemberRequest>
            {
                teamCreateResponse.Dto.Members.FirstOrDefault().CheckForNull("No team members found in the response").ToAddIndividualMemberRequest()
            };
            individualAssessment.CompanyId = Company.Id;
            individualAssessment.TeamUid = teamCreateResponse.Dto.Uid;

            var individualAssessmentResponse = await client.PostAsync<IndividualAssessmentResponse>(RequestUris.IndividualAssessments(), individualAssessment);

            //import a file of participant and reviewer
            client = GetUnauthenticatedClient();
            var queryString = new Dictionary<string, object>
            {
                { "companyId", Company.Id },
                { "batchId", individualAssessmentResponse.Dto.BatchId },
                { "assessmentName", individualAssessmentResponse.Dto.AssessmentName },
                { "pointOfContact", individualAssessment.PointOfContact },
                { "pointOfContactEmail", individualAssessment.PointOfContactEmail },
                { "surveyTypeId", surveyId }
            };
            var response = await client.PostUploadAsync<BatchImportIndividualAssessmentResponse>(RequestUris.AssessmentIndividualImport()
                .AddQueryParameter(queryString), Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                "Resources\\TestData\\TalentDevImport.xlsx"));

            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status code doesn't match.");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("TeamAdmin")]
        public async Task Assessments_Individual_Import_Post_NotFound()
        {
            var client = await GetAuthenticatedClient();
            
            //import a file of participant and reviewer
            var response = await client.PostUploadAsync<BatchImportIndividualAssessmentResponse>(RequestUris.AssessmentIndividualImport().AddQueryParameter("companyId", Company.Id)
                .AddQueryParameter("batchId", 123456), Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                "Resources\\TestData\\TalentDevImport.xlsx"));

            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode, "Status code doesn't match.");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("TeamAdmin")]
        public async Task Assessments_Individual_Import_Post_Forbidden()
        {
            var client = await GetAuthenticatedClient();

            //import a file of participant and reviewer
            var response = await client.PostUploadAsync<BatchImportIndividualAssessmentResponse>(RequestUris.AssessmentIndividualImport().AddQueryParameter("companyId", 2)
                .AddQueryParameter("batchId", 123456), Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                "Resources\\TestData\\TalentDevImport.xlsx"));

            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status code doesn't match.");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("TeamAdmin")]
        public async Task Assessments_Individual_Import_Post_Invalid()
        {
            var client = await GetAuthenticatedClient();

            //create team with member
            var team = TeamFactory.GetValidPostTeamWithMember("GetValidPostTeamWithMember_");
            var teamCreateResponse = await client.PostAsync<TeamResponse>(RequestUris.Teams(), team);
            teamCreateResponse.EnsureSuccess();

            //get survey id
            var surveyId = GetIndividualSurveyId();

            //create an individual assessment
            var individualAssessment = IndividualAssessmentFactory.GetValidIndividualAssessment(Company.Id, User.CompanyName, surveyId);
            individualAssessment.Members = new List<IndividualAssessmentMemberRequest>
            {
                teamCreateResponse.Dto.Members.FirstOrDefault().CheckForNull("No team members found in the response").ToAddIndividualMemberRequest()
            };
            individualAssessment.CompanyId = Company.Id;
            individualAssessment.TeamUid = teamCreateResponse.Dto.Uid;

            var individualAssessmentResponse = await client.PostAsync<IndividualAssessmentResponse>(RequestUris.IndividualAssessments(), individualAssessment);
            
            var queryString = new Dictionary<string, object>
            {
                { "companyId", Company.Id },
                { "batchId", individualAssessmentResponse.Dto.BatchId },
                { "assessmentName", individualAssessmentResponse.Dto.AssessmentName },
                { "pointOfContact", individualAssessment.PointOfContact },
                { "pointOfContactEmail", individualAssessment.PointOfContactEmail },
                { "surveyTypeId", surveyId }
            };
            var response = await client.PostUploadAsync<List<string>>(RequestUris
                .AssessmentIndividualImport()
                .AddQueryParameter(queryString), Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                "Resources\\TestData\\TalentDevImport_Invalid.xlsx"));

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Response status code does not match");
            Assert.AreEqual(2, response.Dto.Count, "Total error does not match");
            Assert.AreEqual("Row number 4: Required for field 'Reviewer Last Name'", response.Dto.FirstOrDefault()?.Trim(), "First error message does not match");
            Assert.AreEqual("Row number 5: Required for field 'Participant Email'", response.Dto.LastOrDefault()?.Trim(), "Second error message does not match");
        }
    }
}
