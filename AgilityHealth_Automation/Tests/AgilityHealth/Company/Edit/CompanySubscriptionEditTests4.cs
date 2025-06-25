using System;
using System.Collections.Generic;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Company;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Company.Edit;
using AtCommon.Api;
using AtCommon.Dtos.Companies;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Company.Edit
{
    [TestClass]
    [TestCategory("Companies")]
    public class CompanySubscriptionEditTests4 : CompanyEditBaseTest
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
        public void Company_UpdateSubscription_SubscriptionHistory()
        {
            var login = new LoginPage(Driver, Log);
            var companyDashboardPage = new CompanyDashboardPage(Driver, Log);
            var editSubscriptionPage = new EditCompanySubscriptionPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            companyDashboardPage.ClickEditIconByCompanyName(_companyRequest.Name);

            editSubscriptionPage.Header.ClickOnSubscriptionTab();
            editSubscriptionPage.WaitUntilLoaded();

            var edit1 = new AddCompanyRequest
            {
                SubscriptionType = "Pilot",
                AssessmentsLimit = 100,
                TeamsLimit = 50,
                DateContractSigned = DateTime.Today,
                ContractEndDate = DateTime.Today.AddYears(1),
                ManagedSubscription= true
            };

            editSubscriptionPage.FillSubscriptionInfo(edit1);
            
            editSubscriptionPage.Header.ClickSaveButton();

            var expectedSubscriptions = new List<SubscriptionHistoryItem>
            {
                new SubscriptionHistoryItem
                {
                    Id = 1,
                    SubscriptionType = _companyRequest.SubscriptionType,
                    TeamsLimit = _companyRequest.TeamsLimit,
                    AssessmentsLimit =  _companyRequest.AssessmentsLimit,
                    DateContractSigned = _companyRequest.DateContractSigned?.ToString("M/d/yyyy"),
                    ContractEndDate = _companyRequest.ContractEndDate?.ToString("M/d/yyyy")
                }
            };

            companyDashboardPage.WaitUntilLoaded();

            companyDashboardPage.ClickEditIconByCompanyName(_companyRequest.Name);

            editSubscriptionPage.Header.ClickOnSubscriptionTab();
            editSubscriptionPage.WaitUntilLoaded();

            Log.Info("Verify subscription info");
            var actualHistory1 = editSubscriptionPage.GetSubscriptionHistory();

            for (var i = 0; i < expectedSubscriptions.Count; i++)
            {
                Assert.AreEqual(expectedSubscriptions[i].Id, actualHistory1[i].Id,
                    $"Id doesn't match for row <{i}>");
                Assert.AreEqual(expectedSubscriptions[i].SubscriptionType, actualHistory1[i].SubscriptionType,
                    $"SubscriptionType doesn't match for row <{i}>");
                Assert.AreEqual(expectedSubscriptions[i].TeamsLimit, actualHistory1[i].TeamsLimit,
                    $"Teams Limit doesn't match for row <{i}>");
                Assert.AreEqual(expectedSubscriptions[i].AssessmentsLimit, actualHistory1[i].AssessmentsLimit,
                    $"Assessment Limit doesn't match for row <{i}>");
                Assert.AreEqual(expectedSubscriptions[i].DateContractSigned, actualHistory1[i].DateContractSigned,
                    $"DateContractSigned doesn't match for row <{i}>");
                Assert.AreEqual(expectedSubscriptions[i].ContractEndDate, actualHistory1[i].ContractEndDate,
                    $"ContractEndDate doesn't match for row <{i}>");
            }

            Log.Info("Verifying whether Managed Subscription checkbox selected or not");
            Assert.AreEqual(edit1.ManagedSubscription, editSubscriptionPage.IsManagedSubscriptionCheckboxSelected(),
                "Managed Subscription checkbox info doesn't match");
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