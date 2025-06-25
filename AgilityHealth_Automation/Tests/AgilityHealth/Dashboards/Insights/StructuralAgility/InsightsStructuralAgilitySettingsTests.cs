using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Insights.StructuralAgility;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Insights.TeamAgility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Dashboards.Insights.StructuralAgility
{
    [TestClass]
    [TestCategory("Insights"), TestCategory("StructuralAgilityDashboard"), TestCategory("Dashboard")]
    public class InsightsStructuralAgilitySettingsTests : BaseTest
    {
        private const string WorkType = "Software Delivery";
        private const string Role = "Developer";

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        public void Insights_StructuralAgility_Settings_Update_Cancel()
        {
            var login = new LoginPage(Driver, Log);
            var structuralAgility = new StructuralAgilityPage(Driver, Log);
            var teamAgility = new TeamAgilityPage(Driver, Log);
            var settings = new StructuralAgilitySettingsPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            teamAgility.NavigateToPage(Company.Id);
            
            teamAgility.ClickOnStructuralAgilityTab();

            structuralAgility.ClickSettingsButton();

            var expectedCount = settings.GetNumberOfTeamsToSupport(WorkType, Role);
            settings.SetNumberOfTeamsToSupport(WorkType, Role, expectedCount + 1);

            settings.ClickCancelButton();

            structuralAgility.ClickSettingsButton();

            Log.Info("Verify the count did not update");
            var actualCount = settings.GetNumberOfTeamsToSupport(WorkType, Role);
            Assert.AreEqual(expectedCount, actualCount, $"Role Allocation for {WorkType} - {Role} should not be updated.");
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        public void Insights_StructuralAgility_Settings_Update_Save()
        {
            var login = new LoginPage(Driver, Log);
            var structuralAgility = new StructuralAgilityPage(Driver, Log);
            var teamAgility = new TeamAgilityPage(Driver, Log);
            var settings = new StructuralAgilitySettingsPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            teamAgility.NavigateToPage(Company.Id);

            teamAgility.ClickOnStructuralAgilityTab();

            structuralAgility.ClickSettingsButton();

            var expectedCount = settings.GetNumberOfTeamsToSupport(WorkType, Role) + 1;
            settings.SetNumberOfTeamsToSupport(WorkType, Role, expectedCount);

            settings.ClickSaveButton();

            structuralAgility.ClickSettingsButton();

            Log.Info("Verify the count did update");
            var actualCount = settings.GetNumberOfTeamsToSupport(WorkType, Role);
            Assert.AreEqual(expectedCount, actualCount, $"Role Allocation for {WorkType} - {Role} should be updated.");
        }

        [TestMethod]
        [TestCategory("KnownDefect")]
        [TestCategory("BLAdmin"), TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public void Insights_StructuralAgility_Settings_NonAdmin()
        {
            var login = new LoginPage(Driver, Log);
            var structuralAgility = new StructuralAgilityPage(Driver, Log);
            var teamAgility = new TeamAgilityPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            teamAgility.NavigateToPage(Company.Id);

            teamAgility.ClickOnStructuralAgilityTab();

            Log.Info("Verify Settings button does not appear.");
            Assert.IsFalse(structuralAgility.IsSettingsButtonVisible(), $"Settings button should not display for {User.Type:G}");
        }
    }
}