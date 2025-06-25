using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AtCommon.Api;
using AtCommon.Dtos.Assessments;
using AtCommon.Dtos.Teams;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiTests.v1.Endpoints.TeamAssessments
{
    [TestClass]
    [TestCategory("TeamAssessment"), TestCategory("Public")]
    public class GetAssessmentsTests : BaseV1Test
    {
        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Assessments_Get_MissingToken_Unauthorized()
        {
            // arrange
            var client = GetUnauthenticatedClient();

            var unique = Guid.NewGuid();

            // act
            var response = await client.GetAsync(RequestUris.Assessments(unique));

            // assert
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status codes do not match");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Assessments_Get_InvalidUid_NotFound()
        {
            // arrange
            var client = await GetAuthenticatedClient();

            var unique = Guid.NewGuid();

            // act
            var response = await client.GetAsync(RequestUris.Assessments(unique));

            // assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode, "Status codes do not match");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("Member")]
        public async Task Assessments_Get_Success()
        {
            // arrange
            var client = await GetAuthenticatedClient();

            var teamsResponse = await client.GetAsync<IList<TeamProfileResponse>>(RequestUris.Teams());
            teamsResponse.EnsureSuccess();
            const string teamName = "Automation Radar Team";
            var firstTeam = teamsResponse.Dto.FirstOrDefault(team => team.Name == teamName);
            if (firstTeam == null) throw new Exception($"<{teamName}> was not found in the response");
            var teamAssessmentsResponse = await client.GetAsync<IList<AssessmentSummaryResponse>>(RequestUris.TeamAssessments(firstTeam.Uid));
            teamAssessmentsResponse.EnsureSuccess();
            var firstDto = teamAssessmentsResponse.Dto.First();
            var assessmentGui = firstDto.Uid;
            var assessmentName = firstDto.AssessmentName;
            
            // act
            var response = await client.GetAsync<AssessmentResponse>(RequestUris.Assessments(assessmentGui));

            // assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status Code does not match.");
            Assert.AreEqual(assessmentName, response.Dto.AssessmentName, "Assessment Name does not match");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member")]
        public async Task Assessments_Results_Get_MissingToken_Unauthorized()
        {
            // arrange
            var client = GetUnauthenticatedClient();

            var unique = Guid.NewGuid();

            // act
            var response = await client.GetAsync(RequestUris.AssessmentsResults(unique));

            // assert
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status codes do not match");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"),TestCategory("PartnerAdmin"),TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Assessments_Results_Get_InvalidUid_Forbidden()
        {
            // arrange
            var client = await GetAuthenticatedClient();

            var unique = Guid.NewGuid();

            // act
            var response = await client.GetAsync(RequestUris.AssessmentsResults(unique));

            // assert
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status codes do not match");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("Member")]
        public async Task Assessments_Results_Get_Success()
        {
            // arrange
            var client = await GetAuthenticatedClient();

            var teamsResponse = await client.GetAsync<IList<TeamProfileResponse>>(RequestUris.Teams());
            teamsResponse.EnsureSuccess();
            const string teamName = SharedConstants.RadarTeam;
            var firstTeam = teamsResponse.Dto.FirstOrDefault(team => team.Name == teamName);
            if (firstTeam == null) throw new Exception($"<{teamName}> was not found in the response");
            var teamAssessmentsResponse = await client.GetAsync<IList<AssessmentSummaryResponse>>(RequestUris.TeamAssessments(firstTeam.Uid));
            teamAssessmentsResponse.EnsureSuccess();

            const string assessmentName = SharedConstants.AssessmentResponse25Radar;
            var assessmentUid = teamAssessmentsResponse.Dto.First(a => a.AssessmentName.Equals(assessmentName)).Uid;
            const int expectedAssessmentResultValue = 5;

            // act
            var response = await client.GetAsync<AssessmentsResultsResponse>(RequestUris.AssessmentsResults(assessmentUid));

            // assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Response Status Code does not match");
            Assert.AreEqual(assessmentName, response.Dto.Name, "Assessment Name does not match");
            Assert.AreEqual(firstTeam.TeamId, response.Dto.TeamId, "Team Id does not match");
            Assert.AreEqual(assessmentUid, response.Dto.UId,"AssessmentUid does not match");

            Assert.IsTrue(response.Dto.Dimensions.All(dimensionsName => dimensionsName.Name != null), "DimensionsName is null");
            Assert.IsTrue(response.Dto.Dimensions.SelectMany(subDimensions => subDimensions.SubDimensions).All(subDimensionsName => 
                subDimensionsName.Name != null), "SubDimensions Name is null");
            Assert.IsTrue(response.Dto.Dimensions.SelectMany(subDimensions => subDimensions.SubDimensions).SelectMany(subDimensionsName => 
                subDimensionsName.Competencies).All(competencies => competencies.Name != null), "Competencies Name is null");
            
            Assert.IsTrue(response.Dto.Dimensions.All(subDimensions => subDimensions.SubDimensions.All(subDimensionsName =>
                    subDimensionsName.Competencies.All(a => a.AverageValue.Equals(expectedAssessmentResultValue)))), "Average value of Competencies does not match");

            Assert.IsTrue(response.Dto.Dimensions.All(subDimensions => subDimensions.SubDimensions.All((subDimensionsName =>
                    subDimensionsName.Competencies.All(competencies => competencies.Min.Equals(expectedAssessmentResultValue))))), "minimum value of competencies does not match");

            Assert.IsTrue(response.Dto.Dimensions.All(subDimensions => subDimensions.SubDimensions.All((subDimensionsName =>
                subDimensionsName.Competencies.All(competencies => competencies.Max.Equals(expectedAssessmentResultValue))))), "maximum value of competencies does not match");


            Assert.IsTrue(response.Dto.Dimensions.SelectMany(subDimensions => subDimensions.SubDimensions).SelectMany(subDimensionsName => 
                subDimensionsName.Competencies).SelectMany(competenciesName => competenciesName.Contacts).All(contact => contact.ContactId >= 0), "ContactId is less then 0");
            
            Assert.IsTrue(response.Dto.Dimensions.All(subDimensions => subDimensions.SubDimensions.All(subDimensionsName =>
                subDimensionsName.Competencies.All(competenciesName => competenciesName.Contacts.All(contact => contact.AverageValue.Equals(expectedAssessmentResultValue))))), "Contact averageValue does not match");

            Assert.IsTrue(response.Dto.Dimensions.All(subDimensions => subDimensions.SubDimensions.All(
                subDimensionsName => subDimensionsName.Competencies.All(competenciesName => competenciesName.Contacts.All(contacts =>
                    contacts.Questions.All(question => question.QuestionId>=0))))), "QuestionsId is less then 0");
            Assert.IsTrue(response.Dto.Dimensions.All(subDimensions => subDimensions.SubDimensions.All(
                    subDimensionsName => subDimensionsName.Competencies.All(competenciesName => competenciesName.Contacts.All(contacts =>
                            contacts.Questions.All(question => question.Value.Equals(expectedAssessmentResultValue)))))), "Questions value does not match");
        }


        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("Member")]
        public async Task Assessments_Results_Get_NoContent()
        {
            // arrange
            var client = await GetAuthenticatedClient();

            var teamsResponse = await client.GetAsync<IList<TeamProfileResponse>>(RequestUris.Teams());
            teamsResponse.EnsureSuccess();
            const string teamName = SharedConstants.RadarTeam;

            var firstTeam = teamsResponse.Dto.FirstOrDefault(team => team.Name == teamName);
            if (firstTeam == null) throw new Exception($"<{teamName}> was not found in the response");
            var teamAssessmentsResponse = await client.GetAsync<IList<AssessmentSummaryResponse>>(RequestUris.TeamAssessments(firstTeam.Uid));
            teamAssessmentsResponse.EnsureSuccess();

            const string assessmentName = SharedConstants.Th2Radar;
            var assessmentUid = teamAssessmentsResponse.Dto.First(a => a.AssessmentName.Equals(assessmentName)).Uid;
            
            // act
            var response = await client.GetAsync<AssessmentsResultsResponse>(RequestUris.AssessmentsResults(assessmentUid));

            // assert
            Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode, "Response Status Code does not match.");
        }
    }
}
