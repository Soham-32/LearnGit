using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.GrowthItems;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.Utilities;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Dashboards.GrowthItems
{
    [TestClass]
    [TestCategory("GrowthItemsDashboard"), TestCategory("Dashboard")]
    public class GiDashboardGridSortingTests : BaseTest
    {
        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [Description("Test 6: Verify that column sorting working properly in Growth Item dashboard, Grid view")]
        public void GrowthItemDashboard_Grid_ColumnSorting()
        {
            var login = new LoginPage(Driver, Log);
            var teamDashboardPage = new TeamDashboardPage(Driver, Log);
            var giDashboardGridViewPage = new GiDashboardGridWidgetPage(Driver, Log);
            var csharpHelpers = new CSharpHelpers();

            Log.Info($"Login as {User.FullName} and navigate to the 'Growth Item' dashboard");
            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(User.Username, User.Password);
            teamDashboardPage.ClickGrowthItemDashBoard();
            giDashboardGridViewPage.ClearFilter();

            Log.Info("Select all columns");
            var expectedColumns = DashboardFactory.GetGrowthItemDashboardColumnHeaderList();
            
            giDashboardGridViewPage.ClearFilter();
            giDashboardGridViewPage.AddSelectedColumns(expectedColumns);

            Log.Info("Sort the all columns and verify that sorting should be working");
            foreach (var column in expectedColumns.Where(column => column != "Description" && column != "Tags" && column != "Competency Target" && column!= "Origination"))
            {
                //Ascending Order
                giDashboardGridViewPage.SortGridColumn(column);
                var actualColumnText = giDashboardGridViewPage.GetColumnValues(column);
                IList<string> expectedColumnText;
                switch (column)
                {
                    case "Id":
                        expectedColumnText = csharpHelpers.SortListAscending(actualColumnText.Select(int.Parse).ToList());
                        break;
                    case "Created":
                    case "Target Date":
                    case "Completion Date":
                        expectedColumnText = csharpHelpers.SortListAscending(giDashboardGridViewPage.FormatColumnDates(actualColumnText));
                        break;
                    default:
                        expectedColumnText = csharpHelpers.SortListAscending(actualColumnText);
                        break;
                }
                for (var i = 0; i < expectedColumnText.Count; i++)
                {
                    Log.Info($"Row {i} - Expected='{expectedColumnText[i]}' Actual='{actualColumnText[i]}'");
                    Assert.AreEqual(expectedColumnText[i], actualColumnText[i], $"{i}th column text doesn't match during ascending");
                }

                //Descending
                giDashboardGridViewPage.SortGridColumn(column);
                actualColumnText = giDashboardGridViewPage.GetColumnValues(column);
                switch (column)
                {
                    case "Id":
                        expectedColumnText = csharpHelpers.SortListDescending(actualColumnText.Select(int.Parse).ToList());
                        break;
                    case "Created":
                    case "Target Date":
                    case "Completion Date":
                        expectedColumnText = csharpHelpers.SortListDescending(giDashboardGridViewPage.FormatColumnDates(actualColumnText));
                        break;
                    default:
                        expectedColumnText = csharpHelpers.SortListDescending(actualColumnText);
                        break;
                }
                for (var i = 0; i < expectedColumnText.Count; i++)
                {
                    Log.Info($"Row {i} - Expected='{expectedColumnText[i]}' Actual='{actualColumnText[i]}'");
                    Assert.AreEqual(expectedColumnText[i], actualColumnText[i], $"{i}th column text doesn't match descending");
                }
            }
        }
    }
}
