using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ApiTests.v1.Endpoints;
using AtCommon.Api;
using AtCommon.Api.Enums;
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
    public class GrowthPlanItemsDetailsTests : BaseV1Test
    {
        private static bool _classInitFailed;
        private static readonly List<GrowthItem> GrowthItems = InsightsFactory.GetGrowthItemsFromExcel();
        private static TeamHierarchyResponse _team;
        private static TeamHierarchyResponse _multiTeam;
        private static TeamHierarchyResponse _enterpriseTeam;
        private static CompanyHierarchyResponse _companyHierarchy;

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            try
            {
                var setup = new SetupTeardownApi(TestEnvironment);
                var user = User.IsSiteAdmin() || User.IsPartnerAdmin() ? User : InsightsUser;
                _companyHierarchy = setup.GetCompanyHierarchy(Company.InsightsId, user);

                _enterpriseTeam = _companyHierarchy.GetTeamByName(SharedConstants.InsightsEnterpriseTeam1);
                _multiTeam = _companyHierarchy.GetTeamByName(SharedConstants.InsightsMultiTeam1);
                _team = _companyHierarchy.GetTeamByName(SharedConstants.InsightsIndividualTeam1);
            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }

        // 200-201
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_TeamGis_Company_NotStarted_Category_OK()
        {
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Team, GrowthItemStatusType.NotStarted, 
                GrowthItemDetailStatusType.InBacklog, GrowthItemSegmentType.Category, 0);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_TeamGis_Company_NotStarted_Competency_OK()
        {
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Team, GrowthItemStatusType.NotStarted, 
                GrowthItemDetailStatusType.InBacklog, GrowthItemSegmentType.Competency, 0);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_TeamGis_Company_NotStarted_All_OK()
        {
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Team, GrowthItemStatusType.NotStarted, 
                GrowthItemDetailStatusType.InBacklog, GrowthItemSegmentType.All, 0);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_TeamGis_Company_Started_Category_OK()
        {
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Team, GrowthItemStatusType.Started, 
                GrowthItemDetailStatusType.Started, GrowthItemSegmentType.Category, 0);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_TeamGis_Company_Started_Competency_OK()
        {
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Team, GrowthItemStatusType.Started, 
                GrowthItemDetailStatusType.Started, GrowthItemSegmentType.Competency, 0);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_TeamGis_Company_Started_All_OK()
        {
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Team, GrowthItemStatusType.Started, 
                GrowthItemDetailStatusType.Started, GrowthItemSegmentType.All, 0);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_TeamGis_Company_StartedWithAge_Category_OK()
        {
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Team, GrowthItemStatusType.StartedWithAge, 
                GrowthItemDetailStatusType.StartedMore3, GrowthItemSegmentType.Category, 0);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_TeamGis_Company_StartedWithAge_Competency_OK()
        {
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Team, GrowthItemStatusType.StartedWithAge, 
                GrowthItemDetailStatusType.StartedMore3, GrowthItemSegmentType.Competency, 0);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_TeamGis_Company_StartedWithAge_All_OK()
        {
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Team, GrowthItemStatusType.StartedWithAge, 
                GrowthItemDetailStatusType.StartedMore3, GrowthItemSegmentType.All, 0);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_TeamGis_Company_Finished_Category_OK()
        {
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Team, GrowthItemStatusType.Finished, 
                GrowthItemDetailStatusType.Finished, GrowthItemSegmentType.Category, 0);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_TeamGis_Company_Finished_Competency_OK()
        {
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Team, GrowthItemStatusType.Finished, 
                GrowthItemDetailStatusType.Finished, GrowthItemSegmentType.Competency, 0);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_TeamGis_Company_Finished_All_OK()
        {
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Team, GrowthItemStatusType.Finished, 
                GrowthItemDetailStatusType.Finished, GrowthItemSegmentType.All, 0);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_TeamGis_Company_Cancelled_Category_OK()
        {
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Team, GrowthItemStatusType.Cancelled, 
                GrowthItemDetailStatusType.Cancelled, GrowthItemSegmentType.Category, 0);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_TeamGis_Company_Cancelled_Competency_OK()
        {
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Team, GrowthItemStatusType.Cancelled, 
                GrowthItemDetailStatusType.Cancelled, GrowthItemSegmentType.Competency, 0);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_TeamGis_Company_Cancelled_All_OK()
        {
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Team, GrowthItemStatusType.Cancelled, 
                GrowthItemDetailStatusType.Cancelled, GrowthItemSegmentType.All, 0);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_TeamGis_Company_OnHold_Category_OK()
        {
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Team, GrowthItemStatusType.OnHold, 
                GrowthItemDetailStatusType.OnHold, GrowthItemSegmentType.Category, 0);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_TeamGis_Company_OnHold_Competency_OK()
        {
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Team, GrowthItemStatusType.OnHold, 
                GrowthItemDetailStatusType.OnHold, GrowthItemSegmentType.Competency, 0);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_TeamGis_Company_OnHold_All_OK()
        {
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Team, GrowthItemStatusType.OnHold, 
                GrowthItemDetailStatusType.OnHold, GrowthItemSegmentType.All, 0);
        }
        
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_TeamGis_Company_Committed_Category_OK()
        {
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Team, GrowthItemStatusType.Committed, 
                GrowthItemDetailStatusType.Committed, GrowthItemSegmentType.Category, 0);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_TeamGis_Company_Committed_Competency_OK()
        {
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Team, GrowthItemStatusType.Committed, 
                GrowthItemDetailStatusType.Committed, GrowthItemSegmentType.Competency, 0);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_TeamGis_Company_Committed_All_OK()
        {
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Team, GrowthItemStatusType.Committed, 
                GrowthItemDetailStatusType.Committed, GrowthItemSegmentType.All, 0);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_TeamGis_Enterprise_NotStarted_Category_OK()
        {
            VerifySetup(_classInitFailed);
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Team, GrowthItemStatusType.NotStarted, 
                GrowthItemDetailStatusType.InBacklog, GrowthItemSegmentType.Category, _enterpriseTeam.TeamId);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_TeamGis_Enterprise_NotStarted_Competency_OK()
        {
            VerifySetup(_classInitFailed);
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Team, GrowthItemStatusType.NotStarted, 
                GrowthItemDetailStatusType.InBacklog, GrowthItemSegmentType.Competency, _enterpriseTeam.TeamId);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_TeamGis_Enterprise_NotStarted_All_OK()
        {
            VerifySetup(_classInitFailed);
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Team, GrowthItemStatusType.NotStarted, 
                GrowthItemDetailStatusType.InBacklog, GrowthItemSegmentType.All, _enterpriseTeam.TeamId);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_TeamGis_Enterprise_Started_Category_OK()
        {
            VerifySetup(_classInitFailed);
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Team, GrowthItemStatusType.Started, 
                GrowthItemDetailStatusType.Started, GrowthItemSegmentType.Category, _enterpriseTeam.TeamId);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_TeamGis_Enterprise_Started_Competency_OK()
        {
            VerifySetup(_classInitFailed);
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Team, GrowthItemStatusType.Started, 
                GrowthItemDetailStatusType.Started, GrowthItemSegmentType.Competency, _enterpriseTeam.TeamId);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_TeamGis_Enterprise_Started_All_OK()
        {
            VerifySetup(_classInitFailed);
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Team, GrowthItemStatusType.Started, 
                GrowthItemDetailStatusType.Started, GrowthItemSegmentType.All, _enterpriseTeam.TeamId);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_TeamGis_Enterprise_StartedWithAge_Category_OK()
        {
            VerifySetup(_classInitFailed);
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Team, GrowthItemStatusType.StartedWithAge, 
                GrowthItemDetailStatusType.StartedMore3, GrowthItemSegmentType.Category, _enterpriseTeam.TeamId);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_TeamGis_Enterprise_StartedWithAge_Competency_OK()
        {
            VerifySetup(_classInitFailed);
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Team, GrowthItemStatusType.StartedWithAge, 
                GrowthItemDetailStatusType.StartedMore3, GrowthItemSegmentType.Competency, _enterpriseTeam.TeamId);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_TeamGis_Enterprise_StartedWithAge_All_OK()
        {
            VerifySetup(_classInitFailed);
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Team, GrowthItemStatusType.StartedWithAge, 
                GrowthItemDetailStatusType.StartedMore3, GrowthItemSegmentType.All, _enterpriseTeam.TeamId);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_TeamGis_Enterprise_Finished_Category_OK()
        {
            VerifySetup(_classInitFailed);
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Team, GrowthItemStatusType.Finished, 
                GrowthItemDetailStatusType.Finished, GrowthItemSegmentType.Category, _enterpriseTeam.TeamId);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_TeamGis_Enterprise_Finished_Competency_OK()
        {
            VerifySetup(_classInitFailed);
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Team, GrowthItemStatusType.Finished, 
                GrowthItemDetailStatusType.Finished, GrowthItemSegmentType.Competency, _enterpriseTeam.TeamId);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_TeamGis_Enterprise_Finished_All_OK()
        {
            VerifySetup(_classInitFailed);
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Team, GrowthItemStatusType.Finished, 
                GrowthItemDetailStatusType.Finished, GrowthItemSegmentType.All, _enterpriseTeam.TeamId);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_TeamGis_Enterprise_Cancelled_Category_OK()
        {
            VerifySetup(_classInitFailed);
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Team, GrowthItemStatusType.Cancelled, 
                GrowthItemDetailStatusType.Cancelled, GrowthItemSegmentType.Category, _enterpriseTeam.TeamId);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_TeamGis_Enterprise_Cancelled_Competency_OK()
        {
            VerifySetup(_classInitFailed);
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Team, GrowthItemStatusType.Cancelled, 
                GrowthItemDetailStatusType.Cancelled, GrowthItemSegmentType.Competency, _enterpriseTeam.TeamId);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_TeamGis_Enterprise_Cancelled_All_OK()
        {
            VerifySetup(_classInitFailed);
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Team, GrowthItemStatusType.Cancelled, 
                GrowthItemDetailStatusType.Cancelled, GrowthItemSegmentType.All, _enterpriseTeam.TeamId);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_TeamGis_Enterprise_OnHold_Category_OK()
        {
            VerifySetup(_classInitFailed);
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Team, GrowthItemStatusType.OnHold, 
                GrowthItemDetailStatusType.OnHold, GrowthItemSegmentType.Category, _enterpriseTeam.TeamId);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_TeamGis_Enterprise_OnHold_Competency_OK()
        {
            VerifySetup(_classInitFailed);
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Team, GrowthItemStatusType.OnHold, 
                GrowthItemDetailStatusType.OnHold, GrowthItemSegmentType.Competency, _enterpriseTeam.TeamId);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_TeamGis_Enterprise_OnHold_All_OK()
        {
            VerifySetup(_classInitFailed);
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Team, GrowthItemStatusType.OnHold, 
                GrowthItemDetailStatusType.OnHold, GrowthItemSegmentType.All, _enterpriseTeam.TeamId);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_TeamGis_Enterprise_Committed_Category_OK()
        {
            VerifySetup(_classInitFailed);
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Team, GrowthItemStatusType.Committed, 
                GrowthItemDetailStatusType.Committed, GrowthItemSegmentType.Category, _enterpriseTeam.TeamId);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_TeamGis_Enterprise_Committed_Competency_OK()
        {
            VerifySetup(_classInitFailed);
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Team, GrowthItemStatusType.Committed, 
                GrowthItemDetailStatusType.Committed, GrowthItemSegmentType.Competency, _enterpriseTeam.TeamId);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_TeamGis_Enterprise_Committed_All_OK()
        {
            VerifySetup(_classInitFailed);
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Team, GrowthItemStatusType.Committed, 
                GrowthItemDetailStatusType.Committed, GrowthItemSegmentType.All, _enterpriseTeam.TeamId);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_TeamGis_MultiTeam_NotStarted_Category_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = _multiTeam.Children.Select(c => c.Name).ToList();
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Team, GrowthItemStatusType.NotStarted, 
                GrowthItemDetailStatusType.InBacklog, GrowthItemSegmentType.Category, _multiTeam.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_TeamGis_MultiTeam_NotStarted_Competency_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = _multiTeam.Children.Select(c => c.Name).ToList();
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Team, GrowthItemStatusType.NotStarted, 
                GrowthItemDetailStatusType.InBacklog, GrowthItemSegmentType.Competency, _multiTeam.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_TeamGis_MultiTeam_NotStarted_All_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = _multiTeam.Children.Select(c => c.Name).ToList();
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Team, GrowthItemStatusType.NotStarted, 
                GrowthItemDetailStatusType.InBacklog, GrowthItemSegmentType.All, _multiTeam.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_TeamGis_MultiTeam_Started_Category_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = _multiTeam.Children.Select(c => c.Name).ToList();
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Team, GrowthItemStatusType.Started, 
                GrowthItemDetailStatusType.Started, GrowthItemSegmentType.Category, _multiTeam.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_TeamGis_MultiTeam_Started_Competency_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = _multiTeam.Children.Select(c => c.Name).ToList();
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Team, GrowthItemStatusType.Started, 
                GrowthItemDetailStatusType.Started, GrowthItemSegmentType.Competency, _multiTeam.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_TeamGis_MultiTeam_Started_All_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = _multiTeam.Children.Select(c => c.Name).ToList();
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Team, GrowthItemStatusType.Started, 
                GrowthItemDetailStatusType.Started, GrowthItemSegmentType.All, _multiTeam.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_TeamGis_MultiTeam_StartedWithAge_Category_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = _multiTeam.Children.Select(c => c.Name).ToList();
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Team, GrowthItemStatusType.StartedWithAge, 
                GrowthItemDetailStatusType.StartedMore3, GrowthItemSegmentType.Category, _multiTeam.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_TeamGis_MultiTeam_StartedWithAge_Competency_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = _multiTeam.Children.Select(c => c.Name).ToList();
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Team, GrowthItemStatusType.StartedWithAge, 
                GrowthItemDetailStatusType.StartedMore3, GrowthItemSegmentType.Competency, _multiTeam.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_TeamGis_MultiTeam_StartedWithAge_All_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = _multiTeam.Children.Select(c => c.Name).ToList();
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Team, GrowthItemStatusType.StartedWithAge, 
                GrowthItemDetailStatusType.StartedMore3, GrowthItemSegmentType.All, _multiTeam.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_TeamGis_MultiTeam_Finished_Category_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = _multiTeam.Children.Select(c => c.Name).ToList();
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Team, GrowthItemStatusType.Finished, 
                GrowthItemDetailStatusType.Finished, GrowthItemSegmentType.Category, _multiTeam.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_TeamGis_MultiTeam_Finished_Competency_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = _multiTeam.Children.Select(c => c.Name).ToList();
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Team, GrowthItemStatusType.Finished, 
                GrowthItemDetailStatusType.Finished, GrowthItemSegmentType.Competency, _multiTeam.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_TeamGis_MultiTeam_Finished_All_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = _multiTeam.Children.Select(c => c.Name).ToList();
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Team, GrowthItemStatusType.Finished, 
                GrowthItemDetailStatusType.Finished, GrowthItemSegmentType.All, _multiTeam.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_TeamGis_MultiTeam_Cancelled_Category_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = _multiTeam.Children.Select(c => c.Name).ToList();
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Team, GrowthItemStatusType.Cancelled, 
                GrowthItemDetailStatusType.Cancelled, GrowthItemSegmentType.Category, _multiTeam.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_TeamGis_MultiTeam_Cancelled_Competency_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = _multiTeam.Children.Select(c => c.Name).ToList();
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Team, GrowthItemStatusType.Cancelled, 
                GrowthItemDetailStatusType.Cancelled, GrowthItemSegmentType.Competency, _multiTeam.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_TeamGis_MultiTeam_Cancelled_All_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = _multiTeam.Children.Select(c => c.Name).ToList();
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Team, GrowthItemStatusType.Cancelled, 
                GrowthItemDetailStatusType.Cancelled, GrowthItemSegmentType.All, _multiTeam.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_TeamGis_MultiTeam_OnHold_Category_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = _multiTeam.Children.Select(c => c.Name).ToList();
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Team, GrowthItemStatusType.OnHold, 
                GrowthItemDetailStatusType.OnHold, GrowthItemSegmentType.Category, _multiTeam.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_TeamGis_MultiTeam_OnHold_Competency_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = _multiTeam.Children.Select(c => c.Name).ToList();
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Team, GrowthItemStatusType.OnHold, 
                GrowthItemDetailStatusType.OnHold, GrowthItemSegmentType.Competency, _multiTeam.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_TeamGis_MultiTeam_OnHold_All_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = _multiTeam.Children.Select(c => c.Name).ToList();
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Team, GrowthItemStatusType.OnHold, 
                GrowthItemDetailStatusType.OnHold, GrowthItemSegmentType.All, _multiTeam.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_TeamGis_MultiTeam_Committed_Category_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = _multiTeam.Children.Select(c => c.Name).ToList();
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Team, GrowthItemStatusType.Committed, 
                GrowthItemDetailStatusType.Committed, GrowthItemSegmentType.Category, _multiTeam.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_TeamGis_MultiTeam_Committed_Competency_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = _multiTeam.Children.Select(c => c.Name).ToList();
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Team, GrowthItemStatusType.Committed, 
                GrowthItemDetailStatusType.Committed, GrowthItemSegmentType.Competency, _multiTeam.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_TeamGis_MultiTeam_Committed_All_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = _multiTeam.Children.Select(c => c.Name).ToList();
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Team, GrowthItemStatusType.Committed, 
                GrowthItemDetailStatusType.Committed, GrowthItemSegmentType.All, _multiTeam.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_TeamGis_Team_NotStarted_Category_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = new List<string> {_team.Name};
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Team, GrowthItemStatusType.NotStarted, 
                GrowthItemDetailStatusType.InBacklog, GrowthItemSegmentType.Category, _team.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_TeamGis_Team_NotStarted_Competency_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = new List<string> {_team.Name};
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Team, GrowthItemStatusType.NotStarted, 
                GrowthItemDetailStatusType.InBacklog, GrowthItemSegmentType.Competency, _team.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_TeamGis_Team_NotStarted_All_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = new List<string> {_team.Name};
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Team, GrowthItemStatusType.NotStarted, 
                GrowthItemDetailStatusType.InBacklog, GrowthItemSegmentType.All, _team.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_TeamGis_Team_Started_Category_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = new List<string> {_team.Name};
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Team, GrowthItemStatusType.Started, 
                GrowthItemDetailStatusType.Started, GrowthItemSegmentType.Category, _team.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_TeamGis_Team_Started_Competency_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = new List<string> {_team.Name};
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Team, GrowthItemStatusType.Started, 
                GrowthItemDetailStatusType.Started, GrowthItemSegmentType.Competency, _team.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_TeamGis_Team_Started_All_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = new List<string> {_team.Name};
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Team, GrowthItemStatusType.Started, 
                GrowthItemDetailStatusType.Started, GrowthItemSegmentType.All, _team.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_TeamGis_Team_StartedWithAge_Category_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = new List<string> {_team.Name};
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Team, GrowthItemStatusType.StartedWithAge, 
                GrowthItemDetailStatusType.StartedMore3, GrowthItemSegmentType.Category, _team.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_TeamGis_Team_StartedWithAge_Competency_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = new List<string> {_team.Name};
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Team, GrowthItemStatusType.StartedWithAge, 
                GrowthItemDetailStatusType.StartedMore3, GrowthItemSegmentType.Competency, _team.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_TeamGis_Team_StartedWithAge_All_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = new List<string> {_team.Name};
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Team, GrowthItemStatusType.StartedWithAge, 
                GrowthItemDetailStatusType.StartedMore3, GrowthItemSegmentType.All, _team.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_TeamGis_Team_Finished_Category_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = new List<string> {_team.Name};
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Team, GrowthItemStatusType.Finished, 
                GrowthItemDetailStatusType.Finished, GrowthItemSegmentType.Category, _team.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_TeamGis_Team_Finished_Competency_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = new List<string> {_team.Name};
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Team, GrowthItemStatusType.Finished, 
                GrowthItemDetailStatusType.Finished, GrowthItemSegmentType.Competency, _team.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_TeamGis_Team_Finished_All_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = new List<string> {_team.Name};
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Team, GrowthItemStatusType.Finished, 
                GrowthItemDetailStatusType.Finished, GrowthItemSegmentType.All, _team.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_TeamGis_Team_Cancelled_Category_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = new List<string> {_team.Name};
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Team, GrowthItemStatusType.Cancelled, 
                GrowthItemDetailStatusType.Cancelled, GrowthItemSegmentType.Category, _team.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_TeamGis_Team_Cancelled_Competency_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = new List<string> {_team.Name};
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Team, GrowthItemStatusType.Cancelled, 
                GrowthItemDetailStatusType.Cancelled, GrowthItemSegmentType.Competency, _team.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_TeamGis_Team_Cancelled_All_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = new List<string> {_team.Name};
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Team, GrowthItemStatusType.Cancelled, 
                GrowthItemDetailStatusType.Cancelled, GrowthItemSegmentType.All, _team.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_TeamGis_Team_OnHold_Category_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = new List<string> {_team.Name};
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Team, GrowthItemStatusType.OnHold, 
                GrowthItemDetailStatusType.OnHold, GrowthItemSegmentType.Category, _team.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_TeamGis_Team_OnHold_Competency_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = new List<string> {_team.Name};
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Team, GrowthItemStatusType.OnHold, 
                GrowthItemDetailStatusType.OnHold, GrowthItemSegmentType.Competency, _team.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_TeamGis_Team_OnHold_All_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = new List<string> {_team.Name};
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Team, GrowthItemStatusType.OnHold, 
                GrowthItemDetailStatusType.OnHold, GrowthItemSegmentType.All, _team.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_TeamGis_Team_Committed_Category_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = new List<string> {_team.Name};
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Team, GrowthItemStatusType.Committed, 
                GrowthItemDetailStatusType.Committed, GrowthItemSegmentType.Category, _team.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_TeamGis_Team_Committed_Competency_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = new List<string> {_team.Name};
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Team, GrowthItemStatusType.Committed, 
                GrowthItemDetailStatusType.Committed, GrowthItemSegmentType.Competency, _team.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_TeamGis_Team_Committed_All_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = new List<string> {_team.Name};
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Team, GrowthItemStatusType.Committed, 
                GrowthItemDetailStatusType.Committed, GrowthItemSegmentType.All, _team.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_OrgGis_Company_NotStarted_Category_OK()
        {
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Organizational, GrowthItemStatusType.NotStarted, 
                GrowthItemDetailStatusType.InBacklog, GrowthItemSegmentType.Category, 0);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_OrgGis_Company_NotStarted_Competency_OK()
        {
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Organizational, GrowthItemStatusType.NotStarted, 
                GrowthItemDetailStatusType.InBacklog, GrowthItemSegmentType.Competency,0);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_OrgGis_Company_NotStarted_All_OK()
        {
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Organizational, GrowthItemStatusType.NotStarted, 
                GrowthItemDetailStatusType.InBacklog, GrowthItemSegmentType.All,0);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_OrgGis_Company_Started_Category_OK()
        {
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Organizational, GrowthItemStatusType.Started, 
                GrowthItemDetailStatusType.Started, GrowthItemSegmentType.Category, 0);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_OrgGis_Company_Started_Competency_OK()
        {
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Organizational, GrowthItemStatusType.Started, 
                GrowthItemDetailStatusType.Started, GrowthItemSegmentType.Competency, 0);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_OrgGis_Company_Started_All_OK()
        {
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Organizational, GrowthItemStatusType.Started, 
                GrowthItemDetailStatusType.Started, GrowthItemSegmentType.All, 0);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_OrgGis_Company_StartedWithAge_Category_OK()
        {
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Organizational, GrowthItemStatusType.StartedWithAge, 
                GrowthItemDetailStatusType.StartedMore3, GrowthItemSegmentType.Category, 0);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_OrgGis_Company_StartedWithAge_Competency_OK()
        {
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Organizational, GrowthItemStatusType.StartedWithAge, 
                GrowthItemDetailStatusType.StartedMore3, GrowthItemSegmentType.Competency, 0);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_OrgGis_Company_StartedWithAge_All_OK()
        {
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Organizational, GrowthItemStatusType.StartedWithAge, 
                GrowthItemDetailStatusType.StartedMore3, GrowthItemSegmentType.All, 0);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_OrgGis_Company_Finished_Category_OK()
        {
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Organizational, GrowthItemStatusType.Finished, 
                GrowthItemDetailStatusType.Finished, GrowthItemSegmentType.Category, 0);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_OrgGis_Company_Finished_Competency_OK()
        {
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Organizational, GrowthItemStatusType.Finished, 
                GrowthItemDetailStatusType.Finished, GrowthItemSegmentType.Competency, 0);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_OrgGis_Company_Finished_All_OK()
        {
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Organizational, GrowthItemStatusType.Finished, 
                GrowthItemDetailStatusType.Finished, GrowthItemSegmentType.All, 0);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_OrgGis_Company_Cancelled_Category_OK()
        {
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Organizational, GrowthItemStatusType.Cancelled, 
                GrowthItemDetailStatusType.Cancelled, GrowthItemSegmentType.Category, 0);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_OrgGis_Company_Cancelled_Competency_OK()
        {
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Organizational, GrowthItemStatusType.Cancelled, 
                GrowthItemDetailStatusType.Cancelled, GrowthItemSegmentType.Competency, 0);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_OrgGis_Company_Cancelled_All_OK()
        {
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Organizational, GrowthItemStatusType.Cancelled, 
                GrowthItemDetailStatusType.Cancelled, GrowthItemSegmentType.All, 0);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_OrgGis_Company_OnHold_Category_OK()
        {
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Organizational, GrowthItemStatusType.OnHold, 
                GrowthItemDetailStatusType.OnHold, GrowthItemSegmentType.Category, 0);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_OrgGis_Company_OnHold_Competency_OK()
        {
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Organizational, GrowthItemStatusType.OnHold, 
                GrowthItemDetailStatusType.OnHold, GrowthItemSegmentType.Competency, 0);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_OrgGis_Company_OnHold_All_OK()
        {
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Organizational, GrowthItemStatusType.OnHold, 
                GrowthItemDetailStatusType.OnHold, GrowthItemSegmentType.All, 0);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_OrgGis_Company_Committed_Category_OK()
        {
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Organizational, GrowthItemStatusType.Committed, 
                GrowthItemDetailStatusType.Committed, GrowthItemSegmentType.Category, 0);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_OrgGis_Company_Committed_Competency_OK()
        {
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Organizational, GrowthItemStatusType.Committed, 
                GrowthItemDetailStatusType.Committed, GrowthItemSegmentType.Competency, 0);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_OrgGis_Company_Committed_All_OK()
        {
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Organizational, GrowthItemStatusType.Committed, 
                GrowthItemDetailStatusType.Committed, GrowthItemSegmentType.All, 0);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_OrgGis_Enterprise_NotStarted_Category_OK()
        {
            VerifySetup(_classInitFailed);
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Organizational, GrowthItemStatusType.NotStarted, 
                GrowthItemDetailStatusType.InBacklog, GrowthItemSegmentType.Category, _enterpriseTeam.TeamId);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_OrgGis_Enterprise_NotStarted_Competency_OK()
        {
            VerifySetup(_classInitFailed);
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Organizational, GrowthItemStatusType.NotStarted, 
                GrowthItemDetailStatusType.InBacklog, GrowthItemSegmentType.Competency,_enterpriseTeam.TeamId);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_OrgGis_Enterprise_NotStarted_All_OK()
        {
            VerifySetup(_classInitFailed);
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Organizational, GrowthItemStatusType.NotStarted, 
                GrowthItemDetailStatusType.InBacklog, GrowthItemSegmentType.All,_enterpriseTeam.TeamId);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_OrgGis_Enterprise_Started_Category_OK()
        {
            VerifySetup(_classInitFailed);
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Organizational, GrowthItemStatusType.Started, 
                GrowthItemDetailStatusType.Started, GrowthItemSegmentType.Category, _enterpriseTeam.TeamId);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_OrgGis_Enterprise_Started_Competency_OK()
        {
            VerifySetup(_classInitFailed);
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Organizational, GrowthItemStatusType.Started, 
                GrowthItemDetailStatusType.Started, GrowthItemSegmentType.Competency, _enterpriseTeam.TeamId);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_OrgGis_Enterprise_Started_All_OK()
        {
            VerifySetup(_classInitFailed);
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Organizational, GrowthItemStatusType.Started, 
                GrowthItemDetailStatusType.Started, GrowthItemSegmentType.All, _enterpriseTeam.TeamId);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_OrgGis_Enterprise_StartedWithAge_Category_OK()
        {
            VerifySetup(_classInitFailed);
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Organizational, GrowthItemStatusType.StartedWithAge, 
                GrowthItemDetailStatusType.StartedMore3, GrowthItemSegmentType.Category, _enterpriseTeam.TeamId);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_OrgGis_Enterprise_StartedWithAge_Competency_OK()
        {
            VerifySetup(_classInitFailed);
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Organizational, GrowthItemStatusType.StartedWithAge, 
                GrowthItemDetailStatusType.StartedMore3, GrowthItemSegmentType.Competency, _enterpriseTeam.TeamId);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_OrgGis_Enterprise_StartedWithAge_All_OK()
        {
            VerifySetup(_classInitFailed);
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Organizational, GrowthItemStatusType.StartedWithAge, 
                GrowthItemDetailStatusType.StartedMore3, GrowthItemSegmentType.All, _enterpriseTeam.TeamId);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_OrgGis_Enterprise_Finished_Category_OK()
        {
            VerifySetup(_classInitFailed);
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Organizational, GrowthItemStatusType.Finished, 
                GrowthItemDetailStatusType.Finished, GrowthItemSegmentType.Category, _enterpriseTeam.TeamId);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_OrgGis_Enterprise_Finished_Competency_OK()
        {
            VerifySetup(_classInitFailed);
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Organizational, GrowthItemStatusType.Finished, 
                GrowthItemDetailStatusType.Finished, GrowthItemSegmentType.Competency, _enterpriseTeam.TeamId);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_OrgGis_Enterprise_Finished_All_OK()
        {
            VerifySetup(_classInitFailed);
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Organizational, GrowthItemStatusType.Finished, 
                GrowthItemDetailStatusType.Finished, GrowthItemSegmentType.All, _enterpriseTeam.TeamId);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_OrgGis_Enterprise_Cancelled_Category_OK()
        {
            VerifySetup(_classInitFailed);
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Organizational, GrowthItemStatusType.Cancelled, 
                GrowthItemDetailStatusType.Cancelled, GrowthItemSegmentType.Category, _enterpriseTeam.TeamId);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_OrgGis_Enterprise_Cancelled_Competency_OK()
        {
            VerifySetup(_classInitFailed);
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Organizational, GrowthItemStatusType.Cancelled, 
                GrowthItemDetailStatusType.Cancelled, GrowthItemSegmentType.Competency, _enterpriseTeam.TeamId);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_OrgGis_Enterprise_Cancelled_All_OK()
        {
            VerifySetup(_classInitFailed);
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Organizational, GrowthItemStatusType.Cancelled, 
                GrowthItemDetailStatusType.Cancelled, GrowthItemSegmentType.All, _enterpriseTeam.TeamId);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_OrgGis_Enterprise_OnHold_Category_OK()
        {
            VerifySetup(_classInitFailed);
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Organizational, GrowthItemStatusType.OnHold, 
                GrowthItemDetailStatusType.OnHold, GrowthItemSegmentType.Category, _enterpriseTeam.TeamId);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_OrgGis_Enterprise_OnHold_Competency_OK()
        {
            VerifySetup(_classInitFailed);
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Organizational, GrowthItemStatusType.OnHold, 
                GrowthItemDetailStatusType.OnHold, GrowthItemSegmentType.Competency, _enterpriseTeam.TeamId);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_OrgGis_Enterprise_OnHold_All_OK()
        {
            VerifySetup(_classInitFailed);
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Organizational, GrowthItemStatusType.OnHold, 
                GrowthItemDetailStatusType.OnHold, GrowthItemSegmentType.All, _enterpriseTeam.TeamId);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_OrgGis_Enterprise_Committed_Category_OK()
        {
            VerifySetup(_classInitFailed);
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Organizational, GrowthItemStatusType.Committed, 
                GrowthItemDetailStatusType.Committed, GrowthItemSegmentType.Category, _enterpriseTeam.TeamId);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_OrgGis_Enterprise_Committed_Competency_OK()
        {
            VerifySetup(_classInitFailed);
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Organizational, GrowthItemStatusType.Committed, 
                GrowthItemDetailStatusType.Committed, GrowthItemSegmentType.Competency, _enterpriseTeam.TeamId);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_OrgGis_Enterprise_Committed_All_OK()
        {
            VerifySetup(_classInitFailed);
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Organizational, GrowthItemStatusType.Committed, 
                GrowthItemDetailStatusType.Committed, GrowthItemSegmentType.All, _enterpriseTeam.TeamId);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_OrgGis_MultiTeam_NotStarted_Category_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = _multiTeam.Children.Select(c => c.Name).ToList();
            teamNames.Add(_multiTeam.Name);
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Organizational, GrowthItemStatusType.NotStarted, 
                GrowthItemDetailStatusType.InBacklog, GrowthItemSegmentType.Category, _multiTeam.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_OrgGis_MultiTeam_NotStarted_Competency_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = _multiTeam.Children.Select(c => c.Name).ToList();
            teamNames.Add(_multiTeam.Name);
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Organizational, GrowthItemStatusType.NotStarted, 
                GrowthItemDetailStatusType.InBacklog, GrowthItemSegmentType.Competency, _multiTeam.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_OrgGis_MultiTeam_NotStarted_All_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = _multiTeam.Children.Select(c => c.Name).ToList();
            teamNames.Add(_multiTeam.Name);
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Organizational, GrowthItemStatusType.NotStarted, 
                GrowthItemDetailStatusType.InBacklog, GrowthItemSegmentType.All, _multiTeam.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_OrgGis_MultiTeam_Started_Category_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = _multiTeam.Children.Select(c => c.Name).ToList();
            teamNames.Add(_multiTeam.Name);
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Organizational, GrowthItemStatusType.Started, 
                GrowthItemDetailStatusType.Started, GrowthItemSegmentType.Category, _multiTeam.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_OrgGis_MultiTeam_Started_Competency_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = _multiTeam.Children.Select(c => c.Name).ToList();
            teamNames.Add(_multiTeam.Name);
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Organizational, GrowthItemStatusType.Started, 
                GrowthItemDetailStatusType.Started, GrowthItemSegmentType.Competency, _multiTeam.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_OrgGis_MultiTeam_Started_All_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = _multiTeam.Children.Select(c => c.Name).ToList();
            teamNames.Add(_multiTeam.Name);
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Organizational, GrowthItemStatusType.Started, 
                GrowthItemDetailStatusType.Started, GrowthItemSegmentType.All, _multiTeam.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_OrgGis_MultiTeam_StartedWithAge_Category_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = _multiTeam.Children.Select(c => c.Name).ToList();
            teamNames.Add(_multiTeam.Name);
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Organizational, GrowthItemStatusType.StartedWithAge, 
                GrowthItemDetailStatusType.StartedMore3, GrowthItemSegmentType.Category, _multiTeam.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_OrgGis_MultiTeam_StartedWithAge_Competency_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = _multiTeam.Children.Select(c => c.Name).ToList();
            teamNames.Add(_multiTeam.Name);
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Organizational, GrowthItemStatusType.StartedWithAge, 
                GrowthItemDetailStatusType.StartedMore3, GrowthItemSegmentType.Competency, _multiTeam.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_OrgGis_MultiTeam_StartedWithAge_All_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = _multiTeam.Children.Select(c => c.Name).ToList();
            teamNames.Add(_multiTeam.Name);
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Organizational, GrowthItemStatusType.StartedWithAge, 
                GrowthItemDetailStatusType.StartedMore3, GrowthItemSegmentType.All, _multiTeam.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_OrgGis_MultiTeam_Finished_Category_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = _multiTeam.Children.Select(c => c.Name).ToList();
            teamNames.Add(_multiTeam.Name);
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Organizational, GrowthItemStatusType.Finished, 
                GrowthItemDetailStatusType.Finished, GrowthItemSegmentType.Category, _multiTeam.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_OrgGis_MultiTeam_Finished_Competency_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = _multiTeam.Children.Select(c => c.Name).ToList();
            teamNames.Add(_multiTeam.Name);
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Organizational, GrowthItemStatusType.Finished, 
                GrowthItemDetailStatusType.Finished, GrowthItemSegmentType.Competency, _multiTeam.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_OrgGis_MultiTeam_Finished_All_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = _multiTeam.Children.Select(c => c.Name).ToList();
            teamNames.Add(_multiTeam.Name);
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Organizational, GrowthItemStatusType.Finished, 
                GrowthItemDetailStatusType.Finished, GrowthItemSegmentType.All, _multiTeam.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_OrgGis_MultiTeam_Cancelled_Category_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = _multiTeam.Children.Select(c => c.Name).ToList();
            teamNames.Add(_multiTeam.Name);
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Organizational, GrowthItemStatusType.Cancelled, 
                GrowthItemDetailStatusType.Cancelled, GrowthItemSegmentType.Category, _multiTeam.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_OrgGis_MultiTeam_Cancelled_Competency_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = _multiTeam.Children.Select(c => c.Name).ToList();
            teamNames.Add(_multiTeam.Name);
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Organizational, GrowthItemStatusType.Cancelled, 
                GrowthItemDetailStatusType.Cancelled, GrowthItemSegmentType.Competency, _multiTeam.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_OrgGis_MultiTeam_Cancelled_All_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = _multiTeam.Children.Select(c => c.Name).ToList();
            teamNames.Add(_multiTeam.Name);
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Organizational, GrowthItemStatusType.Cancelled, 
                GrowthItemDetailStatusType.Cancelled, GrowthItemSegmentType.All, _multiTeam.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_OrgGis_MultiTeam_OnHold_Category_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = _multiTeam.Children.Select(c => c.Name).ToList();
            teamNames.Add(_multiTeam.Name);
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Organizational, GrowthItemStatusType.OnHold, 
                GrowthItemDetailStatusType.OnHold, GrowthItemSegmentType.Category, _multiTeam.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_OrgGis_MultiTeam_OnHold_Competency_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = _multiTeam.Children.Select(c => c.Name).ToList();
            teamNames.Add(_multiTeam.Name);
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Organizational, GrowthItemStatusType.OnHold, 
                GrowthItemDetailStatusType.OnHold, GrowthItemSegmentType.Competency, _multiTeam.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_OrgGis_MultiTeam_OnHold_All_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = _multiTeam.Children.Select(c => c.Name).ToList();
            teamNames.Add(_multiTeam.Name);
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Organizational, GrowthItemStatusType.OnHold, 
                GrowthItemDetailStatusType.OnHold, GrowthItemSegmentType.All, _multiTeam.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_OrgGis_MultiTeam_Committed_Category_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = _multiTeam.Children.Select(c => c.Name).ToList();
            teamNames.Add(_multiTeam.Name);
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Organizational, GrowthItemStatusType.Committed, 
                GrowthItemDetailStatusType.Committed, GrowthItemSegmentType.Category, _multiTeam.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_OrgGis_MultiTeam_Committed_Competency_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = _multiTeam.Children.Select(c => c.Name).ToList();
            teamNames.Add(_multiTeam.Name);
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Organizational, GrowthItemStatusType.Committed, 
                GrowthItemDetailStatusType.Committed, GrowthItemSegmentType.Competency, _multiTeam.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_OrgGis_MultiTeam_Committed_All_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = _multiTeam.Children.Select(c => c.Name).ToList();
            teamNames.Add(_multiTeam.Name);
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Organizational, GrowthItemStatusType.Committed, 
                GrowthItemDetailStatusType.Committed, GrowthItemSegmentType.All, _multiTeam.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_OrgGis_Team_NotStarted_Category_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = new List<string> {_team.Name};
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Organizational, GrowthItemStatusType.NotStarted, 
                GrowthItemDetailStatusType.InBacklog, GrowthItemSegmentType.Category, _team.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_OrgGis_Team_NotStarted_Competency_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = new List<string> {_team.Name};
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Organizational, GrowthItemStatusType.NotStarted, 
                GrowthItemDetailStatusType.InBacklog, GrowthItemSegmentType.Competency, _team.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_OrgGis_Team_NotStarted_All_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = new List<string> {_team.Name};
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Organizational, GrowthItemStatusType.NotStarted, 
                GrowthItemDetailStatusType.InBacklog, GrowthItemSegmentType.All, _team.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_OrgGis_Team_Started_Category_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = new List<string> {_team.Name};
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Organizational, GrowthItemStatusType.Started, 
                GrowthItemDetailStatusType.Started, GrowthItemSegmentType.Category, _team.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_OrgGis_Team_Started_Competency_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = new List<string> {_team.Name};
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Organizational, GrowthItemStatusType.Started, 
                GrowthItemDetailStatusType.Started, GrowthItemSegmentType.Competency, _team.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_OrgGis_Team_Started_All_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = new List<string> {_team.Name};
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Organizational, GrowthItemStatusType.Started, 
                GrowthItemDetailStatusType.Started, GrowthItemSegmentType.All, _team.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_OrgGis_Team_StartedWithAge_Category_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = new List<string> {_team.Name};
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Organizational, GrowthItemStatusType.StartedWithAge, 
                GrowthItemDetailStatusType.StartedMore3, GrowthItemSegmentType.Category, _team.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_OrgGis_Team_StartedWithAge_Competency_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = new List<string> {_team.Name};
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Organizational, GrowthItemStatusType.StartedWithAge, 
                GrowthItemDetailStatusType.StartedMore3, GrowthItemSegmentType.Competency, _team.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_OrgGis_Team_StartedWithAge_All_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = new List<string> {_team.Name};
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Organizational, GrowthItemStatusType.StartedWithAge, 
                GrowthItemDetailStatusType.StartedMore3, GrowthItemSegmentType.All, _team.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_OrgGis_Team_Finished_Category_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = new List<string> {_team.Name};
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Organizational, GrowthItemStatusType.Finished, 
                GrowthItemDetailStatusType.Finished, GrowthItemSegmentType.Category, _team.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_OrgGis_Team_Finished_Competency_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = new List<string> {_team.Name};
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Organizational, GrowthItemStatusType.Finished, 
                GrowthItemDetailStatusType.Finished, GrowthItemSegmentType.Competency, _team.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_OrgGis_Team_Finished_All_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = new List<string> {_team.Name};
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Organizational, GrowthItemStatusType.Finished, 
                GrowthItemDetailStatusType.Finished, GrowthItemSegmentType.All, _team.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_OrgGis_Team_Cancelled_Category_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = new List<string> {_team.Name};
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Organizational, GrowthItemStatusType.Cancelled, 
                GrowthItemDetailStatusType.Cancelled, GrowthItemSegmentType.Category, _team.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_OrgGis_Team_Cancelled_Competency_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = new List<string> {_team.Name};
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Organizational, GrowthItemStatusType.Cancelled, 
                GrowthItemDetailStatusType.Cancelled, GrowthItemSegmentType.Competency, _team.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_OrgGis_Team_Cancelled_All_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = new List<string> {_team.Name};
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Organizational, GrowthItemStatusType.Cancelled, 
                GrowthItemDetailStatusType.Cancelled, GrowthItemSegmentType.All, _team.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_OrgGis_Team_OnHold_Category_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = new List<string> {_team.Name};
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Organizational, GrowthItemStatusType.OnHold, 
                GrowthItemDetailStatusType.OnHold, GrowthItemSegmentType.Category, _team.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_OrgGis_Team_OnHold_Competency_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = new List<string> {_team.Name};
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Organizational, GrowthItemStatusType.OnHold, 
                GrowthItemDetailStatusType.OnHold, GrowthItemSegmentType.Competency, _team.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_OrgGis_Team_OnHold_All_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = new List<string> {_team.Name};
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Organizational, GrowthItemStatusType.OnHold, 
                GrowthItemDetailStatusType.OnHold, GrowthItemSegmentType.All, _team.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_OrgGis_Team_Committed_Category_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = new List<string> {_team.Name};
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Organizational, GrowthItemStatusType.Committed, 
                GrowthItemDetailStatusType.Committed, GrowthItemSegmentType.Category, _team.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_OrgGis_Team_Committed_Competency_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = new List<string> {_team.Name};
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Organizational, GrowthItemStatusType.Committed, 
                GrowthItemDetailStatusType.Committed, GrowthItemSegmentType.Competency, _team.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_OrgGis_Team_Committed_All_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = new List<string> {_team.Name};
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Organizational, GrowthItemStatusType.Committed, 
                GrowthItemDetailStatusType.Committed, GrowthItemSegmentType.All, _team.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_EntGis_Company_NotStarted_Category_OK()
        {
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Enterprise, GrowthItemStatusType.NotStarted, 
                GrowthItemDetailStatusType.InBacklog, GrowthItemSegmentType.Category, 0);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_EntGis_Company_NotStarted_Competency_OK()
        {
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Enterprise, GrowthItemStatusType.NotStarted, 
                GrowthItemDetailStatusType.InBacklog, GrowthItemSegmentType.Competency, 0);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_EntGis_Company_NotStarted_All_OK()
        {
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Enterprise, GrowthItemStatusType.NotStarted, 
                GrowthItemDetailStatusType.InBacklog, GrowthItemSegmentType.All, 0);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_EntGis_Company_Started_Category_OK()
        {
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Enterprise, GrowthItemStatusType.Started, 
                GrowthItemDetailStatusType.Started, GrowthItemSegmentType.Category, 0);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_EntGis_Company_Started_Competency_OK()
        {
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Enterprise, GrowthItemStatusType.Started, 
                GrowthItemDetailStatusType.Started, GrowthItemSegmentType.Competency, 0);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_EntGis_Company_Started_All_OK()
        {
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Enterprise, GrowthItemStatusType.Started, 
                GrowthItemDetailStatusType.Started, GrowthItemSegmentType.All, 0);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_EntGis_Company_StartedWithAge_Category_OK()
        {
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Enterprise, GrowthItemStatusType.StartedWithAge, 
                GrowthItemDetailStatusType.StartedMore3, GrowthItemSegmentType.Category, 0);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_EntGis_Company_StartedWithAge_Competency_OK()
        {
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Enterprise, GrowthItemStatusType.StartedWithAge, 
                GrowthItemDetailStatusType.StartedMore3, GrowthItemSegmentType.Competency, 0);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_EntGis_Company_StartedWithAge_All_OK()
        {
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Enterprise, GrowthItemStatusType.StartedWithAge, 
                GrowthItemDetailStatusType.StartedMore3, GrowthItemSegmentType.All, 0);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_EntGis_Company_Finished_Category_OK()
        {
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Enterprise, GrowthItemStatusType.Finished, 
                GrowthItemDetailStatusType.Finished, GrowthItemSegmentType.Category, 0);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_EntGis_Company_Finished_Competency_OK()
        {
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Enterprise, GrowthItemStatusType.Finished, 
                GrowthItemDetailStatusType.Finished, GrowthItemSegmentType.Competency, 0);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_EntGis_Company_Finished_All_OK()
        {
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Enterprise, GrowthItemStatusType.Finished, 
                GrowthItemDetailStatusType.Finished, GrowthItemSegmentType.All, 0);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_EntGis_Company_Cancelled_Category_OK()
        {
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Enterprise, GrowthItemStatusType.Cancelled, 
                GrowthItemDetailStatusType.Cancelled, GrowthItemSegmentType.Category, 0);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_EntGis_Company_Cancelled_Competency_OK()
        {
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Enterprise, GrowthItemStatusType.Cancelled, 
                GrowthItemDetailStatusType.Cancelled, GrowthItemSegmentType.Competency, 0);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_EntGis_Company_Cancelled_All_OK()
        {
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Enterprise, GrowthItemStatusType.Cancelled, 
                GrowthItemDetailStatusType.Cancelled, GrowthItemSegmentType.All, 0);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_EntGis_Company_OnHold_Category_OK()
        {
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Enterprise, GrowthItemStatusType.OnHold, 
                GrowthItemDetailStatusType.OnHold, GrowthItemSegmentType.Category, 0);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_EntGis_Company_OnHold_Competency_OK()
        {
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Enterprise, GrowthItemStatusType.OnHold, 
                GrowthItemDetailStatusType.OnHold, GrowthItemSegmentType.Competency, 0);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_EntGis_Company_OnHold_All_OK()
        {
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Enterprise, GrowthItemStatusType.OnHold, 
                GrowthItemDetailStatusType.OnHold, GrowthItemSegmentType.All, 0);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_EntGis_Company_Committed_Category_OK()
        {
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Enterprise, GrowthItemStatusType.Committed, 
                GrowthItemDetailStatusType.Committed, GrowthItemSegmentType.Category, 0);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_EntGis_Company_Committed_Competency_OK()
        {
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Enterprise, GrowthItemStatusType.Committed, 
                GrowthItemDetailStatusType.Committed, GrowthItemSegmentType.Competency, 0);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_EntGis_Company_Committed_All_OK()
        {
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Enterprise, GrowthItemStatusType.Committed, 
                GrowthItemDetailStatusType.Committed, GrowthItemSegmentType.All, 0);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_EntGis_Enterprise_NotStarted_Category_OK()
        {
            VerifySetup(_classInitFailed);
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Enterprise, GrowthItemStatusType.NotStarted, 
                GrowthItemDetailStatusType.InBacklog, GrowthItemSegmentType.Category, _enterpriseTeam.TeamId);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_EntGis_Enterprise_NotStarted_Competency_OK()
        {
            VerifySetup(_classInitFailed);
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Enterprise, GrowthItemStatusType.NotStarted, 
                GrowthItemDetailStatusType.InBacklog, GrowthItemSegmentType.Competency, _enterpriseTeam.TeamId);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_EntGis_Enterprise_NotStarted_All_OK()
        {
            VerifySetup(_classInitFailed);
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Enterprise, GrowthItemStatusType.NotStarted, 
                GrowthItemDetailStatusType.InBacklog, GrowthItemSegmentType.All, _enterpriseTeam.TeamId);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_EntGis_Enterprise_Started_Category_OK()
        {
            VerifySetup(_classInitFailed);
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Enterprise, GrowthItemStatusType.Started, 
                GrowthItemDetailStatusType.Started, GrowthItemSegmentType.Category, _enterpriseTeam.TeamId);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_EntGis_Enterprise_Started_Competency_OK()
        {
            VerifySetup(_classInitFailed);
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Enterprise, GrowthItemStatusType.Started, 
                GrowthItemDetailStatusType.Started, GrowthItemSegmentType.Competency, _enterpriseTeam.TeamId);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_EntGis_Enterprise_Started_All_OK()
        {
            VerifySetup(_classInitFailed);
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Enterprise, GrowthItemStatusType.Started, 
                GrowthItemDetailStatusType.Started, GrowthItemSegmentType.All, _enterpriseTeam.TeamId);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_EntGis_Enterprise_StartedWithAge_Category_OK()
        {
            VerifySetup(_classInitFailed);
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Enterprise, GrowthItemStatusType.StartedWithAge, 
                GrowthItemDetailStatusType.StartedMore3, GrowthItemSegmentType.Category, _enterpriseTeam.TeamId);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_EntGis_Enterprise_StartedWithAge_Competency_OK()
        {
            VerifySetup(_classInitFailed);
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Enterprise, GrowthItemStatusType.StartedWithAge, 
                GrowthItemDetailStatusType.StartedMore3, GrowthItemSegmentType.Competency, _enterpriseTeam.TeamId);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_EntGis_Enterprise_StartedWithAge_All_OK()
        {
            VerifySetup(_classInitFailed);
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Enterprise, GrowthItemStatusType.StartedWithAge, 
                GrowthItemDetailStatusType.StartedMore3, GrowthItemSegmentType.All, _enterpriseTeam.TeamId);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_EntGis_Enterprise_Finished_Category_OK()
        {
            VerifySetup(_classInitFailed);
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Enterprise, GrowthItemStatusType.Finished, 
                GrowthItemDetailStatusType.Finished, GrowthItemSegmentType.Category, _enterpriseTeam.TeamId);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_EntGis_Enterprise_Finished_Competency_OK()
        {
            VerifySetup(_classInitFailed);
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Enterprise, GrowthItemStatusType.Finished, 
                GrowthItemDetailStatusType.Finished, GrowthItemSegmentType.Competency, _enterpriseTeam.TeamId);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_EntGis_Enterprise_Finished_All_OK()
        {
            VerifySetup(_classInitFailed);
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Enterprise, GrowthItemStatusType.Finished, 
                GrowthItemDetailStatusType.Finished, GrowthItemSegmentType.All, _enterpriseTeam.TeamId);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_EntGis_Enterprise_Cancelled_Category_OK()
        {
            VerifySetup(_classInitFailed);
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Enterprise, GrowthItemStatusType.Cancelled, 
                GrowthItemDetailStatusType.Cancelled, GrowthItemSegmentType.Category, _enterpriseTeam.TeamId);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_EntGis_Enterprise_Cancelled_Competency_OK()
        {
            VerifySetup(_classInitFailed);
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Enterprise, GrowthItemStatusType.Cancelled, 
                GrowthItemDetailStatusType.Cancelled, GrowthItemSegmentType.Competency, _enterpriseTeam.TeamId);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_EntGis_Enterprise_Cancelled_All_OK()
        {
            VerifySetup(_classInitFailed);
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Enterprise, GrowthItemStatusType.Cancelled, 
                GrowthItemDetailStatusType.Cancelled, GrowthItemSegmentType.All, _enterpriseTeam.TeamId);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_EntGis_Enterprise_OnHold_Category_OK()
        {
            VerifySetup(_classInitFailed);
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Enterprise, GrowthItemStatusType.OnHold, 
                GrowthItemDetailStatusType.OnHold, GrowthItemSegmentType.Category, _enterpriseTeam.TeamId);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_EntGis_Enterprise_OnHold_Competency_OK()
        {
            VerifySetup(_classInitFailed);
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Enterprise, GrowthItemStatusType.OnHold, 
                GrowthItemDetailStatusType.OnHold, GrowthItemSegmentType.Competency, _enterpriseTeam.TeamId);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_EntGis_Enterprise_OnHold_All_OK()
        {
            VerifySetup(_classInitFailed);
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Enterprise, GrowthItemStatusType.OnHold, 
                GrowthItemDetailStatusType.OnHold, GrowthItemSegmentType.All, _enterpriseTeam.TeamId);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_EntGis_Enterprise_Committed_Category_OK()
        {
            VerifySetup(_classInitFailed);
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Enterprise, GrowthItemStatusType.Committed, 
                GrowthItemDetailStatusType.Committed, GrowthItemSegmentType.Category, _enterpriseTeam.TeamId);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_EntGis_Enterprise_Committed_Competency_OK()
        {
            VerifySetup(_classInitFailed);
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Enterprise, GrowthItemStatusType.Committed, 
                GrowthItemDetailStatusType.Committed, GrowthItemSegmentType.Competency, _enterpriseTeam.TeamId);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_EntGis_Enterprise_Committed_All_OK()
        {
            VerifySetup(_classInitFailed);
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Enterprise, GrowthItemStatusType.Committed, 
                GrowthItemDetailStatusType.Committed, GrowthItemSegmentType.All, _enterpriseTeam.TeamId);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_EntGis_MultiTeam_NotStarted_Category_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = _multiTeam.Children.Select(c => c.Name).ToList();
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Enterprise, GrowthItemStatusType.NotStarted, 
                GrowthItemDetailStatusType.InBacklog, GrowthItemSegmentType.Category, _multiTeam.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_EntGis_MultiTeam_NotStarted_Competency_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = _multiTeam.Children.Select(c => c.Name).ToList();
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Enterprise, GrowthItemStatusType.NotStarted, 
                GrowthItemDetailStatusType.InBacklog, GrowthItemSegmentType.Competency, _multiTeam.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_EntGis_MultiTeam_NotStarted_All_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = _multiTeam.Children.Select(c => c.Name).ToList();
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Enterprise, GrowthItemStatusType.NotStarted, 
                GrowthItemDetailStatusType.InBacklog, GrowthItemSegmentType.All, _multiTeam.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_EntGis_MultiTeam_Started_Category_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = _multiTeam.Children.Select(c => c.Name).ToList();
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Enterprise, GrowthItemStatusType.Started, 
                GrowthItemDetailStatusType.Started, GrowthItemSegmentType.Category, _multiTeam.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_EntGis_MultiTeam_Started_Competency_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = _multiTeam.Children.Select(c => c.Name).ToList();
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Enterprise, GrowthItemStatusType.Started, 
                GrowthItemDetailStatusType.Started, GrowthItemSegmentType.Competency, _multiTeam.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_EntGis_MultiTeam_Started_All_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = _multiTeam.Children.Select(c => c.Name).ToList();
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Enterprise, GrowthItemStatusType.Started, 
                GrowthItemDetailStatusType.Started, GrowthItemSegmentType.All, _multiTeam.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_EntGis_MultiTeam_StartedWithAge_Category_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = _multiTeam.Children.Select(c => c.Name).ToList();
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Enterprise, GrowthItemStatusType.StartedWithAge, 
                GrowthItemDetailStatusType.StartedMore3, GrowthItemSegmentType.Category, _multiTeam.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_EntGis_MultiTeam_StartedWithAge_Competency_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = _multiTeam.Children.Select(c => c.Name).ToList();
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Enterprise, GrowthItemStatusType.StartedWithAge, 
                GrowthItemDetailStatusType.StartedMore3, GrowthItemSegmentType.Competency, _multiTeam.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_EntGis_MultiTeam_StartedWithAge_All_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = _multiTeam.Children.Select(c => c.Name).ToList();
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Enterprise, GrowthItemStatusType.StartedWithAge, 
                GrowthItemDetailStatusType.StartedMore3, GrowthItemSegmentType.All, _multiTeam.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_EntGis_MultiTeam_Finished_Category_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = _multiTeam.Children.Select(c => c.Name).ToList();
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Enterprise, GrowthItemStatusType.Finished, 
                GrowthItemDetailStatusType.Finished, GrowthItemSegmentType.Category, _multiTeam.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_EntGis_MultiTeam_Finished_Competency_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = _multiTeam.Children.Select(c => c.Name).ToList();
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Enterprise, GrowthItemStatusType.Finished, 
                GrowthItemDetailStatusType.Finished, GrowthItemSegmentType.Competency, _multiTeam.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_EntGis_MultiTeam_Finished_All_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = _multiTeam.Children.Select(c => c.Name).ToList();
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Enterprise, GrowthItemStatusType.Finished, 
                GrowthItemDetailStatusType.Finished, GrowthItemSegmentType.All, _multiTeam.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_EntGis_MultiTeam_Cancelled_Category_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = _multiTeam.Children.Select(c => c.Name).ToList();
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Enterprise, GrowthItemStatusType.Cancelled, 
                GrowthItemDetailStatusType.Cancelled, GrowthItemSegmentType.Category, _multiTeam.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_EntGis_MultiTeam_Cancelled_Competency_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = _multiTeam.Children.Select(c => c.Name).ToList();
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Enterprise, GrowthItemStatusType.Cancelled, 
                GrowthItemDetailStatusType.Cancelled, GrowthItemSegmentType.Competency, _multiTeam.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_EntGis_MultiTeam_Cancelled_All_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = _multiTeam.Children.Select(c => c.Name).ToList();
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Enterprise, GrowthItemStatusType.Cancelled, 
                GrowthItemDetailStatusType.Cancelled, GrowthItemSegmentType.All, _multiTeam.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_EntGis_MultiTeam_OnHold_Category_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = _multiTeam.Children.Select(c => c.Name).ToList();
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Enterprise, GrowthItemStatusType.OnHold, 
                GrowthItemDetailStatusType.OnHold, GrowthItemSegmentType.Category, _multiTeam.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_EntGis_MultiTeam_OnHold_Competency_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = _multiTeam.Children.Select(c => c.Name).ToList();
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Enterprise, GrowthItemStatusType.OnHold, 
                GrowthItemDetailStatusType.OnHold, GrowthItemSegmentType.Competency, _multiTeam.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_EntGis_MultiTeam_OnHold_All_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = _multiTeam.Children.Select(c => c.Name).ToList();
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Enterprise, GrowthItemStatusType.OnHold, 
                GrowthItemDetailStatusType.OnHold, GrowthItemSegmentType.All, _multiTeam.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_EntGis_MultiTeam_Committed_Category_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = _multiTeam.Children.Select(c => c.Name).ToList();
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Enterprise, GrowthItemStatusType.Committed, 
                GrowthItemDetailStatusType.Committed, GrowthItemSegmentType.Category, _multiTeam.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_EntGis_MultiTeam_Committed_Competency_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = _multiTeam.Children.Select(c => c.Name).ToList();
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Enterprise, GrowthItemStatusType.Committed, 
                GrowthItemDetailStatusType.Committed, GrowthItemSegmentType.Competency, _multiTeam.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_EntGis_MultiTeam_Committed_All_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = _multiTeam.Children.Select(c => c.Name).ToList();
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Enterprise, GrowthItemStatusType.Committed, 
                GrowthItemDetailStatusType.Committed, GrowthItemSegmentType.All, _multiTeam.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_EntGis_Team_NotStarted_Category_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = new List<string> {_team.Name};
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Enterprise, GrowthItemStatusType.NotStarted, 
                GrowthItemDetailStatusType.InBacklog, GrowthItemSegmentType.Category, _multiTeam.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_EntGis_Team_NotStarted_Competency_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = new List<string> {_team.Name};
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Enterprise, GrowthItemStatusType.NotStarted, 
                GrowthItemDetailStatusType.InBacklog, GrowthItemSegmentType.Competency, _team.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_EntGis_Team_NotStarted_All_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = new List<string> {_team.Name};
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Enterprise, GrowthItemStatusType.NotStarted, 
                GrowthItemDetailStatusType.InBacklog, GrowthItemSegmentType.All, _team.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_EntGis_Team_Started_Category_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = new List<string> {_team.Name};
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Enterprise, GrowthItemStatusType.Started, 
                GrowthItemDetailStatusType.Started, GrowthItemSegmentType.Category, _team.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_EntGis_Team_Started_Competency_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = new List<string> {_team.Name};
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Enterprise, GrowthItemStatusType.Started, 
                GrowthItemDetailStatusType.Started, GrowthItemSegmentType.Competency, _team.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_EntGis_Team_Started_All_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = new List<string> {_team.Name};
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Enterprise, GrowthItemStatusType.Started, 
                GrowthItemDetailStatusType.Started, GrowthItemSegmentType.All, _team.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_EntGis_Team_StartedWithAge_Category_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = new List<string> {_team.Name};
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Enterprise, GrowthItemStatusType.StartedWithAge, 
                GrowthItemDetailStatusType.StartedMore3, GrowthItemSegmentType.Category, _team.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_EntGis_Team_StartedWithAge_Competency_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = new List<string> {_team.Name};
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Enterprise, GrowthItemStatusType.StartedWithAge, 
                GrowthItemDetailStatusType.StartedMore3, GrowthItemSegmentType.Competency, _team.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_EntGis_Team_StartedWithAge_All_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = new List<string> {_team.Name};
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Enterprise, GrowthItemStatusType.StartedWithAge, 
                GrowthItemDetailStatusType.StartedMore3, GrowthItemSegmentType.All, _team.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_EntGis_Team_Finished_Category_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = new List<string> {_team.Name};
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Enterprise, GrowthItemStatusType.Finished, 
                GrowthItemDetailStatusType.Finished, GrowthItemSegmentType.Category, _team.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_EntGis_Team_Finished_Competency_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = new List<string> {_team.Name};
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Enterprise, GrowthItemStatusType.Finished, 
                GrowthItemDetailStatusType.Finished, GrowthItemSegmentType.Competency, _team.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_EntGis_Team_Finished_All_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = new List<string> {_team.Name};
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Enterprise, GrowthItemStatusType.Finished, 
                GrowthItemDetailStatusType.Finished, GrowthItemSegmentType.All, _team.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_EntGis_Team_Cancelled_Category_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = new List<string> {_team.Name};
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Enterprise, GrowthItemStatusType.Cancelled, 
                GrowthItemDetailStatusType.Cancelled, GrowthItemSegmentType.Category, _team.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_EntGis_Team_Cancelled_Competency_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = new List<string> {_team.Name};
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Enterprise, GrowthItemStatusType.Cancelled, 
                GrowthItemDetailStatusType.Cancelled, GrowthItemSegmentType.Competency, _team.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_EntGis_Team_Cancelled_All_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = new List<string> {_team.Name};
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Enterprise, GrowthItemStatusType.Cancelled, 
                GrowthItemDetailStatusType.Cancelled, GrowthItemSegmentType.All, _team.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_EntGis_Team_OnHold_Category_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = new List<string> {_team.Name};
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Enterprise, GrowthItemStatusType.OnHold, 
                GrowthItemDetailStatusType.OnHold, GrowthItemSegmentType.Category, _team.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_EntGis_Team_OnHold_Competency_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = new List<string> {_team.Name};
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Enterprise, GrowthItemStatusType.OnHold, 
                GrowthItemDetailStatusType.OnHold, GrowthItemSegmentType.Competency, _team.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_EntGis_Team_OnHold_All_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = new List<string> {_team.Name};
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Enterprise, GrowthItemStatusType.OnHold, 
                GrowthItemDetailStatusType.OnHold, GrowthItemSegmentType.All, _team.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_EntGis_Team_Committed_Category_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = new List<string> {_team.Name};
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Enterprise, GrowthItemStatusType.Committed, 
                GrowthItemDetailStatusType.Committed, GrowthItemSegmentType.Category, _team.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_EntGis_Team_Committed_Competency_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = new List<string> {_team.Name};
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Enterprise, GrowthItemStatusType.Committed, 
                GrowthItemDetailStatusType.Committed, GrowthItemSegmentType.Competency, _team.TeamId, teamNames);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_EntGis_Team_Committed_All_OK()
        {
            VerifySetup(_classInitFailed);
            var teamNames = new List<string> {_team.Name};
            await GrowthPlanItemDetailsValidator(GrowthItemCategory.Enterprise, GrowthItemStatusType.Committed, 
                GrowthItemDetailStatusType.Committed, GrowthItemSegmentType.All, _team.TeamId, teamNames);
        }

        // 400
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_BadRequest()
        {
            // given
            var client = await GetInsightsAuthenticatedClient();
            var request = InsightsFactory.GetGrowthPlanDetailsAnalyticsRequest(GrowthItemCategory.Individual,
                GrowthItemStatusType.Started, GrowthItemDetailStatusType.Started, "", GrowthItemSegmentType.Category, 0);
            // when
            var response = await client.PostAsync<IList<string>>(
                RequestUris.AnalyticsGrowthPlanItemsDetails(Company.InsightsId), request);

            // then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status code doesn't match.");
            Assert.AreEqual(1, response.Dto.Count, "Error message count does not match");
            Assert.AreEqual("'Growth Item Category' has a range of values which does not include '4'.", 
                response.Dto.First(), "Error message does not match");
        }

        // 401
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_Unauthorized()
        {
            // given
            var client = GetUnauthenticatedClient();
            var request = InsightsFactory.GetGrowthPlanDetailsAnalyticsRequest(GrowthItemCategory.Team,
                    GrowthItemStatusType.Started, GrowthItemDetailStatusType.Started, "", GrowthItemSegmentType.Category, 0);
            // when
            var response = await client.PostAsync(RequestUris.AnalyticsGrowthPlanItemsDetails(Company.InsightsId),
                request.ToStringContent());

            // then
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status code doesn't match.");
        }

        // 403
        [TestMethod]
        [TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_GrowthPlanItemsDetails_Post_Forbidden()
        {
            // given
            var client = await GetInsightsAuthenticatedClient();
            var request = InsightsFactory.GetGrowthPlanDetailsAnalyticsRequest(GrowthItemCategory.Individual,
                GrowthItemStatusType.Started, GrowthItemDetailStatusType.Started, "", GrowthItemSegmentType.Category, 0);
            // when
            var response = await client.PostAsync<IList<string>>(
                RequestUris.AnalyticsGrowthPlanItemsDetails(SharedConstants.FakeCompanyId), request);

            // then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status code doesn't match.");
        }

        private async Task GrowthPlanItemDetailsValidator(GrowthItemCategory category, GrowthItemStatusType status,
            GrowthItemDetailStatusType detailStatus, GrowthItemSegmentType segment, int teamId, IList<string> teamNames = null)
        {
            var types = InsightsFactory.GetGiTypesAndCounts(GrowthItems, category, status, segment);
            // given
            var client = await GetInsightsAuthenticatedClient();
            foreach (var type in types)
            {
                var selectedName = (type.Key == "") ? "Not Selected" : type.Key;
                var request = InsightsFactory.GetGrowthPlanDetailsAnalyticsRequest(category,
                    status, detailStatus, selectedName, segment, teamId);
                var expected = InsightsFactory.GetFilteredGrowthItems(GrowthItems, category, status, teamNames);
                expected = segment switch
                {
                    GrowthItemSegmentType.Category => expected.Where(gi => gi.Type == type.Key).ToList(),
                    GrowthItemSegmentType.Competency => expected.Where(gi => gi.CompetencyTarget == type.Key).ToList(),
                    GrowthItemSegmentType.All => expected.Where(gi => gi.Status == type.Key).ToList(),
                    _ => throw new Exception($"<{segment:G}> cannot be used due to lack of data.")
                };

                // when
                var response = await client.PostAsync<PagedResponse<GrowthPlanDetailsResponse>>(
                    RequestUris.AnalyticsGrowthPlanItemsDetails(Company.InsightsId), request);

                // then
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status code doesn't match.");
                Assert.AreEqual(expected.Count, response.Dto.Results.Count, 
                    $"The count for status: <{status:G}>, category: <{category:G}>, and type: <{type.Key}> does not match");
                
                foreach (var gi in expected)
                {
                    var actual = response.Dto.Results.FirstOrDefault(g => g.Title == gi.Title)
                        .CheckForNull($"<{gi.Title}> was not found in the response.");
                    Assert.AreEqual(gi.Category.ToString("G"), actual.Type, $"Type does not match for <{gi.Title}>");
                    Assert.AreEqual(gi.Team, actual.Team, $"Team does not match for <{gi.Title}>");
                    var giTeamId = _companyHierarchy.GetTeamByName(gi.Team).TeamId;
                    Assert.AreEqual(giTeamId, actual.TeamId, $"TeamId does not match for <{gi.Title}>");
                    Assert.AreEqual(gi.Description, (actual.Description ?? string.Empty).Replace("\n", "").Trim(), 
                        $"Description does not match for <{gi.Title}>");
                    Assert.IsTrue(actual.CreatedDate != null && actual.CreatedDate.Value.CompareTo(gi.Created) == 0, 
                        $"Expected: <{gi.Created:d}> Actual: <{actual.CreatedDate:d}>. CreatedDate does not match for <{gi.Title}>");
                    Assert.AreEqual(gi.Priority, actual.Priority, $"Priority does not match for <{gi.Title}>");
                    Assert.AreEqual(gi.Status, actual.Status, $"Status does not match for <{gi.Title}>");
                }
            }
        }
    }
}