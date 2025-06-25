using System;
using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.ManageBusinessOutcomes;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.ManageFeatures;
using AgilityHealth_Automation.Tests.AgilityHealth.BusinessOutcomes;
using AtCommon.Api;
using AtCommon.Api.Enums;
using AtCommon.Dtos.BusinessOutcomes;
using AtCommon.Dtos.Companies;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Settings.ManageBusinessOutcomes
{
    [TestClass]
    [TestCategory("Settings"), TestCategory("ManageBusinessOutcomes")]
    public class BusinessOutcomesDashboardSettingsTests : BusinessOutcomesBaseTest
    {
        private static bool _classInitFailed;
        private static string _classInitFailedMessage;
        private static TeamHierarchyResponse _team;
        private static TeamHierarchyResponse _multiTeam;
        private static TeamHierarchyResponse _enterpriseTeam;
        private static BusinessOutcomeResponse _threeYearOutcome;
        private static BusinessOutcomeResponse _oneYearOutcome;
        private static BusinessOutcomeResponse _oneYearOutcome1;
        private static List<BusinessOutcomeCategoryLabelResponse> _labels;

        [ClassInitialize]
        public static void ClassSetUp(TestContext _)
        {
            try
            {
                var setup = new SetupTeardownApi(TestEnvironment);

                _labels = setup.GetBusinessOutcomesAllLabels(Company.Id);
                var tag1 = _labels.Where(a => a.Name == "Automation Label1").Select(a => a.Tags[0]).ToList().First();
                var tag2 = _labels.Where(a => a.Name == "Automation Label1").Select(a => a.Tags[1]).ToList().First();

                var request1 = GetBusinessOutcomeRequest(SwimlaneType.StrategicIntent);
                request1.Tags.Add(new BusinessOutcomeTagRequest { Name = tag1.Name, Uid = tag1.Uid, CategoryLabelUid = tag1.CategoryLabelUid });
                _threeYearOutcome = setup.CreateBusinessOutcome(request1);

                var request2 = GetBusinessOutcomeRequest(SwimlaneType.StrategicTheme);
                request2.Tags.Add(new BusinessOutcomeTagRequest { Name = tag2.Name, Uid = tag2.Uid, CategoryLabelUid = tag2.CategoryLabelUid });
                _oneYearOutcome = setup.CreateBusinessOutcome(request2);

                var request3 = GetBusinessOutcomeRequest(SwimlaneType.StrategicTheme);
                request3.Tags.Add(new BusinessOutcomeTagRequest { Name = tag1.Name, Uid = tag1.Uid, CategoryLabelUid = tag1.CategoryLabelUid });
                request3.Tags.Add(new BusinessOutcomeTagRequest { Name = tag2.Name, Uid = tag2.Uid, CategoryLabelUid = tag2.CategoryLabelUid });
                _oneYearOutcome1 = setup.CreateBusinessOutcome(request3);

                var companyHierarchy = setup.GetCompanyHierarchy(Company.Id);

                _enterpriseTeam = companyHierarchy.GetTeamByName(SharedConstants.EnterpriseTeam);
                _multiTeam = companyHierarchy.GetTeamByName(SharedConstants.MultiTeam);
                _team = companyHierarchy.GetTeamByName(SharedConstants.Team);
            }
            catch (Exception e)
            {
                _classInitFailed = true;
                _classInitFailedMessage = e.ToLogString(e.StackTrace);
                throw;
            }
        }

        [TestMethod]
        [TestCategory("SiteAdmin")]
        public void BusinessOutcomes_Settings_Dashboard_DisplayCompanyLevelOnEveryLevel_On()
        {
            VerifySetup(_classInitFailed, _classInitFailedMessage);

            var login = new LoginPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var v2SettingsPage = new SettingsV2Page(Driver, Log);
            var boDashboardSettingsPage = new BusinessOutcomesDashboardSettingsPage(Driver, Log);
            var boDashboard = new BusinessOutcomesDashboardPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            manageFeaturesPage.NavigateToPage(Company.Id);
            manageFeaturesPage.TurnOnBusinessOutcomesDashboard();
            manageFeaturesPage.ClickUpdateButton();

            v2SettingsPage.NavigateToPage(Company.Id);
            v2SettingsPage.ClickOnBusinessOutcomesManageSettingsButton();
            boDashboardSettingsPage.ClickOnManageBusinessOutcomesDashboardSettingsButton();
            boDashboardSettingsPage.DisplayOnEveryLevel(true);

            // go to bo dashboard at company level
            boDashboard.NavigateToPage(Company.Id);
            boDashboard.WaitForReload();

            // verify cards displayed
            Assert.IsTrue(boDashboard.IsCardPresentInSwimLane(
                    _threeYearOutcome.SwimlaneType.GetDescription(), _threeYearOutcome.Title),
                $"<{_threeYearOutcome.Title}> not displayed at company level.");
            Assert.IsTrue(boDashboard.IsCardPresentInSwimLane(
                    _oneYearOutcome.SwimlaneType.GetDescription(), _oneYearOutcome.Title),
                $"<{_oneYearOutcome.Title}> not displayed at company level.");

            // go to bo dashboard at ET level
            boDashboard.NavigateToPage(Company.Id, null, _enterpriseTeam.TeamId);
            boDashboard.WaitForReload();

            Assert.IsTrue(boDashboard.IsCardPresentInSwimLane(
                    _threeYearOutcome.SwimlaneType.GetDescription(), _threeYearOutcome.Title),
                $"<{_threeYearOutcome.Title}> not displayed at ET level.");
            Assert.IsFalse(boDashboard.IsCardPresentInSwimLane(
                    _oneYearOutcome.SwimlaneType.GetDescription(), _oneYearOutcome.Title),
                $"<{_oneYearOutcome.Title}> not displayed at ET level.");

            // go to bo dashboard at MT level
            boDashboard.NavigateToPage(Company.Id, new List<int> { _enterpriseTeam.TeamId }, _multiTeam.TeamId);
            boDashboard.WaitForReload();

            Assert.IsTrue(boDashboard.IsCardPresentInSwimLane(
                    _threeYearOutcome.SwimlaneType.GetDescription(), _threeYearOutcome.Title),
                $"<{_threeYearOutcome.Title}> not displayed at MT level.");
            Assert.IsFalse(boDashboard.IsCardPresentInSwimLane(
                    _oneYearOutcome.SwimlaneType.GetDescription(), _oneYearOutcome.Title),
                $"<{_oneYearOutcome.Title}> not displayed at MT level.");

            // go to bo dashboard at team level
            boDashboard.NavigateToPage(Company.Id, new List<int> { _enterpriseTeam.TeamId, _multiTeam.TeamId }, _team.TeamId);
            boDashboard.WaitForReload();

            Assert.IsTrue(boDashboard.IsCardPresentInSwimLane(
                    _threeYearOutcome.SwimlaneType.GetDescription(), _threeYearOutcome.Title),
                $"<{_threeYearOutcome.Title}> not displayed at Team level.");
            Assert.IsFalse(boDashboard.IsCardPresentInSwimLane(
                    _oneYearOutcome.SwimlaneType.GetDescription(), _oneYearOutcome.Title),
                $"<{_oneYearOutcome.Title}> not displayed at Team level.");
        }

        [TestMethod]
        [TestCategory("SiteAdmin")]
        public void BusinessOutcomes_Settings_Dashboard_DisplayCompanyLevelOnEveryLevel_Off()
        {
            VerifySetup(_classInitFailed, _classInitFailedMessage);

            var login = new LoginPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var v2SettingsPage = new SettingsV2Page(Driver, Log);
            var boDashboardSettingsPage = new BusinessOutcomesDashboardSettingsPage(Driver, Log);
            var boDashboard = new BusinessOutcomesDashboardPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            manageFeaturesPage.NavigateToPage(Company.Id);
            manageFeaturesPage.TurnOnBusinessOutcomesDashboard();
            manageFeaturesPage.ClickUpdateButton();

            v2SettingsPage.NavigateToPage(Company.Id);
            v2SettingsPage.ClickOnBusinessOutcomesManageSettingsButton();
            boDashboardSettingsPage.ClickOnManageBusinessOutcomesDashboardSettingsButton();
            boDashboardSettingsPage.DisplayOnEveryLevel(false);

            // go to bo dashboard at company level
            boDashboard.NavigateToPage(Company.Id);
            boDashboard.WaitForReload();

            // verify cards displayed
            Assert.IsTrue(boDashboard.IsCardPresentInSwimLane(
                    _threeYearOutcome.SwimlaneType.GetDescription(), _threeYearOutcome.Title),
                $"<{_threeYearOutcome.Title}> not displayed at company level.");
            Assert.IsTrue(boDashboard.IsCardPresentInSwimLane(
                    _oneYearOutcome.SwimlaneType.GetDescription(), _oneYearOutcome.Title),
                $"<{_oneYearOutcome.Title}> not displayed at company level.");

            // go to bo dashboard at ET level
            boDashboard.NavigateToPage(Company.Id, null, _enterpriseTeam.TeamId);
            boDashboard.WaitForReload();

            Assert.IsFalse(boDashboard.IsCardPresentInSwimLane(
                    _threeYearOutcome.SwimlaneType.GetDescription(), _threeYearOutcome.Title),
                $"<{_threeYearOutcome.Title}> is displayed at ET level.");
            Assert.IsFalse(boDashboard.IsCardPresentInSwimLane(
                    _oneYearOutcome.SwimlaneType.GetDescription(), _oneYearOutcome.Title),
                $"<{_oneYearOutcome.Title}> is displayed at ET level.");

            // go to bo dashboard at MT level
            boDashboard.NavigateToPage(Company.Id, new List<int> { _enterpriseTeam.TeamId }, _multiTeam.TeamId);
            boDashboard.WaitForReload();

            Assert.IsFalse(boDashboard.IsCardPresentInSwimLane(
                    _threeYearOutcome.SwimlaneType.GetDescription(), _threeYearOutcome.Title),
                $"<{_threeYearOutcome.Title}> is displayed at MT level.");
            Assert.IsFalse(boDashboard.IsCardPresentInSwimLane(
                    _oneYearOutcome.SwimlaneType.GetDescription(), _oneYearOutcome.Title),
                $"<{_oneYearOutcome.Title}> is displayed at MT level.");

            // go to bo dashboard at team level
            boDashboard.NavigateToPage(Company.Id, new List<int> { _enterpriseTeam.TeamId, _multiTeam.TeamId }, _team.TeamId);
            boDashboard.WaitForReload();

            Assert.IsFalse(boDashboard.IsCardPresentInSwimLane(
                    _threeYearOutcome.SwimlaneType.GetDescription(), _threeYearOutcome.Title),
                $"<{_threeYearOutcome.Title}> is displayed at Team level.");
            Assert.IsFalse(boDashboard.IsCardPresentInSwimLane(
                    _oneYearOutcome.SwimlaneType.GetDescription(), _oneYearOutcome.Title),
                $"<{_oneYearOutcome.Title}> is displayed at Team level.");
        }

        [TestMethod]
        [TestCategory("SiteAdmin")]
        public void BusinessOutcomes_Settings_Dashboard_BusinessOutcomeFilter_OrAnd()
        {
            VerifySetup(_classInitFailed, _classInitFailedMessage);

            var login = new LoginPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var v2SettingsPage = new SettingsV2Page(Driver, Log);
            var boDashboardSettingsPage = new BusinessOutcomesDashboardSettingsPage(Driver, Log);
            var boDashboard = new BusinessOutcomesDashboardPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Navigate to manage features page and turn on business outcomes dashboard");
            manageFeaturesPage.NavigateToPage(Company.Id);
            manageFeaturesPage.TurnOnBusinessOutcomesDashboard();
            manageFeaturesPage.ClickUpdateButton();

            Log.Info("Navigate to manage business outcome dashboard setting and change Business Outcome filter to 'And' ");
            v2SettingsPage.NavigateToPage(Company.Id);
            v2SettingsPage.ClickOnBusinessOutcomesManageSettingsButton();
            boDashboardSettingsPage.ClickOnManageBusinessOutcomesDashboardSettingsButton();
            boDashboardSettingsPage.BusinessOutcomeFilterOrAndToggle();

            Log.Info("Go to BO dashboard, filter cards from the filter dropdown and verify cards");
            boDashboard.NavigateToPage(Company.Id);
            boDashboard.WaitForReload();

            boDashboard.SelectFilterTags(_oneYearOutcome1.Tags.Select(t => t.Name).ToList());

            Assert.IsTrue(boDashboard.IsBusinessOutcomePresent(_oneYearOutcome1.Title),
                $"Business outcome <{_oneYearOutcome1.Title}> is not present");
            Assert.IsFalse(boDashboard.IsBusinessOutcomePresent(_threeYearOutcome.Title),
                $"Business outcome <{_threeYearOutcome.Title}> is present");
            Assert.IsFalse(boDashboard.IsBusinessOutcomePresent(_oneYearOutcome.Title),
                $"Business outcome <{_oneYearOutcome.Title}> is present");

            Log.Info("Navigate to manage business outcome dashboard setting and change Business Outcome filter to 'Or' ");
            v2SettingsPage.NavigateToPage(Company.Id);
            v2SettingsPage.ClickOnBusinessOutcomesManageSettingsButton();
            boDashboardSettingsPage.ClickOnManageBusinessOutcomesDashboardSettingsButton();
            boDashboardSettingsPage.BusinessOutcomeFilterOrAndToggle(false);

            Log.Info("Go to BO dashboard and verify cards");
            boDashboard.NavigateToPage(Company.Id);
            boDashboard.WaitForReload();

            Assert.IsTrue(boDashboard.IsBusinessOutcomePresent(_threeYearOutcome.Title),
                $"Business outcome <{_threeYearOutcome.Title}> is not present");
            Assert.IsTrue(boDashboard.IsBusinessOutcomePresent(_oneYearOutcome.Title),
                $"Business outcome <{_oneYearOutcome.Title}> is not present");
            Assert.IsTrue(boDashboard.IsBusinessOutcomePresent(_oneYearOutcome1.Title),
                $"Business outcome <{_oneYearOutcome1.Title}> is not present");

        }
    }
}