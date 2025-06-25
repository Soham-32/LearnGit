using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Company;
using AtCommon.Dtos.Companies;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Company.Delete
{
    [TestClass]
    [TestCategory("Companies")]
    public class CompanyDashboardDeleteTests : CompanyEditBaseTest
    {
        private static AddCompanyRequest _companyRequest;

        [ClassInitialize]
        public static void SetUp(TestContext _)
        {
            _companyRequest = CreateCompany();
        }

        [TestMethod]
        [TestCategory("KnownDefect")] // Bug : 48243
        [TestCategory("Critical")]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void CompanyDashboard_DeleteCompany()
        {
            var login = new LoginPage(Driver, Log);
            var companyDashboardPage = new CompanyDashboardPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            companyDashboardPage.WaitUntilLoaded();

            companyDashboardPage.ClickDeleteIconByCompanyName(_companyRequest.Name);

            companyDashboardPage.Search(_companyRequest.Name);

            Log.Info("Validate the deleted company does not show on the dashboard");
            Assert.AreEqual(0, companyDashboardPage.GetColumnValues("Company Name").Count, $"Deleted company <{_companyRequest.Name}> should not appear on the dashboard.");
        }

    }
}
