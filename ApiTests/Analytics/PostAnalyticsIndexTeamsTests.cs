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
    public class PostAnalyticsIndexTeamsTests : BaseV1Test
    {
        private static bool _classInitFailed;
        private static TeamHierarchyResponse _multiTeam1;
        private static TeamHierarchyResponse _multiTeam2;
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
                _multiTeam1 = companyHierarchy.GetTeamByName(SharedConstants.InsightsMultiTeam1);
                _multiTeam2 = companyHierarchy.GetTeamByName(SharedConstants.InsightsMultiTeam2);
            }
            catch (System.Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_IndexTeams_Post_Company_Maturity_PreCrawl_OK()
        {
            const Stage stage = Stage.PreCrawl;
            var indexTeamRequest = InsightsFactory.GetIndexTeamRequest(WidgetType.Maturity, stage, 0);
            var expectedResponse = InsightsFactory.GetPagedIndexTeamResponse(stage);

            await IndexTeamsResponseValidator(indexTeamRequest, expectedResponse);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_IndexTeams_Post_Company_Performance_PreCrawl_OK()
        {
            const Stage stage = Stage.PreCrawl;
            var indexTeamRequest = InsightsFactory.GetIndexTeamRequest(WidgetType.Performance, stage, 0);
            var expectedResponse = InsightsFactory.GetPagedIndexTeamResponse(stage);

            await IndexTeamsResponseValidator(indexTeamRequest, expectedResponse);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_IndexTeams_Post_Company_Maturity_Crawl_OK()
        {
            const Stage stage = Stage.Crawl;
            var indexTeamRequest = InsightsFactory.GetIndexTeamRequest(WidgetType.Maturity, stage, 0);
            var expectedResponse = InsightsFactory.GetEmptyPagedIndexTeamResponse();

            await IndexTeamsResponseValidator(indexTeamRequest, expectedResponse);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_IndexTeams_Post_Company_Performance_Crawl_OK()
        {
            const Stage stage = Stage.Crawl;
            var indexTeamRequest = InsightsFactory.GetIndexTeamRequest(WidgetType.Performance, stage, 0);
            var expectedResponse = InsightsFactory.GetEmptyPagedIndexTeamResponse();

            await IndexTeamsResponseValidator(indexTeamRequest, expectedResponse);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_IndexTeams_Post_Company_Maturity_Walk_OK()
        {
            const Stage stage = Stage.Walk;
            var indexTeamRequest = InsightsFactory.GetIndexTeamRequest(WidgetType.Maturity, stage, 0);
            var expectedResponse = InsightsFactory.GetPagedIndexTeamResponse(stage);

            await IndexTeamsResponseValidator(indexTeamRequest, expectedResponse);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_IndexTeams_Post_Company_Performance_Walk_OK()
        {
            const Stage stage = Stage.Walk;
            var indexTeamRequest = InsightsFactory.GetIndexTeamRequest(WidgetType.Performance, stage, 0);
            var expectedResponse = InsightsFactory.GetPagedIndexTeamResponse(stage);
            
            await IndexTeamsResponseValidator(indexTeamRequest, expectedResponse);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_IndexTeams_Post_Company_Maturity_Run_OK()
        {
            const Stage stage = Stage.Run;
            var indexTeamRequest = InsightsFactory.GetIndexTeamRequest(WidgetType.Maturity, stage, 0);
            var expectedResponse = InsightsFactory.GetPagedIndexTeamResponse(stage);           

            await IndexTeamsResponseValidator(indexTeamRequest, expectedResponse);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_IndexTeams_Post_Company_Performance_Run_OK()
        {
            const Stage stage = Stage.Run;
            var indexTeamRequest = InsightsFactory.GetIndexTeamRequest(WidgetType.Performance, stage, 0);
            var expectedResponse = InsightsFactory.GetPagedIndexTeamResponse(stage);
           
            await IndexTeamsResponseValidator(indexTeamRequest, expectedResponse);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_IndexTeams_Post_Company_Maturity_Fly_OK()
        {
            const Stage stage = Stage.Fly;
            var indexTeamRequest = InsightsFactory.GetIndexTeamRequest(WidgetType.Maturity, stage, 0);
            var expectedResponse = InsightsFactory.GetPagedIndexTeamResponse(stage);
 
            await IndexTeamsResponseValidator(indexTeamRequest, expectedResponse);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_IndexTeams_Post_Company_Performance_Fly_OK()
        {
            const Stage stage = Stage.Fly;
            var indexTeamRequest = InsightsFactory.GetIndexTeamRequest(WidgetType.Performance, stage, 0);
            var expectedResponse = InsightsFactory.GetPagedIndexTeamResponse(stage);

            await IndexTeamsResponseValidator(indexTeamRequest, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_IndexTeams_Post_Enterprise_Maturity_PreCrawl_OK()
        {
            VerifySetup(_classInitFailed);
            const Stage stage = Stage.PreCrawl;
            var indexTeamRequest = InsightsFactory.GetIndexTeamRequest(WidgetType.Maturity, stage, _enterpriseTeam.TeamId);
            var expectedResponse = InsightsFactory.GetPagedIndexTeamResponse(stage);

            await IndexTeamsResponseValidator(indexTeamRequest, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_IndexTeams_Post_Enterprise_Performance_PreCrawl_OK()
        {
            VerifySetup(_classInitFailed);
            const Stage stage = Stage.PreCrawl;
            var indexTeamRequest = InsightsFactory.GetIndexTeamRequest(WidgetType.Performance, stage, _enterpriseTeam.TeamId);
            var expectedResponse = InsightsFactory.GetPagedIndexTeamResponse(stage);

            await IndexTeamsResponseValidator(indexTeamRequest, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_IndexTeams_Post_Enterprise_Maturity_Crawl_OK()
        {
            VerifySetup(_classInitFailed);
            const Stage stage = Stage.Crawl;
            var indexTeamRequest = InsightsFactory.GetIndexTeamRequest(WidgetType.Maturity, stage, _enterpriseTeam.TeamId);
            var expectedResponse = InsightsFactory.GetEmptyPagedIndexTeamResponse();

            await IndexTeamsResponseValidator(indexTeamRequest, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_IndexTeams_Post_Enterprise_Performance_Crawl_OK()
        {
            VerifySetup(_classInitFailed);
            const Stage stage = Stage.Crawl;
            var indexTeamRequest = InsightsFactory.GetIndexTeamRequest(WidgetType.Performance, stage, _enterpriseTeam.TeamId);
            var expectedResponse = InsightsFactory.GetEmptyPagedIndexTeamResponse();

            await IndexTeamsResponseValidator(indexTeamRequest, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_IndexTeams_Post_Enterprise_Maturity_Walk_OK()
        {
            VerifySetup(_classInitFailed);
            const Stage stage = Stage.Walk;
            var indexTeamRequest = InsightsFactory.GetIndexTeamRequest(WidgetType.Maturity, stage, _enterpriseTeam.TeamId);
            var expectedResponse = InsightsFactory.GetPagedIndexTeamResponse(stage);

            await IndexTeamsResponseValidator(indexTeamRequest, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_IndexTeams_Post_Enterprise_Performance_Walk_OK()
        {
            VerifySetup(_classInitFailed);
            const Stage stage = Stage.Walk;
            var indexTeamRequest = InsightsFactory.GetIndexTeamRequest(WidgetType.Performance, stage, _enterpriseTeam.TeamId);
            var expectedResponse = InsightsFactory.GetPagedIndexTeamResponse(stage);
            
            await IndexTeamsResponseValidator(indexTeamRequest, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_IndexTeams_Post_Enterprise_Maturity_Run_OK()
        {
            VerifySetup(_classInitFailed);
            const Stage stage = Stage.Run;
            var indexTeamRequest = InsightsFactory.GetIndexTeamRequest(WidgetType.Maturity, stage, _enterpriseTeam.TeamId);
            var expectedResponse = InsightsFactory.GetPagedIndexTeamResponse(stage);           

            await IndexTeamsResponseValidator(indexTeamRequest, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_IndexTeams_Post_Enterprise_Performance_Run_OK()
        {
            VerifySetup(_classInitFailed);
            const Stage stage = Stage.Run;
            var indexTeamRequest = InsightsFactory.GetIndexTeamRequest(WidgetType.Performance, stage, _enterpriseTeam.TeamId);
            var expectedResponse = InsightsFactory.GetPagedIndexTeamResponse(stage);
           
            await IndexTeamsResponseValidator(indexTeamRequest, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_IndexTeams_Post_Enterprise_Maturity_Fly_OK()
        {
            VerifySetup(_classInitFailed);
            const Stage stage = Stage.Fly;
            var indexTeamRequest = InsightsFactory.GetIndexTeamRequest(WidgetType.Maturity, stage, _enterpriseTeam.TeamId);
            var expectedResponse = InsightsFactory.GetPagedIndexTeamResponse(stage);
 
            await IndexTeamsResponseValidator(indexTeamRequest, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_IndexTeams_Post_Enterprise_Performance_Fly_OK()
        {
            VerifySetup(_classInitFailed);
            const Stage stage = Stage.Fly;
            var indexTeamRequest = InsightsFactory.GetIndexTeamRequest(WidgetType.Performance, stage, _enterpriseTeam.TeamId);
            var expectedResponse = InsightsFactory.GetPagedIndexTeamResponse(stage);

            await IndexTeamsResponseValidator(indexTeamRequest, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_IndexTeams_Post_MultiTeam_Maturity_PreCrawl_OK()
        {
            VerifySetup(_classInitFailed);
            const Stage stage = Stage.PreCrawl;
            var indexTeamRequest = InsightsFactory.GetIndexTeamRequest(WidgetType.Maturity, stage, _multiTeam1.TeamId);
            var expectedResponse = InsightsFactory.GetPagedIndexTeamResponse(stage);

            await IndexTeamsResponseValidator(indexTeamRequest, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_IndexTeams_Post_MultiTeam_Performance_PreCrawl_OK()
        {
            VerifySetup(_classInitFailed);
            const Stage stage = Stage.PreCrawl;
            var indexTeamRequest = InsightsFactory.GetIndexTeamRequest(WidgetType.Performance, stage, _multiTeam1.TeamId);
            var expectedResponse = InsightsFactory.GetPagedIndexTeamResponse(stage);

            await IndexTeamsResponseValidator(indexTeamRequest, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_IndexTeams_Post_MultiTeam_Maturity_Crawl_OK()
        {
            VerifySetup(_classInitFailed);
            const Stage stage = Stage.Crawl;
            var indexTeamRequest = InsightsFactory.GetIndexTeamRequest(WidgetType.Maturity, stage, _multiTeam1.TeamId);
            var expectedResponse = InsightsFactory.GetEmptyPagedIndexTeamResponse();

            await IndexTeamsResponseValidator(indexTeamRequest, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_IndexTeams_Post_MultiTeam_Performance_Crawl_OK()
        {
            VerifySetup(_classInitFailed);
            const Stage stage = Stage.Crawl;
            var indexTeamRequest = InsightsFactory.GetIndexTeamRequest(WidgetType.Performance, stage, _multiTeam1.TeamId);
            var expectedResponse = InsightsFactory.GetEmptyPagedIndexTeamResponse();

            await IndexTeamsResponseValidator(indexTeamRequest, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_IndexTeams_Post_MultiTeam_Maturity_Walk_OK()
        {
            VerifySetup(_classInitFailed);
            const Stage stage = Stage.Walk;
            var indexTeamRequest = InsightsFactory.GetIndexTeamRequest(WidgetType.Maturity, stage, _multiTeam1.TeamId);
            var expectedResponse = InsightsFactory.GetPagedIndexTeamResponse(stage);

            await IndexTeamsResponseValidator(indexTeamRequest, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_IndexTeams_Post_MultiTeam_Performance_Walk_OK()
        {
            VerifySetup(_classInitFailed);
            const Stage stage = Stage.Walk;
            var indexTeamRequest = InsightsFactory.GetIndexTeamRequest(WidgetType.Performance, stage, _multiTeam1.TeamId);
            var expectedResponse = InsightsFactory.GetPagedIndexTeamResponse(stage);
            
            await IndexTeamsResponseValidator(indexTeamRequest, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_IndexTeams_Post_MultiTeam_Maturity_Run_OK()
        {
            VerifySetup(_classInitFailed);
            const Stage stage = Stage.Run;
            var indexTeamRequest = InsightsFactory.GetIndexTeamRequest(WidgetType.Maturity, stage, _multiTeam2.TeamId);
            var expectedResponse = InsightsFactory.GetPagedIndexTeamResponse(stage);           

            await IndexTeamsResponseValidator(indexTeamRequest, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_IndexTeams_Post_MultiTeam_Performance_Run_OK()
        {
            VerifySetup(_classInitFailed);
            const Stage stage = Stage.Run;
            var indexTeamRequest = InsightsFactory.GetIndexTeamRequest(WidgetType.Performance, stage, _multiTeam2.TeamId);
            var expectedResponse = InsightsFactory.GetPagedIndexTeamResponse(stage);
           
            await IndexTeamsResponseValidator(indexTeamRequest, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_IndexTeams_Post_MultiTeam_Maturity_Fly_OK()
        {
            VerifySetup(_classInitFailed);
            const Stage stage = Stage.Fly;
            var indexTeamRequest = InsightsFactory.GetIndexTeamRequest(WidgetType.Maturity, stage, _multiTeam2.TeamId);
            var expectedResponse = InsightsFactory.GetPagedIndexTeamResponse(stage);
 
            await IndexTeamsResponseValidator(indexTeamRequest, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_IndexTeams_Post_MultiTeam_Performance_Fly_OK()
        {
            VerifySetup(_classInitFailed);
            const Stage stage = Stage.Fly;
            var indexTeamRequest = InsightsFactory.GetIndexTeamRequest(WidgetType.Performance, stage, _multiTeam2.TeamId);
            var expectedResponse = InsightsFactory.GetPagedIndexTeamResponse(stage);

            await IndexTeamsResponseValidator(indexTeamRequest, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_IndexTeams_Post_Team_Maturity_PreCrawl_OK()
        {
            VerifySetup(_classInitFailed);
            const Stage stage = Stage.PreCrawl;
            var team = InsightsFactory.GetIndexTeamByMaturity(stage).CheckForNull($"IndexTeam not found for <{stage:G}>");
            var indexTeamRequest = InsightsFactory.GetIndexTeamRequest(WidgetType.Maturity, stage, team.TeamId);
            var expectedResponse = InsightsFactory.GetPagedIndexTeamResponse(stage);

            await IndexTeamsResponseValidator(indexTeamRequest, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_IndexTeams_Post_Team_Performance_PreCrawl_OK()
        {
            VerifySetup(_classInitFailed);
            const Stage stage = Stage.PreCrawl;
            var team = InsightsFactory.GetIndexTeamByMaturity(stage).CheckForNull($"IndexTeam not found for <{stage:G}>");
            var indexTeamRequest = InsightsFactory.GetIndexTeamRequest(WidgetType.Performance, stage, team.TeamId);
            var expectedResponse = InsightsFactory.GetPagedIndexTeamResponse(stage);

            await IndexTeamsResponseValidator(indexTeamRequest, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_IndexTeams_Post_Team_Maturity_Walk_OK()
        {
            VerifySetup(_classInitFailed);
            const Stage stage = Stage.Walk;
            var team = InsightsFactory.GetIndexTeamByMaturity(stage).CheckForNull($"Team not found for <{stage:G}>");
            var indexTeamRequest = InsightsFactory.GetIndexTeamRequest(WidgetType.Maturity, stage, team.TeamId);
            var expectedResponse = InsightsFactory.GetPagedIndexTeamResponse(stage);

            await IndexTeamsResponseValidator(indexTeamRequest, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_IndexTeams_Post_Team_Performance_Walk_OK()
        {
            VerifySetup(_classInitFailed);
            const Stage stage = Stage.Walk;
            var team = InsightsFactory.GetIndexTeamByMaturity(stage).CheckForNull($"IndexTeam not found for <{stage:G}>");
            var indexTeamRequest = InsightsFactory.GetIndexTeamRequest(WidgetType.Performance, stage, team.TeamId);
            var expectedResponse = InsightsFactory.GetPagedIndexTeamResponse(stage);
            
            await IndexTeamsResponseValidator(indexTeamRequest, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_IndexTeams_Post_Team_Maturity_Run_OK()
        {
            VerifySetup(_classInitFailed);
            const Stage stage = Stage.Run;
            var team = InsightsFactory.GetIndexTeamByMaturity(stage).CheckForNull($"IndexTeam not found for <{stage:G}>");
            var indexTeamRequest = InsightsFactory.GetIndexTeamRequest(WidgetType.Maturity, stage, team.TeamId);
            var expectedResponse = InsightsFactory.GetPagedIndexTeamResponse(stage);           

            await IndexTeamsResponseValidator(indexTeamRequest, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_IndexTeams_Post_Team_Performance_Run_OK()
        {
            VerifySetup(_classInitFailed);
            const Stage stage = Stage.Run;
            var team = InsightsFactory.GetIndexTeamByMaturity(stage).CheckForNull($"IndexTeam not found for <{stage:G}>");
            var indexTeamRequest = InsightsFactory.GetIndexTeamRequest(WidgetType.Performance, stage, team.TeamId);
            var expectedResponse = InsightsFactory.GetPagedIndexTeamResponse(stage);
           
            await IndexTeamsResponseValidator(indexTeamRequest, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_IndexTeams_Post_Team_Maturity_Fly_OK()
        {
            VerifySetup(_classInitFailed);
            const Stage stage = Stage.Fly;
            var team = InsightsFactory.GetIndexTeamByMaturity(stage).CheckForNull($"IndexTeam not found for <{stage:G}>");
            var indexTeamRequest = InsightsFactory.GetIndexTeamRequest(WidgetType.Maturity, stage, team.TeamId);
            var expectedResponse = InsightsFactory.GetPagedIndexTeamResponse(stage);
 
            await IndexTeamsResponseValidator(indexTeamRequest, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_IndexTeams_Post_Team_Performance_Fly_OK()
        {
            VerifySetup(_classInitFailed);
            const Stage stage = Stage.Fly;
            var team = InsightsFactory.GetIndexTeamByMaturity(stage).CheckForNull($"IndexTeam not found for <{stage:G}>");
            var indexTeamRequest = InsightsFactory.GetIndexTeamRequest(WidgetType.Performance, stage, team.TeamId);
            var expectedResponse = InsightsFactory.GetPagedIndexTeamResponse(stage);

            await IndexTeamsResponseValidator(indexTeamRequest, expectedResponse);
        }

        public async Task IndexTeamsResponseValidator(IndexTeamRequest indexTeams, PagedResponse<IndexTeamResponse> expected)
        {
            var client = await GetInsightsAuthenticatedClient();
            var response = await client.PostAsync<PagedResponse<IndexTeamResponse>>(
                RequestUris.AnalyticsIndexTeams(Company.InsightsId), indexTeams);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status code does not match.");
            Assert.AreEqual(expected.CurrentPage, response.Dto.CurrentPage, 
                $"CurrentPage does not match for {indexTeams.WidgetType} and {indexTeams.Stage}");
            Assert.AreEqual(expected.PageCount, response.Dto.PageCount, 
                $"PageCount does not match for {indexTeams.WidgetType} and {indexTeams.Stage}");
            Assert.AreEqual(expected.PageSize, response.Dto.PageSize, 
                $"PageSize does not match for {indexTeams.WidgetType} and {indexTeams.Stage}");
            Assert.AreEqual(expected.RowCount, response.Dto.RowCount, 
                $"RowCount does not match for {indexTeams.WidgetType} and {indexTeams.Stage}");
            Assert.AreEqual(expected.FirstRowOnPage, response.Dto.FirstRowOnPage, 
                $"FirstRowOnPage does not match for {indexTeams.WidgetType} and {indexTeams.Stage}");
            Assert.AreEqual(expected.LastRowOnPage, response.Dto.LastRowOnPage, 
                $"LastRowOnPage does not match for {indexTeams.WidgetType} and {indexTeams.Stage}");
            Assert.AreEqual(expected.Results.Count, response.Dto.Results.Count, 
                $"Results.Count does not match for {indexTeams.WidgetType} and {indexTeams.Stage}");

            foreach (var result in expected.Results)
            {
                var actualResults = response.Dto.Results.FirstOrDefault(table => table.Name == result.Name);

                Assert.IsNotNull(actualResults, $"Results with {result.Name} were not found");
                Assert.AreEqual(result.WorkType, actualResults.WorkType, 
                    $"WorkType does not match for {indexTeams.WidgetType} and {indexTeams.Stage}");
                Assert.AreEqual(result.Email, actualResults.Email, 
                    $"Email does not match for {indexTeams.WidgetType} and {indexTeams.Stage}");
                Assert.AreEqual(result.TeamMemberContact, actualResults.TeamMemberContact, 
                    $"TeamMemberContact does not match for {indexTeams.WidgetType} and {indexTeams.Stage}");

            }
        }

        [TestMethod]
        [TestCategory("Avengers")]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_IndexTeams_Post_Unauthorized()
        {
            var indexTeams = InsightsFactory.GetIndexTeamRequest(WidgetType.Maturity, Stage.PreCrawl, (int)AnalyticsMaturityTeamId.Precrawl);

            var client = GetUnauthenticatedClient();
            var response = await client.PostAsync<string>(RequestUris.AnalyticsIndexTeams(Company.InsightsId), indexTeams);
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode,
                "Status Code does not match Unauthorized code.");
        }

        
        [TestMethod]
        [TestCategory("Avengers")]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_IndexTeams_Post_BadRequest()
        {
            var indexTeams = InsightsFactory.GetIndexTeamRequest(WidgetType.FalseTest, Stage.FalseTest, (int)AnalyticsMaturityTeamId.Precrawl);

            var client = await GetInsightsAuthenticatedClient();
            var response = await client.PostAsync<IList<string>>(RequestUris.AnalyticsIndexTeams(Company.InsightsId), 
                indexTeams);
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, 
                "Status Code does not match Bad Request code.");
            Assert.AreEqual("'Widget Type' has a range of values which does not include '4'.", response.Dto.First(),
                "Widget Type value does not match.");
            Assert.AreEqual("'Stage' has a range of values which does not include '6'.", response.Dto.Last(),
                "Stage value does not match.");
        }
    }
}