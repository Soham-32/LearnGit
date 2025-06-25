using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.ManageBusinessOutcomes;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.ManageCustomTypes
{
    internal class ManageCustomFieldsPage : BusinessOutcomesSettingsPage
    {
        public ManageCustomFieldsPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }
        
        private readonly By CustomFieldAddLink = By.CssSelector("button.add__custom__field");
        private readonly By CustomFieldsHeader = By.XPath("//h2[text()='Manage Custom Fields'] | //h2//font[text()='Manage Custom Fields']");
        private readonly By CustomFieldTotalCount = By.CssSelector("input[id^='customFieldsLabel']");
        
        private static By CustomFieldTextBox(string name) => By.XPath($"//div[contains(@data-rbd-draggable-id,'customFieldsLabel')]//input[@value = '{name}']");
        private static By CustomFieldDeleteButton(string name) =>
            By.XPath($"//input[@value = '{name}']/ancestor::li[1]//button[contains(@class, 'delete__category')]");
        private static By DragButton(string name) =>
            By.XPath($"//input[@value = '{name}']/ancestor::li[1]//*[@data-icon = 'grip-vertical']/..");
        
        public bool IsManageCustomFieldsHeaderTextPresent()
        {
            return Driver.IsElementDisplayed(CustomFieldsHeader);
        }

        public void ClickOnAddCustomFieldLink()
        {
            Log.Step(nameof(ManageCustomFieldsPage), "Click on 'Add Custom Field' link");
            Wait.UntilElementClickable(CustomFieldAddLink).Click();
            Wait.UntilJavaScriptReady();
        }

        public int GetTotalCountOfCustomFields()
        {
            return Wait.InCase(CustomFieldTotalCount) == null
                ? 0
                : Wait.UntilAllElementsLocated(CustomFieldTotalCount).Count;
        }

        public List<string> GetAllCustomFieldTexts()
        {
            return Driver.GetTextFromAllElements(CustomFieldTotalCount).ToList();
        }

        public void EnterCustomFieldName(string originalName, string newName)
        {
            Log.Step(nameof(ManageCustomFieldsPage), $"Set custom Field <{newName}>");
            Wait.UntilElementClickable(CustomFieldTextBox(originalName)).SetText(newName, isReact:true);
        }

        public void ClickOnCustomFieldDeleteButton(string customFieldName)
        {
            Log.Step(nameof(ManageCustomFieldsPage), $"Click on Delete button of custom field <{customFieldName}>");
            Wait.UntilElementClickable(CustomFieldDeleteButton(customFieldName)).Click();
        }

        public bool IsCustomFieldDisplayed(string customFieldName)
        {
            return Driver.IsElementDisplayed(CustomFieldTextBox(customFieldName));
        }

        public void DragCustomFieldToCustomField(string movingFieldName, string targetFieldName)
        {
            Log.Step(nameof(ManageCustomFieldsPage), $"Move custom field <{movingFieldName}> to <{targetFieldName}>.");
            Driver.DragElementToElement(DragButton(movingFieldName), DragButton(targetFieldName), 0, -50);
        }

    }
}