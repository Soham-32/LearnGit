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
    public class AddIndividualAssessmentTests : BaseV1Test
    {
        private static readonly Guid InvalidUid = Guid.Empty;

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("TeamAdmin")]
        public async Task Assessments_Individual_Post_Success()
        {
            var client = await GetAuthenticatedClient();

            //create team with member
            var team = TeamFactory.GetValidPostTeamWithMember("GetValidPostTeamWithMember_");
            var teamCreateResponse = await client.PostAsync<TeamResponse>(RequestUris.Teams(), team);
            teamCreateResponse.EnsureSuccess();

            //get survey id
            var surveyId = GetIndividualSurveyId();
            
            //get up individual assessment members
            var individualAssessment = IndividualAssessmentFactory.GetValidIndividualAssessment(Company.Id, User.CompanyName, surveyId);
            individualAssessment.Members = new List<IndividualAssessmentMemberRequest>
            {
                teamCreateResponse.Dto.Members.FirstOrDefault().CheckForNull("No team members found in the response").ToAddIndividualMemberRequest()
            };
            individualAssessment.CompanyId = Company.Id;
            individualAssessment.TeamUid = teamCreateResponse.Dto.Uid;

            var response = await client.PostAsync<IndividualAssessmentResponse>(RequestUris.IndividualAssessments(), individualAssessment);
            response.EnsureSuccess();
            individualAssessment.BatchId = response.Dto.BatchId;
            var response2 = await client.PostAsync<IndividualAssessmentResponse>(RequestUris.IndividualAssessments(), individualAssessment);
            response2.EnsureSuccess();
            await client.PostAsync<string>(RequestUris.IndividualAssessmentsEmailsAndClaims(), individualAssessment);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Response status code does not match");
            Assert.IsTrue(!string.IsNullOrEmpty(response.Dto.BatchId.ToString()), "Batch Id cannot be null or empty");
            Assert.IsTrue(!string.IsNullOrEmpty(response.Dto.AssessmentName), "Assessment name cannot be null or empty");
            Assert.IsTrue(!string.IsNullOrEmpty(response.Dto.TeamUid), "Team Uid cannot be null or empty");
            Assert.AreEqual(individualAssessment.TeamUid.ToString(), response.Dto.TeamUid, "Team Uid is not equal");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("TeamAdmin")]
        public async Task Assessments_Individual_PostInvalidTeamUid()
        {
            var client = await GetAuthenticatedClient();

            //create team with member
            var team = TeamFactory.GetValidPostTeamWithMember("GetValidPostTeamWithMember_");
            var teamCreateResponse = await client.PostAsync<TeamResponse>(RequestUris.Teams(), team);
            teamCreateResponse.EnsureSuccess();

            //get survey id
            var surveyId = GetIndividualSurveyId();
            
            //get up individual assessment members
            var individualAssessment = IndividualAssessmentFactory.GetValidIndividualAssessment(Company.Id, User.CompanyName, surveyId);
            individualAssessment.Members = new List<IndividualAssessmentMemberRequest>
            {
                teamCreateResponse.Dto.Members.FirstOrDefault().CheckForNull("No team members found in the response").ToAddIndividualMemberRequest()
            };
            individualAssessment.CompanyId = Company.Id;
            individualAssessment.TeamUid = InvalidUid;

            var response = await client.PostAsync(RequestUris.IndividualAssessments(), individualAssessment.ToStringContent());

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Response status code does not match");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("TeamAdmin")]
        public async Task Assessments_Individual_Post_InvalidMemberUid()
        {
            var client = await GetAuthenticatedClient();

            //create team with member
            var team = TeamFactory.GetValidPostTeamWithMember("GetValidPostTeamWithMember_");
            var teamCreateResponse = await client.PostAsync<TeamResponse>(RequestUris.Teams(), team);
            teamCreateResponse.EnsureSuccess();
            var teamUid = teamCreateResponse.Dto.Uid;
            
            //get survey id
            var surveyId = GetIndividualSurveyId();
            var individualAssessment = IndividualAssessmentFactory.GetValidIndividualAssessment(Company.Id, User.CompanyName, surveyId); 
            individualAssessment.Members = new List<IndividualAssessmentMemberRequest>
            {
                teamCreateResponse.Dto.Members.FirstOrDefault().CheckForNull("No team members found in the response").ToAddIndividualMemberRequest()
            };
            individualAssessment.CompanyId = Company.Id;
            individualAssessment.TeamUid = teamUid;
            individualAssessment.Members[0].Uid = InvalidUid;

            var response = await client.PostAsync(RequestUris.IndividualAssessments(), individualAssessment.ToStringContent());

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Response status code does not match");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("TeamAdmin"), TestCategory("Member")]
        public async Task Assessments_Individual_Post_Unauthorized()
        {
            var client = await GetAuthenticatedClient();

            //get companyId
            var companyResponse = await client.GetAsync<IList<TeamProfileResponse>>(RequestUris.Teams());
            companyResponse.EnsureSuccess();
            const string teamType = "Team";
            var team = companyResponse.Dto.FirstOrDefault(dto => dto.Type == teamType);
            if (team == null) throw new Exception($"<{teamType}> type was not found in the response");
            var teamUid = team.Uid;
            
            //get survey id
            var surveyId = GetIndividualSurveyId();
            

            //unauthenticate user
            client = GetUnauthenticatedClient();

            //get up individual assessment members
            var individualAssessment = IndividualAssessmentFactory.GetValidIndividualAssessment(Company.Id, User.CompanyName, surveyId); 
            individualAssessment.CompanyId = Company.Id;
            individualAssessment.TeamUid = teamUid;

            var response = await client.PostAsync<IndividualAssessmentResponse>(RequestUris.IndividualAssessments(), individualAssessment);

            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Response status code does not match");
        }

        [TestMethod]
        [TestCategory("Member")]
        public async Task Assessments_Individual_Post_Forbidden()
        {
            var client = await GetAuthenticatedClient();

            //get companyId
            var companyResponse = await client.GetAsync<IList<TeamProfileResponse>>(RequestUris.Teams());
            companyResponse.EnsureSuccess();
            var teamUid = companyResponse.Dto.First(dto => dto.Type == "Team").Uid;

            //get survey id
            var surveyId = GetIndividualSurveyId();
            
            //get up individual assessment members
            var individualAssessment = IndividualAssessmentFactory.GetValidIndividualAssessment(Company.Id, User.CompanyName, surveyId);
            individualAssessment.CompanyId = Company.Id;
            individualAssessment.TeamUid = teamUid;

            var response = await client.PostAsync<IndividualAssessmentResponse>(RequestUris.IndividualAssessments(), individualAssessment);

            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Response status code does not match");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("TeamAdmin")]
        public async Task Assessments_Individual_Post_SaveAsDraft_Success()
        {
            var client = await GetAuthenticatedClient();

            //create team with member
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


            var firstPostResponse = await client.PostAsync<IndividualAssessmentResponse>(RequestUris.IndividualAssessments(), individualAssessment);
            firstPostResponse.EnsureSuccess();

            individualAssessment.BatchId = firstPostResponse.Dto.BatchId;
            individualAssessment.AssessmentName = "I changed it";
            var saveDraftResponse = await client.PostAsync<IndividualAssessmentResponse>(RequestUris.IndividualAssessments(), individualAssessment);

            Assert.AreEqual(HttpStatusCode.OK, saveDraftResponse.StatusCode, "Response status code does not match");
            Assert.IsTrue(!string.IsNullOrEmpty(saveDraftResponse.Dto.BatchId.ToString()), "Batch Id cannot be null or empty");
            Assert.IsTrue(!string.IsNullOrEmpty(saveDraftResponse.Dto.AssessmentName), "Assessment name cannot be null or empty");
            Assert.IsTrue(!string.IsNullOrEmpty(saveDraftResponse.Dto.TeamUid), "Team Uid cannot be null or empty");
            Assert.AreEqual(individualAssessment.TeamUid.ToString(), saveDraftResponse.Dto.TeamUid, "Team Uid is not equal");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("TeamAdmin")]
        public async Task Assessments_Individual_Post_Publish()
        {
            var client = await GetAuthenticatedClient();

            //create team with member
            var team = TeamFactory.GetValidPostTeamWithMember("GetValidPostTeamWithMember_");
            team.Members[0].Email = $"ah_automation+{team.Members[0].FirstName}@agiletransformation.com";
            team.Stakeholders[0].Email = $"ah_automation+{team.Stakeholders[0].FirstName}@agiletransformation.com";
            var teamCreateResponse = await client.PostAsync<TeamResponse>(RequestUris.Teams(), team);
            teamCreateResponse.EnsureSuccess();

            //get survey id
            var surveyId = GetIndividualSurveyId();
            
            //get up individual assessment members
            var individualAssessment = IndividualAssessmentFactory.GetValidIndividualAssessment(Company.Id, User.CompanyName, surveyId);
            individualAssessment.Members = new List<IndividualAssessmentMemberRequest>
            {
                teamCreateResponse.Dto.Members.FirstOrDefault().CheckForNull("No team members found in the response").ToAddIndividualMemberRequest()
            };
            individualAssessment.CompanyId = Company.Id;
            individualAssessment.TeamUid = teamCreateResponse.Dto.Uid;
            individualAssessment.Published = true;

            var response = await client.PostAsync<IndividualAssessmentResponse>(RequestUris.IndividualAssessments(), individualAssessment);
            response.EnsureSuccess();
            individualAssessment.BatchId = response.Dto.BatchId;
            var response2 = await client.PostAsync<IndividualAssessmentResponse>(RequestUris.IndividualAssessments(), individualAssessment);
            response2.EnsureSuccess();
            await client.PostAsync<string>(RequestUris.IndividualAssessmentsEmailsAndClaims(), individualAssessment);

            Assert.AreEqual(HttpStatusCode.OK, response2.StatusCode, $"Response status code does not match. Receiving a {response2.StatusCode}");
            Assert.IsTrue(!string.IsNullOrEmpty(response2.Dto.BatchId.ToString()), "Batch Id cannot be null or empty");
            Assert.IsTrue(!string.IsNullOrEmpty(response2.Dto.AssessmentName), "Assessment name cannot be null or empty");
            Assert.IsTrue(!string.IsNullOrEmpty(response2.Dto.TeamUid), "Team Uid cannot be null or empty");
            Assert.AreEqual(individualAssessment.TeamUid.ToString(), response2.Dto.TeamUid, "Team Uid is not equal");
            
        }
    }
}
