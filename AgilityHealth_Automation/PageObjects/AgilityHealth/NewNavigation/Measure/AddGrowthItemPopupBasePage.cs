using AgilityHealth_Automation.PageObjects.AgilityHealth.Common;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using AgilityHealth_Automation.DataObjects.NewNavigation.GrowthPlan;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.NewNavigation.Measure
{
    public class AddGrowthItemPopupBasePage : CommonGrowthItemsDashboardBasePage
    {
        public AddGrowthItemPopupBasePage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }
        private TimeSpan Timeout => Driver.Manage().Timeouts().AsynchronousJavaScript;
        private const string TargetDateCalendarId = "TargetDate_dateview";

        #region Locators
        private readonly By TitleTextbox = By.Id("Title");
        private readonly By StatusDropdown = By.CssSelector("[aria-owns='StatusId_listbox']");
        private readonly By CompetencyTargetDelete =
            By.CssSelector("#CompetencyTargets_taglist .k-i-close");
        private readonly By PriorityDropdown = By.CssSelector("[aria-owns='Priority_listbox']");
        private readonly By CategoryDropdown = By.CssSelector("[aria-owns='Category_listbox']");
        private readonly By TypeDropdown = By.CssSelector("[aria-owns='Type_listbox']");
        private readonly By RadarTypeDropdown = By.CssSelector("[aria-owns='radartype_listbox']");
        private readonly By CompetencyTargetDropdown = By.XPath("//div[./select[@id='CompetencyTargets']]");
        private readonly By RankTextbox = By.CssSelector(".k-numerictextbox input.k-formatted-value");
        private readonly By OwnerDropdown = By.XPath("//div[./ul[@id='OwnersList_taglist']]");
        private readonly By DeleteOwnerSymbol = By.CssSelector("#OwnersList_taglist .k-i-close");
        private readonly By SizeDropdown = By.CssSelector("[aria-owns='Size_listbox']");
        private readonly By ColorDropdown = By.XPath("//span[./input[@id='Color']]//span[@class='k-select']");
        private readonly By AffectedTeamsDropdown = By.XPath("//div[./select[@id='AffectedTeams']]");
        private readonly By DescriptionTextbox = By.CssSelector("body[contenteditable='true']");
        private readonly By DescriptionIframe = By.CssSelector("iframe[aria-label='Description']");
        private readonly By SaveButton = By.CssSelector(".k-button.k-button-icontext.k-primary.k-grid-update");
        private readonly By CancelButton = By.CssSelector("a.k-grid-cancel");
        private static By StatusDropdownItem(string item) => By.XPath($"//ul[@id='StatusId_listbox']/li[text()='{item}']");
        private static By PriorityDropdownItem(string item) => By.XPath($"//ul[@id='Priority_listbox']/li[text()='{item}']");
        private static By CategoryDropdownItem(string item) => By.XPath($"//ul[@id='Category_listbox']/li[text()='{item}']");
        private static By TypeDropdownItem(string item) => By.XPath($"//ul[@id='Type_listbox']/li[text()='{item}']");
        private static By RadarTypeDropdownItem(string item) => By.XPath($"//ul[@id='radartype_listbox']/li[text()='{item}']");
        private static By CompetencyTargetDropdownItem(string item) => By.XPath($"//ul[@id='CompetencyTargets_listbox']/li[text()='{item}']");
        private static By OwnerDropdownItem(string item) => By.XPath($"//ul[@id='OwnersList_listbox']/li[text()='{item}']");
        private static By SizeDropdownItem(string item) => By.XPath($"//ul[@id='Size_listbox']/li[text()='{item}']");
        private static By ColorDropdownItem(string item) => By.CssSelector($"[data-role='colorpicker'] td[aria-label='{item}']");
        private static By AffectedTeamsDropdownItem(string item) => By.XPath($"//ul[@id='AffectedTeams_listbox']/li[text()='{item}']");
        private static By FieldValidationMessage(string fieldName) => By.Id($"{fieldName}_validationMessage");
        #endregion

        #region Methods
        public void EnterGrowthItemInfo(GrowthItem growthItem, bool deleteCompetency = true)
        {
            Log.Step(nameof(AddGrowthItemPopupBasePage), "Enter growth item info");
            Wait.UntilJavaScriptReady(Timeout);

            // Rank
            if (!string.IsNullOrEmpty(growthItem.Rank))
            {
                Wait.UntilElementVisible(RankTextbox).SetText(growthItem.Rank);
            }

            // Category
            if (!string.IsNullOrEmpty(growthItem.Category))
                SelectCategory(growthItem.Category);

            // Type
            if (!string.IsNullOrEmpty(growthItem.Type))
                SelectType(growthItem.Type);

            // Title
            if (!string.IsNullOrEmpty(growthItem.Title))
                SetTitle(growthItem.Title);

            // Owner
            if (!string.IsNullOrEmpty(growthItem.Owner))
            {
                DeleteOwner();
                SelectItem(OwnerDropdown, OwnerDropdownItem(growthItem.Owner));
            }

            // Status
            if (!string.IsNullOrEmpty(growthItem.Status))
                SelectStatus(growthItem.Status);

            // Target Date
            if (growthItem.TargetDate.HasValue)
            {
                Wait.UntilJavaScriptReady(Timeout);

                var targetDateCal = new CalendarWidget(Driver, TargetDateCalendarId);
                targetDateCal.SetDate(growthItem.TargetDate.Value);
            }

            // Priority
            if (!string.IsNullOrEmpty(growthItem.Priority))
                SelectPriority(growthItem.Priority);

            // Size
            if (!string.IsNullOrEmpty(growthItem.Size)) SelectItem(SizeDropdown, SizeDropdownItem(growthItem.Size));

            // Affected Teams
            if (!string.IsNullOrEmpty(growthItem.AffectedTeams))
                SelectItem(AffectedTeamsDropdown, AffectedTeamsDropdownItem(growthItem.AffectedTeams));

            // Color
            if (!string.IsNullOrEmpty(growthItem.Color)) SelectItem(ColorDropdown, ColorDropdownItem(growthItem.Color));

            // Radar Type
            if (!string.IsNullOrEmpty(growthItem.RadarType)) SelectRadarType(growthItem.RadarType);

            // Competency Target
            if (deleteCompetency)
                if (Driver.IsElementPresent(CompetencyTargetDelete))
                {
                    var deleteButtons = Wait.UntilAllElementsLocated(CompetencyTargetDelete);
                    foreach (var button in deleteButtons)
                    {
                        button.Click();
                        Wait.UntilJavaScriptReady(Timeout);
                    }
                }
            Wait.HardWait(1000); // Wait after deleting all previous competencies
            if (growthItem.CompetencyTargets != null)
                SelectCompetencyTarget(growthItem.CompetencyTargets);

            // Description
            if (string.IsNullOrEmpty(growthItem.Description)) return;
            Driver.SwitchToFrame(DescriptionIframe);
            Wait.UntilJavaScriptReady(Timeout);
            Wait.UntilElementClickable(DescriptionTextbox).SetText(growthItem.Description);
            Driver.SwitchTo().ParentFrame();
            Wait.UntilJavaScriptReady(Timeout);
        }

        public void SelectCategory(string category)
        {
            Log.Step(nameof(AddGrowthItemPopupBasePage), $"Select category {category}");
            SelectItem(CategoryDropdown, CategoryDropdownItem(category));
        }

        public void SelectType(string type)
        {
            Log.Step(nameof(AddGrowthItemPopupBasePage), $"Select type {type}");
            SelectItem(TypeDropdown, TypeDropdownItem(type));
        }

        public void SelectStatus(string status)
        {
            Log.Step(nameof(AddGrowthItemPopupBasePage), $"Select status {status}");
            SelectItem(StatusDropdown, StatusDropdownItem(status));
        }

        public void SelectPriority(string priority)
        {
            Log.Step(nameof(AddGrowthItemPopupBasePage), $"Select priority {priority}");
            SelectItem(PriorityDropdown, PriorityDropdownItem(priority));
        }

        public void SelectCompetencyTarget(List<string> competencyTarget)
        {
            Log.Step(nameof(AddGrowthItemPopupBasePage), $"Select competency target {competencyTarget}");
            foreach (var competency in competencyTarget)
                SelectItem(CompetencyTargetDropdown, CompetencyTargetDropdownItem(competency));
        }

        public void SelectRadarType(string radarType)
        {
            Log.Step(nameof(AddGrowthItemPopupBasePage), $"Select radar type {radarType}");
            SelectItem(RadarTypeDropdown, RadarTypeDropdownItem(radarType));
        }

        public void SetTitle(string title)
        {
            Log.Step(nameof(AddGrowthItemPopupBasePage), $"Set title {title}");
            Wait.UntilElementVisible(TitleTextbox).SetText(title);
        }

        private void DeleteOwner()
        {
            if (!Driver.IsElementPresent(DeleteOwnerSymbol)) return;
            Wait.UntilJavaScriptReady(Timeout);
            Wait.UntilElementClickable(DeleteOwnerSymbol).Click();
            Wait.UntilJavaScriptReady(Timeout);
        }

        public void ClickOnSaveButton()
        {
            Log.Step(nameof(AddGrowthItemPopupBasePage), "Click on Save button");
            Driver.JavaScriptScrollToElement(SaveButton);
            Wait.UntilJavaScriptReady();
            Wait.UntilElementClickable(SaveButton).Click();
            Wait.UntilJavaScriptReady(Timeout);
        }

        public void ClickOnCancelButton()
        {
            Log.Step(nameof(AddGrowthItemPopupBasePage), "Click on Cancel button");
            Wait.UntilElementClickable(CancelButton).Click();
            Wait.UntilJavaScriptReady(Timeout);
        }

        public bool IsFieldValidationMessageDisplayed(string fieldName)
        {
            return Driver.IsElementDisplayed(FieldValidationMessage(fieldName));
        }
        #endregion
    }
}
