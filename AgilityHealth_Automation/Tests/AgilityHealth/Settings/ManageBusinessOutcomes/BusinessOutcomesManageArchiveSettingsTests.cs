using System;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.CardType;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Common;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.ManageBusinessOutcomes;
using AgilityHealth_Automation.PageObjects.AgilityHealth.V2;
using AgilityHealth_Automation.Tests.AgilityHealth.BusinessOutcomes;
using AtCommon.Api;
using AtCommon.Api.Enums;
using AtCommon.Dtos;
using AtCommon.Dtos.BusinessOutcomes;
using AtCommon.Dtos.Companies;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Settings.ManageBusinessOutcomes
{
    [TestClass]
    [TestCategory("Settings"), TestCategory("ManageBusinessOutcomes")]
    public class BusinessOutcomesManageArchiveSettingsTests : BusinessOutcomesBaseTest
    {
        private static bool _classInitFailed;
        private static BusinessOutcomeResponse _boCard;
        private static readonly UserConfig SiteAdminUserConfig = new UserConfig("SA");
        private static User SiteAdmin => SiteAdminUserConfig.GetUserByDescription("user 1");

        [ClassInitialize]
        public static void ClassSetUp(TestContext _)
        {
            try
            {
                var setupApi = new SetupTeardownApi(TestEnvironment);
                var request = GetBusinessOutcomeRequest(SwimlaneType.StrategicTheme);
                request.TeamId = setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(SharedConstants.EnterpriseTeam).TeamId;
                _boCard = setupApi.CreateBusinessOutcome(request);
            }

            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }

        [TestMethod]
        [TestCategory("KnownDefect")] //Bug Id: 51207
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public void BusinessOutcomes_Settings_Delete_Archive_Restore()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);
            var businessOutcomesDashboard = new BusinessOutcomesDashboardPage(Driver, Log);

            Log.Info("Login with admin, Delete card, Restore card and verify all the details");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            BusinessOutcomeCard_Delete_Restore_Method(User.Username);

            Log.Info("Login with site admin, Delete the same card, Restore card and verify all the details");
            topNav.LogOut();
            login.LoginToApplication(SiteAdmin.Username, SiteAdmin.Password);
            businessOutcomesDashboard.NavigateToPage(Company.Id);
            BusinessOutcomeCard_Delete_Restore_Method(SiteAdmin.Username);
        }

        private void BusinessOutcomeCard_Delete_Restore_Method(string username)
        {
            var v2SettingsPage = new SettingsV2Page(Driver, Log);
            var leftNav = new LeftNavPage(Driver, Log);
            var manageBoTagsPage = new ManageBusinessOutcomeTagsPage(Driver, Log);
            var addBusinessOutcomePage = new BusinessOutcomeCardPage(Driver, Log);
            var businessOutcomesArchiveSettingsPage = new BusinessOutcomesArchiveSettingsPage(Driver, Log);
            var businessOutcomesDashboard = new BusinessOutcomesDashboardPage(Driver, Log);

            Log.Info("Navigate to business outcomes dashboard, 'Delete' created card and verify card is not displayed on the dashboard");
            businessOutcomesDashboard.NavigateToPage(Company.Id);
            leftNav.ClickOnTeamName(SharedConstants.EnterpriseTeam);
            businessOutcomesDashboard.WaitForReload();
            businessOutcomesDashboard.ClickOnBusinessOutcomeLink(_boCard.Title);
            addBusinessOutcomePage.ClickOnDeleteButton();
            addBusinessOutcomePage.DeletePopUp_ClickOnDeleteCardButton();
            Assert.IsFalse(businessOutcomesDashboard.IsBusinessOutcomePresent(_boCard.Title), $"New created business outcome with title {_boCard.Title} is displayed on the dashboard.");

            Log.Info("Go to 'Manage Archive Settings', Filter the card with title and verify all the details");
            v2SettingsPage.NavigateToPage(Company.Id);
            v2SettingsPage.ClickOnBusinessOutcomesManageSettingsButton();
            manageBoTagsPage.ClickOnManageArchiveSettingsButton();

            businessOutcomesArchiveSettingsPage.FilterWithCardTitle(_boCard.Title);

            var archivedBusinessOutcome = businessOutcomesArchiveSettingsPage.GetArchivedBusinessOutcome(_boCard.Title);
            Assert.AreEqual(_boCard.Title,archivedBusinessOutcome.Title,"Business Outcome title is not matched");
            Assert.IsTrue(businessOutcomesArchiveSettingsPage.IsRestoreButtonDisplayed(_boCard.Title), "'Restore' button is not displayed");
            Assert.AreEqual(_boCard.PrettyId, archivedBusinessOutcome.Id, "Card 'Id' is not displayed");
            Assert.AreEqual(SharedConstants.EnterpriseTeam, archivedBusinessOutcome.TeamName, "'Team Name' is not displayed");
            Assert.That.TimeIsClose(DateTime.UtcNow, (DateTime)archivedBusinessOutcome.ArchivedAt!,8,"Archived dateTime does not match");
            Assert.AreEqual(username, archivedBusinessOutcome.ArchivedBy, "'Archived By' name is not displayed");
            Assert.AreEqual("", archivedBusinessOutcome.RestoredBy, "'Restored By' name is displayed");
            Assert.AreEqual(User.FullName, archivedBusinessOutcome.CreatedBy, "'Created By' name is not displayed");
            Assert.IsNull(archivedBusinessOutcome.RestoredAt, "'Restored At' date is displayed");

            Log.Info("'Restore' the card and verify all the details");
            businessOutcomesArchiveSettingsPage.ClickOnRestoreButton(_boCard.Title);
            Assert.IsTrue(businessOutcomesArchiveSettingsPage.IsConfirmRestorePopupTitleDisplayed(), "'Confirm Restore' popup title is not displayed");
            businessOutcomesArchiveSettingsPage.ClickOnCancelButtonOfConfirmRestorePopup();
            Assert.IsTrue(businessOutcomesArchiveSettingsPage.IsRestoreButtonDisplayed(_boCard.Title), "'Restore' button is not displayed");

            businessOutcomesArchiveSettingsPage.ClickOnRestoreButton(_boCard.Title);
            businessOutcomesArchiveSettingsPage.ClickOnRestoreButtonOfConfirmRestorePopup();

            archivedBusinessOutcome = businessOutcomesArchiveSettingsPage.GetArchivedBusinessOutcome(_boCard.Title);

            Assert.IsTrue(businessOutcomesArchiveSettingsPage.IsRestoredSuccessfullyToasterMessageDisplayed(), "Business outcome restored successfully toaster message is not displayed");
            Assert.IsFalse(businessOutcomesArchiveSettingsPage.IsRestoreButtonDisplayed(_boCard.Title), "'Restore' button is displayed");
            Assert.AreEqual(_boCard.Title, archivedBusinessOutcome.Title, "Business Outcome title is not matched");
            Assert.AreEqual(_boCard.PrettyId, archivedBusinessOutcome.Id, "Card 'Id' is not displayed");
            Assert.AreEqual(SharedConstants.EnterpriseTeam, archivedBusinessOutcome.TeamName, "'Team Name' is not displayed");
            Assert.IsNull(archivedBusinessOutcome.ArchivedAt, "'Archived At' date is null");
            Assert.AreEqual("", archivedBusinessOutcome.ArchivedBy, "'Archived By' name is displayed");
            Assert.That.TimeIsClose(DateTime.UtcNow, (DateTime)archivedBusinessOutcome.RestoredAt!, 8, "Restored dateTime does not matched");
            Assert.AreEqual(username, archivedBusinessOutcome.RestoredBy, "'Restored By' name is not displayed");
            Assert.AreEqual(User.FullName, archivedBusinessOutcome.CreatedBy, "'Created By' name is not displayed");

            Log.Info("Navigate to business outcomes dashboard and verify restored card is displayed on the dashboard");
            businessOutcomesDashboard.NavigateToPage(Company.Id);
            leftNav.ClickOnTeamName(SharedConstants.EnterpriseTeam);
            businessOutcomesDashboard.WaitForReload();
            Assert.IsTrue(businessOutcomesDashboard.IsBusinessOutcomePresent(_boCard.Title),
                $"New created business outcome with title {_boCard.Title} is not displayed on the dashboard.");

        }
    }
}