using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AtCommon.Api;
using AtCommon.Api.Enums;
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
    public class EditIndividualAssessmentAddReviewerTests : BaseV1Test
    {
        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public async Task Assessments_Individual_Post_AddReviewer()
        {
            var client = await GetAuthenticatedClient();
            var setup = new SetupTeardownApi(TestEnvironment);
            var surveyId = setup.GetSurveyType(Company.Id, Convert.ToInt32(AssessmentType.Individual), SharedConstants.IndividualAssessmentType).Id;

            //Create a team
            var team = TeamFactory.GetValidPostTeamWithMember("GetValidPostTeamWithMember_");
            var teamCreateResponse = await client.PostAsync<TeamResponse>(RequestUris.Teams(), team);
            teamCreateResponse.EnsureSuccess();

            //Add new reviewer
            var reviewer = MemberFactory.GetReviewer();
            var reviewerResponse = await client.PostAsync<ReviewerResponse>(
                RequestUris.AssessmentIndividualReviewer().AddQueryParameter("companyId", Company.Id), reviewer);
            reviewerResponse.EnsureSuccess();

            //get survey id
            var individualSurveyId = GetIndividualSurveyId();
            
            //get up individual assessment members
            var individualAssessment = IndividualAssessmentFactory.GetValidIndividualAssessment(Company.Id, User.CompanyName, individualSurveyId);
            individualAssessment.Members = new List<IndividualAssessmentMemberRequest>
            {
                teamCreateResponse.Dto.Members.FirstOrDefault().CheckForNull("No team members found in the response").ToAddIndividualMemberRequest()
            };
            individualAssessment.CompanyId = Company.Id;
            individualAssessment.TeamUid = teamCreateResponse.Dto.Uid;
            individualAssessment.Published = true;
            individualAssessment.SurveyTypeId = surveyId;

            var createIaResponse = await client.PostAsync<IndividualAssessmentResponse>(RequestUris.IndividualAssessments(), individualAssessment);
            createIaResponse.EnsureSuccess();
            individualAssessment.BatchId = createIaResponse.Dto.BatchId;
            var createIaResponse2 = await client.PostAsync<IndividualAssessmentResponse>(RequestUris.IndividualAssessments(), individualAssessment);
            createIaResponse2.EnsureSuccess();
            await client.PostAsync<string>(RequestUris.IndividualAssessmentsEmailsAndClaims(), individualAssessment);

            //Edit to add reviewer to assessment
            var editIndividualAssessmentRequest = new EditIndividualAssessmentRequest
            {
                Members = new List<IndividualAssessmentMemberRequest>
                {
                    new IndividualAssessmentMemberRequest
                    {
                        Uid = teamCreateResponse.Dto.Members[0].Uid,
                        FirstName = teamCreateResponse.Dto.Members[0].FirstName,
                        LastName = teamCreateResponse.Dto.Members[0].LastName,
                        Email = teamCreateResponse.Dto.Members[0].Email,
                        Reviewers = new List<IndividualAssessmentMemberRequest>
                        {
                            reviewerResponse.Dto.ToAddIndividualMemberRequest()
                        }
                    }
                }
            };

            var queryString = new Dictionary<string, object>
            {
                { "companyId", Company.Id },
                { "assessmentUid", createIaResponse2.Dto.AssessmentList.FirstOrDefault().CheckForNull("AssessmentList").AssessmentUid }
            };
            var response = await client.PostAsync<IndividualAssessmentMemberResponse>(RequestUris.AssessmentIndividualEdit().AddQueryParameter(queryString), editIndividualAssessmentRequest);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, $"Response status code does not match. Received a {response.StatusCode}.");
            Assert.AreEqual(editIndividualAssessmentRequest.Members.Count, response.Dto.Members.Count, "Total member doesn't match");
            Assert.AreEqual(editIndividualAssessmentRequest.Members.First().Reviewers.Count, response.Dto.Members.FirstOrDefault()?.Reviewers.Count, "Total reviewer doesn't match");
            Assert.AreEqual(reviewer.FirstName, response.Dto.Members[0].Reviewers[0].FirstName, "Reviewer first name doesn't match");
            Assert.AreEqual(reviewer.LastName, response.Dto.Members[0].Reviewers[0].LastName, "Reviewer last name doesn't match");
            Assert.AreEqual(reviewer.Email, response.Dto.Members[0].Reviewers[0].Email, "Reviewer email doesn't match");
            var emailSearch = new EmailSearch
            {
                Subject = SharedConstants.IaEmailReviewerSubject,
                To = reviewer.Email,
                Labels = new List<string> { "inbox" },
                Timeout = new TimeSpan(0, 2, 0)
            };
            Assert.IsTrue(GmailUtil.DoesMemberEmailExist(emailSearch),
                $"Could not find email with subject <{emailSearch.Subject}> sent to {emailSearch.To}");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public async Task Assessments_Individual_Post_AddReviewer_Unauthorized()
        {
            var client = GetUnauthenticatedClient();

            var queryString = new Dictionary<string, object>
            {
                { "companyId", Company.Id },
                { "assessmentUid", Guid.NewGuid() }
            };

            var response = await client.PostAsync<IndividualAssessmentMemberResponse>(
                RequestUris.AssessmentIndividualEdit().AddQueryParameter(queryString));

            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status code does not match");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public async Task Assessments_Individual_Post_AddReviewer_Forbidden()
        {
            var client = await GetAuthenticatedClient();

            //Add a team with member
            var team = TeamFactory.GetValidPostTeamWithMember("GetValidPostTeamWithMember_");
            var teamCreateResponse = await client.PostAsync<TeamResponse>(RequestUris.Teams(), team);
            teamCreateResponse.EnsureSuccess();

            //Add new reviewer
            var reviewer = MemberFactory.GetReviewer();
            var reviewerResponse = await client.PostAsync<ReviewerResponse>(
                RequestUris.AssessmentIndividualReviewer().AddQueryParameter("companyId", Company.Id), reviewer);
            reviewerResponse.EnsureSuccess();

            var queryString = new Dictionary<string, object>
            {
                { "companyId", "2" },
                { "assessmentUid", Guid.NewGuid() }
            };

            var editIndividualAssessmentRequest = new EditIndividualAssessmentRequest()
            {
                Members = new List<IndividualAssessmentMemberRequest>
                {
                    new IndividualAssessmentMemberRequest
                    {
                        Uid = teamCreateResponse.Dto.Members[0].Uid,
                        FirstName = teamCreateResponse.Dto.Members[0].FirstName,
                        LastName = teamCreateResponse.Dto.Members[0].LastName,
                        Email = teamCreateResponse.Dto.Members[0].Email,
                        Reviewers = new List<IndividualAssessmentMemberRequest>
                        {
                            reviewerResponse.Dto.ToAddIndividualMemberRequest()
                        }
                    }
                }
            };

            var response = await client.PostAsync<IndividualAssessmentMemberResponse>(
                RequestUris.AssessmentIndividualEdit().AddQueryParameter(queryString), editIndividualAssessmentRequest);

            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status code does not match");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public async Task Assessments_Individual_Post_AddReviewer_InvalidInput()
        {
            var client = await GetAuthenticatedClient();

            //Add a team with member
            var team = TeamFactory.GetValidPostTeamWithMember("GetValidPostTeamWithMember_");
            var teamCreateResponse = await client.PostAsync<TeamResponse>(RequestUris.Teams(), team);
            teamCreateResponse.EnsureSuccess();

            //Add new reviewer
            var reviewer = MemberFactory.GetReviewer();
            var reviewerResponse = await client.PostAsync<ReviewerResponse>(
                RequestUris.AssessmentIndividualReviewer().AddQueryParameter("companyId", Company.Id), reviewer);
            reviewerResponse.EnsureSuccess();

            var queryString = new Dictionary<string, object>
            {
                { "companyId", Company.Id },
                { "assessmentUid", "invalid" }
            };

            var editIndividualAssessmentRequest = new EditIndividualAssessmentRequest()
            {
                Members = new List<IndividualAssessmentMemberRequest>
                {
                    new IndividualAssessmentMemberRequest
                    {
                        Uid = teamCreateResponse.Dto.Members[0].Uid,
                        FirstName = teamCreateResponse.Dto.Members[0].FirstName,
                        LastName = teamCreateResponse.Dto.Members[0].LastName,
                        Email = teamCreateResponse.Dto.Members[0].Email,
                        Reviewers = new List<IndividualAssessmentMemberRequest>
                        {
                            reviewerResponse.Dto.ToAddIndividualMemberRequest()
                        }
                    }
                }
            };

            var response = await client.PostAsync<IList<string>>(
                RequestUris.AssessmentIndividualEdit().AddQueryParameter(queryString), editIndividualAssessmentRequest);

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status code does not match");
            Assert.AreEqual("'Assessment Uid' is not valid", response.Dto.FirstOrDefault(), "Response message for invalid assessment uid  doesn't match");
        }
    }
}