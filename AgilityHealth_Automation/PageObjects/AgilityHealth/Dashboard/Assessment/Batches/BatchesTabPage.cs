using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.Utilities;
using AtCommon.Dtos.Assessments.Team.Custom;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Assessment.Batches
{
    internal class BatchesTabPage : AssessmentDashboardBasePage
    {
        public BatchesTabPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        private readonly By PlusButton = By.CssSelector("div[onclick='redirectToCreateBatch();']");
        private readonly By SearchTextBox = By.Id("teamFilterBox1");
        private readonly By ManageBatchesTabTitle = By.XPath("//h1[text()='Manage Batches']");

        private static By BatchNameFromGrid(int rowIndex) => By.XPath($"//div[@id='batchAssessmentManagement']//tr[{rowIndex}]/td[3]");
        private static By AssessmentNameFromGrid(int rowIndex) => By.XPath($"//div[@id='batchAssessmentManagement']//tr[{rowIndex}]/td[4]");
        private static By AssessmentDateFromGrid(int rowIndex) => By.XPath($"//div[@id='batchAssessmentManagement']//tr[{rowIndex}]/td[5]");
        private static By AssessmentTypeFromGrid(int rowIndex) => By.XPath($"//div[@id='batchAssessmentManagement']//tr[{rowIndex}]/td[6]");

        private static By BatchEditButton(string batchName) => By.XPath(
            $"//div[@id = 'batchAssessmentManagement']//td[text() = '{batchName}']/parent::tr/td//li/span[text() = 'Edit'] | //div[@id = 'batchAssessmentManagement']//td//font[text() = '{batchName}']/ancestor::tr/td//li/span//font[text() = 'Edit']");
        private static By ColumnValue(string columnHeader) => By.XPath($"//div[@class='content clear']//div[@id='gridWrapper1']//tbody/tr/td[count(//div[@id='batchAssessmentManagement']//thead//th[@data-title='{columnHeader}']/preceding-sibling::th)+1]");
        private static By BatchTeamName(string teamName) => By.XPath($"//div[@id='teamsDetails']//tbody//tr//*[text()='{teamName}']");
        private readonly By AssessmentTypeDropdown = By.XPath("//span[@aria-owns='ddlAssessmentType_listbox']");
        private readonly By AssessmentTypeAllValues = By.XPath("//ul[@id='ddlAssessmentType_listbox']/li");

        public List<string> GetAllAssessmentTypes()
        {
            Log.Step(nameof(BatchesTabPage), "Get all assessment types  from 'Assessment Types' dropdown");
            Wait.UntilElementClickable(AssessmentTypeDropdown).Click();
            Wait.UntilJavaScriptReady();
            var assessmentTypes = Driver.GetTextFromAllElements(AssessmentTypeAllValues).ToList();
            Wait.UntilElementClickable(AssessmentTypeDropdown).Click();
            return assessmentTypes;
        }
        public void ClickPlusButton()
        {
            Log.Step(nameof(BatchesTabPage), "Click on Plus button");
            Wait.UntilElementClickable(PlusButton).Click();
            Wait.UntilJavaScriptReady();
        }

        public void SearchBatchName(string batchName)
        {
            Log.Step(nameof(BatchesTabPage), $"Search batch with name <{batchName}>");
            Wait.UntilElementVisible(SearchTextBox).SetText(batchName);
            Wait.UntilJavaScriptReady();
        }

        public BatchAssessment GetBatchItemFromGrid(int rowIndex)
        {
            Log.Step(nameof(BatchesTabPage), $"Get batch item from the Grid for {rowIndex} row");
            Wait.UntilJavaScriptReady();

            string dateText = Wait.UntilElementVisible(AssessmentDateFromGrid(rowIndex)).GetText();
            DateTime parsedDate = DateTime.ParseExact(dateText, "M/d/yyyy", CultureInfo.InvariantCulture);

            var batchAssessment = new BatchAssessment()
            {
                BatchName = Wait.UntilElementVisible(BatchNameFromGrid(rowIndex)).GetText(),
                AssessmentName = Wait.UntilElementVisible(AssessmentNameFromGrid(rowIndex)).GetText(),
                StartDate = parsedDate,
                AssessmentType = Wait.UntilElementVisible(AssessmentTypeFromGrid(rowIndex)).GetText()
            };

            return batchAssessment;
        }

        public void ClickBatchEditButton(string batchName)
        {
            Log.Step(nameof(BatchesTabPage), $"Click on Edit batch <{batchName}> button");
            Wait.UntilElementClickable(BatchEditButton(batchName)).Click();
            Wait.UntilJavaScriptReady();
            Wait.HardWait(1000); //Edit Batch Window takes 1 sec to load properly
        }

        public List<string> GetColumnValues(string columnHeader)
        {
            Log.Step(nameof(BatchesTabPage), "Get all column values");
            return Wait.UntilAllElementsLocated(ColumnValue(columnHeader)).Select(header => Driver.JavaScriptScrollToElement(header).GetText()).ToList();
        }

        public void ClickOnBatchTeam(string teamName)
        {
            Log.Step(nameof(BatchesTabPage), $"Click on batch <{teamName}> team");
            Wait.UntilElementClickable(BatchTeamName(teamName)).Click();
            Wait.UntilJavaScriptReady();
        }

        public string GetManageBatchesTabTitle()
        {
            return Wait.UntilElementVisible(ManageBatchesTabTitle).GetText();
        }

        public void NavigateToPage(int companyId)
        {
            NavigateToUrl($"{BaseTest.ApplicationUrl}/company/{companyId}/BatchAssessmentManagementDashboard");
        }

        public bool IsPlusButtonDisplayed()
        {
            return Driver.IsElementDisplayed(PlusButton);
        }

        //Navigation
        public void NavigateToBatchesTabForProd(string env, int id)
        {
            NavigateToUrl($"https://{env}.agilityinsights.ai/company/{id}/BatchAssessmentManagementDashboard");
        }
    }
}
