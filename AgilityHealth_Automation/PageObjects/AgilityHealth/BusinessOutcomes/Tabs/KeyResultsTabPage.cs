using System;
using AgilityHealth_Automation.Utilities;
using AtCommon.Dtos.BusinessOutcomes;
using AtCommon.Utilities;
using OpenQA.Selenium;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using AtCommon.Dtos.BusinessOutcomes.Custom;
using OpenQA.Selenium.Interactions;


namespace AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.Tabs
{
    public class KeyResultsTabPage : BaseTabPage
    {
        public KeyResultsTabPage(IWebDriver driver, ILogger log) : base(driver, log) { }

        //Locators
        #region LinkKeyResults

        private readonly By LinkKeyResultsButton = By.XPath("//button[text()='Link to Outcome'] | //button//font//font[text()='Link to Outcome']");
        private static By ParentOutcomeKeyResultsCheckbox(string keyResult) => By.XPath($"(//*[text()='{keyResult}']//ancestor::td//preceding-sibling::td//*[contains(@data-testid,'RadioButton')])[1]");
        private readonly By LinkParentOutcomeKrPopupLinkButton = By.XPath("//button//span[text()='Link'] | //button//span//font[text()='Link']");
        private readonly By LinkParentOutcomeKrPopupCancelButton = By.XPath("//button//span[text()='Cancel'] | //button//span//font[text()='Cancel']");
        private readonly By KeyResultsLinkedToasterMessage = By.XPath("//div[text()='2 Key Results Linked Successfully']");

        #endregion

        #region KeyResults LinkToOutcome Aligning/Contributing Popup

        private readonly By KeyResultsLinkedToOutcomeAligningConfirmationPopupTitle = By.CssSelector(".k-window-title.k-dialog-title");
        private readonly By KeyResultsLinkedToOutcomeAligningConfirmationPopupDescription = By.XPath("//div[@class='k-window-content k-dialog-content']//p");
        private readonly By KeyResultsLinkedToOutcomeAligningConfirmationPopupLearnMoreLink = By.XPath("//a[text()='Learn More']");

        #endregion

        #region KeyResultsTab
        private readonly By KeyResultAddButton = By.XPath("//button[contains(text(),'Add Key Results')]//*[local-name()='svg' and @data-testid='AddCircleIcon']");
        private static By DynamicKeyResultName(int rowIndex) => By.Name($"keyResults[{rowIndex - 1}].title");
        private static By DynamicKeyResultArrowDownIcon(int rowIndex) => By.XPath($"//*[@aria-label='expand row'][{rowIndex}]");

        private readonly By DynamicLinkedKeyResultColumnName = By.XPath("//table[contains(@class,'key__results ')]//*[contains(@class,'k-grid k-grid-md')]//span[@class='k-column-title']");

        private static By DynamicLinkedKeyResultTitle(string keyResultTitle) => By.XPath($"//*[text()='{keyResultTitle}']");

        private readonly By DynamicLinkedKeyResultRows = By.XPath($".//td | .//td//p");

        private readonly By DynamicLinkedKeyResultValues = By.XPath("//*[contains(@class,'key__results ')]//tbody[@class='k-table-tbody']/tr[contains(@class, 'k-table-row')]");

