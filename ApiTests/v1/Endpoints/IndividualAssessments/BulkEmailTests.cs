using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AtCommon.Api;
using AtCommon.Dtos.Assessments;
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
    public class BulkEmailTests : BaseV1Test
    {
        
        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public async Task Assessments_Individual_Post_BulkEmail_Batch_Success()
        {
            var client = await GetAuthenticatedClient();

            //Create new reviewer
            var reviewer = MemberFactory.GetReviewer();

            var reviewerResponse = await client.PostAsync<ReviewerResponse>(
                RequestUris.AssessmentIndividualReviewer().AddQueryParameter("companyId", Company.Id), reviewer);
            reviewerResponse.EnsureSuccess();

            //Create an individual assessment
            var team = TeamFactory.GetGoiTeam("GOITeamWithMember_", 1);
            var teamCreateResponse = await client.PostAsync<TeamResponse>(RequestUris.Teams(), team);
            teamCreateResponse.EnsureSuccess();

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
            individualAssessment.Members[0].Reviewers = new List<IndividualAssessmentMemberRequest>
            {
                reviewerResponse.Dto.ToAddIndividualMemberRequest()
            };

            var initialIaResponse = await client.PostAsync<IndividualAssessmentResponse>(RequestUris.IndividualAssessments(), individualAssessment);
            initialIaResponse.EnsureSuccess();
            individualAssessment.BatchId = initialIaResponse.Dto.BatchId;
            var createIaResponse = await client.PostAsync<IndividualAssessmentResponse>(RequestUris.IndividualAssessments(), individualAssessment);
            createIaResponse.EnsureSuccess();
            await client.PostAsync<string>(RequestUris.IndividualAssessmentsEmailsAndClaims(), individualAssessment);

            //assign variables for query string
            var participantUid = createIaResponse.Dto.Participants.FirstOrDefault().CheckForNull("There are no Participants in the response").Uid;
            var reviewerUid = reviewerResponse.Dto.Uid;

            var bulkEmailQueryString = new Dictionary<string, object>
            {
                { "companyId", Company.Id },
                { "emailProcessType", "BatchEdit" },
                { "batchId", createIaResponse.Dto.BatchId }
            };

            var response = await client.PostAsync<List<IndividualAssessmentBatchEmailResponse>>(
                RequestUris.AssessmentIndividualBulkEmail().AddQueryParameter(bulkEmailQueryString));

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status code does not match");
            Assert.AreEqual(response.Dto.Count, 2, "Total emails sent does not match");
            Assert.AreEqual(participantUid, response.Dto.FirstOrDefault()?.MemberUid, "Participant Uid doesn't match");
            Assert.AreEqual(reviewerUid, response.Dto.LastOrDefault()?.MemberUid, "Reviewer Uid doesn't match");
            Assert.AreEqual(response.Dto.FirstOrDefault()?.EmailType, "Participant", "Email type doesn't match");
            Assert.AreEqual(response.Dto.LastOrDefault()?.EmailType, "Reviewer", "Email type doesn't match");
            Assert.IsTrue(response.Dto.FirstOrDefault()?.EmailSentAt.CompareTo(new DateTime()) != 0, "Sent time is null or empty");
            Assert.IsTrue(response.Dto.LastOrDefault()?.EmailSentAt.CompareTo(new DateTime()) != 0, "Sent time is null or empty");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public async Task Assessments_Individual_Post_BulkEmail_Assessment_Success()
        {
            var client = await GetAuthenticatedClient();

            //Create new reviewer
            var reviewer = MemberFactory.GetReviewer();

            var reviewerResponse = await client.PostAsync<ReviewerResponse>(
                RequestUris.AssessmentIndividualReviewer().AddQueryParameter("companyId", Company.Id), reviewer);
            reviewerResponse.EnsureSuccess();

            //Create an individual assessment
            var team = TeamFactory.GetGoiTeam("GOITeamWithMember_", 2);
            var teamCreateResponse = await client.PostAsync<TeamResponse>(RequestUris.Teams(), team);
            teamCreateResponse.EnsureSuccess();

            var surveyId = GetIndividualSurveyId();

            //get up individual assessment members
            var individualAssessment = IndividualAssessmentFactory.GetValidIndividualAssessment(Company.Id, User.CompanyName, surveyId);
            individualAssessment.Members = teamCreateResponse.Dto.Members.Select(m => m.ToAddIndividualMemberRequest()).ToList();
            individualAssessment.CompanyId = Company.Id;
            individualAssessment.TeamUid = teamCreateResponse.Dto.Uid;
            individualAssessment.Published = true;
            individualAssessment.Members[0].Reviewers = new List<IndividualAssessmentMemberRequest>
            {
                reviewerResponse.Dto.ToAddIndividualMemberRequest()
            };

            var initialIaResponse = await client.PostAsync<IndividualAssessmentResponse>(RequestUris.IndividualAssessments(), individualAssessment);
            initialIaResponse.EnsureSuccess();
            individualAssessment.BatchId = initialIaResponse.Dto.BatchId;
            var createIaResponse = await client.PostAsync<IndividualAssessmentResponse>(RequestUris.IndividualAssessments(), individualAssessment);
            createIaResponse.EnsureSuccess();
            await client.PostAsync<string>(RequestUris.IndividualAssessmentsEmailsAndClaims(), individualAssessment);

            //assign variables for query string
            var firstParticipantUid = createIaResponse.Dto.Participants.FirstOrDefault().CheckForNull("There are no participants in the response").Uid;
            var secondParticipantUid = createIaResponse.Dto.Participants.LastOrDefault()?.Uid;
            var reviewerUid = reviewerResponse.Dto.Uid;
            var firstAssessmentUid = createIaResponse.Dto.AssessmentList.FirstOrDefault().CheckForNull("There are no AssessmentList in the response").AssessmentUid;
            var secondAssessmentUid = createIaResponse.Dto.AssessmentList.LastOrDefault()?.AssessmentUid;

            var bulkEmailQueryString = new Dictionary<string, object>
            {
                { "companyId", Company.Id },
                { "emailProcessType", "IndividualAssessment" },
                { "assessmentUid", firstAssessmentUid }
            };

            var firstAssessmentResponse = await client.PostAsync<List<IndividualAssessmentBatchEmailResponse>>(
                RequestUris.AssessmentIndividualBulkEmail().AddQueryParameter(bulkEmailQueryString));

            Assert.AreEqual(HttpStatusCode.OK, firstAssessmentResponse.StatusCode, "Status code does not match");
            Assert.AreEqual(2, firstAssessmentResponse.Dto.Count, "Total emails sent does not match");
            Assert.AreEqual(firstParticipantUid, firstAssessmentResponse.Dto.FirstOrDefault()?.MemberUid, "Participant Uid doesn't match");
            Assert.AreEqual(reviewerUid, firstAssessmentResponse.Dto.LastOrDefault()?.MemberUid, "Reviewer Uid doesn't match");
            Assert.AreEqual(firstAssessmentResponse.Dto.FirstOrDefault()?.EmailType, "Participant", "Email type doesn't match");
            Assert.AreEqual(firstAssessmentResponse.Dto.LastOrDefault()?.EmailType, "Reviewer", "Email type doesn't match");
            Assert.IsTrue(firstAssessmentResponse.Dto.FirstOrDefault()?.EmailSentAt.CompareTo(new DateTime()) != 0, "Sent time is null or empty");
            Assert.IsTrue(firstAssessmentResponse.Dto.LastOrDefault()?.EmailSentAt.CompareTo(new DateTime()) != 0, "Sent time is null or empty");

            bulkEmailQueryString = new Dictionary<string, object>
            {
                { "companyId", Company.Id },
                { "emailProcessType", "IndividualAssessment" },
                { "assessmentUid", secondAssessmentUid }
            };

            var secondAssessmentResponse = await client.PostAsync<List<IndividualAssessmentBatchEmailResponse>>(
                RequestUris.AssessmentIndividualBulkEmail().AddQueryParameter(bulkEmailQueryString));

            Assert.AreEqual(HttpStatusCode.OK, secondAssessmentResponse.StatusCode, "Status code does not match");
            Assert.AreEqual(1, secondAssessmentResponse.Dto.Count, "Total emails sent does not match");
            Assert.AreEqual(secondParticipantUid, secondAssessmentResponse.Dto.FirstOrDefault()?.MemberUid, "Participant Uid doesn't match");
            Assert.AreEqual(secondAssessmentResponse.Dto.FirstOrDefault()?.EmailType, "Participant", "Email type doesn't match");
            Assert.IsTrue(secondAssessmentResponse.Dto.FirstOrDefault()?.EmailSentAt.CompareTo(new DateTime()) != 0, "Sent time is null or empty");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public async Task Assessments_Individual_Post_BulkEmail_BatchID_NotFound()
        {
            var client = await GetAuthenticatedClient();

            var bulkEmailQueryString = new Dictionary<string, object>
            {
                { "companyId", Company.Id },
                { "emailProcessType", "BatchEdit" },
                { "batchId", "12345" }
            };

            var response = await client.PostAsync<List<IndividualAssessmentBatchEmailResponse>>(
                RequestUris.AssessmentIndividualBulkEmail().AddQueryParameter(bulkEmailQueryString));

            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode, "Status codes do not match");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public async Task Assessments_Individual_Post_BulkEmail_AssessmentGui_NotFound()
        {
            var client = await GetAuthenticatedClient();

            var bulkEmailQueryString = new Dictionary<string, object>
            {
                { "companyId", Company.Id },
                { "emailProcessType", "IndividualAssessment" },
                { "assessmentUid", Guid.NewGuid() }
            };

            var response = await client.PostAsync<List<IndividualAssessmentBatchEmailResponse>>(
                RequestUris.AssessmentIndividualBulkEmail().AddQueryParameter(bulkEmailQueryString));

            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode, "Status codes do not match");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"),
         TestCategory("Member")]
        public async Task Assessments_Individual_Post_BulkEmail_Unauthorized()
        {
            var client = GetUnauthenticatedClient();

            var bulkEmailQueryString = new Dictionary<string, object>
            {
                { "companyId", Company.Id },
                { "emailProcessType", "BatchEdit" },
                { "batchId", "12345" }
            };

            var response = await client.PostAsync<List<IndividualAssessmentBatchEmailResponse>>(
                RequestUris.AssessmentIndividualBulkEmail().AddQueryParameter(bulkEmailQueryString));

            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status code does not match");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"),
         TestCategory("Member")]
        public async Task Assessments_Individual_Post_BulkEmail_Forbidden()
        {
            var client = await GetAuthenticatedClient();

            var bulkEmailQueryString = new Dictionary<string, object>
            {
                { "companyId", 2 },
                { "emailProcessType", "BatchEdit" },
                { "batchId", "12345" }
            };

            var response = await client.PostAsync<List<IndividualAssessmentBatchEmailResponse>>(
                RequestUris.AssessmentIndividualBulkEmail().AddQueryParameter(bulkEmailQueryString));

            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status code does not match");
        }
    }
}