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
    public class CompanyDashboardAddCompanyPage4Tests2 : BaseTest
    {
        private static AddCompanyRequest _companyRequest;

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void CompanyDashboard_AddCompany_Page4_Delete()
        {
            var login = new LoginPage(Driver, Log);
            var companyDashboardPage = new CompanyDashboardPage(Driver, Log);
            var addCompanyPage1 = new AddCompany1CompanyProfilePage(Driver, Log);
            var addCompanyPage2 = new AddCompany2RadarSelectionPage(Driver, Log);
            var addCompanyPage3 = new AddCompany3SubscriptionPage(Driver, Log);
            var addCompanyPage4 = new AddCompany4SecurityPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            companyDashboardPage.WaitUntilLoaded();

            companyDashboardPage.ClickOnAddCompanyButton();
            addCompanyPage1.WaitUntilLoaded();

            _companyRequest = CompanyFactory.GetCompany("ZZZ_Add10UI", User.CompanyName);

            addCompanyPage1.FillInCompanyProfileInfo(_companyRequest);
            if (User.IsSiteAdmin())
            {
                addCompanyPage1.FillInAdminInfo(_companyRequest);
            }

            addCompanyPage1.ClickNextButton();
            addCompanyPage2.WaitUntilLoaded();

            const string radarName = SharedConstants.TeamHealthRadarName;
            addCompanyPage2.SelectRadar(radarName);

            addCompanyPage2.ClickNextButton();
            addCompanyPage3.WaitUntilLoaded();

            addCompanyPage3.FillSubscriptionInfo(_companyRequest);

            addCompanyPage3.ClickNextButton();
            addCompanyPage4.WaitUntilLoaded();

            addCompanyPage4.FillSecurityInfo(_companyRequest);

            addCompanyPage4.Header.ClickDeleteButton();
            companyDashboardPage.WaitUntilLoaded();

            Log.Info("Validate the created company does not show on the dashboard");
            Assert.IsFalse(companyDashboardPage.IsCompanyPresent(_companyRequest.Name), 
                $"Company <{_companyRequest.Name}> should NOT have been created.");
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