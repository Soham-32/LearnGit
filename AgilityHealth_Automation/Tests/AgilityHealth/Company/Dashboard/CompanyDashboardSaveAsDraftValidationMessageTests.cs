using System.Collections.Generic;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
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
    public class CompanyDashboardSaveAsDraftValidationMessageTests : CompanyEditBaseTest
    {
        public static readonly List<AddCompanyRequest> Companies = new List<AddCompanyRequest>();
        private static AddCompanyRequest _companyRequest;
        [ClassInitialize]
        public static void SetUp(TestContext _)
        {
            _companyRequest = CompanyFactory.GetCompany(partnerReferralCompany: User.CompanyName);
            _companyRequest.IsDraft = true;
            _companyRequest.Country = null;
            _companyRequest.Size = null;
            _companyRequest.IndustryId = null;
            _companyRequest.TimeZoneInfoId = null;
            _companyRequest.LifeCycleStage = null;

            var setUp = new SetupTeardownApi(TestEnvironment);
            setUp.CreateCompany(_companyRequest).GetAwaiter().GetResult();
        }

        [TestMethod]
        [TestCategory("KnownDefect")] // Bug : 48243
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void Company_SaveAsDraft_Edit_MandatoryField_Validation_Message()
        {
            var login = new LoginPage(Driver, Log);
            var companyDashboardPage = new CompanyDashboardPage(Driver, Log);
            var editCompanyProfilePage = new EditCompanyProfilePage(Driver, Log);

            var expectedRequiredFieldList = new List<string> { "Country of Headquarters", "Company Size", "Industry", "Preferred Timezone", "Life Cycle Stage" };

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            companyDashboardPage.WaitUntilLoaded();
            companyDashboardPage.Search(_companyRequest.Name);
            companyDashboardPage.ClickEditIconByCompanyName(_companyRequest.Name);
            editCompanyProfilePage.Header.ClickSaveButton();

            // Verify the Validation Message for required fields
            foreach (var requiredField in expectedRequiredFieldList)
            {
                Assert.AreEqual("Required", editCompanyProfilePage.GetFieldValidationMessage(requiredField), $"Validation message is not displayed for '{requiredField}' field");
            }
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
