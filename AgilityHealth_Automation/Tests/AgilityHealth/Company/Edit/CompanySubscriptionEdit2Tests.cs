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
    public class CompanySubscriptionEdit2Tests : CompanyEditBaseTest
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
        public void Company_UpdateSubscription_Cancel()
        {
            var login = new LoginPage(Driver, Log);
            var companyDashboardPage = new CompanyDashboardPage(Driver, Log);
            var editSubscriptionPage = new EditCompanySubscriptionPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            companyDashboardPage.ClickEditIconByCompanyName(_companyRequest.Name);

            editSubscriptionPage.Header.ClickOnSubscriptionTab();
            editSubscriptionPage.WaitUntilLoaded();

            var editedCompany = CompanyFactory.GetCompanyForUpdate();

            editSubscriptionPage.FillSubscriptionInfo(editedCompany);

            editSubscriptionPage.FillAccountManagerInfo(editedCompany);

            editSubscriptionPage.ClickLicenseKeyAddButton();

            editSubscriptionPage.Header.ClickCancelButton();
            companyDashboardPage.WaitUntilLoaded();

            companyDashboardPage.ClickEditIconByCompanyName(_companyRequest.Name);

            editSubscriptionPage.Header.ClickOnSubscriptionTab();
            editSubscriptionPage.WaitUntilLoaded();

            var expectedLicenseKeyList = new List<CompanyLicenseDto>
            {
                _companyRequest.CompanyLicenses.First()
            };

            Log.Info("Verify License Key info");
            var actualLicenseKeyInfo = editSubscriptionPage.GetLicenseKeyInfo();

            Assert.AreEqual(expectedLicenseKeyList.Count, actualLicenseKeyInfo.Count, "Length of lists do not match");
            foreach (var licenseInfo in expectedLicenseKeyList)
            {
                var actual = actualLicenseKeyInfo.SingleOrDefault(a => a.Key == licenseInfo.Key).CheckForNull();
                Assert.AreEqual(licenseInfo.Origin, actual.Origin);
                Assert.AreEqual(licenseInfo.Quantity, actual.Quantity);
            }

            Assert.IsFalse(actualLicenseKeyInfo.Any(k => k.Key == editedCompany.CompanyLicenses.First().Key), "New license key was created");

            Log.Info("Verify Subscription info is updated");
            var actualSubscription = editSubscriptionPage.GetSubscriptionInfo();

            Assert.AreEqual(_companyRequest.SubscriptionType, actualSubscription.SubscriptionType, 
                "SubscriptionType doesn't match.");
            Assert.AreEqual(_companyRequest.AssessmentsLimit, actualSubscription.AssessmentsLimit, 
                "AssessmentsLimit doesn't match.");
            Assert.AreEqual(_companyRequest.TeamsLimit, actualSubscription.TeamsLimit, 
                "TeamsLimit doesn't match.");
            if (_companyRequest.DateContractSigned != null && actualSubscription.DateContractSigned != null)
                Assert.AreEqual(_companyRequest.DateContractSigned.Value.Date, actualSubscription.DateContractSigned.Value.Date, "DateContractSigned doesn't match.");
            if (_companyRequest.ContractEndDate != null && actualSubscription.ContractEndDate != null)
                Assert.AreEqual(_companyRequest.ContractEndDate.Value.Date, actualSubscription.ContractEndDate.Value.Date, "ContractEndDate doesn't match.");

            Log.Info("Verify Account Manager info is updated");
            var actualAccountManager = editSubscriptionPage.GetAccountManagerInfo();
            Assert.AreEqual(_companyRequest.AccountManagerFirstName, 
                actualAccountManager.AccountManagerFirstName, "AccountManagerFirstName doesn't match.");
            Assert.AreEqual(_companyRequest.AccountManagerLastName, 
                actualAccountManager.AccountManagerLastName, "AccountManagerLastName doesn't match.");
            Assert.AreEqual(_companyRequest.AccountManagerEmail, 
                actualAccountManager.AccountManagerEmail, "AccountManagerEmail doesn't match.");
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