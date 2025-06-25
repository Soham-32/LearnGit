using System;
using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Radar;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Common;
using AgilityHealth_Automation.Utilities;
using AtCommon.Dtos.GrowthPlan.Custom;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.GrowthPlan.Add
{
    internal class AddGrowthItemPopupPage : AssessmentDetailsCommonPage
    {
        public AddGrowthItemPopupPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        private TimeSpan Timeout => Driver.Manage().Timeouts().AsynchronousJavaScript;

        private const string TargetDateCalendarId = "TargetDate_dateview";

        // Locators

        private readonly By GrowthPlanItemTab = By.XPath("*//div[@id='GrowthPlanItemTabstrip']//ul//li//a[contains(normalize-space(), 'Growth Plan Item')]");
        private readonly By SolutionTab = By.Id("SolutionTab");

        private readonly By CompetencyTargetDelete =
            By.XPath("//ul[@id = 'CompetencyTargets_taglist']//span[@class = 'k-icon k-i-close']");

        private readonly By AffectedTeamsListBox = By.XPath("//select[@id = 'AffectedTeams']/preceding-sibling::div");
        private readonly By CategoryList = By.XPath("//div[@id='Category-list']//div//ul//li[@role='option']");
        private readonly By CategoryText = By.XPath("//div/label[@for='Category']/parent::div/following-sibling::div");
        private readonly By CategoryTextbox = By.XPath("//input[@id='Category']/preceding-sibling::span");
        private readonly By AssessmentDropDown = By.XPath("//input[@id='AssessmentId']/preceding-sibling::span");

        private readonly By ColorTextbox =
            By.XPath("//input[@id='Color']/preceding-sibling::span/span[@class='k-select']");

        private readonly By CompetencyTargetTextbox =
            By.XPath("//select[@id = 'CompetencyTargets']/preceding-sibling::div");

        private readonly By DescriptionTextbox = By.CssSelector("body[contenteditable='true']");
        private readonly By DeleteOwnerSymbol = By.CssSelector(".spn-owner-label~div .k-i-close");
        private readonly By DescriptionIframe = By.XPath("//iframe[@aria-label='Description']");
        private readonly By DescriptionPlaceholder = By.Id("divplaceholder");
        private readonly By OwnerListBox = By.XPath("//select[@id = 'OwnersList']/preceding-sibling::div");
        private readonly By PriorityTextbox = By.XPath("//input[@id='Priority']/preceding-sibling::span");
        private readonly By RadarTypeTextbox = By.XPath("//input[@id='radartype']//ancestor::span[@aria-owns='radartype_listbox']");
        private readonly By RankIncreaseIcon = By.XPath("//input[@data-bind='value:Rank']//parent::span//span[@title='Increase value']");
        private readonly By RankDecreaseIcon = By.XPath("//input[@data-bind='value:Rank']//parent::span//span[@title='Decrease value']");
        private readonly By RankTextbox = By.XPath("//input[@id = 'Rank']/preceding-sibling::input//following-sibling::input");
        private readonly By SaveButton = By.CssSelector("a.k-grid-update");
        private readonly By CancelButton = By.CssSelector("a.k-grid-cancel");
        private readonly By SizeTextbox = By.XPath("//input[@id='Size']/preceding-sibling::span");
        private readonly By StatusTextbox = By.XPath("//input[@id='StatusId']/preceding-sibling::span");
        private readonly By TitleLabel = By.CssSelector("label[for='Title']");
        private readonly By TitleTextBox = By.Id("Title");
        private readonly By TypeTextbox = By.XPath("//input[@id='Type']/preceding-sibling::span");
        private readonly By AddGpItemHeaderText = By.XPath("//span//*[text()='Add Growth Plan Item'] | //span[text()='Add Growth Plan Item']");
        private readonly By EditGpItemHeaderText = By.XPath("//span//*[text()='Edit Growth Plan Item'] | //span[text()='Edit Growth Plan Item']");
        private static By FieldValidationMessage(string fieldName) => By.Id($"{fieldName}_validationMessage");

        private static By CategoryListItem(string category) => By.XPath($"//ul[@id='Category_listbox']//*[text()='{category}']");
        private static By AssessmentListItem(string assessment) => By.XPath($"//ul[@id='AssessmentId_listbox']/li[contains(normalize-space(), '{assessment}')]");
        private static By TypeListItem(string type) => By.XPath($"//ul[@id='Type_listbox']/li[text()='{type}'] | //ul[@id='Type_listbox']/li//font[text()='{type}']");

        private readonly By TypeListItems = By.XPath("//ul[@id='Type_listbox']/li");
        private static By RadarTypeListItem(string radarType) => By.XPath($"//ul[@id='radartype_listbox']/li[text()='{radarType}'] | //ul[@id='radartype_listbox']/li//font[text()='{radarType}']");
        private static By StatusListItem(string status) => By.XPath($"//ul[@id='StatusId_listbox']/li[text()='{status}'] | //ul[@id='StatusId_listbox']/li//font[text()='{status}']");
        private static By PriorityListItem(string priority) => By.XPath($"//ul[@id='Priority_listbox']/li[text() = '{priority}'] | //ul[@id='Priority_listbox']/li//font[text() = '{priority}']");
        private static By OwnerListItem(string item) => By.XPath($"//ul[@id = 'OwnersList_listbox']/li[text() = '{item}'] | //ul[@id = 'OwnersList_listbox']/li//font[text() = '{item}']");
        private static By SizeListItem(string size) => By.XPath($"//ul[@id='Size_listbox']/li[contains(normalize-space(),'{size}')]");
        private static By AffectedTeamsListItem(string item) => By.XPath($"//ul[@id = 'AffectedTeams_listbox']/li[text() = '{item}']");
        private static By ColorListItem(string color) => By.XPath($"//td[@aria-label='{color}']");
        private static By CompetencyTargetListItem(string item) => By.XPath($"//ul[@id='CompetencyTargets_listbox']//li[contains(text(),'{item}')] | //ul[@id = 'CompetencyTargets_listbox']/li//font[text()= '{item}']");
        private readonly By ColorTextboxValue = By.XPath("//input[@id='Color']/preceding-sibling::span//parent::span");
        private readonly By RadarTypeDropDown = By.XPath("//span[@aria-owns='radartype_listbox']//span//span");
        //RadarType Tooltip Icon 
        private readonly By RadarTypeTooltipIcon = By.Id("radarTypeHelper");

        //RadarType Tooltip Icon Message
        private readonly By RadarTypeTooltipMessage = By.CssSelector(".tooltipster-box #tooltip_contentRadarType");


        // Methods

        public void EnterGrowthItemInfo(GrowthItem growthItem, bool deleteCompetency = true)
        {
            Log.Step(nameof(AddGrowthItemPopupPage), "Enter growth item info");
            Wait.UntilJavaScriptReady(Timeout);

            // Rank
            if (!string.IsNullOrEmpty(growthItem.Rank))
                SetRankValue(growthItem.Rank);

            // Category
            if (!string.IsNullOrEmpty(growthItem.Category))
                SelectGiCategory(growthItem.Category);

            // Assessment
            if (!string.IsNullOrEmpty(growthItem.Assessment))
                SelectGiAssessment(growthItem.Assessment);

            // Type
            if (!string.IsNullOrEmpty(growthItem.Type))
                SelectGiType(growthItem.Type);

            // Title
            if (!string.IsNullOrEmpty(growthItem.Title))
                SetGiTitle(growthItem.Title);

            // Owner
            if (!string.IsNullOrEmpty(growthItem.Owner)) SelectOwner(growthItem.Owner);

            // Status
            SelectStatus(growthItem.Status);

            // Target Date
            if (growthItem.TargetDate.HasValue)
            {
                Wait.UntilJavaScriptReady(Timeout);

                var targetDateCal = new CalendarWidget(Driver, TargetDateCalendarId);
                Wait.HardWait(3000); // Wait Until Calender Displayed
                targetDateCal.SetDate(growthItem.TargetDate.Value);
            }

            // Priority
            if(!string.IsNullOrEmpty(growthItem.Priority))
                SelectPriority(growthItem.Priority);

            // Size
            if (!string.IsNullOrEmpty(growthItem.Size)) SelectItem(SizeTextbox, SizeListItem(growthItem.Size));

            // Affected Teams
            if (!string.IsNullOrEmpty(growthItem.AffectedTeams))
                SelectItem(AffectedTeamsListBox, AffectedTeamsListItem(growthItem.AffectedTeams));

            // Color
            if (!string.IsNullOrEmpty(growthItem.Color)) SelectColor(growthItem.Color);

            if (!string.IsNullOrEmpty(growthItem.RadarType)) SelectGiRadarType(growthItem.RadarType);

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
            if (!string.IsNullOrEmpty(growthItem.Description))
            {
                Driver.SwitchToFrame(DescriptionIframe);
                Wait.UntilJavaScriptReady(Timeout);
                if (Driver.IsElementPresent(DescriptionPlaceholder))
                    Wait.UntilElementClickable(DescriptionPlaceholder).Click();

                Wait.UntilElementClickable(DescriptionTextbox).SetText(growthItem.Description);
                Driver.SwitchTo().DefaultContent();
                Wait.UntilJavaScriptReady(Timeout);
            }
        }

        public void SetRankValue(string rankValue)
        {

            var currentRankValue = GetRankValue();

            if (currentRankValue == "")
            {
                currentRankValue = "0";
            }
            var value = int.Parse(currentRankValue) - int.Parse(rankValue);

            if (value > 0)
            {
                for (var i = 0; i < value; i++)
                {
                    Wait.UntilElementClickable(RankDecreaseIcon).Click();
                }
            }
            else
            {
                value = Math.Abs(value);
                for (var i = 0; i < value; i++)
                {
                    Wait.UntilElementClickable(RankIncreaseIcon).Click();
                }
            }
        }

        public string GetRankValue()
        {
            return Wait.UntilElementExists(RankTextbox).GetAttribute("aria-valuenow");
        }

        public void SelectGiCategory(string category)
        {
            SelectItem(CategoryTextbox, CategoryListItem(category));
        }

        public void SelectGiAssessment(string assessment)
        {
            SelectItem(AssessmentDropDown, AssessmentListItem(assessment));
        }

        public void SetGiTitle(string title)
        {
            Wait.UntilElementClickable(TitleTextBox).SetText(title);
            Wait.UntilElementClickable(TitleLabel)
                .Click(); //Click on Title label as validation text appears on editing, and that creates problem on Status selection (while editing)

        }

        public void SelectCompetencyTarget(List<string> competencyTarget)
        {
            foreach (var competency in competencyTarget)
                SelectItem(CompetencyTargetTextbox, CompetencyTargetListItem(competency));
        }

        public void SelectGiType(string type)
        {
            SelectItem(TypeTextbox, TypeListItem(type));
        }

        private void SelectOwner(string owner)
        {
            DeleteOwner();
            SelectItem(OwnerListBox, OwnerListItem(owner));
        }

        public bool IsRadarTypeEnabled()
        {
            return !Boolean.Parse(Wait.UntilElementClickable(RadarTypeTextbox).GetAttribute("aria-disabled"));
        }

        public bool IsRadarTypeTooltipIconDisplayed()
        {
            return Driver.IsElementDisplayed(RadarTypeTooltipIcon);
        }

        public bool IsAssessmentFieldDisplayed()
        {
            return Driver.IsElementDisplayed(AssessmentDropDown);
        }

        private void SelectGiRadarType(string radarType)
        {
            if (Wait.InCase(RadarTypeTextbox) != null && IsRadarTypeEnabled())
            {
                SelectItem(RadarTypeTextbox, RadarTypeListItem(radarType));
            }
        }

        private void DeleteOwner()
        {
            if (!Driver.IsElementPresent(DeleteOwnerSymbol)) return;
            Wait.UntilJavaScriptReady(Timeout);
            Wait.UntilElementClickable(DeleteOwnerSymbol).Click();
            Wait.UntilJavaScriptReady(Timeout);
        }

        public void SelectStatus(string status)
        {
            SelectItem(StatusTextbox, StatusListItem(status));
        }
        public void SelectPriority(string priority)
        {
            SelectItem(PriorityTextbox, PriorityListItem(priority));
        }

        private void SelectColor(string color)
        {
            SelectItem(ColorTextbox, ColorListItem(color));
        }

        public bool IsFieldValidationMessageDisplayed(string fieldName)
        {
            return Driver.IsElementDisplayed(FieldValidationMessage(fieldName));
        }

        public void ClickSaveButton(bool isOnNoteSection=false)
        {
            Log.Step(nameof(AddGrowthItemPopupPage), "Click on Save button");
            if(!isOnNoteSection)
            {
                Wait.UntilElementClickable(TitleLabel).Click();
            }
            Wait.UntilElementClickable(SaveButton).Click();
            Wait.UntilJavaScriptReady(Timeout);
            Wait.HardWait(2000); //Wait till the growth item saved
        }

        public void ClickCancelButton()
        {
            Log.Step(nameof(AddGrowthItemPopupPage), "Click on Cancel button");
            Wait.UntilElementClickable(CancelButton).Click();
            Wait.UntilJavaScriptReady(Timeout);
        }

        public IList<string> GetTypeListItems()
        {
            return Driver.GetAttributeFromAllElements(TypeListItems, "textContent");
        }
        public void ClickOnCategoryDropDown()
        {
            Log.Step(nameof(AddGrowthItemPopupPage), "Click on 'Category' drop-Down");
            Wait.UntilElementClickable(CategoryTextbox).Click();
            Wait.UntilJavaScriptReady();
        }

        public IList<string> GetCategoryList()
        {
            Log.Step(nameof(AddGrowthItemPopupPage), "Get 'Category' list");
            return Wait.UntilAllElementsLocated(CategoryList).Select(a => Driver.JavaScriptScrollToElement(a).GetText()).ToList();
        }

        public string GetCategoryText()
        {
            Log.Step(nameof(AddGrowthItemPopupPage), "Get 'Category' text");
            return Wait.UntilElementVisible(CategoryText).GetText();
        }

        public string GetTitleValue()
        {
            Log.Step(nameof(AddGrowthItemPopupPage), "Get 'Title' Value");
            Wait.UntilJavaScriptReady();
            return Wait.UntilElementVisible(TitleTextBox).GetText();
        }
        public string GetStatusValue()
        {
            Log.Step(nameof(AddGrowthItemPopupPage), "Get 'Status' Value");
            Wait.UntilJavaScriptReady();
            return Wait.UntilElementVisible(StatusTextbox).GetText().Replace("\r\nselect", "");
        }
        public string GetPriorityValue()
        {
            Log.Step(nameof(AddGrowthItemPopupPage), "Get 'Priority' Value");
            Wait.UntilJavaScriptReady();
            return Wait.UntilElementVisible(PriorityTextbox).GetText().Replace("\r\nselect", "");
        }
        public string GetCategoryValue()
        {
            Log.Step(nameof(AddGrowthItemPopupPage), "Get 'Category' Value");
            Wait.UntilJavaScriptReady();
            return Wait.UntilElementVisible(CategoryTextbox).GetText().Replace("\r\nselect", "");
        }
        public string GetTypeValue()
        {
            Log.Step(nameof(AddGrowthItemPopupPage), "Get 'Type' Value");
            Wait.UntilJavaScriptReady();
            return Wait.UntilElementVisible(TypeTextbox).GetText().Replace("\r\nselect", "");
        }
        public string GetColorValue()
        {
            Log.Step(nameof(AddGrowthItemPopupPage), "Get 'Color' Value");
            Wait.UntilJavaScriptReady();
            return Wait.UntilElementVisible(ColorTextboxValue).GetAttribute("aria-label").Replace("Current selected color is ", "");
        }
        public string GetDescription()
        {
            Log.Step(nameof(AddGrowthItemPopupPage), "Get 'Description' text");
            Driver.SwitchToFrame(DescriptionIframe);
            if (Driver.IsElementPresent(DescriptionPlaceholder))
                Wait.UntilElementClickable(DescriptionPlaceholder).Click();
            var description = Wait.UntilElementClickable(DescriptionTextbox).GetText();
            Driver.SwitchTo().DefaultContent();
            Wait.UntilJavaScriptReady(Timeout);
            return description;
        }

        public string GetSizeValue()
        {
            Log.Step(nameof(AddGrowthItemPopupPage), "Get 'Size' Value");
            Wait.UntilJavaScriptReady();
            return Wait.UntilElementVisible(SizeTextbox).GetText().Replace("\r\nselect", "");
        }

        public string GetRadarTypeValue()
        {
            Log.Step(nameof(AddGrowthItemPopupPage), "Get 'Radar Type' Value");
            return Wait.UntilElementVisible(RadarTypeDropDown).GetText();
        }
        public string GetRadarTypeTooltipMessage()
        {
            Log.Step(nameof(AddGrowthItemPopupPage), "Get 'Radar Type' tool tip message");
            Driver.MoveToElement(Wait.UntilElementClickable(RadarTypeTooltipIcon));
            return Wait.UntilElementVisible(RadarTypeTooltipMessage).GetText();
        }

        public bool IsAddGrowthPlanItemHeaderTextDisplayed()
        {
            return Driver.IsElementDisplayed(AddGpItemHeaderText);
        }
        public bool IsEditGrowthPlanItemHeaderTextDisplayed()
        {
            return Driver.IsElementDisplayed(EditGpItemHeaderText);
        }
        // GI Tabs
        public void ClickOnSolutionTab()
        {
            Log.Step(nameof(AddGrowthItemNotePage), "Click On Solution Tab");
            Wait.UntilElementClickable(SolutionTab).Click();
            Wait.UntilJavaScriptReady();
        }
        public void ClickOnGrowthPlanItemTab()
        {
            Log.Step(nameof(AddGrowthItemNotePage), "Click On Growth Plan Item Tab");
            Wait.UntilElementClickable(GrowthPlanItemTab).Click();
            Wait.UntilJavaScriptReady();
        }
    }
}