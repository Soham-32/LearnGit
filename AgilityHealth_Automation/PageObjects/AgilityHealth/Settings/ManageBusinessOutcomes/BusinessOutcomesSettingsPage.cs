using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.ManageBusinessOutcomes
{
    public class BusinessOutcomesSettingsPage : BasePage
    {
        public BusinessOutcomesSettingsPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        // Locators
        private static By SettingOptions(string option) => By.XPath($"//button[contains(normalize-space(),'{option}')]");
        private readonly By CancelButton = By.Id("Cancel__btn");
        private readonly By SaveButton = By.Id("Save__btn");
        private readonly By CloseButton = By.Id("cancel_btn");

        private readonly By ConfirmCancel = By.ClassName("cancel__confirmchanges");
        private readonly By ConfirmDiscard = By.ClassName("discard__confirmchanges");
        private readonly By ConfirmSave = By.XPath("//button[contains(@class,'save__confirmchanges')]");

        // Methods
        public void SelectSettingsOption(string option)
        {
            Log.Step(nameof(BusinessOutcomesSettingsPage), $"Click on the <{option}> option");
            Wait.UntilElementClickable(SettingOptions(option)).Click();
            Wait.UntilJavaScriptReady();
        }

        public void ClickOnManageBusinessOutcomesTagsButton()
        {
            SelectSettingsOption("Manage Business Outcomes Tags");
        }

        public void ClickOnManageBusinessOutcomesDashboardSettingsButton()
        {
            SelectSettingsOption("Manage Dashboard Settings");
            Wait.HardWait(3000); // need to wait till card type data loads
        }

        public void ClickOnManageCustomFieldsButton()
        {
            SelectSettingsOption("Manage Custom Fields");
        }

        public void ClickOnManageArchiveSettingsButton()
        {
            Log.Step(GetType().Name, "Click on the 'Manage Archive Dashboard' button");
            SelectSettingsOption("Manage Archive Settings");
        }


        public void ClickOnSaveButton()
        {
            Log.Step(GetType().Name, "Click on the 'Save' button");
            Driver.JavaScriptScrollToElement(SaveButton, false);
            Wait.UntilElementClickable(SaveButton).Click();
        }

        public void ClickOnCancelButton()
        {
            Driver.JavaScriptScrollToElement(CancelButton, false);
            Log.Step(GetType().Name, "Click on the 'Cancel' button");
            Wait.UntilElementClickable(CancelButton).Click();
            Wait.UntilJavaScriptReady();
        }

        public void ClickOnCloseButton()
        {
            Log.Step(GetType().Name, "Click on the 'Close' button");
            Wait.UntilElementClickable(CloseButton).Click();
            Wait.UntilJavaScriptReady();
        }

        public void ConfirmPopUpClickOnSaveChangesButton()
        {
            Log.Step(GetType().Name, "Click on the 'Save Changes' button in the popup");
            Driver.JavaScriptScrollToElement(Wait.UntilElementExists(ConfirmSave)).Click();
            Wait.UntilJavaScriptReady();
        }

        public void ConfirmPopUpClickOnDiscardChangesButton()
        {
            Log.Step(GetType().Name, "Click on the 'Discard' button in the popup");
            Driver.JavaScriptScrollToElement(Wait.UntilElementExists(ConfirmDiscard)).Click();
            Wait.UntilJavaScriptReady();
        }

        public void ConfirmPopUpClickOnCancelButton()
        {
            Log.Step(GetType().Name, "Click on the 'Cancel' button in the popup");
            Driver.JavaScriptScrollToElement(Wait.UntilElementExists(ConfirmCancel)).Click();
            Wait.UntilJavaScriptReady();
        }

    }
}