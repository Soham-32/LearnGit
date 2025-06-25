using System.Linq;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.GrowthPlan.GrowthPlanV2.Dashboard;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.GrowthPlan.GrowthPlanV2.Dashboard
{
    [TestClass]
    [TestCategory("GrowthPlanV2"), TestCategory("GrowthPlan")]
    public class Gpv2DashboardGridColumnsSortingTests : BaseTest
    {

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void GPV2_VerifyItemsSortedByClickingOnColumn()
        {
            var login = new LoginPage(Driver, Log);
            var growthPlanDashboard = new GrowthPlanDashboardPage(Driver, Log);
            var csharpHelpers = new CSharpHelpers();

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            growthPlanDashboard.NavigateToPage(Company.Id);

            var allColumns = GrowthPlanDashboardPage.ColumnLocators.Keys.ToList();
            growthPlanDashboard.AddColumns(allColumns);

            //Ascending !
            foreach (var column in allColumns)
            {
                growthPlanDashboard.SortColumnIntoAscendingOrder(column);

                var actualColumnValue = growthPlanDashboard.GetAllColumnValues(column).ToList();

                var expectedColumnValue = column switch
                {
                    "Id" => csharpHelpers.SortListAscending(actualColumnValue.Select(int.Parse).ToList()),
                    "Title" => csharpHelpers.SortListAscending(actualColumnValue).ToList(),
                    "Priority" => actualColumnValue.OrderBy(i => "Very Low")
                        .ThenBy(j => "Low")
                        .ThenBy(k => "Medium")
                        .ThenBy(l => "High")
                        .ThenBy(m => "Very High")
                        .ToList(),
                    "Type" => csharpHelpers.SortListAscending(actualColumnValue).ToList(),
                    "Status" => csharpHelpers.SortListAscending(actualColumnValue).ToList(),
                    "Category" => csharpHelpers.SortListAscending(actualColumnValue).ToList(),
                    "Location" => csharpHelpers.SortListAscending(actualColumnValue).ToList(),
                    "Assessment" => csharpHelpers.SortListAscending(actualColumnValue).ToList(),
                    "Team" => csharpHelpers.SortListAscending(actualColumnValue).ToList(),
                    "Competency Target" => csharpHelpers.SortListAscending(actualColumnValue).ToList(),
                    "Radar Type" => csharpHelpers.SortListAscending(actualColumnValue).ToList(),
                    "Created Date" => csharpHelpers.SortListAscending(
                        growthPlanDashboard.FormatColumnDates(actualColumnValue)),
                    "Owner(s)" => csharpHelpers.SortListAscending(actualColumnValue).ToList(),
                    "Updated By" => csharpHelpers.SortListAscending(actualColumnValue).ToList(),
                    "Target Date" => csharpHelpers.SortListAscending(
                        growthPlanDashboard.FormatColumnDates(actualColumnValue)),
                    "Completion Date" => csharpHelpers.SortListAscending(
                        growthPlanDashboard.FormatColumnDates(actualColumnValue)),
                    "Size" => csharpHelpers.SortListAscending(actualColumnValue.ToList()),
                    "Affected Teams" => csharpHelpers.SortListAscending(actualColumnValue).ToList(),
                    "Tags" => csharpHelpers.SortListAscending(actualColumnValue).ToList(),
                    "Origination" => csharpHelpers.SortListAscending(actualColumnValue).ToList(),
                    "External Identifier" => csharpHelpers.SortListAscending(actualColumnValue).ToList(),
                    _ => csharpHelpers.SortListAscending(actualColumnValue)
                };

                for (var i = 0; i < expectedColumnValue.Count; i++)
                {
                    Log.Info($"Row {i} - Expected='{expectedColumnValue[i]}' Actual='{expectedColumnValue[i]}'");
                    Assert.AreEqual(expectedColumnValue[i].ToLower(), actualColumnValue[i].ToLower(), $"{i}th column text doesn't match");
                }
            }

            //Descending !
            foreach (var column in allColumns)
            {
                growthPlanDashboard.SortColumnIntoDescendingOrder(column);

                var actualColumnValue = growthPlanDashboard.GetAllColumnValues(column).ToList();

                var expectedColumnValue = column switch
                {
                    "Id" => csharpHelpers.SortListDescending(actualColumnValue.Select(int.Parse).ToList()),
                    "Title" => csharpHelpers.SortListDescending(actualColumnValue).ToList(),
                    "Priority" => actualColumnValue.OrderBy(i => "Very High")
                        .ThenBy(j => "High")
                        .ThenBy(k => "Medium")
                        .ThenBy(l => "Low")
                        .ThenBy(m => "Very Low")
                        .ToList(),
                    "Type" => csharpHelpers.SortListDescending(actualColumnValue).ToList(),
                    "Status" => csharpHelpers.SortListDescending(actualColumnValue).ToList(),
                    "Category" => csharpHelpers.SortListDescending(actualColumnValue).ToList(),
                    "Location" => csharpHelpers.SortListDescending(actualColumnValue).ToList(),
                    "Assessment" => csharpHelpers.SortListDescending(actualColumnValue).ToList(),
                    "Team" => csharpHelpers.SortListDescending(actualColumnValue).ToList(),
                    "Competency Target" => csharpHelpers.SortListDescending(actualColumnValue).ToList(),
                    "Radar Type" => csharpHelpers.SortListDescending(actualColumnValue).ToList(),
                    "Created Date" => csharpHelpers.SortListDescending(
                        growthPlanDashboard.FormatColumnDates(actualColumnValue)),
                    "Owner(s)" => csharpHelpers.SortListDescending(actualColumnValue).ToList(),
                    "Updated By" => csharpHelpers.SortListDescending(actualColumnValue).ToList(),
                    "Target Date" => csharpHelpers.SortListDescending(
                        growthPlanDashboard.FormatColumnDates(actualColumnValue)),
                    "Completion Date" => csharpHelpers.SortListDescending(
                        growthPlanDashboard.FormatColumnDates(actualColumnValue)),
                    "Size" => csharpHelpers.SortListDescending(actualColumnValue.ToList()),
                    "Affected Teams" => csharpHelpers.SortListDescending(actualColumnValue).ToList(),
                    "Tags" => csharpHelpers.SortListDescending(actualColumnValue).ToList(),
                    "Origination" => csharpHelpers.SortListDescending(actualColumnValue).ToList(),
                    "External Identifier" => csharpHelpers.SortListDescending(actualColumnValue).ToList(),
                    _ => csharpHelpers.SortListDescending(actualColumnValue)
                };

                for (var i = 0; i < expectedColumnValue.Count; i++)
                {
                    Log.Info($"Row {i} - Expected='{expectedColumnValue[i]}' Actual='{expectedColumnValue[i]}'");
                    Assert.AreEqual(expectedColumnValue[i].ToLower(), actualColumnValue[i].ToLower(), $"{i}th column text doesn't match");
                }
            }
        }
    }
}