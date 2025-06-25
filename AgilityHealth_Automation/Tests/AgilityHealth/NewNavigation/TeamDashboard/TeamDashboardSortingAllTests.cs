using System.Collections.Generic;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.NewNavigation.Dashboard;
using AgilityHealth_Automation.Utilities;
using AtCommon.ObjectFactories.NewNavigation;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.NewNavigation.TeamDashboard
{
    [TestClass]
    [TestCategory("TeamDashboard"), TestCategory("NewNavigation")]
    public class TeamDashboardSortingAllTests : BaseTest
    {
        [TestMethod]
        [TestCategory("ColumnSorting")]
        [TestCategory("CompanyAdmin")]
        [TestCategory("KnownDefect")]//Bug 49586
        public void TeamDashboard_GridView_ColumnSorting_All()
        {
            var login = new LoginPage(Driver, Log);
            var teamDashboardPage = new TeamDashboardPage(Driver, Log);
            var csharpHelpers = new CSharpHelpers();
            var teamsList = new List<string>() { "Enterprise Teams", "Sub-Teams", "Multi Teams" };

            Log.Info($"Login as {User.FullName} and navigate to the 'Team Dashboard' Page");
            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Switch to Grid view");
            teamDashboardPage.SwitchToGridView();

            Log.Info("Select the 'All' from 'Level' dropdown and add all column");
            teamDashboardPage.FilterTeamType("All");
            teamDashboardPage.ClickOnGridColumn("Work Type");
            var columnList = TeamDashboardFactory.GetTeamsDashboardColumnHeaderList();

            if (User.Type != UserType.Member)
            {
                columnList.Add("No of Individual Assessments");
            }
            teamDashboardPage.AddColumns(columnList);

            Log.Info("Sort the all columns and verify that sorting should be working");
            foreach (var column in columnList)
            {
                Log.Info("Sort in ascending order");
                teamDashboardPage.ClickOnGridColumn(column);
                var actualColumnText = teamDashboardPage.GetColumnValues(column);

                if (teamsList.Contains(column))
                {
                    actualColumnText = teamDashboardPage.GetTeamsColumnValues(column);
                }
                
                IList<string> expectedColumnText;
                switch (column)
                {
                    case "Last Date of Assessment":
                    case "Date Deleted":
                        expectedColumnText = csharpHelpers.SortListAscending(teamDashboardPage.FormatColumnDates(actualColumnText));
                        break;
                    default:
                        expectedColumnText = csharpHelpers.SortListAscending(actualColumnText);
                        break;
                }
                for (var i = 0; i < expectedColumnText.Count; i++)
                {
                    Log.Info($"Row {i} - Expected='{expectedColumnText[i]}' Actual='{actualColumnText[i]}'");
                    Assert.AreEqual(expectedColumnText[i], actualColumnText[i],$"{i}th {column} text doesn't match during Ascending");
                }

                Log.Info("Sort in descending order");
                teamDashboardPage.ClickOnGridColumn(column);
                actualColumnText = teamDashboardPage.GetColumnValues(column);
                if (teamsList.Contains(column))
                {
                     actualColumnText = teamDashboardPage.GetTeamsColumnValues(column);
                }
                switch (column)
                {
                    case "Last Date of Assessment":
                    case "Date Deleted":
                        expectedColumnText = csharpHelpers.SortListDescending(teamDashboardPage.FormatColumnDates(actualColumnText));
                        break;
                    default:
                        expectedColumnText = csharpHelpers.SortListDescending(actualColumnText);
                        break;
                }
                for (var i = 0; i < expectedColumnText.Count; i++)
                {
                    Log.Info($"Column {i} - Expected='{expectedColumnText[i]}' Actual='{actualColumnText[i]}'");
                    Assert.AreEqual(expectedColumnText[i], actualColumnText[i], $"{i}th {column} text doesn't match during Descending");
                }
            }
        }
    }
}
