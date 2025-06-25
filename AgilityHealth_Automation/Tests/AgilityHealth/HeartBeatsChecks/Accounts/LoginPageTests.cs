using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Common;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Company;
using AgilityHealth_Automation.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.HeartBeatsChecks.Accounts
{
    [TestClass]
    [TestCategory("HeartBeatChecks"), TestCategory("OE_Login")]
    public class LoginPageTests : BaseTest
    {
        [TestMethod]
        [DynamicData(nameof(Constants.KeyCustomerCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]

        //We haven't included the 'Agilityhealth', 'Demo' domains because we don't have credentials for these domains
        //We haven't inlcuded fidelity because we don't access to a 'fidelity' domain        
        public void VerifyThatLoginSuccessfully(string env)
        {
            var companyDashboardPage = new CompanyDashboardPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);

            Log.Info("Verify that user is able to login and logout successfully");
            LoginToProductionEnvironment(env);
            Assert.IsTrue(companyDashboardPage.IsAddCompanyButtonDisplayed(), $"Company dashboard not loaded successfully for '{env}'");
            Assert.IsTrue(companyDashboardPage.IsCompanyNameListDisplayed(), $"Company list is not displayed for '{env}'");

            topNav.LogOut();
            if (topNav.IsLogoutSuccessfulTextPresent())
            {
                Assert.IsTrue(topNav.IsLogoutSuccessfulTextPresent(), $"The Logout Successful text is not matched for '{env}'");
            }
            else if (topNav.IsAccessYourAccountTextPresent())
            {
                Assert.IsTrue(topNav.IsAccessYourAccountTextPresent(), $"The Access Your Account text is not matched for '{env}'");
            }
            else
            {
                Assert.Fail($"Neither logout confirmation nor access account message is displayed for '{env}'");
            }
        }
    }
}
