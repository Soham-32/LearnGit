using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;
using System.Collections.Generic;
using System.Linq;
using AtCommon.Dtos.BusinessOutcomes;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.Tabs
{
    public class CheckListTabPage : BaseTabPage
    {
        public CheckListTabPage(IWebDriver driver, ILogger log) : base(driver, log) { }

        #region Checklist
        private readonly By AddChecklistItemButton = By.XPath("//button[contains(text(),'Checklist')]//*[local-name()='svg' and @data-testid='AddCircleIcon']");

        private static By SelectOwner(int rowIndex) => By.XPath($"(//*[@data-rbd-droppable-id='BO-Checklist']//input[@role='combobox'])[{rowIndex}]");
        private static By OwnerName(int rowIndex) => By.XPath($"(//*[@data-rbd-droppable-id='BO-Checklist']//input[@role='combobox'])[{rowIndex}]//preceding-sibling::div/span[1]");

        private readonly By GetChecklistCount = By.XPath("//tbody[@data-rbd-droppable-id='BO-Checklist']//tr[@role='button']");

        private static By CheckListTitle(int rowIndex) => By.XPath($"(//tbody[@data-rbd-droppable-id='BO-Checklist']//tr//textarea[1])[{rowIndex}]");

        private static By CompleteCheckList(int rowIndex) => By.XPath($"(//tbody[@data-rbd-droppable-id='BO-Checklist']//input[@type='checkbox'])[{rowIndex}]");

        private static By CheckListOwnerDropdown(string owner) => By.XPath($"//ul[@role='listbox']//*[text()='{owner}']");

        private readonly By ChecklistProgressPercentage = By.XPath("//p[text()='Checklist Progress']/parent::div/div/p");

        private readonly By ChecklistProgressIndication = By.XPath("//p[text()='Checklist Progress']/parent::div//span[@role='progressbar']/span");

        #endregion

        #region Methods

        #region Checklist

        public void EnterCheckListTitle(string title)
        {
            Log.Step(nameof(CheckListTabPage), "Add Checklist Title");
            var newRowIndex = Driver.GetElementCount(GetChecklistCount);
            Wait.UntilElementVisible(CheckListTitle(newRowIndex)).ClearTextbox();
            Driver.JavaScriptScrollToElement(CheckListTitle(newRowIndex)).SetText(title, isReact: true);
        }
        public void SelectOwner(IEnumerable<string> owners, int rowIndex = 0)
        {
            Log.Step(nameof(CheckListTabPage), "Click on 'Select Owners'");
            if (rowIndex == 0)
            {
                rowIndex = Driver.GetElementCount(GetChecklistCount);
            }

            Log.Step(nameof(CheckListTabPage), "Click on 'Select Owners'");
            if (owners.ToList().Count <= 0) return;
            foreach (var owner in owners)
            {
                
                Driver.JavaScriptClickOn(Wait.UntilElementClickable(SelectOwner(rowIndex)));
                var element = Wait.UntilElementExists(SelectOwner(rowIndex)).SetText(owner);
                Wait.HardWait(3000);
                element.SetText(owner).Click();
                Wait.HardWait(4000);//Wait Added to search for Owners
                var ownerDropdown = Wait.UntilElementExists(CheckListOwnerDropdown(owner));
                ownerDropdown.Click();
            }
        }

        public void ClickOnAddChecklistButton()
        {
            Log.Step(nameof(CheckListTabPage), "Click on 'Add Check List' button");
            Driver.JavaScriptScrollToElement(AddChecklistItemButton).Click();
        }

        public void EnterCheckListItem(UpdateBusinessOutcomeChecklistItemRequest request)
        {
            Log.Step(nameof(CheckListTabPage), "Enter new checklist details");
            ClickOnAddChecklistButton();
            EnterCheckListTitle(request.ItemText);
            SelectOwner(request.Owners);
        }

        public List<BusinessOutcomeChecklistItemResponse> GetCheckListResponse()
        {
            Log.Step(nameof(CheckListTabPage), "Get the created Checklist response");
            var checklistCount = Wait.UntilAllElementsLocated(GetChecklistCount).Count;
            var list = new List<BusinessOutcomeChecklistItemResponse>();

            for (var i = 1; i <= checklistCount; i++)
            {
                var checkListTitle = Wait.UntilElementVisible(CheckListTitle(i)).GetText();
                var owner = Wait.UntilElementExists(OwnerName(i)).GetText();
                list.Add(new BusinessOutcomeChecklistItemResponse()
                {
                    ItemText = checkListTitle,
                    Owners = new List<BusinessOutcomeChecklistOwnerResponse>()
            {
                new BusinessOutcomeChecklistOwnerResponse()
                {
                    DisplayName = owner
                }
            },
                });
            }

            return list;
        }

        public string GetChecklistProgressPercentage()
        {
            Log.Step(nameof(CheckListTabPage), "Get the Checklist Progress Percentage");
            return Wait.UntilElementVisible(ChecklistProgressPercentage).GetText();
        }

        public string GetChecklistProgressIndication()
        {
            Log.Step(nameof(CheckListTabPage), "Get the Checklist Progress Indication");
            return Wait.UntilElementVisible(ChecklistProgressIndication).GetElementAttribute("style");
        }

        public void CompleteChecklistCheckBox(int completeChecklistCount)
        {
            Log.Step(nameof(CheckListTabPage), "Complete the check list check box ");
            Wait.UntilElementClickable(CompleteCheckList(completeChecklistCount)).Check();

        }
        #endregion

        #endregion
    }
}
