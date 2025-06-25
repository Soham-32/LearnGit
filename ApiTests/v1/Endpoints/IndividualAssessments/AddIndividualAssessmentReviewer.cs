using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AtCommon.Api;
using AtCommon.Dtos.Assessments;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiTests.v1.Endpoints.IndividualAssessments
{
    [TestClass]
    [TestCategory("TalentDevelopment")]
    public class AddIndividualAssessmentReviewer : BaseV1Test
    {
        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Assessments_Individual_Post_Reviewer_Created()
        {
            var client = await GetAuthenticatedClient();
            var reviewer = MemberFactory.GetValidReviewer();
            
            var response = await client.PostAsync<ReviewerResponse>(
                RequestUris.AssessmentIndividualReviewer().AddQueryParameter("companyId", Company.Id), reviewer);

            response.EnsureSuccess();
            Assert.IsTrue(!string.IsNullOrEmpty(response.Dto.Id.ToString()), "Id is null or empty");
            Assert.AreEqual(reviewer.FirstName, response.Dto.FirstName, "FirstName does not match");
            Assert.IsTrue(!string.IsNullOrEmpty(response.Dto.FirstName), "Id is null or empty");
            Assert.AreEqual(reviewer.LastName, response.Dto.LastName, "LastName does not match");
            Assert.IsTrue(!string.IsNullOrEmpty(response.Dto.LastName), "Id is null or empty");
            Assert.AreEqual(reviewer.Email, response.Dto.Email, "Email does not match");
            Assert.IsTrue(!string.IsNullOrEmpty(response.Dto.Email), "Id is null or empty");
            Assert.IsFalse(response.Dto.MemberExists, "Reviewer already existed");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Assessments_Individual_Post_Reviewer_CreatedWithRole()
        {
            var client = await GetAuthenticatedClient();
            var reviewer = new CreateReviewerRequest
            {
                FirstName = $"Reviewer{Guid.NewGuid()}",
                LastName = SharedConstants.TeamMemberLastName,
                Email = $"ah_automation+R{Guid.NewGuid():N}@agiletransformation.com",
                RoleTags = new List<RoleResponse>
                {
                    new RoleResponse
                    {
                        Tags = new List<TagRoleResponse>
                        {
                            new TagRoleResponse
                            {
                                Id = 1,
                                Name = "Customer"
                            },
                            new TagRoleResponse
                            {
                                Id = 1,
                                Name = "Reviewer"
                            }
                        }
                    }
                }
            };

            var response = await client.PostAsync<ReviewerResponse>(
                RequestUris.AssessmentIndividualReviewer().AddQueryParameter("companyId", Company.Id), reviewer);

            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode, "Status codes do not match");
            Assert.IsTrue(!string.IsNullOrEmpty(response.Dto.Id.ToString()), "Id is null or empty");
            Assert.AreEqual(reviewer.FirstName, response.Dto.FirstName, "FirstName does not match");
            Assert.IsTrue(!string.IsNullOrEmpty(response.Dto.FirstName), "Id is null or empty");
            Assert.AreEqual(reviewer.LastName, response.Dto.LastName, "LastName does not match");
            Assert.IsTrue(!string.IsNullOrEmpty(response.Dto.LastName), "Id is null or empty");
            Assert.AreEqual(reviewer.Email, response.Dto.Email, "Email does not match");
            Assert.IsTrue(!string.IsNullOrEmpty(response.Dto.Email), "Id is null or empty");
            Assert.IsFalse(response.Dto.MemberExists, "Reviewer already existed");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Assessments_Individual_Post_Reviewer_Unauthorized()
        {
            var client = GetUnauthenticatedClient();
            var reviewer = MemberFactory.GetValidReviewer();
            
            var response = await client.PostAsync<ReviewerResponse>(
                RequestUris.AssessmentIndividualReviewer().AddQueryParameter("companyId", Company.Id), reviewer);

            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status codes do not match");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Assessments_Individual_Post_Reviewer_MissingEmailField_BadRequest()
        {
            var client = await GetAuthenticatedClient();
            var reviewer = MemberFactory.GetValidReviewer();
            reviewer.Email = "";

            var response = await client.PostAsync<IList<string>>(
                RequestUris.AssessmentIndividualReviewer().AddQueryParameter("companyId", Company.Id), reviewer);

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status codes do not match");
            Assert.IsTrue(response.Dto.Contains("'Email' should not be empty."));
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Assessments_Individual_Post_Reviewer_MissingAllFields_BadRequest()
        {
            var client = await GetAuthenticatedClient();
            var reviewer = MemberFactory.GetValidReviewer();
            reviewer.FirstName = "";
            reviewer.LastName = "";
            reviewer.Email = "";

            var response = await client.PostAsync<IList<string>>(
                RequestUris.AssessmentIndividualReviewer().AddQueryParameter("companyId", Company.Id), reviewer);

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, $"Status codes do not match");
            Assert.IsTrue(response.Dto.Contains("'FirstName' should not be empty"));
            Assert.IsTrue(response.Dto.Contains("'LastName' should not be empty"));
            Assert.IsTrue(response.Dto.Contains("'Email' should not be empty."));
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Assessments_Individual_Post_Reviewer_Null_BadRequest()
        {
            var client = await GetAuthenticatedClient();
            string reviewer = null;

            var response = await client.PostAsync(
                RequestUris.AssessmentIndividualReviewer().AddQueryParameter("companyId", Company.Id), 
                reviewer.ToStringContent());

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, $"Status codes do not match");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Assessments_Individual_Post_Reviewer_UpdateSameReviewerInfo_Success()
        {
            var client = await GetAuthenticatedClient();
            var reviewer = MemberFactory.GetValidReviewer();

            var reviewerQueryString = new Dictionary<string, object> {{ "companyId", Company.Id }};

            var firstReviewerResponse = await client.PostAsync<ReviewerResponse>(
                RequestUris.AssessmentIndividualReviewer().AddQueryParameter(reviewerQueryString), reviewer);
            firstReviewerResponse.EnsureSuccess();

            var secondReviewerResponse = await client.PostAsync<ReviewerResponse>(
                RequestUris.AssessmentIndividualReviewer().AddQueryParameter(reviewerQueryString), reviewer);

            Assert.AreEqual(HttpStatusCode.OK, secondReviewerResponse.StatusCode, "Status codes do not match");
            Assert.IsTrue(!string.IsNullOrEmpty(secondReviewerResponse.Dto.Id.ToString()), "Id is null or empty");
            Assert.AreEqual(reviewer.FirstName, secondReviewerResponse.Dto.FirstName, "FirstName does not match");
            Assert.IsTrue(!string.IsNullOrEmpty(secondReviewerResponse.Dto.FirstName), "Id is null or empty");
            Assert.AreEqual(reviewer.LastName, secondReviewerResponse.Dto.LastName, "LastName does not match");
            Assert.IsTrue(!string.IsNullOrEmpty(secondReviewerResponse.Dto.LastName), "Id is null or empty");
            Assert.AreEqual(reviewer.Email, secondReviewerResponse.Dto.Email, "Email does not match");
            Assert.IsTrue(!string.IsNullOrEmpty(secondReviewerResponse.Dto.Email), "Id is null or empty");
            Assert.IsTrue(secondReviewerResponse.Dto.MemberExists, "Reviewer did not previously exist");
        }
    }
}
