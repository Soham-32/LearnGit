using System.Collections.Generic;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.Utilities;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Dashboards.Teams
{
    [TestClass]
    [TestCategory("TeamsDashboard"), TestCategory("Dashboard")]
    public class TeamDashboardSortingAllTests : BaseTest
    {
        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("OrgLeader"), TestCategory("BLAdmin"), TestCategory("Member")]
        public void TeamDashboard_GridView_ColumnSorting_All()
        {
            var login = new LoginPage(Driver, Log);
            var teamDashboardPage = new TeamDashboardPage(Driver, Log);
            var csharpHelpers = new CSharpHelpers();
            var teamsList = new List<string>() { "Enterprise Teams", "Sub Teams", "Multi Teams" };

            Log.Info($"Login as {User.FullName} and navigate to the 'Team Dashboard' Page");
            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(User.Username, User.Password);
            teamDashboardPage.GridTeamView();

            Log.Info("Select the 'All' from 'TeamType' dropdown and add all column");
            teamDashboardPage.FilterTeamType("All");
            teamDashboardPage.SortGridColumn("Work Type");
            var columnList = DashboardFactory.GetTeamsDashboardColumnHeaderList();

            if (User.Type != UserType.Member)
            {
                columnList.Add("Number of Individual Assessments");
            }
            teamDashboardPage.AddColumns(columnList);

            Log.Info("Sort the all columns and verify that sorting should be working");
            foreach (var column in columnList)
            {
                //Ascending Oder
                teamDashboardPage.SortGridColumn(column);
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

                //Descending Order
                teamDashboardPage.SortGridColumn(column);
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
