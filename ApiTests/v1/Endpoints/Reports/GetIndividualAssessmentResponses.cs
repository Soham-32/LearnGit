using AtCommon.Api;
using AtCommon.Dtos.Reports;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ApiTests.v1.Endpoints.Reports
{
    [TestClass]
    [TestCategory("Reports")]
    public class GetIndividualAssessmentResponses : BaseV1Test
    {
        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public async Task Reports_Get_IndividualAssessmentResponses_Success()
        {
            var client = await GetAuthenticatedClient();
            var individualAssessmentResponse = await client.GetAsync<IList<IndividualAssessmentGrowthPlanItemsSummaryResponse>>(RequestUris.ReportsGrowthPlanItems());
            individualAssessmentResponse.EnsureSuccess();

            var companyId = individualAssessmentResponse.Dto.First().CompanyId;
            var originalSurveyId = individualAssessmentResponse.Dto.First().SurveyOriginalVersion;
            var endDate = individualAssessmentResponse.Dto.First().AssessmentEndDate;

            var queryString = new Dictionary<string, object>
            {
                {"companyId", companyId},
                {"originalSurveyId", originalSurveyId},
                {"endDate", endDate}
            };

            var response = await client.GetAsync(
                RequestUris.ReportsIndividualAssessmentResponses().AddQueryParameter(queryString));

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Response Status Code does not match.");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public async Task Reports_Get_IndividualAssessmentResponses_InvalidEndDate()
        {
            var client = await GetAuthenticatedClient();
            var individualAssessmentResponse = await client.GetAsync<IList<IndividualAssessmentGrowthPlanItemsSummaryResponse>>(RequestUris.ReportsGrowthPlanItems());
            individualAssessmentResponse.EnsureSuccess();

            var companyId = individualAssessmentResponse.Dto.First().CompanyId;
            var originalSurveyId = individualAssessmentResponse.Dto.First().SurveyOriginalVersion;
            const string endDate = "";

            var queryString = new Dictionary<string, object>
            {
                {"companyId", companyId},
                {"originalSurveyId", originalSurveyId},
                {"endDate", endDate}
            };

            var response = await client.GetAsync(
                RequestUris.ReportsIndividualAssessmentResponses().AddQueryParameter(queryString));

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode,
                "Response Status Code does not match.");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public async Task Reports_Get_IndividualAssessmentResponses_InvalidOriginalSurveyId()
        {
            var client = await GetAuthenticatedClient();
            var individualAssessmentResponse = await client.GetAsync<IList<IndividualAssessmentGrowthPlanItemsSummaryResponse>>(RequestUris.ReportsGrowthPlanItems());
            individualAssessmentResponse.EnsureSuccess();

            var companyId = individualAssessmentResponse.Dto.First().CompanyId;
            const string originalSurveyId = "";
            var endDate = individualAssessmentResponse.Dto.First().AssessmentEndDate;

            var queryString = new Dictionary<string, object>
            {
                {"companyId", companyId},
                {"originalSurveyId", originalSurveyId},
                {"endDate", endDate}
            };

            var response = await client.GetAsync(
                RequestUris.ReportsIndividualAssessmentResponses().AddQueryParameter(queryString));

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode,
                "Response Status Code does not match.");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public async Task Reports_Get_IndividualAssessmentResponses_InvalidCompanyId()
        {
            var client = await GetAuthenticatedClient();
            var individualAssessmentResponse = await client.GetAsync<IList<IndividualAssessmentGrowthPlanItemsSummaryResponse>>(RequestUris.ReportsGrowthPlanItems());
            individualAssessmentResponse.EnsureSuccess();

            const string companyId = "";
            var originalSurveyId = individualAssessmentResponse.Dto.First().SurveyOriginalVersion;
            var endDate = individualAssessmentResponse.Dto.First().AssessmentEndDate;

            var queryString = new Dictionary<string, object>
            {
                {"companyId", companyId},
                {"originalSurveyId", originalSurveyId},
                {"endDate", endDate}
            };

            var response = await client.GetAsync(
                RequestUris.ReportsIndividualAssessmentResponses().AddQueryParameter(queryString));

            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode,
                "Response Status Code does not match.");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public async Task Reports_Get_IndividualAssessmentResponses_MissingToken()
        {
            var client = GetUnauthenticatedClient();

            const string companyId = "79";
            const string originalSurveyId = "98";
            var endDate = DateTime.Now.ToString("MM-dd-yyyy");

            var queryString = new Dictionary<string, object>
            {
                {"companyId", companyId},
                {"originalSurveyId", originalSurveyId},
                {"endDate", endDate}
            };

            var response = await client.GetAsync(RequestUris.ReportsIndividualAssessmentResponses().AddQueryParameter(queryString));

            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Response Status Code does not match.");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public async Task Reports_Get_IndividualAssessmentResponses_EmptyList()
        {
            var client = await GetAuthenticatedClient();
            var individualAssessmentResponse = await client.GetAsync<IList<IndividualAssessmentGrowthPlanItemsSummaryResponse>>(RequestUris.ReportsGrowthPlanItems());
            individualAssessmentResponse.EnsureSuccess();

            var companyId = individualAssessmentResponse.Dto.First().CompanyId;
            var originalSurveyId = individualAssessmentResponse.Dto.First().SurveyOriginalVersion;
            var endDate = DateTime.Now.ToString("MM-dd-yyyy");

            var queryString = new Dictionary<string, object>
            {
                {"companyId", companyId},
                {"originalSurveyId", originalSurveyId},
                {"endDate", endDate}
            };

            var response = await client.GetAsync(RequestUris.ReportsIndividualAssessmentResponses().AddQueryParameter(queryString));

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Response Status Code does not match.");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Reports_Get_IndividualAssessmentResponses_UnauthorizedUser()
        {
            var client = await GetAuthenticatedClient();

            const string companyId = "79";
            const string originalSurveyId = "100";
            var endDate = DateTime.Now.ToString("MM-dd-yyyy");

            var queryString = new Dictionary<string, object>
            {
                {"companyId", companyId},
                {"originalSurveyId", originalSurveyId},
                {"endDate", endDate}
            };

            var response = await client.GetAsync(RequestUris.ReportsIndividualAssessmentResponses().AddQueryParameter(queryString));

            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Response Status Code does not match.");
        }
    }
}
