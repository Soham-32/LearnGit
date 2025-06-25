using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Company;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Company.Add;
using AtCommon.Api;
using AtCommon.Dtos.Companies;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Company.Add
{
    [TestClass]
    [TestCategory("Companies")]
    public class CompanyDashboardAddCompanyPage1Tests2 : BaseTest
    {
        private static AddCompanyRequest _companyRequest;

        [TestMethod]
        [TestCategory("KnownDefect")] //Bug Id: 51038
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void CompanyDashboard_AddCompany_Page1_Delete()
        {
            var login = new LoginPage(Driver, Log);
            var companyDashboardPage = new CompanyDashboardPage(Driver, Log);
            var addCompanyPage1 = new AddCompany1CompanyProfilePage(Driver, Log);

            Log.Info("Login to the application and navigate to company dashboard page");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            companyDashboardPage.WaitUntilLoaded();

            Log.Info("Click on 'Add a Company' button on the company dashboard page");
            companyDashboardPage.ClickOnAddCompanyButton();
            addCompanyPage1.WaitUntilLoaded();
            _companyRequest = CompanyFactory.GetCompany("ZZZ_Add1UI", User.CompanyName);

            Log.Info("Verify all the languages from 'Preferred Language' dropdown on the company profile page");
            var expectedLanguageList = ManageRadarFactory.Languages();
            var actualLanguageList = addCompanyPage1.GetAllLanguages();
            Assert.That.ListsAreEqual(expectedLanguageList, actualLanguageList, "Language list doesn't match");

            Log.Info("Fill all the company information on the company profile page");
            addCompanyPage1.FillInCompanyProfileInfo(_companyRequest);
            if (User.IsSiteAdmin())
            {
                addCompanyPage1.FillInAdminInfo(_companyRequest);
            }

            Log.Info("Click on 'Delete' button on the company profile page");
            addCompanyPage1.Header.ClickDeleteButton();
            companyDashboardPage.WaitUntilLoaded();

            Log.Info($"Search {_companyRequest.Name} company on company dashboard page");
            companyDashboardPage.Search(_companyRequest.Name);

            Log.Info("Validate the created company does not show on the dashboard");
            Assert.AreEqual(0, companyDashboardPage.GetColumnValues("Company Name").Count,
                $"Company <{_companyRequest.Name}> should not have been created.");
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
