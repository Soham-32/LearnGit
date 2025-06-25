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
    public class PostAnalyticsDimensionsItemsTests : BaseV1Test
    {
        private static bool _classInitFailed;
        private static TeamHierarchyResponse _team;
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
                _team = companyHierarchy.GetTeamByName(SharedConstants.InsightsIndividualTeam1);
            }
            catch (System.Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_DimensionsItems_Post_Company_Maturity_Low_Benchmark_OK()
        {
            var dimensionsItems = InsightsFactory.GetDimensionsRequest(WidgetType.Maturity, DimensionSortOrder.Low, 0, 1);
            var expectedResponse = InsightsFactory.GetDimensionItemsResponse(WidgetType.Maturity, 55, BenchmarkValue);

            await DimensionsResponseValidator(dimensionsItems, expectedResponse);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_DimensionsItems_Post_Company_Maturity_High_Benchmark_OK()
        {
            var dimensionsItems = InsightsFactory.GetDimensionsRequest(WidgetType.Maturity, DimensionSortOrder.High, 0, 1);
            var expectedResponse = InsightsFactory.GetDimensionItemsResponse(WidgetType.Maturity, 55, BenchmarkValue);

            await DimensionsResponseValidator(dimensionsItems, expectedResponse);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_DimensionsItems_Post_Company_Performance_Low_Benchmark_OK()
        {
            var dimensionsItems = InsightsFactory.GetDimensionsRequest(WidgetType.Performance, DimensionSortOrder.Low, 0, 1);
            var expectedResponse = InsightsFactory.GetDimensionItemsResponse(WidgetType.Performance, 55, BenchmarkValue);

            await DimensionsResponseValidator(dimensionsItems, expectedResponse);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_DimensionsItems_Post_Company_Performance_High_Benchmark_OK()
        {
            var dimensionsItems = InsightsFactory.GetDimensionsRequest(WidgetType.Performance, DimensionSortOrder.High, 0, 1);
            var expectedResponse = InsightsFactory.GetDimensionItemsResponse(WidgetType.Performance, 55, BenchmarkValue);

            await DimensionsResponseValidator(dimensionsItems, expectedResponse);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_DimensionsItems_Post_Company_Maturity_Low_NoBenchmark_OK()
        {
            var dimensionsItems = InsightsFactory.GetDimensionsRequest(WidgetType.Maturity, DimensionSortOrder.Low, 0);
            var expectedResponse = InsightsFactory.GetDimensionItemsResponse(WidgetType.Maturity, 55);

            await DimensionsResponseValidator(dimensionsItems, expectedResponse);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_DimensionsItems_Post_Company_Maturity_High_NoBenchmark_OK()
        {
            var dimensionsItems = InsightsFactory.GetDimensionsRequest(WidgetType.Maturity, DimensionSortOrder.High, 0);
            var expectedResponse = InsightsFactory.GetDimensionItemsResponse(WidgetType.Maturity, 55);

            await DimensionsResponseValidator(dimensionsItems, expectedResponse);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_DimensionsItems_Post_Company_Performance_Low_NoBenchmark_OK()
        {
            var dimensionsItems = InsightsFactory.GetDimensionsRequest(WidgetType.Performance, DimensionSortOrder.Low, 0);
            var expectedResponse = InsightsFactory.GetDimensionItemsResponse(WidgetType.Performance, 55);

            await DimensionsResponseValidator(dimensionsItems, expectedResponse);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_DimensionsItems_Post_Company_Performance_High_NoBenchmark_OK()
        {
            var dimensionsItems = InsightsFactory.GetDimensionsRequest(WidgetType.Performance, DimensionSortOrder.High, 0);
            var expectedResponse = InsightsFactory.GetDimensionItemsResponse(WidgetType.Performance, 55);

            await DimensionsResponseValidator(dimensionsItems, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_DimensionsItems_Post_Enterprise_Maturity_Low_Benchmark_OK()
        {
            VerifySetup(_classInitFailed);
            var dimensionsItems = InsightsFactory.GetDimensionsRequest(WidgetType.Maturity, DimensionSortOrder.Low, _enterpriseTeam.TeamId, 1);
            var expectedResponse = InsightsFactory.GetDimensionItemsResponse(WidgetType.Maturity, 55, BenchmarkValue);

            await DimensionsResponseValidator(dimensionsItems, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_DimensionsItems_Post_Enterprise_Maturity_High_Benchmark_OK()
        {
            VerifySetup(_classInitFailed);
            var dimensionsItems = InsightsFactory.GetDimensionsRequest(WidgetType.Maturity, DimensionSortOrder.High, _enterpriseTeam.TeamId, 1);
            var expectedResponse = InsightsFactory.GetDimensionItemsResponse(WidgetType.Maturity, 55, BenchmarkValue);

            await DimensionsResponseValidator(dimensionsItems, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_DimensionsItems_Post_Enterprise_Performance_Low_Benchmark_OK()
        {
            VerifySetup(_classInitFailed);
            var dimensionsItems = InsightsFactory.GetDimensionsRequest(WidgetType.Performance, DimensionSortOrder.Low, _enterpriseTeam.TeamId, 1);
            var expectedResponse = InsightsFactory.GetDimensionItemsResponse(WidgetType.Performance, 55, BenchmarkValue);

            await DimensionsResponseValidator(dimensionsItems, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_DimensionsItems_Post_Enterprise_Performance_High_Benchmark_OK()
        {
            VerifySetup(_classInitFailed);
            var dimensionsItems = InsightsFactory.GetDimensionsRequest(WidgetType.Performance, DimensionSortOrder.High, _enterpriseTeam.TeamId, 1);
            var expectedResponse = InsightsFactory.GetDimensionItemsResponse(WidgetType.Performance, 55, BenchmarkValue);

            await DimensionsResponseValidator(dimensionsItems, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_DimensionsItems_Post_Enterprise_Maturity_Low_NoBenchmark_OK()
        {
            VerifySetup(_classInitFailed);
            var dimensionsItems = InsightsFactory.GetDimensionsRequest(WidgetType.Maturity, DimensionSortOrder.Low, _enterpriseTeam.TeamId);
            var expectedResponse = InsightsFactory.GetDimensionItemsResponse(WidgetType.Maturity, 55);

            await DimensionsResponseValidator(dimensionsItems, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_DimensionsItems_Post_Enterprise_Maturity_High_NoBenchmark_OK()
        {
            VerifySetup(_classInitFailed);
            var dimensionsItems = InsightsFactory.GetDimensionsRequest(WidgetType.Maturity, DimensionSortOrder.High, _enterpriseTeam.TeamId);
            var expectedResponse = InsightsFactory.GetDimensionItemsResponse(WidgetType.Maturity, 55);

            await DimensionsResponseValidator(dimensionsItems, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_DimensionsItems_Post_Enterprise_Performance_Low_NoBenchmark_OK()
        {
            VerifySetup(_classInitFailed);
            var dimensionsItems = InsightsFactory.GetDimensionsRequest(WidgetType.Performance, DimensionSortOrder.Low, _enterpriseTeam.TeamId);
            var expectedResponse = InsightsFactory.GetDimensionItemsResponse(WidgetType.Performance, 55);

            await DimensionsResponseValidator(dimensionsItems, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_DimensionsItems_Post_Enterprise_Performance_High_NoBenchmark_OK()
        {
            VerifySetup(_classInitFailed);
            var dimensionsItems = InsightsFactory.GetDimensionsRequest(WidgetType.Performance, DimensionSortOrder.High, _enterpriseTeam.TeamId);
            var expectedResponse = InsightsFactory.GetDimensionItemsResponse(WidgetType.Performance, 55);

            await DimensionsResponseValidator(dimensionsItems, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_DimensionsItems_Post_MultiTeam_Maturity_Low_Benchmark_OK()
        {
            VerifySetup(_classInitFailed);
            var dimensionsItems = InsightsFactory.GetDimensionsRequest(WidgetType.Maturity, DimensionSortOrder.Low, _multiTeam.TeamId, 1);
            var expectedResponse = InsightsFactory.GetDimensionItemsResponse(WidgetType.Maturity, 25, BenchmarkValue);

            await DimensionsResponseValidator(dimensionsItems, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_DimensionsItems_Post_MultiTeam_Maturity_High_Benchmark_OK()
        {
            VerifySetup(_classInitFailed);
            var dimensionsItems = InsightsFactory.GetDimensionsRequest(WidgetType.Maturity, DimensionSortOrder.High, _multiTeam.TeamId, 1);
            var expectedResponse = InsightsFactory.GetDimensionItemsResponse(WidgetType.Maturity, 25, BenchmarkValue);

            await DimensionsResponseValidator(dimensionsItems, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_DimensionsItems_Post_MultiTeam_Performance_Low_Benchmark_OK()
        {
            VerifySetup(_classInitFailed);
            var dimensionsItems = InsightsFactory.GetDimensionsRequest(WidgetType.Performance, DimensionSortOrder.Low, _multiTeam.TeamId, 1);
            var expectedResponse = InsightsFactory.GetDimensionItemsResponse(WidgetType.Performance, 25, BenchmarkValue);

            await DimensionsResponseValidator(dimensionsItems, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_DimensionsItems_Post_MultiTeam_Performance_High_Benchmark_OK()
        {
            VerifySetup(_classInitFailed);
            var dimensionsItems = InsightsFactory.GetDimensionsRequest(WidgetType.Performance, DimensionSortOrder.High, _multiTeam.TeamId, 1);
            var expectedResponse = InsightsFactory.GetDimensionItemsResponse(WidgetType.Performance, 25, BenchmarkValue);

            await DimensionsResponseValidator(dimensionsItems, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_DimensionsItems_Post_MultiTeam_Maturity_Low_NoBenchmark_OK()
        {
            VerifySetup(_classInitFailed);
            var dimensionsItems = InsightsFactory.GetDimensionsRequest(WidgetType.Maturity, DimensionSortOrder.Low, _multiTeam.TeamId);
            var expectedResponse = InsightsFactory.GetDimensionItemsResponse(WidgetType.Maturity, 25);

            await DimensionsResponseValidator(dimensionsItems, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_DimensionsItems_Post_MultiTeam_Maturity_High_NoBenchmark_OK()
        {
            VerifySetup(_classInitFailed);
            var dimensionsItems = InsightsFactory.GetDimensionsRequest(WidgetType.Maturity, DimensionSortOrder.High, _multiTeam.TeamId);
            var expectedResponse = InsightsFactory.GetDimensionItemsResponse(WidgetType.Maturity, 25);

            await DimensionsResponseValidator(dimensionsItems, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_DimensionsItems_Post_MultiTeam_Performance_Low_NoBenchmark_OK()
        {
            VerifySetup(_classInitFailed);
            var dimensionsItems = InsightsFactory.GetDimensionsRequest(WidgetType.Performance, DimensionSortOrder.Low, _multiTeam.TeamId);
            var expectedResponse = InsightsFactory.GetDimensionItemsResponse(WidgetType.Performance, 25);

            await DimensionsResponseValidator(dimensionsItems, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_DimensionsItems_Post_MultiTeam_Performance_High_NoBenchmark_OK()
        {
            VerifySetup(_classInitFailed);
            var dimensionsItems = InsightsFactory.GetDimensionsRequest(WidgetType.Performance, DimensionSortOrder.High, _multiTeam.TeamId);
            var expectedResponse = InsightsFactory.GetDimensionItemsResponse(WidgetType.Performance, 25);

            await DimensionsResponseValidator(dimensionsItems, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_DimensionsItems_Post_Team_Maturity_Low_Benchmark_OK()
        {
            VerifySetup(_classInitFailed);
            var dimensionsItems = InsightsFactory.GetDimensionsRequest(WidgetType.Maturity, DimensionSortOrder.Low, _team.TeamId, 1);
            var expectedResponse = InsightsFactory.GetDimensionItemsResponse(WidgetType.Maturity, 10, BenchmarkValue);

            await DimensionsResponseValidator(dimensionsItems, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_DimensionsItems_Post_Team_Maturity_High_Benchmark_OK()
        {
            VerifySetup(_classInitFailed);
            var dimensionsItems = InsightsFactory.GetDimensionsRequest(WidgetType.Maturity, DimensionSortOrder.High, _team.TeamId, 1);
            var expectedResponse = InsightsFactory.GetDimensionItemsResponse(WidgetType.Maturity, 10, BenchmarkValue);

            await DimensionsResponseValidator(dimensionsItems, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_DimensionsItems_Post_Team_Performance_Low_Benchmark_OK()
        {
            VerifySetup(_classInitFailed);
            var dimensionsItems = InsightsFactory.GetDimensionsRequest(WidgetType.Performance, DimensionSortOrder.Low, _team.TeamId, 1);
            var expectedResponse = InsightsFactory.GetDimensionItemsResponse(WidgetType.Performance, 10, BenchmarkValue);

            await DimensionsResponseValidator(dimensionsItems, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_DimensionsItems_Post_Team_Performance_High_Benchmark_OK()
        {
            VerifySetup(_classInitFailed);
            var dimensionsItems = InsightsFactory.GetDimensionsRequest(WidgetType.Performance, DimensionSortOrder.High, _team.TeamId, 1);
            var expectedResponse = InsightsFactory.GetDimensionItemsResponse(WidgetType.Performance, 10, BenchmarkValue);

            await DimensionsResponseValidator(dimensionsItems, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_DimensionsItems_Post_Team_Maturity_Low_NoBenchmark_OK()
        {
            VerifySetup(_classInitFailed);
            var dimensionsItems = InsightsFactory.GetDimensionsRequest(WidgetType.Maturity, DimensionSortOrder.Low, _team.TeamId);
            var expectedResponse = InsightsFactory.GetDimensionItemsResponse(WidgetType.Maturity, 10);

            await DimensionsResponseValidator(dimensionsItems, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_DimensionsItems_Post_Team_Maturity_High_NoBenchmark_OK()
        {
            VerifySetup(_classInitFailed);
            var dimensionsItems = InsightsFactory.GetDimensionsRequest(WidgetType.Maturity, DimensionSortOrder.High, _team.TeamId);
            var expectedResponse = InsightsFactory.GetDimensionItemsResponse(WidgetType.Maturity, 10);

            await DimensionsResponseValidator(dimensionsItems, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_DimensionsItems_Post_Team_Performance_Low_NoBenchmark_OK()
        {
            VerifySetup(_classInitFailed);
            var dimensionsItems = InsightsFactory.GetDimensionsRequest(WidgetType.Performance, DimensionSortOrder.Low, _team.TeamId);
            var expectedResponse = InsightsFactory.GetDimensionItemsResponse(WidgetType.Performance, 10);

            await DimensionsResponseValidator(dimensionsItems, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_DimensionsItems_Post_Team_Performance_High_NoBenchmark_OK()
        {
            VerifySetup(_classInitFailed);
            var dimensionsItems = InsightsFactory.GetDimensionsRequest(WidgetType.Performance, DimensionSortOrder.High, _team.TeamId);
            var expectedResponse = InsightsFactory.GetDimensionItemsResponse(WidgetType.Performance, 10);

            await DimensionsResponseValidator(dimensionsItems, expectedResponse);
        }

        public async Task DimensionsResponseValidator(DimensionsAnalyticsRequest dimensionsItems, IList<DimensionsResponse> expectedResponse)
        {
            var client = await GetInsightsAuthenticatedClient();
            var actualResponse = await client.PostAsync<IList<DimensionsResponse>>(RequestUris.AnalyticsDimensionsItems(Company.InsightsId), dimensionsItems);
            
            Assert.AreEqual(HttpStatusCode.OK, actualResponse.StatusCode, "StatusCode does not match.");
            Assert.AreEqual(expectedResponse.Count, actualResponse.Dto.Count, "Table Count does not match.");

            foreach (var item in expectedResponse)
            {
                var actual = actualResponse.Dto.FirstOrDefault(table => table.Dimension == item.Dimension)
                    .CheckForNull($"<{item.Dimension}> was not found in the response.");
                Assert.AreEqual(item.ResultPercentage, actual.ResultPercentage, $"AvgValue does not match for {item.Dimension} Segment.");
                Assert.AreEqual(item.BenchmarkResultPercentage, actual.BenchmarkResultPercentage, $"BenchmarkAvgValue does not match for {item.Dimension} Segment.");
            }
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_DimensionsItems_Post_Unauthorized()
        {
            var client = GetUnauthenticatedClient();
            var request = InsightsFactory.GetDimensionsRequest(WidgetType.Maturity, DimensionSortOrder.Low, 0);
            
            var response = await client.PostAsync(RequestUris.AnalyticsDimensionsItems(Company.InsightsId), 
                request.ToStringContent());
            
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "StatusCode does not match.");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_DimensionsItems_Post_Forbidden()
        {
            var request = InsightsFactory.GetDimensionsRequest(WidgetType.Maturity, DimensionSortOrder.Low, 1);

            var client = await GetInsightsAuthenticatedClient();
            var response = await client.PostAsync(RequestUris.AnalyticsDimensionsItems(Company.InsightsId), 
                request.ToStringContent());

            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "StatusCode does not match.");
        }

    }
}