        private static By DynamicKeyResultWeight(int rowIndex) => By.CssSelector($"*[name*='keyResults[{rowIndex}].weight']");
        private static By DynamicKeyResultLink(int rowIndex) => By.XPath($"(//*[@alt='Parent Key Result Link']//parent::a)[{rowIndex}]");
        private static By KeyResultTitle(string title) => By.XPath($"//p[text() = '{title}'] | //textarea[text()='{title}']");
        private static By DynamicKeyResultImpactButton(string keyResultName) => By.XPath($"//textarea[text()='{keyResultName}']//ancestor::td//preceding-sibling::td//button[@aria-label='Impact']");
        private static By DynamicKeyResultMetricInput(string value) => By.XPath($"//div[@role='presentation']//ul//li//font[contains(text(),'{value}')] | //div[@role='presentation']//ul//li[contains(text(),'{value}')]");
        private static By DynamicKeyResultMetricDropdown(int rowIndex) => By.XPath($"//textarea[@name='keyResults[{rowIndex - 1}].title']/ancestor::td/following-sibling::td//div/textarea[contains(@placeholder,'Select Metric')]");
        private static By DynamicKeyResultStart(int rowIndex) => By.Name($"keyResults[{rowIndex - 1}].start");
        private static By DynamicKeyResultGoal(int rowIndex) => By.Name($"keyResults[{rowIndex - 1}].target");
        private static By DynamicKeyResultCurrent(int rowIndex) => By.Name($"keyResults[{rowIndex - 1}].progress");
        private static By DynamicKeyResultStretch(int rowIndex) => By.Name($"keyResults[{rowIndex - 1}].stretchGoal");
        private static By DynamicKeyResultProgressBar(int rowIndex) => By.XPath($"(//table[contains(@class,'key__result')]//div[contains(@class,'k-progressbar k-progressbar-horizontal')])[{rowIndex}]");
        private static By DynamicKeyResultProgressPercentage(int rowIndex) => By.XPath($"(//div[contains(@class,'k-progressbar k-progressbar-horizontal')]//parent::div//parent::div/following-sibling::span)[{rowIndex}]");
        private readonly By KeyResultDeleteButton = By.XPath("//li[contains(text(),'Delete')]");
        private readonly By ExistingKeyResultTextArea = By.XPath("//div//p[contains(@id,'text-item-translation')]");
        private readonly By KeyResultTextArea = By.Name("//textarea[@name='keyResults[0].title']");
        private readonly By KeyResultsRows = By.CssSelector("table.key__results tbody tr");
        private readonly By KeyResultMoreIconLinkToOutcomeButton = By.XPath("//ul//li[text()='Link to Outcome']");
        private readonly By KeyResultMoreIconUnlinkButton = By.XPath("//ul//li[text()='Unlink']");
        private readonly By UnlinkConfirmationPopupUnlinkButton = By.XPath("//span[text()='Unlink']");
        private static By KeyResultActionKebabMenu(string keyResultTitle) => By.XPath($"//textarea[text()='{keyResultTitle}']//ancestor::tr//*[local-name()='svg' and @data-testid='MoreVertIcon'] | //p[text()='{keyResultTitle}']//ancestor::tr//*[local-name()='svg' and @data-testid='MoreVertIcon']");
        private static By ParentKeyResultLinkedIcon(string keyResultTitle) =>
            By.XPath($"//p[text()='{keyResultTitle}']//ancestor::tr//img[@alt='Parent Key Result Link']");
        private static By UnlinkConfirmationPopupKeyResultTitle(string keyResultTitle) =>
            By.XPath($"//div[contains(@class,'k-window-content')]//ul//p[text()='{keyResultTitle}']");
        private static By KeyResultsExpandCollapseIcon(string keyResultTitle) => By.XPath(
            $"//*[text()='{keyResultTitle}']/ancestor::tr//button[@aria-label='expand row']//*[local-name()='svg']");
        private static By KeyResultsCaretIconButton(string keyResultTitle) => By.XPath(
            $"//td[p//font[text()='{keyResultTitle}'] | //p[text()='{keyResultTitle}']]/ancestor::p//preceding-sibling::tr//button[@aria-label='expand row']//*[local-name()='svg']/parent::button[@style='visibility: visible;']");
        private static By KeyResultInitiativeCardName(string keyResultTitle) =>
            By.XPath($"//table[@role='presentation']//tr//*[text()='{keyResultTitle}']");

        #endregion

        #region KeyResultsLock

        private readonly By KeyResultUnlockedButton = By.XPath("//*[local-name()='svg' and @data-icon='lock-open' and not(@aria-hidden='true')]");
        private readonly By KeyResultLockedButton = By.XPath("//*[local-name()='svg' and @data-icon='lock' and not(@aria-hidden='true')]");
        private readonly By KeyResultLockUnLockPopUpTitle = By.CssSelector(".k-window-title.k-dialog-title");
        private readonly By KeyResultLockUnLockPopUpDescription = By.XPath("//div[@class='k-window-content k-dialog-content']//*[contains(@class,'MuiTypography-root MuiTypography-body1')]");
        private readonly By ConfirmButton = By.XPath("//span[text()='Confirm']");
        private readonly By CancelButton = By.XPath("//span[text()='Cancel']");
        private readonly By KeyResultLockToUnlockImage = By.XPath("//img[contains(@alt, 'Lock to unlock image')]");
        private readonly By KeyResultUnLockToLockImage = By.XPath("//img[contains(@alt, 'Unlock to lock image')]");

        #endregion

        #region View Details

