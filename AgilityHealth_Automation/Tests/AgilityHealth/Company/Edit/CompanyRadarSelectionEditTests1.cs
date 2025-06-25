using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Company;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Company.Edit;
using AtCommon.Api;
using AtCommon.Dtos.Companies;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Company.Edit
{
    [TestClass]
    [TestCategory("Companies")]
    public class CompanyRadarSelectionEditTests1 : CompanyEditBaseTest
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
        public void Company_UpdateRadarSelection_Save()
        {
            var login = new LoginPage(Driver, Log);
            var companyDashboardPage = new CompanyDashboardPage(Driver, Log);
            var editCompanyProfilePage = new EditCompanyProfilePage(Driver, Log);
            var radarSelectionPage = new EditRadarSelectionPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            companyDashboardPage.WaitUntilLoaded();

            companyDashboardPage.ClickEditIconByCompanyName(_companyRequest.Name);
            editCompanyProfilePage.WaitUntilLoaded();

            editCompanyProfilePage.Header.ClickOnRadarSelectionTab();
            radarSelectionPage.WaitUntilLoaded();

            const string radarName = SharedConstants.TeamHealthRadarName;
            radarSelectionPage.SelectRadar(radarName);

            radarSelectionPage.Header.ClickSaveButton();
            companyDashboardPage.WaitUntilLoaded();
            
            companyDashboardPage.ClickEditIconByCompanyName(_companyRequest.Name);

            editCompanyProfilePage.WaitUntilLoaded();

            editCompanyProfilePage.Header.ClickOnRadarSelectionTab();
            radarSelectionPage.WaitUntilLoaded();

            Log.Info($"Verify that {radarName} is still checked");
            Assert.IsTrue(radarSelectionPage.IsRadarChecked(radarName), 
                $"The radar <{radarName}> should be checked.");

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
