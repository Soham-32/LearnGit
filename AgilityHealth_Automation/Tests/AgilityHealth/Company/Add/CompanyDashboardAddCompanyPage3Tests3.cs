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
    public class CompanyDashboardAddCompanyPage3Tests3 : BaseTest
    {
        private static AddCompanyRequest _companyRequest;

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void CompanyDashboard_AddCompany_Page3_Back()
        {
            var login = new LoginPage(Driver, Log);
            var companyDashboardPage = new CompanyDashboardPage(Driver, Log);
            var addCompanyPage1 = new AddCompany1CompanyProfilePage(Driver, Log);
            var addCompanyPage2 = new AddCompany2RadarSelectionPage(Driver, Log);
            var addCompanyPage3 = new AddCompany3SubscriptionPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            companyDashboardPage.WaitUntilLoaded();
            companyDashboardPage.ClickOnAddCompanyButton();
            
            addCompanyPage1.WaitUntilLoaded();

            _companyRequest = CompanyFactory.GetCompany("ZZZ_Add8UI", User.CompanyName);

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
            addCompanyPage3.ClickBackButton();
            
            addCompanyPage2.WaitUntilLoaded();

            Log.Info("Verify that the first page is displayed");
            Assert.IsTrue(addCompanyPage2.IsRadarSelectionTextVisible(), "The second page should be visible.");
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