        private readonly By KeyResultActionKebabViewDetails = By.XPath("//ul//li[text()='View Details']");

        #endregion

        #region AddNewMetric

        //Add New Metric
        private static By MetricValue(int rowIndex) => By.XPath($"(//textarea[@placeholder='Select Metric'])[{rowIndex}]");
        private readonly By MetricTypeDropdown = By.XPath("//input[@id='addNewMetricSelect']//preceding-sibling::div");
        private static By MetricDropdownValues(string value) => By.XPath($"//li[text()='{value}']");
        private readonly By MetricSaveButton = By.XPath("//button//span[text()='Save'] | //button//span//font[text()='Save']");
        private readonly By MetricPopUpHeaderText = By.XPath("//h2[text()='Add New Metric'] | //h2//font[text()='Add New Metric']");
        private readonly By BusinessOutcomeLeyMetricSaveAlertDialog = By.XPath("//div[@role='alert']//div[text()='Added new metric successfully']");

        #endregion

        //Progress %
        private readonly By ProgressBar = By.XPath("//div//p[text()='Overall Progress']/../div//div[@role='progressbar']");
        private readonly By Percentage = By.XPath("//div//p[text()='Overall Progress']/..//p[2]");

        #region SelectParentOutcomeDropdown
        private readonly By SelectParentOutcomeDropdown = By.XPath("//span[(text()='Select Parent' or font//font[text()='Select Parent'])]//following-sibling::div//input");
        public static By SelectParentOutcomeDropdownItem(string parentOutcome) => By.XPath($"//ul[@id='selectedCard-listbox']//li[contains(text(),'{parentOutcome}') or //font[contains(text(),'{parentOutcome}')]]");

        #endregion

        //Methods

        #region Key Results tab

        #region Link Key Results

        public void ClickOnLinkKeyResultsButton()
        {
            Log.Info("Click on 'Link Key Results Button' ");
            Wait.UntilElementClickable(LinkKeyResultsButton).Click();
            Wait.HardWait(2000); //Need to wait till KRs loaded
        }

        public bool IsLinkToOutcomeDisplayed()
        {
            return Driver.IsElementDisplayed(LinkKeyResultsButton);
        }

        public bool IsLinkToOutcomeEnabled()
        {
            return Driver.IsElementEnabled(LinkKeyResultsButton);
        }

        public void SelectParentOutcomeKeyResults(List<string> keyResults)
        {
            Log.Info("Select Parent outcome 'Key Results' from the Popup ");
            foreach (var keyResult in keyResults)
            {
                var checkboxElement = Wait.UntilElementClickable(ParentOutcomeKeyResultsCheckbox(keyResult));
                new Actions(Driver).MoveToElement(checkboxElement).Click().Perform();
                Wait.UntilJavaScriptReady();
            }
            Wait.UntilElementClickable(LinkParentOutcomeKrPopupLinkButton).Click();
            Wait.HardWait(3000); // Need to wait till KRs are selected
        }

        public void ClickOnLinkKeyResultPopupCancelButton()
        {
            Log.Info("Click on 'Link Key Results' Cancel Button ");
            Wait.UntilElementClickable(LinkParentOutcomeKrPopupCancelButton).Click();
        }
        public void SelectLinkParentOutcomeKeyResult(string keyResultTitle)
        {
            Log.Info("Select one Parent KeyResult outcome");
            Wait.UntilElementExists(ParentOutcomeKeyResultsCheckbox(keyResultTitle)).Click();
            Wait.UntilJavaScriptReady();
            Wait.UntilElementClickable(LinkParentOutcomeKrPopupLinkButton).Click();
        }

        public string GetKeyResultsLinkedSuccessfullyToasterMessage()
        {
            var toasterMessage = Wait.UntilElementVisible(KeyResultsLinkedToasterMessage).GetText();
            Wait.UntilElementInvisible(KeyResultsLinkedToasterMessage);
            return toasterMessage;
        }

        public bool IsKeyResultDisplayed()
        {
            var rowIndex = Driver.GetElementCount(KeyResultsRows) - 1;
            return Driver.IsElementDisplayed(DynamicKeyResultName(rowIndex));
        }

        #endregion

        public void ClickOnKeyResultAddButton()
        {
            Log.Step(nameof(KeyResultsTabPage), "Click on 'Add Key Result' button");
            Driver.JavaScriptScrollToElement(KeyResultAddButton).Click();
        }

