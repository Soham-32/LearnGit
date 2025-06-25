using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.ManageFeatures;
using AtCommon.Api;
using AtCommon.Dtos.AhTrial;
using AtCommon.ObjectFactories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace AgilityHealth_Automation.Tests.AgilityHealth.AhTrial
{
    [TestClass]
    [TestCategory("AhTrial")]
    public class AhTrialCompanyDefaultFeaturesTests : BaseTest
    {
        private static bool _classInitFailed;
        private static AhTrialCompanyRequest _ahTrialCompanyRequest;
        private static AhTrialBaseCompanyResponse _ahTrialBaseCompanyResponse;

        [ClassInitialize]
        public static void SetUp(TestContext _)
        {
            try
            {
                _ahTrialCompanyRequest = AhTrialFactory.GetValidAhTrialCompany();

                var setup = new SetupTeardownApi(TestEnvironment);
                _ahTrialBaseCompanyResponse = setup.CreateAhTrialCompany(_ahTrialCompanyRequest);
            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }

        [TestMethod]
        [TestCategory("SiteAdmin")]
        [TestCategory("KnownDefect")] //Bug Id: 47507
        public void AhTrialCompany_Create_Default_Feature_OnOff()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var v2Settings = new SettingsV2Page(Driver, Log);

            Log.Info("Login as SA");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Navigate to 'Manage feature' page and verify all feature status");
            v2Settings.NavigateToPage(_ahTrialBaseCompanyResponse.Id);
            v2Settings.ClickOnManageFeatureButton();

            var onFeaturesByDefault = new Dictionary<string, string>
            {
                {"Team Agility Dashboard", "team-agility"},
                {"Notify Users Of Pending Assessments", "enableAssessmentReminderNotificationUi"},
                {"Zendesk Integration", "zendesk-features"},
                {"MT/Enterprise Team", "team-features"},
                {"Growth Portal", "GrowthPortal-features"},
                {"Business Outcomes Dashboard", "business-outcomes-features"},
                {"Highlight Stakeholder Responses", "StakeColor-features"},
                {"Benchmarking", "Benchmarking-features"},
                {"Assessment PIN Access", "allowAssessmentPin"},
                {"Team Member Log In", "teamMemberLogIn-settings"},
                {"Disable Add From Directory", "DisableAddFromDirectory-features"},
            };

            var offFeaturesByDefault = new Dictionary<string, string>()
            {
                {"Metrics Features", "metrics-features"},
                {"Financial Features", "financial-settings"},
                {"Structural Agility Dashboard", "structural-agility"},
                {"Enterprise Agility Dashboard", "enterprise-agility"},
                {"My Dashboard", "yellowfin-my-dashboard"},
                {"Campaigns", "campaignsFeatures"},
                {"Assessment Settings", "survey-features"},
                {"Individual Assessments", "individual-features"},
                {"Hide Assessment Status Icons", "hideassessmentstatusicons-feature"},
                {"Integrations", "integration-features"},
                {"Admin Dashboard", "myDashboard-features"},
                {"Enterprise Dashboard", "enterpriseDashboard-features"},
                {"Reports", "report-features"},
                {"Enable N-Tier Architecture", "ntier-features"},
                {"Team vs Agile Coach Comparison", "TeamVsAgileCoach-features"},
                {"Facilitator Assessment", "AHFSurvey-features"},
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
                {"Automatically Grant Team Access To Assigned Facilitators", "AutomaticallyGrantTeamAccessToAssignedFacilitators-features"},
                {"All Participants Can View Talent Dev Aggregate Results", "AllParticipantsCanViewTalentDevAggregateResults-features" }
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
    }

}
