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
    public class EditIndividualAssessmentResendReviewerEmailTests : BaseV1Test
    {
        
        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public async Task Assessments_Individual_Post_ResendReviewerEmail_Success()
        {
            var client = await GetAuthenticatedClient();

            //Create new reviewer
            var reviewer = MemberFactory.GetReviewer();

            var reviewerResponse = await client.PostAsync<ReviewerResponse>(
                RequestUris.AssessmentIndividualReviewer().AddQueryParameter("companyId", Company.Id), reviewer);
            reviewerResponse.EnsureSuccess();

            //Create an individual assessment
            var team = TeamFactory.GetValidPostTeamWithMember("GetValidPostTeamWithMember_");
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
            var assessmentUid = createIaResponse.Dto.AssessmentList.First().AssessmentUid;

            var emailQueryString = new Dictionary<string, object>
            {
                { "companyId", Company.Id },
                { "assessmentUid", assessmentUid },
                { "memberUid", reviewerResponse.Dto.Uid },
                {"emailType", SharedConstants.EmailTypeReviewer }
            };

            var response = await client.PostAsync<IndividualAssessmentEmailResponse>(
                RequestUris.AssessmentIndividualEmail().AddQueryParameter(emailQueryString));

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status code does not match");
            Assert.IsTrue(response.Dto.EmailSentAt.CompareTo(new DateTime()) != 0, "Sent time is null or empty");
        }
        
        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public async Task Assessments_Individual_Post_ResendParticipantEmail_Success()
        {
            var client = await GetAuthenticatedClient();

            //Create new reviewer
            var reviewer = MemberFactory.GetReviewer();

            var reviewerResponse = await client.PostAsync<ReviewerResponse>(
                RequestUris.AssessmentIndividualReviewer().AddQueryParameter("companyId", Company.Id), reviewer);
            reviewerResponse.EnsureSuccess();

            //Create an individual assessment
            var team = TeamFactory.GetValidPostTeamWithMember("GetValidPostTeamWithMember_");
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
            var assessmentUid = createIaResponse.Dto.AssessmentList.First().AssessmentUid;
            var participantUid = createIaResponse.Dto.Participants.First().Uid;

            var emailQueryString = new Dictionary<string, object>
            {
                { "companyId", Company.Id },
                { "assessmentUid", assessmentUid },
                { "memberUid", participantUid},
                {"emailType", SharedConstants.EmailTypeParticipant }
            };

            var response = await client.PostAsync<IndividualAssessmentEmailResponse>(
                RequestUris.AssessmentIndividualEmail().AddQueryParameter(emailQueryString));

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status code does not match");
            Assert.IsTrue(response.Dto.EmailSentAt.CompareTo(new DateTime()) != 0, "Sent time is null or empty");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        public async Task Assessments_Individual_Post_ResendEmail_AssessmentUid_NotFound()
        {
            var client = await GetAuthenticatedClient();

            var emailQueryString = new Dictionary<string, object>
            {
                { "companyId", Company.Id },
                { "assessmentUid", Guid.NewGuid() },
                { "memberUid", Guid.NewGuid()},
                {"emailType", SharedConstants.EmailTypeReviewer }
            };

            var response = await client.PostAsync<IndividualAssessmentEmailResponse>(
                RequestUris.AssessmentIndividualEmail().AddQueryParameter(emailQueryString));

            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode, "Status codes do not match");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"),
         TestCategory("Member")]
        public async Task Assessments_Individual_Post_ResendEmail_Unauthorized()
        {
            var client = GetUnauthenticatedClient();

            var emailQueryString = new Dictionary<string, object>
            {
                { "companyId", Company.Id },
                { "assessmentUid", Guid.NewGuid() },
                { "memberUid", Guid.NewGuid() },
                {"emailType", SharedConstants.EmailTypeReviewer }
            };

            var response = await client.PostAsync<IndividualAssessmentEmailResponse>(
                RequestUris.AssessmentIndividualEmail().AddQueryParameter(emailQueryString));

            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status code does not match");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"),
         TestCategory("Member")]
        public async Task Assessments_Individual_Post_ResendEmail_Forbidden()
        {
            var client = await GetAuthenticatedClient();

            var emailQueryString = new Dictionary<string, object>
            {
                { "companyId", "2" },
                { "assessmentUid", Guid.NewGuid() },
                { "memberUid", Guid.NewGuid() },
                {"emailType", SharedConstants.EmailTypeReviewer }
            };

            var response = await client.PostAsync<IndividualAssessmentEmailResponse>(
                RequestUris.AssessmentIndividualEmail().AddQueryParameter(emailQueryString));

            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status code does not match");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"),
         TestCategory("Member")]
        public async Task Assessments_Individual_Post_ResendEmail_InvalidInput()
        {
            var client = await GetAuthenticatedClient();

            //invalid assessmentUid
            var emailQueryString = new Dictionary<string, object>
            {
                { "companyId", Company.Id },
                { "assessmentUid", "invalidAssessmentUid" },
                { "memberUid", Guid.NewGuid() },
                {"emailType", SharedConstants.EmailTypeReviewer }
            };

            var response = await client.PostAsync<IList<string>>(RequestUris.AssessmentIndividualEmail()
                .AddQueryParameter(emailQueryString));

            Assert.AreEqual(1, response.Dto.Count, "Total response message doesn't match");
            Assert.AreEqual("'Assessment Uid' is not valid", response.Dto.FirstOrDefault(), "Response message for invalid assessment uid doesn't match");
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status code does not match");

            //invalid memberUid
            emailQueryString = new Dictionary<string, object>
            {
                { "companyId", Company.Id },
                { "assessmentUid", Guid.NewGuid() },
                { "memberUid", "invalidMemberUid" },
                {"emailType", SharedConstants.EmailTypeReviewer }
            };
  
            response = await client.PostAsync<IList<string>>(RequestUris.AssessmentIndividualEmail()
                .AddQueryParameter(emailQueryString));

            Assert.AreEqual(1, response.Dto.Count, "Total response message doesn't match");
            Assert.AreEqual("'Member Uid' is not valid", response.Dto.FirstOrDefault(), "Response message for invalid member uid  doesn't match");
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status code does not match");
            
            //invalid emailType
            emailQueryString = new Dictionary<string, object>
            {
                { "companyId", Company.Id },
                { "assessmentUid", Guid.NewGuid() },
                { "memberUid", Guid.NewGuid() },
                {"emailType", "Neither" }
            };

            response = await client.PostAsync<IList<string>>(RequestUris.AssessmentIndividualEmail()
                .AddQueryParameter(emailQueryString));

            Assert.AreEqual(1, response.Dto.Count, "Total response message doesn't match");
            Assert.AreEqual("'EmailType' options are Participant or Reviewer", response.Dto.FirstOrDefault(), "Response message for invalid email type doesn't match");
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status code does not match");

            //invalid assessmentUid, memberUid, and emailType
            emailQueryString = new Dictionary<string, object>
            {
                { "companyId", Company.Id },
                { "assessmentUid", "invalidAssessmentUid" },
                { "memberUid", "invalidMemberUid" },
                {"emailType", "Neither" }
            };

            var errorList = new List<string>
            {
                "'Assessment Uid' is not valid",
                "'Member Uid' is not valid",
                "'EmailType' options are Participant or Reviewer"
            };

            response = await client.PostAsync<IList<string>>(RequestUris.AssessmentIndividualEmail()
                .AddQueryParameter(emailQueryString));

            Assert.AreEqual(3, response.Dto.Count, "Total response message doesn't match");
            Assert.That.ListsAreEqual(response.Dto.ToList(), errorList, "Error messages list do not match");
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status code does not match");
        }
    }
}