        public bool IsKeyResultButtonDisplayed()
        {
            return Driver.IsElementDisplayed(KeyResultAddButton);
        }

        public bool IsKeyResultButtonEnabled()
        {
            return Driver.IsElementEnabled(KeyResultAddButton);
        }

        public void EnterKeyResultTitle(int rowIndex, string title)
        {
            if (Driver.IsElementPresent(KeyResultTextArea))
            {
                Wait.UntilElementClickable(KeyResultTextArea).Click();
            }
            if (!Driver.IsElementPresent(DynamicKeyResultName(rowIndex))) return;
            Wait.UntilElementVisible(DynamicKeyResultName(rowIndex)).ClearTextbox();
            Driver.JavaScriptScrollToElement(DynamicKeyResultName(rowIndex)).SetText(title, isReact: true);
        }

        public string GetKeyResultTitleText(int rowIndex)
        {
            return Driver.IsElementPresent(ExistingKeyResultTextArea) ? Wait.UntilElementExists(ExistingKeyResultTextArea).GetText() : Wait.UntilElementExists(DynamicKeyResultName(rowIndex)).GetText();
        }

        public void AddKeyResult(KeyResultRequest request)
        {
            Log.Step(nameof(KeyResultsTabPage), "Add new key result");
            ClickOnKeyResultAddButton();
            var newRowIndex = Driver.GetElementCount(KeyResultsRows);
            EnterKeyResultTitle(newRowIndex, request.Title);

            SelectMetric(newRowIndex, request.Metric.Name);
            Driver.JavaScriptScrollToElement(DynamicKeyResultStart(newRowIndex)).SetText(request.Start).SendKeys(Keys.Tab);
            Driver.JavaScriptScrollToElement(DynamicKeyResultGoal(newRowIndex)).SetText(request.Target).SendKeys(Keys.Tab);
            Driver.JavaScriptScrollToElement(DynamicKeyResultCurrent(newRowIndex)).SetText(request.Progress.ToString("N")).SendKeys(Keys.Tab);

            var element = Driver
                .JavaScriptScrollToElement(DynamicKeyResultImpactButton(request.Title));

            if (request.IsImpact && element.GetAttribute("title").Contains("Enable") ||
                !request.IsImpact && element.GetAttribute("title").Contains("Disable"))
            {
                element.Click();
            }
        }
        public void EditKeyResult(string originalTitle, KeyResultRequest updatedRequest)
        {
            Log.Step(nameof(KeyResultsTabPage), $"Edit key result with title {updatedRequest.Title}");

            var newRowIndex = Driver.GetElementCount(KeyResultsRows);
            EnterKeyResultTitle(newRowIndex, updatedRequest.Title);

            SelectMetric(newRowIndex, updatedRequest.Metric.Name);
            Driver.JavaScriptScrollToElement(DynamicKeyResultStart(newRowIndex)).SetText(updatedRequest.Start).SendKeys(Keys.Tab);
            Driver.JavaScriptScrollToElement(DynamicKeyResultGoal(newRowIndex)).SetText(updatedRequest.Target).SendKeys(Keys.Tab);
            Driver.JavaScriptScrollToElement(DynamicKeyResultCurrent(newRowIndex)).SetText(updatedRequest.Progress.ToString("N")).SendKeys(Keys.Tab);

            var element = Driver
                .JavaScriptScrollToElement(DynamicKeyResultImpactButton(updatedRequest.Title));

            if (updatedRequest.IsImpact && element.GetAttribute("title").Contains("Enable") ||
                !updatedRequest.IsImpact && element.GetAttribute("title").Contains("Disable"))
            {
                element.Click();
            }
        }

        public List<LinkedKeyResult> GetLinkedKeyResults()
        {
            Log.Step(nameof(KeyResultsTabPage), $"Get the added Key Results");
            var keyResultsLinkedValues = new List<LinkedKeyResult>();
            var rows = Wait.UntilAllElementsLocated(DynamicLinkedKeyResultValues);
            for (var i = 0; i < rows.Count; i++)
            {
                // Retrieve all column values (including nested <p> elements)
                var columns = rows[i].FindElements(DynamicLinkedKeyResultRows);

                // Extract and clean text values
                var keyResultValues = columns.Select(col => col.Text.Trim()).Where(text => !string.IsNullOrEmpty(text)).Distinct().ToList();

                // Map extracted values to DTO properties
                keyResultsLinkedValues.Add(new LinkedKeyResult()
                {
                    LinkedKeyResultsTitle = keyResultValues[0],
                    Relationship = keyResultValues[1],
                    Metric = keyResultValues[2],
                    CardType = keyResultValues[3],
                    CardTitle = keyResultValues[4],
                    Team = keyResultValues[5],
                    Start = keyResultValues[6],
                    Target = keyResultValues[7],
                    Progress = keyResultValues[8], // Handle double parsing
                    ProgressBar = keyResultValues[9]
                });
            }
            return keyResultsLinkedValues;
        }

