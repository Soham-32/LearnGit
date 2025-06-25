using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.ManageBusinessOutcomes
{
    internal class ManageBusinessOutcomeTagsPage : BusinessOutcomesSettingsPage
    {
        public ManageBusinessOutcomeTagsPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        private readonly By DemandTagHeader = By.XPath("//h2[text()='Manage Business Outcomes Tags'] | //h2//font[text()='Manage Business Outcomes Tags']");

        // Labels
        private readonly By LabelAddButton = By.ClassName("add__label");
        private static By LabelTextbox(string labelName) => By.Id($"{labelName}");
        private readonly By LabelTextBoxes = By.XPath("//*[text()='Text']//ancestor::div[3]/div[2]//input[@value='']");

        private readonly By LabelNameList =
            By.XPath(
                "//div[contains(@class,'jss')]//input[@type='text' and not (contains(@class,'MuiInputBase-inputAdornedStart'))]");
        private static By LabelDeleteButton(string labelName) => By.XPath(
            $"//*[@id = '{labelName}']/ancestor::div[contains(@class, 'input__label')]/following-sibling::div/button[contains(@aria-label, 'Delete Label')]");
        private static By LabelDropdown(string labelName) => By.XPath($"//ul[@role='menu']//span[contains(normalize-space(),'{labelName}')]");

        // Kanban mode
        private static By KanbanModeToggle(string labelName) =>
            By.XPath($"//*[@id = '{labelName}']//ancestor::div[4]//span[contains(@class, 'MuiSwitch-switchBase')]");

        // Tags
        private static By AddBusinessOutcomeTagButton(string labelName) => By.XPath(
            $"//*[@id = '{labelName}']//ancestor::div[4]//button[contains(@class, 'add__category__tag')]");

        private static By TagTextbox(string labelName, string tagName) =>
            By.XPath($"//input[@value='{labelName}']/ancestor::div[3]/following-sibling::div//input[@value='{tagName}']");
        private static By TagTextboxes(string labelName) => By.XPath($"//*[starts-with(@id, '{labelName}')]/ancestor::div[3]//following-sibling::div/ul//input[@type='text']");
        private static By TagDeleteButton(string labelName, string tagName) => By.XPath(
            $"//input[@value='{labelName}']/ancestor::div[3]/following-sibling::div//input[@value='{tagName}']/ancestor::li//button[contains(@class, 'delete__category')]");
        private static By TagDragButton(string labelName, string tagName) =>
            By.XPath($"//input[@value='{labelName}']/ancestor::div[3]/following-sibling::div//input[@value='{tagName}']/ancestor::li//*[@data-icon = 'grip-vertical']/..");

        // delete label popup
        private readonly By DeleteLabelPopupDeleteLabelButton = By.XPath("//button[text()='DELETE LABEL']");
        private readonly By DeleteLabelPopupCancelButton = By.XPath("//div[@role='dialog']//button[text()='CANCEL']");

        public bool IsManageBusinessOutcomeTagHeaderTextPresent()
        {
            return Driver.IsElementPresent(DemandTagHeader);
        }

        public void ClickOnAddLabelButton()
        {
            Log.Step(nameof(ManageBusinessOutcomeTagsPage), "Click on 'Add Business Outcome Tag Label' button ");
            Wait.UntilElementClickable(LabelAddButton).Click();
        }

        public void EnterLabelName(string originalName, string newName)
        {
            Log.Step(nameof(ManageBusinessOutcomeTagsPage), $"Enter new label name <{newName}>");
            Driver.JavaScriptScrollToElement(LabelTextbox(originalName), false).SetText(newName, isReact:true);
        }

        public void AddNewLabel(string labelName, string labelValue)
        {
            ClickOnAddLabelButton();
            Wait.UntilElementClickable(LabelDropdown(labelValue)).Click();
            Wait.UntilElementClickable(LabelTextBoxes).SetText(labelName);
        }

        public void ClickLabelDeleteButton(string labelName)
        {
            Driver.JavaScriptScrollToElement(LabelDeleteButton(labelName), false).Click();
        }

        public bool IsLabelDisplayed(string labelName)
        {
            return Driver.IsElementDisplayed(LabelTextbox(labelName));
        }

        public List<string> GetAllLabelNames()
        {
            return Driver.GetTextFromAllElements(LabelNameList).ToList();
        }

        public void ToggleKanbanMode(string labelName, bool on)
        {
            var onStr = on ? "on" : "off";
            Log.Step(nameof(ManageBusinessOutcomeTagsPage), $"Turn Kanban Mode {onStr} for the label <{labelName}>");
            if (IsKanbanModeOn(labelName) == on) return;
            Driver.JavaScriptScrollToElement(KanbanModeToggle(labelName), false).Click();
            Wait.UntilAttributeContains(KanbanModeToggle(labelName), "class", "Mui-checked", on);
        }

        public bool IsKanbanModeOn(string labelName)
        {
            var switchLocator = Driver.JavaScriptScrollToElement(KanbanModeToggle(labelName), false);
            return switchLocator.GetAttribute("class").Contains("Mui-checked");
        }

        public void ClickOnAddBusinessOutcomeTagButton(string labelName)
        {
            Wait.UntilElementClickable(AddBusinessOutcomeTagButton(labelName)).Click();
        }

        public void EnterTagName(string labelName, string originalTagName, string newTagName)
        {
            Wait.UntilElementClickable(TagTextbox(labelName, originalTagName)).SetText(newTagName, isReact:true);
        }

        public void ClickOnDeleteTagButton(string labelName, string tagName)
        {
            Wait.UntilElementClickable(TagDeleteButton(labelName, tagName)).Click();
        }

        public bool IsTagDisplayed(string labelName, string tagName)
        {
            return Driver.IsElementPresent(TagTextbox(labelName, tagName));
        }

        public List<string> GetTagNames(string labelName)
        {
            return Driver.GetTextFromAllElements(TagTextboxes(labelName)).ToList();
        }

        public void DragTagToTag(string labelName, string movingTagName, string targetTagName)
        {
            Log.Step(nameof(ManageBusinessOutcomeTagsPage), $"Drag tag from index <{movingTagName}> to <{targetTagName}> for the label <{labelName}>");
            Driver.DragElementToElement(TagDragButton(labelName, movingTagName), TagDragButton(labelName, targetTagName), 0, -30);
        }

        // Delete Label popup
        public void ClickOnDeleteLabelPopupDeleteLabelButton()
        {
            Log.Step(nameof(ManageBusinessOutcomeTagsPage), "Click on the 'Delete Label' button on the Delete label popup");
            Wait.UntilElementClickable(DeleteLabelPopupDeleteLabelButton).Click();
            Wait.UntilElementNotExist(DeleteLabelPopupDeleteLabelButton);
        }

        public void ClickOnDeleteLabelCancelButton()
        {
            Log.Step(nameof(ManageBusinessOutcomeTagsPage), "Click on the 'Cancel' button on the Delete label popup");
            Wait.UntilElementClickable(DeleteLabelPopupCancelButton).Click();
            Wait.UntilElementNotExist(DeleteLabelPopupCancelButton);
        }

    }
}