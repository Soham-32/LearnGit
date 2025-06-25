using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.ObjectFactories;
using AtCommon.Dtos;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using AtCommon.Api;
using AgilityHealth_Automation.Utilities;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.GrowthItems;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.ManageFeatures;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Dashboards.GrowthItems
{
    [TestClass]
    [TestCategory("GrowthItemsDashboard"),TestCategory("GenAi"), TestCategory("Smoke"), TestCategory("HeartBeatChecks")]
    public class GiDashboardGenAiSummarizationTests : BaseTest
    {
        public static ProductionEnvironmentDatabase ProductionEnvironmentTestData = File.ReadAllText(new FileUtil().GetBasePath() + "Resources/TestData/ProductionEnvironmentDatabase.json").DeserializeJsonObject<ProductionEnvironmentDatabase>();
        private static Environments _productionDatabaseConnectionInfo;

        [TestMethod]
        [DynamicData(nameof(Constants.GetProdCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]

        public void AiSummarizationVerifyPromptsGivingAnswerSuccessfully(string companyName)
        {
            var growthItemsDashboardPage = new GrowthItemsDashboardPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var giDashboardGridView = new GiDashboardGridWidgetPage(Driver, Log);

            _productionDatabaseConnectionInfo = ProductionEnvironmentDatabaseFactory.GetProductionDatabaseConnectionInfo(ProductionEnvironmentTestData, companyName);

            Log.Info("Login using valid credentials in the production environment.");
            LoginToProductionEnvironment(_productionDatabaseConnectionInfo.Name);

            Log.Info("Navigate to the 'Manage Feature Settings' page and verify that 'Enable Ai Summarization' toggle is on.");
            manageFeaturesPage.NavigateToManageFeaturePageForProd(_productionDatabaseConnectionInfo.Name, _productionDatabaseConnectionInfo.CompanyId);
            if (!manageFeaturesPage.IsFeatureToggleButtonOn("EnableAIInsights-features"))
            {
                return;
            }

            Log.Info("Navigate to the 'Growth Items' page and verify that the 'AI Summarization' button is displayed.");
            growthItemsDashboardPage.NavigateToGrowthItemsPageForProd(_productionDatabaseConnectionInfo.Name, _productionDatabaseConnectionInfo.CompanyId);
            giDashboardGridView.ClearFilter();
            Assert.IsTrue(growthItemsDashboardPage.IsAiSummarizationButtonDisplayed(), "The 'AI Summarization' button is not displayed.");

            Log.Info("Click on 'Ai Summarization' toggle button and verify that the summarization popup is displayed.");
            growthItemsDashboardPage.ClickOnAiSummarizationButton();
            Assert.IsTrue(growthItemsDashboardPage.IsAiSummarizationPopUpDisplayed(), "The Ai Summarization popup is not displayed.");

            Log.Info("Click on the prompts and verify that the response text is displayed successfully.");
            foreach (var prompt in Constants.ListOfAiSummarizationPrompts)
            {
                growthItemsDashboardPage.ClickOnThePrompt(prompt);

                Assert.IsTrue(growthItemsDashboardPage.IsPromptResponseDisplayed(prompt), $"The response for the prompt:- '{prompt}' is not displayed");
                Assert.IsTrue(growthItemsDashboardPage.IsRegenerateButtonDisplayed(), "The 'Regenerate' button is not displayed");
                Assert.IsTrue(growthItemsDashboardPage.IsCopyLinkDisplayed(), $"The 'Copy' link is not displayed in response of the prompt: {prompt}");
                Assert.IsTrue(growthItemsDashboardPage.IsReadOurDataAndPrivacyPolicyLinkDisplayed(), "The 'Read out Data and Privacy Policy' link is not displayed");

                growthItemsDashboardPage.ClickOnBackToPromptsLink();
            }

            Log.Info("Click on 'Summarize by team' checkbox and verify that the checkbox is checked.");
            growthItemsDashboardPage.ClickOnSummarizeByTeamCheckbox();
            Assert.IsTrue(growthItemsDashboardPage.IsSummarizeByTeamCheckboxChecked(), "The 'Summarize by team' checkbox is not checked.");

            Log.Info("Click on the prompts and verify that the response text is displayed successfully.");
            foreach (var prompt in Constants.ListOfAiSummarizationPrompts)
            {
                growthItemsDashboardPage.ClickOnThePrompt(prompt);

                Assert.IsTrue(growthItemsDashboardPage.IsPromptResponseDisplayed(prompt), $"The response for the prompt:- '{prompt}' is not displayed");
                Assert.IsTrue(growthItemsDashboardPage.IsRegenerateButtonDisplayed(), "The 'Regenerate' button is not displayed");
                Assert.IsTrue(growthItemsDashboardPage.IsCopyLinkDisplayed(), $"The 'Copy' link is not displayed in response of the prompt: {prompt}");
                Assert.IsTrue(growthItemsDashboardPage.IsReadOurDataAndPrivacyPolicyLinkDisplayed(), "The 'Read out Data and Privacy Policy' link is not displayed");

                growthItemsDashboardPage.ClickOnBackToPromptsLink();
            }
        }
    }
}