        public void ClickOnKeyResultExpandArrowIcon(int row)
        {
            Log.Step(nameof(KeyResultsTabPage), "Click on Arrow Icon for KeyResult");
            Wait.UntilElementClickable(DynamicKeyResultArrowDownIcon(row)).Click();
        }

        public void ClickOnLinkedKeyResultName(string keyResultTitle)
        {
            Log.Step(nameof(KeyResultsTabPage), "Click on 'Linked Key Result' button");
            Wait.UntilElementClickable(DynamicLinkedKeyResultTitle(keyResultTitle)).Click();

        }

        public string GetLinkedKeyResultTooltipText(int row)
        {
            Log.Step(nameof(KeyResultsTabPage), "Get the Linked KeyResult Tooltip Value");
            return Wait.UntilElementVisible(DynamicKeyResultLink(row)).GetAttribute("aria-label");
        }

        public void ClickLinkedKeyResult(int row)
        {
            Log.Step(nameof(KeyResultsTabPage), "Click on Link icon");
            Wait.UntilElementClickable(DynamicKeyResultLink(row)).Click();
        }

        public List<string> GetLinkedKeyResultsColumnName()
        {
            return Driver.GetTextFromAllElements(DynamicLinkedKeyResultColumnName).ToList();
        }

        public List<KeyResult> GetKeyResult()
        
