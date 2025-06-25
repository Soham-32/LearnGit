using System.Collections.Generic;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.NewNavigation.Dashboard;
using AgilityHealth_Automation.Utilities;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.NewNavigation.TeamDashboard
{
    [TestClass]
    [TestCategory("TeamDashboard"), TestCategory("NewNavigation")]
    public class TeamDashboardSortingTeamTests : BaseTest
    {
        [TestMethod]
        [TestCategory("ColumnSorting")]
        [TestCategory("CompanyAdmin")]
        [TestCategory("KnownDefect")]//Bug 49586
        public void TeamDashboard_GridView_ColumnSorting_Team()
        {
            var login = new LoginPage(Driver, Log);
            var dashboardPage = new TeamDashboardPage(Driver, Log);
            var csharpHelpers = new CSharpHelpers();

            Log.Info($"Login as {User.FullName} and navigate to the 'Team Dashboard' Page");
            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Switch to Grid view");
            dashboardPage.SwitchToGridView();

            Log.Info("Select the 'Team' from 'Level' dropdown and add all column");
            dashboardPage.FilterTeamType("Team");
            dashboardPage.ClickOnGridColumn("Work Type");
            var teamColumns = new List<string>() { "Team Name", "Work Type", "No of Team Assessments", "Last Date of Assessment", "Team Tags", "External ID", "No of Team Members" };
            dashboardPage.AddColumns(teamColumns);
            
            foreach (var column in teamColumns)
            {
                Log.Info("Sort in ascending order");
                dashboardPage.ClickOnGridColumn(column);
                var actualColumnText = dashboardPage.GetColumnValues(column);
                
                var expectedColumnText = csharpHelpers.SortListAscending(actualColumnText);

                for (var i = 0; i < expectedColumnText.Count; i++)
                {
                    Log.Info($"Column {i} - Expected='{expectedColumnText[i]}' Actual='{actualColumnText[i]}'");
                    Assert.AreEqual(expectedColumnText[i], actualColumnText[i], $"{i}th column text doesn't match");
                }

                Log.Info("Sort in descending order");
                dashboardPage.ClickOnGridColumn(column);
                actualColumnText = dashboardPage.GetColumnValues(column);
                
                expectedColumnText = csharpHelpers.SortListDescending(actualColumnText);

                for (var i = 0; i < expectedColumnText.Count; i++)
                {
                    Log.Info($"Column {i} - Expected='{expectedColumnText[i]}' Actual='{actualColumnText[i]}'");
                    Assert.AreEqual(expectedColumnText[i], actualColumnText[i], $"{i}th column text doesn't match");
                }
            }
        }
    }
}
