using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.ManageCustomTypes
{
    public class ManageCustomTypesPage : BasePage
    {
        public ManageCustomTypesPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        //Locators

        //Header Buttons
        private readonly By CancelButton = AutomationId.Equals("cancelBtn");
        private readonly By SaveButton = AutomationId.Equals("saveBtn");
        private readonly By DeleteAllCustomGrowthItemTypesButton = By.XPath("*//div[text()='Custom Growth Item Types']//button[not(@aria-label)]");

        //Key Customer Verification

        #region Custom Growth Item Types Title

        private readonly By CustomGrowthItemTypesTitle = By.XPath("//div[text()='Custom Growth Item Types']");
        #endregion
        private readonly By CustomGrowthItemInformationIcon = By.XPath("*//div[text()='Custom Growth Item Types']//button[@aria-label='Information']");
        private readonly By CustomGrowthItemInformationToolTip = By.CssSelector("#mui-1 > div");

        // Custom Growth Item Types
        private readonly By CustomGrowthItemTypesList = By.XPath("*//div[@id = 'settings']//div//ul//li//input");
        private readonly By CustomGrowthItemTypeAddButton = By.XPath("*//div[@id = 'settings']//div[contains(normalize-space(), 'Add Custom Growth Item Type')]//button//*[local-name()='svg'][@data-icon='plus-circle']");
        private static By CustomGrowthItemTypeDeleteButton(string customTypeName) => AutomationId.Equals($"{customTypeName}_delete");
        private static By CustomGrowthItemTypeNameTextbox(string customTypeName) => By.XPath($"*//div[@id = 'settings']//div//ul//li//input[@value='{customTypeName}']");

        // Delete Confirmation Popup
        private readonly By ConfirmDeletePopup = By.XPath("//div//h2[text() = 'Confirm Delete']");
        private readonly By ConfirmDeletePopUpDeleteButton = By.XPath("//div[@role='dialog']//button[text()='DELETE']");
        private readonly By ConfirmDeletePopupWarningMessage = By.XPath("//div[@role='dialog']//div[contains(@class, 'MuiDialogContent-root')]");

        private readonly By ConfirmCancelDiscardChangesButton = By.XPath("//div[@role='dialog']//button[text()='DISCARD CHANGES'] | //div[@role='dialog']//button//font[text()='DISCARD CHANGES']");
        private readonly By ConfirmCancelSaveChangesButton = By.XPath("//div[@role='dialog']//button[text()='SAVE CHANGES'] | //div[@role='dialog']//button//font[text()='SAVE CHANGES']");
        private readonly By ConfirmCancelPopUpCancelButton = By.XPath("//div[@role='dialog']//button[text()='CANCEL'] | //div[@role='dialog']//button//font[text()='CANCEL']");

        // Methods

        // Header Methods
        public void ClickOnCancelButton()
        {
            Log.Step(GetType().Name, "Click on 'Cancel' button");
            Driver.JavaScriptScrollToElement(CancelButton, false);
            Wait.UntilAllElementsLocated(CustomGrowthItemTypesList);
            Wait.UntilElementClickable(CancelButton).Click();
            Wait.UntilElementVisible(ConfirmCancelPopUpCancelButton);
        }
        public void ClickOnSaveButton()
        {
            Log.Step(GetType().Name, "Click on 'Save' button");
            Driver.JavaScriptScrollToElement(SaveButton, false);
            Wait.UntilElementClickable(SaveButton).Click();
        }
        public void ClickOnDeleteAllCustomGrowthItemTypesButton()
        {
            Log.Step(GetType().Name, "Click on 'Delete All Custom Growth Item Types' button");
            Driver.JavaScriptScrollToElement(DeleteAllCustomGrowthItemTypesButton, false);
            Wait.UntilElementClickable(DeleteAllCustomGrowthItemTypesButton).Click();
        }

        // Custom Growth Item Types
        
        //Key Customer Verification
        #region Custom Growth Item Types Title
        public string GetCustomGrowthItemTypesTitle()
        {
            return Wait.UntilElementVisible(CustomGrowthItemTypesTitle).GetText();
        }
        #endregion
        public void ClickOnAddCustomGrowthItemTypeButton()
        {
            Log.Step(GetType().Name, "Click on 'Add Custom Growth Item Type' button");
            Driver.JavaScriptScrollToElement(CustomGrowthItemTypeAddButton);
            Wait.UntilElementClickable(CustomGrowthItemTypeAddButton).Click();
        }
        public void EnterCustomGrowthItemTypeName(string originalCustomTypeName, string newCustomTypeName)
        {
            Log.Step(nameof(ManageCustomTypesPage), $"Set custom growth item type to <{newCustomTypeName}>");
            Wait.UntilElementEnabled(SaveButton);
            Driver.JavaScriptScrollToElement(Wait.InCase(CustomGrowthItemTypeNameTextbox(originalCustomTypeName)));
            Wait.HardWait(2000); // Hard wait to avoid flakiness - else previous textbox value was getting appeared again.
            Wait.UntilElementClickable(CustomGrowthItemTypeNameTextbox(originalCustomTypeName))
                .SetText(newCustomTypeName, isReact: true);
        }
        public List<string> GetAllCustomGrowthItemTypeList()
        {
            Log.Step(nameof(ManageCustomTypesPage), "Get custom growth item type list");
            Wait.UntilElementVisible(CustomGrowthItemTypesList);
            return Wait.UntilAllElementsLocated(CustomGrowthItemTypesList).Select(column => column.GetText()).ToList();
        }
        public void ClickOnDeleteCustomGrowthItemTypeButton(string customTypeName)
        {
            Log.Step(nameof(ManageCustomTypesPage), "Click on 'Delete' button for the custom growth item type");
            Wait.UntilElementVisible(CustomGrowthItemTypeDeleteButton(customTypeName));
            Wait.UntilElementClickable(CustomGrowthItemTypeDeleteButton(customTypeName)).Click();
        }
        public bool IsCustomGrowthItemTypeDisplayed(string customTypeName)
        {
            return Driver.IsElementDisplayed(CustomGrowthItemTypeNameTextbox(customTypeName),10);
        }

        public void WaitUntilCustomGrowthItemTypeListIsLoaded()
        {
            Wait.UntilAllElementsLocated(CustomGrowthItemTypesList);
        }
        public string GetCustomGrowthItemTypeInformationText()
        {
            Log.Step(nameof(ManageCustomTypesPage), "Getting Information text for 'Custom Growth Item Type'");
            Wait.UntilElementClickable(CustomGrowthItemInformationIcon);
            Driver.MoveToElement(Wait.UntilElementClickable(CustomGrowthItemInformationIcon)).Click();
            return Wait.UntilElementExists(CustomGrowthItemInformationToolTip).GetText();
        }


        // Delete Confirmation Popup
        public void ConfirmationDeletePopupClickOnDeleteButton()
        {
            Log.Step(nameof(ManageCustomTypesPage), "Click on 'Delete' button from 'Confirm Delete' pop up");
            Wait.UntilElementClickable(ConfirmDeletePopUpDeleteButton).Click();
            Wait.UntilElementNotExist(ConfirmDeletePopup);
            Wait.HardWait(1000); //Tags list takes time to refresh, so have to put wait to avoid false failure
        }
        public void ConfirmationCancelPopupClickOnCancelButton()
        {
            Log.Step(nameof(ManageCustomTypesPage), "Click on 'Close' button from 'Confirm Delete' pop up");
            Wait.UntilElementClickable(ConfirmCancelPopUpCancelButton).Click();
            Wait.UntilElementNotExist(ConfirmCancelPopUpCancelButton);
        }
        public string GetConfirmationPopupDeleteWarningMessage()
        {
            Log.Step(nameof(ManageCustomTypesPage), "Get confirmation popup delete warning message");
            return Wait.UntilElementVisible(ConfirmDeletePopupWarningMessage).GetText();
        }

        //Cancel Confirmation Popup
        public void ConfirmationCancelPopupClickOnDiscardChangesButton()
        {
            Log.Step(nameof(ManageCustomTypesPage), "Click on 'Discard Changes' button on 'Cancel' pop up");
            Wait.UntilElementClickable(ConfirmCancelDiscardChangesButton).Click();
            Wait.UntilElementNotExist(ConfirmCancelDiscardChangesButton);
        }
        public void ConfirmationCancelPopupClickOnSaveChangesButton()
        {
            Log.Step(nameof(ManageCustomTypesPage), "Click on 'Save Changes' button on 'Cancel' pop up");
            Wait.UntilElementClickable(ConfirmCancelSaveChangesButton).Click();
            Wait.UntilElementNotExist(ConfirmCancelSaveChangesButton);
        }
    }
}