        {
            Log.Step(nameof(KeyResultsTabPage), $"Get the added Key Results");
            var keyResults = new List<KeyResult>();

            var keyResultRows = Wait.UntilAllElementsLocated(KeyResultsRows);

            for (var i = 0; i < keyResultRows.Count; i++)
            {
                try
                {
                    // Get Key Result Title
                    var keyResultTitle = keyResultRows[i].FindElement(DynamicKeyResultName(i + 1)).GetText();
                    var weightElement = keyResultRows[i].FindElement(DynamicKeyResultWeight(i)).GetText();
                    var keyResultWeight = double.Parse(weightElement);

                    var keyResultMetricValue = keyResultRows[i].FindElement(MetricValue(i + 1)).GetText();
                    var keyResultStart = keyResultRows[i].FindElement(DynamicKeyResultStart(i + 1)).GetAttribute("value");

                    // Get Key Result Target
                    var keyResultTarget = keyResultRows[i].FindElement(DynamicKeyResultGoal(i + 1)).GetAttribute("value");

                    // Get Key Result Stretch
                    var keyResultStretch = keyResultRows[i].FindElement(DynamicKeyResultStretch(i + 1)).GetAttribute("value");

                    // Get Key Result Current
                    var current = keyResultRows[i].FindElement(DynamicKeyResultCurrent(i + 1)).GetAttribute("value");
                    var keyResultCurrent = double.Parse(current);

                    // Get Key Result Progress
                    var keyResultProgress = keyResultRows[i].FindElement(DynamicKeyResultProgressBar(i + 1)).GetAttribute("aria-valuenow");
                    // Get Key Result Progress Bar Percentage 
                    var progressBarPercentage = keyResultRows[i].FindElement(DynamicKeyResultProgressPercentage(i + 1)).GetText();

                    // Add the key result to the list
                    keyResults.Add(new KeyResult
                    {
                        Title = keyResultTitle,
                        Weight = keyResultWeight,
                        Metric = new Metric() { Name = keyResultMetricValue },
                        Start = keyResultStart,
                        Target = keyResultTarget,
                        Progress = keyResultCurrent,
                        Stretch = keyResultStretch,
                        ProgressBar = keyResultProgress,
                        ProgressBarPercentage = progressBarPercentage
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error extracting data from Key Result row: " + ex.Message);
                }
            }

            return keyResults;
        }
        private int GetKeyResultIndexByTitle(string title)
        {
            var name = Wait.UntilElementExists(KeyResultTitle(title)).GetAttribute("name");
            return name.Replace("keyResults[", "").Replace("].title", "").ToInt() + 1;
        }

        public void SelectMetric(int keyResultRow, string value)
        {
            Log.Step(nameof(KeyResultsTabPage), $"Select result metric <{value}> for key result <{keyResultRow}>");
            Wait.UntilJavaScriptReady();
            SelectItem(DynamicKeyResultMetricDropdown(keyResultRow), DynamicKeyResultMetricInput(value));
        }

        public int GetTotalNumberOfKeyResultRows() => Driver.GetElementCount(KeyResultsRows);

        public void DeleteKeyResult(string title)
        {
            Log.Step(nameof(KeyResultsTabPage), $"Click on 'Delete' button for key result <{title}>");
            Driver.JavaScriptScrollToElement(KeyResultActionKebabMenu(title)).Click();
            Driver.JavaScriptScrollToElement(KeyResultDeleteButton);
            Driver.JavaScriptClickOn(Wait.UntilElementClickable(KeyResultDeleteButton));
        }

        public bool IsKeyResultLinkedIconDisplayed(string keyResultTitle)
        {
            return Driver.IsElementDisplayed(ParentKeyResultLinkedIcon(keyResultTitle));
        }

        public void ClickOnUnlinkKeyResultButton(string keyResultTitle)
        {
            Log.Step(nameof(KeyResultsTabPage), "Click on 'UnLink' button for KeyResult");
            Wait.UntilElementClickable(KeyResultActionKebabMenu(keyResultTitle)).Click();
            Wait.UntilElementClickable(KeyResultMoreIconUnlinkButton).Click();
        }

        public void ClickOnConfirmPopupUnlinkButton()
        {
            Log.Step(nameof(KeyResultsTabPage), "Click on 'Unlink' button from Confirm popup");
            Wait.UntilElementClickable(UnlinkConfirmationPopupUnlinkButton).Click();
        }

        public void ClickOnLinkToOutcomeKeyResultButton(string keyResultTitle)
        {
            Log.Step(nameof(KeyResultsTabPage), "Click on 'Link to Outcome' button for KeyResult");
            Wait.UntilElementClickable(KeyResultActionKebabMenu(keyResultTitle)).Click();
            Wait.UntilElementClickable(KeyResultMoreIconLinkToOutcomeButton).Click();
        }

        public bool IsKeyResultTitleDisplayed(string keyResultTitle)
        {
            return Driver.IsElementDisplayed(UnlinkConfirmationPopupKeyResultTitle(keyResultTitle));
        }

        public void ClickOnLinkedKrLinkedIcon(string keyResultTitle)
        {
            Log.Step(nameof(KeyResultsTabPage), "Click on 'Linked Kr' icon for KeyResult");
            Wait.UntilElementClickable(ParentKeyResultLinkedIcon(keyResultTitle)).Click();
        }

        public void ClickOnKeyResultExpandIcon(string keyResultTitle)
        {
            Log.Step(nameof(KeyResultsTabPage), "Click on key result 'Caret' button to expand or collapse");
            if (Wait.UntilElementVisible(KeyResultsExpandCollapseIcon(keyResultTitle)).GetAttribute("data-testid") == "KeyboardArrowDownIcon")
            {
                Wait.UntilElementClickable(KeyResultsExpandCollapseIcon(keyResultTitle)).Click();
            }
        }

        public bool IsKeyResultsCaretIconDisplayed(string keyResultTitle)
        {
            return Driver.IsElementDisplayed(KeyResultsCaretIconButton(keyResultTitle));
        }

        public bool IsInitiativeCardNamePresent(string cardTitle)
        {
            return Driver.IsElementPresent(KeyResultInitiativeCardName(cardTitle));
        }

        #endregion

        #region Metrics
        public void AddNewMetric(string keyResultTitle, BusinessOutcomeMetricRequest request)
        {
            var keyResultRow = GetKeyResultIndexByTitle(keyResultTitle);
            Log.Step(nameof(KeyResultsTabPage), $"Add new metric type to KR <{keyResultTitle}> with name <{request.Name}> and type <{request.TypeId}>");

            Driver.JavaScriptScrollToElement(DynamicKeyResultMetricDropdown(keyResultRow)).SendKeys(request.Name);
            Wait.UntilElementClickable(DynamicKeyResultMetricInput(request.Name)).Click();
            Wait.UntilElementExists(MetricPopUpHeaderText);

            Driver.JavaScriptScrollToElement(MetricTypeDropdown).Click();
            Wait.UntilJavaScriptReady();

            Wait.UntilElementExists(MetricDropdownValues(((MetricType)request.TypeId).ToString("G"))).Click();
            Wait.UntilJavaScriptReady();

            Wait.UntilElementClickable(MetricSaveButton).Click();
            Wait.UntilElementVisible(BusinessOutcomeLeyMetricSaveAlertDialog);
            Wait.UntilElementNotExist(BusinessOutcomeLeyMetricSaveAlertDialog);
        }

        public bool IsMetricTypePopUpPresent()
        {
            return Driver.IsElementDisplayed(MetricPopUpHeaderText);
        }


        public string GetOverallProgressInfo()
        {
            return Math.Round(
                double.TryParse(
                    Driver.JavaScriptScrollToElement(Wait.UntilElementExists(ProgressBar))
                        .GetAttribute("aria-valuenow"),
                    out var progress) ? progress : 0,
                MidpointRounding.AwayFromZero).ToString(CultureInfo.InvariantCulture);

        }

        public string GetOverallPercentageInfo()
        {
            return Driver.JavaScriptScrollToElement(Wait.UntilElementExists(Percentage)).GetText();
        }
        #endregion

        #region SelectParentOutcome

        public void SelectParentOutcome(string parentOutcome)
        {
            Log.Step(nameof(KeyResultsTabPage), "Click on 'Select Parent Outcome' Dropdown and select parent card");
            Wait.UntilElementExists(SelectParentOutcomeDropdown).Click();
            Wait.UntilElementExists(SelectParentOutcomeDropdown).SetText(parentOutcome);
            Wait.HardWait(2000);// need to wait till load the data
            SelectItem(SelectParentOutcomeDropdown, SelectParentOutcomeDropdownItem(parentOutcome));
        }

        #endregion

        #region KeyResult Aligning/Contributing Confirmation Popup

        public string GetKeyResultLockUnlockPopupConfirmationTitle()
        {
            return Wait.UntilElementVisible(KeyResultLockUnLockPopUpTitle).GetText();
        }

        public string GetKeyResultLinkToOutcomeAligningContributingConfirmationPopupDescription()
        {
            var innerHtml = Wait.UntilElementVisible(KeyResultsLinkedToOutcomeAligningConfirmationPopupDescription).GetAttribute("innerHTML");
            innerHtml = innerHtml.Replace("<br>", "\n").Replace("<br/>", "\n").Replace("<br />", "\n");

            // Remove remaining HTML tags using Regex
            return Regex.Replace(innerHtml, "<.*?>", "").Trim();
        }

        public string GetKeyResultLinkToOutcomeAligningContributingConfirmationPopupLearnMoreLink()
        {
            return Wait.UntilElementVisible(KeyResultsLinkedToOutcomeAligningConfirmationPopupLearnMoreLink).GetAttribute("href");
        }

        #endregion

        #region Lock Button
        public bool IsLockIconDisplayed()
        {
            return Driver.IsElementDisplayed(KeyResultUnlockedButton);
        }

        public void ClickKeyResultLockIcon(bool unlock = true)
        {
            Log.Step(nameof(BaseTabPage), "Click on Key Result Lock Button");
            var lockButton = unlock ? KeyResultUnlockedButton : KeyResultLockedButton;
            Wait.UntilElementClickable(lockButton).Click();
        }

        public string GetKeyResultLinkToOutcomeAligningContributingConfirmationPopupTitle()
        {
            return Wait.UntilElementVisible(KeyResultsLinkedToOutcomeAligningConfirmationPopupTitle).GetText();
        }

        public string GetKeyResultLockUnlockPopupConfirmationDescription()
        {
            return Wait.UntilElementVisible(KeyResultLockUnLockPopUpDescription).GetText();
        }

        public string GetKeyResultLockStatusTooltipTitle(bool unlock = true)
        {
            Log.Info("Hover on the 'Lock' icon");
            var tooltipTitle = (string)Driver.JsExecutor().ExecuteScript("return document.querySelector('[data-icon*=\"lock\"] title').textContent;");
            return tooltipTitle;
        }

        public string GetKeyResultIconStatus(bool unlock = true)
        {
            var lockIconStatus = unlock ? Wait.UntilElementVisible(KeyResultUnlockedButton).GetAttribute("class") : Wait.UntilElementVisible(KeyResultLockedButton).GetAttribute("class");
            return lockIconStatus.Contains("lock-open") ? "UnLocked" : "Locked";
        }

        public bool IsLockToUnlockImageDisplayed()
        {
            return Driver.IsElementDisplayed(KeyResultLockToUnlockImage);
        }

        public bool IsUnlockToLockImageDisplayed()
        {
            return Driver.IsElementDisplayed(KeyResultUnLockToLockImage);
        }
        public bool IsConfirmButtonDisplayed()
        {
            return Driver.IsElementDisplayed(ConfirmButton);
        }

        public bool IsCancelButtonDisplayed()
        {
            return Driver.IsElementDisplayed(CancelButton);
        }

        public void ClickOnConfirmButton()
        {
            Wait.UntilElementClickable(ConfirmButton).Click();
        }

        public void ClickOnCancelButton()
        {
            Wait.UntilElementClickable(CancelButton).Click();
        }
        public bool IsKeyResultAddButtonDisabled()
        {
            var buttonElement = Wait.UntilElementVisible(KeyResultAddButton);

            // Check if the button contains the 'Mui-disabled' class or 'disabled' attribute
            var isDisabled = buttonElement.GetAttribute("class").Contains("Mui-disabled") || buttonElement.GetAttribute("disabled") != null;

            return isDisabled;
        }

        // Method to return whether the key result fields are enabled/disabled
        public bool AreKeyResultFieldsDisabled(bool align = false)
        {

            // Check if the start field is non-editable (read-only)
            var isKeyResultStartDisabled = Driver.FindElement(DynamicKeyResultStart(1)).GetAttribute("contenteditable") != "true";

            // Check if the goal field is non-editable (read-only)
            var isKeyResultGoalDisabled = Driver.FindElement(DynamicKeyResultGoal(1)).GetAttribute("contenteditable") != "true";

            // Check if the current field is editable (should be true for enabled)
            var isKeyResultCurrentEnabled = Driver.FindElement(DynamicKeyResultGoal(1)).GetAttribute("contenteditable") == "true";

            // Check if the name field is disabled (not displayed is equivalent to being disabled for this context)
            var isKeyResultNameDisabled = !Driver.IsElementDisplayed(DynamicKeyResultName(1));

            // Check if the metric dropdown is displayed (should be hidden or disabled for this case)
            var isKeyResultMetricDisplayed = !Driver.IsElementDisplayed(DynamicKeyResultMetricDropdown(1));

            // Return true only if all conditions match expected values
            if (!align)
                return isKeyResultNameDisabled && isKeyResultMetricDisplayed && isKeyResultStartDisabled &&
                       isKeyResultGoalDisabled && !isKeyResultCurrentEnabled;
            //isKeyResultCurrentEnabled = Driver.FindElement(DynamicKeyResultGoal(1)).GetAttribute("contenteditable") != "true";
            return isKeyResultNameDisabled && isKeyResultMetricDisplayed && isKeyResultStartDisabled && isKeyResultGoalDisabled && !isKeyResultCurrentEnabled;
        }
        #endregion

        #region Action 

        public bool IsKeyResultMoreDeleteDisplayed()
        {
            return Driver.IsElementDisplayed(KeyResultDeleteButton);
        }

        public void ClickOnKeyResultsMoreButton(string keyResultTitle)
        {
            Log.Step(nameof(KeyResultsTabPage), "Click on 'UnLink' button for KeyResult");
            Driver.JavaScriptScrollToElement(KeyResultActionKebabMenu(keyResultTitle));
            Wait.UntilElementClickable(KeyResultActionKebabMenu(keyResultTitle)).Click();
        }
        public bool IsKeyResultMoreLinkToOutcomeDisplayed()
        {
            return Driver.IsElementDisplayed(KeyResultMoreIconLinkToOutcomeButton);
        }

        public void ClickOnActionLinkToOutcomeButton()
        {
            Log.Step(nameof(KeyResultsTabPage), "Click on 'Link to Outcome' button");
            Wait.UntilElementClickable(KeyResultMoreIconLinkToOutcomeButton).Click();
        }
        public bool IsKeyResultMoreViewDetailsDisplayed()
        {
            return Driver.IsElementDisplayed(KeyResultActionKebabViewDetails);
        }
        #endregion

    }
}
