using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Company;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Company.Edit;
using AtCommon.Api;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Company.Edit
{
    [TestClass]
    [TestCategory("Companies")]
    public class CompanyProfileEditTests1 : CompanyEditBaseTest
    {
        private static readonly List<string> Companies = new List<string>();

        [ClassInitialize]
        public static void SetUp(TestContext _)
        {
            var company = CreateCompany();
            Companies.Add(company.Name);
        }

        [TestMethod]
        [TestCategory("KnownDefect")] //Bug Id: 48243
        [TestCategory("Critical")]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void Company_UpdateCompanyProfile_Save()
        {
            var login = new LoginPage(Driver, Log);
            var companyDashboardPage = new CompanyDashboardPage(Driver, Log);
            var editCompanyProfilePage = new EditCompanyProfilePage(Driver, Log);

            Log.Info("Login to the application and navigate to company dashboard page");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            companyDashboardPage.WaitUntilLoaded();

            Log.Info($"Edit {Companies.First()} company by clicking on 'Edit' icon");
            companyDashboardPage.ClickEditIconByCompanyName(Companies.First());
            editCompanyProfilePage.WaitUntilLoaded();
            var editedCompany = CompanyFactory.GetCompanyForUpdate();
            editedCompany.Logourl = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\TestData\\1.jpg");

            Log.Info("Verify all the languages from 'Preferred Language' dropdown on the company profile page");
            var expectedLanguageList = ManageRadarFactory.Languages();
            var actualLanguageList = editCompanyProfilePage.GetAllLanguages();
            Assert.That.ListsAreEqual(expectedLanguageList, actualLanguageList, "Language list doesn't match");

            Log.Info("Fill all the company information on the company profile page");
            editCompanyProfilePage.FillInCompanyProfileInfo(editedCompany);
            if (User.IsSiteAdmin())
            {
                editCompanyProfilePage.FillInAdminInfo(editedCompany);
            }

            Log.Info("Click on 'Save' button on the company profile page");
            editCompanyProfilePage.Header.ClickSaveButton();
            Companies.Add(editedCompany.Name);
            companyDashboardPage.WaitUntilLoaded();

            Log.Info($"Edit {editedCompany.Name} company by clicking on 'Edit' icon");
            companyDashboardPage.ClickEditIconByCompanyName(editedCompany.Name);
            editCompanyProfilePage.WaitUntilLoaded();

            Log.Info("Verify the updated info is displayed");
            var actualCompanyProfile = editCompanyProfilePage.GetCompanyProfile();

            // profile
            Assert.IsTrue(actualCompanyProfile.Logourl.Contains("/companylogos/"), "Logourl does not contain '/companylogos/' ");
            Assert.AreEqual(editedCompany.Name, actualCompanyProfile.Name, "Name does not match");
            Assert.AreEqual(editedCompany.Country, actualCompanyProfile.Country, "Country does not match.");
            Assert.AreEqual(editedCompany.Size, actualCompanyProfile.Size, "Size does not match.");
            Assert.AreEqual(editedCompany.Industry, actualCompanyProfile.Industry, "Industry does not match.");
            Assert.AreEqual(editedCompany.LifeCycleStage, actualCompanyProfile.LifeCycleStage, 
                "LifeCycleStage does not match.");
            Assert.AreEqual(editedCompany.IsoLanguageCode, actualCompanyProfile.IsoLanguageCode, "Preferred Language does not match.");

            // company admin
            var actualCompanyAdmin = editCompanyProfilePage.GetCompanyAdmin();
            Assert.AreEqual(editedCompany.CompanyAdminFirstName, actualCompanyAdmin.FirstName, 
                "AccountManagerFirstName does not match.");
            Assert.AreEqual(editedCompany.CompanyAdminLastName, actualCompanyAdmin.LastName, 
                "AccountManagerLastName does not match");
            Assert.AreEqual(editedCompany.CompanyAdminEmail, actualCompanyAdmin.Email, 
                "AccountManagerEmail does not match");

            // site admin only
            if (User.IsSiteAdmin())
            {
                var actualAdminInfo = editCompanyProfilePage.GetWatchListInfo();
                Assert.AreEqual(editedCompany.CompanyType, actualAdminInfo.Type, "Company Type does not match");
                Assert.AreEqual(editedCompany.WatchList, actualAdminInfo.WatchList, "WatchList does not match");
                Assert.AreEqual(editedCompany.ReferralType.ToLower(), actualAdminInfo.ReferralType, 
                    "Referral Type does not match");
                Assert.AreEqual(editedCompany.CompanyPartnerReferral, actualAdminInfo.CompanyPartnerReferral, 
                    "CompanyPartnerReferral does not match");
            }
        }

        [ClassCleanup]
        public static void ClassTearDown()
        {
            if (!User.IsSiteAdmin() && !User.IsPartnerAdmin()) return;
            var setup = new SetupTeardownApi(TestEnvironment);
            foreach (var company in Companies)
            {
                setup.DeleteCompany(company).GetAwaiter().GetResult(); 
            }
        }

    }
}
