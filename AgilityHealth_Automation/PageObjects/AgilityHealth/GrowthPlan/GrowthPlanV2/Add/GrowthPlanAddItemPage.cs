using System;
using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.V2;
using AgilityHealth_Automation.Utilities;
using AtCommon.Dtos.GrowthPlan.Custom;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.GrowthPlan.GrowthPlanV2.Add
{

    internal class GrowthPlanAddItemPage : BasePage
    {
        public GrowthPlanAddItemPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        //Get Lists From DropDown
        private readonly By GrowthPlanTitle = AutomationId.Equals("growthPlanTitle", "input");
        private readonly By OwnerList = By.XPath("//div[@id='menu-owners']//div//ul//li[@role='option']");
        private readonly By StatusList = By.XPath("//div[@id='menu-status']//div//ul//li[@role='option']");
        private readonly By PriorityList = By.XPath("//div[@id='menu-priority']//div//ul//li[@role='option']");
        private readonly By CategoryList = By.XPath("//div[@id='menu-category']//div//ul//li[@role='option']");
        private readonly By AssessmentList = By.XPath("//div[@id='menu-assessmentType']//div//ul//li[@role='option']");
        private readonly By TargetCompetenciesList = By.XPath("//div[@id='menu-targetCompetencies']//div//ul//li[@role='option']");
        private readonly By TypesList = By.XPath("//div[@id='menu-type']//div//ul//li[@role='option']");

        private readonly By OwnersSelectListBox = AutomationId.Equals("owners");
        private static By OwnersSelectListItem(string item) => By.XPath($"//div[@id='menu-owners']//li[text()='{item}']");
        private static By SelectedOwner(string item) => By.XPath($"//div//div[starts-with(@automation-id, 'ownersChip_')]//span[text() = '{item}']");

        private readonly By SelectedOwnersList = By.XPath("//div[contains(@automation-id,'ownersChip')]//span[@class='MuiChip-label']");
        private static By DeselectOwnerItem(string item) => By.XPath($"//span[text()='{item}']//following-sibling::*[local-name()='svg']");
        private static By AssessmentTypeSelect(string item) => By.XPath($"//div[@id='menu-assessmentType']//li[text()='{item}']");
        private const string DueDateTimePickerInputId = "dueDate";
        private readonly By DescriptionTextarea = By.XPath("//div[@role='presentation']//div[@class='DraftEditor-editorContainer']//div");

        private readonly By StatusSelectListBox = AutomationId.Equals("status");
        private static By StatusSelectListItem(string item) => By.XPath($"//div[@id='menu-status']//li[text()='{item}']");
        private readonly By PrioritySelectListBox = AutomationId.Equals("priority");
        private static By PrioritySelectListItem(string item) => By.XPath($"//div[@id='menu-priority']//li[text()='{item}']");

        private readonly By CategorySelectListBox = AutomationId.Equals("category");
        private static By CategorySelectItem(string item) => By.XPath($"//div[@id='menu-category']//li[text()='{item}']");

        private readonly By AssessmentTypeSelectListBox = AutomationId.Equals("assessmentType");
        private static By AssessmentTypeSelectItem(string item) => By.XPath($"//div[@id='menu-assessmentType']//li[text()='{item}']");

        private readonly By TargetCompetenciesSelectListBox = AutomationId.Equals("targetCompetencies");
        private static By TargetCompetenciesSelectItem(string item) => By.XPath($"//div[@id='menu-targetCompetencies']//li[text()='{item}']");
        private readonly By TargetCompetenciesFieldDisabled =
            By.XPath("*//div[@id= 'targetCompetenciesChip']//div//div[contains(@class, 'Mui-disabled')]");
        private static By DeselectTargetCompetenciesItem(string item) => By.XPath($"//div[@automation-id='targetCompetenciesChip_{item}']//following-sibling::*[local-name()='svg']");
        private static By SelectedTargetCompetenciesItem(string item) => AutomationId.Equals($"targetCompetenciesChip_{item}");
        private readonly By SelectedTargetCompetenciesList = By.XPath("//div[contains(@automation-id,'targetCompetenciesChip')]//span[@class='MuiChip-label']");
        private readonly By TypesSelectListBox = AutomationId.Equals("type");
        private static By TypesSelectItem(string item) => By.XPath($"//div[@id='menu-type']//li[text()='{item}']");
        private static By DeselectTypesItem(string item) => By.XPath($"//div[@id='typesChip']//span[text()='{item}']//following-sibling::*[local-name()='svg']");
        private static By SelectedTypesItem(string item) => By.XPath($"//div[@id='typesChip']//span[text()='{item}']");
        private readonly By SelectedTypesList = By.XPath("//div[@id='typesChip']//span[@class='MuiChip-label']");

        private readonly By DescriptionPlaceholderText = By.XPath("//div[contains(@id,'placeholder')]/div");
        private readonly By InfoToolTipIcon = By.XPath("//button[@aria-label='Information']");
        private readonly By TooltipDescriptionText = By.XPath("//div[@role='tooltip']/div/div");
        private readonly By TooltipSupportArticleButton = By.XPath("//div[contains(@class,'MuiTooltip-tooltip')]//a[@automation-id='supportArticleBtn']");

        //Common
        private readonly By DeleteButton = AutomationId.Equals("deleteBtn");
        private readonly By SaveButton = AutomationId.Equals("saveBtn");
        private readonly By CloseIconButton = AutomationId.Equals("headerCancelBtn");

        // Confirmation Popup For Close Icon
        private readonly By ConfirmationPopUpCancelButton = By.XPath("*//div[@role = 'dialog']//button/span[text()='CANCEL']");
        private readonly By ConfirmationPopUpDiscardChangesButton = By.XPath("*//div[@role = 'dialog']//button/span[text()='DISCARD CHANGES']");
        private readonly By ConfirmationPopUpSaveChangesButton = By.XPath("*//div[@role = 'dialog']//button/span[text()='SAVE CHANGES']");

        // Confirmation Popup For Delete Button
        private readonly By ConfirmationDeletePopUpCancelButton = AutomationId.Equals("cancelBtn");
        private readonly By ConfirmationDeletePopUpDeleteButton =
            By.XPath("*//div[@role='dialog']//button//span[text()='DELETE']");

        // Click On Drop-Down Button
        public void ClickOnOwnerDropDown()
        {
            Log.Step(nameof(GrowthPlanAddItemPage), "Click On 'Owner' Drop-Down");
            Wait.UntilElementVisible(OwnersSelectListBox);
            Wait.UntilElementClickable(OwnersSelectListBox).Click();
        }
        public void DoubleClickOnOwnerDropDown()
        {
            Log.Step(nameof(GrowthPlanAddItemPage), "Double Click On 'Owner' Drop-Down");
            Wait.UntilElementClickable(OwnersSelectListBox).DoubleClick(Driver);
        }
        public void ClickOnStatusDropDown()
        {
            Log.Step(nameof(GrowthPlanAddItemPage), "Click On 'Status' Drop-Down");
            Wait.UntilElementClickable(StatusSelectListBox).Click();
        }
        public void ClickOnPriorityDropDown()
        {
            Log.Step(nameof(GrowthPlanAddItemPage), "Click On 'Priority' Drop-Down");
            Wait.UntilElementClickable(PrioritySelectListBox).Click();
        }
        public void ClickOnCategoryDropDown()
        {
            Log.Step(nameof(GrowthPlanAddItemPage), "Click On 'Category' Drop-Down");
            Wait.UntilElementClickable(CategorySelectListBox).Click();
        }
        public void ClickOnSelectAssessmentDropDown()
        {
            Log.Step(nameof(GrowthPlanAddItemPage), "Click On 'Select Assessment Type' Drop-Down");
            Wait.UntilElementClickable(AssessmentTypeSelectListBox).Click();
        }
        public void ClickOnAssessmentTypeSelect(string item)
        {
            Log.Step(nameof(GrowthPlanAddItemPage), $"Click On {item} from 'Select Assessment Type' Drop-Down");
            Wait.UntilElementClickable(AssessmentTypeSelect(item)).Click();
            Wait.UntilJavaScriptReady();
        }
        public void ClickOnTargetCompetenciesDropDown()
        {
            Log.Step(nameof(GrowthPlanAddItemPage), "Click On 'Target Competencies' Drop-Down");
            Wait.UntilElementClickable(TargetCompetenciesSelectListBox).Click();
            Wait.UntilJavaScriptReady();
        }
        public void ClickOnTypesDropDown()
        {
            Log.Step(nameof(GrowthPlanAddItemPage), "Click On 'Types' Drop-Down");
            Wait.UntilElementClickable(TypesSelectListBox).Click();
        }
        public bool IsOwnerSelected(string owner)
        {
            Log.Step(nameof(GrowthPlanAddItemPage), $"Is owner - {owner} selected ? ");
            return Driver.IsElementDisplayed(SelectedOwner(owner));
        }
        public void DeselectOwner(string owner)
        {
            Log.Step(nameof(GrowthPlanAddItemPage), $"Deselect owner {owner}");
            Wait.UntilElementClickable(DeselectOwnerItem(owner)).Click();
        }
        public void DeselectTargetCompetency(string competency)
        {
            Log.Step(nameof(GrowthPlanAddItemPage), $"Deselect TargetCompetencies item {competency}");
            Wait.UntilElementClickable(DeselectTargetCompetenciesItem(competency)).Click();
        }
        public bool IsTargetCompetenciesItemSelected(string competency)
        {
            Log.Step(nameof(GrowthPlanAddItemPage), $"Is TargetCompetencies - {competency} selected ? ");
            return Driver.IsElementDisplayed(SelectedTargetCompetenciesItem(competency));
        }
        public bool IsTypesItemSelected(string type)
        {
            Log.Step(nameof(GrowthPlanAddItemPage), $"Is Types - {type} selected ?");
            return Driver.IsElementDisplayed(SelectedTypesItem(type));
        }
        public void DeselectType(string type)
        {
            Log.Step(nameof(GrowthPlanAddItemPage), $"Deselect Type - {type}");
            Wait.UntilElementClickable(DeselectTypesItem(type)).Click();
        }

        // Getting List from Drop-Down
        public IList<string> GetOwnerList()
        {
            Log.Step(nameof(GrowthPlanAddItemPage), "Get 'Owner' List");
            return Wait.UntilAllElementsLocated(OwnerList).Select(a => Driver.JavaScriptScrollToElement(a).GetText()).ToList();
        }
        public IList<string> GetSelectedOwnersList()
        {
            Log.Step(nameof(GrowthPlanAddItemPage), "Get selected 'Owners' List");
            return Wait.UntilAllElementsLocated(SelectedOwnersList).Select(a => Driver.JavaScriptScrollToElement(a).GetText()).ToList();
        }

        public IList<string> GetStatusList()
        {
            Log.Step(nameof(GrowthPlanAddItemPage), "Get 'Status' List");
            Wait.UntilJavaScriptReady();
            return Wait.UntilAllElementsLocated(StatusList).Select(a => Driver.JavaScriptScrollToElement(a).GetText()).ToList();
        }
        public IList<string> GetPriorityList()
        {
            Log.Step(nameof(GrowthPlanAddItemPage), "Get 'Priority' List");
            return Wait.UntilAllElementsLocated(PriorityList).Select(a => Driver.JavaScriptScrollToElement(a).GetText()).ToList();
        }

        public IList<string> GetCategoryList()
        {
            Log.Step(nameof(GrowthPlanAddItemPage), "Get 'Category' List");
            return Wait.UntilAllElementsLocated(CategoryList).Select(a => Driver.JavaScriptScrollToElement(a).GetText()).ToList();
        }
        public IList<string> GetAssessmentTypeList()
        {
            Log.Step(nameof(GrowthPlanAddItemPage), "Get 'Assessment Type' List");
            return Wait.UntilAllElementsLocated(AssessmentList).Select(a => Driver.JavaScriptScrollToElement(a).GetText()).ToList();
        }
        public IList<string> GetTargetCompetenciesList()
        {
            Log.Step(nameof(GrowthPlanAddItemPage), "Get 'Target Competencies' List");
            return Wait.UntilAllElementsLocated(TargetCompetenciesList).Select(a => Driver.JavaScriptScrollToElement(a).GetText()).ToList();
        }
        public IList<string> GetSelectedTargetCompetenciesList()
        {
            Log.Step(nameof(GrowthPlanAddItemPage), "Get selected 'TargetCompetencies' List");
            return Wait.UntilAllElementsLocated(SelectedTargetCompetenciesList).Select(a => Driver.JavaScriptScrollToElement(a).GetText()).ToList();
        }

        public IList<string> GetTypesList()
        {
            Log.Step(nameof(GrowthPlanAddItemPage), "Get selected 'Types' List");
            return Wait.UntilAllElementsLocated(TypesList).Select(a => Driver.JavaScriptScrollToElement(a).GetText()).ToList();
        }
        public IList<string> GetSelectedTypesList()
        {
            Log.Step(nameof(GrowthPlanAddItemPage), "Get 'Types' List");
            return Wait.UntilAllElementsLocated(SelectedTypesList).Select(a => Driver.JavaScriptScrollToElement(a).GetText()).ToList();
        }

        // Select on Drop-Down value

        public void EnterGrowthPlanTitle(string titleName)
        {
            Log.Step(nameof(GrowthPlanAddItemPage), "Enter Title");
            Wait.UntilElementClickable(GrowthPlanTitle).ClearTextbox();
            Wait.UntilElementClickable(GrowthPlanTitle).SetText(titleName);
        }
        public void EnterDescription(string description)
        {
            Log.Step(nameof(GrowthPlanAddItemPage), " Click on description tab and enter description");
            Wait.UntilElementVisible(DescriptionTextarea);
            Wait.UntilElementClickable(DescriptionTextarea).Click();
            Wait.UntilElementClickable(DescriptionTextarea).SetText(description);
        }
        public void SelectOwner(string owner)
        {
            Log.Step(nameof(GrowthPlanAddItemPage), "Select 'Owner' value from Drop-Down");
            Wait.UntilElementClickable(OwnersSelectListItem(owner)).Click();
        }
        public void SelectAssessmentType(string owner)
        {
            Log.Step(nameof(GrowthPlanAddItemPage), "Select 'Assessment Type' from Drop-Down");
            Wait.UntilElementClickable(AssessmentTypeSelect(owner)).Click();
            Wait.UntilElementNotExist(TargetCompetenciesFieldDisabled, 15);  // Waiting till Target Competencies field is enabled 
        }
        public void SelectTargetCompetencies(string owner)
        {
            Log.Step(nameof(GrowthPlanAddItemPage), "Select 'Target Competencies' value from Drop-Down");
            Wait.UntilElementClickable(TargetCompetenciesSelectItem(owner)).Click();
        }
        public void SelectStatus(string status)
        {
            Log.Step(nameof(GrowthPlanAddItemPage), "Select 'Status' value from Drop-Down");
            Wait.UntilElementClickable(StatusSelectListItem(status)).Click();
        }
        public void SelectPriority(string priority)
        {
            Log.Step(nameof(GrowthPlanAddItemPage), "Select 'Priority' value from Drop-Down");
            Wait.UntilElementClickable(PrioritySelectListItem(priority)).Click();
        }
        public void SelectCategory(string category)
        {
            Log.Step(nameof(GrowthPlanAddItemPage), "Select 'Category' value from Drop-Down");
            Wait.UntilElementClickable(CategorySelectItem(category)).Click();
        }
        public void SelectType(string type)
        {
            Log.Step(nameof(GrowthPlanAddItemPage), "Select 'Type' value from Drop-Down");
            Wait.UntilElementVisible(TypesSelectItem(type));
            Wait.UntilElementClickable(TypesSelectItem(type)).Click();
        }

        // common
        public void SetStartDate(DateTime startDate)
        {
            Log.Step(nameof(GrowthPlanAddItemPage), "Select date for GrowthPlanItem");
            if (startDate.CompareTo(new DateTime()) == 0) return;
            var calendar = new V2Calendar(Driver, DueDateTimePickerInputId);
            calendar.SetDateAndTime(startDate);
        }

        public void FillForm(GrowthItem growthItem)
        {
            Log.Step(nameof(GrowthPlanAddItemPage), "Fill Growth Plan Item form");
            EnterGrowthPlanTitle(growthItem.Title);
            EnterDescription(growthItem.Description);
            SelectItem(OwnersSelectListBox, OwnersSelectListItem(growthItem.Owner));
            SetStartDate(DateTime.Parse(growthItem.TargetDate?.ToString("MM/dd/yyyy tt")));
            Wait.UntilElementClickable(DescriptionTextarea).SetText(growthItem.Description);
            SelectItem(StatusSelectListBox, StatusSelectListItem(growthItem.Status));
            SelectItem(PrioritySelectListBox, PrioritySelectListItem(growthItem.Priority));
            SelectItem(CategorySelectListBox, CategorySelectItem(growthItem.Category));
            SelectItem(AssessmentTypeSelectListBox, AssessmentTypeSelectItem(growthItem.RadarType));
            SelectItem(TargetCompetenciesSelectListBox, TargetCompetenciesSelectItem(growthItem.CompetencyTargets[0]));
            SelectItem(TypesSelectListBox, TypesSelectItem(growthItem.Type));
        }
        public void ClickOnSaveButton()
        {
            Log.Step(nameof(GrowthPlanAddItemPage), "Click on 'Save' button");
            Wait.UntilElementClickable(SaveButton).Click();
            Wait.UntilElementNotExist(SaveButton, 20);
        }

        public bool IsSaveButtonEnabled()
        {
            Log.Step(nameof(GrowthPlanAddItemPage), "Is 'Save' button enabled ?");
            return Driver.IsElementEnabled(SaveButton, 15);
        }
        public bool IsGrowthPlanTitleDisplayed()
        {
            Log.Step(nameof(GrowthPlanAddItemPage), "Is Growth Plan 'Title' displayed ?");
            return Driver.IsElementDisplayed(GrowthPlanTitle, 15);
        }

        public void ClickOnDeleteButton()
        {
            Log.Step(nameof(GrowthPlanAddItemPage), "Click on 'Delete' button");
            Wait.UntilElementClickable(DeleteButton).Click();
        }

        public bool IsDeleteButtonEnabled()
        {
            Log.Step(nameof(GrowthPlanAddItemPage), "Is 'Delete' button enabled ?");
            return Driver.IsElementEnabled(DeleteButton);
        }

        public void ClickOnCloseIconButton()
        {
            Log.Step(nameof(GrowthPlanAddItemPage), "Click on 'Close Icon' button");
            Wait.UntilElementClickable(CloseIconButton).Click();
        }

        // Confirmation Popup
        public void ConfirmationPopUpClickOnCancelButton()
        {
            Log.Step(nameof(GrowthPlanAddItemPage), "Click on 'Cancel' button from confirmation popup");
            Wait.UntilElementClickable(ConfirmationPopUpCancelButton).Click();
        }
        public void ConfirmationPopUpClickOnDiscardChangesButton()
        {
            Log.Step(nameof(GrowthPlanAddItemPage), "Click on 'Discard Changes' button from confirmation popup");
            Wait.UntilElementClickable(ConfirmationPopUpDiscardChangesButton).Click();
        }
        public void ConfirmationPopUpClickOnSaveChangesButton()
        {
            Log.Step(nameof(GrowthPlanAddItemPage), "Click on 'Save Changes' button from confirmation popup");
            Wait.UntilElementClickable(ConfirmationPopUpSaveChangesButton).Click();
            Wait.UntilElementNotExist(ConfirmationPopUpSaveChangesButton, 5);
        }
        public void ConfirmationDeletePopUpClickOnDeleteButton()
        {
            Log.Step(nameof(GrowthPlanAddItemPage), "Click on 'Delete' button from confirmation delete popup");
            Wait.UntilElementVisible(ConfirmationDeletePopUpDeleteButton);
            Wait.UntilElementClickable(ConfirmationDeletePopUpDeleteButton).Click();
            Wait.UntilElementNotExist(ConfirmationDeletePopUpDeleteButton, 5);
        }
        public void ConfirmationDeletePopUpClickOnCancelButton()
        {
            Log.Step(nameof(GrowthPlanAddItemPage), "Click on 'Cancel' button from confirmation delete popup");
            Wait.UntilElementClickable(ConfirmationDeletePopUpCancelButton).Click();
        }

        // Description Field
        public string GetDefaultDescriptionPlaceholderText()
        {
            Log.Info("Get default texts from description field");
            return Wait.UntilElementVisible(DescriptionPlaceholderText).GetText();
        }
        public string GetTooltipDescriptionText()
        {
            Log.Info("Hover to tool tip icon and get tooltip description texts");
            Driver.MoveToElement(InfoToolTipIcon);
            return Wait.UntilElementVisible(TooltipDescriptionText).GetText();
        }
        public bool IsSupportArticleButtonDisplayed()
        {
            return Driver.IsElementDisplayed(TooltipSupportArticleButton);
        }
        public void ClickOnSupportArticleButton()
        {
            Log.Info("Hover to tool tip icon and click on 'Support Article' button");
            Driver.MoveToElement(InfoToolTipIcon);
            Wait.UntilElementClickable(TooltipSupportArticleButton).Click();
        }

    }
}