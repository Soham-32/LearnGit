using System;
using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.Utilities;
using AtCommon.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.ManageFeatures
{
    public class ManageFeaturesPage : BasePage
    {
        public ManageFeaturesPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        public static bool IsPageBusy;

        //Update Button
        private readonly By UpdateSettingButton = By.CssSelector("button.update-financial-settings");
        private readonly By ClosePopupButton = By.Id("errorClose");

        //Toggles
        private readonly By OffFeatureToggles = By.CssSelector("div[class$='fl-lt switch-toggle'][style='background-color:red']");
        private readonly By OnFeatureToggles = By.CssSelector("div[class$='fl-lt switch-toggle']");
        public readonly string OnFeatureToggleBackgroundColor = "rgba(56, 179, 72, 1)";
        public readonly string OffFeatureToggleBackgroundColor = "rgba(255, 0, 0, 1)";
        public readonly string NewFeatureToggles = "handle ico ease ";

        //Feature specific toggles
        private readonly By FinancialFeatureOnOffToggle = By.Id("financial-settings");
        private readonly By TeamAgilityDashboardOnOffToggle = By.Id("team-agility");
        private readonly By StructuralAgilityDashboardOnOffToggle = By.Id("structural-agility");
        private readonly By EnterpriseAgilityDashboardOnOffToggle = By.Id("enterprise-agility");
        private readonly By FourLenzDashboardOnOffToggle = By.Id("fourlenz-features");
        private readonly By MyDashboardOnOffToggle = By.Id("yellowfin-my-dashboard");
        private readonly By TeamMemberLoginOnOffToggle = By.Id("teamMemberLogIn-settings");
        private readonly By TeamMemberLogInAllowAhfToOverWriteCheckbox = By.Id("TeamMemberLogInAllowAhfToOverWrite");
        private readonly By TeamMemberLogInAfterSubmitSurveyRadioButton = By.Id("TeamMemberLogInAfterSubmitSurveySpan");
        private readonly By TeamMemberLogInOnAssessmentEndDateRadioButton = By.Id("TeamMemberLogInOnAssessmentEndDateSpan");
        private readonly By MetricsFeatureOnOffToggle = By.Id("metrics-features");
        private readonly By IndividualAssessmentOnOffToggle = By.Id("individual-features");
        private readonly By AhfSchedulerOnOffToggle = By.Id("AHFScheduler-features");
        private readonly By TeamMaturityOnOffToggle = By.Id("TeamMaturity-features");
        private readonly By GrowthItemDisplayToggle = By.Id("growth-item-display");
        private readonly By GrowthItemClassicView = By.CssSelector("#growth-item-display a.on");
        private readonly By FacilitatorAssessmentOnOffToggle = By.Id("AHFSurvey-features");
        private readonly By BenchmarkingOnOffToggle = By.Id("Benchmarking-features");
        private const string BenchmarkingSubfeatureDivId = "benchmarking-children";
        private readonly By OrganizationalHierarchyOnOffToggle = By.Id("OrganizationalHierarchy-features");
        private readonly By MtEtTeamOnOffToggle = By.Id("team-features");
        private readonly By ZendeskIntegrationOnOffToggle = By.Id("zendesk-features");
        private readonly By GrowthPortalOnOffToggle = By.Id("GrowthPortal-features");
        private readonly By GrowthPortalDefaultContentOnOffToggle = By.Id("GrowthPortal-features-GrowthPortalDefaultMaster");
        private readonly By GrowthPortalCustomContentOnOffToggle = By.Id("GrowthPortal-features-GrowthPortalCustomContent");
        private readonly By AssessmentSettingsOnOffToggle = By.Id("survey-features");
        private readonly By SurveyPinAccessOnOffToggle = By.Id("allowAssessmentPin");
        private readonly By StakeColorOnOffToggle = By.Id("StakeColor-features");
        private readonly By TeamVsAgileCoachOnOffToggle = By.Id("TeamVsAgileCoach-features");
        private readonly By IntegrationsOnOffToggle = By.Id("integration-features");
        private const string GrowthPortalDefaultContentSubFeatureDivId = "GrowthPortal-master-Child";
        private const string GrowthPortalCustomContentSubFeatureDivId = "GrowthPortal-CustomContent-Child";
        private readonly By FindAFacilitatorOnOffToggle = By.Id("AutoFacilitator-features");
        private readonly By TeamHealthAssessmentWidgetOnOffToggle = By.Id("enableTeamHealthAssessmentsDetailedWidget");
        private readonly By NotifyUsersOfPendingAssessmentsOnOffToggle = By.Id("enableAssessmentReminderNotificationUi");
        private readonly By ReportsOnOffToggle = By.Id("report-features");
        private const string ReportsSubFeatureDivId = "reports-children";
        private readonly By AllowAssessmentFromAhFsOnlyOnOffToggle = By.Id("EnableAssessmentCreationFromAHFOnly-features");
        private readonly By StrictExternalIdentifierConfigurationOnOffToggle = By.Id("ExternalIdentifierConfig-features");
        private readonly By TeamCommentsReportOnOffToggle = By.Id("TeamCommentsReport-features");
        private const string TeamCommentsReportSubFeatureDivId = "myDashboard-children-TeamCommentsReport";
        private readonly By CampaignsOnOffToggle = By.Id("campaignsFeatures");
        private readonly By DisableAddFromDirectoryOnOffToggle = By.Id("DisableAddFromDirectory-features");
        private readonly By GrowthItemTypesToggle = By.Id("NewGrowthItemTypes-features");
        private readonly By HideAssessmentStatusIconsOnOffToggle = By.Id("hideassessmentstatusicons-feature");
        private readonly By NTierOnOffToggle = By.Id("ntier-features");
        private readonly By BusinessOutcomeOnOffToggle = By.Id("business-outcomes-features");
        private readonly By EnableBulkDataManagementOnOffToggle = By.Id("BulkOperations-features");
        private readonly By ShareAssessmentResultOnOffToggle = By.Id("ShareAssessmentResults-features");
        private readonly By EnableHideAssessmentCommentsOnOffToggle = By.Id("EnableHideAssessmentComments-features");
        private readonly By GrowthItemTypeFieldRequiredOnOffToggle = By.Id("Enable-GrowthItemTypeFieldRequired");
        private readonly By EnableOauthOnOffToggle = By.Id("EnableOAuth-features");
        private readonly By CustomGrowthItemTypesOnOffToggle = By.Id("Enable-CustomGrowthItemTypes");
        private readonly By LanguageSelectionOnOffToggle = By.Id("EnableLanguageSelection-features");
        private readonly By DisplayTeamAgilityHealthIndexOnOffToggle = By.Id("DisplayTeamAgilityHealthIndex-features");
        private readonly By AllowUsersToCreateAndManagePrivateAppKeyOnOffToggle = By.Id("EnableManagePrivateAppKey-features");
        private readonly By EnableAiInsightsOnOffToggle = By.Id("EnableAIInsights-features");
        private readonly By EnablePulseAssessmentsOnOffToggle = By.Id("EnablePulse-features");
        private readonly By DisableFileUploadOnOffToggle = By.Id("DisableFileUpload-features");
        private readonly By EnableExternalLinksOnOffToggle = By.Id("ExternalLinks-features");
        private readonly By DisableUserImpersonationOnOffToggle = By.Id("DisableImpersonation-features");
        private readonly By StandardDeviationModelOnOffToggle = By.Id("EnableStandardDeviationModel-features");
        private readonly By EnableExternalIdentifierDuplicateCheckingOnOffToggle = By.Id("Enable-ExternalIdentifierDuplicateChecking");
        private readonly By EnableMaturityCalculationsOnOffToggle = By.Id("EnableMaturityCalculations-features");
        private readonly By EnablePrePopulateGrowthItemDescriptionOnOffToggle = By.Id("EnableGrowthItemButton-features");
        private readonly By EnableAllParticipantsCanViewTalentDevAggregateResultsOnOffToggle = By.Id("AllParticipantsCanViewTalentDevAggregateResults-features");
        private readonly By EnableQuickLaunchForTeamAndAssessmentsOnOffToggle = By.Id("EnableQuickLaunch-features");

        //Common
        private static By DynamicLocatorForTurningOnSubFeature(string subFeatureName) => By.XPath($"//span[text() = '{subFeatureName}']//following-sibling::span[1]/input");
        private static By DynamicLocatorForTurningOffSubFeature(string subFeatureName) => By.XPath($"//span[text() = '{subFeatureName}']//following-sibling::span[2]/input");
        private static By DynamicLocatorForTurningOnSubFeature(string subFeatureName, string subFeatureDivId) => By.XPath($"//div[@id='{subFeatureDivId}']//span[normalize-space() = '{subFeatureName}']//following-sibling::span[1]/input");
        private static By DynamicLocatorForTurningOffSubFeature(string subFeatureName, string subFeatureDivId) => By.XPath($"//div[@id='{subFeatureDivId}']//span[normalize-space() = '{subFeatureName}']//following-sibling::span[2]/input");


        // Verify Feature Toggle Button On
        public bool IsFeatureToggleButtonOn(string featureId)
        {
            Log.Step(nameof(ManageFeaturesPage), $"Verify that feature {featureId} toggle is On or not");
            var featureToggle = By.Id($"{featureId}");
            return Driver.JavaScriptScrollToElement(Wait.UntilElementVisible(featureToggle)).GetCssValue("background-color").Equals(OnFeatureToggleBackgroundColor);
        }

        //Verify feature toggle button is set to New
        public bool IsFeatureToggleButtonSetToNew(string featureId)
        {
            Log.Step(nameof(ManageFeaturesPage), $"Verify that feature {featureId} toggle is set to New or not");
            var featureToggle = By.XPath($"//div[@id='{featureId}']/a");
            return Driver.JavaScriptScrollToElement(Wait.UntilElementVisible(featureToggle)).GetAttribute("class").Equals(NewFeatureToggles);
        }


        //Update button
        public void ClickUpdateButton()
        {
            Log.Step(nameof(ManageFeaturesPage), "Click on Update button");
            Wait.UntilElementClickable(UpdateSettingButton).Click();
            Wait.HardWait(4000);
            Wait.UntilJavaScriptReady();
            Wait.UntilElementClickable(ClosePopupButton).Click();
            Wait.HardWait(2000);
            Wait.UntilElementInvisible(ClosePopupButton);
            Wait.UntilJavaScriptReady();
            IsPageBusy = false;
        }

        //Toggles
        public void TurnOnAllFeatures()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn on all features");
            var attempts = 0;
            var elements = Wait.InCases(OffFeatureToggles);
            while (elements.Count > 0 && attempts < 100)
            {
                elements[0].Click();
                Wait.UntilJavaScriptReady();
                elements = Wait.InCases(OffFeatureToggles);
                attempts++;
            }

            if (attempts >= 100) throw new Exception($"Failed to turn on all features. Tried {attempts} times.");
        }

        public void TurnOffAllFeatures()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn off all features");
            var attempts = 0;
            var elements = Wait.UntilAllElementsLocated(OnFeatureToggles)
                .Where(element => element.Displayed && element.GetCssValue("background-color")
                                      .Equals(OnFeatureToggleBackgroundColor)).ToList();
            while (elements.Count > 0)
            {
                elements[0].Click();
                Wait.UntilJavaScriptReady();
                elements = Wait.UntilAllElementsLocated(OnFeatureToggles)
                    .Where(element => element.Displayed && element.GetCssValue("background-color")
                                          .Equals(OnFeatureToggleBackgroundColor)).ToList();
                attempts++;
            }

            if (attempts >= 100) throw new Exception($"Failed to turn off all features. Tried {attempts} times.");
        }
        public bool AreAllFeaturesTurnedOn() => Driver.GetElementCount(OffFeatureToggles) == 0;
        public bool AreAllFeaturesTurnedOff() =>
            Wait.UntilAllElementsLocated(OnFeatureToggles).Where(e => e.Displayed && e.GetCssValue("background-color")
                                                                          .Equals(OnFeatureToggleBackgroundColor)).ToList().Count == 0;
        public Dictionary<string, string> GetToggleIdsAndStatuses()
        {
            return Wait.UntilAllElementsLocated(OnFeatureToggles).Where(e => e.Displayed)
                .ToDictionary(e => e.GetAttribute("id"), e => e.GetCssValue("background-color"));
        }

        //Common
        public void TurnOnSubFeature(string subFeatureName)
        {
            Log.Step(nameof(ManageFeaturesPage), $"Turn on sub feature <{subFeatureName}>");
            Wait.UntilElementClickable(DynamicLocatorForTurningOnSubFeature(subFeatureName)).Click();
        }
        public void TurnOffSubFeature(string subFeatureName)
        {
            Log.Step(nameof(ManageFeaturesPage), $"Turn off sub feature <{subFeatureName}>");
            Wait.UntilElementClickable(DynamicLocatorForTurningOffSubFeature(subFeatureName)).Click();
        }
        public void TurnOnSubFeature(string subFeatureName, string subFeatureDivId)
        {
            Wait.UntilElementClickable(DynamicLocatorForTurningOnSubFeature(subFeatureName, subFeatureDivId)).Click();
        }
        public void TurnOffSubFeature(string subFeatureName, string subFeatureDivId) =>
            Wait.UntilElementClickable(DynamicLocatorForTurningOffSubFeature(subFeatureName, subFeatureDivId)).Click();
        private void TurnOnFeatureToggle(By locator)
        {
            if (!Wait.UntilElementVisible(locator).GetAttribute("style").Contains("red")) return;
            Wait.UntilElementClickable(locator).Click();
            Wait.UntilJavaScriptReady();
        }
        private void TurnOffFeatureToggle(By locator)
        {
            if (!Wait.UntilElementVisible(locator).GetCssValue("background-color")
                .Equals(OnFeatureToggleBackgroundColor)) return;
            Wait.UntilElementClickable(locator).Click();
            Wait.UntilJavaScriptReady();
        }


        //Features 
        //Metrics Feature
        public void TurnOnMetricsFeature()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn On 'Metrics Features'");
            TurnOnFeatureToggle(MetricsFeatureOnOffToggle);
        }
        public void TurnOffMetricsFeature()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn Off 'Metrics Features'");
            TurnOffFeatureToggle(MetricsFeatureOnOffToggle);
        }

        //Financial Feature
        public void TurnOnFinancialFeature()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn On 'Financial Features'");
            TurnOnFeatureToggle(FinancialFeatureOnOffToggle);
        }
        public void TurnOffFinancialFeature()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn Off 'Financial Features'");
            TurnOffFeatureToggle(FinancialFeatureOnOffToggle);
        }

        //Team Agility Dashboard
        public void TurnOnTeamAgilityDashboard()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn On 'Team Agility Dashboard' feature");
            TurnOnFeatureToggle(TeamAgilityDashboardOnOffToggle);
        }
        public void TurnOffTeamAgilityDashboard()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn Off 'Team Agility Dashboard' feature");
            TurnOffFeatureToggle(TeamAgilityDashboardOnOffToggle);
        }

        //Structural Agility Dashboard
        public void TurnOnStructuralAgilityDashboard()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn On 'Structural Agility Dashboard' feature");
            TurnOnFeatureToggle(StructuralAgilityDashboardOnOffToggle);
        }
        public void TurnOffStructuralAgilityDashboard()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn Off 'Structural Agility Dashboard' feature");
            TurnOffFeatureToggle(StructuralAgilityDashboardOnOffToggle);
        }

        //Enterprise Agility Dashboard
        public void TurnOnEnterpriseAgilityDashboard()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn On 'Enterprise Agility Dashboard' feature");
            TurnOnFeatureToggle(EnterpriseAgilityDashboardOnOffToggle);
        }
        public void TurnOffEnterpriseAgilityDashboard()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn Off 'Enterprise Agility Dashboard' feature");
            TurnOffFeatureToggle(EnterpriseAgilityDashboardOnOffToggle);
        }

        //4 LENZ Dashboard
        public void TurnOnFourLenzDashboard()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn On '4 LENZ Dashboard' feature");
            TurnOnFeatureToggle(FourLenzDashboardOnOffToggle);
        }

        public void TurnOffFourLenzDashboard()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn Off '4 LENZ Dashboard' feature");
            TurnOffFeatureToggle(FourLenzDashboardOnOffToggle);
        }

        //My Dashboard
        public void TurnOnMyDashboard()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn On 'My Dashboard' feature");
            TurnOnFeatureToggle(MyDashboardOnOffToggle);
        }
        public void TurnOffMyDashboard()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn Off 'My Dashboard' feature");
            TurnOffFeatureToggle(MyDashboardOnOffToggle);
        }

        //Campaigns
        public void TurnOnCampaigns()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn On 'Campaigns' feature");
            TurnOnFeatureToggle(CampaignsOnOffToggle);
        }
        public void TurnOffCampaigns()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn Off 'Campaigns' feature");
            TurnOffFeatureToggle(CampaignsOnOffToggle);
        }

        // Notify Users Of Pending Assessments
        public void TurnOnNotifyUsersOfPendingAssessments()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn On 'Notify Users Of Pending Assessments' feature");
            TurnOnFeatureToggle(NotifyUsersOfPendingAssessmentsOnOffToggle);
        }
        public void TurnOffNotifyUsersOfPendingAssessments()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn Off 'Notify Users Of Pending Assessments' feature");
            TurnOffFeatureToggle(NotifyUsersOfPendingAssessmentsOnOffToggle);
        }

        //Assessment Settings
        public void TurnOnAssessmentSettings()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn On 'Assessment Settings' feature");
            TurnOnFeatureToggle(AssessmentSettingsOnOffToggle);
        }
        public void TurnOffAssessmentSettings()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn Off 'Assessment Settings' feature");
            TurnOffFeatureToggle(AssessmentSettingsOnOffToggle);
        }

        // Individual Assessment Feature
        public void TurnOnIndividualAssessments()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn On 'Individual Assessment' features");
            TurnOnFeatureToggle(IndividualAssessmentOnOffToggle);
        }
        public void TurnOffIndividualAssessments()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn Off 'Individual Assessment' features");
            TurnOffFeatureToggle(IndividualAssessmentOnOffToggle);
        }

        //Hide Assessment Status Icons
        public void TurnOnHideAssessmentStatusIcons()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn On 'Hide Assessment Status Icons' feature");
            TurnOnFeatureToggle(HideAssessmentStatusIconsOnOffToggle);
        }
        public void TurnOffHideAssessmentStatusIcons()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn Off 'Hide Assessment Status Icons' feature");
            TurnOffFeatureToggle(HideAssessmentStatusIconsOnOffToggle);
        }

        //Integration
        public void TurnOnIntegrations()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn On 'Integration' feature");
            TurnOnFeatureToggle(IntegrationsOnOffToggle);
        }
        public void TurnOffIntegrations()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn Off 'Integration' feature");
            TurnOffFeatureToggle(IntegrationsOnOffToggle);
        }

        //Zendesk Integration
        public void TurnOnZendeskIntegrationFeature()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn On 'Zendesk Integration' feature");
            TurnOnFeatureToggle(ZendeskIntegrationOnOffToggle);
        }
        public void TurnOffZendeskIntegrationFeature()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn Off 'Zendesk Integration' feature");
            TurnOffFeatureToggle(ZendeskIntegrationOnOffToggle);
        }

        //Reports
        public void TurnOnReportsFeature()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn On 'Report' feature");
            TurnOnFeatureToggle(ReportsOnOffToggle);
        }
        public void TurnOffReportsFeature()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn Off 'Report' feature");
            TurnOffFeatureToggle(ReportsOnOffToggle);
        }
        public void Reports_TurnOnSubFeature(string featureName)
        {
            Log.Step(nameof(ManageFeaturesPage), $"Turn On 'Report' sub feature <{featureName}>");
            TurnOnSubFeature(featureName, ReportsSubFeatureDivId);
        }

        //MT/Enterprise Team 
        public void TurnOnMtEtTeamFeature()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn On 'MT/Enterprise Team' feature");
            TurnOnFeatureToggle(MtEtTeamOnOffToggle);
        }
        public void TurnOffMtEtTeamFeature()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn Off 'MT/Enterprise Team' feature");
            TurnOffFeatureToggle(MtEtTeamOnOffToggle);
        }

        //Growth Portal
        public void TurnOnGrowthPortal()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn On 'Growth Portal' feature");
            TurnOnFeatureToggle(GrowthPortalOnOffToggle);
        }
        public void TurnOffGrowthPortal()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn Off 'Growth Portal' feature");
            TurnOffFeatureToggle(GrowthPortalOnOffToggle);
        }
        public void TurnOnGrowthPortalDefaultContent()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn On 'Growth Portal' sub feature 'Default Content' feature");
            TurnOnFeatureToggle(GrowthPortalDefaultContentOnOffToggle);
        }
        public void TurnOffGrowthPortalDefaultContent()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn Off 'Growth Portal' sub feature 'Default Content's feature");
            TurnOffFeatureToggle(GrowthPortalDefaultContentOnOffToggle);
        }
        public void TurnOnDefaultContentSubFeature(string subFeatureName)
        {
            Log.Step(nameof(ManageFeaturesPage), $"Turn On 'Default Content' sub feature <{subFeatureName}>");
            TurnOnSubFeature(subFeatureName, GrowthPortalDefaultContentSubFeatureDivId);
        }
        public void TurnOffDefaultContentSubFeature(string subFeatureName)
        {
            Log.Step(nameof(ManageFeaturesPage), $"Turn Off 'Default Content' sub feature <{subFeatureName}>");
            TurnOffSubFeature(subFeatureName, GrowthPortalDefaultContentSubFeatureDivId);
        }
        public void TurnOnGrowthPortalCustomContent()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn On 'Growth Portal' sub feature 'Custom Content' feature");
            TurnOnFeatureToggle(GrowthPortalCustomContentOnOffToggle);
        }
        public void TurnOffGrowthPortalCustomContent()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn Off 'Growth Portal' sub feature 'Custom Content' feature");
            TurnOffFeatureToggle(GrowthPortalCustomContentOnOffToggle);
        }
        public void TurnOnCustomContentSubFeature(string subFeatureName)
        {
            Log.Step(nameof(ManageFeaturesPage), $"Turn On 'Custom Content' sub feature <{subFeatureName}>");
            TurnOnSubFeature(subFeatureName, GrowthPortalCustomContentSubFeatureDivId);
        }
        public void TurnOffCustomContentSubFeature(string subFeatureName)
        {
            Log.Step(nameof(ManageFeaturesPage), $"Turn Off 'Custom Content' sub feature <{subFeatureName}>");
            TurnOffSubFeature(subFeatureName, GrowthPortalCustomContentSubFeatureDivId);
        }

        //Business Outcomes Dashboard
        public void TurnOnBusinessOutcomesDashboard()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn on 'Business Outcomes Dashboard' feature");
            TurnOnFeatureToggle(BusinessOutcomeOnOffToggle);
        }
        public void TurnOffBusinessOutcomesDashboard()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn off 'Business Outcomes Dashboard' feature");
            TurnOffFeatureToggle(BusinessOutcomeOnOffToggle);
        }

        //Enable N-Tier Architecture
        public void TurnOnNTier()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn On 'Enable N-Tier Architecture' feature");
            TurnOnFeatureToggle(NTierOnOffToggle);
        }
        public void TurnOffNTier()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn Off 'Enable N-Tier Architecture' feature");
            TurnOffFeatureToggle(NTierOnOffToggle);
        }

        //Growth Items Display
        public void TurnGiDisplayToNew()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn New 'Growth Items Display' feature");
            if (Wait.InCase(GrowthItemClassicView) != null)
            {
                Wait.UntilElementVisible(GrowthItemDisplayToggle).Click();
            }
        }
        public void TurnGiDisplayToClassic()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn Classic 'Growth Items Display' feature");
            if (Wait.InCase(GrowthItemClassicView) == null)
            {
                Wait.UntilElementVisible(GrowthItemDisplayToggle).Click();
            }
        }

        //Highlight Stakeholder Responses
        public void TurnOnHighlightStakeholderResponses()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn On 'Highlight Stakeholder Responses' feature");
            TurnOnFeatureToggle(StakeColorOnOffToggle);
        }
        public void TurnOffHighlightStakeholderResponses()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn Off 'Highlight Stakeholder Responses' feature");
            TurnOffFeatureToggle(StakeColorOnOffToggle);
        }

        //Team vs Agile Coach Comparison 
        public void TurnOnTeamVsAgileCoach()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn On 'Team vs Agile Coach Comparison' feature");
            TurnOnFeatureToggle(TeamVsAgileCoachOnOffToggle);
        }
        public void TurnOffTeamVsAgileCoach()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn Off 'Team vs Agile Coach Comparison' feature");
            TurnOffFeatureToggle(TeamVsAgileCoachOnOffToggle);
        }

        //Facilitator Assessment
        public void TurnOnFacilitatorAssessment()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn On 'Facilitator Assessment' feature");
            TurnOnFeatureToggle(FacilitatorAssessmentOnOffToggle);
        }
        public void TurnOffFacilitatorAssessment()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn Off 'Facilitator Assessment' feature");
            TurnOffFeatureToggle(FacilitatorAssessmentOnOffToggle);
        }

        //Assessment Scheduler --
        public void TurnOnAssessmentScheduler()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn On 'Assessment Scheduler' feature");
            TurnOnFeatureToggle(AhfSchedulerOnOffToggle);
        }
        public void TurnOffAssessmentScheduler()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn Off 'Assessment Scheduler' feature");
            TurnOffFeatureToggle(AhfSchedulerOnOffToggle);
        }

        //Assessment Checklist And Custom Maturity Model --
        public void TurnOnTeamMaturity()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn On 'Assessment Checklist And Custom Maturity Model' feature");
            TurnOnFeatureToggle(TeamMaturityOnOffToggle);
        }
        public void TurnOffTeamMaturity()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn Off 'Assessment Checklist And Custom Maturity Model' feature");
            TurnOffFeatureToggle(TeamMaturityOnOffToggle);
        }

        //Benchmarking Feature
        public void TurnOnBenchmarking()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn On 'Benchmarking' feature");
            TurnOnFeatureToggle(BenchmarkingOnOffToggle);
        }
        public void TurnOffBenchmarking()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn Off 'Benchmarking' feature");
            TurnOffFeatureToggle(BenchmarkingOnOffToggle);
        }
        public void TurnOnBenchmarkingSubFeature(string subFeatureName)
        {
            Log.Step(nameof(ManageFeaturesPage), $"Turn On 'Benchmarking' sub feature <{subFeatureName}>");
            TurnOnSubFeature(subFeatureName, BenchmarkingSubfeatureDivId);
        }
        public void TurnOffBenchmarkingSubFeature(string subFeatureName)
        {
            Log.Step(nameof(ManageFeaturesPage), $"Turn Off 'Benchmarking' sub feature <{subFeatureName}>");
            TurnOffSubFeature(subFeatureName, BenchmarkingSubfeatureDivId);
        }

        //Assessment PIN Access --
        public void TurnOnSurveyPinAccess()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn On 'Assessment PIN Access' feature");
            TurnOnFeatureToggle(SurveyPinAccessOnOffToggle);
        }
        public void TurnOffSurveyPinAccess()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn Off 'Assessment PIN Access' feature");
            TurnOffFeatureToggle(SurveyPinAccessOnOffToggle);
        }

        // Team Health Assessment Detail Widget
        public void TurnOnTeamHealthAssessmentWidget()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn On 'Team Health Assessment Details Widget' feature");
            TurnOnFeatureToggle(TeamHealthAssessmentWidgetOnOffToggle);
        }
        public void TurnOffTeamHealthAssessmentWidget()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn Off 'Team Health Assessment Details Widget' feature");
            TurnOffFeatureToggle(TeamHealthAssessmentWidgetOnOffToggle);
        }

        //Team Member Login
        public void TurnOnTeamMemberLogin()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn On 'Team Member Log In' feature");
            TurnOnFeatureToggle(TeamMemberLoginOnOffToggle);
        }
        public void TurnOffTeamMemberLogin()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn Off 'Team Member Log In' feature");
            TurnOffFeatureToggle(TeamMemberLoginOnOffToggle);
        }
        public void TurnOnSubFeatureTeamMemberLogInAfterSubmitSurvey()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn On sub feature 'When the participants submit the assessment'");
            Wait.UntilElementVisible(TeamMemberLogInAfterSubmitSurveyRadioButton).Click();
        }
        public void TurnOnSubFeatureTeamMemberLogInOnAssessmentEndDate()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn On sub feature 'When the assessment end date has been reached'");
            Wait.UntilElementVisible(TeamMemberLogInOnAssessmentEndDateRadioButton).Click();
        }
        public void TurnOnOffSubFeatureTeamMemberLogInAllowAhfToOverWriteCheckbox(bool check = true)
        {
            Log.Step(nameof(ManageFeaturesPage), "Check sub feature 'Allow AHFs to overwrite company settings when launching assessments'");
            Wait.UntilElementVisible(TeamMemberLogInAllowAhfToOverWriteCheckbox).Check(check);
        }

        // Find A Facilitator
        public void TurnOnFindAFacilitator()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn On 'Find A Facilitator' feature");
            TurnOnFeatureToggle(FindAFacilitatorOnOffToggle);
        }
        public void TurnOffFindAFacilitator()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn Off 'Find A Facilitator' feature");
            TurnOffFeatureToggle(FindAFacilitatorOnOffToggle);
        }

        //Organizational Hierarchy
        public void TurnOnOrganizationalHierarchy()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn On 'Organizational Hierarchy' feature");
            TurnOnFeatureToggle(OrganizationalHierarchyOnOffToggle);
        }
        public void TurnOffOrganizationalHierarchy()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn Off 'Organizational Hierarchy' feature");
            TurnOffFeatureToggle(OrganizationalHierarchyOnOffToggle);
        }

        //Enable Maturity Calculations
        public void TurnOnEnableMaturityCalculations()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn On 'Enable Maturity Calculations' feature");
            TurnOnFeatureToggle(EnableMaturityCalculationsOnOffToggle);
        }
        public void TurnOffEnableMaturityCalculations()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn Off 'Enable Maturity Calculations' feature");
            TurnOffFeatureToggle(EnableMaturityCalculationsOnOffToggle);
        }

        //Pre-Populate Growth Item Description
        public void TurnOnPrePopulateGrowthItemDescription()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn On 'Pre-Populate Growth Item Description' feature");
            TurnOnFeatureToggle(EnablePrePopulateGrowthItemDescriptionOnOffToggle);
        }
        public void TurnOffPrePopulateGrowthItemDescription()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn Off 'Pre-Populate Growth Item Description' feature");
            TurnOffFeatureToggle(EnablePrePopulateGrowthItemDescriptionOnOffToggle);
        }

        //Standard Deviation Model
        public void TurnOnStandardDeviationModel()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn On 'Standard Deviation Model' feature");
            TurnOnFeatureToggle(StandardDeviationModelOnOffToggle);
        }
        public void TurnOffStandardDeviationModel()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn Off 'Standard Deviation Model' feature");
            TurnOffFeatureToggle(StandardDeviationModelOnOffToggle);
        }

        //Allow Assessment Creation From AHFs Only
        public void TurnOnAllowAssessmentCreationFromAhFs()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn On 'Allow Assessment Creation From AHFs Only' feature");
            TurnOnFeatureToggle(AllowAssessmentFromAhFsOnlyOnOffToggle);
        }
        public void TurnOffAllowAssessmentCreationFromAhFs()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn Off 'Allow Assessment Creation From AHFs Only' feature");
            TurnOffFeatureToggle(AllowAssessmentFromAhFsOnlyOnOffToggle);
        }

        //Strict External Identifier Configuration
        public void TurnOnStrictExternalIdentifier()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn On 'Strict External Identifier Configuration' feature");
            TurnOnFeatureToggle(StrictExternalIdentifierConfigurationOnOffToggle);
        }
        public void TurnOffStrictExternalIdentifier()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn Off 'Strict External Identifier Configuration' feature");
            TurnOffFeatureToggle(StrictExternalIdentifierConfigurationOnOffToggle);
        }

        //Disable User Impersonation
        public void TurnOnDisableUserImpersonation()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn On 'Disable User Impersonation' feature");
            TurnOnFeatureToggle(DisableUserImpersonationOnOffToggle);
        }
        public void TurnOffDisableUserImpersonation()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn Off 'Disable User Impersonation' feature");
            TurnOffFeatureToggle(DisableUserImpersonationOnOffToggle);
        }

        // Team Comments Report
        public void TurnOnTeamCommentsReport()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn On 'Team Comments Report' feature");
            TurnOnFeatureToggle(TeamCommentsReportOnOffToggle);
        }
        public void TurnOffTeamCommentsReport()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn Off 'Team Comments Report' feature");
            TurnOffFeatureToggle(TeamCommentsReportOnOffToggle);
        }
        public void TeamCommentsReport_TurnOnSubFeature(string subFeatureName)
        {
            Log.Step(nameof(ManageFeaturesPage), $"Turn On 'Team Comments Report' sub feature <{subFeatureName}>");
            TurnOnSubFeature(subFeatureName, TeamCommentsReportSubFeatureDivId);
        }
        public void TeamCommentsReport_TurnOffSubFeature(string subFeatureName)
        {
            Log.Step(nameof(ManageFeaturesPage), $"Turn Off 'Team Comments Report' sub feature <{subFeatureName}>");
            TurnOffSubFeature(subFeatureName, TeamCommentsReportSubFeatureDivId);
        }

        //Disable File Upload
        public void TurnOnDisableFileUpload()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn On 'Disable File Upload' feature");
            TurnOnFeatureToggle(DisableFileUploadOnOffToggle);
        }
        public void TurnOffDisableFileUpload()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn Off 'Disable File Upload' feature");
            TurnOffFeatureToggle(DisableFileUploadOnOffToggle);
        }

        //Disable Add From Directory
        public void TurnOnDisableAddFromDirectory()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn On 'Disable Add From Directory' feature");
            TurnOnFeatureToggle(DisableAddFromDirectoryOnOffToggle);
        }
        public void TurnOffDisableAddFromDirectory()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn Off 'Disable Add From Directory' feature");
            TurnOffFeatureToggle(DisableAddFromDirectoryOnOffToggle);
        }

        // Growth Item Types --GrowthItemTypesToggle [new Classis]  GrowthItemTypesNewClassisToggle
        public void TurnOnNewGrowthItemTypes()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn New 'Growth Item Types' feature");
            TurnOnFeatureToggle(GrowthItemTypesToggle);
        }
        public void TurnOffNewGrowthItemTypes()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn New 'Growth Item Types' feature");
            TurnOffFeatureToggle(GrowthItemTypesToggle);
        }

        //Enable External Links
        public void TurnOnEnableExternalLinks()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn On 'Enable External Links' feature");
            TurnOnFeatureToggle(EnableExternalLinksOnOffToggle);
        }
        public void TurnOffEnableExternalLinks()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn Off 'Enable External Links' feature");
            TurnOffFeatureToggle(EnableExternalLinksOnOffToggle);
        }

        //Enable Bulk Data Management
        public void TurnOnEnableBulkDataManagement()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn On 'Enable Bulk Data Management' feature");
            TurnOnFeatureToggle(EnableBulkDataManagementOnOffToggle);
        }
        public void TurnOffEnableBulkDataManagement()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn Off 'Enable Bulk Data Management' feature");
            TurnOffFeatureToggle(EnableBulkDataManagementOnOffToggle);
        }

        //Enable Share Assessment Results
        public void TurnOnEnableShareAssessmentResult()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn On 'Enable Share Assessment Results' feature");
            TurnOnFeatureToggle(ShareAssessmentResultOnOffToggle);
        }
        public void TurnOffEnableShareAssessmentResult()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn Off 'Enable Share Assessment Results' feature");
            TurnOffFeatureToggle(ShareAssessmentResultOnOffToggle);
        }

        //Enable Hide Assessment Comments
        public void TurnOnEnableHideAssessmentComments()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn On 'Enable Hide Assessment Comments' feature");
            TurnOnFeatureToggle(EnableHideAssessmentCommentsOnOffToggle);
        }
        public void TurnOffEnableHideAssessmentComments()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn Off 'Enable Hide Assessment Comments' feature");
            TurnOffFeatureToggle(EnableHideAssessmentCommentsOnOffToggle);
        }

        //Allow Users To Create OAuth App Registrations For API Access.
        public void TurnOnOauth()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn On 'Allow Users To Create OAuth App Registrations For API Access.' feature");
            TurnOnFeatureToggle(EnableOauthOnOffToggle);
        }
        public void TurnOffOauth()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn Off 'Allow Users To Create OAuth App Registrations For API Access.' feature");
            TurnOffFeatureToggle(EnableOauthOnOffToggle);
        }

        // Growth Item Type Field Required
        public void TurnOnGrowthItemTypeFieldRequired()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn On 'Growth Item Type Field Required' feature");
            TurnOnFeatureToggle(GrowthItemTypeFieldRequiredOnOffToggle);
        }
        public void TurnOffGrowthItemTypeFieldRequired()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn Off 'Growth Item Type Field Required' feature");
            TurnOffFeatureToggle(GrowthItemTypeFieldRequiredOnOffToggle);
        }

        //Enable External Identifier Duplicate Checking
        public void TurnOnEnableExternalIdentifierDuplicateChecking()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn On 'Enable External Identifier Duplicate Checking' feature");
            TurnOnFeatureToggle(EnableExternalIdentifierDuplicateCheckingOnOffToggle);
        }
        public void TurnOffEnableExternalIdentifierDuplicateChecking()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn Off 'Enable External Identifier Duplicate Checking' feature");
            TurnOffFeatureToggle(EnableExternalIdentifierDuplicateCheckingOnOffToggle);
        }

        //Custom Growth Item Types
        public void TurnOnCustomGrowthItemsTypes()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn On 'Custom Growth Item Types' feature");
            TurnOnFeatureToggle(CustomGrowthItemTypesOnOffToggle);
        }
        public void TurnOffCustomGrowthItemsTypes()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn Off 'Custom Growth Item Types' feature");
            TurnOffFeatureToggle(CustomGrowthItemTypesOnOffToggle);
        }

        //Enable Language Selection
        public void TurnOnEnableLanguageSelection()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn On 'Enable Language Selection' feature");
            TurnOnFeatureToggle(LanguageSelectionOnOffToggle);
        }
        public void TurnOffEnableLanguageSelection()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn Off 'Enable Language Selection' feature");
            TurnOffFeatureToggle(LanguageSelectionOnOffToggle);
        }

        //Display Team AgilityHealth Index
        public void TurnOnDisplayTeamAgilityHealthIndex()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn On 'Display Team AgilityHealth Index' feature");
            TurnOnFeatureToggle(DisplayTeamAgilityHealthIndexOnOffToggle);
        }
        public void TurnOffDisplayTeamAgilityHealthIndex()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn Off 'Display Team AgilityHealth Index' feature");
            TurnOffFeatureToggle(DisplayTeamAgilityHealthIndexOnOffToggle);
        }

        //Enable Maturity Calculation
        public void TurnOnEnableMaturityCalculation()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn on 'Enable Maturity Calculations' features");
            TurnOnFeatureToggle(EnableMaturityCalculationsOnOffToggle);
        }
        public void TurnOffEnableMaturityCalculation()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn off 'Enable Maturity Calculations' features");
            TurnOffFeatureToggle(EnableMaturityCalculationsOnOffToggle);
        }

        //Enable Pulse Assessments
        public void TurnOnEnablePulseAssessments()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn On 'Enable Pulse Assessments' feature");
            TurnOnFeatureToggle(EnablePulseAssessmentsOnOffToggle);
        }
        public void TurnOffEnablePulseAssessments()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn Off 'Enable Pulse Assessments' feature");
            TurnOffFeatureToggle(EnablePulseAssessmentsOnOffToggle);
        }

        //Allow Users To Create And Manage Private App Key
        public void TurnOnAllowUsersToCreateAndManagePrivateAppKey()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn On 'Allow Users To Create And Manage Private App Key' feature");
            TurnOnFeatureToggle(AllowUsersToCreateAndManagePrivateAppKeyOnOffToggle);
        }
        public void TurnOffAllowUsersToCreateAndManagePrivateAppKey()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn Off 'Allow Users To Create And Manage Private App Key' feature");
            TurnOffFeatureToggle(AllowUsersToCreateAndManagePrivateAppKeyOnOffToggle);
        }

        //Enable AI Insights
        public void TurnOnEnableAiInsights()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn On 'Enable AI Insights' feature");
            TurnOnFeatureToggle(EnableAiInsightsOnOffToggle);
        }
        public void TurnOffEnableAiInsights()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn Off 'Enable AI Insights' feature");
            TurnOffFeatureToggle(EnableAiInsightsOnOffToggle);
        }

        //All Participants Can View Talent Dev Aggregate Results
        public void TurnOnAllParticipantsCanViewTalentDevAggregateResults()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn On 'All Participants Can View Talent Dev Aggregate Results' feature");
            TurnOnFeatureToggle(EnableAllParticipantsCanViewTalentDevAggregateResultsOnOffToggle);
        }
        public void TurnOffAllParticipantsCanViewTalentDevAggregateResults()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn Off 'All Participants Can View Talent Dev Aggregate Results' feature");
            TurnOffFeatureToggle(EnableAllParticipantsCanViewTalentDevAggregateResultsOnOffToggle);
        }

        //Enable Quick Launch For Team And Assessments
        public void TurnOnEnableQuickLaunchForTeamAndAssessments()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn On 'Enable Quick Launch For Team And Assessments' feature");
            TurnOnFeatureToggle(EnableQuickLaunchForTeamAndAssessmentsOnOffToggle);
        }
        public void TurnOffEnableQuickLaunchForTeamAndAssessments()
        {
            Log.Step(nameof(ManageFeaturesPage), "Turn Off 'Enable Quick Launch For Team And Assessments' feature");
            TurnOffFeatureToggle(EnableQuickLaunchForTeamAndAssessmentsOnOffToggle);
        }

        public void NavigateToPage(int companyId)
        {
            for (var i = 0; i < 20; i++)
            {
                if (IsPageBusy)
                {
                    Wait.HardWait(3000 * CSharpHelpers.RandomNumber(1));
                }
                else
                {
                    Wait.HardWait(2000);
                    break;
                }
            }
            IsPageBusy = true;
            NavigateToUrl($"{BaseTest.ApplicationUrl}/feature/company/{companyId}");
            for (var i = 0; i < 3; i++)
            {
                try
                {
                    Driver.IsElementDisplayed(UpdateSettingButton, 20);
                    break;
                }
                catch
                {
                    Driver.RefreshPage();
                }
            }
        }
        public void NavigateToManageFeaturePageForProd(string env, int companyId)
        {
            NavigateToUrl($"https://{env}.agilityinsights.ai/feature/company/{companyId}");
        }
    }
}