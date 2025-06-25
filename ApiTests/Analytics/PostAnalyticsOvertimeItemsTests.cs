using ApiTests.v1.Endpoints;
using AtCommon.Api;
using AtCommon.Dtos.Analytics;
using AtCommon.Dtos.Companies;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ApiTests.Analytics
{
    //[TestClass]
    [TestCategory("Insights")]
    public class PostAnalyticsOvertimeItemsTests : BaseV1Test
    {
        private static bool _classInitFailed;
        private static TeamHierarchyResponse _preCrawlTeam;
        private static TeamHierarchyResponse _walkTeam;
        private static TeamHierarchyResponse _runTeam;
        private static TeamHierarchyResponse _flyTeam;
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
                _preCrawlTeam = companyHierarchy.GetTeamByName(SharedConstants.InsightsIndividualTeam1);
                _walkTeam = companyHierarchy.GetTeamByName(SharedConstants.InsightsIndividualTeam2);
                _runTeam = companyHierarchy.GetTeamByName(SharedConstants.InsightsIndividualTeam3);
                _flyTeam = companyHierarchy.GetTeamByName(SharedConstants.InsightsIndividualTeam4);
            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_OvertimeItems_Post_Company_Maturity_Benchmark_OK()
        {
            var request = InsightsFactory.GetValidOvertimeRequest(WidgetType.Maturity, 0, 1);
            var expectedResponse = InsightsFactory.GetValidOvertimeResponse(4, 55, 4, 55.0m);

            await OvertimeItemsResponseValidator(request, expectedResponse);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_OvertimeItems_Post_Company_Performance_Benchmark_OK()
        {
            var request = InsightsFactory.GetValidOvertimeRequest(WidgetType.Performance, 0, 1);
            var expectedResponse = InsightsFactory.GetValidOvertimeResponse(4, 55, 4, 55.0m);

            await OvertimeItemsResponseValidator(request, expectedResponse);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_OvertimeItems_Post_Company_Agility_Benchmark_OK()
        {
            var request = InsightsFactory.GetValidOvertimeRequest(WidgetType.Agility, 0, 1);
            var expectedResponse = InsightsFactory.GetValidOvertimeResponse(4, 55.0m, 4, 55.0m);

            await OvertimeItemsResponseValidator(request, expectedResponse);
        }
        
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_OvertimeItems_Post_Company_Maturity_NoBenchmark_OK()
        {
            var request = InsightsFactory.GetValidOvertimeRequest(WidgetType.Maturity, 0);
            var expectedResponse = InsightsFactory.GetValidOvertimeResponse(4, 55);
            await OvertimeItemsResponseValidator(request, expectedResponse);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_OvertimeItems_Post_Company_Performance_NoBenchmark_OK()
        {
            var request = InsightsFactory.GetValidOvertimeRequest(WidgetType.Performance, 0);
            var expectedResponse = InsightsFactory.GetValidOvertimeResponse(4, 55);
            await OvertimeItemsResponseValidator(request, expectedResponse);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_OvertimeItems_Post_Company_Agility_NoBenchmark_OK()
        {
            var request = InsightsFactory.GetValidOvertimeRequest(WidgetType.Agility, 0);
            var expectedResponse = InsightsFactory.GetValidOvertimeResponse(4, 55.0m);
            
            await OvertimeItemsResponseValidator(request, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_OvertimeItems_Post_Enterprise_Maturity_Benchmark_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetValidOvertimeRequest(WidgetType.Maturity, _enterpriseTeam.TeamId, 1);
            var expectedResponse = InsightsFactory.GetValidOvertimeResponse(4, 55, 4, 55.0m);

            await OvertimeItemsResponseValidator(request, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_OvertimeItems_Post_Enterprise_Performance_Benchmark_OK()
        {
            VerifySetup(_classInitFailed);
            var overtimeItems = InsightsFactory.GetValidOvertimeRequest(WidgetType.Performance, _enterpriseTeam.TeamId, 1);
            var expectedResponse = InsightsFactory.GetValidOvertimeResponse(4, 55, 4, 55.0m);

            await OvertimeItemsResponseValidator(overtimeItems, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_OvertimeItems_Post_Enterprise_Agility_Benchmark_OK()
        {
            VerifySetup(_classInitFailed);
            var overtimeItems = InsightsFactory.GetValidOvertimeRequest(WidgetType.Agility, _enterpriseTeam.TeamId, 1);
            var expectedResponse = InsightsFactory.GetValidOvertimeResponse(4, 55.0m, 4, 55.0m);

            await OvertimeItemsResponseValidator(overtimeItems, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_OvertimeItems_Post_Enterprise_Maturity_NoBenchmark_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetValidOvertimeRequest(WidgetType.Maturity, _enterpriseTeam.TeamId);
            var expectedResponse = InsightsFactory.GetValidOvertimeResponse( 4, 55);
            await OvertimeItemsResponseValidator(request, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_OvertimeItems_Post_Enterprise_Performance_NoBenchmark_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetValidOvertimeRequest(WidgetType.Performance, _enterpriseTeam.TeamId);
            var expectedResponse = InsightsFactory.GetValidOvertimeResponse(4, 55);
            await OvertimeItemsResponseValidator(request, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_OvertimeItems_Post_Enterprise_Agility_NoBenchmark_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetValidOvertimeRequest(WidgetType.Agility, _enterpriseTeam.TeamId);
            var expectedResponse = InsightsFactory.GetValidOvertimeResponse(4, 55.0m);

            await OvertimeItemsResponseValidator(request, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_OvertimeItems_Post_MultiTeam_Maturity_Benchmark_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetValidOvertimeRequest(WidgetType.Maturity, _multiTeam.TeamId, 1);
            var expectedResponse = InsightsFactory.GetValidOvertimeResponse(
                2, 25, 4, 55.0m);

            await OvertimeItemsResponseValidator(request, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_OvertimeItems_Post_MultiTeam_Performance_Benchmark_OK()
        {
            VerifySetup(_classInitFailed);
            var overtimeItems = InsightsFactory.GetValidOvertimeRequest(WidgetType.Performance, _multiTeam.TeamId, 1);
            var expectedResponse = InsightsFactory.GetValidOvertimeResponse(
                2, 25, 4, 55.0m);

            await OvertimeItemsResponseValidator(overtimeItems, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_OvertimeItems_Post_MultiTeam_Agility_Benchmark_OK()
        {
            VerifySetup(_classInitFailed);
            var overtimeItems = InsightsFactory.GetValidOvertimeRequest(WidgetType.Agility, _multiTeam.TeamId, 1);
            var expectedResponse = InsightsFactory.GetValidOvertimeResponse(2, 25.0m, 
                4, 55.0m);

            await OvertimeItemsResponseValidator(overtimeItems, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_OvertimeItems_Post_MultiTeam_Maturity_NoBenchmark_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetValidOvertimeRequest(WidgetType.Maturity, _multiTeam.TeamId);
            var expectedResponse = InsightsFactory.GetValidOvertimeResponse(2, 25);

            await OvertimeItemsResponseValidator(request, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_OvertimeItems_Post_MultiTeam_Performance_NoBenchmark_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetValidOvertimeRequest(WidgetType.Performance, _multiTeam.TeamId);
            var expectedResponse = InsightsFactory.GetValidOvertimeResponse( 2, 25);

            await OvertimeItemsResponseValidator(request, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_OvertimeItems_Post_MultiTeam_Agility_NoBenchmark_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetValidOvertimeRequest(WidgetType.Agility, _multiTeam.TeamId);
            var expectedResponse = InsightsFactory.GetValidOvertimeResponse(2, 25.0m);
            
            await OvertimeItemsResponseValidator(request, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_OvertimeItems_Post_TeamPreCrawl_Maturity_Benchmark_OK()
        {
            VerifySetup(_classInitFailed);
            var overtimeItems = InsightsFactory.GetValidOvertimeRequest(WidgetType.Maturity, _preCrawlTeam.TeamId, 1);
            var expectedResponse = InsightsFactory.GetValidOvertimeResponse(1, 10.0m, 4, 55.0m);

            await OvertimeItemsResponseValidator(overtimeItems, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_OvertimeItems_Post_TeamPreCrawl_Performance_Benchmark_OK()
        {
            VerifySetup(_classInitFailed);
            var overtimeItems = InsightsFactory.GetValidOvertimeRequest(WidgetType.Performance, _preCrawlTeam.TeamId, 1);
            var expectedResponse = InsightsFactory.GetValidOvertimeResponse(1, 10.0m, 4, 55.0m);

            await OvertimeItemsResponseValidator(overtimeItems, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_OvertimeItems_Post_TeamPreCrawl_Agility_Benchmark_OK()
        {
            VerifySetup(_classInitFailed);
            var overtimeItems = InsightsFactory.GetValidOvertimeRequest(WidgetType.Agility, _preCrawlTeam.TeamId, 1);
            var expectedResponse = InsightsFactory.GetValidOvertimeResponse(1, 10.0m, 4, 55.0m);

            await OvertimeItemsResponseValidator(overtimeItems, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_OvertimeItems_Post_TeamPreCrawl_Maturity_NoBenchmark_OK()
        {
            VerifySetup(_classInitFailed);
            var overtimeItems = InsightsFactory.GetValidOvertimeRequest(WidgetType.Maturity, _preCrawlTeam.TeamId);
            var expectedResponse = InsightsFactory.GetValidOvertimeResponse(1, 10.0m);

            await OvertimeItemsResponseValidator(overtimeItems, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_OvertimeItems_Post_TeamPreCrawl_Performance_NoBenchmark_OK()
        {
            VerifySetup(_classInitFailed);
            var overtimeItems = InsightsFactory.GetValidOvertimeRequest(WidgetType.Performance, _preCrawlTeam.TeamId);
            var expectedResponse = InsightsFactory.GetValidOvertimeResponse(1, 10.0m);

            await OvertimeItemsResponseValidator(overtimeItems, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_OvertimeItems_Post_TeamPreCrawl_Agility_NoBenchmark_OK()
        {
            VerifySetup(_classInitFailed);
            var overtimeItems = InsightsFactory.GetValidOvertimeRequest(WidgetType.Agility, _preCrawlTeam.TeamId);
            var expectedResponse = InsightsFactory.GetValidOvertimeResponse(1, 10.0m);

            await OvertimeItemsResponseValidator(overtimeItems, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_OvertimeItems_Post_TeamWalk_Maturity_Benchmark_OK()
        {
            VerifySetup(_classInitFailed);
            var overtimeItems = InsightsFactory.GetValidOvertimeRequest(WidgetType.Maturity, _walkTeam.TeamId, 1);
            var expectedResponse = InsightsFactory.GetValidOvertimeResponse(1, 40.0m, 4, 55.0m);

            await OvertimeItemsResponseValidator(overtimeItems, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_OvertimeItems_Post_TeamWalk_Performance_Benchmark_OK()
        {
            VerifySetup(_classInitFailed);
            var overtimeItems = InsightsFactory.GetValidOvertimeRequest(WidgetType.Performance, _walkTeam.TeamId, 1);
            var expectedResponse = InsightsFactory.GetValidOvertimeResponse(1, 40.0m, 4, 55.0m);

            await OvertimeItemsResponseValidator(overtimeItems, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_OvertimeItems_Post_TeamWalk_Agility_Benchmark_OK()
        {
            VerifySetup(_classInitFailed);
            var overtimeItems = InsightsFactory.GetValidOvertimeRequest(WidgetType.Agility, _walkTeam.TeamId, 1);
            var expectedResponse = InsightsFactory.GetValidOvertimeResponse(1, 40.0m, 4, 55.0m);

            await OvertimeItemsResponseValidator(overtimeItems, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_OvertimeItems_Post_TeamWalk_Maturity_NoBenchmark_OK()
        {
            VerifySetup(_classInitFailed);
            var overtimeItems = InsightsFactory.GetValidOvertimeRequest(WidgetType.Maturity, _walkTeam.TeamId);
            var expectedResponse = InsightsFactory.GetValidOvertimeResponse(1, 40.0m);

            await OvertimeItemsResponseValidator(overtimeItems, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_OvertimeItems_Post_TeamWalk_Performance_NoBenchmark_OK()
        {
            VerifySetup(_classInitFailed);
            var overtimeItems = InsightsFactory.GetValidOvertimeRequest(WidgetType.Performance, _walkTeam.TeamId);
            var expectedResponse = InsightsFactory.GetValidOvertimeResponse(1, 40.0m);

            await OvertimeItemsResponseValidator(overtimeItems, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_OvertimeItems_Post_TeamWalk_Agility_NoBenchmark_OK()
        {
            VerifySetup(_classInitFailed);
            var overtimeItems = InsightsFactory.GetValidOvertimeRequest(WidgetType.Agility, _walkTeam.TeamId);
            var expectedResponse = InsightsFactory.GetValidOvertimeResponse(1, 40.0m);

            await OvertimeItemsResponseValidator(overtimeItems, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_OvertimeItems_Post_TeamRun_Maturity_Benchmark_OK()
        {
            VerifySetup(_classInitFailed);
            var overtimeItems = InsightsFactory.GetValidOvertimeRequest(WidgetType.Maturity, _runTeam.TeamId, 1);
            var expectedResponse = InsightsFactory.GetValidOvertimeResponse(1, 70.0m,
                4, 55.0m);

            await OvertimeItemsResponseValidator(overtimeItems, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_OvertimeItems_Post_TeamRun_Performance_Benchmark_OK()
        {
            VerifySetup(_classInitFailed);
            var overtimeItems = InsightsFactory.GetValidOvertimeRequest(WidgetType.Performance, _runTeam.TeamId, 1);
            var expectedResponse = InsightsFactory.GetValidOvertimeResponse(1, 70.0m,
                4, 55.0m);

            await OvertimeItemsResponseValidator(overtimeItems, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_OvertimeItems_Post_TeamRun_Agility_Benchmark_OK()
        {
            VerifySetup(_classInitFailed);
            var overtimeItems = InsightsFactory.GetValidOvertimeRequest(WidgetType.Agility, _runTeam.TeamId, 1);
            var expectedResponse = InsightsFactory.GetValidOvertimeResponse(1, 70.0m,
                4, 55.0m);

            await OvertimeItemsResponseValidator(overtimeItems, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_OvertimeItems_Post_TeamRun_Maturity_NoBenchmark_OK()
        {
            VerifySetup(_classInitFailed);
            var overtimeItems = InsightsFactory.GetValidOvertimeRequest(WidgetType.Maturity, _runTeam.TeamId);
            var expectedResponse = InsightsFactory.GetValidOvertimeResponse(1, 70.0m);

            await OvertimeItemsResponseValidator(overtimeItems, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_OvertimeItems_Post_TeamRun_Performance_NoBenchmark_OK()
        {
            VerifySetup(_classInitFailed);
            var overtimeItems = InsightsFactory.GetValidOvertimeRequest(WidgetType.Performance, _runTeam.TeamId);
            var expectedResponse = InsightsFactory.GetValidOvertimeResponse(1, 70.0m);

            await OvertimeItemsResponseValidator(overtimeItems, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_OvertimeItems_Post_TeamRun_Agility_NoBenchmark_OK()
        {
            VerifySetup(_classInitFailed);
            var overtimeItems = InsightsFactory.GetValidOvertimeRequest(WidgetType.Agility, _runTeam.TeamId);
            var expectedResponse = InsightsFactory.GetValidOvertimeResponse(1, 70.0m);

            await OvertimeItemsResponseValidator(overtimeItems, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_OvertimeItems_Post_TeamFly_Maturity_Benchmark_OK()
        {
            VerifySetup(_classInitFailed);
            var overtimeItems = InsightsFactory.GetValidOvertimeRequest(WidgetType.Maturity, _flyTeam.TeamId, 1);
            var expectedResponse = InsightsFactory.GetValidOvertimeResponse(1, 100.0m,
                4, 55.0m);

            await OvertimeItemsResponseValidator(overtimeItems, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_OvertimeItems_Post_TeamFly_Performance_Benchmark_OK()
        {
            VerifySetup(_classInitFailed);
            var overtimeItems = InsightsFactory.GetValidOvertimeRequest(WidgetType.Performance, _flyTeam.TeamId, 1);
            var expectedResponse = InsightsFactory.GetValidOvertimeResponse(1, 100.0m,
                4, 55.0m);

            await OvertimeItemsResponseValidator(overtimeItems, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_OvertimeItems_Post_TeamFly_Agility_Benchmark_OK()
        {
            VerifySetup(_classInitFailed);
            var overtimeItems = InsightsFactory.GetValidOvertimeRequest(WidgetType.Agility, _flyTeam.TeamId, 1);
            var expectedResponse = InsightsFactory.GetValidOvertimeResponse(1, 100.0m,
                4, 55.0m);

            await OvertimeItemsResponseValidator(overtimeItems, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_OvertimeItems_Post_TeamFly_Maturity_NoBenchmark_OK()
        {
            VerifySetup(_classInitFailed);
            var overtimeItems = InsightsFactory.GetValidOvertimeRequest(WidgetType.Maturity, _flyTeam.TeamId);
            var expectedResponse = InsightsFactory.GetValidOvertimeResponse(1, 100.0m);

            await OvertimeItemsResponseValidator(overtimeItems, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_OvertimeItems_Post_TeamFly_Performance_NoBenchmark_OK()
        {
            VerifySetup(_classInitFailed);
            var overtimeItems = InsightsFactory.GetValidOvertimeRequest(WidgetType.Performance, _flyTeam.TeamId);
            var expectedResponse = InsightsFactory.GetValidOvertimeResponse(1, 100.0m);

            await OvertimeItemsResponseValidator(overtimeItems, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_OvertimeItems_Post_TeamFly_Agility_NoBenchmark_OK()
        {
            VerifySetup(_classInitFailed);
            var overtimeItems = InsightsFactory.GetValidOvertimeRequest(WidgetType.Agility, _flyTeam.TeamId);
            var expectedResponse = InsightsFactory.GetValidOvertimeResponse(1, 100.0m);

            await OvertimeItemsResponseValidator(overtimeItems, expectedResponse);
        }

        public async Task OvertimeItemsResponseValidator(OvertimeAnalyticsRequest overtimeRequest, 
            IList<OvertimeAnalyticsResponse> expected)
        {
            var client = await GetInsightsAuthenticatedClient();
            var response = await client.PostAsync<IList<OvertimeAnalyticsResponse>>(
                RequestUris.AnalyticsOvertimeItems(Company.InsightsId), overtimeRequest);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status Code does not match.");
            Assert.AreEqual(expected.Count, response.Dto.Count, $"Table Count does not match.");

            foreach (var item in expected)
            {
                var actualResults = response.Dto.FirstOrDefault(table => table.QuarterName == item.QuarterName);

                Assert.IsNotNull(actualResults, $"Results for {item.QuarterName} were not found");
                Assert.AreEqual(item.DateKey, actualResults.DateKey, 
                    $"DateKey does not match for {overtimeRequest.WidgetType} and {overtimeRequest.TeamIds.FirstOrDefault()}.");
                Assert.AreEqual(item.AssessmentCount, actualResults.AssessmentCount, 
                    $"AssessmentCount does not match for {overtimeRequest.WidgetType} and {overtimeRequest.TeamIds.FirstOrDefault()}.");
                Assert.AreEqual(item.ResultPercentage, actualResults.ResultPercentage, 
                    $"ResultPercentage does not match for {overtimeRequest.WidgetType} and {overtimeRequest.TeamIds.FirstOrDefault()}.");
                Assert.AreEqual(item.BenchmarkAssessmentCount, actualResults.BenchmarkAssessmentCount, 
                    $"BenchmarkAssessmentCount does not match for {overtimeRequest.WidgetType} and {overtimeRequest.TeamIds.FirstOrDefault()}.");
                Assert.AreEqual(item.BenchmarkResultPercentage, actualResults.BenchmarkResultPercentage, 
                    $"BenchmarkResultPercentage does not match for {overtimeRequest.WidgetType} and {overtimeRequest.TeamIds.FirstOrDefault()}.");
            }
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_OvertimeItems_Post_Unauthorized_Maturity_PreCrawl()
        {
            var client = GetUnauthenticatedClient();
            var request = InsightsFactory.GetValidOvertimeRequest(WidgetType.Maturity, (int)AnalyticsMaturityTeamId.Precrawl);
            
            var response = await client.PostAsync<string>(RequestUris.AnalyticsOvertimeItems(Company.InsightsId), request);

            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status Code does not match.");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_OvertimeItems_Post_BadRequest()
        {
            var client = await GetInsightsAuthenticatedClient();
            var request = InsightsFactory.GetValidOvertimeRequest(WidgetType.FalseTest, 0);
            
            var response = await client.PostAsync<IList<string>>(RequestUris.AnalyticsOvertimeItems(Company.InsightsId), request);
            
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status Code does not match.");
            Assert.AreEqual("'Widget Type' has a range of values which does not include '4'.", response.Dto.First());
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_OvertimeItems_Post_Forbidden()
        {
            var client = await GetInsightsAuthenticatedClient();
            var request = InsightsFactory.GetValidOvertimeRequest(WidgetType.Agility, 1);
            
            var response = await client.PostAsync(RequestUris.AnalyticsOvertimeItems(Company.InsightsId), 
                request.ToStringContent());
            
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status Code does not match.");
            
        }
    }
}
