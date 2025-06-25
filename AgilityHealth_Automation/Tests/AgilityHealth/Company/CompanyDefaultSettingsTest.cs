using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings;
using AtCommon.Api;
using AtCommon.Dtos.Companies;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.ManageFeatures;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Company
{
    [TestClass]
    [TestCategory("Companies")]
    public class CompanyDefaultSettingsTest : CompanyEditBaseTest
    {
        private static AddCompanyRequest _companyRequest;

        [ClassInitialize]
        public static void SetUp(TestContext _)
        {
            _companyRequest = CreateCompany();
        }

        [TestMethod]
        [TestCategory("KnownDefect")] // Bug : 48243
        [TestCategory("SiteAdmin")]
        public void Company_Create_Default_FeatureSettings_OnOff()
        {
            var login = new LoginPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);

            var v2Settings = new SettingsV2Page(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            v2Settings.NavigateToPage(Company.Id);
            v2Settings.SelectCompany(_companyRequest.Name);
            v2Settings.ClickOnManageFeatureButton();

            var onFeaturesByDefault = new Dictionary<string, string>
            {
                {"Individual Assessments", "individual-features"},
                {"Zendesk Integration", "zendesk-features"},
                {"MT/Enterprise Team", "team-features"},
                {"Growth Portal", "GrowthPortal-features"},
                {"Business Outcomes Dashboard", "business-outcomes-features"},
                {"Enable N-Tier Architecture", "ntier-features"},
                {"Highlight Stakeholder Responses", "StakeColor-features"},
                {"Facilitator Assessment", "AHFSurvey-features"},
                {"Benchmarking", "Benchmarking-features"},
                {"Integrations", "integration-features"},
                {"Assessment PIN Access", "allowAssessmentPin"},
                {"Team Member Log In", "teamMemberLogIn-settings"},
                {"Automatically Grant Team Access To Assigned Facilitators", "AutomaticallyGrantTeamAccessToAssignedFacilitators-features"}
            };

            var offFeaturesByDefault = new Dictionary<string, string>()
            {
                {"Metrics Features", "metrics-features"},
                {"Financial Features", "financial-settings"},
                {"Team Agility Dashboard", "team-agility"},
                {"Structural Agility Dashboard", "structural-agility"},
                {"My Dashboard", "yellowfin-my-dashboard"},
                {"Enterprise Agility Dashboard", "enterprise-agility"},
                {"Campaigns", "campaignsFeatures"},
                {"Notify Users Of Pending Assessments", "enableAssessmentReminderNotificationUi"},
                {"Assessment Settings", "survey-features"},
                {"Hide Assessment Status Icons", "hideassessmentstatusicons-feature"},
                {"Admin Dashboard", "myDashboard-features"},
                {"Enterprise Dashboard", "enterpriseDashboard-features"},
                {"Reports", "report-features"},
                {"Team vs Agile Coach Comparison", "TeamVsAgileCoach-features"},
                {"Assessment Scheduler", "AHFScheduler-features"},
                {"Assessment Checklist And Custom Maturity Model", "TeamMaturity-features"},
                {"Team Health Assessments Detailed Widget", "enableTeamHealthAssessmentsDetailedWidget"},
                {"Find A Facilitator", "AutoFacilitator-features"},
                {"Organizational Hierarchy", "OrganizationalHierarchy-features"},
                {"Assessment Reminder", "assessment-reminder-notification"},
                {"Enable Maturity Calculations", "EnableMaturityCalculations-features"},
                {"Pre-Populate Growth Item Description", "EnableGrowthItemButton-features"},
                {"Standard Deviation Model", "EnableStandardDeviationModel-features"},
                {"Allow Assessment Creation From AHFs Only", "EnableAssessmentCreationFromAHFOnly-features"},
                {"Strict External Identifier Configuration", "ExternalIdentifierConfig-features"},
                {"Disable User Impersonation", "DisableImpersonation-features"},
                {"Team Comments Report", "TeamCommentsReport-features"},
                {"Disable File Upload", "DisableFileUpload-features"},
                {"Disable Add From Directory", "DisableAddFromDirectory-features"},
                {"Enable External Links", "ExternalLinks-features"},
                {"Enable Bulk Data Management", "BulkOperations-features"},
                {"Enable Share Assessment Results", "ShareAssessmentResults-features"},
                {"Enable Hide Assessment Comments", "EnableHideAssessmentComments-features"},
                {"Allow users to create OAuth App Registrations for API access.", "EnableOAuth-features"},
                {"Growth Item Type field required", "Enable-GrowthItemTypeFieldRequired"},
                {"Enable External Identifier Duplicate Checking", "Enable-ExternalIdentifierDuplicateChecking"},
                {"Custom Growth Item Types", "Enable-CustomGrowthItemTypes"},
                {"Enable Language Selection", "EnableLanguageSelection-features"},
                {"Display Team AgilityHealth Index", "DisplayTeamAgilityHealthIndex-features"},
                {"Enable Pulse Assessments", "EnablePulse-features"},
                {"Allow Users To Create And Manage Private App Key", "EnableManagePrivateAppKey-features"},
                {"Enable Quick Launch For Team And Assessments", "EnableQuickLaunch-features"},
            };

            var newFeaturesByDefault = new Dictionary<string, string>
            {
                {"Growth Item Types", "NewGrowthItemTypes-features"}
            };

            var classicFeaturesByDefault = new Dictionary<string, string>
            {
                {"Growth Items Display", "growth-item-display"}
            };

            // Verifying default features one by one
            foreach (var featureOn in onFeaturesByDefault)
            {
                Assert.IsTrue(manageFeaturesPage.IsFeatureToggleButtonOn(featureOn.Value), $"{featureOn.Key} Feature Is Off ");
            }

            foreach (var featureOff in offFeaturesByDefault)
            {
                Assert.IsFalse(manageFeaturesPage.IsFeatureToggleButtonOn(featureOff.Value), $"{featureOff.Key} Feature Is On");
            }

            foreach (var featureNew in newFeaturesByDefault)
            {
                Assert.IsTrue(manageFeaturesPage.IsFeatureToggleButtonSetToNew(featureNew.Value), $"{featureNew.Key} Feature Is Set To Classic");
            }

            foreach (var featureClassic in classicFeaturesByDefault)
            {
                Assert.IsFalse(manageFeaturesPage.IsFeatureToggleButtonSetToNew(featureClassic.Value), $"{featureClassic.Key} Feature Is Set To New");
            }
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