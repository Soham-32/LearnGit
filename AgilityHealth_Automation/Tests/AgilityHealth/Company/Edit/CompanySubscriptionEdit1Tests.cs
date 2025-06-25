using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Company;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Company.Edit;
using AtCommon.Api;
using AtCommon.Dtos.Companies;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Company.Edit
{
    [TestClass]
    [TestCategory("Companies")]
    public class CompanySubscriptionEdit1Tests : CompanyEditBaseTest
    {
        private static AddCompanyRequest _companyRequest;

        [ClassInitialize]
        public static void SetUp(TestContext _)
        {
            _companyRequest = CreateCompany();
        }

        [TestMethod]
        [TestCategory("KnownDefect")] // Bug : 48243
        [TestCategory("Critical")]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void Company_UpdateSubscription_Save()
        {
            var login = new LoginPage(Driver, Log);
            var companyDashboardPage = new CompanyDashboardPage(Driver, Log);
            var editSubscriptionPage = new EditCompanySubscriptionPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            companyDashboardPage.WaitUntilLoaded();

            companyDashboardPage.ClickEditIconByCompanyName(_companyRequest.Name);

            editSubscriptionPage.Header.ClickOnSubscriptionTab();
            editSubscriptionPage.WaitUntilLoaded();

            var editedCompany = CompanyFactory.GetCompanyForUpdate();

            editSubscriptionPage.FillLicenseKeyInfo(editedCompany.CompanyLicenses.First());

            editSubscriptionPage.FillSubscriptionInfo(editedCompany);

            editSubscriptionPage.FillAccountManagerInfo(editedCompany);

            editSubscriptionPage.Header.ClickSaveButton();
            companyDashboardPage.WaitUntilLoaded();

            companyDashboardPage.ClickEditIconByCompanyName(_companyRequest.Name);

            editSubscriptionPage.Header.ClickOnSubscriptionTab();
            editSubscriptionPage.WaitUntilLoaded();

            var expectedLicenseKeyList = new List<CompanyLicenseDto>
            {
                _companyRequest.CompanyLicenses.First(),
                editedCompany.CompanyLicenses.First()
            };

            Log.Info("Verify License Key info");
            var actualLicenseKeyInfo = editSubscriptionPage.GetLicenseKeyInfo();

            Assert.AreEqual(expectedLicenseKeyList.Count, actualLicenseKeyInfo.Count, "Length of lists do not match");
            foreach (var licenseInfo in expectedLicenseKeyList)
            {
                var actual = actualLicenseKeyInfo.SingleOrDefault(a => a.Key == licenseInfo.Key).CheckForNull();
                Assert.AreEqual(licenseInfo.Origin, actual.Origin, "Origins do not match");
                Assert.AreEqual(licenseInfo.Quantity, actual.Quantity, "The quantities do not match");
            }
            
            Log.Info("Verify Subscription info is updated");
            var actualSubscription = editSubscriptionPage.GetSubscriptionInfo();

            Assert.AreEqual(editedCompany.SubscriptionType, actualSubscription.SubscriptionType, 
                "SubscriptionType doesn't match.");
            Assert.AreEqual(editedCompany.AssessmentsLimit, actualSubscription.AssessmentsLimit, 
                "AssessmentsLimit doesn't match.");
            Assert.AreEqual(editedCompany.TeamsLimit, actualSubscription.TeamsLimit, 
                "TeamsLimit doesn't match.");
            Assert.AreEqual(editedCompany.DateContractSigned, actualSubscription.DateContractSigned, 
                "DateContractSigned doesn't match.");
            Assert.AreEqual(editedCompany.ContractEndDate, actualSubscription.ContractEndDate, 
                "ContractEndDate doesn't match.");

            Log.Info("Verify Account Manager info is updated");
            var actualAccountManager = editSubscriptionPage.GetAccountManagerInfo();
            Assert.AreEqual(editedCompany.AccountManagerFirstName, actualAccountManager.AccountManagerFirstName, 
                "AccountManagerFirstName doesn't match.");
            Assert.AreEqual(editedCompany.AccountManagerLastName, actualAccountManager.AccountManagerLastName, 
                "AccountManagerLastName doesn't match.");
            Assert.AreEqual(editedCompany.AccountManagerEmail, actualAccountManager.AccountManagerEmail, 
                "AccountManagerEmail doesn't match.");
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
