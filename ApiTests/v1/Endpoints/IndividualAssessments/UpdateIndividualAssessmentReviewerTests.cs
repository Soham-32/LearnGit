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
    public class UpdateIndividualAssessmentReviewerTests : BaseV1Test
    {
        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Assessments_Individual_Put_Individual_Reviewer_Success()
        {
            var client = await GetAuthenticatedClient();

            //create reviewer
            var reviewer = MemberFactory.GetValidReviewer();
            var reviewerResponse = await client.PostAsync<ReviewerResponse>(RequestUris.AssessmentIndividualReviewer(), reviewer);
            reviewerResponse.EnsureSuccess();

            //update reviewer
            var updatedReviewer = MemberFactory.UpdateValidReviewer(reviewerResponse.Dto.Id);
            updatedReviewer.Email = reviewerResponse.Dto.Email;
            var updatedReviewerResponse =
                await client.PutAsync<ReviewerResponse>(
                    RequestUris.AssessmentIndividualReviewer().AddQueryParameter("companyId", Company.Id), updatedReviewer);

            Assert.AreEqual(HttpStatusCode.OK, updatedReviewerResponse.StatusCode, "Status codes do not match");
            Assert.AreNotEqual(reviewerResponse.Dto.FirstName, updatedReviewerResponse.Dto.FirstName, "First name did not change");
            Assert.AreNotEqual(reviewerResponse.Dto.LastName, updatedReviewerResponse.Dto.LastName, "Last name did not change");
            Assert.AreEqual(reviewerResponse.Dto.Email, updatedReviewerResponse.Dto.Email, "Email address is not the same");
            Assert.AreEqual(reviewerResponse.Dto.Id, updatedReviewerResponse.Dto.Id, "Id is not the same");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Assessments_Individual_Put_Individual_Reviewer_EmptyEmail_BadRequest()
        {
            var client = await GetAuthenticatedClient();

            //create reviewer
            var reviewer = MemberFactory.GetValidReviewer();
            var reviewerResponse = await client.PostAsync<ReviewerResponse>(RequestUris.AssessmentIndividualReviewer(), reviewer);
            reviewerResponse.EnsureSuccess();

            //update reviewer
            var updatedReviewer = MemberFactory.UpdateValidReviewer(reviewerResponse.Dto.Id);
            updatedReviewer.Email = "";
            var updatedReviewerResponse = await client.PutAsync(
                RequestUris.AssessmentIndividualReviewer().AddQueryParameter("companyId", Company.Id), updatedReviewer.ToStringContent());

            Assert.AreEqual(HttpStatusCode.BadRequest, updatedReviewerResponse.StatusCode, "Status codes do not match");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Assessments_Individual_Put_Individual_Reviewer_EmptyFirstName_BadRequest()
        {
            var client = await GetAuthenticatedClient();

            //create reviewer
            var reviewer = MemberFactory.GetValidReviewer();
            var reviewerResponse = await client.PostAsync<ReviewerResponse>(RequestUris.AssessmentIndividualReviewer(), reviewer);
            reviewerResponse.EnsureSuccess();

            //update reviewer
            var updatedReviewer = MemberFactory.UpdateValidReviewer(reviewerResponse.Dto.Id);
            updatedReviewer.FirstName = "";
            var updatedReviewerResponse = await client.PutAsync(
                RequestUris.AssessmentIndividualReviewer().AddQueryParameter("companyId", Company.Id), updatedReviewer.ToStringContent());

            Assert.AreEqual(HttpStatusCode.BadRequest, updatedReviewerResponse.StatusCode, "Status codes do not match");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Assessments_Individual_Put_Individual_Reviewer_EmptyLastName_BadRequest()
        {
            var client = await GetAuthenticatedClient();

            //create reviewer
            var reviewer = MemberFactory.GetValidReviewer();
            var reviewerResponse = await client.PostAsync<ReviewerResponse>(RequestUris.AssessmentIndividualReviewer(), reviewer);
            reviewerResponse.EnsureSuccess();

            //update reviewer
            var updatedReviewer = MemberFactory.UpdateValidReviewer(reviewerResponse.Dto.Id);
            updatedReviewer.LastName = "";
            var updatedReviewerResponse = await client.PutAsync(
                RequestUris.AssessmentIndividualReviewer().AddQueryParameter("companyId", Company.Id), updatedReviewer.ToStringContent());

            Assert.AreEqual(HttpStatusCode.BadRequest, updatedReviewerResponse.StatusCode, "Status codes do not match");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Assessments_Individual_Put_Individual_Reviewer_EmptyId_BadRequest()
        {
            var client = await GetAuthenticatedClient();

            //create reviewer
            var reviewer = MemberFactory.GetValidReviewer();
            var reviewerResponse = await client.PostAsync<ReviewerResponse>(RequestUris.AssessmentIndividualReviewer(), reviewer);
            reviewerResponse.EnsureSuccess();

            //update reviewer
            var updatedReviewer = MemberFactory.UpdateValidReviewer(reviewerResponse.Dto.Id);
            updatedReviewer.Id = 0;
            var updatedReviewerResponse =
                await client.PutAsync(
                    RequestUris.AssessmentIndividualReviewer().AddQueryParameter("companyId", Company.Id), updatedReviewer.ToStringContent());

            Assert.AreEqual(HttpStatusCode.BadRequest, updatedReviewerResponse.StatusCode, "Status codes do not match");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Assessments_Individual_Put_Individual_Reviewer_Unauthorized()
        {
            var client = await GetAuthenticatedClient();

            //create reviewer
            var reviewer = MemberFactory.GetValidReviewer();
            var reviewerResponse = await client.PostAsync<ReviewerResponse>(RequestUris.AssessmentIndividualReviewer(), reviewer);
            reviewerResponse.EnsureSuccess();

            //update reviewer
            client = GetUnauthenticatedClient();
            var updatedReviewer = MemberFactory.UpdateValidReviewer(reviewerResponse.Dto.Id);
            updatedReviewer.Email = reviewerResponse.Dto.Email;
            var updatedReviewerResponse = await client.PutAsync<ReviewerResponse>(
                    RequestUris.AssessmentIndividualReviewer().AddQueryParameter("companyId", Company.Id), updatedReviewer);

            Assert.AreEqual(HttpStatusCode.Unauthorized, updatedReviewerResponse.StatusCode, "Status codes do not match");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Assessments_Individual_Put_Individual_Reviewer_Forbidden()
        {
            var client = await GetAuthenticatedClient();

            //create reviewer
            var reviewer = MemberFactory.GetValidReviewer();
            var reviewerResponse = await client.PostAsync<ReviewerResponse>(RequestUris.AssessmentIndividualReviewer(), reviewer);
            reviewerResponse.EnsureSuccess();

            //update reviewer
            var updatedReviewer = MemberFactory.UpdateValidReviewer(reviewerResponse.Dto.Id);
            updatedReviewer.Email = reviewerResponse.Dto.Email;
            var updatedReviewerResponse = await client.PutAsync<ReviewerResponse>(
                    RequestUris.AssessmentIndividualReviewer().AddQueryParameter("companyId", SharedConstants.FakeCompanyId), updatedReviewer);

            Assert.AreEqual(HttpStatusCode.Forbidden, updatedReviewerResponse.StatusCode, "Status codes do not match");
        }

    }
}