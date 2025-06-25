using System;
using System.IO;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Company;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Company.Add;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Company.Edit;
using AtCommon.Api;
using AtCommon.Dtos.Companies;
using AtCommon.ObjectFactories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Company.Add
{
    [TestClass]
    [TestCategory("Companies")]
    public class CompanyDashboardAddCompanyPage1Tests3 : BaseTest
    {
        private static AddCompanyRequest _companyRequest;

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void CompanyDashboard_AddCompany_Page1_SaveAsDraft()
        {
            var login = new LoginPage(Driver, Log);
            var companyDashboardPage = new CompanyDashboardPage(Driver, Log);
            var addCompanyPage1 = new AddCompany1CompanyProfilePage(Driver, Log);
            var editCompanyProfilePage = new EditCompanyProfilePage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            companyDashboardPage.WaitUntilLoaded();

            companyDashboardPage.ClickOnAddCompanyButton();
            addCompanyPage1.WaitUntilLoaded();

            _companyRequest = CompanyFactory.GetCompany("ZZZ_Add2UI", User.CompanyName);
            _companyRequest.Logourl = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\TestData\\1.jpg");

            addCompanyPage1.FillInCompanyProfileInfo(_companyRequest);
            if (User.IsSiteAdmin())
            {
                addCompanyPage1.FillInAdminInfo(_companyRequest);
            }

            addCompanyPage1.Header.ClickSaveAsDraftButton();
            companyDashboardPage.WaitUntilLoaded();
            
            Log.Info("Validate the created company shows on the dashboard");
            Assert.IsTrue(companyDashboardPage.IsCompanyPresent(_companyRequest.Name), 
                $"Company <{_companyRequest.Name}> should have been created.");

            Log.Info("Validate the company Life Cycle shows as 'Draft'");
            Assert.AreEqual("Draft",companyDashboardPage.GetCompanyDetail(_companyRequest.Name, "Life Cycle"), "Life Cycle does not match");

            companyDashboardPage.ClickEditIconByCompanyName(_companyRequest.Name);
            editCompanyProfilePage.WaitUntilLoaded();

            Log.Info("Verify the updated info is displayed");
            var actualCompanyProfile = editCompanyProfilePage.GetCompanyProfile();

            // profile
            Assert.IsTrue(actualCompanyProfile.Logourl.Contains("/companylogos/"), "Logourl does not contain '/companylogos/' ");
            Assert.AreEqual(_companyRequest.Name, actualCompanyProfile.Name, "Name does not match");
            Assert.AreEqual(_companyRequest.Country, actualCompanyProfile.Country, "Country does not match.");
            Assert.AreEqual(_companyRequest.Size, actualCompanyProfile.Size, "Size does not match.");
            Assert.AreEqual(_companyRequest.Industry, actualCompanyProfile.Industry, "Industry does not match.");
            Assert.IsTrue(string.IsNullOrEmpty(actualCompanyProfile.LifeCycleStage),"LifeCycleStage is still selected");
            Assert.AreEqual(_companyRequest.IsoLanguageCode, actualCompanyProfile.IsoLanguageCode,"Preferred Language does not match.");

            // company admin
            var actualCompanyAdmin = editCompanyProfilePage.GetCompanyAdmin();
            Assert.AreEqual(_companyRequest.CompanyAdminFirstName, actualCompanyAdmin.FirstName,
                "AccountManagerFirstName does not match.");
            Assert.AreEqual(_companyRequest.CompanyAdminLastName, actualCompanyAdmin.LastName,
                "AccountManagerLastName does not match");
            Assert.AreEqual(_companyRequest.CompanyAdminEmail, actualCompanyAdmin.Email,
                "AccountManagerEmail does not match");

            // site admin only
            if (User.IsSiteAdmin())
            {
                var actualAdminInfo = editCompanyProfilePage.GetWatchListInfo();
                Assert.AreEqual(_companyRequest.CompanyType, actualAdminInfo.Type, "Company Type does not match");
                Assert.AreEqual(_companyRequest.WatchList, actualAdminInfo.WatchList, "WatchList does not match");
                Assert.AreEqual(_companyRequest.ReferralType.ToLower(), actualAdminInfo.ReferralType,
                    "Referral Type does not match");
                Assert.AreEqual(_companyRequest.CompanyPartnerReferral, actualAdminInfo.CompanyPartnerReferral,
                    "CompanyPartnerReferral does not match");
            }

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
