using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.Utilities;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Chrome;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Accounts.Login
{
    [TestClass]
    [TestCategory("Login"),TestCategory("LanguageTranslation")]
    public class AutoDetectGeoLocationTests : BaseTest
    {
        private static ChromeDriver _driver;

        [ClassInitialize]
        public static void ClassSetUp(TestContext _)
        {
            var browser = new Browser();
            _driver = browser.GeoLocationSetUp(TestEnvironment);
        }

        //[TestMethod]
        [TestCategory("Smoke")]
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("Member")]
        public void VerifyGeoLocationLanguageTranslation()
        {
            Log.Info("Navigate to application and verify that the language displayed is from correct Geo-Location");
            var login = new LoginPage(_driver, Log);
            _driver.NavigateToPage(ApplicationUrl);
            var countryName = login.GetCountryName();

            var actualLanguage = login.GetLanguage();
            var expectedLanguage = SharedConstants.CountryLanguages[countryName];
            Assert.AreEqual(expectedLanguage,actualLanguage,"Location Language doesn't match");
        }

        [ClassCleanup]
        public static void ClassTearDown()
        {
            if (_driver == null) return;
            try
            {
                _driver.Close();
                _driver.Quit();
            }
            catch
            {
                _driver.Quit();
            }
        }
    }
}