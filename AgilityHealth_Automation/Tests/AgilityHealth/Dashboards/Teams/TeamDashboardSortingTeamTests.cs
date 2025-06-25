using System.Collections.Generic;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.Utilities;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Dashboards.Teams
{
    [TestClass]
    [TestCategory("TeamsDashboard"), TestCategory("Dashboard")]
    public class TeamDashboardSortingTeamTests : BaseTest
    {
        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("OrgLeader"), TestCategory("BLAdmin"), TestCategory("Member")]
        public void TeamDashboard_GridView_ColumnSorting_Team()
        {
            var login = new LoginPage(Driver, Log);
            var dashboardPage = new TeamDashboardPage(Driver, Log);
            var csharpHelpers = new CSharpHelpers();

            Driver.NavigateToPage(ApplicationUrl);

            login.LoginToApplication(User.Username, User.Password);

            dashboardPage.GridTeamView();

            dashboardPage.FilterTeamType("Team");
            //reset the sorting to a column other than team name
            dashboardPage.SortGridColumn("Work Type");

            var teamColumns = new List<string>() { "Team Name", "Work Type", "Number of Team Assessments", "Last Date of Assessment", "Team Tags", "External ID", "Number of Team Members" };
            dashboardPage.AddColumns(teamColumns);
            foreach (var column in teamColumns)
            {

                dashboardPage.SortGridColumn(column);
                var actualColumnText = dashboardPage.GetColumnValues(column);
                
                var expectedColumnText = csharpHelpers.SortListAscending(actualColumnText);

                for (var i = 0; i < expectedColumnText.Count; i++)
                {
                    Log.Info($"Column {i} - Expected='{expectedColumnText[i]}' Actual='{actualColumnText[i]}'");
                    Assert.AreEqual(expectedColumnText[i], actualColumnText[i], $"{i}th column text doesn't match");
                }

                dashboardPage.SortGridColumn(column);
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
