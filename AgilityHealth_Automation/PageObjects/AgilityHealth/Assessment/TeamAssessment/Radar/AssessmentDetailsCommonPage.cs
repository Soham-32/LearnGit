using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.Enum.Radar;
using AgilityHealth_Automation.Utilities;
using AtCommon.Utilities;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Radar
{
    internal class AssessmentDetailsCommonPage : BasePage
    {
        public AssessmentDetailsCommonPage(IWebDriver driver, ILogger log) : base(driver, log) { }

        private TimeSpan Timeout => Driver.Manage().Timeouts().AsynchronousJavaScript;

        //Header
        private readonly By AssessmentTitle = By.XPath("//div[@id='slider_inner']//h1");

        //Radar
        private readonly By RadarViewDropdownArrow = By.CssSelector("span[aria-owns='radarSelect_listbox']");
        private static By RadarViewItem(string item) => By.XPath($"//ul[@id='radarSelect_listbox']//li[. = '{item}' or .//font[text()='{item}']]");

        //Benchmarking options popup
        private readonly By BenchMarkingPopup = By.Id("benchmarkingOptions");
        private readonly By BenchmarkingPopupTitleText = By.Id("benchmarkingOptions_wnd_title");
        private readonly By BenchmarkingPopupInfoText = By.Id("benchmarkingOptions");
        private readonly By BenchmarkingPopupBenchmarkingOptionsFieldText = By.XPath("//div[@id='benchmarkingOptions']//label[@id='label_BenchmarkingOption']");
        private readonly By BenchmarkingOptionsDropdown = By.CssSelector("span[aria-owns='BenchmarkingOption_listbox']");
        private readonly By BenchmarkingOptionsDropdownValuesList = By.XPath("//ul[@id='BenchmarkingOption_listbox']//li[contains(@class,'k-item')]");
        private static By BenchmarkingOptionsDropdownValue(string item) => By.XPath($"//ul[@id='BenchmarkingOption_listbox']/li[.//text()='{item}']");

        //Benchmarking options popup work type
        private readonly By BenchmarkingPopupWorkTypeFieldText = By.XPath("//div[@id='benchmarkingWorkType']/div/label");

        //Benchmarking options popup maturity 
        private readonly By BenchmarkingPopupMaturityFieldText = By.XPath("//div[@id='benchmarkingMaturity']/div/label");

        //Benchmarking options popup cancel and select buttons
        private readonly By BenchmarkingPopupCancelAndSelectButtonsTextList = By.XPath("//button[contains(@id,'Benchmarking')]");
        private readonly By BenchmarkingPopupCancelButton = By.Id("cancelBenchmarking");

        //Comment Section
        private readonly By TeamCommentsTitleText = By.CssSelector(".teamComments");

        //Hide All Assessment Comments (Presenter view)
        private readonly By HideAllCommentsIcon = By.Id("hideAllComments");
        private readonly By HideAllCommentsConfirmationPopUpYesButton = By.Id("do_UpdateComments");

        private readonly By HeaderText = By.Id("HideUnHideCommentsView_wnd_title");
        //Notes
        private readonly By NotesTable = By.Id("DimensionNotesGrid");
        private readonly By NotesTableUpdateButton = By.CssSelector("#DimensionNotesGrid a.k-grid-update");

        //Hide All Team/Stakeholder Comments button
        private readonly By HideAllTeamCommentsButton = By.Id("hideAllTeamComments");
        private readonly By UnHideAllTeamCommentsButton = By.Id("unHideAllTeamComments");
        private readonly By HideAllStakeholderCommentsButton = By.Id("hideAllStakeHolderComments");
        private readonly By UnHideAllStakeholderCommentsButton = By.Id("unHideAllStakeHolderComments");
        private readonly By HideUnHideAllCommentsConfirmationPopupYesButton = By.Id("do_UpdateComments_dim");

        //Hide/UnHide all team/stakeholder comments popup  
        private readonly By HideUnHideAllTeamStakeholderCommentsPopupTitleText = By.Id("HideUnHideCommentsView_dim_wnd_title");
        private readonly By HideUnHideAllTeamStakeholderCommentsPopupInfoTextList = By.XPath("//div[@id='HideUnHideCommentsView_dim']/div/div[not(contains(@id,'mode_dim'))]");
        private readonly By HideUnHideAllTeamStakeholderCommentsPopupNoCancelButton = By.Id("do_cancel_dim");

        //Open-ended questions button
        private readonly By CommentsEditButtonTextList = By.CssSelector("a.k-button.k-button-icontext.k-grid-edit");
        private static By CommentsEditButton(string comment) => By.XPath(
            $"//div[@id='DimensionNotesGrid']//tbody//td[contains(text(),'{comment}')]//following-sibling::td//a[contains(@class,'k-grid-edit')]");
        private readonly By CommentsUpdateButtonText = By.XPath("//div[@id='DimensionNotesGrid']//tbody//following-sibling::tr/td/a[contains(@class,'k-grid-update')]");
        private readonly By CommentsCancelButtonText = By.XPath("//div[@id='DimensionNotesGrid']//tbody//following-sibling::tr/td/a[contains(@class,'k-grid-cancel')]");
        private readonly By CommentsHideButtonTextList = By.CssSelector("a.k-button.k-button-icontext.k-grid-HideComment");
        private static By CommentsHideUnHideButtons(string comment) => By.XPath($"//div[@id='DimensionNotesGrid']//tbody//td[contains(text(),'{comment}')]//following-sibling::td//a[contains(@class,'k-grid-HideComment')]");

        //Improvements, Impediments, Strengths, Growth Plan titles
        private readonly By ImpedimentsTitleText = By.Id("titleImpediments");
        private readonly By StrengthsTitleText = By.Id("titleStrengths");
        private readonly By GrowthOpportunitiesTitleText = By.XPath("//h5[@id='titleGrowthOpportunities']//font | //h5[@id='titleGrowthOpportunities']");
        private readonly By GrowthPlanTitleText = By.XPath("//i[@class='title-growth-plan-ico']/parent::h5");
        private readonly By EditButton = By.XPath("//a[@title='Edit Growth Plan Item']");

        //Improvements, Impediments, Strengths, Growth Plan description 
        private static By DescriptionText(string sectionName) => By.XPath($"//div[@id='{sectionName}']//table//th[@data-field='Answer']/a");
        private readonly By GrowthPlanInfoTextList = By.XPath("//div[@class='assess-item']//div[contains(@id,'msgAssessItem')]");

        private static By NotesOpenEndedSurveyItem(string section) =>
            By.XPath($"//*[@id = '{section.Replace(' ', '_')}']//tbody/tr/td[2]");
        private static By NotesOpenEndedQuestions(string section, string note) =>
            By.XPath($"//*[@id = '{section.Replace(' ', '_')}']//tbody/tr/td[text()='{note}']");

        private readonly By NoteUpdateButton = By.CssSelector("a.k-grid-update");

        private static By NoteSection(string section) => By.XPath($" //h5[starts-with(text(), '{section}')]/following-sibling::div[1] | //font[starts-with(text(),'{section}')]//..//..//following-sibling::div[1]");

        private static By CommentHideUnHideButton(string subDimensionName, string comment) => By.XPath(
            $"//div[@id='DimensionNotesGrid']//tbody//tr[@name='{subDimensionName}']//td[contains(text(),'{comment}')]//following-sibling::td//a[contains(@class,'k-grid-HideComment')]");
        private static By CommentGrayedOut(string subDimensionName, string comment) => By.XPath($"*//div[@id='DimensionNotesGrid']//td[text()='{comment}']//parent::tr[@name='{subDimensionName}' and contains(@class,'backgroundRowGray')]");

        //Metrics Summary
        private static By MetricsSummary_DynamicTabLocator(string tabName) =>
            By.XPath($"//div[@id='metric_summary_tabstrip']//span[contains(text(),'{tabName}')]");

        private readonly By MetricsSummaryHeader = By.Id("titleMetricsSummary");
        private readonly By YesButton = By.Id("yesButton");

        protected readonly By GrowthPlanSectionMarker = By.Id("growth_plan");
        private readonly By GrowthPlanGridView = By.XPath("//div[@id='toggle-header']/a[@class='handle ico ease']");
        private readonly By GrowthPlanKanbanView = By.XPath("//div[@id='toggle-header']/a[@class='handle ico ease on']");
        private readonly By ToggleButton = By.Id("toggle-header");

        private static By SurveyNoteItem(string section) =>
            By.XPath($"//*[@id = 'DimensionNotesGrid']//td[text() = '{section}']/following-sibling::td[1]");

        private static By DynamicRadarDot(string dotType, string color, string competencyName) =>
            By.XPath($"//*[contains(@class,'{dotType}')][@fill='{color}'][@competency='{competencyName}']");

        private static By DimensionNotesRow(string competency) =>
            By.CssSelector($"#DimensionNotesGrid tr[name^='{competency}']");
        private static By DimensionSurveyNote(string subDimensionName, string comment) => By.XPath(
            $"*//div[@id='DimensionNotesGrid']//td[text()='{comment}']//parent::tr[@name='{subDimensionName}']");

        private static By FilterCheckbox(string item) => By.XPath($"//input[@name='{item.RemoveWhitespace()}']");

        private static By FilterLabelColor(string item) =>
            By.XPath($"//input[@name = '{item.RemoveWhitespace()}']//ancestor::li//label/span/span");

        //H5 Header
        private void MoveToHeaders(string header)
        {
            Log.Step(GetType().Name, $"Scroll the page to the <{header}> section");
            Driver.JavaScriptScrollToElement(NoteSection(header), false);
        }

        public string GetAssessmentTitleText()
        {
            Log.Step(GetType().Name, "Get Assessment Title Text");
            return Wait.UntilElementVisible(AssessmentTitle).GetText();
        }

        //Radar
        /// <summary>
        /// Get the values of plotted dots on radar
        /// </summary>
        /// <param name="dotType">dots,avg</param>
        /// <param name="color">hex code of color</param>
        /// <param name="competency">competency name</param>
        /// <returns>dot value</returns>
        public List<string> Radar_GetDotValue(string dotType, string color, string competency)
        {
            var dots = Wait.UntilAllElementsLocated(DynamicRadarDot(dotType, color, competency));
            return (from dot in dots where dot.Displayed select dot.GetAttribute("val")).ToList();
        }
        

        public void RadarSwitchView(ViewType viewType)
        {
            Log.Step(GetType().Name, $"Switch radar view to <{viewType}>");
            SelectItem(RadarViewDropdownArrow, RadarViewItem(viewType.ToString()));
        }
        private void SelectBenchmarkingOptions(string item)
        {
            Log.Step(nameof(AssessmentDetailsCommonPage), "Change benchmarking options popup dropdown value");
            SelectItem(BenchmarkingOptionsDropdown, BenchmarkingOptionsDropdownValue(item));
        }

        public bool DoesAgileCoachDotDisplay()
        {
            var agileCoachDot = By.ClassName(Filter_GetFilterItemCheckboxValueAttribute("Agile Coach") + "_avg");
            return Driver.IsElementDisplayed(agileCoachDot);
        }

        // Hide All Assessment Comments
        public void HideUnHideAllComments()
        {
            Log.Step(GetType().Name, "Click on 'Hide All Comments' icon");
            Wait.UntilElementVisible(HideAllCommentsIcon);
            Driver.JavaScriptScrollToElement(HideAllCommentsIcon, false);
            Wait.UntilElementClickable(HideAllCommentsIcon).Click();
            Wait.HardWait(3000);//wait till confirmation popup is coming
            Wait.UntilElementVisible(HideAllCommentsConfirmationPopUpYesButton);
            Wait.UntilElementClickable(HideAllCommentsConfirmationPopUpYesButton).Click();
            Wait.HardWait(3000);//wait till all comments will hide
        }
        public void ClickOnHideAllCommentsIcon()
        {
            Log.Step(GetType().Name, "Click on 'Hide All Comments' icon");
            Wait.UntilElementVisible(HideAllCommentsIcon);
            Driver.JavaScriptScrollToElement(HideAllCommentsIcon, false);
            Wait.UntilElementClickable(HideAllCommentsIcon).Click();
        }

        public string GetPopupHeaderText()
        {
            Log.Step(GetType().Name, "Get header text from hide-unhide comments popup");
            return Wait.UntilElementExists(HeaderText).GetText();
        }
        public string GetHideAllCommentsIconTitleAttribute()
        {
            Log.Step(GetType().Name, "Getting 'Title' Attribute for 'Hide All Comments' icon");
            return Wait.UntilElementExists(HideAllCommentsIcon).GetAttribute("title");
        }
        public bool IsHideAllCommentsIconDisplayed()
        {
            Log.Step(GetType().Name, "Is 'Hide All Comments' icon displayed ?");
            return Driver.IsElementDisplayed(HideAllCommentsIcon);
        }

        //Open Ended Section
        public bool IsOpenEndedNotesDisplayed(string section, string note)
        {
            var notes = GetOpenEndedNotes(section);

            return notes.Contains(note);
        }

        public List<string> GetOpenEndedNotes(string section)
        {
            MoveToHeaders(section);
            return Driver.GetTextFromAllElements(NotesOpenEndedSurveyItem(section)).ToList();
        }

        private IWebElement GetOpenEndedNoteElement(string section, string expectedNote)
        {
            var notes = Wait.UntilAllElementsLocated(NotesOpenEndedSurveyItem(section));

            return notes.FirstOrDefault(n => n.GetText().Contains(expectedNote))
                .CheckForNull($"Could not find note with text <{expectedNote}>.");
        }

        public void EditOpenEndNote(string section, string originalValue, string newValue)
        {
            Log.Step(GetType().Name, $"Change the Open Ended Note in section <{section}> from <{originalValue}> to <{newValue}>");
            MoveToHeaders(section);

            var note = GetOpenEndedNoteElement(section, originalValue);
            Wait.UntilElementClickable(note);

            //Click on Edit button
            var editButton = Wait.ForSubElement(note, By.XPath("./following-sibling::td/a"));
            Wait.UntilElementClickable(editButton).Click();

            //Entering notes
            var textarea = Wait.ForSubElement(note, By.XPath("./textarea"));
            Wait.UntilElementClickable(textarea).SetText(newValue);

            //Click on update button
            Wait.UntilElementClickable(NoteUpdateButton).Click();
            Wait.UntilElementNotExist(NoteUpdateButton);
        }

        //Notes Section
        private IWebElement GetDimensionNoteElement(string competency, string expectedNote)
        {
            OpenAllNotesSections();
            var notes = Wait.UntilAllElementsLocated(DimensionNotesRow(competency));

            return notes.FirstOrDefault(n => n.GetText().Contains(expectedNote))
                .CheckForNull($"Could not find element that contains text <{expectedNote}>.");
        }

        public bool IsDimensionNoteDisplayed(string competency, string expectedNote)
        {
            return GetDimensionNotes(competency).Any(element => element.Contains(expectedNote));
        }

        public List<string> GetDimensionNotes(string competency)
        {
            Wait.UntilJavaScriptReady();
            OpenAllNotesSections();
            return Driver.GetTextFromAllElements(SurveyNoteItem(competency)).ToList();
        }

        public void OpenAllNotesSections()
        {
            Driver.JavaScriptScrollToElement(NotesTable, false);
            // open all the notes sections
            Driver.ExecuteJavaScript("$('.k-plus').each(function() { $(this.click())})");
            Wait.UntilJavaScriptReady();
        }

        public void EditDimensionNote(string competency, string originalValue, string newValue)
        {
            Log.Step(GetType().Name, $"Change the Dimension Note for competency <{competency}> from <{originalValue}> to <{newValue}>");
            var note = GetDimensionNoteElement(competency, originalValue);
            Driver.MoveToElement(note);
            Wait.UntilElementClickable(note);

            //Click on Edit button
            var editButton = Wait.ForSubElement(note, By.TagName("a"));
            Driver.MoveToElement(editButton);
            Wait.UntilElementClickable(editButton).Click();

            //Entering notes
            var textarea = Wait.ForSubElement(note, By.TagName("textarea"));
            Wait.UntilElementClickable(textarea).SetText(newValue);

            //Click on update button
            Wait.UntilElementClickable(NotesTableUpdateButton).Click();
            Wait.UntilElementNotExist(NotesTableUpdateButton);
        }

        // Hide Assessment Comments
        public void ClickOnCommentHideUnHideButton(string subDimension, string comment)
        {
            Log.Step(GetType().Name, $"Move to {subDimension} & {comment} raw");
            var dimensionNote = GetDimensionNoteElement(subDimension, comment);
            Driver.MoveToElement(dimensionNote);

            Log.Step(GetType().Name, "Click on Hide/UnHide button");
            Wait.UntilElementClickable(CommentHideUnHideButton(subDimension, comment)).Click();
            Wait.UntilJavaScriptReady();
        }

        // Hide/UnHide All Team Comments Button
        public void ClickOnHideAllTeamCommentsButton()
        {
            Log.Step(GetType().Name, "Click on 'Hide All Team Comments' button");
            Driver.MoveToElement(HideAllTeamCommentsButton);
            Wait.UntilElementClickable(HideAllTeamCommentsButton).Click();
            Wait.UntilJavaScriptReady();

            Log.Step(GetType().Name, "Click on 'Yes Proceed' button from 'Hide All Team Comments' confirmation popup");
            Wait.UntilElementClickable(HideUnHideAllCommentsConfirmationPopupYesButton).Click();
            Wait.UntilJavaScriptReady();
        }
        public void ClickOnUnHideAllTeamCommentsButton()
        {
            Log.Step(GetType().Name, "Click on 'UnHide All Team Comments' button");
            Driver.MoveToElement(UnHideAllTeamCommentsButton);
            Wait.UntilElementClickable(UnHideAllTeamCommentsButton).Click();
            Wait.UntilJavaScriptReady();

            Log.Step(GetType().Name, "Click on 'Yes Proceed' button from 'UnHide All Team Comments' confirmation popup");
            Wait.UntilElementClickable(HideUnHideAllCommentsConfirmationPopupYesButton).Click();
            Wait.UntilJavaScriptReady();
        }
        public bool IsHideAllTeamCommentsButtonDisplayed()
        {
            Log.Step(GetType().Name, "Is 'Hide All Team Comments' button displayed?");
            return Driver.IsElementDisplayed(HideAllTeamCommentsButton);
        }
        public bool IsUnHideAllTeamCommentsButtonDisplayed()
        {
            Log.Step(GetType().Name, "Is 'Hide All Team Comments' button displayed?");
            return Driver.IsElementDisplayed(UnHideAllTeamCommentsButton);
        }

        // Hide/UnHide All Stakeholder Comments Button
        public void ClickOnHideAllStakeholderCommentsButton()
        {
            Log.Step(GetType().Name, "Click on 'Hide All Stakeholder Comments' button");
            Driver.MoveToElement(HideAllStakeholderCommentsButton);
            Wait.UntilElementClickable(HideAllStakeholderCommentsButton).Click();
            Wait.UntilJavaScriptReady();

            Log.Step(GetType().Name, "Click on 'Yes Proceed' button from 'Hide All Stakeholder Comments' confirmation popup");
            Wait.UntilElementClickable(HideUnHideAllCommentsConfirmationPopupYesButton).Click();
            Wait.UntilJavaScriptReady();
        }
        public void ClickOnUnHideAllStakeholderCommentsButton()
        {
            Log.Step(GetType().Name, "Click on 'UnHide All Stakeholder Comments' button");
            Driver.MoveToElement(UnHideAllStakeholderCommentsButton);
            Wait.UntilElementClickable(UnHideAllStakeholderCommentsButton).Click();
            Wait.UntilJavaScriptReady();

            Log.Step(GetType().Name, "Click on 'Yes Proceed' button from 'UnHide All Stakeholder Comments' confirmation popup");
            Wait.UntilElementClickable(HideUnHideAllCommentsConfirmationPopupYesButton).Click();
            Wait.UntilJavaScriptReady();
        }
        public bool IsHideAllStakeholderCommentsButtonDisplayed()
        {
            Log.Step(GetType().Name, "Is 'Hide All Stakeholder Comments' button displayed?");
            return Driver.IsElementDisplayed(HideAllStakeholderCommentsButton);
        }
        public bool IsUnHideAllStakeholderCommentsButtonDisplayed()
        {
            Log.Step(GetType().Name, "Is 'UnHide All Stakeholder Comments' button displayed?");
            return Driver.IsElementDisplayed(UnHideAllStakeholderCommentsButton);
        }

        public bool IsCommentHideButtonDisplayed(string subDimension, string comment)
        {
            Log.Step(GetType().Name, $"Is 'Hide' button displayed for Sub Dimension : {subDimension} & Comment : {comment}");
            OpenAllNotesSections();
            return Driver.IsElementDisplayed(CommentHideUnHideButton(subDimension, comment));
        }

        public bool IsCommentGrayedOut(string subDimension, string comment)
        {
            Log.Step(GetType().Name, $"Is comment grayed for Sub Dimension : {subDimension} & Comment : {comment} ? ");
            OpenAllNotesSections();
            return Driver.IsElementPresent(CommentGrayedOut(subDimension, comment));
        }


        //Filter
        public void Filter_SelectFilterItemCheckboxByName(string filterBy)
        {
            Log.Step(GetType().Name, $"Check the filter checkbox for <{filterBy}>");
            Wait.UntilElementClickable(FilterCheckbox(filterBy)).Click();
            Wait.UntilJavaScriptReady(Timeout);
        }

        public string Filter_GetFilterItemCheckboxValueAttribute(string filterBy)
        {
            return Wait.UntilElementClickable(FilterCheckbox(filterBy)).GetElementAttribute("value");
        }

        public bool Filter_IsFilterItemCheckboxSelected(string filterBy)
        {
            return Wait.UntilElementClickable(FilterCheckbox(filterBy)).Selected;
        }

        public string Filter_GetFilterItemColor(string filterBy)
        {
            var color = Wait.UntilElementVisible(FilterLabelColor(filterBy)).GetCssValue("background-color");
            return CSharpHelpers.ConvertRgbToHex(color);
        }

        public bool DoesFilterItemDisplay(string item)
        {
            return Driver.IsElementDisplayed(FilterCheckbox(item));
        }

        //Metrics Summary
        public bool MetricsSummary_IsTabPresent(string tabName)
        {
            return Driver.IsElementPresent(MetricsSummary_DynamicTabLocator(tabName));
        }

        public bool IsMetricsSummaryDisplayed()
        {
            return Driver.IsElementDisplayed(MetricsSummaryHeader);
        }

        //Kanban View
        public void SwitchToKanbanView()
        {
            Log.Step(GetType().Name, "Switch the Growth Plan to Kanban view");
            Wait.UntilJavaScriptReady(Timeout);
            if (Driver.IsElementDisplayed(GrowthPlanKanbanView)) return;
            Driver.JavaScriptScrollToElementCenter(ToggleButton);
            Wait.UntilElementClickable(ToggleButton).Click();
            Wait.UntilJavaScriptReady(Timeout);
        }

        public void SwitchToGridView()
        {
            Log.Step(GetType().Name, "Switch the Growth Plan to Grid view");
            Wait.UntilJavaScriptReady(Timeout);
            if (Driver.IsElementDisplayed(GrowthPlanGridView)) return;
            Wait.UntilElementClickable(ToggleButton).Click();
            Wait.UntilJavaScriptReady(Timeout);
        }

        public void AcceptDelete()
        {
            Log.Step(GetType().Name, "Click on the 'Yes' button on the Delete Confirmation popup");
            try
            {
                Driver.AcceptAlert();
            }
            catch (NoAlertPresentException)
            {
                Wait.UntilElementVisible(YesButton);
                Wait.UntilElementClickable(YesButton).Click();
                Wait.UntilElementInvisible(YesButton);
                Wait.UntilJavaScriptReady(Timeout);
            }
        }

        public bool DoesBenchMarkingPopupDisplay()
        {
            return Driver.IsElementDisplayed(BenchMarkingPopup);
        }

        public string GetSurveyNoteColor(string subDimensionName, string comment)
        {
            var noteColor = Wait.UntilElementVisible(DimensionSurveyNote(subDimensionName, comment)).GetCssValue("color");
            return CSharpHelpers.ConvertRgbToHex(noteColor).ToLower();
        }

        public string GetSurveyOpenEndQuestionNoteColor(string section, string comment)
        {
            var noteColor = Wait.UntilElementVisible(NotesOpenEndedQuestions(section, comment)).GetCssValue("color");
            return CSharpHelpers.ConvertRgbToHex(noteColor).ToLower();
        }

        public bool DoesBenchmarkingViewExist()
        {
            return Driver.IsElementPresent(RadarViewItem("Benchmarking"));
        }
        public bool IsBenchmarkingOptionDisplayed(string benchmarkingOption)
        {
            return Driver.IsElementDisplayed(RadarViewItem(benchmarkingOption));
        }
        //Benchmarking options popup
        public string GetBenchmarkingPopupTitleText(string benchmarkingOption)
        {
            Log.Step(nameof(AssessmentDetailsCommonPage), "Get 'Benchmarking Options' popup title");
            RadarSwitchView(ViewType.Benchmarking);
            return Wait.UntilElementVisible(BenchmarkingPopupTitleText).GetText();
        }
        public string GetBenchmarkingPopupInfoText()
        {
            Log.Step(nameof(AssessmentDetailsCommonPage), "Get 'Benchmarking Options' popup information text");
            var textList = Wait.UntilElementVisible(BenchmarkingPopupInfoText).GetText();
            return new List<string>(textList.Split(new[] { "\r\n" }, StringSplitOptions.None))[0];
        }
        public string GetBenchmarkingPopupBenchmarkingOptionsFieldText()
        {
            Log.Step(nameof(AssessmentDetailsCommonPage), "Get 'Benchmarking Options' field text from the benchmarking options popup");
            return Wait.UntilElementVisible(BenchmarkingPopupBenchmarkingOptionsFieldText).GetText().Replace(":", "");
        }
        public List<string> GetBenchmarkingPopupBenchmarkingOptionsDropdownList()
        {
            Log.Step(nameof(AssessmentDetailsCommonPage), "Get 'Benchmarking Options' popup benchmarking options all dropdown values");
            Wait.UntilElementClickable(BenchmarkingOptionsDropdown).Click();
            Wait.HardWait(1000); //wait till all dropdown values here
            return Driver.GetTextFromAllElements(BenchmarkingOptionsDropdownValuesList).ToList();
        }
        //Benchmarking options popup work type
        public void SelectBenchmarkingOptionsDropdownOption(string option)
        {
            Log.Step(nameof(AssessmentDetailsCommonPage), "Select benchmarking options popup benchmarking options dropdown option");
            SelectBenchmarkingOptions(option);
        }
        public bool IsBenchmarkingPopupWorkTypeFieldTextDisplayed()
        {
            return Driver.IsElementDisplayed(BenchmarkingPopupWorkTypeFieldText);
        }
        public string GetBenchmarkingPopupWorkTypeFieldText()
        {
            Log.Step(nameof(AssessmentDetailsCommonPage), "Get work type text of 'Benchmarking Options' popup");
            return Wait.UntilElementVisible(BenchmarkingPopupWorkTypeFieldText).GetText().Replace(":", "");
        }

        //Benchmarking options popup maturity 
        public bool IsBenchmarkingPopupMaturityFieldTextDisplayed()
        {
            return Driver.IsElementDisplayed(BenchmarkingPopupMaturityFieldText);
        }
        public string GetBenchmarkingPopupMaturityFieldText()
        {
            Log.Step(nameof(AssessmentDetailsCommonPage), "Get 'Benchmarking Options' popup maturity text");
            return Wait.UntilElementVisible(BenchmarkingPopupMaturityFieldText).GetText().Replace(":", "");
        }

        //Benchmarking options popup cancel and select buttons
        public List<string> GetBenchmarkingPopupCancelAndSelectButtonsTextList()
        {
            Log.Step(nameof(AssessmentDetailsCommonPage), "Get 'Benchmarking Options' popup 'Cancel' and 'Select' buttons");
            return Driver.GetTextFromAllElements(BenchmarkingPopupCancelAndSelectButtonsTextList).ToList();
        }
        public void ClickOnBenchmarkingPopupCancelButton()
        {
            Log.Step(nameof(AssessmentDetailsCommonPage), "Click on the 'Benchmarking Options' popup 'Cancel' button");
            Wait.UntilElementClickable(BenchmarkingPopupCancelButton).Click();
        }
        public string GetTeamCommentsTitleText()
        {
            Log.Step(nameof(AssessmentDetailsCommonPage), "Get 'Team Comments' title");
            Driver.JavaScriptScrollToElementCenter(TeamCommentsTitleText);
            return Wait.UntilElementVisible(TeamCommentsTitleText).GetText();
        }
        public string GetHideAllTeamCommentsButtonText()
        {
            Log.Step(nameof(AssessmentDetailsCommonPage), "Get 'Hide all Team Comments' button text");
            Driver.JavaScriptScrollToElementCenter(HideAllTeamCommentsButton);
            return Wait.UntilElementVisible(HideAllTeamCommentsButton).GetAttribute("value");
        }
        public string GetHideAllTeamCommentsPopupTitleText()
        {
            Log.Step(nameof(AssessmentDetailsCommonPage), "Click on the 'Hide all Team Comments' button and get the popup title");
            Wait.UntilElementClickable(HideAllTeamCommentsButton).Click();
            return Wait.UntilElementVisible(HideUnHideAllTeamStakeholderCommentsPopupTitleText).GetText();
        }
        public List<string> GetHideAllTeamCommentsPopupInfoTextList()
        {
            Log.Step(nameof(AssessmentDetailsCommonPage), "Get 'Hide all Team Comments' popup information text");
            return Driver.GetTextFromAllElements(HideUnHideAllTeamStakeholderCommentsPopupInfoTextList).ToList();
        }
        public string GetHideAllTeamCommentsPopupNoButtonText()
        {
            Log.Step(nameof(AssessmentDetailsCommonPage), "Get 'Hide all Team Comments' popup 'No, Cancel' button text");
            return Wait.UntilElementVisible(HideUnHideAllTeamStakeholderCommentsPopupNoCancelButton).GetText();
        }
        public string GetHideUnHideAllTeamCommentsPopupYesButtonText()
        {
            Log.Step(nameof(AssessmentDetailsCommonPage), "Get 'Hide all Team Comments' popup 'Yes proceed' button text");
            return Wait.UntilElementVisible(HideUnHideAllCommentsConfirmationPopupYesButton).GetText();
        }
        public void ClickOnHideAllCommentsPopupNoCancelButton()
        {
            Log.Step(nameof(AssessmentDetailsCommonPage), "Click on the hide all comments popup 'No, cancel' button");
            Wait.UntilElementClickable(HideUnHideAllTeamStakeholderCommentsPopupNoCancelButton).Click();
        }
        public void ClickOnHideAllCommentsPopupYesProceedButton()
        {
            Log.Step(nameof(AssessmentDetailsCommonPage), "Click on the hide all comments popup 'Yes, proceed' button");
            Driver.MoveToElement(HideAllTeamCommentsButton);
            Wait.UntilElementClickable(HideAllTeamCommentsButton).Click();
            Wait.UntilElementClickable(HideUnHideAllCommentsConfirmationPopupYesButton).Click();
        }
        public string GetUnHideAllTeamCommentsButtonText()
        {
            Log.Step(nameof(AssessmentDetailsCommonPage), "Get 'UnHide all Team Comments' button text");
            return Wait.UntilElementVisible(UnHideAllTeamCommentsButton).GetAttribute("value");
        }
        public string GetUnHideAllTeamCommentsPopupTitleText()
        {
            Log.Step(nameof(AssessmentDetailsCommonPage), "Click on the 'UnHide all Team Comments' button and get the popup title");
            Wait.UntilElementClickable(UnHideAllTeamCommentsButton).Click();
            return Wait.UntilElementVisible(HideUnHideAllTeamStakeholderCommentsPopupTitleText).GetText();
        }
        public List<string> GetUnHideAllTeamCommentsPopupInfoTextList()
        {
            Log.Step(nameof(AssessmentDetailsCommonPage), "Get information text from the 'UnHide all Team Comments' popup");
            return Driver.GetTextFromAllElements(HideUnHideAllTeamStakeholderCommentsPopupInfoTextList).ToList();
        }
        public void ClickOnUnHideAllCommentsPopupYesButton()
        {
            Log.Step(nameof(AssessmentDetailsCommonPage), "Click on the 'UnHide all Team Comments' popup 'Yes proceed' button");
            Wait.UntilElementClickable(HideUnHideAllCommentsConfirmationPopupYesButton).Click();
        }
        public string GetHideAllStakeholderCommentsButtonText()
        {
            Log.Step(nameof(AssessmentDetailsCommonPage), "Get 'Hide all Stakeholder Comments' button text");
            Driver.JavaScriptScrollToElementCenter(HideAllStakeholderCommentsButton);
            return Wait.UntilElementVisible(HideAllStakeholderCommentsButton).GetAttribute("value");
        }
        public string GetHideAllStakeholderCommentsPopupTitleText()
        {
            Log.Step(nameof(AssessmentDetailsCommonPage), "Click on the 'Hide all Stakeholder Comments' button and get the popup title");
            Wait.UntilElementClickable(HideAllStakeholderCommentsButton).Click();
            return Wait.UntilElementVisible(HideUnHideAllTeamStakeholderCommentsPopupTitleText).GetText();
        }
        public List<string> GetHideAllStakeholderCommentsPopupInfoTextList()
        {
            Log.Step(nameof(AssessmentDetailsCommonPage), "Get 'Hide all Stakeholder Comments' popup information text");
            return Driver.GetTextFromAllElements(HideUnHideAllTeamStakeholderCommentsPopupInfoTextList).ToList();
        }
        public string GetUnHideAllStakeholderCommentsButtonText()
        {
            Log.Step(nameof(AssessmentDetailsCommonPage), "Get 'UnHide all Stakeholder Comments' button text");
            return Wait.UntilElementVisible(UnHideAllStakeholderCommentsButton).GetAttribute("value");
        }
        public string GetHideAllStakeholderCommentsPopupNoButtonText()
        {
            Log.Step(nameof(AssessmentDetailsCommonPage), "Get 'Hide all Stakeholder Comments' popup 'No, cancel' button text");
            return Wait.UntilElementVisible(HideUnHideAllTeamStakeholderCommentsPopupNoCancelButton).GetText();
        }
        public string GetHideAllStakeholderCommentsPopupYesButtonText()
        {
            Log.Step(nameof(AssessmentDetailsCommonPage), "Get 'Hide all Stakeholder Comments' popup 'Yes proceed' button text");
            return Wait.UntilElementVisible(HideUnHideAllCommentsConfirmationPopupYesButton).GetText();
        }
        public string GetUnHideAllStakeholderCommentsPopupTitleText()
        {
            Log.Step(nameof(AssessmentDetailsCommonPage), "Get 'UnHide all Stakeholder Comments' popup title");
            Wait.UntilElementClickable(UnHideAllStakeholderCommentsButton).Click();
            return Wait.UntilElementVisible(HideUnHideAllTeamStakeholderCommentsPopupTitleText).GetText();
        }
        public List<string> GetUnHideAllStakeholderCommentsPopupInfoTextList()
        {
            Log.Step(nameof(AssessmentDetailsCommonPage), "Get 'UnHide all Stakeholder Comments' popup information text");
            return Driver.GetTextFromAllElements(HideUnHideAllTeamStakeholderCommentsPopupInfoTextList).ToList();
        }
        public bool IsCommentsEditButtonsTextTranslatedInPreferredLanguage(string editButtonText)
        {
            var getAllEditButton = Driver.GetTextFromAllElements(CommentsEditButtonTextList).ToList();
            return !(from item in getAllEditButton where item != editButtonText select new { }).Any();
        }
        public void ClickOnCommentsEditButton(string comment)
        {
            Log.Step(nameof(AssessmentDetailsCommonPage), "Click on the comments 'Edit' button");
            Wait.UntilJavaScriptReady();
            Driver.MoveToElement(CommentsEditButton(comment));
            Wait.HardWait(500);
            Wait.UntilElementClickable(CommentsEditButton(comment)).Click();
        }
        public string GetCommentsUpdateButtonsText()
        {
            Log.Step(nameof(AssessmentDetailsCommonPage), "Click on the 'Edit' button and get 'Update' button text");
            return Wait.UntilElementVisible(CommentsUpdateButtonText).GetText();
        }
        public string GetCommentsCancelButtonText()
        {
            Log.Step(nameof(AssessmentDetailsCommonPage), "Get 'Cancel' button text");
            return Wait.UntilElementClickable(CommentsCancelButtonText).GetText();
        }
        public void ClickOnCancelButton()
        {
            Log.Step(nameof(AssessmentDetailsCommonPage), "Click on the 'Cancel' button");
            Wait.UntilElementClickable(CommentsCancelButtonText).Click();
        }
        public bool IsUpdateButtonDisplayed()
        {
            return Driver.IsElementDisplayed(CommentsUpdateButtonText);
        }
        public bool IsCommentsHideButtonsTextTranslatedInPreferredLanguage(string hideButtonText)
        {
            Driver.JavaScriptScrollToElementCenter(CommentsHideButtonTextList);
            var getHideButtonText = Driver.GetTextFromAllElements(CommentsHideButtonTextList).ToList();
            return !(from item in getHideButtonText where item != hideButtonText select new { }).Any();
        }
        public string GetCommentsUnHideButtonText(string subDimension, string comment)
        {
            Log.Step(nameof(AssessmentDetailsCommonPage), "Get 'UnHide' button text");
            Wait.UntilElementClickable(CommentsHideUnHideButtons(comment)).Click();
            Wait.UntilJavaScriptReady();
            return Wait.UntilElementVisible(CommentsHideUnHideButtons(comment)).GetText();
        }
        public void ClickOnCommentsUnHideButton(string comments)
        {
            Log.Step(nameof(AssessmentDetailsCommonPage), "Click on the 'Hide' button");
            Wait.UntilElementClickable(CommentsHideUnHideButtons(comments)).Click();
        }
        public bool IsHideButtonDisplayed(string comments)
        {
            return Driver.IsElementDisplayed(CommentsHideUnHideButtons(comments));
        }
        public string GetGrowthOpportunitiesTitleText()
        {
            Log.Step(nameof(AssessmentDetailsCommonPage), "Get 'Growth Opportunities' title text ");
            return Wait.UntilElementVisible(GrowthOpportunitiesTitleText).GetText();
        }
        public string GetImpedimentsTitleText()
        {
            Log.Step(nameof(AssessmentDetailsCommonPage), "Get 'Impediments' title text");
            return Wait.UntilElementVisible(ImpedimentsTitleText).GetText();
        }
        public string GetStrengthsTitleText()
        {
            Log.Step(nameof(AssessmentDetailsCommonPage), "Get 'Strengths' title text");
            return Wait.UntilElementVisible(StrengthsTitleText).GetText();
        }
        public string GetDescriptionText(string sectionName)
        {
            Log.Step(nameof(AssessmentDetailsCommonPage), "Get description text");
            return Wait.UntilElementVisible(DescriptionText(sectionName)).GetText();
        }
        public string GetGrowthPlanTitleText()
        {
            Log.Step(nameof(AssessmentDetailsCommonPage), "Get 'Growth plan' title text");
            var title = Wait.UntilElementVisible(GrowthPlanTitleText).GetText();
            return new List<string>(title.Split(new[] { "\r\n" }, StringSplitOptions.None))[0];
        }
        public List<string> GetGrowthPlanInfoTextList()
        {
            Log.Step(nameof(AssessmentDetailsCommonPage), "Get 'Growth plan' information text");
            Driver.JavaScriptScrollToElementCenter(GrowthPlanInfoTextList);
            return Driver.GetTextFromAllElements(GrowthPlanInfoTextList).ToList();
        }
        public bool IsGrowthPlanItemDisplayed()
        {
            return Driver.IsElementDisplayed(EditButton);
        }
        public void ClickOnGrowthItemEditButton()
        {
            Wait.UntilElementClickable(EditButton).Click();
        }
    }

    public struct DimensionNote
    {
        public string Dimension { get; set; }
        public string SubDimension { get; set; }
        public string Note { get; set; }

        public string SurveySection => $"{Dimension} - {SubDimension}";
    }
}