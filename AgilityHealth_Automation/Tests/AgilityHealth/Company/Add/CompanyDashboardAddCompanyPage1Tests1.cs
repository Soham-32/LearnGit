using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Company;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Company.Add;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Company.Add
{
    [TestClass]
    [TestCategory("Companies")]
    public class CompanyDashboardAddCompanyPage1Tests1 : BaseTest
    {

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void CompanyDashboard_AddCompany_Page1_Close()
        {
            var login = new LoginPage(Driver, Log);
            var companyDashboardPage = new CompanyDashboardPage(Driver, Log);
            var addCompanyPage1 = new AddCompany1CompanyProfilePage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            companyDashboardPage.WaitUntilLoaded();

            companyDashboardPage.ClickOnAddCompanyButton();
            addCompanyPage1.WaitUntilLoaded();

            addCompanyPage1.Header.ClickCloseButton();
            companyDashboardPage.WaitUntilLoaded();

            Log.Info("Validate the user is taken directly back to the Company Dashboard");
            Assert.IsTrue(companyDashboardPage.IsColumnVisible("Company Name"),
                "Company Dashboard should be displayed");
        }

    }
}