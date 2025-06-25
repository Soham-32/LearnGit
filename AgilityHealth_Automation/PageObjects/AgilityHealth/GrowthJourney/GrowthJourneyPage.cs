using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;
using System.Collections.Generic;
using System.Linq;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.GrowthJourney
{
    public class GrowthJourneyPage : BasePage
    {
        public GrowthJourneyPage(IWebDriver driver, ILogger log) : base(driver, log) { }

        // Team Radar
        private static By RadarViewDropdownArrow => By.CssSelector("span[aria-owns='compareSelect_listbox']");
        private readonly By GrowthJourneyDescription = By.XPath("//div[@class='status']/div/div");
        private static By RadarViewType(string view) => By.XPath($"//ul[@id ='compareSelect_listbox']/li[text() = '{view}'] | //ul[@id ='compareSelect_listbox']//font[text() = '{view}']");
        private readonly By ExportPdfButton = By.Id("pdfImage");
        private readonly By CreatePdfButton = By.XPath("//span[@id='pdf_submit' or @id='createPdf']");
        private readonly By GeneratePdfIndicator = By.Id("generatePdf");
        private readonly By RadarTypeDropdown = By.XPath("//div[@id='surveyDiv']//span[@class='k-icon k-i-arrow-s']");
        private static By RadarType(string radarName) => By.XPath($"//ul[@id = 'surveyDropdownList_listbox']/li[contains(text(), '{radarName}')]");
        private static By RadarTypeList => By.XPath("//ul[@id = 'surveyDropdownList_listbox']/li");
        private static By RadarTypeDropdownNormalTeam => By.XPath("//ul[@id='surveyDropdownList_listbox']/li[@data-offset-index='1']");

        private readonly By FilterDimensionsDropdown = By.XPath("//span[@aria-owns='DimensionDropDown_listbox']//span//span[@class='k-input']");
        private readonly By FilterDimensionsDropdownList = By.XPath("//ul[@id='DimensionDropDown_listbox']//li");
        private readonly By DimensionDropdown = By.XPath("//ul[@id='DimensionDropDown_listbox']/li[@data-offset-index='1']");
        private readonly By FilterSubDimensionsDropdown = By.XPath("//span[@aria-owns='SubdimensionDropDown_listbox']//span//span[@class='k-input']");
        private readonly By SubDimensionDropdown = By.XPath("//ul[@id='SubdimensionDropDown_listbox']/li[@data-offset-index='1']");
        private readonly By FilterSubDimensionsDropdownList = By.XPath("//ul[@id='SubdimensionDropDown_listbox']//li");
        private static By FilterDropdownValue(string value) => By.XPath($"//li[text()='{value}'] | //li//font[text()='{value}']");
        private readonly By TimelineChartTitleList = By.XPath("//*[local-name()='svg']//following-sibling::span");
        // Left Nav Filter
        private readonly By FilterSymbol = By.ClassName("open-filters");
        private readonly By AssessmentTabLeftNav = By.XPath("//a[@href='#FilterAway-1']");
        private static By CompareDropdownArrow => By.CssSelector("span[aria-owns='Period_listbox'] span.k-input");
        private static By CompareDropdownPeriodList => By.Id("Period_listbox");
        private static By CompareDropdownDetailText => By.XPath("//h2[@id='compareHeading']/parent::div/p");
        private static By CompareType(string type) => By.XPath($"//ul[@id ='Period_listbox']/li[text() = '{type}']");

        private readonly By CampaignName = By.XPath("//input[@class='filter Campaign']/../following-sibling::span");
        private static By FilterItem(string filterBy) => By.CssSelector($"input[title='{filterBy}']");
        private static By FilterTabsList => By.XPath("//li[@role='tab']");
        private static By AssessmentList => By.XPath("//div[@id = 'FilterAway-1']//span[@tooltipfor]");
        private static By AssessmentCheckbox(string assessment) => By.CssSelector($"input[title='{assessment}']");

        // Individual growth journey filters
        private readonly By FilterIndividualTab = By.CssSelector("div#FilterAway li[aria-controls='FilterAway-2']");
        private static By FilterAssessmentColor(string assessment) =>
            By.XPath($"//span[normalize-space(text()) = '{assessment}']//span[@class = 'color-marker'] | //font[normalize-space(text()) = '{assessment}']//ancestor::span//following-sibling::span[@class = 'color-marker']");

        private readonly By FilterSelectAllLink = By.CssSelector("div#FilterAway-1 span#selectAllDots");
        private readonly By FilterClearAllLink = By.CssSelector("div#FilterAway-1 span#clearAllDots");

        private static By Filter_IndividualAssessmentCheckbox(string assessment) =>
            By.CssSelector($"input[tag_name='{assessment.Substring(0, 25)}...'].individualFilter");
        private readonly By FilterIndividualClearAllLink = By.CssSelector("div#FilterAway-2 span.clearAllDots");

        // Compare Radar Analysis
        private readonly By ExportExcelButton = By.XPath("//a[text()='Export to Excel']");
        private readonly By ShowHideMetricsText = By.Id("showHideMetricsDesc");
        private readonly By ShowHideMetricsButton = By.XPath("//div[@aria-describedby='showHideMetricsDesc']");
        private readonly By ShowMetricsButton = By.XPath("//a[@class='handle ico ease']");
        private readonly By HideMetricsButton = By.XPath("//a[@class='handle ico ease on']");
        private readonly By CompareRadarDimension = By.XPath("//p[@class='k-reset'][contains(text(),'Dimension')]");
        private readonly By CompareRadarSubDimension = By.XPath("//p[@class='k-reset'][contains(text(),'Subdimension')]");
        private readonly By DimensionGrid = By.XPath("(//p[contains(text(),'Dimension')]/a[contains(@class,'k-icon k-i-collapse') or (@class='k-icon k-i-expand')])[1]");
        private readonly By SubDimensionGrid = By.XPath("(//p[contains(text(),'Subdimension')]/a[@class='k-icon k-i-collapse' or @class='k-icon k-i-expand'])[1]");
        private static By AllRowValues() => By.XPath("//table[@role='grid']//tbody//tr");

        private static By ColumnList(string name) => By.XPath($"//div[@id='analysisgrid']/div[@class='k-grid-header']/div/table//th[@data-title ='{name}']");
        private static By AllColumnHeaderList => By.CssSelector(".k-grid-header-wrap table thead tr th");
        private readonly By AnalysisGridDimensions = By.XPath("//p[contains(., 'Dimension:')]");
        private readonly By AnalysisGridSubDimension = By.XPath("//p[contains(text(),'Subdimension:')] | //p//font[contains(text(),'Subdimension:')]");
        private readonly By AnalysisGridCompetency = By.XPath("//span[contains(@class, 'competencyNameClass')]");
        private static By AnalysisGridSurveyValue(string competency) => By.XPath($"//span[contains(text(),'{competency}')]//ancestor::tr//td[not(@style)][last()] | //span//font[text()='{competency}']//ancestor::tr//td[not(@style)][last()]");
        private static By AnalysisGridSurveyValueForAssessments(string competency) => By.XPath($"//span[contains(text(), '{competency}')]//ancestor::td//following-sibling::td[not(@style)]//span[@class='lg-font ']  | //span//font[text()='{competency}']//ancestor::td//following-sibling::td[not(@style)]//span[@class='lg-font ']");

        // Compare Radar
        private static By RadarDots(string color, string competency) =>
            By.CssSelector($"[fill='{color}'][competency='{competency}']");

        private static By RadarLines(string color) => By.CssSelector($"line[stroke = '{color}']");
        private static By RadarDotsValue(string color, string competency) =>
            By.CssSelector($"[fill='{color}'][competency='{competency}'][val]");
        //Growth Journey
        private readonly By GrowthJourneyTab = By.XPath("//a[@id='GrowthJourney' and @href='#']");
        //Methods

        //RadarDropdown
        public void SelectRadarFromRadarTypeDropDown(string radarName)
        {
            Log.Step(nameof(GrowthJourneyPage), $"'Select {radarName}' from 'Radar Type' Dropdown");
            SelectItem(RadarTypeDropdown, RadarType(radarName));
        }

        public List<string> GetAllRadarTypesFromDropdown()
        {
            Log.Step(nameof(GrowthJourneyPage), "Get all the radars from 'radar' dropdown");
            Wait.UntilElementClickable(RadarTypeDropdown).Click();
            Wait.UntilJavaScriptReady();
            return Driver.GetTextFromAllElements(RadarTypeList).ToList();
        }

        public bool IsRadarTypeDropdownDisplayed()
        {
            return Driver.IsElementDisplayed(RadarTypeDropdown);
        }

        public void ClickOnTheFirstRadarTypeFromDropdown()
        {
            Log.Step(nameof(GrowthJourneyPage), "Click on the first radar type from dropdown");
            Wait.UntilElementClickable(RadarTypeDropdown).Click();
            Wait.UntilJavaScriptReady();
            Wait.UntilElementClickable(RadarTypeDropdownNormalTeam).Click();
            Wait.UntilJavaScriptReady();
        }
        // Team Radar
        private void RadarSwitchView(string view) => SelectItem(RadarViewDropdownArrow, RadarViewType(view));

        public string GetTeamRadarDescription()
        {
            Log.Step(nameof(GrowthJourneyPage), "Get team radar description");
            return Wait.UntilElementVisible(GrowthJourneyDescription).GetText();
        }
        public void SwitchToCompareRadarView()
        {
            Log.Step(nameof(GrowthJourneyPage), "Switch to compare radar view");
            RadarSwitchView("Compare Radar");
        }

        public void SwitchToTimelineView()
        {
            Log.Step(nameof(GrowthJourneyPage), "Switch to timeline view");
            RadarSwitchView("Timeline");
        }
        public void ClickExportToPdf()
        {
            Log.Step(nameof(GrowthJourneyPage), "Click on Export to PDF");
            Wait.UntilElementVisible(ExportPdfButton);
            Wait.UntilElementClickable(ExportPdfButton).Click();
        }

        public void ClickCreatePdf()
        {
            Log.Step(nameof(GrowthJourneyPage), "Click on Create PDF");
            Wait.UntilElementVisible(CreatePdfButton);
            Wait.UntilElementClickable(CreatePdfButton).Click();
            Wait.UntilElementVisible(GeneratePdfIndicator);
            Wait.UntilElementInvisible(GeneratePdfIndicator);
            Wait.UntilJavaScriptReady();
        }
        public List<string> GetAllTitlesFromTimeLineChart()
        {
            Log.Step(nameof(GrowthJourneyPage), "Get 'Titles' from the 'Timeline chart'.");
            return Driver.GetTextFromAllElements(TimelineChartTitleList).ToList();
        }
        public List<string> GetDimensionsListFromFilterDropdown()
        {
            Log.Step(nameof(GrowthJourneyPage), "Get all the 'Dimensions' from the Filter dropdown");
            Wait.UntilElementVisible(FilterDimensionsDropdownList);
            return Driver.GetTextFromAllElements(FilterDimensionsDropdownList).ToList();
        }
        public List<string> GetSubDimensionsFromFilterDropdown()
        {
            Log.Step(nameof(GrowthJourneyPage), "Get all the Sub-dimension from Filter dropdown");
            Wait.UntilElementVisible(FilterSubDimensionsDropdownList);
            return Driver.GetTextFromAllElements(FilterSubDimensionsDropdownList).ToList();
        }
        public void SelectDimensionFromFilterDropdown(string dimension)
        {
            Log.Step(nameof(GrowthJourneyPage), "Select Dimension {dimension} from the dimension dropdown");
            SelectItem(FilterDimensionsDropdown, FilterDropdownValue(dimension));
        }
        public void SelectSubDimensionFromFilterDropdown(string subDimension)
        {
            Log.Step(nameof(GrowthJourneyPage), "Select Sub-dimension {subDimension} from the sub-dimension dropdown");
            SelectItem(FilterSubDimensionsDropdown, FilterDropdownValue(subDimension));
        }
        public void ClickOnDimensionFilterDropdown()
        {
            Log.Step(nameof(GrowthJourneyPage), "Click on the 'Dimension' dropdown");
            Wait.UntilElementClickable(FilterDimensionsDropdown).Click();
        }
        public void ClickOnFirstDimension()
        {
            Log.Step(nameof(GrowthJourneyPage), "Click on the first dimension from the dropdown.");
            Wait.UntilElementClickable(DimensionDropdown).Click();
            Wait.UntilJavaScriptReady();
        }
        public string GetFirstDimension()
        {
            Log.Step(nameof(GrowthJourneyPage), "Get the first dimension name.");
            Wait.UntilElementClickable(FilterDimensionsDropdown).Click();
            Wait.UntilJavaScriptReady();
            return Wait.UntilElementClickable(DimensionDropdown).GetText();
        }
        public void ClickOnSubDimensionFilterDropdown()
        {
            Log.Step(nameof(GrowthJourneyPage), "Click on the 'Sub-Dimension' dropdown");
            Wait.UntilElementClickable(FilterSubDimensionsDropdown).Click();
            Wait.UntilJavaScriptReady();
        }
        public void ClickOnFirstSubDimension()
        {
            Log.Step(nameof(GrowthJourneyPage), "Click on the first subdimension dropdown.");
            Wait.UntilElementClickable(SubDimensionDropdown).Click();
            Wait.UntilJavaScriptReady();
        }

        public string GetFirstSubDimension()
        {
            Log.Step(nameof(GrowthJourneyPage), "Get the first subdimension name.");
            Wait.UntilElementClickable(FilterSubDimensionsDropdown).Click();
            Wait.UntilJavaScriptReady();
            return Wait.UntilElementClickable(SubDimensionDropdown).GetText();
        }

        // Left Nav Filter
        public void OpenFilterSidebar()
        {
            Log.Step(nameof(GrowthJourneyPage), "Open filter side bar");
            Wait.UntilElementVisible(FilterSymbol);
            Wait.UntilElementClickable(FilterSymbol).Click();
            Wait.UntilJavaScriptReady();
        }
        public bool IsAssessmentTabVisible()
        {
            return Driver.IsElementDisplayed(AssessmentTabLeftNav);
        }
        public List<string> GetAllCompareDropdownValues()
        {
            Log.Step(nameof(GrowthJourneyPage), "Get all time period dropdown values.");
            var texts = new List<string>();
            var items = Wait.UntilElementVisible(CompareDropdownPeriodList).FindElements(By.TagName("li"));
            foreach (var item in items)
            {
                texts.Add(item.Text);
            }

            return texts;

        }
        public List<string> GetTabsListFromFilter()
        {
            Log.Step(nameof(GrowthJourneyPage), "Get all the 'Tab' list");
            return Driver.GetTextFromAllElements(FilterTabsList).ToList();
        }

        public void SelectCompareTypeFromFilter(string type)
        {
            Log.Step(nameof(GrowthJourneyPage), $"Select compare type <{type}>");
            SelectItem(CompareDropdownArrow, CompareType(type));
            Wait.UntilJavaScriptReady();
        }

        public string GetCompareDropdownDetailText()
        {
            Log.Step(nameof(GrowthJourneyPage), "Get the compare dropdown detail text.");
            return Wait.UntilElementVisible(CompareDropdownDetailText).Text;
        }
        public string GetSelectedCompareTypeFromFilter()
        {
            return Wait.UntilElementClickable(CompareDropdownArrow).GetText();
        }

        public void ClickCompareType()
        {
            Log.Step(nameof(GrowthJourneyPage), "Click on compare type dropdown arrow.");
            Wait.UntilElementClickable(CompareDropdownArrow).Click();
        }

        public bool DoesCompareTypeDisplay(string type)
        {
            Wait.UntilElementClickable(CompareDropdownArrow).Click();
            return Driver.IsElementDisplayed(CompareType(type));
        }

        public void SelectFilterItemCheckboxByNameFromFilter(string filterBy, bool check = true)
        {
            Log.Step(nameof(GrowthJourneyPage), $"Select filter item checkbox by name <{filterBy}>");
            Wait.UntilElementClickable(FilterItem(filterBy)).Check(check);
            Wait.HardWait(2000); //Wait till filter open.
        }
        public List<string> GetAllAssessmentList()
        {
            Log.Step(nameof(GrowthJourneyPage), "Get all the 'Assessments' from the 'Compare Assessment' list");
            return Driver.GetTextFromAllElements(AssessmentList).ToList();
        }

        //CheckBoxes
        public void UnCheckAssessmentTypeCheckbox(string assessmentName)
        {
            Log.Step(nameof(GrowthJourneyPage), $"Uncheck '{assessmentName}' checkbox");
            Wait.UntilElementClickable(AssessmentCheckbox(assessmentName)).Check(false);
        }
        public bool IsAssessmentCheckboxChecked(string assessmentName)
        {
            return Wait.UntilElementExists(AssessmentCheckbox(assessmentName)).Selected;
        }

        public bool IsFilterItemCheckboxSelectedFromFilter(string filterBy) =>
            Wait.UntilElementVisible(FilterItem(filterBy)).Selected;

        public string GetFilterItemColorFromFilter(string filterBy) =>
            Wait.UntilElementVisible(FilterItem(filterBy)).GetElementAttribute("color");

        // Individual growth journey filters
        internal void ClickOnIndividualTab()
        {
            Log.Step(nameof(GrowthJourneyPage), "Click on individual tab");
            Wait.UntilElementClickable(FilterIndividualTab).Click();
        }

        public string GetFilterAssessmentColor(string assessment)
        {
            Log.Step(nameof(GrowthJourneyPage), $"Get assessment color for {assessment}");
            Wait.UntilJavaScriptReady();
            return Wait.UntilElementClickable(FilterAssessmentColor(assessment)).GetAttribute("style").Split(':')[1].Trim().Trim(';');
        }

        public string GetIndividualFilterColor(string assessment)
        {
            Wait.UntilJavaScriptReady();
            return Wait.UntilElementClickable(Filter_IndividualAssessmentCheckbox(assessment)).GetAttribute("tag_color");
        }

        public void SelectIndividualFilter(string assessment)
        {
            Log.Step(nameof(GrowthJourneyPage), $"Select individual filter for assessment <{assessment}>");
            Wait.UntilJavaScriptReady();
            Wait.UntilElementClickable(Filter_IndividualAssessmentCheckbox(assessment)).Check();
            Wait.UntilJavaScriptReady();
        }

        public void ClickOnSelectAllLink()
        {
            Log.Step(nameof(GrowthJourneyPage), "Click on 'Select All' link");
            Wait.UntilElementClickable(FilterSelectAllLink).Click();
            Wait.UntilJavaScriptReady();
        }

        public void ClickOnClearAllLink()
        {
            Log.Step(nameof(GrowthJourneyPage), "Click on 'Clear All' link");
            Wait.UntilElementClickable(FilterClearAllLink).Click();
            Wait.UntilJavaScriptReady();
        }

        public void ClickIndividualClearAllLink()
        {
            Log.Step(nameof(GrowthJourneyPage), "Click on individual clear all link");
            Wait.UntilElementClickable(FilterIndividualClearAllLink).Click();
            Wait.UntilJavaScriptReady();
        }

        public void SelectGroupFilter(string assessment)
        {
            Log.Step(nameof(GrowthJourneyPage), $"Select group filter for assessment <{assessment}>");
            Wait.UntilJavaScriptReady();
            Wait.UntilElementClickable(FilterAssessmentColor(assessment)).Check();
            Wait.UntilJavaScriptReady();
        }
        public string GetCampaignName()
        {
            Log.Step(nameof(GrowthJourneyPage), "Get Campaign name");
            return Wait.UntilElementClickable(CampaignName).GetText();
        }

        // Compare Radar Analysis

        public string GetShowHideMetricsText()
        {
            Log.Step(nameof(GrowthJourneyPage), "Get show hide metrics text.");
            return Wait.UntilElementVisible(ShowHideMetricsText).GetText();
        }

        public void ClickOnShowHideMetricsButton()
        {
            Log.Step(nameof(GrowthJourneyPage), "Click on show hide metrics button.");
            Wait.UntilElementClickable(ShowHideMetricsButton).Click();
        }

        public bool IsShowMetricsButtonOn()
        {
            return Driver.IsElementDisplayed(ShowMetricsButton);
        }

        public bool IsHideMetricsButtonOn()
        {
            return Driver.IsElementDisplayed(HideMetricsButton);
        }

        public void ClickOnDimensionDropdown()
        {
            Log.Step(nameof(GrowthJourneyPage), "Click on Dimension dropdown.");
            Wait.UntilElementClickable(DimensionGrid).Click();
        }

        public void ClickOnSubDimensionDropdown()
        {
            Log.Step(nameof(GrowthJourneyPage), "Click on Sub dimension dropdown.");
            Wait.UntilElementClickable(SubDimensionGrid).Click();
        }

        public string GetDimensionClass()
        {
            Log.Step(nameof(GrowthJourneyPage), "Get dimension class");
            return Wait.UntilElementVisible(DimensionGrid).GetAttribute("class");
        }

        public string GetSubDimensionClass()
        {
            Log.Step(nameof(GrowthJourneyPage), "Get subdimension class");
            return Wait.UntilElementVisible(SubDimensionGrid).GetAttribute("class");
        }
        public void ClickExportToExcel()
        {
            Log.Step(nameof(GrowthJourneyPage), "Click on 'Export to Excel' Button");
            Wait.UntilElementVisible(ExportExcelButton);
            Wait.UntilElementClickable(ExportExcelButton).Click();
        }

        public string GetDimensionFromCompareRadarAnalysis()
        {
            Log.Step(nameof(GrowthJourneyPage), "Get dimension from compare radar analysis");
            return Wait.UntilElementVisible(CompareRadarDimension).Text;
        }
        public string GetSubDimensionFromCompareRadarAnalysis()
        {
            Log.Step(nameof(GrowthJourneyPage), "Get subimension from compare radar analysis");
            return Wait.UntilElementVisible(CompareRadarSubDimension).Text;
        }
        public IList<string> GetRowValues()
        {
            return Driver.GetTextFromAllElements(AllRowValues());
        }
        public bool IsItemPresentInColumnList(string itemName)
        {
            return Driver.IsElementDisplayed(ColumnList(itemName));
        }

        public List<string> GetHeaderList()
        {
            Log.Step(nameof(GrowthJourneyPage), "Get all the 'Header' list from column radar analysis");
            return Driver.GetTextFromAllElements(AllColumnHeaderList).ToList();
        }
        public List<string> GetDimensionListFromAnalysisGrid()
        {
            Log.Step(nameof(GrowthJourneyPage), "Get all the 'Dimensions' from the analysis grid");
            return (Driver.GetTextFromAllElements(AnalysisGridDimensions)).Select(x => x.Replace("Dimension: ", "")).ToList();
        }
        public List<string> GetSubDimensionListFromAnalysisGrid()
        {
            Log.Step(nameof(GrowthJourneyPage), "Get all the 'Sub-Dimensions' from the analysis grid");
            return (Driver.GetTextFromAllElements(AnalysisGridSubDimension)).Select(x => x.Replace("Subdimension: ", "")).ToList();
        }
        public List<string> GetCompetencyListFromAnalysisGrid()
        {
            Log.Step(nameof(GrowthJourneyPage), "Get all the 'Competency' from the grid");
            return Driver.GetTextFromAllElements(AnalysisGridCompetency).ToList();
        }
        public int GetSurveyValueFromAnalysisGrid(string competency)
        {
            Log.Step(nameof(GrowthJourneyPage), "Get 'Survey value' for the competency from the analysis grid");
            return int.Parse(Wait.UntilElementVisible(AnalysisGridSurveyValue(competency)).GetText());
        }

        public List<string> GetSurveyValueFromAnalysisGridForAssessment(string competency)

        {
            Log.Step(nameof(GrowthJourneyPage), "Get 'Survey value' for the competency from the analysis grid");
            return Wait.UntilAllElementsLocated(AnalysisGridSurveyValueForAssessments(competency)).Select((val => val.Text)).ToList();
        }

        // Compare Radar
        public int GetDotCountByColor(string color, string comp)
        {
            Log.Step(nameof(GrowthJourneyPage), "Get dot count by color & competency");
            return Wait.UntilAllElementsLocated(RadarDots(color, comp)).Where(dot => dot.Displayed).Select(dot => dot.GetAttribute("val")).ToList().Count;
        }

        public List<string> GetRadarDotValue(string color, string comp)
        {
            Log.Step(nameof(GrowthJourneyPage), "Get radar dot value by color & competency");
            return Wait.UntilAllElementsLocated(RadarDotsValue(color, comp)).Select(e => e.GetAttribute("val")).ToList();
        }

        public List<string> Radar_GetDotValue(string color, string competency)
        {
            var dots = Wait.UntilAllElementsLocated(RadarDots(color, competency));
            return dots.Where(dot => dot.Displayed).Select(dot => dot.GetAttribute("val")).ToList();
        }

        public int GetLineCountByColor(string color)
        {
            Log.Step(nameof(GrowthJourneyPage), "Get line count by color");
            return Wait.UntilAllElementsLocated(RadarLines(color)).Where(dot => dot.Displayed).Select(dot => dot.GetAttribute("val")).ToList().Count;
        }

        public void NavigateToGrowthJourneyPage(int teamId)
        {
            NavigateToUrl($"{BaseTest.ApplicationUrl}/team/{teamId}/growthjourney");
        }
        public bool IsGrowthJourneyTabPresent()
        {
            return Driver.IsElementPresent(GrowthJourneyTab);
        }

        //Growth Journey Navigation
        public void NavigateToTeamGrowthJourneyPageForProd(string env, int teamId)
        {
            NavigateToUrl($"https://{env}.agilityinsights.ai/team/{teamId}/growthjourney");
        }
        public void NavigateToTeamGrowthJourneyPageForProd(string env, int teamId, int assessmentId, string teamHierarchy = "multiteam")
        {
            NavigateToUrl($"https://{env}.agilityinsights.ai/{teamHierarchy}/{teamId}/growthjourney/{assessmentId}");
        }

    }
}