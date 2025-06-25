using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ApiTests.v1.Endpoints;
using AtCommon.Api;
using AtCommon.Dtos.Analytics;
using AtCommon.Dtos.Analytics.Custom;
using AtCommon.Dtos.Companies;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiTests.Analytics
{
    //[TestClass]
    [TestCategory("Insights")]
    public class PostAnalyticsIndexItemsTests : BaseV1Test
    {
        private static bool _classInitFailed;
        private static TeamHierarchyResponse _multiTeam;
        private static TeamHierarchyResponse _enterpriseTeam;

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

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_IndexItems_Post_Company_Maturity_OK()
        {
            
            var request = new IndexAnalyticsRequest
            {
                WidgetType = WidgetType.Maturity,
                TeamIds = new List<int> { 0 },
                EndQuarter = AnalyticsQuarter.GetValidQuarters().Last().QuarterName
            };

            var expectedResponse = new List<IndexAnalyticsResponse>
            {
                new IndexAnalyticsResponse
                {
                    IndexType = "Maturity",
                    ResultPercentage = 55,
                    TeamsCount = 4,
                    PreCrawl = 1,
                    Walk = 1,
                    Run = 1,
                    Fly = 1
                }
            };

            await IndexItemsResponseValidator(request, expectedResponse);
        }

        [TestMethod]
        [TestCategory("KnownDefect")] // Bug Id : 47061
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_IndexItems_Post_Company_Performance_OK()
        {
            var request = new IndexAnalyticsRequest
            {
                WidgetType = WidgetType.Performance,
                TeamIds = new List<int> { 0 },
                EndQuarter = AnalyticsQuarter.GetValidQuarters()[0].QuarterName
            };

            var expectedResponse = new List<IndexAnalyticsResponse>
            {
                new IndexAnalyticsResponse
                {
                    IndexType = "Performance",
                    ResultPercentage = 55,
                    TeamsCount = 4,
                    PreCrawl = 1,
                    Walk = 1,
                    Run = 1,
                    Fly = 1
                }
            };

            await IndexItemsResponseValidator(request, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_IndexItems_Post_Enterprise_Maturity_OK()
        {
            VerifySetup(_classInitFailed);
            var request = new IndexAnalyticsRequest
            {
                WidgetType = WidgetType.Maturity,
                TeamIds = new List<int> { _enterpriseTeam.TeamId },
                EndQuarter = AnalyticsQuarter.GetValidQuarters().Last().QuarterName
            };

            var expectedResponse = new List<IndexAnalyticsResponse>
            {
                new IndexAnalyticsResponse
                {
                    IndexType = "Maturity",
                    ResultPercentage = 55,
                    TeamsCount = 4,
                    PreCrawl = 1,
                    Walk = 1,
                    Run = 1,
                    Fly = 1
                }
            };

            await IndexItemsResponseValidator(request, expectedResponse);
        }

       [TestMethod]
        [TestCategory("KnownDefect")] // Bug Id : 47061
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_IndexItems_Post_Enterprise_Performance_OK()
        {
            VerifySetup(_classInitFailed);
            var request = new IndexAnalyticsRequest
            {
                WidgetType = WidgetType.Performance,
                TeamIds = new List<int> { _enterpriseTeam.TeamId },
                EndQuarter = AnalyticsQuarter.GetValidQuarters()[0].QuarterName
            };

            var expectedResponse = new List<IndexAnalyticsResponse>
            {
                new IndexAnalyticsResponse
                {
                    IndexType = "Performance",
                    ResultPercentage = 55,
                    TeamsCount = 4,
                    PreCrawl = 1,
                    Walk = 1,
                    Run = 1,
                    Fly = 1
                }
            };

            await IndexItemsResponseValidator(request, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_IndexItems_Post_MultiTeam_Maturity_OK()
        {
            VerifySetup(_classInitFailed);
            var request = new IndexAnalyticsRequest
            {
                WidgetType = WidgetType.Maturity,
                TeamIds = new List<int> { _multiTeam.TeamId },
                EndQuarter = AnalyticsQuarter.GetValidQuarters().Last().QuarterName
            };

            var expectedResponse = new List<IndexAnalyticsResponse>
            {
                new IndexAnalyticsResponse
                {
                    IndexType = "Maturity",
                    ResultPercentage = 25,
                    TeamsCount = 2,
                    PreCrawl = 1,
                    Walk = 1
                }
            };

            await IndexItemsResponseValidator(request, expectedResponse);
        }

        [TestMethod]
        [TestCategory("KnownDefect")] // Bug Id : 47061
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_IndexItems_Post_MultiTeam_Performance_OK()
        {
            VerifySetup(_classInitFailed);
            var request = new IndexAnalyticsRequest
            {
                WidgetType = WidgetType.Performance,
                TeamIds = new List<int> { _multiTeam.TeamId },
                EndQuarter = AnalyticsQuarter.GetValidQuarters()[0].QuarterName
            };

            var expectedResponse = new List<IndexAnalyticsResponse>
            {
                new IndexAnalyticsResponse
                {
                    IndexType = "Performance",
                    ResultPercentage = 25,
                    TeamsCount = 2,
                    PreCrawl = 1,
                    Walk = 1
                }
            };

            await IndexItemsResponseValidator(request, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_IndexItems_Post_PreCrawl_Maturity_OK()
        {
            VerifySetup(_classInitFailed);
            var request = new IndexAnalyticsRequest
            {
                WidgetType = WidgetType.Maturity,
                TeamIds = new List<int> { (int)AnalyticsMaturityTeamId.Precrawl },
                EndQuarter = AnalyticsQuarter.GetValidQuarters()[0].QuarterName
            };

            var expectedResponse = new List<IndexAnalyticsResponse>
            {
                new IndexAnalyticsResponse
                {
                    IndexType = "Maturity",
                    ResultPercentage = 10,
                    TeamsCount = 1,
                    PreCrawl = 1
                }
            };

            await IndexItemsResponseValidator(request, expectedResponse);
        }

        [TestMethod]
        [TestCategory("KnownDefect")] // Bug Id : 47061
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_IndexItems_Post_Performance_PreCrawl_OK()
        {
            VerifySetup(_classInitFailed);
            var request = new IndexAnalyticsRequest
            {
                WidgetType = WidgetType.Performance,
                TeamIds = new List<int> { (int)AnalyticsMaturityTeamId.Precrawl },
                EndQuarter = AnalyticsQuarter.GetValidQuarters()[0].QuarterName
            };

            var expectedResponse = new List<IndexAnalyticsResponse>
            {
                new IndexAnalyticsResponse
                {
                    IndexType = "Performance",
                    ResultPercentage = 10,
                    TeamsCount = 1,
                    PreCrawl = 1
                }
            };

            await IndexItemsResponseValidator(request, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_IndexItems_Post_Walk_Maturity_OK()
        {
            VerifySetup(_classInitFailed);
            var request = new IndexAnalyticsRequest
            {
                WidgetType = WidgetType.Maturity,
                TeamIds = new List<int> { (int)AnalyticsMaturityTeamId.Walk },
                EndQuarter = AnalyticsQuarter.GetValidQuarters()[0].QuarterName
            };

            var expectedResponse = new List<IndexAnalyticsResponse>
            {
                new IndexAnalyticsResponse
                {
                    IndexType = "Maturity",
                    ResultPercentage = 40,
                    TeamsCount = 1,
                    Walk = 1
                }
            };

            await IndexItemsResponseValidator(request, expectedResponse);
        }

        [TestMethod]
        [TestCategory("KnownDefect")] // Bug Id : 47061
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_IndexItems_Post_Walk_Performance_OK()
        {
            VerifySetup(_classInitFailed);
            var request = new IndexAnalyticsRequest
            {
                WidgetType = WidgetType.Performance,
                TeamIds = new List<int> { (int)AnalyticsMaturityTeamId.Walk },
                EndQuarter = AnalyticsQuarter.GetValidQuarters()[0].QuarterName
            };

            var expectedResponse = new List<IndexAnalyticsResponse>
            {
                new IndexAnalyticsResponse
                {
                    IndexType = "Performance",
                    ResultPercentage = 40,
                    TeamsCount = 1,
                    Walk = 1
                }
            };

            await IndexItemsResponseValidator(request, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_IndexItems_Post_Run_Maturity_OK()
        {
            VerifySetup(_classInitFailed);
            var request = new IndexAnalyticsRequest
            {
                WidgetType = WidgetType.Maturity,
                TeamIds = new List<int> { (int)AnalyticsMaturityTeamId.Run },
                EndQuarter = AnalyticsQuarter.GetValidQuarters()[0].QuarterName
            };

            var expectedResponse = new List<IndexAnalyticsResponse>
            {
                new IndexAnalyticsResponse
                {
                    IndexType = "Maturity",
                    ResultPercentage = 70,
                    TeamsCount = 1,
                    Run = 1
                }
            };

            await IndexItemsResponseValidator(request, expectedResponse);
        }

        [TestMethod]
        [TestCategory("KnownDefect")] // Bug Id : 47061
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_IndexItems_Post_Run_Performance_OK()
        {
            VerifySetup(_classInitFailed);
            var request = new IndexAnalyticsRequest
            {
                WidgetType = WidgetType.Performance,
                TeamIds = new List<int> { (int)AnalyticsMaturityTeamId.Run },
                EndQuarter = AnalyticsQuarter.GetValidQuarters()[0].QuarterName
            };

            var expectedResponse = new List<IndexAnalyticsResponse>
            {
                new IndexAnalyticsResponse
                {
                    IndexType = "Performance",
                    ResultPercentage = 70,
                    TeamsCount = 1,
                    Run = 1
                }
            };

            await IndexItemsResponseValidator(request, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_IndexItems_Post_Fly_Maturity_OK()
        {
            VerifySetup(_classInitFailed);
            var request = new IndexAnalyticsRequest
            {
                WidgetType = WidgetType.Maturity,
                TeamIds = new List<int> { (int)AnalyticsMaturityTeamId.Fly },
                EndQuarter = AnalyticsQuarter.GetValidQuarters()[0].QuarterName
            };

            var expectedResponse = new List<IndexAnalyticsResponse>
            {
                new IndexAnalyticsResponse
                {
                    IndexType = "Maturity",
                    ResultPercentage = 100,
                    TeamsCount = 1,
                    Fly = 1
                }
            };

            await IndexItemsResponseValidator(request, expectedResponse);
        }
        
        [TestMethod]
        [TestCategory("KnownDefect")] // Bug Id : 47061
        [TestCategory("Avengers")]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_IndexItems_Post_Fly_Performance_OK()
        {
            VerifySetup(_classInitFailed);
            var request = new IndexAnalyticsRequest
            {
                WidgetType = WidgetType.Performance,
                TeamIds = new List<int> { (int)AnalyticsMaturityTeamId.Fly },
                EndQuarter = AnalyticsQuarter.GetValidQuarters()[0].QuarterName
            };

            var expectedResponse = new List<IndexAnalyticsResponse>
            {
                new IndexAnalyticsResponse
                {
                    IndexType = "Performance",
                    ResultPercentage = 100,
                    TeamsCount = 1,
                    Fly = 1
                }
            };

            await IndexItemsResponseValidator(request, expectedResponse);
        }

        public async Task IndexItemsResponseValidator(IndexAnalyticsRequest indexItems, 
            IList<IndexAnalyticsResponse> expectedResponse)
        {
            var client = await GetInsightsAuthenticatedClient();
            var response = await client.PostAsync<IList<IndexAnalyticsResponse>>(
                RequestUris.AnalyticsIndexItems(Company.InsightsId), indexItems);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status code does not match.");
            Assert.AreEqual(expectedResponse.Count, response.Dto.Count, 
                "Table Count does not match.");
            foreach (var item in expectedResponse)
            {
                var tableResponse = response.Dto.FirstOrDefault(table => table.IndexType == item.IndexType)
                    .CheckForNull($"<{item.IndexType}> was not found in the response.");
                Assert.AreEqual(item.ResultPercentage, tableResponse.ResultPercentage, 
                    $"AvgValue does not match for <{item.IndexType}>.");
                Assert.AreEqual(item.TeamsCount, tableResponse.TeamsCount, 
                    $"TeamsCount does not match for <{item.IndexType}>.");
                Assert.AreEqual(item.PreCrawl, tableResponse.PreCrawl, 
                    $"PreCrawl does not match for <{item.IndexType}>.");
                Assert.AreEqual(item.Crawl, tableResponse.Crawl, $"Crawl does not match for <{item.IndexType}>.");
                Assert.AreEqual(item.Walk, tableResponse.Walk, $"Walk does not match for <{item.IndexType}>.");
                Assert.AreEqual(item.Run, tableResponse.Run, $"Run does not match for <{item.IndexType}>.");
                Assert.AreEqual(item.Fly, tableResponse.Fly, $"Fly does not match for <{item.IndexType}>.");
            }
        }

        [TestMethod]
        [TestCategory("Avengers")]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_IndexItems_Post_Agility_Unauthorized()
        {
            var request = InsightsFactory.GetIndexRequest(WidgetType.Agility, 0);

            var client = GetUnauthenticatedClient();
            var response = await client.PostAsync(
                RequestUris.AnalyticsIndexItems(Company.InsightsId), request.ToStringContent());
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status Code does not match.");
        }

        [TestMethod]
        [TestCategory("Avengers")]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_IndexItems_Post_Agility_BadRequest()
        {
            // Given
            var client = await GetInsightsAuthenticatedClient();
            var request = InsightsFactory.GetIndexRequest(WidgetType.FalseTest, 0);

            // When
            var response = await client.PostAsync<IList<string>>(
                RequestUris.AnalyticsIndexItems(Company.InsightsId), request);
            
            // Then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status Code does not match.");
            Assert.AreEqual($"'Widget Type' has a range of values which does not include '{WidgetType.FalseTest:D}'.", 
                response.Dto.FirstOrDefault(), "Error message does not match.");
        }
    }
}
