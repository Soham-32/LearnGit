using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.CardType;
using AgilityHealth_Automation.Utilities;
using AtCommon.Dtos.BusinessOutcomes;
using AtCommon.Utilities;
using OpenQA.Selenium;
using System.Collections.Generic;
using System;
using System.Linq;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes
{
    public class BusinessOutcomeBasePage : BasePage
    {
        public BusinessOutcomeBasePage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }
        #region Locators

        private readonly By LoadingSpinner = By.XPath("//p[contains(normalize-space(), 'Loading')]//img[contains(@src, 'load') and contains(@src, '.gif')]");//

        private readonly By CardViewPageColumns = By.XPath("//div[@id='swimlanesWrapper']/div/div");
        private readonly By ExportToExcelButton = By.XPath("//button[contains(text(),'Export To Excel')]");
        private readonly By CardTypeDropDownOptions = By.XPath("(//*[local-name()='svg' and @data-testid='ArrowDropDownIcon']/preceding-sibling::div[@aria-haspopup='listbox'])[last()]");


        #region BusinessOutcome Dashboard Header
        private readonly By OverallPerformanceTab = AutomationId.Equals("business-outcomes-Overall-Performance-Tab");
        private readonly By OutputsProjectTab = AutomationId.Equals("bo-overallprogress-page_overall_performance_tab_output-Tab");
        private static By CompanyNameFromCardTitle(string companyName) => By.XPath($"(//div[@class='MuiBox-root css-0']//div[contains(@class,'k-card-title jss')]//span[contains(text(),'{companyName}')])");
        private readonly By FinancialsTabOnOverallPerformanceTab = AutomationId.Equals("bo-overallprogress-financialsGridView.financials-Tab");
        private readonly By CurrentFinancialProgressText = By.XPath("//p[text() ='Current Financial Progress']");
        private readonly By MaturityTab = AutomationId.Equals("bo-overallprogress-page_overall_performance_tab_maturity-Tab");
        private readonly By CardViewTab = AutomationId.Equals("business-outcomes-Card-View-Tab");
        private readonly By OutcomeTreeViewIdText = By.XPath("//a[@aria-label='ID Column menu']//preceding-sibling::span//span[contains(@class,'k-column-title')]");
        private readonly By ViewTab = By.XPath("//a[@value='gridview']");
        private readonly By MeetingNotesTab = By.XPath("//div[@aria-label='Meeting Notes']");
        private readonly By FinancialsTab = By.XPath("//a[@value='financials']");
        #endregion

        #region BusinessOutcomeForm

        //Business Outcome form
        private readonly By BusinessOutcomeForm = By.CssSelector("form#businessOutcomeForm");
        private readonly By BusinessOutcomeFormSubHeader = By.CssSelector("#businessOutcomeForm > div");
        private readonly By BusinessOutcomeMinimizeForm = By.ClassName("MuiDialog-paperFullWidth");
        private readonly By BusinessOutcomeMaximizeForm = By.ClassName("MuiDialog-paperFullScreen");
        private readonly By BusinessOutcomeAlertDialog = By.XPath("//div[@role='alert']//div[contains(normalize-space(),'Successfully saved')]");
        private readonly By DefaultCardLabelTitle = By.XPath("//div[@id = 'swimlanesWrapper']//h1");
        private static By TabName(string tabName) => By.XPath($"//div[@role='tablist']/button[contains(normalize-space(),'{tabName}')]");
        private static By BusinessOutcomeCardTabs() => By.XPath("(//div[@role='tablist'])[2]/button");
        #endregion

        #region DeleteAndCreationInfo

        //Delete
        private readonly By DeleteButton = By.CssSelector("button[title='Delete Business Outcome']");
        private readonly By DeletePopUpDeleteButton = By.CssSelector("button[label='Delete Card']");
        private readonly By DeletePopupCancelButton = By.CssSelector("button[label='Cancel']");

        //Creation Info
        private readonly By CreatedInfoSpan = By.XPath("//form[@id='businessOutcomeForm']//p[contains(normalize-space(),'Created on')]/parent::div//span");
        private readonly By UpdatedInfoSpan = By.XPath("//form[@id='businessOutcomeForm']//p[contains(normalize-space(),'Last updated on')]/parent::div//span");


        #endregion

        #region AddCard

        //Add
        private readonly By Title = By.XPath("//p[@id='title-translation'] | //p[@id='title-translation']//font | //textarea[@name='title']");
        private readonly By TitleTextArea = By.XPath("//p[contains(@id,'translation')]");
        private readonly By TitleValidationText = By.CssSelector("p.Mui-error");
        private readonly By CardDescription = By.Id("editorContainer");
        private readonly By DescriptionIframe = By.XPath("//iframe[contains(@title,'Editable')]");
        private readonly By DescriptionArea = By.XPath("//div[contains(@class,'k-content')]");
        private readonly By ColorDropdown = By.Id("color__picker__button");
        private static By ColorSelection(string colorHexValue) => By.CssSelector($"div[title='Select {colorHexValue}']");
        private readonly By DemandTagsInput = By.XPath("//div//*[text()='Select Tags']");
        private static By Tags(string tagName) => By.XPath($"//*[text()='{tagName}']//ancestor::p//preceding-sibling::span/input");
        private readonly By CardTagCheckBox = By.XPath("//*[contains(@class, 'MuiCheckbox-root')]");
        private readonly By CardStartDate = By.XPath("//div[@id = 'datePickerContainerstart_date']//div//input[@placeholder='mm/dd/yyyy']");
        private readonly By CardEndDate = By.XPath("//div[@id = 'datePickerContainerend_date']//div//input[@placeholder='mm/dd/yyyy']");
        private readonly By CardSelectOwnersDropdown = By.XPath("//div[@id = 'user-autocomplete']/p[text()='Select owner(s)']");
        private readonly By CardSearchOwners = By.XPath("//div[@class= 'MuiFormControl-root MuiTextField-root css-i44wyl']//input[@placeholder='Search']");
        private static By CardOwnerCheckBox(string name) => By.XPath($"//p[text()='{name}']/../span/input");


        #endregion

        #region KeyResultsTab
        private static By DynamicKeyResultName(int rowIndex) => By.XPath($"(//tbody[@data-rbd-droppable-id='BO-Key-Results']/tr/td[2]//p)[{rowIndex}]");
        private static By DynamicKeyResultMetricDropdown(int rowIndex) => By.XPath($"(//tbody[@data-rbd-droppable-id='BO-Key-Results']/tr/td[2]//p)[{rowIndex}]/ancestor::tr/td[5]/p");
        private static By DynamicKeyResultStart(int rowIndex) => By.Name($"keyResults[{rowIndex - 1}].start");
        private static By DynamicKeyResultGoal(int rowIndex) => By.Name($"keyResults[{rowIndex - 1}].target");
        private readonly By ExistingKeyResultTextArea = By.XPath("//div//p[contains(@id,'text-item-translation')]");
        private readonly By KeyResultsRowsForNoRecord = By.XPath("//table[contains(@class,'key__results')]//td[(contains(normalize-space(),'No records available'))]");
        private readonly By KeyResultsRows = By.CssSelector("table.key__results tbody tr");
        private static By DynamicKeyResultActionButton(string keyResultName) => By.XPath($"//tr[.//textarea[text()='{keyResultName}']]//button[contains(@class, 'keyresults__actionmenu')]");
        private static By KeyResultActions(string action) => By.XPath($"//li[text()='{action}']");

        #endregion

        #region Action

        //Action
        private readonly By SaveButton = By.XPath("//*[contains(text(),'Save')]");
        private readonly By DetailSaveButton = By.XPath("//span[text()='Save']");
        private readonly By SaveAndCloseButton = By.XPath("//li[text()='Save & Close']");
        private readonly By SaveDropdownButton = By.CssSelector("button#project-save-actions");
        private readonly By SaveDropdown = By.Id("project-save-actions");

        private readonly By CloseIcon = By.CssSelector("form#businessOutcomeForm button[title='Close']");
        private readonly By MaximizeIcon = By.CssSelector("form#businessOutcomeForm button[title='Enter Fullscreen mode']");
        private readonly By MinimizeIcon = By.CssSelector("form#businessOutcomeForm button[title='Exit Fullscreen mode']");
        private readonly By OutcomeTreeViewTab = By.Id("id_tab_header_treeview");
        private readonly By OutcomeViewLoader = By.XPath("//p[text()='Loading Tree View...']");
        private readonly By SearchCard = By.XPath("//input[@placeholder='Search Cards']");
        #endregion

        #region ConfirmPopup

        //Confirm pop up
        private readonly By ConfirmSave = By.ClassName("save__confirmchanges");
        private readonly By ConfirmDiscard = By.ClassName("discard__confirmchanges");
        private readonly By ConfirmCancel = By.ClassName("cancel__confirmchanges");

        //Pendo Pop up
        private readonly By PendoCloseIcon = By.XPath("//div[@id='pendo-guide-container']//button[@class='_pendo-close-guide']");
        #endregion


        #endregion

        #region Methods
        public void FillOutBusinessOutcomeCardDetails(BusinessOutcomeRequest card)
        {
            Log.Step(nameof(BusinessOutcomeBasePage), "Enter new business outcome info");
            EnterTitle(card.Title);
            EnterDescription(card.Description);
            SetCardStartDate(DateTime.Today.ToString("MM/dd/yyyy"));
            SetCardEndDate(DateTime.Today.AddDays(2).ToString("MM/dd/yyyy"));
        }

        public void FillOutChildCardDetails(BusinessOutcomeRequest cardDetails)
        {
            Log.Step(nameof(BusinessOutcomeBasePage), "Enter new business outcome info");
            EnterTitle(cardDetails.Title);
            EnterDescription(cardDetails.Description);
            SelectCardTag();
            SetCardStartDate(DateTime.Today.ToString("MM/dd/yyyy"));
            SetCardEndDate(DateTime.Today.AddDays(2).ToString("MM/dd/yyyy"));
            Wait.HardWait(2000); // wait until error popup closed

        }

        public void ClickOnOutcomeTreeViewTab()
        {
            Log.Step(nameof(BusinessOutcomeBasePage), "Click on Outcome Tree View Tab");
            Wait.UntilElementClickable(OutcomeTreeViewTab).Click();
            Wait.UntilElementNotExist(OutcomeViewLoader);
        }
        public void CardSearch(string name)
        {
            Log.Step(nameof(BusinessOutcomeBasePage), "Search a card in Card view page");
            Wait.UntilElementVisible(SearchCard).ClearTextbox();
            Wait.UntilElementVisible(SearchCard).SetText(name);
            Wait.UntilJavaScriptReady();
        }
        public void WaitUntilBusinessOutcomesPageLoaded()
        {
            Log.Step(nameof(BusinessOutcomeCardPage), "Wait until Business Outcome page is loaded");
            Wait.UntilElementNotExist(LoadingSpinner);
            Wait.UntilElementVisible(CardViewPageColumns);
        }

        public List<string> GetBusinessOutcomesHeaderTabs()
        {
            Log.Step(nameof(BusinessOutcomeCardPage), "Get all Business Outcome Header Tabs");
            var headerTexts = new List<string>
            {
                Wait.UntilElementVisible(OverallPerformanceTab).GetText(),
                Wait.UntilElementVisible(CardViewTab).GetText(),
                Wait.UntilElementVisible(ViewTab).GetText(),
                Wait.UntilElementVisible(MeetingNotesTab).GetText(),
            };
            if (Driver.IsElementPresent(FinancialsTab))
            {
                Wait.UntilElementVisible(FinancialsTab).GetText();
            }
            return headerTexts;
        }

        public void ClickOnOverallPerformanceTab()
        {
            Log.Step(nameof(BusinessOutcomeCardPage), "Click On Overall Performance tab");
            var overallPerformanceTab = Driver.FindElement(OverallPerformanceTab);
            ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].scrollIntoView({block: 'center'});", overallPerformanceTab);
            Wait.HardWait(300); // Allow time for page load
            overallPerformanceTab.Click();
            Wait.HardWait(2000); // Allow time for tab content to load
        }
        public void ClickOnCardViewTab()
        {
            Log.Step(nameof(BusinessOutcomeCardPage), "Click On card view tab");
            Wait.UntilElementClickable(CardViewTab).Click();
        }

        public string GetCompanyNameFromCardTitle(string companyName)
        {
            Log.Step(nameof(BusinessOutcomeCardPage), "Get company name from card title");
            return Wait.UntilElementVisible(CompanyNameFromCardTitle(companyName)).GetText();
        }

        public void ClickOnOutputsProjectsTab()
        {
            Log.Step(nameof(BusinessOutcomeCardPage), "Click On Outputs/Projects tab");
            Wait.UntilElementClickable(OutputsProjectTab).Click();
        }

        public void ClickOnFinancialsTabOnOverallPerformanceTab()
        {
            Log.Step(nameof(BusinessOutcomeCardPage), "Click On Financials tab form the overall performance tab");
            Wait.UntilElementClickable(FinancialsTabOnOverallPerformanceTab).Click();
        }

        public string GetCurrentFinancialProgressText()
        {
            Log.Step(nameof(BusinessOutcomeCardPage), "Get Current Financial Progress text from the Financial tab");
            return Wait.UntilElementVisible(CurrentFinancialProgressText).GetText();
        }

        public bool IsMaturityTabDisplayed()
        {
            return Driver.IsElementDisplayed(MaturityTab);
        }

        public void ClickOnMaturityTab()
        {
            Log.Step(nameof(BusinessOutcomeCardPage), "Click On Maturity tab");
            Wait.UntilElementClickable(MaturityTab).Click();
        }

        public void ClickOnFinancialTab()
        {
            Log.Step(nameof(BusinessOutcomeCardPage), "Click on Financial Tab");
            Wait.UntilElementClickable(FinancialsTab).Click();
        }

        public string GetIdTextFromOutcomeTreeViewTab()
        {
            Log.Step(nameof(BusinessOutcomeCardPage), "Get Id text from the Outcome Tree View tab");
            return Wait.UntilElementVisible(OutcomeTreeViewIdText).GetText();
        }

        public void ClickOnGridViewTab()
        {
            Log.Step(nameof(BusinessOutcomeCardPage), "Click On GridView Tab");
            Wait.UntilElementClickable(ViewTab).Click();
            Wait.UntilJavaScriptReady();
        }

        public void ClickOnExportToExcelButton()
        {
            Log.Step(nameof(BusinessOutcomeCardPage), "Click On 'Export to Excel' button");
            Driver.JavaScriptScrollToElementCenter(ExportToExcelButton);
            Wait.UntilElementClickable(ExportToExcelButton).Click();
            Wait.HardWait(3000);// Added weight as Excel file are having same name in parallel run
        }

        public void ClickOnCardTypeDropdown()
        {
            Log.Step(nameof(BusinessOutcomeCardPage), "Click On Filter dropdown Tab");
            Wait.UntilElementClickable(CardTypeDropDownOptions).Click();
        }

        public void ClickOnMeetingNotes()
        {
            Log.Step(nameof(BusinessOutcomeCardPage), "Click on Meeting Notes");
            Wait.UntilElementClickable(MeetingNotesTab).Click();
        }
        #region BusinessOutcomeForm
        public bool IsBusinessOutcomeFormDisplayed()
        {
            return Driver.IsElementDisplayed(BusinessOutcomeForm);
        }

        public string GetCardColor()
        {
            return CSharpHelpers.ConvertRgbToHex(Wait.UntilElementVisible(BusinessOutcomeFormSubHeader).GetCssValue("border-left-color"));
        }

        public string MaximizeFormGetCssValue(string cssProperty)
        {
            return Wait.UntilElementClickable(BusinessOutcomeMaximizeForm).GetCssValue(cssProperty);
        }

        public string MinimizeFormGetCssValue(string cssProperty)
        {
            return Wait.UntilElementClickable(BusinessOutcomeMinimizeForm).GetCssValue(cssProperty);
        }

        public List<string> GetAllTabsName()
        {
            return Driver.GetTextFromAllElements(BusinessOutcomeCardTabs()).ToList();
        }

        public void ClickOnTab(string tabName)
        {
            Log.Step(nameof(BusinessOutcomeCardPage), $"Click on tab: {tabName}");
            Driver.FindElements(TabName(tabName)).Last(element => element.Displayed && element.Enabled).Click();
            Wait.HardWait(2000); //Wait for tab elements to load
        }

        #endregion

        #region DeleteBusuinessOutcome

        public string GetDefaultCardLabelTitle()
        {
            return Wait.UntilElementVisible(DefaultCardLabelTitle).GetText();
        }

        //Delete
        public void ClickOnDeleteButton()
        {
            Log.Step(nameof(BusinessOutcomeCardPage), "Click on 'Delete' button");
            Driver.JavaScriptScrollToElement(DeleteButton, false).Click();
            Wait.UntilJavaScriptReady();
        }

        public void DeletePopUp_ClickOnDeleteCardButton()
        {
            Log.Step(nameof(BusinessOutcomeCardPage), "Click on 'Delete' button on confirmation popup");
            Wait.UntilElementClickable(DeletePopUpDeleteButton).Click();
            Wait.UntilElementNotExist(DeletePopUpDeleteButton);
            Wait.HardWait(3000); // It takes time delete the card
        }

        public void DeletePopUp_ClickOnCancelButton()
        {
            Log.Step(nameof(BusinessOutcomeCardPage), "Click on 'Cancel' button on confirmation popup");
            Wait.UntilElementClickable(DeletePopupCancelButton).Click();
            Wait.UntilElementNotExist(DeletePopupCancelButton);
        }

        #endregion

        #region AddBusinessOutcome

        public void FillForm(BusinessOutcomeRequest request)
        {
            Log.Step(nameof(BusinessOutcomeCardPage), "Enter new business outcome info");
            EnterTitle(request.Title);
            EnterDescription(request.Description);

            if (request.Tags.Count != 0)
            {
                Wait.UntilElementClickable(DemandTagsInput).Click();
                foreach (var tag in request.Tags)
                {
                    Wait.UntilElementExists(Tags(tag.Name)).Click();
                }
                Wait.UntilElementExists(Tags(request.Tags.First().Name)).SendKeys(Keys.Escape);
            }

            if (!string.IsNullOrEmpty(request.CardColor))
            {
                Wait.UntilElementVisible(ColorDropdown).Click();
                Wait.UntilJavaScriptReady();
                Wait.UntilElementVisible(ColorSelection(request.CardColor.ToUpper())).Click();
                Wait.UntilJavaScriptReady();
            }

        }

        public void EnterDescription(string description)
        {
            Log.Step(nameof(BusinessOutcomeCardPage), "Enter description");
            if (Driver.IsElementPresent(CardDescription))
            {
                Wait.UntilElementClickable(CardDescription).Click();
            }
            Driver.SwitchToFrame(DescriptionIframe);
            Wait.UntilElementVisible(DescriptionArea).ClearTextbox(); // At times Clear text doesn't work from SetText method, so putting 1 more to avoid false failures
            Wait.UntilElementVisible(DescriptionArea).SetText(description);
            Driver.SwitchTo().DefaultContent();
            Wait.UntilJavaScriptReady();
        }

        public void EnterTitle(string titleText)
        {
            Log.Step(nameof(BusinessOutcomeCardPage), "Enter title");
            if (Driver.IsElementPresent(TitleTextArea))
            {
                Wait.UntilElementClickable(TitleTextArea).Click();
            }
            Wait.UntilElementVisible(Title).ClearTextbox(); // At times Clear text doesn't work from SetText method, so putting 1 more to avoid false failures
            Wait.UntilElementClickable(Title).SetText(titleText, isReact: true);
            Wait.UntilJavaScriptReady();
        }

        public string GetDescriptionText()
        {
            if (Driver.IsElementPresent(CardDescription))
            {
                return Wait.UntilElementExists(CardDescription).GetText();
            }
            Driver.SwitchToFrame(DescriptionIframe);
            var a = Wait.UntilElementVisible(DescriptionArea).GetText();
            Driver.SwitchTo().DefaultContent();
            return a;
        }

        public string GetTitleValidationText()
        {
            return Wait.UntilElementExists(TitleValidationText).GetText();
        }

        public string GetTitleText()
        {
            return Wait.UntilElementVisible(Title).GetText();
        }
        public void SetCardStartDate(string date)
        {
            Log.Step(nameof(BusinessOutcomeCardPage), "Enter start date");
            var startDateElement = Driver.FindElement(CardStartDate);
            startDateElement.Clear();
            startDateElement.SendKeys(date);
            startDateElement.SendKeys(Keys.Enter);
        }
        public void SetCardEndDate(string date)
        {
            Log.Step(nameof(BusinessOutcomeCardPage), "Enter end date");
            var endDateElement = Driver.FindElement(CardEndDate);
            Wait.UntilElementExists(CardEndDate).Clear();
            Wait.UntilElementExists(CardEndDate).SendKeys(date);
            endDateElement.SendKeys(Keys.Enter);
        }
        public void SetCardOwner(string ownerName)
        {
            Log.Step(nameof(BusinessOutcomeCardPage), "Select Owner name");
            Driver.JavaScriptScrollToElementCenter(CardSelectOwnersDropdown).Click();
            Wait.UntilElementClickable(CardSearchOwners).SendKeys(ownerName);
            Wait.HardWait(3000);  // Wait until search value displayed
            Wait.UntilElementExists(CardOwnerCheckBox(ownerName)).Check();
            var action = new Actions(Driver);
            action.SendKeys(Keys.Escape).Perform();
        }

        public void SelectCardTag()
        {
            Log.Step(nameof(BusinessOutcomeCardPage), "Select Tag");
            Wait.UntilElementClickable(DemandTagsInput).Click();
            var firstTag = Driver.FindElements(CardTagCheckBox).FirstOrDefault();
            firstTag?.Click();
            var action = new Actions(Driver);
            action.SendKeys(Keys.Escape).Perform();
        }


        #endregion

        #region GetBusinessOutcome
        public BusinessOutcomeResponse GetBusinessOutcomeInfo()
        {
            var createdText = Driver.FindElements(CreatedInfoSpan).Select(element => element.Text.Trim()).ToList();
            var updatedText = Driver.FindElements(UpdatedInfoSpan).Select(element => element.Text.Trim()).ToList();
            var keyResults = new List<KeyResultResponse>();
            var rowIndex = Driver.IsElementPresent(KeyResultsRowsForNoRecord) ? 0 : Driver.GetElementCount(KeyResultsRows);

            for (var i = 1; i <= rowIndex; i++)
            {
                if (Driver.IsElementPresent(ExistingKeyResultTextArea))
                {
                    Wait.UntilElementClickable(ExistingKeyResultTextArea).Click();
                }
                keyResults.Add(new KeyResultResponse()
                {
                    Title = Wait.UntilElementExists(DynamicKeyResultName(i)).Text,
                    Metric = new BusinessOutcomeMetricResponse()
                    {
                        Name = Wait.UntilElementExists(DynamicKeyResultMetricDropdown(i)).Text
                    },
                    Start = Wait.UntilElementExists(DynamicKeyResultStart(i)).GetAttribute("data-value"),
                    Target = Wait.UntilElementExists(DynamicKeyResultGoal(i)).GetAttribute("data-value")

                });
            }

            return new BusinessOutcomeResponse
            {
                Title = Wait.UntilElementVisible(Title).GetText(),
                Description = GetDescriptionText(),
                CardColor = GetCardColor(),
                Owner = createdText[2],
                CreatedAt = DateTime.Parse(createdText[0]),
                UpdatedBy = updatedText[2],
                UpdatedAt = DateTime.Parse(updatedText[0]),
                KeyResults = keyResults
            };
        }


        #endregion

        #region Actions

        //Action
        public void ClickOnSaveButton(bool waitForConfirmationDialog = true)
        {
            Log.Step(nameof(BusinessOutcomeCardPage), "Click on 'Save' button");
            Driver.JavaScriptScrollToElement(SaveButton).Click();
            Wait.HardWait(3000);// need to wait till updated card shows on dashboard
            if (!waitForConfirmationDialog) return;
            Wait.UntilElementVisible(BusinessOutcomeAlertDialog);

        }
        public void ClickOnDetailSaveButton()
        {
            Log.Step(nameof(BusinessOutcomeCardPage), "Click on 'Save' button");
            Driver.JavaScriptScrollToElement(DetailSaveButton).Click();
            Wait.HardWait(3000);// need to wait till updated card shows on dashboard

        }

        public void ClickOnSaveAndCloseButton()
        {
            Log.Step(nameof(BusinessOutcomeCardPage), "Click on 'Save & Close' button in the dropdown");

            var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(10));
            var dropdown = wait.Until(driver =>driver.FindElements(SaveDropdownButton).LastOrDefault(e => e.Displayed && e.Enabled));
            dropdown?.Click(); ;
            Wait.UntilElementClickable(SaveAndCloseButton).Click();
            Wait.UntilElementVisible(BusinessOutcomeAlertDialog);
            Wait.HardWait(3000); // Need to Wait because toaster message is displayed first and then it is getting Saved
        }

        public bool IsSaveButtonEnabled()
        {
            return Wait.UntilElementExists(SaveButton).Enabled;
        }

        public bool IsSaveDropdownButtonEnabled()
        {
            return Wait.UntilElementExists(SaveDropdownButton).Enabled;
        }

        public void ClickOnCloseIcon()
        {
            Log.Step(nameof(BusinessOutcomeCardPage), "Click on 'Close' button");
            Driver.JavaScriptScrollToElement(CloseIcon).Click();
        }

        public void ClickOnMaximizeIcon()
        {
            Log.Step(nameof(BusinessOutcomeCardPage), "Click on 'Maximize' button");
            Driver.JavaScriptScrollToElement(MaximizeIcon).Click();
            Wait.UntilJavaScriptReady();
        }

        public void ClickOnMinimizeIcon()
        {
            Log.Step(nameof(BusinessOutcomeCardPage), "Click on 'Minimize' button");
            Driver.JavaScriptScrollToElement(MinimizeIcon).Click();
            Wait.UntilJavaScriptReady();
        }

        #endregion

        #region ConfirmPopup

        //Confirm pop up

        public void ConfirmPopUpClickOnSaveChangesButton()
        {
            Log.Step(nameof(BusinessOutcomeCardPage), "Click on 'Save Changes' button on the confirmation popup");
            Driver.JavaScriptScrollToElement(ConfirmSave).Click();
            Wait.UntilElementNotExist(ConfirmSave);
        }

        public void ConfirmPopUpClickOnDiscardChangesButton()
        {
            Log.Step(nameof(BusinessOutcomeCardPage), "Click on 'Discard Changes' button on the confirmation popup");
            Driver.JavaScriptScrollToElement(ConfirmDiscard).Click();
            Wait.UntilElementNotExist(ConfirmDiscard);
        }

        public void ConfirmPopUpClickOnCancelButton()
        {
            Log.Step(nameof(BusinessOutcomeCardPage), "Click on 'Cancel' button on the confirmation popup");
            Driver.JavaScriptScrollToElement(ConfirmCancel).Click();
            Wait.UntilElementNotExist(ConfirmCancel);
        }

        //Pendo Pop Up
        public bool IsPendoPopUpDisplayed()
        {
            return Driver.IsElementDisplayed(PendoCloseIcon);
        }
        public void ClickOnPendoCloseIcon()
        {
            Log.Step(nameof(BusinessOutcomeCardPage), "Click on 'Close' icon on the pendo popup");
            Wait.UntilElementClickable(PendoCloseIcon).Click();
        }

        #endregion

        #endregion

        public void ClickOnKeyResultActionButton(string keyResultName)
        {
            Log.Step(nameof(BusinessOutcomeCardPage), "Click on 'Action' button");
            Driver.JavaScriptScrollToElement(DynamicKeyResultActionButton(keyResultName)).Click();
        }

        public void SelectAction(string action)
        {
            Log.Step(nameof(BusinessOutcomeCardPage), $"Select action - {action}");
            Wait.UntilElementClickable(KeyResultActions(action)).Click();
            Wait.UntilJavaScriptReady();
        }
    }
}
