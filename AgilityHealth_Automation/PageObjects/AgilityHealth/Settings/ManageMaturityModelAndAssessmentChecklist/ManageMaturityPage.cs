using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.ManageMaturityModelAndAssessmentChecklist
{
    internal class ManageMaturityPage : BasePage
    {
        public ManageMaturityPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        // Team Stages
        private readonly By TeamStageTab = By.CssSelector("li[aria-controls='TeamMaturityChecklist-1']");
        private readonly By AddStageButton = By.CssSelector("#gridCustomMaturity a.k-grid-add");
        private readonly By AhEquivalentList = By.CssSelector("span[aria-owns='MasterTeamMaturityId_listbox']");
        private readonly By MaturityTextbox = By.Id("Maturity");
        private readonly By SaveButton = By.CssSelector(".k-grid-update");

        // Elements

        // Radar Type
        private readonly By SurveyTypeDropDownArrow = By.CssSelector("span[aria-owns='MaturitySurvey_listbox']");
        private readonly By YesButton = By.Id("yesButton");
        private readonly By AcceptDeleteChecklistButton = By.Id("yesButtonTeamMaturity");
        private readonly By AddNewOptionButton = By.CssSelector("#gridOfSelection .k-grid-add");

        // Assessment Checklist
        private readonly By AssessmentChecklistTab = By.CssSelector("li[aria-controls='TeamMaturityChecklist-2']");
        private readonly By ChecklistOptionsDescription =
            By.CssSelector("#gridOfSelection table[role='grid'] tbody tr td:nth-of-type(2)");

        private readonly By DescriptionBody = By.CssSelector("body.k-state-active");
        private readonly By DescriptionIframe = By.XPath("(//iframe[@class='k-content'])[1]");
        private readonly By DescriptionTextbox = By.Id("divplaceholder");
        private readonly By MultiOption = By.CssSelector("#multipleSelectOption_ ~ label");
        private readonly By NewChecklistButton = By.CssSelector(".k-grid-New");
        private readonly By OptionTextbox = By.CssSelector("input[name='Option']");
        private readonly By SaveChecklistButton = By.CssSelector(".k-grid-update");
        private readonly By SingleOption = By.CssSelector("#singleSelectOption_ ~ label");
        private readonly By UpdateOptionButton = By.CssSelector("#gridOfSelection .k-icon.k-update");

        private static By DynamicSurveyType(string surveyType) =>
            By.XPath($"//ul[@id ='MaturitySurvey_listbox']//li[text() = '{surveyType}'] | //ul[@id ='MaturitySurvey_listbox']//li//font[text() = '{surveyType}']");

        private static By AhEquivalentItem(string item) =>
            By.XPath($"//ul[@id='MasterTeamMaturityId_listbox']//li[text()='{item}'] | //ul[@id='MasterTeamMaturityId_listbox']//li//font[text()='{item}']");

        private static By MaturityItem(string name, string ah) =>
            By.XPath($"//div[@id='gridCustomMaturity']//table/tbody//td[text()='{name}']/following-sibling::td[text()='{ah}'] | //div[@id='gridCustomMaturity']//table/tbody//td//font[text()='{name}']/ancestor::td/following-sibling::td//font[text()='{ah}']");

        private static By EditMaturityButton(string item) => By.XPath(
            $"//div[@id='gridCustomMaturity']//table/tbody//td[text()='{item}']/following-sibling::td/a[contains(@class, 'k-grid-edit')] | //div[@id='gridCustomMaturity']//table/tbody//td//font[text()='{item}']/ancestor::td/following-sibling::td/a[contains(@class, 'k-grid-edit')]/span");

        private static By DeleteMaturityButton(string item) => By.XPath(
            $"//div[@id='gridCustomMaturity']//table/tbody//td[text()='{item}']/following-sibling::td/a[contains(@class, 'DeleteTeamMaturity')] | //div[@id='gridCustomMaturity']//table/tbody//td//font[text()='{item}']/ancestor::td/following-sibling::td/a[contains(@class, 'DeleteTeamMaturity')]");

        private static By EditChecklistButton(string item) => By.XPath(
            $"//div[@id='GridTeamMaturity']/div[@class='k-grid-content']//td[@class='description'][text()='{item}']/following-sibling::td/a[contains(@class, 'k-grid-edit')] | //div[@id='GridTeamMaturity']/div[@class='k-grid-content']//td[@class='description']//font[text()='{item}']/ancestor::td/following-sibling::td/a[contains(@class, 'k-grid-edit')]");

        private static By DeleteChecklistButton(string item) => By.XPath(
            $"//div[@id='GridTeamMaturity']/div[@class='k-grid-content']//td[@class='description'][contains(normalize-space(),'{item}')]/following-sibling::td/a[contains(@class, 'DeleteTeamMaturity1')]");
        private static By ChecklistRow(string description, string type) => By.XPath(
            $"//div[@id='GridTeamMaturity']/div[@class='k-grid-content']//td[@class='description'][text()='{description}']/following-sibling::td[@class='type'][text()='{type}'] | //div[@id='GridTeamMaturity']/div[@class='k-grid-content']//td[@class='description']//font[text()='{description}']/ancestor::td/following-sibling::td[@class='type']//font[text()='{type}']");

        // Methods

        // Radar Type
        public void SelectRadarType(string radarType)
        {
            Log.Step(nameof(ManageMaturityPage), $"Select radar type <{radarType}>");
            Wait.UntilElementClickable(SurveyTypeDropDownArrow).Click();
            Driver.JavaScriptClickOn(Driver.JavaScriptScrollToElement(Wait.UntilElementExists(DynamicSurveyType(radarType))));
            Wait.UntilJavaScriptReady();
            Wait.UntilElementClickable(TeamStageTab).Click();
            Wait.UntilJavaScriptReady();
        }

        // Team Stages
        public void ClickAddStage()
        {
            Log.Step(nameof(ManageMaturityPage), "Click on Add Stage");
            Wait.UntilElementClickable(AddStageButton).Click();
            Wait.UntilElementClickable(SaveButton);
        }

        public void EnterMaturityName(string name)
        {
            Log.Step(nameof(ManageMaturityPage), $"Enter maturity name <{name}>");
            var textBox = Wait.UntilElementClickable(MaturityTextbox);
            textBox.Click();
            textBox.SetText(name);
        }

        public void SelectAhEquivalent(string item)
        {
            Log.Step(nameof(ManageMaturityPage), $"Select AH equivalent <{item}>");
            SelectItem(AhEquivalentList, AhEquivalentItem(item));
        }

        public void ClickSaveButton()
        {
            Log.Step(nameof(ManageMaturityPage), "Click on Save button");
            Driver.JavaScriptClickOn(Wait.UntilElementClickable(SaveButton)); //at times, normal click doesn't work.
            Wait.UntilJavaScriptReady();
        }

        public void ClickDeleteButton(string name)
        {
            Log.Step(nameof(ManageMaturityPage), $"Click on Delete button for <{name}>");
            Driver.JavaScriptScrollToElement(DeleteMaturityButton(name)).Click();
            Wait.UntilElementVisible(YesButton);
        }

        public void AcceptDelete()
        {
            Log.Step(nameof(ManageMaturityPage), "Accept deleting");
            Wait.UntilElementClickable(YesButton).Click();
            Wait.UntilJavaScriptReady();
        }

        public void ClickEditMaturity(string name)
        {
            Log.Step(nameof(ManageMaturityPage), $"Click on edit <{name}> maturity");
            Driver.JavaScriptScrollToElement(EditMaturityButton(name)).Click();
            Wait.UntilJavaScriptReady();
            Wait.UntilElementClickable(SaveButton);
        }

        public bool DoesMaturityExist(string name, string ah) => Driver.IsElementDisplayed(MaturityItem(name, ah));

        // Assessment Checklist
        public void EnterChecklistDescription(string description)
        {
            Log.Step(nameof(ManageMaturityPage), $"Enter checklist description <{description}>");
            Driver.SwitchToFrame(DescriptionIframe);
            if (Driver.IsElementDisplayed(DescriptionTextbox))
                Wait.UntilElementClickable(DescriptionTextbox).Click();
            else
                Wait.UntilElementClickable(By.CssSelector("body")).Click();
            Wait.UntilJavaScriptReady();
            Wait.UntilElementClickable(DescriptionBody).SetText(description);
            Driver.SwitchTo().DefaultContent();
        }

        public void SelectAssessmentCheckListTab()
        {
            Log.Step(nameof(ManageMaturityPage), "Select assessment checklist tab");
            Wait.UntilElementClickable(AssessmentChecklistTab).Click();
            Wait.UntilJavaScriptReady();
        }

        public void ClickAddNewChecklistButton()
        {
            Log.Step(nameof(ManageMaturityPage), "Click on Add New Checklist button");
            Wait.UntilAllElementsLocated(NewChecklistButton).First(e => Wait.UntilElementVisible(e) != null).Click();
            Wait.UntilElementVisible(AddNewOptionButton);
        }

        public void SelectOptionType(bool single = true)
        {
            Log.Step(nameof(ManageMaturityPage), $"Select {(single ? "Single" : "Multi")} option");
            Wait.UntilElementVisible(SingleOption);
            if (single)
                Wait.UntilElementClickable(SingleOption).Click();
            else
                Wait.UntilElementClickable(MultiOption).Click();
        }

        public void ClickAddNewOption()
        {
            Log.Step(nameof(ManageMaturityPage), "Click on Add New button");
            Wait.UntilElementClickable(AddNewOptionButton).Click();
            Wait.UntilElementVisible(OptionTextbox);
        }

        public void EnterOptionTextbox(string option)
        {
            Log.Step(nameof(ManageMaturityPage), $"Enter option <{option}>");
            Wait.UntilElementVisible(OptionTextbox).SetText(option, false);
        }

        public void ClickSaveOptionButton()
        {
            Log.Step(nameof(ManageMaturityPage), "Click on Save Option button");
            Wait.UntilElementVisible(UpdateOptionButton).Click();
            Wait.UntilElementNotExist(UpdateOptionButton);
        }

        public void ClickSaveChecklistButton()
        {
            Log.Step(nameof(ManageMaturityPage), "Click on Save Checklist button");
            Wait.UntilElementVisible(SaveChecklistButton).Click();
            Wait.UntilElementNotExist(SaveChecklistButton);
            Wait.UntilJavaScriptReady();
        }

        public void ClickEditChecklistButton(string description)
        {
            Log.Step(nameof(ManageMaturityPage), $"Click on edit checklist for <{description}>");
            Wait.UntilElementClickable(EditChecklistButton(description)).Click();
            Wait.UntilElementVisible(AddNewOptionButton);
        }

        public void ClickDeleteChecklistButton(string description)
        {
            Log.Step(nameof(ManageMaturityPage), $"Click on delete checklist for <{description}>");
            Wait.UntilElementClickable(DeleteChecklistButton(description)).Click();
            Wait.UntilElementVisible(AcceptDeleteChecklistButton);
        }

        public void AcceptDeleteChecklist()
        {
            Log.Step(nameof(ManageMaturityPage), "Accept deleting checklist");
            Wait.UntilElementClickable(AcceptDeleteChecklistButton).Click();
            Wait.UntilJavaScriptReady();
        }

        public bool DoesChecklistExist(string description, string type)
        {
            return Driver.IsElementDisplayed(ChecklistRow(description, type));
        }

        public List<string> GetAddedChecklistOptions() => Driver.GetTextFromAllElements(ChecklistOptionsDescription).ToList();
    }
}