using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.ObjectFactories;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Insights.StructuralAgility;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.ManageFeatures;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Dashboards.Insights.StructuralAgility
{
    [TestClass]
    public class InsightsStructuralAgilityNlqQuestionsTests : BaseTest
    {
        public static ProductionEnvironmentDatabase ProductionEnvironmentTestData = File.ReadAllText(new FileUtil().GetBasePath() + "Resources/TestData/ProductionEnvironmentDatabase.json").DeserializeJsonObject<ProductionEnvironmentDatabase>();
        private static Environments _productionDatabaseConnectionInfo;

        [TestMethod]
        [TestCategory("StructuralAgility"), TestCategory("Critical"), TestCategory("Smoke"), TestCategory("HeartBeatChecks")]
        [DynamicData(nameof(Constants.GetProdCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]

        public void StructuralAgilityVerifyNlqFunctionality(string companyName)
        {
            _productionDatabaseConnectionInfo = ProductionEnvironmentDatabaseFactory.GetProductionDatabaseConnectionInfo(ProductionEnvironmentTestData, companyName);
            var structuralAgilityPage = new StructuralAgilityPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);

            Log.Info("Login using valid credentials in the production environment");
            LoginToProductionEnvironment(_productionDatabaseConnectionInfo.Name);

            Log.Info("Navigate to the 'Manage Feature Settings' page and verify that 'NLQ (Natural Language Query)' toggle is on.");
            manageFeaturesPage.NavigateToManageFeaturePageForProd(_productionDatabaseConnectionInfo.Name, _productionDatabaseConnectionInfo.CompanyId);

            if (!manageFeaturesPage.IsFeatureToggleButtonOn("NLQ-features"))
            {
                return;
            }

            Log.Info("Navigate to the Structural Agility page and verify that the 'Ask Agility' toggle button is displayed.");
            structuralAgilityPage.NavigateToInsightsStructuralAgilityPageForProd(_productionDatabaseConnectionInfo.Name, _productionDatabaseConnectionInfo.CompanyId);
            Assert.IsTrue(structuralAgilityPage.IsAskAgilityToggleButtonDisplayed(), "The 'Ask Agility' toggle button is not displayed.");

            Log.Info("Click on 'Ask Agility' toggle button and verify that the 'Agility Insights' dialog box is displayed.");
            structuralAgilityPage.ClickOnAskAgilityToggleButton();
            Assert.IsTrue(structuralAgilityPage.IsAskAgilityInsightsDialogBoxDisplayed(), "The Agility Insights Dialog box is not displayed.");

            Log.Info("Navigate to the Question Bank tab and click on a first question to copy it.");
            structuralAgilityPage.ClickOnQuestionBankTab();
            var enteredQuestion = structuralAgilityPage.ClickAndGetFirstQuestion();
            Assert.IsTrue(structuralAgilityPage.IsQuestionBankTabActive(), "The question are not displayed.");

            Log.Info("Navigate to 'Ask AgilityInsights' tab and Paste the copied question in the question text box.");
            structuralAgilityPage.ClickOnAskAgilityInsightsTab();
            structuralAgilityPage.SwitchToNlqIframe();
            structuralAgilityPage.EnterCopiedQuestionInTheInputBox();

            Log.Info($"Verify that the response for the question: '{enteredQuestion}' is not blank.");
            Assert.IsFalse(structuralAgilityPage.IsReceivedResponseBlank(), $"The received response is blank for this question: {enteredQuestion}");
        }
    }
}
