using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Company;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Company.Edit;
using AtCommon.Api;
using AtCommon.Dtos.Companies;
using AtCommon.ObjectFactories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Company.Edit
{
    [TestClass]
    [TestCategory("Companies")]
    public class CompanyProfileEditTests3 : CompanyEditBaseTest
    {
        private static AddCompanyRequest _companyRequest;

        [ClassInitialize]
        public static void SetUp(TestContext _)
        {
            _companyRequest = CreateCompany(true);
        }

        [TestMethod]
        [TestCategory("KnownDefect")] // Bug : 48243
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void Company_UpdateCompanyProfile_SaveAsDraft()
        {

            var login = new LoginPage(Driver, Log);
            var companyDashboardPage = new CompanyDashboardPage(Driver, Log);
            var editCompanyProfilePage = new EditCompanyProfilePage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            companyDashboardPage.WaitUntilLoaded();

            companyDashboardPage.ClickEditIconByCompanyName(_companyRequest.Name);
            editCompanyProfilePage.WaitUntilLoaded();

            var editedCompany = CompanyFactory.GetCompanyForUpdate();
            editedCompany.Name = _companyRequest.Name;

            editCompanyProfilePage.SelectCountry(editedCompany.Country);

            editCompanyProfilePage.Header.ClickCloseButton();
            editCompanyProfilePage.Header.ClickSaveAsDraftButton();

            Log.Info("Validate the company Life Cycle shows as 'Draft'");
            Assert.AreEqual("Draft",companyDashboardPage.GetCompanyDetail(editedCompany.Name, "Life Cycle"), "Life Cycle does not match");
            
            companyDashboardPage.ClickEditIconByCompanyName(editedCompany.Name);
            editCompanyProfilePage.WaitUntilLoaded();

            Log.Info("Verify the updated info is displayed");
            var actualCompanyProfile = editCompanyProfilePage.GetCompanyProfile();

            // profile
            Assert.AreEqual(editedCompany.Name, actualCompanyProfile.Name, "Name does not match");
            Assert.AreEqual(editedCompany.Country, actualCompanyProfile.Country, "Country does not match.");

        }

        [ClassCleanup]
        public static void ClassTearDown()
        {
            if (!User.IsSiteAdmin() && !User.IsPartnerAdmin()) return;
            var setup = new SetupTeardownApi(TestEnvironment);
            setup.DeleteCompany(_companyRequest.Name).GetAwaiter().GetResult();
        }

    }
}
