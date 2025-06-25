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
using AtCommon.Dtos.Users;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using RoleRequest = AtCommon.Dtos.IndividualAssessments.RoleRequest;
using TagRoleRequest = AtCommon.Dtos.IndividualAssessments.TagRoleRequest;

namespace ApiTests.v1.Endpoints.IndividualAssessments
{
    [TestClass]
    [TestCategory("TalentDevelopment")]
    public class EditReviewerRolesTests : BaseV1Test
    {
        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public async Task Assessments_Individual_Edit_Reviewer_Roles_Success()
        {
            var client = await GetAuthenticatedClient();

            //Create a team
            var team = TeamFactory.GetValidPostTeamWithMember("GetValidPostTeamWithMember_");
            var teamCreateResponse = await client.PostAsync<TeamResponse>(RequestUris.Teams(), team);
            teamCreateResponse.EnsureSuccess();
            var teamUid = teamCreateResponse.Dto.Uid;
            var memberResponse = await client.GetAsync<IList<TeamMemberResponse>>(RequestUris.TeamMembers(teamUid));
            memberResponse.EnsureSuccess();
            var memberId = memberResponse.Dto.First().Uid;

            //Get available individual tags
            var rolesQuery = new Dictionary<string, object>
            {
                { "companyId", Company.Id },
                { "roleType", SharedConstants.RoleIndividual }
            };
            var individualRolesResponse = await client.GetAsync<UserRolesResponse>(RequestUris.CompaniesUsersRoles().AddQueryParameter(rolesQuery));
            var individualRoles = individualRolesResponse.Dto.Roles;

            //Add new reviewer
            var reviewer = MemberFactory.GetReviewer();
            var reviewerResponse = await client.PostAsync<ReviewerResponse>(
                RequestUris.AssessmentIndividualReviewer().AddQueryParameter("companyId", Company.Id), reviewer);
            reviewerResponse.EnsureSuccess();

            //Create an individual assessment
            var surveyId = GetIndividualSurveyId();
            var individualAssessment = IndividualAssessmentFactory.GetValidIndividualAssessment(Company.Id, User.CompanyName, surveyId);
            individualAssessment.Members[0].Uid = memberId;
            individualAssessment.Members[0].Reviewers.Add(reviewerResponse.Dto.ToAddIndividualMemberRequest());
            individualAssessment.CompanyId = Company.Id;
            individualAssessment.TeamUid = teamUid;
            individualAssessment.Published = true;

            var initIaResponse = await client.PostAsync<IndividualAssessmentResponse>(RequestUris.IndividualAssessments().AddQueryParameter("companyId", individualAssessment.CompanyId), individualAssessment);
            initIaResponse.EnsureSuccess();
            individualAssessment.BatchId = initIaResponse.Dto.BatchId;
            var createIaResponse1 = await client.PostAsync<IndividualAssessmentResponse>(RequestUris.IndividualAssessments(), individualAssessment);
            createIaResponse1.EnsureSuccess();
            var assessmentUid = createIaResponse1.Dto.AssessmentList[0].AssessmentUid;
            var individualAssessmentResponse = await client.GetAsync<IndividualAssessmentMemberResponse>(RequestUris.AssessmentGetIndividual(assessmentUid));
            
            // Edit reviewers tags
            var reviewerRolesRequest = new ReviewerRolesRequest
            {
                AssessmentUid = assessmentUid,
                ReviewerUid = individualAssessmentResponse.Dto.Members.First().Reviewers.First().Uid,
                RoleTags = new List<RoleRequest>
                {
                    new RoleRequest
                    {
                        Key = "Role",
                        Tags = JsonConvert.DeserializeObject<List<TagRoleRequest>>(JsonConvert.SerializeObject(individualRoles))
                    }
                }
            };
            var editReviewerRolesResponse = await client.PostAsync<ReviewerRolesResponse>(RequestUris.AssessmentIndividualReviewerTags().AddQueryParameter("companyId", Company.Id), reviewerRolesRequest);
            editReviewerRolesResponse.EnsureSuccess();

            var requestRoles = reviewerRolesRequest.RoleTags.FirstOrDefault().CheckForNull().Tags;
            var responseRoles = editReviewerRolesResponse.Dto.RoleTags.FirstOrDefault().CheckForNull().Tags;
            Assert.AreEqual(HttpStatusCode.OK, editReviewerRolesResponse.StatusCode, $"Response status code does not match. Receiving a {editReviewerRolesResponse.StatusCode}");
            Assert.AreEqual(requestRoles.Count, responseRoles.Count, "Total reviewer roles doesn't match");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public async Task Assessments_Individual_Edit_Reviewer_Roles_Role_NotFound()
        {
            var client = await GetAuthenticatedClient();

            //Create a team
            var team = TeamFactory.GetValidPostTeamWithMember("GetValidPostTeamWithMember_");
            var teamCreateResponse = await client.PostAsync<TeamResponse>(RequestUris.Teams(), team);
            teamCreateResponse.EnsureSuccess();
            var teamUid = teamCreateResponse.Dto.Uid;
            var memberResponse = await client.GetAsync<IList<TeamMemberResponse>>(RequestUris.TeamMembers(teamUid));
            memberResponse.EnsureSuccess();
            var memberId = memberResponse.Dto.First().Uid;

            //Add new reviewer
            var reviewer = MemberFactory.GetReviewer();
            var reviewerResponse = await client.PostAsync<ReviewerResponse>(
                RequestUris.AssessmentIndividualReviewer().AddQueryParameter("companyId", Company.Id), reviewer);
            reviewerResponse.EnsureSuccess();

            //Create an individual assessment
            var surveyId = GetIndividualSurveyId();
            var individualAssessment = IndividualAssessmentFactory.GetValidIndividualAssessment(Company.Id, User.CompanyName, surveyId);
            individualAssessment.Members[0].Uid = memberId;
            individualAssessment.Members[0].Reviewers.Add(reviewerResponse.Dto.ToAddIndividualMemberRequest());
            individualAssessment.CompanyId = Company.Id;
            individualAssessment.TeamUid = teamUid;
            individualAssessment.Published = true;

            var response = await client.PostAsync<IndividualAssessmentResponse>(RequestUris.IndividualAssessments().AddQueryParameter("companyId", individualAssessment.CompanyId), individualAssessment);
            response.EnsureSuccess();
            individualAssessment.BatchId = response.Dto.BatchId;
            var createIaResponse = await client.PostAsync<IndividualAssessmentResponse>(RequestUris.IndividualAssessments(), individualAssessment);
            createIaResponse.EnsureSuccess();
            var assessmentUid = createIaResponse.Dto.AssessmentList[0].AssessmentUid;
            var individualAssessmentResponse = await client.GetAsync<IndividualAssessmentMemberResponse>(RequestUris.AssessmentGetIndividual(assessmentUid));

            // Edit reviewers tags
            var reviewerRolesRequest = new ReviewerRolesRequest
            {
                AssessmentUid = assessmentUid,
                ReviewerUid = individualAssessmentResponse.Dto.Members.First().Reviewers.First().Uid,
                RoleTags = new List<RoleRequest>
                {
                    new RoleRequest
                    {
                        Key = "Role",
                        Tags = new List<TagRoleRequest>
                        {
                            new TagRoleRequest
                            {
                                Id = 123,
                                Name = "Test"
                            }
                        }
                    }
                }
            };
            var editReviewerRolesResponse = await client.PostAsync<ReviewerRolesResponse>(
                RequestUris.AssessmentIndividualReviewerTags().AddQueryParameter("companyId", Company.Id),
                reviewerRolesRequest);
            Assert.AreEqual(HttpStatusCode.NotFound, editReviewerRolesResponse.StatusCode, $"Response status code does not match. Receiving a {editReviewerRolesResponse.StatusCode}");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public async Task Assessments_Individual_Edit_Reviewer_Roles_Unauthorized()
        {
            var client = GetUnauthenticatedClient();

            // Edit reviewers tags
            var reviewerRolesRequest = new ReviewerRolesRequest
            {
                AssessmentUid = Guid.NewGuid(),
                ReviewerUid = Guid.NewGuid(),
                RoleTags = new List<RoleRequest>
                {
                    new RoleRequest
                    {
                        Key = "Role",
                        Tags = new List<TagRoleRequest>
                        {
                            new TagRoleRequest
                            {
                                Id = 123,
                                Name = "Test"
                            }
                        }
                    }
                }
            };
            var editReviewerRolesResponse = await client.PostAsync<ReviewerRolesResponse>(
                RequestUris.AssessmentIndividualReviewerTags().AddQueryParameter("companyId", Company.Id),
                reviewerRolesRequest);
            Assert.AreEqual(HttpStatusCode.Unauthorized, editReviewerRolesResponse.StatusCode, "Status codes do not match");
        }
    }
}
