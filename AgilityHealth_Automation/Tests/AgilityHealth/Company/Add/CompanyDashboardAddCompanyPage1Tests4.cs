using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Company;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Company.Add;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Company.Add
{
    [TestClass]
    [TestCategory("Companies")]
    public class CompanyDashboardAddCompanyPage1Tests4 : BaseTest
    {

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void CompanyDashboard_AddCompany_Page1_IsSiteAdmin_True()
        {
            var login = new LoginPage(Driver, Log);
            var companyDashboardPage = new CompanyDashboardPage(Driver, Log);
            var addCompanyPage1 = new AddCompany1CompanyProfilePage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            companyDashboardPage.WaitUntilLoaded();

            companyDashboardPage.ClickOnAddCompanyButton();
            addCompanyPage1.WaitUntilLoaded();

            var email = User.Username;
            if (User.IsPartnerAdmin())
            {
                var siteAdminUserConfig = new UserConfig("SA");
                var siteAdminUser = siteAdminUserConfig.GetUserByDescription("user 1");
                email = siteAdminUser.Username;
            }
            addCompanyPage1.EnterCompanyAdminEmailInfo(email,4);

            Log.Info("Verify that correct validation message is displayed");
            Assert.AreEqual("Site Admins cannot be added as the Company Admin", addCompanyPage1.GetCompanyAdminEmailValidationMessage(), "Validation message does not match");
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void CompanyDashboard_AddCompany_Page1_IsSiteAdmin_False()
        {
            var login = new LoginPage(Driver, Log);
            var companyDashboardPage = new CompanyDashboardPage(Driver, Log);
            var addCompanyPage1 = new AddCompany1CompanyProfilePage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            companyDashboardPage.WaitUntilLoaded();

            companyDashboardPage.ClickOnAddCompanyButton();
            addCompanyPage1.WaitUntilLoaded();

            var email = CSharpHelpers.Random8Number() + "test@mail.com";
            
            addCompanyPage1.EnterCompanyAdminEmailInfo(email,4);

            Log.Info("Verify that validation message is NOT displayed");
            var message = addCompanyPage1.GetCompanyAdminEmailValidationMessage();
            Assert.IsTrue(message.Length.Equals(0), "Validation message is displayed : " + message);
        }


    }
}
