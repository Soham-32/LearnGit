using System.Linq;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Common;
using AgilityHealth_Automation.PageObjects.AgilityHealth.GrowthPlan.GrowthPlanV2.Dashboard;
using AgilityHealth_Automation.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.GrowthPlan.GrowthPlanV2.Dashboard
{
    [TestClass]
    [TestCategory("GrowthPlanV2"), TestCategory("GrowthPlan")]
    public class Gpv2SettingsVerification : BaseTest
    {

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void GPV2_VerifyAllCheckboxSelectDeselectWorksSuccessfullyInSettingPopup()
        {
            var login = new LoginPage(Driver, Log);
            var growthPlanDashboard = new GrowthPlanDashboardPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            growthPlanDashboard.NavigateToPage(Company.Id);

            var expectedColumn = GrowthPlanDashboardPage.ColumnLocators.Keys.ToList();

            growthPlanDashboard.RemoveColumns(expectedColumn);
            Driver.RefreshPage();

            growthPlanDashboard.ShowColumnSettings();
            foreach (var columnName in expectedColumn)
            {
                Assert.IsFalse(growthPlanDashboard.IsColumnCheckboxSelected(columnName), $"The '{columnName}' is selected when it should not be.");
            }
            growthPlanDashboard.HideColumnSettings();

            foreach (var column in expectedColumn)
            {
                Assert.IsFalse(growthPlanDashboard.IsColumnVisible(column),
                    $"The '{column}' is visible when it should not be.");
            }

            growthPlanDashboard.AddColumns(expectedColumn);
            Driver.RefreshPage();

            growthPlanDashboard.ShowColumnSettings();
            foreach (var columnName in expectedColumn)
            {
                Assert.IsTrue(growthPlanDashboard.IsColumnCheckboxSelected(columnName), $"The '{columnName}' is not selected when it should be.");
            }
            growthPlanDashboard.HideColumnSettings();

            foreach (var column in expectedColumn)
            {
                Assert.IsTrue(growthPlanDashboard.IsColumnVisible(column),
                    $"The '{column}' is not visible when it should be.");
            }

            var expectedSortedColumn = GrowthPlanDashboardPage.ColumnLocators.Keys.Skip(10).ToList();
            growthPlanDashboard.RemoveColumns(expectedSortedColumn);

            topNav.LogOut();
            login.LoginToApplication(User.Username, User.Password);

            growthPlanDashboard.NavigateToPage(Company.Id);

            growthPlanDashboard.ShowColumnSettings();
            foreach (var columnName in expectedSortedColumn)
            {
                Assert.IsFalse(growthPlanDashboard.IsColumnCheckboxSelected(columnName), $"The '{columnName}' is selected when it should not be.");
            }
        }
    }
}
