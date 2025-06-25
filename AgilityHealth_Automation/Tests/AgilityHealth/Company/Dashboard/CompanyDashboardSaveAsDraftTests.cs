using System.Collections.Generic;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Common;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Company;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Company.Edit;
using AtCommon.Api;
using AtCommon.Dtos.Companies;
using AtCommon.ObjectFactories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Company.Dashboard
{
    [TestClass]
    [TestCategory("Companies")]
    public class CompanyDashboardSaveAsDraftTests : CompanyEditBaseTest
    {
        private static readonly List<AddCompanyRequest> Companies = new List<AddCompanyRequest>();
        private static AddCompanyRequest _companyRequest;
        private static AddCompanyRequest _editedCompany;

        [ClassInitialize]
        public static void SetUp(TestContext _)
        {
            _companyRequest = CreateCompany(true);
            Companies.Add(_companyRequest);
        }

        [TestMethod]
        [TestCategory("KnownDefect")] // Bug : 38622
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void Company_Draft_Edit_Successful()
        {
            var login = new LoginPage(Driver, Log);
            var companyDashboardPage = new CompanyDashboardPage(Driver, Log);
            var editCompanyProfilePage = new EditCompanyProfilePage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            companyDashboardPage.WaitUntilLoaded();
            companyDashboardPage.Search(_companyRequest.Name);
            companyDashboardPage.ClickOnCompanyName(_companyRequest.Name);

            Log.Info("Verify that 'Draft Company' popup is displayed");
            Assert.IsTrue(companyDashboardPage.IsDraftCompanyPopupDisplayed(), "'Draft Company' popup is not displayed");

            Log.Info("Click on 'Cancel' button from 'Draft Company' popup and verify that 'Company Dashboard' is displayed");
            companyDashboardPage.ClickOnDraftCompanyPopupCancelButton();
            Assert.IsTrue(companyDashboardPage.IsCompanySearchFilterTextBoxDisplayed(), "'Company dashboard' page is not displayed");

            Log.Info("Click on 'Edit' button from 'Draft Company' popup and verify that 'Edit a Company' page is displayed");
            companyDashboardPage.ClickOnCompanyName(_companyRequest.Name);
            companyDashboardPage.ClickOnDraftCompanyPopupEditButton();
            editCompanyProfilePage.WaitUntilLoaded();

            // get company profile info and verify
            var actualCompanyProfile = editCompanyProfilePage.GetCompanyProfile();
            Assert.AreEqual(_companyRequest.Name, actualCompanyProfile.Name, "Name does not match");
            Assert.AreEqual(_companyRequest.Country, actualCompanyProfile.Country, "Country does not match.");
            Assert.AreEqual(_companyRequest.Size, actualCompanyProfile.Size, "Size does not match.");
            Assert.AreEqual(_companyRequest.Industry, actualCompanyProfile.Industry, "Industry does not match.");
            Assert.IsTrue(string.IsNullOrEmpty(actualCompanyProfile.LifeCycleStage), "LifeCycleStage is still selected");
            Assert.AreEqual("English", actualCompanyProfile.IsoLanguageCode, "Preferred Language does not match.");

            Log.Info("Change the company details and save");
            _editedCompany = CompanyFactory.GetCompanyForUpdate();
            editCompanyProfilePage.FillInCompanyProfileInfo(_editedCompany);
            editCompanyProfilePage.Header.ClickSaveButton();
            Companies.Add(_editedCompany);

            companyDashboardPage.WaitUntilLoaded();
            companyDashboardPage.Search(_editedCompany.Name);

            Assert.IsTrue(companyDashboardPage.IsCompanyPresent(_editedCompany.Name), $"Updated Company {_editedCompany.Name} is not present");
            Assert.IsTrue(companyDashboardPage.IsLifeCycleUpdated(_editedCompany.Name, _editedCompany.LifeCycleStage), $"Life Cycle did not update to <{_editedCompany.LifeCycleStage}>");

            Log.Info($"Verify user is redirected to 'Team Dashboard' Page' after clicking on company name <{_editedCompany.Name}> from 'Company Dashboard' Page");
            companyDashboardPage.ClickOnCompanyName(_editedCompany.Name);
            var breadcrumbs = topNav.GetBreadcrumbText();
            Assert.IsTrue(breadcrumbs.Contains(_editedCompany.Name),
                $"The company name <{_editedCompany.Name}> was not found in the breadcrumbs <{breadcrumbs}.>");
        }

        [ClassCleanup]
        public static void ClassTearDown()
        {
            if (!User.IsSiteAdmin() && !User.IsPartnerAdmin()) return;
            var setup = new SetupTeardownApi(TestEnvironment);
            foreach (var company in Companies)
            {
                setup.DeleteCompany(company.Name).GetAwaiter().GetResult();
            }
            
        }
    }
}
