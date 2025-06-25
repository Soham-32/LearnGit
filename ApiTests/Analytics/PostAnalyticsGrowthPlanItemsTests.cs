using System.Collections.Generic;
using ApiTests.v1.Endpoints;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AtCommon.Dtos.Analytics;
using System.Threading.Tasks;
using AtCommon.Api;
using System.Linq;
using System.Net;
using AtCommon.Api.Enums;
using AtCommon.Dtos.Analytics.Custom;
using AtCommon.Dtos.Companies;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;

namespace ApiTests.Analytics
{
    //[TestClass]
    [TestCategory("Insights")]
    public class PostAnalyticsGrowthPlanItemsTests : BaseV1Test
    {
        private static bool _classInitFailed;
        private static readonly List<GrowthItem> GrowthItems = InsightsFactory.GetGrowthItemsFromExcel();
        private static TeamHierarchyResponse _team;
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
        public async Task Analytics_GrowthPlanItems_Post_TeamGis_Company_Category_NotStarted_OK()
        {
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Team, GrowthItemStatusType.NotStarted,
                0, GrowthItemSegmentType.Category);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_TeamGis_Company_Category_Started_OK()
        {
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Team, GrowthItemStatusType.Started,
                0, GrowthItemSegmentType.Category);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_TeamGis_Company_Category_StartedWithAge_OK()
        {
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Team, GrowthItemStatusType.StartedWithAge,
                0, GrowthItemSegmentType.Category);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_TeamGis_Company_Category_Finished_OK()
        {
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Team, GrowthItemStatusType.Finished,
                0, GrowthItemSegmentType.Category);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_TeamGis_Company_Category_Cancelled_OK()
        {
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Team, GrowthItemStatusType.Cancelled,
                0, GrowthItemSegmentType.Category);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_TeamGis_Company_Category_OnHold_OK()
        {
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Team, GrowthItemStatusType.OnHold,
                0, GrowthItemSegmentType.Category);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_TeamGis_Company_Category_All_OK()
        {
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Team, GrowthItemStatusType.All,
                0, GrowthItemSegmentType.Category);
            await GrowthPlanItemsResponseValidator(request);
        }
        
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_TeamGis_Company_Category_Committed_OK()
        {
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Team, GrowthItemStatusType.Committed,
                0, GrowthItemSegmentType.Category);
            await GrowthPlanItemsResponseValidator(request);
        }
        
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_TeamGis_Company_Competency_NotStarted_OK()
        {
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Team, GrowthItemStatusType.NotStarted,
                0, GrowthItemSegmentType.Competency);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_TeamGis_Company_Competency_Started_OK()
        {
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Team, GrowthItemStatusType.Started,
                0, GrowthItemSegmentType.Competency);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_TeamGis_Company_Competency_StartedWithAge_OK()
        {
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Team, GrowthItemStatusType.StartedWithAge,
                0, GrowthItemSegmentType.Competency);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_TeamGis_Company_Competency_Finished_OK()
        {
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Team, GrowthItemStatusType.Finished,
                0, GrowthItemSegmentType.Competency);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_TeamGis_Company_Competency_Cancelled_OK()
        {
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Team, GrowthItemStatusType.Cancelled,
                0, GrowthItemSegmentType.Category);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_TeamGis_Company_Competency_OnHold_OK()
        {
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Team, GrowthItemStatusType.OnHold,
                0, GrowthItemSegmentType.Category);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_TeamGis_Company_Competency_All_OK()
        {
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Team, GrowthItemStatusType.All,
                0, GrowthItemSegmentType.Competency);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_TeamGis_Company_Competency_Committed_OK()
        {
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Team, GrowthItemStatusType.Committed,
                0, GrowthItemSegmentType.Competency);
            await GrowthPlanItemsResponseValidator(request);
        }
        
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_TeamGis_Company_All_NotStarted_OK()
        {
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Team, GrowthItemStatusType.NotStarted,
                0, GrowthItemSegmentType.All);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_TeamGis_Company_All_Started_OK()
        {
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Team, GrowthItemStatusType.Started,
                0, GrowthItemSegmentType.All);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_TeamGis_Company_All_StartedWithAge_OK()
        {
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Team, GrowthItemStatusType.StartedWithAge,
                0, GrowthItemSegmentType.All);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_TeamGis_Company_All_Finished_OK()
        {
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Team, GrowthItemStatusType.Finished,
                0, GrowthItemSegmentType.All);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_TeamGis_Company_All_Cancelled_OK()
        {
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Team, GrowthItemStatusType.Cancelled,
                0, GrowthItemSegmentType.All);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_TeamGis_Company_All_OnHold_OK()
        {
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Team, GrowthItemStatusType.OnHold,
                0, GrowthItemSegmentType.All);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_TeamGis_Company_All_All_OK()
        {
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Team, GrowthItemStatusType.All,
                0, GrowthItemSegmentType.All);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_TeamGis_Company_All_Committed_OK()
        {
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Team, GrowthItemStatusType.Committed,
                0, GrowthItemSegmentType.All);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_TeamGis_Enterprise_Category_NotStarted_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Team, GrowthItemStatusType.NotStarted,
                _enterpriseTeam.TeamId, GrowthItemSegmentType.Category);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_TeamGis_Enterprise_Category_Started_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Team, GrowthItemStatusType.Started,
                _enterpriseTeam.TeamId, GrowthItemSegmentType.Category);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_TeamGis_Enterprise_Category_StartedWithAge_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Team, GrowthItemStatusType.StartedWithAge,
                _enterpriseTeam.TeamId, GrowthItemSegmentType.Category);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_TeamGis_Enterprise_Category_OnHold_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Team, GrowthItemStatusType.OnHold,
                _enterpriseTeam.TeamId, GrowthItemSegmentType.Category);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_TeamGis_Enterprise_Category_Cancelled_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Team, GrowthItemStatusType.Cancelled,
                _enterpriseTeam.TeamId, GrowthItemSegmentType.Category);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_TeamGis_Enterprise_Category_Finished_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Team, GrowthItemStatusType.Finished,
                _enterpriseTeam.TeamId, GrowthItemSegmentType.Category);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_TeamGis_Enterprise_Category_All_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Team, GrowthItemStatusType.All,
                _enterpriseTeam.TeamId, GrowthItemSegmentType.Category);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_TeamGis_Enterprise_Category_Committed_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Team, GrowthItemStatusType.Committed,
                _enterpriseTeam.TeamId, GrowthItemSegmentType.Category);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_TeamGis_Enterprise_Competency_NotStarted_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Team, GrowthItemStatusType.NotStarted,
                _enterpriseTeam.TeamId, GrowthItemSegmentType.Competency);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_TeamGis_Enterprise_Competency_Started_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Team, GrowthItemStatusType.Started,
                _enterpriseTeam.TeamId, GrowthItemSegmentType.Competency);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_TeamGis_Enterprise_Competency_StartedWithAge_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Team, GrowthItemStatusType.StartedWithAge,
                _enterpriseTeam.TeamId, GrowthItemSegmentType.Competency);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_TeamGis_Enterprise_Competency_Finished_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Team, GrowthItemStatusType.Finished,
                _enterpriseTeam.TeamId, GrowthItemSegmentType.Competency);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_TeamGis_Enterprise_Competency_OnHold_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Team, GrowthItemStatusType.OnHold,
                _enterpriseTeam.TeamId, GrowthItemSegmentType.Competency);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_TeamGis_Enterprise_Competency_Cancelled_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Team, GrowthItemStatusType.Cancelled,
                _enterpriseTeam.TeamId, GrowthItemSegmentType.Competency);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_TeamGis_Enterprise_Competency_All_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Team, GrowthItemStatusType.All,
                _enterpriseTeam.TeamId, GrowthItemSegmentType.Competency);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_TeamGis_Enterprise_Competency_Committed_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Team, GrowthItemStatusType.Committed,
                _enterpriseTeam.TeamId, GrowthItemSegmentType.Competency);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_TeamGis_Enterprise_All_NotStarted_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Team, GrowthItemStatusType.NotStarted,
                _enterpriseTeam.TeamId, GrowthItemSegmentType.All);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_TeamGis_Enterprise_All_Started_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Team, GrowthItemStatusType.Started,
                _enterpriseTeam.TeamId, GrowthItemSegmentType.All);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_TeamGis_Enterprise_All_StartedWithAge_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Team, GrowthItemStatusType.StartedWithAge,
                _enterpriseTeam.TeamId, GrowthItemSegmentType.All);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_TeamGis_Enterprise_All_Finished_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Team, GrowthItemStatusType.Finished,
                _enterpriseTeam.TeamId, GrowthItemSegmentType.All);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_TeamGis_Enterprise_All_OnHold_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Team, GrowthItemStatusType.OnHold,
                _enterpriseTeam.TeamId, GrowthItemSegmentType.All);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_TeamGis_Enterprise_All_Cancelled_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Team, GrowthItemStatusType.Cancelled,
                _enterpriseTeam.TeamId, GrowthItemSegmentType.All);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_TeamGis_Enterprise_All_All_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Team, GrowthItemStatusType.All,
                _enterpriseTeam.TeamId, GrowthItemSegmentType.All);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_TeamGis_Enterprise_All_Committed_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Team, GrowthItemStatusType.Committed,
                _enterpriseTeam.TeamId, GrowthItemSegmentType.All);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_TeamGis_MultiTeam_Category_NotStarted_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Team, GrowthItemStatusType.NotStarted,
                _multiTeam.TeamId, GrowthItemSegmentType.Category);
            var teams = _multiTeam.Children.Select(c => c.Name).ToList();
            
            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_TeamGis_MultiTeam_Category_Started_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Team, GrowthItemStatusType.Started,
                _multiTeam.TeamId, GrowthItemSegmentType.Category);
            var teams = _multiTeam.Children.Select(c => c.Name).ToList();
            
            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_TeamGis_MultiTeam_Category_StartedWithAge_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Team, GrowthItemStatusType.StartedWithAge,
                _multiTeam.TeamId, GrowthItemSegmentType.Category);
            var teams = _multiTeam.Children.Select(c => c.Name).ToList();
            
            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_TeamGis_MultiTeam_Category_Finished_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Team, GrowthItemStatusType.Finished,
                _multiTeam.TeamId, GrowthItemSegmentType.Category);
            var teams = _multiTeam.Children.Select(c => c.Name).ToList();
            
            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_TeamGis_MultiTeam_Category_Cancelled_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Team, GrowthItemStatusType.Cancelled,
                _multiTeam.TeamId, GrowthItemSegmentType.Category);
            var teams = _multiTeam.Children.Select(c => c.Name).ToList();
            
            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_TeamGis_MultiTeam_Category_OnHold_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Team, GrowthItemStatusType.OnHold,
                _multiTeam.TeamId, GrowthItemSegmentType.Category);
            var teams = _multiTeam.Children.Select(c => c.Name).ToList();
            
            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_TeamGis_MultiTeam_Category_All_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Team, GrowthItemStatusType.All,
                _multiTeam.TeamId, GrowthItemSegmentType.Category);
            var teams = _multiTeam.Children.Select(c => c.Name).ToList();
            
            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_TeamGis_MultiTeam_Category_Committed_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Team, GrowthItemStatusType.Committed,
                _multiTeam.TeamId, GrowthItemSegmentType.Category);
            var teams = _multiTeam.Children.Select(c => c.Name).ToList();
            
            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_TeamGis_MultiTeam_Competency_NotStarted_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Team, GrowthItemStatusType.NotStarted,
                _multiTeam.TeamId, GrowthItemSegmentType.Competency);
            var teams = _multiTeam.Children.Select(c => c.Name).ToList();
            
            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_TeamGis_MultiTeam_Competency_Started_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Team, GrowthItemStatusType.Started,
                _multiTeam.TeamId, GrowthItemSegmentType.Competency);
            var teams = _multiTeam.Children.Select(c => c.Name).ToList();
            
            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_TeamGis_MultiTeam_Competency_StartedWithAge_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Team, GrowthItemStatusType.StartedWithAge,
                _multiTeam.TeamId, GrowthItemSegmentType.Competency);
            var teams = _multiTeam.Children.Select(c => c.Name).ToList();

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_TeamGis_MultiTeam_Competency_Finished_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Team, GrowthItemStatusType.Finished,
                _multiTeam.TeamId, GrowthItemSegmentType.Competency);
            var teams = _multiTeam.Children.Select(c => c.Name).ToList();

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_TeamGis_MultiTeam_Competency_Cancelled_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Team, GrowthItemStatusType.Cancelled,
                _multiTeam.TeamId, GrowthItemSegmentType.Competency);
            var teams = _multiTeam.Children.Select(c => c.Name).ToList();

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_TeamGis_MultiTeam_Competency_OnHold_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Team, GrowthItemStatusType.OnHold,
                _multiTeam.TeamId, GrowthItemSegmentType.Competency);
            var teams = _multiTeam.Children.Select(c => c.Name).ToList();

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_TeamGis_MultiTeam_Competency_All_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Team, GrowthItemStatusType.All,
                _multiTeam.TeamId, GrowthItemSegmentType.Competency);
            var teams = _multiTeam.Children.Select(c => c.Name).ToList();

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_TeamGis_MultiTeam_Competency_Committed_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Team, GrowthItemStatusType.Committed,
                _multiTeam.TeamId, GrowthItemSegmentType.Competency);
            var teams = _multiTeam.Children.Select(c => c.Name).ToList();

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_TeamGis_MultiTeam_All_NotStarted_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Team, GrowthItemStatusType.NotStarted,
                _multiTeam.TeamId, GrowthItemSegmentType.All);
            var teams = _multiTeam.Children.Select(c => c.Name).ToList();
            
            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_TeamGis_MultiTeam_All_Started_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Team, GrowthItemStatusType.Started,
                _multiTeam.TeamId, GrowthItemSegmentType.All);
            var teams = _multiTeam.Children.Select(c => c.Name).ToList();
            
            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_TeamGis_MultiTeam_All_StartedWithAge_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Team, GrowthItemStatusType.StartedWithAge,
                _multiTeam.TeamId, GrowthItemSegmentType.All);
            var teams = _multiTeam.Children.Select(c => c.Name).ToList();

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_TeamGis_MultiTeam_All_Finished_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Team, GrowthItemStatusType.Finished,
                _multiTeam.TeamId, GrowthItemSegmentType.All);
            var teams = _multiTeam.Children.Select(c => c.Name).ToList();

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_TeamGis_MultiTeam_All_Cancelled_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Team, GrowthItemStatusType.Cancelled,
                _multiTeam.TeamId, GrowthItemSegmentType.All);
            var teams = _multiTeam.Children.Select(c => c.Name).ToList();

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_TeamGis_MultiTeam_All_OnHold_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Team, GrowthItemStatusType.OnHold,
                _multiTeam.TeamId, GrowthItemSegmentType.All);
            var teams = _multiTeam.Children.Select(c => c.Name).ToList();

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_TeamGis_MultiTeam_All_All_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Team, GrowthItemStatusType.All,
                _multiTeam.TeamId, GrowthItemSegmentType.All);
            var teams = _multiTeam.Children.Select(c => c.Name).ToList();

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_TeamGis_MultiTeam_All_Committed_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Team, GrowthItemStatusType.Committed,
                _multiTeam.TeamId, GrowthItemSegmentType.All);
            var teams = _multiTeam.Children.Select(c => c.Name).ToList();

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_TeamGis_Team_Category_NotStarted_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Team, GrowthItemStatusType.NotStarted,
                _team.TeamId, GrowthItemSegmentType.Category);
            var teams = new List<string> { _team.Name };
            
            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_TeamGis_Team_Category_Started_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Team, GrowthItemStatusType.Started,
                _team.TeamId, GrowthItemSegmentType.Category);
            var teams = new List<string> { _team.Name };
            
            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_TeamGis_Team_Category_StartedWithAge_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Team, GrowthItemStatusType.StartedWithAge,
                _team.TeamId, GrowthItemSegmentType.Category);
            var teams = new List<string> { _team.Name };
            
            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_TeamGis_Team_Category_Finished_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Team, GrowthItemStatusType.Finished,
                _team.TeamId, GrowthItemSegmentType.Category);
            var teams = new List<string> { _team.Name };
            
            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_TeamGis_Team_Category_Cancelled_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Team, GrowthItemStatusType.Cancelled,
                _team.TeamId, GrowthItemSegmentType.Category);
            var teams = new List<string> { _team.Name };
            
            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_TeamGis_Team_Category_OnHold_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Team, GrowthItemStatusType.OnHold,
                _team.TeamId, GrowthItemSegmentType.Category);
            var teams = new List<string> { _team.Name };
            
            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_TeamGis_Team_Category_All_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Team, GrowthItemStatusType.All,
                _team.TeamId, GrowthItemSegmentType.Category);
            var teams = new List<string> { _team.Name };
            
            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_TeamGis_Team_Category_Committed_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Team, GrowthItemStatusType.Committed,
                _team.TeamId, GrowthItemSegmentType.Category);
            var teams = new List<string> { _team.Name };
            
            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_TeamGis_Team_Competency_NotStarted_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Team, GrowthItemStatusType.NotStarted,
                _team.TeamId, GrowthItemSegmentType.Competency);
            var teams = new List<string> { _team.Name };
            
            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_TeamGis_Team_Competency_Started_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Team, GrowthItemStatusType.Started,
                _team.TeamId, GrowthItemSegmentType.Competency);
            var teams = new List<string> { _team.Name };
            
            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_TeamGis_Team_Competency_StartedWithAge_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Team, GrowthItemStatusType.StartedWithAge,
                _team.TeamId, GrowthItemSegmentType.Competency);
            var teams = new List<string> { _team.Name };

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_TeamGis_Team_Competency_Finished_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Team, GrowthItemStatusType.Finished,
                _team.TeamId, GrowthItemSegmentType.Competency);
            var teams = new List<string> { _team.Name };

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_TeamGis_Team_Competency_Cancelled_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Team, GrowthItemStatusType.Cancelled,
                _team.TeamId, GrowthItemSegmentType.Competency);
            var teams = new List<string> { _team.Name };
            
            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_TeamGis_Team_Competency_OnHold_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Team, GrowthItemStatusType.OnHold,
                _team.TeamId, GrowthItemSegmentType.Competency);
            var teams = new List<string> { _team.Name };
            
            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_TeamGis_Team_Competency_All_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Team, GrowthItemStatusType.All,
                _team.TeamId, GrowthItemSegmentType.Competency);
            var teams = new List<string> { _team.Name };

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_TeamGis_Team_Competency_Committed_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Team, GrowthItemStatusType.Committed,
                _team.TeamId, GrowthItemSegmentType.Competency);
            var teams = new List<string> { _team.Name };

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_TeamGis_Team_All_NotStarted_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Team, GrowthItemStatusType.NotStarted,
                _team.TeamId, GrowthItemSegmentType.All);
            var teams = new List<string> { _team.Name };
            
            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_TeamGis_Team_All_Started_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Team, GrowthItemStatusType.Started,
                _team.TeamId, GrowthItemSegmentType.All);
            var teams = new List<string> { _team.Name };
            
            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_TeamGis_Team_All_StartedWithAge_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Team, GrowthItemStatusType.StartedWithAge,
                _team.TeamId, GrowthItemSegmentType.All);
            var teams = new List<string> { _team.Name };

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_TeamGis_Team_All_Finished_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Team, GrowthItemStatusType.Finished,
                _team.TeamId, GrowthItemSegmentType.All);
            var teams = new List<string> { _team.Name };

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_TeamGis_Team_All_Cancelled_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Team, GrowthItemStatusType.Cancelled,
                _team.TeamId, GrowthItemSegmentType.All);
            var teams = new List<string> { _team.Name };
            
            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_TeamGis_Team_All_OnHold_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Team, GrowthItemStatusType.OnHold,
                _team.TeamId, GrowthItemSegmentType.All);
            var teams = new List<string> { _team.Name };
            
            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_TeamGis_Team_All_All_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Team, GrowthItemStatusType.All,
                _team.TeamId, GrowthItemSegmentType.All);
            var teams = new List<string> { _team.Name };

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_TeamGis_Team_All_Committed_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Team, GrowthItemStatusType.Committed,
                _team.TeamId, GrowthItemSegmentType.All);
            var teams = new List<string> { _team.Name };

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_OrgGis_Company_Category_NotStarted()
        {
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Organizational, GrowthItemStatusType.NotStarted,
                0, GrowthItemSegmentType.Category);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_OrgGis_Company_Category_Started()
        {
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Organizational, GrowthItemStatusType.Started,
                0, GrowthItemSegmentType.Category);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_OrgGis_Company_Category_StartedWithAge()
        {
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Organizational, GrowthItemStatusType.StartedWithAge,
                0, GrowthItemSegmentType.Category);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_OrgGis_Company_Category_Finished_OK()
        {
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Organizational, GrowthItemStatusType.Finished,
                0, GrowthItemSegmentType.Category);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_OrgGis_Company_Category_Cancelled_OK()
        {
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Organizational, GrowthItemStatusType.Cancelled,
                0, GrowthItemSegmentType.Category);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_OrgGis_Company_Category_OnHold_OK()
        {
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Organizational, GrowthItemStatusType.OnHold,
                0, GrowthItemSegmentType.Category);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_OrgGis_Company_Category_All_OK()
        {
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Organizational, GrowthItemStatusType.All,
                0, GrowthItemSegmentType.Category);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_OrgGis_Company_Category_Committed_OK()
        {
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Organizational, GrowthItemStatusType.Committed,
                0, GrowthItemSegmentType.Category);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_OrgGis_Company_Competency_NotStarted_OK()
        {
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Organizational, GrowthItemStatusType.NotStarted,
                0, GrowthItemSegmentType.Competency);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_OrgGis_Company_Competency_Started_OK()
        {
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Organizational, GrowthItemStatusType.Started,
                0, GrowthItemSegmentType.Competency);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_OrgGis_Company_Competency_StartedWithAge_OK()
        {
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Organizational, GrowthItemStatusType.StartedWithAge,
                0, GrowthItemSegmentType.Competency);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_OrgGis_Company_Competency_Finished_OK()
        {
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Organizational, GrowthItemStatusType.Finished,
                0, GrowthItemSegmentType.Competency);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_OrgGis_Company_Competency_Cancelled_OK()
        {
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Organizational, GrowthItemStatusType.Cancelled,
                0, GrowthItemSegmentType.Competency);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_OrgGis_Company_Competency_OnHold_OK()
        {
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Organizational, GrowthItemStatusType.OnHold,
                0, GrowthItemSegmentType.Competency);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_OrgGis_Company_Competency_All_OK()
        {
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Organizational, GrowthItemStatusType.All,
                0, GrowthItemSegmentType.Competency);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_OrgGis_Company_Competency_Committed_OK()
        {
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Organizational, GrowthItemStatusType.Committed,
                0, GrowthItemSegmentType.Competency);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_OrgGis_Enterprise_Category_NotStarted_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Organizational, GrowthItemStatusType.NotStarted,
                _enterpriseTeam.TeamId, GrowthItemSegmentType.Category);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_OrgGis_Enterprise_Category_Started_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Organizational, GrowthItemStatusType.Started,
                _enterpriseTeam.TeamId, GrowthItemSegmentType.Category);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_OrgGis_Enterprise_Category_StartedWithAge()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Organizational, GrowthItemStatusType.StartedWithAge,
                _enterpriseTeam.TeamId, GrowthItemSegmentType.Category);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_OrgGis_Enterprise_Category_Finished_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Organizational, GrowthItemStatusType.Finished,
                _enterpriseTeam.TeamId, GrowthItemSegmentType.Category);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_OrgGis_Enterprise_Category_Cancelled_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Organizational, GrowthItemStatusType.Cancelled,
                _enterpriseTeam.TeamId, GrowthItemSegmentType.Category);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_OrgGis_Enterprise_Category_OnHold_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Organizational, GrowthItemStatusType.OnHold,
                _enterpriseTeam.TeamId, GrowthItemSegmentType.Category);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_OrgGis_Enterprise_Category_All_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Organizational, GrowthItemStatusType.All,
                _enterpriseTeam.TeamId, GrowthItemSegmentType.Category);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_OrgGis_Enterprise_Category_Committed_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Organizational, GrowthItemStatusType.Committed,
                _enterpriseTeam.TeamId, GrowthItemSegmentType.Category);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_OrgGis_Enterprise_Competency_NotStarted_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Organizational, GrowthItemStatusType.NotStarted,
                _enterpriseTeam.TeamId, GrowthItemSegmentType.Competency);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_OrgGis_Enterprise_Competency_Started_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Organizational, GrowthItemStatusType.Started,
                _enterpriseTeam.TeamId, GrowthItemSegmentType.Competency);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_OrgGis_Enterprise_Competency_StartedWithAge()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Organizational, GrowthItemStatusType.StartedWithAge,
                _enterpriseTeam.TeamId, GrowthItemSegmentType.Competency);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_OrgGis_Enterprise_Competency_Finished_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Organizational, GrowthItemStatusType.Finished,
                _enterpriseTeam.TeamId, GrowthItemSegmentType.Competency);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_OrgGis_Enterprise_Competency_Cancelled_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Organizational, GrowthItemStatusType.Cancelled,
                _enterpriseTeam.TeamId, GrowthItemSegmentType.Competency);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_OrgGis_Enterprise_Competency_OnHold_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Organizational, GrowthItemStatusType.OnHold,
                _enterpriseTeam.TeamId, GrowthItemSegmentType.Competency);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_OrgGis_Enterprise_Competency_All_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Organizational, GrowthItemStatusType.All,
                _enterpriseTeam.TeamId, GrowthItemSegmentType.Competency);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_OrgGis_Enterprise_Competency_Committed_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Organizational, GrowthItemStatusType.Committed,
                _enterpriseTeam.TeamId, GrowthItemSegmentType.Competency);
            await GrowthPlanItemsResponseValidator(request);
        }

        
        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_OrgGis_Enterprise_All_NotStarted_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Organizational, GrowthItemStatusType.NotStarted,
                _enterpriseTeam.TeamId, GrowthItemSegmentType.All);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_OrgGis_Enterprise_All_Started_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Organizational, GrowthItemStatusType.Started,
                _enterpriseTeam.TeamId, GrowthItemSegmentType.All);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_OrgGis_Enterprise_All_StartedWithAge()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Organizational, GrowthItemStatusType.StartedWithAge,
                _enterpriseTeam.TeamId, GrowthItemSegmentType.All);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_OrgGis_Enterprise_All_Finished_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Organizational, GrowthItemStatusType.Finished,
                _enterpriseTeam.TeamId, GrowthItemSegmentType.All);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_OrgGis_Enterprise_All_Cancelled_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Organizational, GrowthItemStatusType.Cancelled,
                _enterpriseTeam.TeamId, GrowthItemSegmentType.All);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_OrgGis_Enterprise_All_OnHold_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Organizational, GrowthItemStatusType.OnHold,
                _enterpriseTeam.TeamId, GrowthItemSegmentType.All);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_OrgGis_Enterprise_All_All_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Organizational, GrowthItemStatusType.All,
                _enterpriseTeam.TeamId, GrowthItemSegmentType.All);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_OrgGis_Enterprise_All_Committed_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Organizational, GrowthItemStatusType.Committed,
                _enterpriseTeam.TeamId, GrowthItemSegmentType.All);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_OrgGis_MultiTeam_Category_NotStarted_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Organizational, GrowthItemStatusType.NotStarted,
                _multiTeam.TeamId, GrowthItemSegmentType.Category);
            var teams = _multiTeam.Children.Select(c => c.Name).ToList();
            teams.Add(_multiTeam.Name);

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_OrgGis_MultiTeam_Category_Started_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Organizational, GrowthItemStatusType.Started,
                _multiTeam.TeamId, GrowthItemSegmentType.Category);
            var teams = _multiTeam.Children.Select(c => c.Name).ToList();
            teams.Add(_multiTeam.Name);

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_OrgGis_MultiTeam_Category_StartedWithAge()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Organizational, GrowthItemStatusType.StartedWithAge,
                _multiTeam.TeamId, GrowthItemSegmentType.Category);
            var teams = _multiTeam.Children.Select(c => c.Name).ToList();
            teams.Add(_multiTeam.Name);

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_OrgGis_MultiTeam_Category_Finished_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Organizational, GrowthItemStatusType.Finished,
                _multiTeam.TeamId, GrowthItemSegmentType.Category);
            var teams = _multiTeam.Children.Select(c => c.Name).ToList();
            teams.Add(_multiTeam.Name);

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_OrgGis_MultiTeam_Category_Cancelled_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Organizational, GrowthItemStatusType.Cancelled,
                _multiTeam.TeamId, GrowthItemSegmentType.Category);
            var teams = _multiTeam.Children.Select(c => c.Name).ToList();
            teams.Add(_multiTeam.Name);

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_OrgGis_MultiTeam_Category_OnHold_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Organizational, GrowthItemStatusType.OnHold,
                _multiTeam.TeamId, GrowthItemSegmentType.Category);
            var teams = _multiTeam.Children.Select(c => c.Name).ToList();
            teams.Add(_multiTeam.Name);

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_OrgGis_MultiTeam_Category_All_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Organizational, GrowthItemStatusType.All,
                _multiTeam.TeamId, GrowthItemSegmentType.Category);
            var teams = _multiTeam.Children.Select(c => c.Name).ToList();
            teams.Add(_multiTeam.Name);

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_OrgGis_MultiTeam_Category_Committed_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Organizational, GrowthItemStatusType.Committed,
                _multiTeam.TeamId, GrowthItemSegmentType.Category);
            var teams = _multiTeam.Children.Select(c => c.Name).ToList();
            teams.Add(_multiTeam.Name);

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_OrgGis_MultiTeam_Competency_NotStarted_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Organizational, GrowthItemStatusType.NotStarted,
                _multiTeam.TeamId, GrowthItemSegmentType.Competency);
            var teams = _multiTeam.Children.Select(c => c.Name).ToList();
            teams.Add(_multiTeam.Name);

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_OrgGis_MultiTeam_Competency_Started_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Organizational, GrowthItemStatusType.Started,
                _multiTeam.TeamId, GrowthItemSegmentType.Competency);
            var teams = _multiTeam.Children.Select(c => c.Name).ToList();
            teams.Add(_multiTeam.Name);

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_OrgGis_MultiTeam_Competency_StartedWithAge()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Organizational, GrowthItemStatusType.StartedWithAge,
                _multiTeam.TeamId, GrowthItemSegmentType.Competency);
            var teams = _multiTeam.Children.Select(c => c.Name).ToList();
            teams.Add(_multiTeam.Name);

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_OrgGis_MultiTeam_Competency_Finished_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Organizational, GrowthItemStatusType.Finished,
                _multiTeam.TeamId, GrowthItemSegmentType.Competency);
            var teams = _multiTeam.Children.Select(c => c.Name).ToList();
            teams.Add(_multiTeam.Name);

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_OrgGis_MultiTeam_Competency_Cancelled_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Organizational, GrowthItemStatusType.Cancelled,
                _multiTeam.TeamId, GrowthItemSegmentType.Competency);
            var teams = _multiTeam.Children.Select(c => c.Name).ToList();
            teams.Add(_multiTeam.Name);

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_OrgGis_MultiTeam_Competency_OnHold_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Organizational, GrowthItemStatusType.OnHold,
                _multiTeam.TeamId, GrowthItemSegmentType.Competency);
            var teams = _multiTeam.Children.Select(c => c.Name).ToList();
            teams.Add(_multiTeam.Name);

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_OrgGis_MultiTeam_Competency_All_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Organizational, GrowthItemStatusType.All,
                _multiTeam.TeamId, GrowthItemSegmentType.Competency);
            var teams = _multiTeam.Children.Select(c => c.Name).ToList();
            teams.Add(_multiTeam.Name);

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_OrgGis_MultiTeam_Competency_Committed_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Organizational, GrowthItemStatusType.Committed,
                _multiTeam.TeamId, GrowthItemSegmentType.Competency);
            var teams = _multiTeam.Children.Select(c => c.Name).ToList();
            teams.Add(_multiTeam.Name);

            await GrowthPlanItemsResponseValidator(request, teams);
        }
        
        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_OrgGis_MultiTeam_All_NotStarted_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Organizational, GrowthItemStatusType.NotStarted,
                _multiTeam.TeamId, GrowthItemSegmentType.All);
            var teams = _multiTeam.Children.Select(c => c.Name).ToList();
            teams.Add(_multiTeam.Name);

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_OrgGis_MultiTeam_All_Started_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Organizational, GrowthItemStatusType.Started,
                _multiTeam.TeamId, GrowthItemSegmentType.All);
            var teams = _multiTeam.Children.Select(c => c.Name).ToList();
            teams.Add(_multiTeam.Name);

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_OrgGis_MultiTeam_All_StartedWithAge()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Organizational, GrowthItemStatusType.StartedWithAge,
                _multiTeam.TeamId, GrowthItemSegmentType.All);
            var teams = _multiTeam.Children.Select(c => c.Name).ToList();
            teams.Add(_multiTeam.Name);

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_OrgGis_MultiTeam_All_Finished_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Organizational, GrowthItemStatusType.Finished,
                _multiTeam.TeamId, GrowthItemSegmentType.All);
            var teams = _multiTeam.Children.Select(c => c.Name).ToList();
            teams.Add(_multiTeam.Name);

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_OrgGis_MultiTeam_All_Cancelled_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Organizational, GrowthItemStatusType.Cancelled,
                _multiTeam.TeamId, GrowthItemSegmentType.All);
            var teams = _multiTeam.Children.Select(c => c.Name).ToList();
            teams.Add(_multiTeam.Name);

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_OrgGis_MultiTeam_All_OnHold_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Organizational, GrowthItemStatusType.OnHold,
                _multiTeam.TeamId, GrowthItemSegmentType.All);
            var teams = _multiTeam.Children.Select(c => c.Name).ToList();
            teams.Add(_multiTeam.Name);

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_OrgGis_MultiTeam_All_All_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Organizational, GrowthItemStatusType.All,
                _multiTeam.TeamId, GrowthItemSegmentType.All);
            var teams = _multiTeam.Children.Select(c => c.Name).ToList();
            teams.Add(_multiTeam.Name);

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_OrgGis_MultiTeam_All_Committed_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Organizational, GrowthItemStatusType.Committed,
                _multiTeam.TeamId, GrowthItemSegmentType.All);
            var teams = _multiTeam.Children.Select(c => c.Name).ToList();
            teams.Add(_multiTeam.Name);

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_OrgGis_Team_Category_NotStarted_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Organizational, GrowthItemStatusType.NotStarted,
                _team.TeamId, GrowthItemSegmentType.Category);
            var teams = new List<string> { _team.Name };

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_OrgGis_Team_Category_Started_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Organizational, GrowthItemStatusType.Started,
                _team.TeamId, GrowthItemSegmentType.Category);
            var teams = new List<string> { _team.Name };

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_OrgGis_Team_Category_StartedWithAge()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Organizational, GrowthItemStatusType.StartedWithAge,
                _team.TeamId, GrowthItemSegmentType.Category);
            var teams = new List<string> { _team.Name };

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_OrgGis_Team_Category_Finished_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Organizational, GrowthItemStatusType.Finished,
                _team.TeamId, GrowthItemSegmentType.Category);
            var teams = new List<string> { _team.Name };

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_OrgGis_Team_Category_Cancelled_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Organizational, GrowthItemStatusType.Cancelled,
                _team.TeamId, GrowthItemSegmentType.Category);
            var teams = new List<string> { _team.Name };

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_OrgGis_Team_Category_OnHold_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Organizational, GrowthItemStatusType.OnHold,
                _team.TeamId, GrowthItemSegmentType.Category);
            var teams = new List<string> { _team.Name };

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_OrgGis_Team_Category_All_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Organizational, GrowthItemStatusType.All,
                _team.TeamId, GrowthItemSegmentType.Category);
            var teams = new List<string> { _team.Name };

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_OrgGis_Team_Category_Committed_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Organizational, GrowthItemStatusType.Committed,
                _team.TeamId, GrowthItemSegmentType.Category);
            var teams = new List<string> { _team.Name };

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_OrgGis_Team_Competency_NotStarted_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Organizational, GrowthItemStatusType.NotStarted,
                _team.TeamId, GrowthItemSegmentType.Competency);
            var teams = new List<string> { _team.Name };

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_OrgGis_Team_Competency_Started_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Organizational, GrowthItemStatusType.Started,
                _team.TeamId, GrowthItemSegmentType.Competency);
            var teams = new List<string> { _team.Name };

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_OrgGis_Team_Competency_StartedWithAge()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Organizational, GrowthItemStatusType.StartedWithAge,
                _team.TeamId, GrowthItemSegmentType.Competency);
            var teams = new List<string> { _team.Name };

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_OrgGis_Team_Competency_Finished_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Organizational, GrowthItemStatusType.Finished,
                _team.TeamId, GrowthItemSegmentType.Competency);
            var teams = new List<string> { _team.Name };

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_OrgGis_Team_Competency_Cancelled_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Organizational, GrowthItemStatusType.Cancelled,
                _team.TeamId, GrowthItemSegmentType.Competency);
            var teams = new List<string> { _team.Name };

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_OrgGis_Team_Competency_OnHold_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Organizational, GrowthItemStatusType.OnHold,
                _team.TeamId, GrowthItemSegmentType.Competency);
            var teams = new List<string> { _team.Name };

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_OrgGis_Team_Competency_All_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Organizational, GrowthItemStatusType.All,
                _team.TeamId, GrowthItemSegmentType.Competency);
            var teams = new List<string> { _team.Name };

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_OrgGis_Team_Competency_Committed_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Organizational, GrowthItemStatusType.Committed,
                _team.TeamId, GrowthItemSegmentType.Competency);
            var teams = new List<string> { _team.Name };

            await GrowthPlanItemsResponseValidator(request, teams);
        }
        
        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_OrgGis_Team_All_NotStarted_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Organizational, GrowthItemStatusType.NotStarted,
                _team.TeamId, GrowthItemSegmentType.All);
            var teams = new List<string> { _team.Name };

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_OrgGis_Team_All_Started_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Organizational, GrowthItemStatusType.Started,
                _team.TeamId, GrowthItemSegmentType.All);
            var teams = new List<string> { _team.Name };

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_OrgGis_Team_All_StartedWithAge_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Organizational, GrowthItemStatusType.StartedWithAge,
                _team.TeamId, GrowthItemSegmentType.All);
            var teams = new List<string> { _team.Name };

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_OrgGis_Team_All_Finished_OK()
        {
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Organizational, GrowthItemStatusType.Finished,
                _team.TeamId, GrowthItemSegmentType.All);
            var teams = new List<string> { _team.Name };

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_OrgGis_Team_All_Cancelled_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Organizational, GrowthItemStatusType.Cancelled,
                _team.TeamId, GrowthItemSegmentType.All);
            var teams = new List<string> { _team.Name };

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_OrgGis_Team_All_OnHold_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Organizational, GrowthItemStatusType.OnHold,
                _team.TeamId, GrowthItemSegmentType.All);
            var teams = new List<string> { _team.Name };

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_OrgGis_Team_All_All_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Organizational, GrowthItemStatusType.All,
                _team.TeamId, GrowthItemSegmentType.All);
            var teams = new List<string> { _team.Name };

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_OrgGis_Team_All_Committed_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Organizational, GrowthItemStatusType.Committed,
                _team.TeamId, GrowthItemSegmentType.All);
            var teams = new List<string> { _team.Name };

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_EnterpriseGis_Company_Category_NotStarted_OK()
        {
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Enterprise, GrowthItemStatusType.Started,
                0, GrowthItemSegmentType.Category);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_EnterpriseGis_Company_Category_Started_OK()
        {
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Enterprise, GrowthItemStatusType.Started,
                0, GrowthItemSegmentType.Category);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_EnterpriseGis_Company_Category_StartedWithAge_OK()
        {
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Enterprise, GrowthItemStatusType.StartedWithAge,
                0, GrowthItemSegmentType.Category);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_EnterpriseGis_Company_Category_Finished_OK()
        {
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Enterprise, GrowthItemStatusType.Finished,
                0, GrowthItemSegmentType.Category);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_EnterpriseGis_Company_Category_Cancelled_OK()
        {
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Enterprise, GrowthItemStatusType.Cancelled,
                0, GrowthItemSegmentType.Category);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_EnterpriseGis_Company_Category_OnHold_OK()
        {
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Enterprise, GrowthItemStatusType.OnHold,
                0, GrowthItemSegmentType.Category);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_EnterpriseGis_Company_Category_All_OK()
        {
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Enterprise, GrowthItemStatusType.All,
                0, GrowthItemSegmentType.Category);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_EnterpriseGis_Company_Category_Committed_OK()
        {
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Enterprise, GrowthItemStatusType.Committed,
                0, GrowthItemSegmentType.Category);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_EnterpriseGis_Company_Competency_NotStarted_OK()
        {
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Enterprise, GrowthItemStatusType.NotStarted,
                0, GrowthItemSegmentType.Competency);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_EnterpriseGis_Company_Competency_Started_OK()
        {
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Enterprise, GrowthItemStatusType.Started,
                0, GrowthItemSegmentType.Competency);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_EnterpriseGis_Company_Competency_StartedWithAge_OK()
        {
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Enterprise, GrowthItemStatusType.StartedWithAge,
                0, GrowthItemSegmentType.Competency);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_EnterpriseGis_Company_Competency_Finished_OK()
        {
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Enterprise, GrowthItemStatusType.Finished,
                0, GrowthItemSegmentType.Competency);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_EnterpriseGis_Company_Competency_Cancelled_OK()
        {
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Enterprise, GrowthItemStatusType.Cancelled,
                0, GrowthItemSegmentType.Competency);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_EnterpriseGis_Company_Competency_OnHold_OK()
        {
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Enterprise, GrowthItemStatusType.OnHold,
                0, GrowthItemSegmentType.Competency);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_EnterpriseGis_Company_Competency_All_OK()
        {
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Enterprise, GrowthItemStatusType.All,
                0, GrowthItemSegmentType.Competency);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_EnterpriseGis_Company_Competency_Committed_OK()
        {
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Enterprise, GrowthItemStatusType.Committed,
                0, GrowthItemSegmentType.Competency);
            await GrowthPlanItemsResponseValidator(request);
        }
        
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_EnterpriseGis_Company_All_NotStarted_OK()
        {
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Enterprise, GrowthItemStatusType.NotStarted,
                0, GrowthItemSegmentType.All);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_EnterpriseGis_Company_All_Started_OK()
        {
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Enterprise, GrowthItemStatusType.Started,
                0, GrowthItemSegmentType.All);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_EnterpriseGis_Company_All_StartedWithAge_OK()
        {
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Enterprise, GrowthItemStatusType.StartedWithAge,
                0, GrowthItemSegmentType.All);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_EnterpriseGis_Company_All_Finished_OK()
        {
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Enterprise, GrowthItemStatusType.Finished,
                0, GrowthItemSegmentType.All);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_EnterpriseGis_Company_All_Cancelled_OK()
        {
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Enterprise, GrowthItemStatusType.Cancelled,
                0, GrowthItemSegmentType.All);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_EnterpriseGis_Company_All_OnHold_OK()
        {
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Enterprise, GrowthItemStatusType.OnHold,
                0, GrowthItemSegmentType.All);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_EnterpriseGis_Company_All_All_OK()
        {
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Enterprise, GrowthItemStatusType.All,
                0, GrowthItemSegmentType.All);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_EnterpriseGis_Company_All_Committed_OK()
        {
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Enterprise, GrowthItemStatusType.Committed,
                0, GrowthItemSegmentType.All);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("BLAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_EnterpriseGis_Enterprise_Category_NotStarted_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Enterprise, GrowthItemStatusType.NotStarted,
                _enterpriseTeam.TeamId, GrowthItemSegmentType.Category);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("BLAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_EnterpriseGis_Enterprise_Category_Started_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Enterprise, GrowthItemStatusType.Started,
                _enterpriseTeam.TeamId, GrowthItemSegmentType.Category);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("BLAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_EnterpriseGis_Enterprise_Category_StartedWithAge_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Enterprise, GrowthItemStatusType.StartedWithAge,
                _enterpriseTeam.TeamId, GrowthItemSegmentType.Category);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_EnterpriseGis_Enterprise_Category_Finished_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Enterprise, GrowthItemStatusType.Finished,
                _enterpriseTeam.TeamId, GrowthItemSegmentType.Category);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_EnterpriseGis_Enterprise_Category_Cancelled_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Enterprise, GrowthItemStatusType.Cancelled,
                _enterpriseTeam.TeamId, GrowthItemSegmentType.Category);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_EnterpriseGis_Enterprise_Category_OnHold_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Enterprise, GrowthItemStatusType.OnHold,
                _enterpriseTeam.TeamId, GrowthItemSegmentType.Category);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("BLAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_EnterpriseGis_Enterprise_Category_All_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Enterprise, GrowthItemStatusType.All,
                _enterpriseTeam.TeamId, GrowthItemSegmentType.Category);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("BLAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_EnterpriseGis_Enterprise_Category_Committed_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Enterprise, GrowthItemStatusType.Committed,
                _enterpriseTeam.TeamId, GrowthItemSegmentType.Category);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("BLAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_EnterpriseGis_Enterprise_Competency_NotStarted_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Enterprise, GrowthItemStatusType.NotStarted,
                _enterpriseTeam.TeamId, GrowthItemSegmentType.Competency);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("BLAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_EnterpriseGis_Enterprise_Competency_Started_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Enterprise, GrowthItemStatusType.Started,
                _enterpriseTeam.TeamId, GrowthItemSegmentType.Competency);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("BLAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_EnterpriseGis_Enterprise_Competency_StartedWithAge_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Enterprise, GrowthItemStatusType.StartedWithAge,
                _enterpriseTeam.TeamId, GrowthItemSegmentType.Competency);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_EnterpriseGis_Enterprise_Competency_Finished_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Enterprise, GrowthItemStatusType.Finished,
                _enterpriseTeam.TeamId, GrowthItemSegmentType.Competency);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_EnterpriseGis_Enterprise_Competency_Cancelled_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Enterprise, GrowthItemStatusType.Cancelled,
                _enterpriseTeam.TeamId, GrowthItemSegmentType.Competency);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_EnterpriseGis_Enterprise_Competency_OnHold_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Enterprise, GrowthItemStatusType.OnHold,
                _enterpriseTeam.TeamId, GrowthItemSegmentType.Competency);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("BLAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_EnterpriseGis_Enterprise_Competency_All_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Enterprise, GrowthItemStatusType.All,
                _enterpriseTeam.TeamId, GrowthItemSegmentType.Competency);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("BLAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_EnterpriseGis_Enterprise_Competency_Committed_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Enterprise, GrowthItemStatusType.Committed,
                _enterpriseTeam.TeamId, GrowthItemSegmentType.Competency);
            await GrowthPlanItemsResponseValidator(request);
        }
        
        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("BLAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_EnterpriseGis_Enterprise_All_NotStarted_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Enterprise, GrowthItemStatusType.NotStarted,
                _enterpriseTeam.TeamId, GrowthItemSegmentType.All);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("BLAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_EnterpriseGis_Enterprise_All_Started_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Enterprise, GrowthItemStatusType.Started,
                _enterpriseTeam.TeamId, GrowthItemSegmentType.All);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("BLAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_EnterpriseGis_Enterprise_All_StartedWithAge_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Enterprise, GrowthItemStatusType.StartedWithAge,
                _enterpriseTeam.TeamId, GrowthItemSegmentType.All);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_EnterpriseGis_Enterprise_All_Finished_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Enterprise, GrowthItemStatusType.Finished,
                _enterpriseTeam.TeamId, GrowthItemSegmentType.All);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_EnterpriseGis_Enterprise_All_Cancelled_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Enterprise, GrowthItemStatusType.Cancelled,
                _enterpriseTeam.TeamId, GrowthItemSegmentType.All);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_EnterpriseGis_Enterprise_All_OnHold_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Enterprise, GrowthItemStatusType.OnHold,
                _enterpriseTeam.TeamId, GrowthItemSegmentType.All);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("BLAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_EnterpriseGis_Enterprise_All_All_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Enterprise, GrowthItemStatusType.All,
                _enterpriseTeam.TeamId, GrowthItemSegmentType.All);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("BLAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_EnterpriseGis_Enterprise_All_Committed_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Enterprise, GrowthItemStatusType.Committed,
                _enterpriseTeam.TeamId, GrowthItemSegmentType.All);
            await GrowthPlanItemsResponseValidator(request);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("BLAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_EnterpriseGis_MultiTeam_Category_NotStarted_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Enterprise, GrowthItemStatusType.NotStarted,
                _multiTeam.TeamId, GrowthItemSegmentType.Category);
            var teams = _multiTeam.Children.Select(c => c.Name).ToList();
            teams.Add(_multiTeam.Name);

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("BLAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_EnterpriseGis_MultiTeam_Category_Started_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Enterprise, GrowthItemStatusType.Started,
                _multiTeam.TeamId, GrowthItemSegmentType.Category);
            var teams = _multiTeam.Children.Select(c => c.Name).ToList();
            teams.Add(_multiTeam.Name);

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("BLAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_EnterpriseGis_MultiTeam_Category_StartedWithAge_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Enterprise, GrowthItemStatusType.StartedWithAge,
                _multiTeam.TeamId, GrowthItemSegmentType.Category);
            var teams = _multiTeam.Children.Select(c => c.Name).ToList();
            teams.Add(_multiTeam.Name);

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_EnterpriseGis_MultiTeam_Category_Finished_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Enterprise, GrowthItemStatusType.Finished,
                _multiTeam.TeamId, GrowthItemSegmentType.Category);
            var teams = _multiTeam.Children.Select(c => c.Name).ToList();
            teams.Add(_multiTeam.Name);

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("BLAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_EnterpriseGis_MultiTeam_Category_Cancelled_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Enterprise, GrowthItemStatusType.Cancelled,
                _multiTeam.TeamId, GrowthItemSegmentType.Category);
            var teams = _multiTeam.Children.Select(c => c.Name).ToList();
            teams.Add(_multiTeam.Name);

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("BLAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_EnterpriseGis_MultiTeam_Category_OnHold_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Enterprise, GrowthItemStatusType.OnHold,
                _multiTeam.TeamId, GrowthItemSegmentType.Category);
            var teams = _multiTeam.Children.Select(c => c.Name).ToList();
            teams.Add(_multiTeam.Name);

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("BLAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_EnterpriseGis_MultiTeam_Category_All_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Enterprise, GrowthItemStatusType.All,
                _multiTeam.TeamId, GrowthItemSegmentType.Category);
            var teams = _multiTeam.Children.Select(c => c.Name).ToList();
            teams.Add(_multiTeam.Name);

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("BLAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_EnterpriseGis_MultiTeam_Category_Committed_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Enterprise, GrowthItemStatusType.Committed,
                _multiTeam.TeamId, GrowthItemSegmentType.Category);
            var teams = _multiTeam.Children.Select(c => c.Name).ToList();
            teams.Add(_multiTeam.Name);

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("BLAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_EnterpriseGis_MultiTeam_Competency_NotStarted_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Enterprise, GrowthItemStatusType.NotStarted,
                _multiTeam.TeamId, GrowthItemSegmentType.Competency);
            var teams = _multiTeam.Children.Select(c => c.Name).ToList();
            teams.Add(_multiTeam.Name);

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("BLAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_EnterpriseGis_MultiTeam_Competency_Started_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Enterprise, GrowthItemStatusType.Started,
                _multiTeam.TeamId, GrowthItemSegmentType.Competency);
            var teams = _multiTeam.Children.Select(c => c.Name).ToList();
            teams.Add(_multiTeam.Name);

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("BLAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_EnterpriseGis_MultiTeam_Competency_StartedWithAge_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Enterprise, GrowthItemStatusType.StartedWithAge,
                _multiTeam.TeamId, GrowthItemSegmentType.Competency);
            var teams = _multiTeam.Children.Select(c => c.Name).ToList();
            teams.Add(_multiTeam.Name);

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_EnterpriseGis_MultiTeam_Competency_Finished_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Enterprise, GrowthItemStatusType.Finished,
                _multiTeam.TeamId, GrowthItemSegmentType.Competency);
            var teams = _multiTeam.Children.Select(c => c.Name).ToList();
            teams.Add(_multiTeam.Name);

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("BLAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_EnterpriseGis_MultiTeam_Competency_Cancelled_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Enterprise, GrowthItemStatusType.Cancelled,
                _multiTeam.TeamId, GrowthItemSegmentType.Competency);
            var teams = _multiTeam.Children.Select(c => c.Name).ToList();
            teams.Add(_multiTeam.Name);

            await GrowthPlanItemsResponseValidator(request, teams);
        }
        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("BLAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_EnterpriseGis_MultiTeam_Competency_OnHold_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Enterprise, GrowthItemStatusType.OnHold,
                _multiTeam.TeamId, GrowthItemSegmentType.Competency);
            var teams = _multiTeam.Children.Select(c => c.Name).ToList();
            teams.Add(_multiTeam.Name);

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("BLAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_EnterpriseGis_MultiTeam_Competency_All_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Enterprise, GrowthItemStatusType.All,
                _multiTeam.TeamId, GrowthItemSegmentType.Competency);
            var teams = _multiTeam.Children.Select(c => c.Name).ToList();
            teams.Add(_multiTeam.Name);

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("BLAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_EnterpriseGis_MultiTeam_Competency_Committed_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Enterprise, GrowthItemStatusType.Committed,
                _multiTeam.TeamId, GrowthItemSegmentType.Competency);
            var teams = _multiTeam.Children.Select(c => c.Name).ToList();
            teams.Add(_multiTeam.Name);

            await GrowthPlanItemsResponseValidator(request, teams);
        }
        
        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("BLAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_EnterpriseGis_MultiTeam_All_NotStarted_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Enterprise, GrowthItemStatusType.NotStarted,
                _multiTeam.TeamId, GrowthItemSegmentType.All);
            var teams = _multiTeam.Children.Select(c => c.Name).ToList();
            teams.Add(_multiTeam.Name);

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("BLAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_EnterpriseGis_MultiTeam_All_Started_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Enterprise, GrowthItemStatusType.Started,
                _multiTeam.TeamId, GrowthItemSegmentType.All);
            var teams = _multiTeam.Children.Select(c => c.Name).ToList();
            teams.Add(_multiTeam.Name);

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("BLAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_EnterpriseGis_MultiTeam_All_StartedWithAge_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Enterprise, GrowthItemStatusType.StartedWithAge,
                _multiTeam.TeamId, GrowthItemSegmentType.All);
            var teams = _multiTeam.Children.Select(c => c.Name).ToList();
            teams.Add(_multiTeam.Name);

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_EnterpriseGis_MultiTeam_All_Finished_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Enterprise, GrowthItemStatusType.Finished,
                _multiTeam.TeamId, GrowthItemSegmentType.All);
            var teams = _multiTeam.Children.Select(c => c.Name).ToList();
            teams.Add(_multiTeam.Name);

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("BLAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_EnterpriseGis_MultiTeam_All_Cancelled_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Enterprise, GrowthItemStatusType.Cancelled,
                _multiTeam.TeamId, GrowthItemSegmentType.All);
            var teams = _multiTeam.Children.Select(c => c.Name).ToList();
            teams.Add(_multiTeam.Name);

            await GrowthPlanItemsResponseValidator(request, teams);
        }
        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("BLAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_EnterpriseGis_MultiTeam_All_OnHold_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Enterprise, GrowthItemStatusType.OnHold,
                _multiTeam.TeamId, GrowthItemSegmentType.All);
            var teams = _multiTeam.Children.Select(c => c.Name).ToList();
            teams.Add(_multiTeam.Name);

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("BLAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_EnterpriseGis_MultiTeam_All_All_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Enterprise, GrowthItemStatusType.All,
                _multiTeam.TeamId, GrowthItemSegmentType.All);
            var teams = _multiTeam.Children.Select(c => c.Name).ToList();
            teams.Add(_multiTeam.Name);

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("BLAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_EnterpriseGis_MultiTeam_All_Committed_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Enterprise, GrowthItemStatusType.Committed,
                _multiTeam.TeamId, GrowthItemSegmentType.All);
            var teams = _multiTeam.Children.Select(c => c.Name).ToList();
            teams.Add(_multiTeam.Name);

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("BLAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_EnterpriseGis_Team_Category_NotStarted_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Enterprise, GrowthItemStatusType.NotStarted,
                _team.TeamId, GrowthItemSegmentType.Category);
            var teams = new List<string> { _team.Name };

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("BLAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_EnterpriseGis_Team_Category_Started_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Enterprise, GrowthItemStatusType.Started,
                _team.TeamId, GrowthItemSegmentType.Category);
            var teams = new List<string> { _team.Name };

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("BLAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_EnterpriseGis_Team_Category_StartedWithAge_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Enterprise, GrowthItemStatusType.StartedWithAge,
                _team.TeamId, GrowthItemSegmentType.Category);
            var teams = new List<string> { _team.Name };

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_EnterpriseGis_Team_Category_Finished_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Enterprise, GrowthItemStatusType.Finished,
                _team.TeamId, GrowthItemSegmentType.Category);
            var teams = new List<string> { _team.Name };

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_EnterpriseGis_Team_Category_Cancelled_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Enterprise, GrowthItemStatusType.Cancelled,
                _team.TeamId, GrowthItemSegmentType.Category);
            var teams = new List<string> { _team.Name };

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_EnterpriseGis_Team_Category_OnHold_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Enterprise, GrowthItemStatusType.OnHold,
                _team.TeamId, GrowthItemSegmentType.Category);
            var teams = new List<string> { _team.Name };

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("BLAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_EnterpriseGis_Team_Category_All_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Enterprise, GrowthItemStatusType.All,
                _team.TeamId, GrowthItemSegmentType.Category);
            var teams = new List<string> { _team.Name };

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("BLAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_EnterpriseGis_Team_Category_Committed_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Enterprise, GrowthItemStatusType.Committed,
                _team.TeamId, GrowthItemSegmentType.Category);
            var teams = new List<string> { _team.Name };

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("BLAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_EnterpriseGis_Team_Competency_NotStarted_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Enterprise, GrowthItemStatusType.NotStarted,
                _team.TeamId, GrowthItemSegmentType.Competency);
            var teams = new List<string> { _team.Name };

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("BLAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_EnterpriseGis_Team_Competency_Started_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Enterprise, GrowthItemStatusType.Started,
                _team.TeamId, GrowthItemSegmentType.Competency);
            var teams = new List<string> { _team.Name };

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("BLAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_EnterpriseGis_Team_Competency_StartedWithAge_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Enterprise, GrowthItemStatusType.StartedWithAge,
                _team.TeamId, GrowthItemSegmentType.Competency);
            var teams = new List<string> { _team.Name };

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_EnterpriseGis_Team_Competency_Finished_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Enterprise, GrowthItemStatusType.Finished,
                _team.TeamId, GrowthItemSegmentType.Competency);
            var teams = new List<string> { _team.Name };

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_EnterpriseGis_Team_Competency_Cancelled_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Enterprise, GrowthItemStatusType.Cancelled,
                _team.TeamId, GrowthItemSegmentType.Competency);
            var teams = new List<string> { _team.Name };

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_EnterpriseGis_Team_Competency_OnHold_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Enterprise, GrowthItemStatusType.OnHold,
                _team.TeamId, GrowthItemSegmentType.Competency);
            var teams = new List<string> { _team.Name };

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("BLAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_EnterpriseGis_Team_Competency_All_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Enterprise, GrowthItemStatusType.All,
                _team.TeamId, GrowthItemSegmentType.Competency);
            var teams = new List<string> { _team.Name };

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("BLAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_EnterpriseGis_Team_Competency_Committed_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Enterprise, GrowthItemStatusType.Committed,
                _team.TeamId, GrowthItemSegmentType.Competency);
            var teams = new List<string> { _team.Name };

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("BLAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_EnterpriseGis_Team_All_NotStarted_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Enterprise, GrowthItemStatusType.NotStarted,
                _team.TeamId, GrowthItemSegmentType.All);
            var teams = new List<string> { _team.Name };

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("BLAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_EnterpriseGis_Team_All_Started_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Enterprise, GrowthItemStatusType.Started,
                _team.TeamId, GrowthItemSegmentType.All);
            var teams = new List<string> { _team.Name };

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("BLAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_EnterpriseGis_Team_All_StartedWithAge_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Enterprise, GrowthItemStatusType.StartedWithAge,
                _team.TeamId, GrowthItemSegmentType.All);
            var teams = new List<string> { _team.Name };

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_EnterpriseGis_Team_All_Finished_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Enterprise, GrowthItemStatusType.Finished,
                _team.TeamId, GrowthItemSegmentType.All);
            var teams = new List<string> { _team.Name };

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_EnterpriseGis_Team_All_Cancelled_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Enterprise, GrowthItemStatusType.Cancelled,
                _team.TeamId, GrowthItemSegmentType.All);
            var teams = new List<string> { _team.Name };

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_EnterpriseGis_Team_All_OnHold_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Enterprise, GrowthItemStatusType.OnHold,
                _team.TeamId, GrowthItemSegmentType.All);
            var teams = new List<string> { _team.Name };

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("BLAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_EnterpriseGis_Team_All_All_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Enterprise, GrowthItemStatusType.All,
                _team.TeamId, GrowthItemSegmentType.All);
            var teams = new List<string> { _team.Name };

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("BLAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_EnterpriseGis_Team_All_Committed_OK()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Enterprise, GrowthItemStatusType.Committed,
                _team.TeamId, GrowthItemSegmentType.All);
            var teams = new List<string> { _team.Name };

            await GrowthPlanItemsResponseValidator(request, teams);
        }

        public async Task GrowthPlanItemsResponseValidator(GrowthPlanAnalyticsRequest request, IList<string> teamNames = null)
        {
            
            var expectedResponse = InsightsFactory.GetGiTypesAndCounts(GrowthItems, request.GrowthItemCategory,
                request.GrowthItemStatusType, request.SubDimensionId, teamNames);

            var client = await GetInsightsAuthenticatedClient();
            var response = await client.PostAsync<IList<GrowthPlanAnalyticsResponse>>(
                RequestUris.AnalyticsGrowthPlanItems(Company.InsightsId), request);
            if (expectedResponse.Count == 0)
                Assert.AreEqual(expectedResponse.Count, response.Dto.Count, $"response count does not match for {request.GrowthItemCategory}, {request.GrowthItemStatusType}, and {request.GrowthItemStatusType}");
            
            foreach (var item in expectedResponse)
            {
                var group = item.Key == "" ? "Not Selected" : item.Key;
                var tableResponse = response.Dto.Where(table => table.Group == group)
                    .CheckForNull($"<{group}> was not found in the response table.");
                Assert.AreEqual(item.Value, tableResponse.Sum(g => g.GrowthItemCount), $"GrowthItemCount does not match for {group}.");

            }
        }

        //Unauthorized Tests
        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_Unauthorized()
        {
            var growthPlanItem = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Team, GrowthItemStatusType.Started,
                0, GrowthItemSegmentType.Category);
            var client = GetUnauthenticatedClient();
            var response = await client.PostAsync(RequestUris.AnalyticsGrowthPlanItems(Company.InsightsId), 
                growthPlanItem.ToStringContent());
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, 
                "Status Code does not match.");           
        }

        //Bad Request Tests
        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_BadRequest()
        {
            var growthPlanItem = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Individual, GrowthItemStatusType.All,
                0, GrowthItemSegmentType.Category);
            var client = await GetInsightsAuthenticatedClient();
            var response = await client.PostAsync<IList<string>>(
                RequestUris.AnalyticsGrowthPlanItems(Company.InsightsId), growthPlanItem);
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, 
                "Status Code does not match.");
            Assert.AreEqual("'Growth Item Category' has a range of values which does not include '4'.", 
                response.Dto.FirstOrDefault(), "Error message does not match.");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItems_Post_Forbidden()
        {
            var growthPlanItem = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Team, GrowthItemStatusType.All,
                0, GrowthItemSegmentType.Category);
            var client = await GetInsightsAuthenticatedClient();
            var response = await client.PostAsync<IList<string>>(
                RequestUris.AnalyticsGrowthPlanItems(SharedConstants.FakeCompanyId), growthPlanItem);
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, 
                "Status Code does not match.");
        }

    }

}
