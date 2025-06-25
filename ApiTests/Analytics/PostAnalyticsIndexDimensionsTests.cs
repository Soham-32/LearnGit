using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ApiTests.v1.Endpoints;
using AtCommon.Api;
using AtCommon.Dtos.Analytics;
using AtCommon.Dtos.Companies;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiTests.Analytics
{
    //[TestClass]
    [TestCategory("Insights")]
    public class PostAnalyticsIndexDimensionsTests : BaseV1Test
    {
        private static bool _classInitFailed;
        private static TeamHierarchyResponse _multiTeam;
        private static TeamHierarchyResponse _enterpriseTeam;
        private const decimal BenchmarkValue = 55;

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            try
            {
                var setup = new SetupTeardownApi(TestEnvironment);
                var user = User.IsSiteAdmin() || User.IsPartnerAdmin() ? User : InsightsUser;
                var companyHierarchy = setup.GetCompanyHierarchy(Company.InsightsId, user);
                _enterpriseTeam = companyHierarchy.GetTeamByName(SharedConstants.InsightsEnterpriseTeam1);
                _multiTeam = companyHierarchy.GetTeamByName(SharedConstants.InsightsMultiTeam1);
            }
            catch (System.Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }

        // 200-201
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_IndexDimensions_Post_Company_Benchmark_OK()
        {
            var request = InsightsFactory.GetIndexDimensionsRequest(1);
            var expectedResponse = InsightsFactory.GetIndexDimensionsResponse(55, BenchmarkValue);

            await IndexDimensionsValidator(request, expectedResponse);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_IndexDimensions_Post_Company_NoBenchmark_OK()
        {
            var request = InsightsFactory.GetIndexDimensionsRequest(0);
            var expectedResponse = InsightsFactory.GetIndexDimensionsResponse(55);

            await IndexDimensionsValidator(request, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_IndexDimensions_Post_Enterprise_Benchmark_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetIndexDimensionsRequest(1, _enterpriseTeam.TeamId);
            var expectedResponse = InsightsFactory.GetIndexDimensionsResponse(55, BenchmarkValue);

            await IndexDimensionsValidator(request, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_IndexDimensions_Post_Enterprise_NoBenchmark_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetIndexDimensionsRequest(0, _enterpriseTeam.TeamId);
            var expectedResponse = InsightsFactory.GetIndexDimensionsResponse(55);

            await IndexDimensionsValidator(request, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_IndexDimensions_Post_MultiTeam_Benchmark_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetIndexDimensionsRequest(1, _multiTeam.TeamId);
            var expectedResponse = InsightsFactory.GetIndexDimensionsResponse(25, BenchmarkValue);

            await IndexDimensionsValidator(request, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_IndexDimensions_Post_MultiTeam_NoBenchmark_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetIndexDimensionsRequest(0, _multiTeam.TeamId);
            var expectedResponse = InsightsFactory.GetIndexDimensionsResponse(25);

            await IndexDimensionsValidator(request, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_IndexDimensions_Post_PreCrawl_Benchmark_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetIndexDimensionsRequest(1, (int)AnalyticsMaturityTeamId.Precrawl);
            var expectedResponse = InsightsFactory.GetIndexDimensionsResponse(10, BenchmarkValue);

            await IndexDimensionsValidator(request, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_IndexDimensions_Post_PreCrawl_NoBenchmark_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetIndexDimensionsRequest(0, (int)AnalyticsMaturityTeamId.Precrawl);
            var expectedResponse = InsightsFactory.GetIndexDimensionsResponse(10);

            await IndexDimensionsValidator(request, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_IndexDimensions_Post_Walk_Benchmark_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetIndexDimensionsRequest(1, (int)AnalyticsMaturityTeamId.Walk);
            var expectedResponse = InsightsFactory.GetIndexDimensionsResponse(40, BenchmarkValue);

            await IndexDimensionsValidator(request, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_IndexDimensions_Post_Walk_NoBenchmark_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetIndexDimensionsRequest(0, (int)AnalyticsMaturityTeamId.Walk);
            var expectedResponse = InsightsFactory.GetIndexDimensionsResponse(40);

            await IndexDimensionsValidator(request, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_IndexDimensions_Post_Run_Benchmark_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetIndexDimensionsRequest(1, (int)AnalyticsMaturityTeamId.Run);
            var expectedResponse = InsightsFactory.GetIndexDimensionsResponse(70, BenchmarkValue);

            await IndexDimensionsValidator(request, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_IndexDimensions_Post_Run_NoBenchmark_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetIndexDimensionsRequest(0, (int)AnalyticsMaturityTeamId.Run);
            var expectedResponse = InsightsFactory.GetIndexDimensionsResponse(70);

            await IndexDimensionsValidator(request, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_IndexDimensions_Post_Fly_Benchmark_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetIndexDimensionsRequest(1, (int)AnalyticsMaturityTeamId.Fly);
            var expectedResponse = InsightsFactory.GetIndexDimensionsResponse(100, BenchmarkValue);

            await IndexDimensionsValidator(request, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_IndexDimensions_Post_Fly_NoBenchmark_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetIndexDimensionsRequest(0, (int)AnalyticsMaturityTeamId.Fly);
            var expectedResponse = InsightsFactory.GetIndexDimensionsResponse(100);

            await IndexDimensionsValidator(request, expectedResponse);
        }

        // 401
        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_IndexDimensions_Post_Unauthorized()
        {
            // given
            var client = GetUnauthenticatedClient();
            var request = InsightsFactory.GetIndexDimensionsRequest(1);
            // when
            var response = await client.PostAsync(
                RequestUris.AnalyticsIndexDimensions(Company.Id), request.ToStringContent());

            // then
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status code doesn't match.");
        }



        // 403
        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_IndexDimensions_Post_Forbidden()
        {
            // given
            var client = await GetAuthenticatedClient();
            var request = InsightsFactory.GetIndexDimensionsRequest(1, 1);
            // when
            var response = await client.PostAsync(
                RequestUris.AnalyticsIndexDimensions(Company.Id), request.ToStringContent());

            // then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status code doesn't match.");
        }

        private async Task IndexDimensionsValidator(IndexDimensionsRequest request,
            IEnumerable<DimensionsResponse> expectedResponse)
        {
            // given
            var client = await GetInsightsAuthenticatedClient();

            // when
            var response = await client.PostAsync<IList<DimensionsResponse>>(
                RequestUris.AnalyticsIndexDimensions(Company.InsightsId), request);

            // then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status code doesn't match.");
            foreach (var dimension in expectedResponse)
            {
                var actualDimension = response.Dto.FirstOrDefault(d => d.Dimension == dimension.Dimension)
                    .CheckForNull($"{dimension.Dimension} was not found in the response.");
                Assert.AreEqual(dimension.ResultPercentage, actualDimension.ResultPercentage, 
                    $"ResultPercentage does not match for {dimension.Dimension}");
                Assert.AreEqual(dimension.BenchmarkResultPercentage, actualDimension.BenchmarkResultPercentage, 
                    $"BenchmarkResultPercentage does not match for {dimension.Dimension}");
            }
        }
    }
}