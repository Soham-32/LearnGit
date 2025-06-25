using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.V2;
using AtCommon.Api;
using AtCommon.Api.Enums;
using AtCommon.Dtos.BusinessOutcomes;
using AtCommon.Dtos.Companies;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace AgilityHealth_Automation.Tests.AgilityHealth.BusinessOutcomes.Dashboard
{
    [TestClass]
    [TestCategory("BusinessOutcomes")]
    public class BusinessOutcomesDashboardChildOutcomeShownAtParentLevelTests : BusinessOutcomesBaseTest
    {
        private static BusinessOutcomeResponse _response1;
        private static string TeamName = SharedConstants.Team;
        private static CompanyHierarchyResponse _teams;

        [ClassInitialize]
        public static void ClassSetUp(TestContext _)
        {
            var setupApi = new SetupTeardownApi(TestEnvironment);
            _teams = setupApi.GetCompanyHierarchy(Company.Id);
            var request1 = GetBusinessOutcomeRequest(SwimlaneType.QuarterlyObjective);
            if (_teams?.Children?.Any(m => m.Type == "Outcome") == true)
            {
                var team = _teams.Children.FirstOrDefault(m => m.Type == "Outcome")?.Children?.FirstOrDefault()?.Children?.FirstOrDefault()?.Children?.FirstOrDefault();

                if (team != null)
                {
                    TeamName = team.Name;
                    request1.TeamId = team.TeamId;
                }
            }
            request1.TeamId = setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(TeamName).TeamId;
            _response1 = setupApi.CreateBusinessOutcome(request1);
        }

        [TestMethod]
        [TestCategory("Smoke")]
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("TeamAdmin")]
        public void BusinessOutcomes_Dashboard_ChildOutcomeShownAtParentLevel()
        {
            var login = new LoginPage(Driver, Log);
            var businessOutcomesDashboard = new BusinessOutcomesDashboardPage(Driver, Log);
            var leftNav = new LeftNavPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Go to Business Outcomes and verify the child outcome shows at parent level.");
            businessOutcomesDashboard.NavigateToPage(Company.Id);

            var topTeam = _teams.Children.FirstOrDefault();
            Assert.IsNotNull(topTeam, "Top-level team is missing in _teams data.");

            var getListOfColumns = businessOutcomesDashboard.GetAllColumnNames().Skip(2).ToList();

            if (topTeam.Type == "Enterprise")
            {
                var listOfEnterpriseTeams = _teams.Children.Where(a => a.Type == "Enterprise").Select(s => s.Name.ToUpper()).ToList();
                Assert.That.ListsAreEqual(listOfEnterpriseTeams, getListOfColumns, "List of columns for enterprise teams does not match");

                leftNav.ClickOnTeamName(SharedConstants.EnterpriseTeam);
                businessOutcomesDashboard.WaitForReload();

                var listOfMultiTeams = _teams.Children.Where(a => a.Name == SharedConstants.EnterpriseTeam)
                    .SelectMany(s => s.Children.Select(a => a.Name.ToUpper())).ToList();

                getListOfColumns = businessOutcomesDashboard.GetAllColumnNames().Skip(2).ToList();

                Assert.That.ListsAreEqual(listOfMultiTeams, getListOfColumns, "List of columns for multi teams does not match");

                leftNav.ClickOnTeamExpandButton(SharedConstants.EnterpriseTeam);
                leftNav.ClickOnTeamName(SharedConstants.MultiTeam);
                businessOutcomesDashboard.WaitForReload();

                var listOfTeams = _teams.Children.Where(a => a.Name == SharedConstants.EnterpriseTeam)
                    .SelectMany(s => s.Children.SelectMany(d => d.Children).Select(a => a.Name.ToUpper())).ToList();

                getListOfColumns = businessOutcomesDashboard.GetAllColumnNames().Skip(2).ToList();

                Assert.That.ListsAreEqual(listOfTeams, getListOfColumns, "List of columns for teams does not match");

                leftNav.ClickOnTeamExpandButton(SharedConstants.MultiTeam);
                leftNav.ClickOnTeamName(TeamName);

                businessOutcomesDashboard.WaitForReload();
                businessOutcomesDashboard.TagsViewSelectTag(TimeFrameTags.Quarterly.GetDescription());
                Assert.IsTrue(businessOutcomesDashboard.IsCardPresentInSwimLane(_response1.SwimlaneType.GetDescription(), _response1.Title),
                    $"Card with title <{_response1.Title}> not visible under swimlane <{_response1.SwimlaneType.GetDescription()}> at team level.");

                leftNav.ClickOnTeamName(SharedConstants.MultiTeam);
                businessOutcomesDashboard.WaitForReload();

                Assert.IsTrue(businessOutcomesDashboard.IsCardPresentInSwimLane(TeamName, _response1.Title),
                    $"Card with title <{_response1.Title}> not visible under swimlane <{_response1.SwimlaneType.GetDescription()}> at mulit-team level.");
            }
            else if (topTeam.Type == "Outcome")
            {
                // N-Tier level hierarchy: N-Tier > Enterprise > Multi > Team
                leftNav.ClickOnTeamName(topTeam.Name);
                businessOutcomesDashboard.WaitForReload();

                var enterpriseTeams = topTeam.Children.Select(c => c.Name.ToUpper()).ToList();
                getListOfColumns = businessOutcomesDashboard.GetAllColumnNames().Skip(2).ToList();
                Assert.That.ListsAreEqual(enterpriseTeams, getListOfColumns,
                    "List of columns for enterprise teams under N-Tier does not match");

                var enterpriseTeam = topTeam.Children.FirstOrDefault();
                Assert.IsNotNull(enterpriseTeam, "Enterprise team under N-Tier not found.");

                leftNav.ClickOnTeamExpandButton(topTeam.Name);
                leftNav.ClickOnTeamName(enterpriseTeam.Name);
                businessOutcomesDashboard.WaitForReload();

                var multiTeams = enterpriseTeam.Children.Select(c => c.Name.ToUpper()).ToList();
                getListOfColumns = businessOutcomesDashboard.GetAllColumnNames().Skip(2).ToList();
                Assert.That.ListsAreEqual(multiTeams, getListOfColumns,
                    "List of columns for multi teams does not match");

                var multiTeam = enterpriseTeam.Children.FirstOrDefault();
                Assert.IsNotNull(multiTeam, "Multi team under enterprise not found.");

                leftNav.ClickOnTeamExpandButton(enterpriseTeam.Name);
                leftNav.ClickOnTeamName(multiTeam.Name);
                businessOutcomesDashboard.WaitForReload();

                var teams = multiTeam.Children.Select(c => c.Name.ToUpper()).ToList();
                getListOfColumns = businessOutcomesDashboard.GetAllColumnNames().Skip(2).ToList();
                Assert.That.ListsAreEqual(teams, getListOfColumns, "List of columns for teams does not match");

                var team = multiTeam.Children.FirstOrDefault();
                Assert.IsNotNull(team, "Team under multi team not found.");

                leftNav.ClickOnTeamExpandButton(multiTeam.Name);
                leftNav.ClickOnTeamName(team.Name);
                businessOutcomesDashboard.WaitForReload();

                businessOutcomesDashboard.TagsViewSelectTag(TimeFrameTags.Quarterly.GetDescription());

                Assert.IsTrue(
                    businessOutcomesDashboard.IsCardPresentInSwimLane(_response1.SwimlaneType.GetDescription(),
                        _response1.Title),
                    $"Card with title <{_response1.Title}> not visible under swimlane <{_response1.SwimlaneType.GetDescription()}> at team level.");

                leftNav.ClickOnTeamName(multiTeam.Name);
                businessOutcomesDashboard.WaitForReload();

                Assert.IsTrue(
                    businessOutcomesDashboard.IsCardPresentInSwimLane(team.Name, _response1.Title),
                    $"Card with title <{_response1.Title}> not visible under swimlane <{team.Name.ToUpper()}> at multi-team level.");
            }
        }
    }
}