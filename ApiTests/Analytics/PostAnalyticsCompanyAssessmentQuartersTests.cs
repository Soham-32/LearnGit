using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ApiTests.v1.Endpoints;
using AtCommon.Api;
using AtCommon.Dtos.Analytics;
using AtCommon.Dtos.Analytics.Custom;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiTests.Analytics
{
    //[TestClass]
    [TestCategory("Insights")]
    public class PostAnalyticsCompanyAssessmentQuartersTests : BaseV1Test
    {
        // 200-201
        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_CompanyAssessmentQuarters_Post_TeamHealth_OK()
        {
            // given
            var client = await GetInsightsAuthenticatedClient();
            var request = new GetCompanyAssessmentQuartersRequest
            {
                RadarType = 1
            };

            // when
            var response = await client.PostAsync<List<CompanyAssessmentQuartersResponse>>(RequestUris.AnalyticsCompanyAssessmentQuarters(Company.InsightsId), request);

            // then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status code doesn't match.");
            var validQuarters = AnalyticsQuarter.GetValidQuarters();
            Assert.AreEqual(validQuarters.Count, response.Dto.Count, "Count of quarters does not match");
            foreach (var validQuarter in validQuarters)
            {
                var actual = response.Dto.FirstOrDefault(q => q.QuarterName == validQuarter.QuarterName)
                    .CheckForNull($"{validQuarter.QuarterName} was not found in the response.");
                Assert.AreEqual(validQuarter.LastDayOfQuarter, actual.LastDayOfQuarter, 
                    "LastDayOfQuarter does not match.");
                Assert.AreEqual(validQuarter.IsCurrentQuarter, actual.IsCurrentQuarter, 
                    "IsCurrentQuarter does not match.");
            }
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_CompanyAssessmentQuarters_Post_All_OK()
        {
            // given
            var client = await GetInsightsAuthenticatedClient();
            var request = new GetCompanyAssessmentQuartersRequest
            {
                RadarType = 0
            };

            // when
            var response = await client.PostAsync<List<CompanyAssessmentQuartersResponse>>(RequestUris.AnalyticsCompanyAssessmentQuarters(Company.InsightsId), request);

            // then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status code doesn't match.");
            var validQuarters = AnalyticsQuarter.GetValidQuarters();
            Assert.AreEqual(validQuarters.Count, response.Dto.Count, "Count of quarters does not match");
            foreach (var validQuarter in validQuarters)
            {
                var actual = response.Dto.FirstOrDefault(q => q.QuarterName == validQuarter.QuarterName)
                    .CheckForNull($"{validQuarter.QuarterName} was not found in the response.");
                Assert.AreEqual(validQuarter.LastDayOfQuarter, actual.LastDayOfQuarter, 
                    "LastDayOfQuarter does not match.");
                Assert.AreEqual(validQuarter.IsCurrentQuarter, actual.IsCurrentQuarter, 
                    "IsCurrentQuarter does not match.");
            }
        }

        // 400
        [TestMethod]
        [TestCategory("SiteAdmin")]
        public async Task Analytics_CompanyAssessmentQuarters_Post_CompanyId_BadRequest()
        {
            var client = await GetInsightsAuthenticatedClient();
            var request = new GetCompanyAssessmentQuartersRequest
            {
                RadarType = 1
            };

            // when
            var response = await client.PostAsync<List<string>>(RequestUris.AnalyticsCompanyAssessmentQuarters(0), request);

            // then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status code doesn't match.");
            Assert.AreEqual("Request must include a valid integer value for company id.", 
                response.Dto.FirstOrDefault(), "Error message does not match");
        }

        // 401
        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_CompanyAssessmentQuarters_Post_Unauthorized()
        {
            var client = GetUnauthenticatedClient();
            var request = new GetCompanyAssessmentQuartersRequest
            {
                RadarType = 1
            };

            // when
            var response = await client.PostAsync(
                RequestUris.AnalyticsCompanyAssessmentQuarters(SharedConstants.FakeCompanyId), request.ToStringContent());

            // then
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status code doesn't match.");
        }

        // 403
        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_CompanyAssessmentQuarters_Post_Forbidden()
        {
            var client = await GetInsightsAuthenticatedClient();
            var request = new GetCompanyAssessmentQuartersRequest
            {
                RadarType = 1
            };

            // when
            var response = await client.PostAsync(
                RequestUris.AnalyticsCompanyAssessmentQuarters(SharedConstants.FakeCompanyId), request.ToStringContent());

            // then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status code doesn't match.");
        }
    }
}