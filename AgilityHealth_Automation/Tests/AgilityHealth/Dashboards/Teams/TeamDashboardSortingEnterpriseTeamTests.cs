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
    public class TeamDashboardSortingEnterpriseTeamTests : BaseTest
    {
        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("OrgLeader"), TestCategory("BLAdmin"), TestCategory("Member")]
        public void TeamDashboard_GridView_ColumnSorting_EnterpriseTeam()
        {
            var login = new LoginPage(Driver, Log);
            var dashboardPage = new TeamDashboardPage(Driver, Log);
            var csharpHelpers = new CSharpHelpers();

            Driver.NavigateToPage(ApplicationUrl);

            login.LoginToApplication(User.Username, User.Password);

            dashboardPage.GridTeamView();

            dashboardPage.FilterTeamType("Enterprise Team");
            //reset the sorting to a column other than team name
            dashboardPage.SortGridColumn("Work Type");

            var enterpriseTeamColumns = new List<string>() { "Team Name", "Work Type", "Number of Sub Teams", "Team Tags", "External ID", "Number of Team Members" };
            dashboardPage.AddColumns(enterpriseTeamColumns);
            foreach (var column in enterpriseTeamColumns)
            {

                dashboardPage.SortGridColumn(column);
                var actualColumnText = dashboardPage.GetColumnValues(column);
                
                var expectedColumnText = csharpHelpers.SortListAscending(actualColumnText);

                for (var i = 0; i < expectedColumnText.Count; i++)
                {
                    Log.Info($"Column {i} - Expected='{expectedColumnText[i]}' Actual='{actualColumnText[i]}'");
                    Assert.AreEqual(expectedColumnText[i], actualColumnText[i], $"column <{i}> text doesn't match");
                }

                dashboardPage.SortGridColumn(column);
                actualColumnText = dashboardPage.GetColumnValues(column);
                
                expectedColumnText = csharpHelpers.SortListDescending(actualColumnText);

                for (var i = 0; i < expectedColumnText.Count; i++)
                {
                    Log.Info($"Column {i} - Expected='{expectedColumnText[i]}' Actual='{actualColumnText[i]}'");
                    Assert.AreEqual(expectedColumnText[i], actualColumnText[i], $"column <{i}> text doesn't match");
                }
            }
        }
    }
}
