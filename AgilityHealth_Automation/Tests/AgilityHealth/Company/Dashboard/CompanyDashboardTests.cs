using System.Collections.Generic;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Company;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Company.Dashboard
{
    [TestClass]
    [TestCategory("Companies")]
    public class CompanyDashboardTests : BaseTest
    {
        [TestMethod]
        [TestCategory("CompanyManagement"), TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void CompanyDashboard_FilterByCompanyName()
        {

            var login = new LoginPage(Driver, Log);
            var companyDashboardPage = new CompanyDashboardPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            companyDashboardPage.WaitUntilLoaded();

            const string searchTerm = "Automation";
            companyDashboardPage.Search(searchTerm);

            Log.Info("Verify all the Companies shown match the search term");
            var actualCompanyNames = companyDashboardPage.GetColumnValues("Company Name");

            Assert.IsTrue(actualCompanyNames.Count > 0, $"No company were found that match the search term <{searchTerm}>");
            foreach (var companyName in actualCompanyNames)
            {
                Assert.IsTrue(companyName.Contains(searchTerm),
                    $"The Company Name <{companyName}> does not contain the search term {searchTerm}");
            }

        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void CompanyDashboard_ChangeColumnSettings()
        {
            var login = new LoginPage(Driver, Log);
            var companyDashboardPage = new CompanyDashboardPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            companyDashboardPage.WaitUntilLoaded();

            var expectedColumns = new List<string>
            {
                "Company Name",
                "Industry",
                "Comp Admin",
                "Ref. Partner",
                "Last Asmt",
                "Asmt",
                "Teams",
                "Subscript",
                "Life Cycle",
                "Comp Type"
            };

            companyDashboardPage.RemoveColumns(expectedColumns);
            foreach (var column in expectedColumns)
            {
                Assert.IsFalse(companyDashboardPage.IsColumnVisible(column), $"The '{column}' is visible when it should not be.");
            }

            companyDashboardPage.AddColumns(expectedColumns);
            foreach (var column in expectedColumns)
            {
                Log.Info($"Verify <{column}> column is showing.");
                Assert.IsTrue(companyDashboardPage.IsColumnVisible(column), $"The '{column}' is not visible when it should be.");
            }
        }

    }
}
