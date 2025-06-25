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
    public class CompanyProfileEditTests5 : CompanyEditBaseTest
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
        public void Company_UpdateCompanyProfile_Archive()
        {
            var login = new LoginPage(Driver, Log);
            var companyDashboardPage = new CompanyDashboardPage(Driver, Log);
            var editCompanyProfilePage = new EditCompanyProfilePage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            companyDashboardPage.WaitUntilLoaded();

            companyDashboardPage.ClickEditIconByCompanyName(_companyRequest.Name);
            editCompanyProfilePage.WaitUntilLoaded();

            var editedCompany = new UpdateCompanyRequest
            {
                LifeCycleStage = "Archived",
            };

            editCompanyProfilePage.SelectLifeCycleStage(editedCompany.LifeCycleStage);

            editCompanyProfilePage.Header.ClickSaveButton();

            companyDashboardPage.WaitUntilLoaded();
            companyDashboardPage.Search(_companyRequest.Name);

            Log.Info($"Verify Life Cycle updated to <{editedCompany.LifeCycleStage}>");
            Assert.IsTrue(
                companyDashboardPage.IsLifeCycleUpdated(_companyRequest.Name, editedCompany.LifeCycleStage),
                $"Life Cycle did not update to <{editedCompany.LifeCycleStage}>");

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
