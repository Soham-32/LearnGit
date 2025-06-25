using System;
using System.IO;
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
    public class CompanyProfileEditTests2 : CompanyEditBaseTest
    {
        private static AddCompanyRequest _companyRequest;

        [ClassInitialize]
        public static void SetUp(TestContext _)
        {
            _companyRequest = CreateCompany();
        }

        [TestMethod]
        [TestCategory("KnownDefect")] // Bug : 48243
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void Company_UpdateCompanyProfile_Cancel()
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
            editedCompany.Logourl = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\TestData\\1.jpg");

            editCompanyProfilePage.FillInCompanyProfileInfo(editedCompany);
            if (User.IsSiteAdmin())
            {
                editCompanyProfilePage.FillInAdminInfo(editedCompany);
            }

            editCompanyProfilePage.Header.ClickCancelButton();

            companyDashboardPage.WaitUntilLoaded();

            companyDashboardPage.ClickEditIconByCompanyName(_companyRequest.Name);
            editCompanyProfilePage.WaitUntilLoaded();

            Log.Info("Verify the updated info is displayed");
            var actualCompanyProfile = editCompanyProfilePage.GetCompanyProfile();

            // profile
            Assert.AreEqual(actualCompanyProfile.Logourl.Length,0, "Logourl isn't empty");
            Assert.AreEqual(_companyRequest.Name, actualCompanyProfile.Name, "Name does not match");
            Assert.AreEqual(_companyRequest.Country, actualCompanyProfile.Country, "Country does not match.");
            Assert.AreEqual(_companyRequest.Size, actualCompanyProfile.Size, "Size does not match.");
            Assert.AreEqual(_companyRequest.Industry, actualCompanyProfile.Industry, "Industry does not match.");
            Assert.AreEqual(_companyRequest.LifeCycleStage, actualCompanyProfile.LifeCycleStage, 
                "LifeCycleStage does not match.");
            Assert.IsTrue(actualCompanyProfile.IsoLanguageCode.ToLower().Contains(_companyRequest.IsoLanguageCode), "Preferred language does not match.");

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
    

