using System;
using System.Linq;
using OpenQA.Selenium;
using System.Collections.Generic;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.Utilities;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Assessment.ManageCampaigns
{
    public class ManageCampaignsCommonPage : BasePage
    {
        public ManageCampaignsCommonPage(IWebDriver driver, ILogger log) : base(driver, log) { }

        //Locators
        protected readonly By LoadingSpinner = By.XPath("//p[contains(text(), 'Loading')]//img[contains(@src, 'load') and contains(@src, '.gif')]");
        protected readonly By SetUpCampaignHeaderText = By.XPath("//h1[contains(@class,'MuiTypography-root MuiTypography-h1')]");
        private readonly By SelectTeamsHeaderText = By.XPath("//h3[contains(@class,'MuiTypography-root MuiTypography-h3')]");

        protected readonly By CampaignUpdatedSuccessfullyToastMessage = By.XPath("//div[text()='Campaign updated successfully!']");
        protected readonly By CampaignCreatedSuccessfullyToastMessage = By.XPath("//div[text()='Campaign created successfully!']");
        protected readonly By DraftSavedSuccessfullyToastMessage = By.XPath("//div[text()='Draft saved successfully!']");
        protected readonly By ToastMessageCloseButton = By.XPath("//div[@class='Toastify']//button[@aria-label='close']");
        protected readonly By SaveAsDraftButton = By.XPath("//button[text()='Save as draft']");
        protected readonly By DeleteButton = By.XPath("//button[text()='Delete']");
        protected readonly By ExitIcon = AutomationId.Equals("headerCloseBtn");
        protected readonly By DeleteCampaignPopupTitle = By.Id("alertDialogTitle");
        protected readonly By DeleteCampaignPopupCancelButton = By.XPath("//button[text()='Cancel']");
        protected readonly By DeleteCampaignPopupOkButton = By.XPath("//button[text()='Ok']");
        protected readonly By CloseCampaignPopupTitle = By.Id("alertDialogTitle");
        protected readonly By CloseCampaignPopupCancelButton = By.XPath("//button[text()='Cancel']");
        protected readonly By CloseCampaignPopupOkButton = By.XPath("//button[text()='Ok']");

        protected static By WizardStepper(string step) => By.XPath($"//div[contains(@class,'MuiStep')]//span[text()='{step}']");
        protected static By ColumnValuesList(string columnValueDataField) => By.XPath($"//div[contains(@class,'MuiDataGrid-virtualScrollerRenderZone')]//div[@data-id]/div[count(//div[@aria-rowindex='2']/div[@data-field='{columnValueDataField}']//preceding-sibling::div[@data-field] )+1]/div");

        protected static By RowValues(int index, string columnValueDataField) => By.XPath($"//div[contains(@class,'MuiDataGrid-virtualScrollerRenderZone')]//div[@data-id][{index}]/div[count(//div[@aria-rowindex='2']/div[@data-field='{columnValueDataField}']//preceding-sibling::div[@data-field] )+1]/*");

        protected static By ColumnName(string columnValueDataField) =>
            By.XPath($"//div[@aria-rowindex='2']/div[@data-field='{columnValueDataField}']//div[@aria-label]");

        protected By ResetFiltersButton = By.XPath("//button[contains(text(),'Reset filters')] | //font[contains(text(),'Reset filters')]//parent::font//parent::button");
        protected static By ListItemsByColumnName(string columnName) => By.XPath($"//table[contains(@class,'k-grid-tab')]//td[count(//table[contains(@class,'k-grid-header')]//th[@colspan='1']//span[text()='{columnName}']//ancestor::th//preceding-sibling::th)+1]");
        protected readonly By BackButton = By.XPath("//button[text()='Back']");
        protected readonly By RemoveButton = By.XPath("//button[text()='Remove']");
        public readonly By AddToCampaignButton = By.XPath("//span[contains(text(),'Add to campaign')]//parent::button | //button[contains(text(),'Add to campaign')]");
        protected readonly By ProgressBar = By.XPath("//div//span[@role='progressbar']");
        protected readonly By SelectTeamAndFacilitatorModelCloseButton = By.XPath("//button[@aria-label='close']");
        protected string SelectTeamsPrefixText = "//div[@id='select-teams-grid']";
        protected string SelectFacilitatorsPrefixText = "//div[@id='select-facilitators-grid']";
        private static By AllTooltipIcon(string name) => By.XPath($"//label[contains(text(),'{name}')]//parent::div//following-sibling::div//button");
        private readonly By AllTooltipIconText = By.XPath("//div[@role='tooltip']/div");

        //methods
        public string GetSetUpCampaignHeaderTitle()
        {
            WaitTillSpinnerNotExist();
            return Wait.UntilElementVisible(SetUpCampaignHeaderText).GetText();
        }

        public string GetSelectTeamsHeaderText()
        {
            return Wait.UntilElementVisible(SelectTeamsHeaderText).GetText();
        }

        public bool IsLoadingSpinnerDisplayed()
        {
            return Driver.IsElementDisplayed(LoadingSpinner);
        }

        public void WaitTillSpinnerNotExist()
        {
            Log.Step(GetType().Name, "Get 'Add to Campaign' button is not exist");
            Wait.UntilElementNotExist(LoadingSpinner);
        }

        public bool IsCampaignUpdatedSuccessfullyToastMessageDisplayed()
        {
            Wait.UntilJavaScriptReady();
            return Driver.IsElementDisplayed(CampaignUpdatedSuccessfullyToastMessage);
        }

        public bool IsCampaignCreatedSuccessfullyToastMessageDisplayed()
        {
            Wait.UntilJavaScriptReady();
            return Driver.IsElementDisplayed(CampaignCreatedSuccessfullyToastMessage);
        }

        public bool IsDraftSavedSuccessfullyToastMessageDisplayed()
        {
            Wait.UntilJavaScriptReady();
            return Driver.IsElementDisplayed(DraftSavedSuccessfullyToastMessage);
        }

        public void ClickOnToastMessageCloseButton()
        {
            Log.Step(GetType().Name, "Click on Toast message 'Close' button");
            Wait.UntilElementClickable(ToastMessageCloseButton).Click();
        }
        public void ClickOnSaveAsDraftButton()
        {
            Log.Step(GetType().Name, "Click on 'Save as Draft' button");
            Wait.UntilElementClickable(SaveAsDraftButton).Click();
            Wait.HardWait(2000); // Wait for saving campaign
        }

        public void ClickOnDeleteButton()
        {
            Log.Step(GetType().Name, "Click on 'Delete' button");
            Wait.UntilElementClickable(DeleteButton).Click();
        }

        public void ClickOnExitIcon()
        {
            Log.Step(GetType().Name, "Click on 'Exit' Icon");
            Wait.UntilElementClickable(ExitIcon).Click();
        }

        public string GetDeleteCampaignPopupTitle()
        {
            return Wait.UntilElementVisible(DeleteCampaignPopupTitle).GetText();
        }

        public bool IsDeleteCampaignPopupDisplayed()
        {
            return Driver.IsElementDisplayed(DeleteCampaignPopupTitle);
        }

        public void ClickOnDeleteCampaignPopupCancelButton()
        {
            Log.Step(GetType().Name, "Click on Delete Campaign popup Cancel button");
            Wait.UntilElementClickable(DeleteCampaignPopupCancelButton).Click();
            Wait.UntilJavaScriptReady();
        }

        public void ClickOnDeleteCampaignPopupOkButton()
        {
            Log.Step(GetType().Name, "Click on Delete Campaign popup Ok button");
            Wait.UntilElementClickable(DeleteCampaignPopupOkButton).Click();
        }

        public bool IsCloseCampaignPopupDisplayed()
        {
            return Driver.IsElementDisplayed(CloseCampaignPopupTitle);
        }

        public string GetCloseCampaignPopupTitle()
        {
            return Wait.UntilElementVisible(CloseCampaignPopupTitle).GetText();
        }

        public void ClickOnCloseCampaignPopupCancelButton()
        {
            Log.Step(GetType().Name, "Click on Close Campaign popup Cancel button");
            Wait.UntilElementClickable(CloseCampaignPopupCancelButton).Click();
            Wait.UntilJavaScriptReady();
        }

        public void ClickOnCloseCampaignPopupOkButton()
        {
            Log.Step(GetType().Name, "Click on Close Campaign popup Ok button");
            Wait.UntilElementClickable(CloseCampaignPopupOkButton).Click();
        }

        public bool IsResetFilterButtonEnabledDisabled()
        {
            return Driver.IsElementEnabled(ResetFiltersButton);
        }

        public void SelectWizardStep(string step)
        {
            Log.Step(GetType().Name, $"Click on '{step}' Stepper wizard");
            Wait.UntilElementClickable(WizardStepper(step)).Click();
            Wait.HardWait(3000);
        }

        public bool IsWizardStepperPresent()
        {
            return Driver.IsElementPresent(WizardStepper("Select Teams"));
        }
        public List<string> GetTableColumnValuesList(string dataField)
        {
            Log.Step(GetType().Name, "Get list of all column values as per dataField");
            return Driver.GetTextFromAllElements(ColumnValuesList(dataField)).ToList();
        }

        public List<string> GetItemsListByColumnName(string columnName, bool isPopUp, string page = "")
        {
            Log.Step(GetType().Name, "Get list of 'Team Name'");
            WaitTillSpinnerNotExist();
            var prefixText = page == "Teams" ? SelectTeamsPrefixText : SelectFacilitatorsPrefixText;
            var itemsList = isPopUp ? Driver.GetTextFromAllElements(PrefixXPath(ListItemsByColumnName(columnName), prefixText)).ToList() : Driver.GetTextFromAllElements(ListItemsByColumnName(columnName)).ToList();
            return itemsList;
        }

        public void ClickOnBackButton()
        {
            Log.Step(GetType().Name, "Click on 'Back' button to select teams");
            Wait.UntilElementClickable(BackButton).Click();
            Wait.HardWait(2000);//Need to wait until description message got changed.
        }

        public void ClickOnRemoveButton()
        {
            Log.Step(GetType().Name, "Click on 'Remove' button to select teams");
            Driver.JavaScriptScrollToElement(RemoveButton, false);
            Wait.UntilElementClickable(RemoveButton).Click();
            Wait.UntilJavaScriptReady();
        }

        public void ClickOnAddToCampaignButton()
        {
            Log.Step(GetType().Name, "Click on 'Add to Campaign' button to select teams");
            Driver.JavaScriptScrollToElement(AddToCampaignButton);
            Wait.UntilElementClickable(AddToCampaignButton).Click();
            Wait.HardWait(3000); //Wait to open Campaign details page
        }

        public bool IsAddToCampaignButtonIsEnabled()
        {
            return Driver.IsElementEnabled(AddToCampaignButton);
        }

        public void ClickOnSelectTeamAndFacilitatorModelCloseButton()
        {
            Log.Step(GetType().Name, "Click on 'Select Team And Facilitator Model Close' button to select teams");
            WaitTillSpinnerNotExist();
            Wait.UntilElementClickable(SelectTeamAndFacilitatorModelCloseButton).Click();
        }

        public bool IsSelectTeamAndFacilitatorModelCloseButtonDisplayed()
        {
            WaitTillSpinnerNotExist();
            return Driver.IsElementDisplayed(SelectTeamAndFacilitatorModelCloseButton);
        }

        public string GetTooltipMessage(string tooltipName)
        {
            var tooltipSteps = new Dictionary<string, string>
            {
                { "Campaign Name", "Get 'Campaign Name' tooltip value" },
                { "Radar Type", "Get 'Radar Type' tooltip value" },
                { "Start Date", "Get 'Start Date' tooltip value" },
                { "End Date", "Get 'End Date' tooltip value" },
                { "Parent Team", "Get 'Parent Team' tooltip value" },
                { "Target Number of Teams Per Facilitator", "Get 'Target Number of Teams Per Facilitator' tooltip value" }
            };

            if (!tooltipSteps.TryGetValue(tooltipName, out var stepDescription))
                throw new ArgumentException($"Tooltip name '{tooltipName}' is not recognized.");
            Log.Step(GetType().Name, stepDescription);
            Driver.MoveToElement(AllTooltipIcon(tooltipName));
            Wait.HardWait(1000); // wait till tooltip text displayed
            return Wait.UntilElementVisible(AllTooltipIconText).GetText();
        }

        public string GetSplitAssessmentTooltipMessage(string tooltipName)
        {
            var tooltipSteps = new Dictionary<string, string>
            {
                { "Stakeholder Launch Date", "Get 'Stakeholder Launch Date' tooltip value" },
                { "Team Member Launch Date", "Get 'Team Member Launch Date' tooltip value" },
                { "Assessment Close Date", "Get 'Assessment Close Date' tooltip value" },
                { "Retrospective Window Start", "Get 'Retrospective Window Start' tooltip value" },
                { "Retrospective Window End", "Get 'Retrospective Window End' tooltip value" }
            };

            if (!tooltipSteps.TryGetValue(tooltipName, out var stepDescription))
                throw new ArgumentException($"Tooltip name '{tooltipName}' is not recognized.");
            Log.Step(GetType().Name, stepDescription);
            Driver.MoveToElement(AllTooltipIcon(tooltipName));
            Wait.HardWait(1000); // wait till tooltip text displayed
            return Wait.UntilElementVisible(AllTooltipIconText).GetText();
        }

        public string GetOneAssessmentTooltipMessage(string tooltipName)
        {
            var tooltipSteps = new Dictionary<string, string>
            {
                { "Stakeholder Window Start", "Get 'Stakeholder Window Start' tooltip value" },
                { "Stakeholder Window End", "Get 'Stakeholder Window End' tooltip value" },
                { "Retrospective Window Start", "Get 'Retrospective Window Start' tooltip value" },
                { "Team Member Launch Date", "Get 'Team Member Launch Date' tooltip value" },
                { "Retrospective Window End", "Get 'Retrospective Window End' tooltip value" }
            };

            if (!tooltipSteps.TryGetValue(tooltipName, out var stepDescription))
                throw new ArgumentException($"Tooltip name '{tooltipName}' is not recognized.");
            Log.Step(GetType().Name, stepDescription);
            Driver.MoveToElement(AllTooltipIcon(tooltipName));
            Wait.HardWait(1000); // wait till tooltip text displayed
            return Wait.UntilElementVisible(AllTooltipIconText).GetText();
        }

        public string GetRetroAssessmentTooltipMessage(string tooltipName)
        {
            var tooltipSteps = new Dictionary<string, string>
            {
                { "Assessment Start Date", "Get 'Assessment Start Date' tooltip value" },
                { "Assessment Close Date", "Get 'Assessment Close Date' tooltip value" },
                { "Retrospective Window Start", "Get 'Retrospective Window Start' tooltip value" },
                { "Retrospective Window End", "Get 'Retrospective Window End' tooltip value" }
            };

            if (!tooltipSteps.TryGetValue(tooltipName, out var stepDescription))
                throw new ArgumentException($"Tooltip name '{tooltipName}' is not recognized.");
            Log.Step(GetType().Name, stepDescription);
            Driver.MoveToElement(AllTooltipIcon(tooltipName));
            Wait.HardWait(1000); // wait till tooltip text displayed
            return Wait.UntilElementVisible(AllTooltipIconText).GetText();
        }

        public static By PrefixXPath(By originalLocator, string prefix)
        {
            if (originalLocator == null)
                throw new ArgumentNullException(nameof(originalLocator));

            if (string.IsNullOrWhiteSpace(prefix))
                throw new ArgumentNullException(nameof(prefix));

            var originalXPath = originalLocator.ToString().Replace("By.XPath: ", "");
            var newXPath = prefix + originalXPath;
            return By.XPath(newXPath);
        }

        public void ClickOnResetFiltersButton(bool isPopUp, string page = "")
        {
            Log.Step(GetType().Name, "Click on 'Reset Filters' button to reset filters");
            var originalResetFiltersButton = ResetFiltersButton;
            var tempResetFiltersButton = originalResetFiltersButton;
            Wait.HardWait(5000);
            if (isPopUp)
            {
                var prefixText = page == "Teams" ? SelectTeamsPrefixText : SelectFacilitatorsPrefixText;
                tempResetFiltersButton = PrefixXPath(originalResetFiltersButton, prefixText);
            }
            Wait.UntilElementClickable(tempResetFiltersButton).Click();
            WaitTillSpinnerNotExist();
        }

        public bool IsListSorted(List<string> list)
        {
            var sortedList = list.OrderBy(item => item, StringComparer.OrdinalIgnoreCase).ToList();
            return list.SequenceEqual(sortedList);
        }

        public static By PostfixXPath(By originalLocator, string postfix)
        {
            if (originalLocator == null)
                throw new ArgumentNullException(nameof(originalLocator));

            if (string.IsNullOrWhiteSpace(postfix))
                throw new ArgumentNullException(nameof(postfix));

            var originalXPath = originalLocator.ToString().Replace("By.XPath: ", "");
            var newXPath = originalXPath + postfix;
            return By.XPath(newXPath);
        }
    }
}