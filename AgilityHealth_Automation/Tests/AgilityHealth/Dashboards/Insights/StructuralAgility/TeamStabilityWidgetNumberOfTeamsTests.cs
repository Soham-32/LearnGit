using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Insights.StructuralAgility;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Insights.TeamAgility;
using AtCommon.Api;
using AtCommon.Dtos;
using AtCommon.Dtos.Teams;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Dashboards.Insights.StructuralAgility
{
    [TestClass]
    [TestCategory("Insights"), TestCategory("StructuralAgilityDashboard"), TestCategory("Dashboard")]
    public class TeamStabilityWidgetNumberOfTeamsTests : BaseTest
    {
        private static readonly UserConfig CompanyAdminUserConfig = new UserConfig("CA");
        private static SetupTeardownApi _setupApi;
        private static IList<TeamProfileResponse> _teamProfileResponse;
        private static int _totalTeamsCount;
        private static User CompanyAdminUser => CompanyAdminUserConfig.GetUserByDescription("insights user");

        [ClassInitialize]
        public static void ClassSetUp(TestContext _)
        {
            _setupApi = new SetupTeardownApi(TestEnvironment);
            _teamProfileResponse = _setupApi.GetTeamProfileResponse("", CompanyAdminUser);
            var nonGoiTeamNamesList = _teamProfileResponse.Where(a => a.Type.Equals("Team") & a.DeletedAt == null).Select(b=>b.Name).ToList();
            foreach (var teamName in nonGoiTeamNamesList)
            {
                if (_setupApi.GetTeamMemberResponse(teamName, CompanyAdminUser, Company.InsightsId).Count > 0)
                {
                    _totalTeamsCount++;
                }
            }
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        public void Insights_StructuralAgility_TeamStabilityWidgetNumberOfTeams()
        {
            var login = new LoginPage(Driver, Log);
            var structuralAgility = new StructuralAgilityPage(Driver, Log);
            var teamAgility = new TeamAgilityPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(InsightsUser.Username, InsightsUser.Password);

            teamAgility.NavigateToPage(Company.InsightsId);

            teamAgility.ClickOnStructuralAgilityTab();

            Log.Info("Verify that number of team displays correctly");
            Assert.AreEqual($"({_totalTeamsCount} teams)", structuralAgility.GetTeamStabilityNumberOfTeam(),
                $"Number of team should be (1 teams) but found {structuralAgility.GetTeamStabilityNumberOfTeam()}");

            structuralAgility.SelectTeam(SharedConstants.InsightsEnterpriseTeam1);
            teamAgility.ClickOnStructuralAgilityTab();

            teamAgility.ClickOnStructuralAgilityTab();
            Log.Info("Verify that number of team displays correctly when selecting team");
            Assert.AreEqual($"({_totalTeamsCount} teams)", structuralAgility.GetTeamStabilityNumberOfTeam(), $"Number of team should be (2 teams) but found {structuralAgility.GetTeamStabilityNumberOfTeam()}");
        }
    }
}
