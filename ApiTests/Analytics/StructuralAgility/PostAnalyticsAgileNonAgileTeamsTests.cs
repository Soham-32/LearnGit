using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ApiTests.v1.Endpoints;
using AtCommon.Api;
using AtCommon.Dtos.Analytics.StructuralAgility;
using AtCommon.Dtos.Companies;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiTests.Analytics.StructuralAgility
{
    [TestClass]
    [TestCategory("Insights")]
    public class PostAnalyticsAgileNonAgileTeamsTests : BaseV1Test
    {
        private static bool _classInitFailed;
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
        public async Task Analytics_AgileNonAgileTeams_Post_Company_Average_AgileAdoption_OK()
        {

            var request = new AgileNonAgileTeamsRequest
            {
                WidgetType = StructuralAgilityWidgetType.Average,
                TeamIds = new List<int> { 0 },
                SelectedTeamCategoryName = "Agile Adoption",
                SelectedTeamParents = "0"
            };

            var expectedResponse = new List<AgileNonAgileTeamsResponse>
            {
                new AgileNonAgileTeamsResponse { WidgetType = "Summary", TeamCategorySelection = "In-Progress", 
                    TeamCount = 2, CategoryRank = 1, ChildTeamName = "" },
                new AgileNonAgileTeamsResponse { WidgetType = "Summary", TeamCategorySelection = "Activated", 
                    TeamCount = 1, CategoryRank = 2, ChildTeamName = "" },
                new AgileNonAgileTeamsResponse { WidgetType = "Summary", TeamCategorySelection = "Other", 
                    TeamCount = 1, CategoryRank = 3, ChildTeamName = "" },
                new AgileNonAgileTeamsResponse { TeamCategorySelection = "Total", TeamCount = 4}
            };

            await AgileNonAgileValidator(request, expectedResponse);

        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_AgileNonAgileTeams_Post_Company_Average_TeamFormation_OK()
        {

            var request = new AgileNonAgileTeamsRequest
            {
                WidgetType = StructuralAgilityWidgetType.Average,
                TeamIds = new List<int> { 0 },
                SelectedTeamCategoryName = "Team Formation",
                SelectedTeamParents = "0"
            };

            var expectedResponse = new List<AgileNonAgileTeamsResponse>
            {
                new AgileNonAgileTeamsResponse { WidgetType = "Summary", TeamCategorySelection = "Formed", 
                    TeamCount = 1, CategoryRank = 1, ChildTeamName = "" },
                new AgileNonAgileTeamsResponse { WidgetType = "Summary", TeamCategorySelection = "Forming", 
                    TeamCount = 1, CategoryRank = 2, ChildTeamName = "" },
                new AgileNonAgileTeamsResponse { WidgetType = "Summary", TeamCategorySelection = "Not Formed", 
                    TeamCount = 1, CategoryRank = 3, ChildTeamName = "" },
                new AgileNonAgileTeamsResponse { WidgetType = "Summary", TeamCategorySelection = "Other", 
                    TeamCount = 1, CategoryRank = 4, ChildTeamName = "" },
                new AgileNonAgileTeamsResponse { TeamCategorySelection = "Total", TeamCount = 4}
            };

            await AgileNonAgileValidator(request, expectedResponse);

        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_AgileNonAgileTeams_Post_Company_Average_WorkType_OK()
        {

            var request = new AgileNonAgileTeamsRequest
            {
                WidgetType = StructuralAgilityWidgetType.Average,
                TeamIds = new List<int> { 0 },
                SelectedTeamCategoryName = "Work Type",
                SelectedTeamParents = "0"
            };

            var expectedResponse = new List<AgileNonAgileTeamsResponse>
            {
                new AgileNonAgileTeamsResponse { WidgetType = "Summary", TeamCategorySelection = "Software Delivery", 
                    TeamCount = 2, ChildTeamName = "", CategoryRank = 1 },
                new AgileNonAgileTeamsResponse { WidgetType = "Summary", TeamCategorySelection = "Business Operations", 
                    TeamCount = 1, ChildTeamName = "", CategoryRank = 2 },
                new AgileNonAgileTeamsResponse { WidgetType = "Summary", TeamCategorySelection = "Service and Support", 
                    TeamCount = 1, ChildTeamName = "", CategoryRank = 3 },
                new AgileNonAgileTeamsResponse { TeamCategorySelection = "Total", TeamCount = 4}
            };

            await AgileNonAgileValidator(request, expectedResponse);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_AgileNonAgileTeams_Post_Company_Distribution_AgileAdoption_OK()
        {

            var request = new AgileNonAgileTeamsRequest
            {
                WidgetType = StructuralAgilityWidgetType.Distribution,
                TeamIds = new List<int> { 0 },
                SelectedTeamCategoryName = "Agile Adoption",
                SelectedTeamParents = "0"
            };

            var expectedResponse = new List<AgileNonAgileTeamsResponse>
            {
                new AgileNonAgileTeamsResponse { TeamCategorySelection = "In-Progress", TeamCount = 2,
                    ChildTeamId = _enterpriseTeam.TeamId, ChildTeamName = _enterpriseTeam.Name },
                new AgileNonAgileTeamsResponse { TeamCategorySelection = "Activated", TeamCount = 1,
                    ChildTeamId = _enterpriseTeam.TeamId, ChildTeamName = _enterpriseTeam.Name },
                new AgileNonAgileTeamsResponse { TeamCategorySelection = "Other", TeamCount = 1,
                    ChildTeamId = _enterpriseTeam.TeamId, ChildTeamName = _enterpriseTeam.Name }
            };

            await AgileNonAgileValidator(request, expectedResponse);

        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_AgileNonAgileTeams_Post_Company_Distribution_TeamFormation_OK()
        {

            var request = new AgileNonAgileTeamsRequest
            {
                WidgetType = StructuralAgilityWidgetType.Distribution,
                TeamIds = new List<int> { 0 },
                SelectedTeamCategoryName = "Team Formation",
                SelectedTeamParents = "0"
            };

            var expectedResponse = new List<AgileNonAgileTeamsResponse>
            {
                new AgileNonAgileTeamsResponse { TeamCategorySelection = "Formed", TeamCount = 1,
                    ChildTeamId = _enterpriseTeam.TeamId, ChildTeamName = _enterpriseTeam.Name },
                new AgileNonAgileTeamsResponse { TeamCategorySelection = "Forming", TeamCount = 1,
                    ChildTeamId = _enterpriseTeam.TeamId, ChildTeamName = _enterpriseTeam.Name },
                new AgileNonAgileTeamsResponse { TeamCategorySelection = "Not Formed", TeamCount = 1,
                    ChildTeamId = _enterpriseTeam.TeamId, ChildTeamName = _enterpriseTeam.Name },
                new AgileNonAgileTeamsResponse { TeamCategorySelection = "Other", TeamCount = 1,
                    ChildTeamId = _enterpriseTeam.TeamId, ChildTeamName = _enterpriseTeam.Name }
            };

            await AgileNonAgileValidator(request, expectedResponse);

        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_AgileNonAgileTeams_Post_Company_Distribution_WorkType_OK()
        {

            var request = new AgileNonAgileTeamsRequest()
            {
                WidgetType = StructuralAgilityWidgetType.Distribution,
                TeamIds = new List<int> { 0 },
                SelectedTeamCategoryName = "Work Type",
                SelectedTeamParents = "0"
            };

            var expectedResponse = new List<AgileNonAgileTeamsResponse>
            {
                new AgileNonAgileTeamsResponse { TeamCategorySelection = "Software Delivery", TeamCount = 2,
                    ChildTeamId = _enterpriseTeam.TeamId, ChildTeamName = _enterpriseTeam.Name },
                new AgileNonAgileTeamsResponse { TeamCategorySelection = "Business Operations", TeamCount = 1,
                    ChildTeamId = _enterpriseTeam.TeamId, ChildTeamName = _enterpriseTeam.Name },
                new AgileNonAgileTeamsResponse { TeamCategorySelection = "Service and Support", TeamCount = 1,
                    ChildTeamId = _enterpriseTeam.TeamId, ChildTeamName = _enterpriseTeam.Name }
            };

            await AgileNonAgileValidator(request, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_AgileNonAgileTeams_Post_Enterprise_Average_AgileAdoption_OK()
        {
            VerifySetup(_classInitFailed);
            var request = new AgileNonAgileTeamsRequest
            {
                WidgetType = StructuralAgilityWidgetType.Average,
                TeamIds = new List<int> { _enterpriseTeam.TeamId },
                SelectedTeamCategoryName = "Agile Adoption",
                SelectedTeamParents = "0"
            };

            var expectedResponse = new List<AgileNonAgileTeamsResponse>
            {
                new AgileNonAgileTeamsResponse { WidgetType = "Summary", TeamCategorySelection = "In-Progress", 
                    TeamCount = 2, CategoryRank = 1, ChildTeamName = "" },
                new AgileNonAgileTeamsResponse { WidgetType = "Summary", TeamCategorySelection = "Activated", 
                    TeamCount = 1, CategoryRank = 2, ChildTeamName = "" },
                new AgileNonAgileTeamsResponse { WidgetType = "Summary", TeamCategorySelection = "Other", 
                    TeamCount = 1, CategoryRank = 3, ChildTeamName = "" },
                new AgileNonAgileTeamsResponse { TeamCategorySelection = "Total", TeamCount = 4}
            };

            await AgileNonAgileValidator(request, expectedResponse);

        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_AgileNonAgileTeams_Post_Enterprise_Average_TeamFormation_OK()
        {
            VerifySetup(_classInitFailed);
            var request = new AgileNonAgileTeamsRequest
            {
                WidgetType = StructuralAgilityWidgetType.Average,
                TeamIds = new List<int> { _enterpriseTeam.TeamId },
                SelectedTeamCategoryName = "Team Formation",
                SelectedTeamParents = "0"
            };

            var expectedResponse = new List<AgileNonAgileTeamsResponse>
            {
                new AgileNonAgileTeamsResponse { WidgetType = "Summary", TeamCategorySelection = "Formed", 
                    TeamCount = 1, CategoryRank = 1, ChildTeamName = "" },
                new AgileNonAgileTeamsResponse { WidgetType = "Summary", TeamCategorySelection = "Forming", 
                    TeamCount = 1, CategoryRank = 2, ChildTeamName = "" },
                new AgileNonAgileTeamsResponse { WidgetType = "Summary", TeamCategorySelection = "Not Formed", 
                    TeamCount = 1, CategoryRank = 3, ChildTeamName = "" },
                new AgileNonAgileTeamsResponse { WidgetType = "Summary", TeamCategorySelection = "Other", 
                    TeamCount = 1, CategoryRank = 4, ChildTeamName = "" },
                new AgileNonAgileTeamsResponse { TeamCategorySelection = "Total", TeamCount = 4}
            };

            await AgileNonAgileValidator(request, expectedResponse);

        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_AgileNonAgileTeams_Post_Enterprise_Average_WorkType_OK()
        {
            VerifySetup(_classInitFailed);
            var request = new AgileNonAgileTeamsRequest
            {
                WidgetType = StructuralAgilityWidgetType.Average,
                TeamIds = new List<int> { _enterpriseTeam.TeamId },
                SelectedTeamCategoryName = "Work Type",
                SelectedTeamParents = "0"
            };

            var expectedResponse = new List<AgileNonAgileTeamsResponse>
            {
                new AgileNonAgileTeamsResponse { WidgetType = "Summary", TeamCategorySelection = "Software Delivery", 
                    TeamCount = 2, ChildTeamName = "", CategoryRank = 1 },
                new AgileNonAgileTeamsResponse { WidgetType = "Summary", TeamCategorySelection = "Business Operations", 
                    TeamCount = 1, ChildTeamName = "", CategoryRank = 2 },
                new AgileNonAgileTeamsResponse { WidgetType = "Summary", TeamCategorySelection = "Service and Support", 
                    TeamCount = 1, ChildTeamName = "", CategoryRank = 3 },
                new AgileNonAgileTeamsResponse { TeamCategorySelection = "Total", TeamCount = 4}
            };

            await AgileNonAgileValidator(request, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_AgileNonAgileTeams_Post_Enterprise_Distribution_AgileAdoption_OK()
        {
            VerifySetup(_classInitFailed);
            var request = new AgileNonAgileTeamsRequest
            {
                WidgetType = StructuralAgilityWidgetType.Distribution,
                TeamIds = new List<int> { _enterpriseTeam.TeamId },
                SelectedTeamCategoryName = "Agile Adoption",
                SelectedTeamParents = "0"
            };

            var team1 = _enterpriseTeam.Children.SingleOrDefault(t => t.Name == SharedConstants.InsightsMultiTeam1).CheckForNull();
            var team2 = _enterpriseTeam.Children.SingleOrDefault(t => t.Name == SharedConstants.InsightsMultiTeam2).CheckForNull();

            var expectedResponse = new List<AgileNonAgileTeamsResponse>
            {
                new AgileNonAgileTeamsResponse { TeamCategorySelection = "In-Progress", TeamCount = 1,
                    ChildTeamId = team1.TeamId, ChildTeamName = team1.Name },
                new AgileNonAgileTeamsResponse { TeamCategorySelection = "Activated", TeamCount = 1,
                    ChildTeamId = team1.TeamId, ChildTeamName = team1.Name },
                new AgileNonAgileTeamsResponse { TeamCategorySelection = "Other", TeamCount = 0,
                    ChildTeamId = team1.TeamId, ChildTeamName = team1.Name },
                new AgileNonAgileTeamsResponse { TeamCategorySelection = "In-Progress", TeamCount = 1,
                    ChildTeamId = team2.TeamId, ChildTeamName = team2.Name },
                new AgileNonAgileTeamsResponse { TeamCategorySelection = "Activated", TeamCount = 0,
                    ChildTeamId = team2.TeamId, ChildTeamName = team2.Name },
                new AgileNonAgileTeamsResponse { TeamCategorySelection = "Other", TeamCount = 1,
                    ChildTeamId = team2.TeamId, ChildTeamName = team2.Name }
            };

            await AgileNonAgileValidator(request, expectedResponse);

        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_AgileNonAgileTeams_Post_Enterprise_Distribution_TeamFormation_OK()
        {
            VerifySetup(_classInitFailed);
            var request = new AgileNonAgileTeamsRequest
            {
                WidgetType = StructuralAgilityWidgetType.Distribution,
                TeamIds = new List<int> { _enterpriseTeam.TeamId },
                SelectedTeamCategoryName = "Team Formation",
                SelectedTeamParents = "0"
            };

            var team1 = _enterpriseTeam.Children.SingleOrDefault(t => t.Name == SharedConstants.InsightsMultiTeam1).CheckForNull();
            var team2 = _enterpriseTeam.Children.SingleOrDefault(t => t.Name == SharedConstants.InsightsMultiTeam2).CheckForNull();

            var expectedResponse = new List<AgileNonAgileTeamsResponse>
            {
                new AgileNonAgileTeamsResponse { TeamCategorySelection = "Formed", TeamCount = 1,
                    ChildTeamId = team1.TeamId, ChildTeamName = team1.Name },
                new AgileNonAgileTeamsResponse { TeamCategorySelection = "Forming", TeamCount = 1,
                    ChildTeamId = team1.TeamId, ChildTeamName = team1.Name },
                new AgileNonAgileTeamsResponse { TeamCategorySelection = "Not Formed", TeamCount = 0,
                    ChildTeamId = team1.TeamId, ChildTeamName = team1.Name },
                new AgileNonAgileTeamsResponse { TeamCategorySelection = "Other", TeamCount = 0,
                    ChildTeamId = team1.TeamId, ChildTeamName = team1.Name },
                new AgileNonAgileTeamsResponse { TeamCategorySelection = "Formed", TeamCount = 0,
                    ChildTeamId = team2.TeamId, ChildTeamName = team2.Name },
                new AgileNonAgileTeamsResponse { TeamCategorySelection = "Forming", TeamCount = 0,
                    ChildTeamId = team2.TeamId, ChildTeamName = team2.Name },
                new AgileNonAgileTeamsResponse { TeamCategorySelection = "Not Formed", TeamCount = 1,
                    ChildTeamId = team2.TeamId, ChildTeamName = team2.Name },
                new AgileNonAgileTeamsResponse { TeamCategorySelection = "Other", TeamCount = 1,
                    ChildTeamId = team2.TeamId, ChildTeamName = team2.Name },
            };

            await AgileNonAgileValidator(request, expectedResponse);

        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_AgileNonAgileTeams_Post_Enterprise_Distribution_WorkType_OK()
        {
            VerifySetup(_classInitFailed);
            var request = new AgileNonAgileTeamsRequest()
            {
                WidgetType = StructuralAgilityWidgetType.Distribution,
                TeamIds = new List<int> { _enterpriseTeam.TeamId },
                SelectedTeamCategoryName = "Work Type",
                SelectedTeamParents = "0"
            };

            var team1 = _enterpriseTeam.Children.SingleOrDefault(t => t.Name == SharedConstants.InsightsMultiTeam1).CheckForNull();
            var team2 = _enterpriseTeam.Children.SingleOrDefault(t => t.Name == SharedConstants.InsightsMultiTeam2).CheckForNull();

            var expectedResponse = new List<AgileNonAgileTeamsResponse>
            {
                new AgileNonAgileTeamsResponse { TeamCategorySelection = "Software Delivery", TeamCount = 1,
                    ChildTeamId = team1.TeamId, ChildTeamName = team1.Name },
                new AgileNonAgileTeamsResponse { TeamCategorySelection = "Business Operations", TeamCount = 1,
                    ChildTeamId = team1.TeamId, ChildTeamName = team1.Name },
                new AgileNonAgileTeamsResponse { TeamCategorySelection = "Service and Support", TeamCount = 0,
                    ChildTeamId = team1.TeamId, ChildTeamName = team1.Name },
                new AgileNonAgileTeamsResponse { TeamCategorySelection = "Software Delivery", TeamCount = 1,
                    ChildTeamId = team2.TeamId, ChildTeamName = team2.Name },
                new AgileNonAgileTeamsResponse { TeamCategorySelection = "Business Operations", TeamCount = 0,
                    ChildTeamId = team2.TeamId, ChildTeamName = team2.Name },
                new AgileNonAgileTeamsResponse { TeamCategorySelection = "Service and Support", TeamCount = 1,
                    ChildTeamId = team2.TeamId, ChildTeamName = team2.Name }
            };

            await AgileNonAgileValidator(request, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_AgileNonAgileTeams_Post_MultiTeam_Average_AgileAdoption_OK()
        {
            VerifySetup(_classInitFailed);
            var request = new AgileNonAgileTeamsRequest
            {
                WidgetType = StructuralAgilityWidgetType.Average,
                TeamIds = new List<int> { _multiTeam.TeamId },
                SelectedTeamCategoryName = "Agile Adoption",
                SelectedTeamParents = _multiTeam.ParentId.ToString()
            };

            var expectedResponse = new List<AgileNonAgileTeamsResponse>
            {
                new AgileNonAgileTeamsResponse { WidgetType = "Summary", TeamCategorySelection = "In-Progress", 
                    TeamCount = 1, CategoryRank = 2, ChildTeamName = "" },
                new AgileNonAgileTeamsResponse { WidgetType = "Summary", TeamCategorySelection = "Activated", 
                    TeamCount = 1, CategoryRank = 1, ChildTeamName = "" },
                new AgileNonAgileTeamsResponse { TeamCategorySelection = "Total", TeamCount = 2}
            };

            await AgileNonAgileValidator(request, expectedResponse);

        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_AgileNonAgileTeams_Post_MultiTeam_Average_TeamFormation_OK()
        {
            VerifySetup(_classInitFailed);
            var request = new AgileNonAgileTeamsRequest
            {
                WidgetType = StructuralAgilityWidgetType.Average,
                TeamIds = new List<int> { _multiTeam.TeamId },
                SelectedTeamCategoryName = "Team Formation",
                SelectedTeamParents = _multiTeam.ParentId.ToString()
            };

            var expectedResponse = new List<AgileNonAgileTeamsResponse>
            {
                new AgileNonAgileTeamsResponse { WidgetType = "Summary", TeamCategorySelection = "Formed", 
                    TeamCount = 1, CategoryRank = 1, ChildTeamName = "" },
                new AgileNonAgileTeamsResponse { WidgetType = "Summary", TeamCategorySelection = "Forming", 
                    TeamCount = 1, CategoryRank = 2, ChildTeamName = "" },
                new AgileNonAgileTeamsResponse { TeamCategorySelection = "Total", TeamCount = 2}
            };

            await AgileNonAgileValidator(request, expectedResponse);

        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_AgileNonAgileTeams_Post_MultiTeam_Average_WorkType_OK()
        {
            VerifySetup(_classInitFailed);
            var request = new AgileNonAgileTeamsRequest
            {
                WidgetType = StructuralAgilityWidgetType.Average,
                TeamIds = new List<int> { _multiTeam.TeamId },
                SelectedTeamCategoryName = "Work Type",
                SelectedTeamParents = _multiTeam.ParentId.ToString()
            };

            var expectedResponse = new List<AgileNonAgileTeamsResponse>
            {
                new AgileNonAgileTeamsResponse { WidgetType = "Summary", TeamCategorySelection = "Software Delivery", 
                    TeamCount = 1, ChildTeamName = "", CategoryRank = 2 },
                new AgileNonAgileTeamsResponse { WidgetType = "Summary", TeamCategorySelection = "Business Operations", 
                    TeamCount = 1, ChildTeamName = "", CategoryRank = 1 },
                new AgileNonAgileTeamsResponse { TeamCategorySelection = "Total", TeamCount = 2}
            };

            await AgileNonAgileValidator(request, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_AgileNonAgileTeams_Post_MultiTeam_Distribution_AgileAdoption_OK()
        {
            VerifySetup(_classInitFailed);
            var request = new AgileNonAgileTeamsRequest
            {
                WidgetType = StructuralAgilityWidgetType.Distribution,
                TeamIds = new List<int> { _multiTeam.TeamId },
                SelectedTeamCategoryName = "Agile Adoption",
                SelectedTeamParents = _multiTeam.ParentId.ToString()
            };

            var team1 = _multiTeam.Children.SingleOrDefault(t => t.Name == SharedConstants.InsightsIndividualTeam1).CheckForNull();
            var team2 = _multiTeam.Children.SingleOrDefault(t => t.Name == SharedConstants.InsightsIndividualTeam2).CheckForNull();

            var expectedResponse = new List<AgileNonAgileTeamsResponse>
            {
                new AgileNonAgileTeamsResponse { TeamCategorySelection = "In-Progress", TeamCount = 0,
                    ChildTeamId = team1.TeamId, ChildTeamName = team1.Name },
                new AgileNonAgileTeamsResponse { TeamCategorySelection = "Activated", TeamCount = 1,
                    ChildTeamId = team1.TeamId, ChildTeamName = team1.Name },
                new AgileNonAgileTeamsResponse { TeamCategorySelection = "In-Progress", TeamCount = 1,
                    ChildTeamId = team2.TeamId, ChildTeamName = team2.Name },
                new AgileNonAgileTeamsResponse { TeamCategorySelection = "Activated", TeamCount = 0,
                    ChildTeamId = team2.TeamId, ChildTeamName = team2.Name }

            };

            await AgileNonAgileValidator(request, expectedResponse);

        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_AgileNonAgileTeams_Post_MultiTeam_Distribution_TeamFormation_OK()
        {
            VerifySetup(_classInitFailed);
            var request = new AgileNonAgileTeamsRequest
            {
                WidgetType = StructuralAgilityWidgetType.Distribution,
                TeamIds = new List<int> { _multiTeam.TeamId },
                SelectedTeamCategoryName = "Team Formation",
                SelectedTeamParents = _multiTeam.ParentId.ToString()
            };

            var team1 = _multiTeam.Children.SingleOrDefault(t => t.Name == SharedConstants.InsightsIndividualTeam1).CheckForNull();
            var team2 = _multiTeam.Children.SingleOrDefault(t => t.Name == SharedConstants.InsightsIndividualTeam2).CheckForNull();

            var expectedResponse = new List<AgileNonAgileTeamsResponse>
            {
                new AgileNonAgileTeamsResponse { TeamCategorySelection = "Formed", TeamCount = 1,
                    ChildTeamId = team1.TeamId, ChildTeamName = team1.Name },
                new AgileNonAgileTeamsResponse { TeamCategorySelection = "Forming", TeamCount = 0,
                    ChildTeamId = team1.TeamId, ChildTeamName = team1.Name },
                new AgileNonAgileTeamsResponse { TeamCategorySelection = "Formed", TeamCount = 0,
                    ChildTeamId = team2.TeamId, ChildTeamName = team2.Name },
                new AgileNonAgileTeamsResponse { TeamCategorySelection = "Forming", TeamCount = 1,
                    ChildTeamId = team2.TeamId, ChildTeamName = team2.Name }

            };

            await AgileNonAgileValidator(request, expectedResponse);

        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_AgileNonAgileTeams_Post_MultiTeam_Distribution_WorkType_OK()
        {
            VerifySetup(_classInitFailed);
            var request = new AgileNonAgileTeamsRequest
            {
                WidgetType = StructuralAgilityWidgetType.Distribution,
                TeamIds = new List<int> { _multiTeam.TeamId },
                SelectedTeamCategoryName = "Work Type",
                SelectedTeamParents = _multiTeam.ParentId.ToString()
            };

            var team1 = _multiTeam.Children.SingleOrDefault(t => t.Name == SharedConstants.InsightsIndividualTeam1).CheckForNull();
            var team2 = _multiTeam.Children.SingleOrDefault(t => t.Name == SharedConstants.InsightsIndividualTeam2).CheckForNull();

            var expectedResponse = new List<AgileNonAgileTeamsResponse>
            {
                new AgileNonAgileTeamsResponse { TeamCategorySelection = "Software Delivery", TeamCount = 1,
                    ChildTeamId = team1.TeamId, ChildTeamName = team1.Name },
                new AgileNonAgileTeamsResponse { TeamCategorySelection = "Business Operations", TeamCount = 0,
                    ChildTeamId = team1.TeamId, ChildTeamName = team1.Name },
                new AgileNonAgileTeamsResponse { TeamCategorySelection = "Software Delivery", TeamCount = 0,
                    ChildTeamId = team2.TeamId, ChildTeamName = team2.Name },
                new AgileNonAgileTeamsResponse { TeamCategorySelection = "Business Operations", TeamCount = 1,
                    ChildTeamId = team2.TeamId, ChildTeamName = team2.Name }

            };

            await AgileNonAgileValidator(request, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_AgileNonAgileTeams_Post_Team_Average_AgileAdoption_OK()
        {
            VerifySetup(_classInitFailed);
            var request = new AgileNonAgileTeamsRequest
            {
                WidgetType = StructuralAgilityWidgetType.Average,
                TeamIds = new List<int> { _team.TeamId },
                SelectedTeamCategoryName = "Agile Adoption",
                SelectedTeamParents = _team.ParentId.ToString()
            };

            var expectedResponse = new List<AgileNonAgileTeamsResponse>
            {
                new AgileNonAgileTeamsResponse { WidgetType = "Summary", TeamCategorySelection = "Activated", 
                    TeamCount = 1, CategoryRank = 1, ChildTeamName = "" },
                new AgileNonAgileTeamsResponse { TeamCategorySelection = "Total", TeamCount = 1}
            };

            await AgileNonAgileValidator(request, expectedResponse);

        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_AgileNonAgileTeams_Post_Team_Average_TeamFormation_OK()
        {
            VerifySetup(_classInitFailed);
            var request = new AgileNonAgileTeamsRequest
            {
                WidgetType = StructuralAgilityWidgetType.Average,
                TeamIds = new List<int> { _team.TeamId },
                SelectedTeamCategoryName = "Team Formation",
                SelectedTeamParents = _team.ParentId.ToString()
            };

            var expectedResponse = new List<AgileNonAgileTeamsResponse>
            {
                new AgileNonAgileTeamsResponse { WidgetType = "Summary", TeamCategorySelection = "Formed", 
                    TeamCount = 1, CategoryRank = 1, ChildTeamName = "" },
                new AgileNonAgileTeamsResponse { TeamCategorySelection = "Total", TeamCount = 1}
            };

            await AgileNonAgileValidator(request, expectedResponse);

        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_AgileNonAgileTeams_Post_Team_Average_WorkType_OK()
        {
            VerifySetup(_classInitFailed);
            var request = new AgileNonAgileTeamsRequest
            {
                WidgetType = StructuralAgilityWidgetType.Average,
                TeamIds = new List<int> { _team.TeamId },
                SelectedTeamCategoryName = "Work Type",
                SelectedTeamParents = _team.ParentId.ToString()
            };

            var expectedResponse = new List<AgileNonAgileTeamsResponse>
            {
                new AgileNonAgileTeamsResponse { WidgetType = "Summary", TeamCategorySelection = "Software Delivery", 
                    TeamCount = 1, ChildTeamName = "", CategoryRank = 1 },
                new AgileNonAgileTeamsResponse { TeamCategorySelection = "Total", TeamCount = 1}
            };

            await AgileNonAgileValidator(request, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_AgileNonAgileTeams_Post_Team_Distribution_AgileAdoption_OK()
        {
            VerifySetup(_classInitFailed);
            var request = new AgileNonAgileTeamsRequest
            {
                WidgetType = StructuralAgilityWidgetType.Distribution,
                TeamIds = new List<int> { _team.TeamId },
                SelectedTeamCategoryName = "Agile Adoption",
                SelectedTeamParents = _team.ParentId.ToString()
            };

            var expectedResponse = new List<AgileNonAgileTeamsResponse>
            {
                new AgileNonAgileTeamsResponse { TeamCategorySelection = "Activated", TeamCount = 1,
                    ChildTeamId = _team.TeamId, ChildTeamName = _team.Name }
            };

            await AgileNonAgileValidator(request, expectedResponse);

        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_AgileNonAgileTeams_Post_Team_Distribution_TeamFormation_OK()
        {
            VerifySetup(_classInitFailed);
            var request = new AgileNonAgileTeamsRequest
            {
                WidgetType = StructuralAgilityWidgetType.Distribution,
                TeamIds = new List<int> { _team.TeamId },
                SelectedTeamCategoryName = "Team Formation",
                SelectedTeamParents = _team.ParentId.ToString()
            };

            var expectedResponse = new List<AgileNonAgileTeamsResponse>
            {
                new AgileNonAgileTeamsResponse { TeamCategorySelection = "Formed", TeamCount = 1,
                    ChildTeamId = _team.TeamId, ChildTeamName = _team.Name },
            };

            await AgileNonAgileValidator(request, expectedResponse);

        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_AgileNonAgileTeams_Post_Team_Distribution_WorkType_OK()
        {
            VerifySetup(_classInitFailed);
            var request = new AgileNonAgileTeamsRequest()
            {
                WidgetType = StructuralAgilityWidgetType.Distribution,
                TeamIds = new List<int> { _team.TeamId },
                SelectedTeamCategoryName = "Work Type",
                SelectedTeamParents = _team.ParentId.ToString()
            };

            var expectedResponse = new List<AgileNonAgileTeamsResponse>
            {
                new AgileNonAgileTeamsResponse { TeamCategorySelection = "Software Delivery", TeamCount = 1,
                    ChildTeamId = _team.TeamId, ChildTeamName = _team.Name },
            };

            await AgileNonAgileValidator(request, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_AgileNonAgileTeams_Post_Unauthorized()
        {
            // given
            var client = GetUnauthenticatedClient();

            // when
            var agileNonAgileRequest = InsightsFactory.GetAgileNonAgileTeamsAverageRequest("Agile Adoption");

            var response = await client.PostAsync<string>(
                    RequestUris.AnalyticsAgileNonAgileTeams(Company.InsightsId), agileNonAgileRequest);

            // then
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status Code does not match.");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_AgileNonAgileTeams_Post_Forbidden()
        {
            // given
            var client = await GetInsightsAuthenticatedClient();

            // when
            var agileNonAgileRequest = InsightsFactory.GetAgileNonAgileTeamsAverageRequest("Agile Adoption");

            var response = await client.PostAsync<string>(
                    RequestUris.AnalyticsAgileNonAgileTeams(SharedConstants.FakeCompanyId), agileNonAgileRequest);
            // then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status Code does not match.");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_AgileNonAgileTeams_Post_BadRequest()
        {
            // given
            var client = await GetInsightsAuthenticatedClient();

            // when
            var agileNonAgileRequest = InsightsFactory.GetAgileNonAgileTeamsAverageRequest("Agile Adoption");
            agileNonAgileRequest.WidgetType = StructuralAgilityWidgetType.BadRequest;

            var response =
                await client.PostAsync<IList<string>>(
                    RequestUris.AnalyticsAgileNonAgileTeams(Company.InsightsId), agileNonAgileRequest);

            // then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status Code does not match.");
            Assert.AreEqual("'Widget Type' has a range of values which does not include '100'.", response.Dto.FirstOrDefault(), "Response string does not read \"'Widget Type' has a range of values which does not include '100'.\"");
        }

        private async Task AgileNonAgileValidator(AgileNonAgileTeamsRequest request, IReadOnlyCollection<AgileNonAgileTeamsResponse> expectedResponse)
        {
            var client = await GetInsightsAuthenticatedClient();

            var response = await client.PostAsync<IList<AgileNonAgileTeamsResponse>>(
                    RequestUris.AnalyticsAgileNonAgileTeams(Company.InsightsId), request);

            // then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status Code does not match.");
            Assert.AreEqual(expectedResponse.Count, response.Dto.Count,
                "Response list count does not match.");
            foreach (var item in expectedResponse)
            {
                var actualResult = response.Dto.FirstOrDefault(r => r.TeamCategorySelection == item.TeamCategorySelection && r.ChildTeamId == item.ChildTeamId)
                    .CheckForNull($"{item.TeamCategorySelection} was not found in the response.");
                Assert.AreEqual(item.ChildTeamId, actualResult.ChildTeamId,
                    $"ChildTeamId does not match for <{item.TeamCategorySelection}>");
                Assert.AreEqual(item.ChildTeamName, actualResult.ChildTeamName,
                    $"ChildTeamName does not match for <{item.TeamCategorySelection}>");
                Assert.AreEqual(item.TeamCount, actualResult.TeamCount,
                    $"TeamCount does not match for <{item.TeamCategorySelection}>");
                Assert.AreEqual(item.CategoryRank, actualResult.CategoryRank, 
                    $"CategoryRank does not match for <{item.TeamCategorySelection}>");
            }
        }
    }

}
