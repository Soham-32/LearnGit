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
    public class CompanySubscriptionEdit3Tests : CompanyEditBaseTest
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
        public void Company_UpdateSubscription_CurrentUsage()
        {
            var login = new LoginPage(Driver, Log);
            var companyDashboardPage = new CompanyDashboardPage(Driver, Log);
            var editSubscriptionPage = new EditCompanySubscriptionPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            companyDashboardPage.ClickEditIconByCompanyName(_companyRequest.Name);

            editSubscriptionPage.Header.ClickOnSubscriptionTab();
            editSubscriptionPage.WaitUntilLoaded();

            var actualCurrentUsage = editSubscriptionPage.GetCurrentUsage();

            Assert.AreEqual(_companyRequest.Name, actualCurrentUsage.CompanyName, 
                "CompanyName does not match");
            // TODO: need to find a way to test actual values
            Assert.AreEqual(0, actualCurrentUsage.NumberOfTeams, "NumberOfTeams does not match");
            Assert.AreEqual(0, actualCurrentUsage.NumberOfMultiTeams, 
                "NumberOfMultiTeams does not match");
            Assert.AreEqual(0, actualCurrentUsage.NumberOfEnterpriseTeams, 
                "NumberOfEnterpriseTeams does not match");
            Assert.AreEqual(0, actualCurrentUsage.NumberOfTeamAssessments, 
                "NumberOfTeamAssessments does not match");
            Assert.AreEqual(0, actualCurrentUsage.NumberOfIndividualAssessments, 
                "NumberOfIndividualAssessments does not match");
            Assert.AreEqual(_companyRequest.CompanyType, actualCurrentUsage.CompanyType, 
                "CompanyType does not match");
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