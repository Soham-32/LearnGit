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
    public class CompanySecurityEditTests1 : CompanyEditBaseTest
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
        public void Company_UpdateSecurity_Save()
        {
            var login = new LoginPage(Driver, Log);
            var companyDashboardPage = new CompanyDashboardPage(Driver, Log);
            var editSecurityPage = new EditCompanySecurityPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            companyDashboardPage.WaitUntilLoaded();

            companyDashboardPage.ClickEditIconByCompanyName(_companyRequest.Name);
            
            editSecurityPage.Header.ClickOnSecurityTab();
            editSecurityPage.WaitUntilLoaded();

            var editedCompany = CompanyFactory.GetCompanyForUpdate();

            editSecurityPage.FillSecurityInfo(editedCompany);

            editSecurityPage.Header.ClickSaveButton();
            companyDashboardPage.WaitUntilLoaded();

            companyDashboardPage.ClickEditIconByCompanyName(_companyRequest.Name);

            editSecurityPage.Header.ClickOnSecurityTab();
            editSecurityPage.WaitUntilLoaded();

            Log.Info("Verify Security info is updated");
            var actualSecurity = editSecurityPage.GetSecurityInfo();

            Assert.AreEqual(editedCompany.SessionTimeout, actualSecurity.SessionTimeout, 
                "SessionTimeout does not match.");
            Assert.AreEqual(editedCompany.MaxSessionLength, actualSecurity.MaxSessionLength, 
                "MaxSessionLength does not match.");
            Assert.AreEqual(editedCompany.ForcePasswordUpdate, actualSecurity.ForcePasswordUpdate, 
                "ForcePasswordUpdate does not match.");
            Assert.AreEqual(editedCompany.RequireSecurityQuestions, actualSecurity.RequireSecurityQuestions,
                "RequireSecurityQuestions does not match.");
            Assert.AreEqual(editedCompany.TwoFactorCompanyAdmin, actualSecurity.TwoFactorCompanyAdmin, 
                "TwoFactorCompanyAdmin does not match.");
            Assert.AreEqual(editedCompany.TwoFactorOrgLeader, actualSecurity.TwoFactorOrgLeader, 
                "TwoFactorOrgLeader does not match.");
            Assert.AreEqual(editedCompany.TwoFactorBuAdmin, actualSecurity.TwoFactorBuAdmin, 
                "TwoFactorBuAdmin does not match.");
            Assert.AreEqual(editedCompany.TwoFactorTeamAdmin, actualSecurity.TwoFactorTeamAdmin, 
                "TwoFactorTeamAdmin does not match.");
            Assert.AreEqual(editedCompany.LogoutUrl, actualSecurity.LogoutUrl, 
                "LogoutUrl does not match.");
            Assert.AreEqual(editedCompany.AuditTrailRetentionPeriod, 
                actualSecurity.AuditTrailRetentionPeriod, "SessionTimeout does not match.");
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