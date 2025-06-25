using System;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;
using System.Linq;
using System.Collections.Generic;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AtCommon.Utilities;
using System.ComponentModel;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Radar
{
    internal class RadarPage : BasePage
    {
        public RadarPage(IWebDriver driver, ILogger log) : base(driver, log) { }

        private TimeSpan Timeout => Driver.Manage().Timeouts().AsynchronousJavaScript;
        //Title
        private readonly By RadarTitle = By.XPath("//div[@class='pg-title']//div/h1");
        private readonly By RadarIconOnHeader = By.XPath("//a[@title='Radar']");

        //Filter
        private readonly By FilterSymbol = By.ClassName("open-filters");
        private readonly By FilterSidebarDescriptionTextList = By.XPath("//div[@id='filterbar']//div[@class='title']");
        private readonly By FilterTabsList = By.XPath("//li[@role='tab']//a");
        private readonly By FilterParticipantGroupsTab = By.XPath("//div[@id='FilterAway']//a[text()='Participant Groups']");
        private readonly By ParticipantGroupsTagsList = By.XPath("//div[@id='participantGroups']//li");
        private static By TagColor(string tagName) => By.XPath($"//li//label[text()='{tagName}']/span/span");
        private static By TagCheckBox(string tagName) => By.CssSelector($"input[tag_name='{tagName}']");

        //PDF
        private readonly By ExportPdfButton = By.Id("pdfImage");
        private readonly By CreatePdfButton = By.XPath("//span[@id='pdf_submit' or @id='createPdf']");
        private readonly By GeneratePdfIndicator = By.Id("generatePdf");

        //Excel
        private readonly By ExportExcelButton = By.XPath("//span[@id='showPdfIcon']//img[@title='Generate Report']");
        private readonly By ExportQuestionsButton = By.CssSelector("img[title = 'Generate Question Key']");

        //Tabs
        private readonly By GrowthJourneyTab = By.LinkText("Growth Journey");

        //Quick Links
        private static By JumpLink(string jumpLinkTitle) => By.XPath($"//a[@title='{jumpLinkTitle}']");
        private readonly By GrowthPlanQuickLink = By.CssSelector("body > div .pg-title a[href='#growth_plan']");
        private readonly By IdeaboardQuickLink = By.CssSelector(".pg-title .pg-title-quick-links a[title='Idea Board']");
        private readonly By LanguageDropDown = By.Id("lang-menu-button");
        private static By PreferredLanguage(string language) => By.XPath($"//div[@id='language-selection-menu']//li[@id='{language}']/div");
        private readonly By AllLanguagesList = By.XPath("//div[@id='language-selection-menu']//li[@id]");
        private readonly By HeaderJumpLinksNamesTextList = By.XPath("//div[@id='slider_inner']//a[@title]");

        //Radar View Dropdown
        private readonly By RadarViewDropdownDetailOption = By.XPath("//div[@id='ddl']//span[@class='k-input']");
        private readonly By RadarViewDropDownValuesTextList = By.XPath("//ul[@id='radarSelect_listbox']//li[@class]");

        //Action icons  
        private readonly By ActionIconsTitleTextList = By.XPath("//span[@id='showPdfIcon']//*[@title]");

        //Radar
        private readonly By Radar = By.Id("radar2_div");
        private static By DynamicCompetencyLink(string competencyId) => By.CssSelector($"[competencyid='{competencyId}']");
        private readonly By RadarDimensionList = By.XPath("(//*[local-name()='svg'])[2]//*[local-name()='text' and @class='dim_font']//*");
        private readonly By RadarSubDimensionList = By.XPath("(//*[local-name()='svg'])[2]//*[local-name()='text' and @class='fa_font']//*");
        private static By RadarCompetencyNameList(int competencyId) => By.XPath($"(//*[local-name()='svg'])[2]//*[local-name()='text' and @textcompetencyid='{competencyId}' and @class='competencyfont']//*");

        //Competency Popup
        private readonly By CompetencyValue = By.ClassName("txtCompetencyValue");
        private readonly By SaveCompetencyButton = By.Id("competencybtn");
        private readonly By GrowthPlanAddGiButton = By.ClassName("growth-plan-add-item-btn");

        private readonly By SurveyTypeList = By.CssSelector("span[aria-owns='surveyDropdownList_listbox']");
        private static By SurveyTypeListItem(string item) => By.XPath($"//ul[@id = 'surveyDropdownList_listbox']/li[text() = '{item}'] | //ul[@id = 'surveyDropdownList_listbox']/li//*[text() = '{item}']");
        private readonly By TeamCommentsExcelButton = By.Id("excelDownload");
        private readonly By NoTeamCommentsCloseButton = By.Id("closenodata");

        //Growth journey
        private readonly By AnalysisExportToExcelButton = By.CssSelector("#analysisgrid a.k-grid-excel");

        private readonly By ExportAssessmentChecklistButton = By.LinkText("Export Assessment Checklist");

        //Analytics Cards and tooltips
        private readonly By AnalyticsHeaderTitleText = By.Id("titleAnalyticsX");
        private readonly By TopFiveCompetenciesTitleText = By.XPath("//div[@id='topFive']//div[contains(@class,'analyticsHeader')]");
        private readonly By LowestFiveCompetenciesTitleText = By.XPath("//div[@id='lowest5']//div[contains(@class,'analyticsHeader')]");
        private readonly By FiveHighestConsensusCompetenciesTitleText = By.XPath("//div[@id='convergence']//div[contains(@class,'analyticsHeader')]");
        private readonly By FiveHighestConsensusCompetenciesTooltipButton = By.XPath("//div[@id='convergence']//img");
        private readonly By FiveHighestConsensusCompetenciesTooltipDescriptionText = By.XPath("//div[@class='highest5ConsensusTooltip']/parent::span");
        private readonly By FiveHighestConsensusCompetenciesTooltipSupportText = By.XPath("//div[@class='highest5ConsensusTooltip']/parent::span/a");
        private readonly By FiveLowestConsensusCompetenciesTitleText = By.XPath("//div[@id='divergent']//div[contains(@class,'analyticsHeader')]");
        private readonly By FiveLowestConsensusCompetenciesTooltipButton = By.XPath("//div[@id='divergent']//img");
        private readonly By FiveLowestConsensusCompetenciesTooltipDescriptionText = By.XPath("//div[contains(@class,'lowest5ConsensusTooltip')]//parent::span");

        //AnalyticsAllCompetencies
        private readonly By TopFiveCompetencies = By.XPath("//div[@id='topFive']//div[@class='data_row rowPadding']");
        private readonly By LowestFiveCompetencies = By.XPath("//div[@id='lowest5']//div[@class='data_row rowPadding']");
        private readonly By LowestFiveConsensusCompetencies = By.XPath("//div[@id='divergent']//div[@class='data_row rowPadding']");
        private readonly By HighestFiveConsensusCompetencies = By.XPath("//div[@id='convergence']//div[@class='data_row rowPadding']");

        //Standard Deviation Model
        private readonly By HighestFiveConsensusCompetenciesStandardDeviation = By.XPath("//div[@id='convergence']//div[contains(@class,'stdDeviationHeader')]");
        private readonly By LowestFiveConsensusCompetenciesStandardDeviation = By.XPath("//div[@id='divergent']//div[contains(@class,'stdDeviationHeader')]");

        //Growth Recommendation
        private readonly By TeamMaturitySection = By.XPath("//div[@id='maturity-section']//div[contains(@class, 'analyticsMaturityHeader')]");

        //Competency Popup
        private readonly By CompetencyProfileTitleText = By.Id("competencywindow_wnd_title");
        private readonly By CompetencyNameText = By.XPath("//span[contains(@class, 'competencyHeaderName')]");
        private readonly By GrowthPortalButtonText = By.XPath("//*[@id='growthPortalBtn' or @id='growthPortalBtnRec']");
        private readonly By QuestionsTitleText = By.XPath("//*[@id='questionLabel' or @id='questionsLabel' or @id='questionslabelmsgRec']");
        private readonly By AvgResponseText = By.XPath("//*[@id='avgResponseLabel' or @id ='avgResponseLabelRec']");
        private readonly By CompetencyPopupCloseIcon = By.XPath("//span[contains(@id, 'competencywindow_wnd_title')]/following-sibling::div/a");

        //Analytics AllCompetencies
        public IList<string> GetTopFiveCompetenciesList()
        {
            Log.Step(nameof(RadarPage), "Get Top-5 Competencies List");
            return Wait.UntilAllElementsLocated(TopFiveCompetencies).Where(e => e.Displayed).Select(e => e.GetText().ReplaceStringData()).ToList();
        }
        public IList<string> GetLowestFiveCompetenciesList()
        {
            Log.Step(nameof(RadarPage), "Get Lowest-5 Competencies List");
            return Wait.UntilAllElementsLocated(LowestFiveCompetencies).Where(e => e.Displayed).Select(e => e.GetText().ReplaceStringData()).ToList();
        }
        public IList<string> GetFiveHighestConsensusCompetenciesList()
        {
            Log.Step(nameof(RadarPage), "Get Five Highest Consensus Competencies List");
            Wait.HardWait(3000); // Wait Until Page Load
            return Wait.UntilAllElementsLocated(HighestFiveConsensusCompetencies).Where(e => e.Displayed).Select(e => e.GetText().ReplaceStringData()).ToList();
        }
        public IList<string> GetFiveLowestConsensusCompetenciesList()
        {
            Log.Step(nameof(RadarPage), "Get Five Lowest Consensus Competencies List");
            Wait.HardWait(3000); // Wait Until Page Load
            return Wait.UntilAllElementsLocated(LowestFiveConsensusCompetencies).Where(e => e.Displayed).Select(e => e.GetText().ReplaceStringData()).ToList();
        }
        public bool IsHighestConsensusCompetenciesPresent()
        {
            return Driver.IsElementPresent(HighestFiveConsensusCompetencies);
        }
        public bool IsLowestConsensusCompetenciesPresent()
        {
            return Driver.IsElementPresent(LowestFiveConsensusCompetencies);
        }

        //Title
        public string GetRadarTitle()
        {
            var element = Wait.UntilElementExists(RadarTitle);
            Driver.MoveToElement(element);
            return element.GetText();
        }

        //PDF
        public void ClickExportToPdf()
        {
            Log.Step(nameof(RadarPage), "Click on 'Export to PDF' button");
            Wait.UntilElementVisible(ExportPdfButton);
            Wait.UntilElementClickable(ExportPdfButton).Click();
        }

        public void ClickCreatePdf()
        {
            Log.Step(nameof(RadarPage), "Click on 'Create PDF' button");
            Wait.UntilElementVisible(CreatePdfButton);
            Wait.UntilElementClickable(CreatePdfButton).Click();
            Wait.UntilElementVisible(GeneratePdfIndicator);
            Wait.UntilElementInvisible(GeneratePdfIndicator);

            if (Driver.IsInternetExplorer())
            {
                AutoIt.InternetExplorerDownloadClickOnSave(Driver.Title);
            }
            Wait.HardWait(3000); // Wait until file downloaded
        }

        //Excel
        public void ClickExportToExcel()
        {
            Log.Step(nameof(RadarPage), "Click on 'Export to Excel' button");
            Wait.UntilElementVisible(ExportExcelButton);
            Wait.UntilElementClickable(ExportExcelButton).Click();
            Wait.UntilJavaScriptReady(Timeout);
        }

        public void ClickExportQuestionsButton()
        {
            Log.Step(nameof(RadarPage), "Click on 'Question Export' button");
            Wait.UntilElementClickable(ExportQuestionsButton).Click();
        }

        public void ClickExportAssessmentChecklistButton()
        {
            Log.Step(nameof(RadarPage), "Click on the 'Export Assessment Checklist' button");
            Wait.UntilElementClickable(ExportAssessmentChecklistButton).Click();
            Wait.UntilJavaScriptReady();
        }

        //Filter
        public void Filter_OpenFilterSidebar()
        {
            Log.Step(nameof(RadarPage), "Open filter sidebar");
            Wait.UntilElementVisible(FilterSymbol);
            Wait.UntilElementClickable(FilterSymbol).Click();
            Wait.UntilJavaScriptReady();
        }
        public bool IsFilterSidebarIconDisplayed()
        {
            return Driver.IsElementDisplayed(FilterSymbol);
        }
        public List<string> GetFilterSidebarDescriptionTextList()
        {
            var filterSidebarDescriptionTextList = Wait.UntilElementVisible(FilterSidebarDescriptionTextList).GetText();
            return new List<string>(filterSidebarDescriptionTextList.Split(new[] { "\r\n" }, StringSplitOptions.None));
        }
        public bool IsFilterSidebarDescriptionTextDisplayed()
        {
            return Driver.IsElementDisplayed(FilterSidebarDescriptionTextList);
        }
        public List<string> Filter_GetTabsList()
        {
            return Driver.GetTextFromAllElements(FilterTabsList).ToList();
        }
        public List<string> Filter_GetParticipantGroupsTagsList()
        {
            return Driver.GetTextFromAllElements(ParticipantGroupsTagsList).ToList();
        }
        public void Filter_ClickOnParticipantGroupsTab()
        {
            Log.Step(nameof(RadarPage), "Click on 'Participant Groups' tab");
            Wait.UntilElementClickable(FilterParticipantGroupsTab).Click();
            Wait.UntilJavaScriptReady();
        }
        public void Filter_SelectTagCheckboxByTagName(string tagName, bool check = true)
        {
            Log.Step(nameof(RadarPage), $"On Filter, Non Team tab, {(check ? "select" : "deselect")} {tagName}");
            Wait.UntilElementClickable(TagCheckBox(tagName)).Check(check);
            Wait.UntilJavaScriptReady();
        }

        public string Filter_GetTagColor(string tagName)
        {
            var color = Wait.UntilElementVisible(TagColor(tagName)).GetCssValue("background-color");
            return CSharpHelpers.ConvertRgbToHex(color);
        }

        //Tabs
        public void ClickGrowthJourneyTab()
        {
            Log.Step(nameof(RadarPage), "Click on Growth Journey tab");
            Wait.UntilElementVisible(GrowthJourneyTab).Click();
        }
        public bool IsGrowthJourneyTabDisplayed()
        {
            return Driver.IsElementPresent(GrowthJourneyTab);
        }

        //Quick Links
        public void ClickOnJumpLink(string quickLinkName)
        {
            Log.Step(nameof(RadarPage), $"Click on {quickLinkName} quick link");
            Driver.MoveToElement(Wait.UntilElementClickable(JumpLink(quickLinkName))).Click();
        }
        public bool IsJumpLinkPresent(string name)
        {
            try
            {
                return Driver.FindElement(JumpLink(name)).Displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        public void ClickOnGrowthPlanQuickLink()
        {
            Driver.MoveToElement(Wait.UntilElementVisible(GrowthPlanQuickLink)).Click();
        }

        public void ClickOnIdeaboardLink()
        {
            Log.Step(nameof(RadarPage), "Click on Ideaboard quick link");
            Driver.MoveToElement(Wait.UntilElementClickable(IdeaboardQuickLink)).Click();
        }

        //Radar
        public void WaitForRadarPageToLoad()
        {
            Wait.UntilElementVisible(Radar);
        }
        public void ClickCompetency(string competency)
        {
            Log.Step(nameof(RadarPage), "Click on Competency");
            Wait.UntilJavaScriptReady(Timeout);
            Wait.UntilElementExists(DynamicCompetencyLink(competency));
            var element = Wait.UntilAllElementsLocated(DynamicCompetencyLink(competency)).First(e => e.Displayed);
            Driver.JavaScriptScrollToElement(element, false);
            Wait.UntilElementClickable(element).Click();
            Wait.UntilJavaScriptReady(Timeout);
        }

        //All dimension name 
        public List<string> GetAllTheRadarDimensionTextList()
        {
            Log.Step(nameof(RadarPage), "Get all the radar dimension text list");
            return Wait.UntilAllElementsLocated(RadarDimensionList).Select(e => e.GetText().ToLower()).ToList();
        }

        public List<string> GetAllTheRadarSubDimensionTextList()
        {
            Log.Step(nameof(RadarPage), "Get all the radar sub dimension text list");
            return Wait.UntilAllElementsLocated(RadarSubDimensionList).Select(e => e.GetText()).ToList();
        }

        public List<string> GetAllTheRadarCompetencyTextList()
        {
            Log.Step(nameof(RadarPage), "Get all the radar competency text list");
            var competenciesName = new List<string>();

            for (var i = 1; i < 39; i++)
            {
                _ = i == 11 ? i = 43 : i;

                var listOfCompetenciesName = Wait.UntilAllElementsLocated(RadarCompetencyNameList(i)).Select(e => e.GetText()).ToList();
                var joinTwoCompetenciesName = string.Join(" ", listOfCompetenciesName);
                competenciesName.Add(joinTwoCompetenciesName);
                Wait.UntilJavaScriptReady(Timeout);

                i = (i == 43) ? 11 : i;
            }
            return competenciesName;
        }

        //Competency Popup
        public void UpdateCompetency(string competency)
        {
            Log.Step(nameof(RadarPage), "Update Competency");
            Wait.UntilElementVisible(CompetencyValue).SetText(competency);
            Wait.UntilJavaScriptReady(Timeout);
            Driver.JavaScriptScrollToElement(SaveCompetencyButton, false);
            Wait.UntilElementClickable(SaveCompetencyButton).Click();
            Wait.UntilElementNotExist(SaveCompetencyButton);
            Wait.UntilJavaScriptReady(Timeout);
            Wait.UntilJavaScriptReady(Timeout);
        }

        public string GetCompetencyValue()
        {
            return Wait.UntilElementVisible(CompetencyValue).GetElementAttribute("value");
        }

        public void ClickGrowthPlanAddGiButton()
        {
            Log.Step(nameof(RadarPage), "Click Add Growth Item on competency popup");
            Wait.UntilElementClickable(GrowthPlanAddGiButton).Click();
            Wait.UntilJavaScriptReady();
        }

        public void SelectSurveyType(string type)
        {
            Log.Step(nameof(RadarPage), $"Select Survey Type ${type}");
            SelectItem(SurveyTypeList, SurveyTypeListItem(type));
        }

        public bool IsTeamCommentsExcelButtonDisplayed()
        {
            return Driver.IsElementPresent(TeamCommentsExcelButton);
        }

        public void ClickTeamCommentsExcelButton()
        {
            Log.Step(nameof(RadarPage), "Click on the 'Excel' button");
            Wait.UntilElementClickable(TeamCommentsExcelButton).Click();
            Wait.UntilJavaScriptReady(Timeout);
        }

        public bool IsTeamCommentsNoDataPopupDisplayed()
        {
            return Driver.IsElementPresent(NoTeamCommentsCloseButton);
        }

        public string GetCompetencyProfileTitleText()
        {
            Log.Step(nameof(RadarPage), "Get the Competency profile title text");
            return Wait.UntilElementVisible(CompetencyProfileTitleText).GetText();
        }

        public string GetCompetencyNameText()
        {
            Log.Step(nameof(RadarPage), "Get the Competency name text");
            return Wait.UntilElementVisible(CompetencyNameText).GetText();
        }

        public string GetGrowthPortalButtonText()
        {
            Log.Step(nameof(RadarPage), "Get the 'Growth Portal' button text");
            return Wait.UntilElementVisible(GrowthPortalButtonText).GetText();
        }

        public string GetQuestionTitleText()
        {
            Log.Step(nameof(RadarPage), "Get 'Question' title text");
            return Wait.UntilElementVisible(QuestionsTitleText).GetText();
        }

        public string GetAvgResponseText()
        {
            Log.Step(nameof(RadarPage), "Get 'Avg. Response' text");
            return Wait.UntilElementVisible(AvgResponseText).GetText();
        }

        public void ClickOnCompetencyPopupCloseIcon()
        {
            Log.Step(nameof(RadarPage), "Click on Competency popup close icon");
            Wait.UntilElementClickable(CompetencyPopupCloseIcon).Click();
        }

        public bool IsRadarTitleEnabled()
        {
            Log.Step(GetType().Name, "Is 'Radar title' enabled?");
            return Driver.IsElementEnabled(RadarTitle);
        }

        // growth journey
        public void ClickOnAnalysisExportToExcel()
        {
            Log.Step(nameof(RadarPage), "Click on Analysis Export to Excel button");
            Wait.UntilElementClickable(AnalysisExportToExcelButton).Click();
        }

        //Standard Deviation Model
        public string GetHighestConsensusCompetenciesStandardDeviationTitle()
        {
            return Wait.InCase(HighestFiveConsensusCompetenciesStandardDeviation, 3) != null ? Wait.UntilElementVisible(HighestFiveConsensusCompetenciesStandardDeviation).GetText() : "";
        }
        public string GetLowestConsensusCompetenciesStandardDeviationTitle()
        {
            return Wait.InCase(LowestFiveConsensusCompetenciesStandardDeviation, 3) != null ? Wait.UntilElementVisible(HighestFiveConsensusCompetenciesStandardDeviation).GetText() : "";
        }
        public bool IsExportPdfButtonDisplayed()
        {
            return Driver.IsElementDisplayed(ExportPdfButton);
        }
        public bool IsExportExcelButtonDisplayed()
        {
            return Driver.IsElementDisplayed(ExportExcelButton);
        }
        public bool IsExportQuestionsButtonDisplayed()
        {
            return Driver.IsElementDisplayed(ExportQuestionsButton);
        }
        public void NavigateToPage(int teamId, int radarId, TeamType teamType)
        {
            Log.Step(nameof(RadarPage), $"Navigate to radar ${radarId} in team ${teamId}");
            switch (teamType)
            {
                case TeamType.EnterpriseTeam:
                    NavigateToUrl($"{BaseTest.ApplicationUrl}/enterprise/{teamId}/radar/{radarId}");
                    break;
                case TeamType.Team:
                    NavigateToUrl($"{BaseTest.ApplicationUrl}/teams/{teamId}/radar/{radarId}");
                    break;
                case TeamType.MultiTeam:
                    NavigateToUrl($"{BaseTest.ApplicationUrl}/multiteam/{teamId}/radar/{radarId}");
                    break;
                case TeamType.NTier:
                default:
                    throw new ArgumentOutOfRangeException(nameof(teamType), teamType, null);
            }
        }

        //Growth Recommendation
        public string GetTeamMaturityTitle()
        {
            return Wait.InCase(TeamMaturitySection) != null ? Wait.UntilElementVisible(TeamMaturitySection).GetText() : "";
        }

        public void NavigateToGrowthJourney(int teamId, int radarId, TeamType teamType)
        {
            Log.Step(nameof(RadarPage), $"Navigate to growth journey in radar ${radarId} in team ${teamId}");
            switch (teamType)
            {
                case TeamType.EnterpriseTeam:
                    NavigateToUrl($"{BaseTest.ApplicationUrl}/Enterprise/{teamId}/growthjourney/{radarId}");
                    break;
                case TeamType.Team:
                    NavigateToUrl($"{BaseTest.ApplicationUrl}/team/{teamId}/growthjourney/{radarId}");
                    break;
                case TeamType.MultiTeam:
                    NavigateToUrl($"{BaseTest.ApplicationUrl}/multiteam/{teamId}/growthjourney/{radarId}");
                    break;
                case TeamType.NTier:
                default:
                    throw new ArgumentOutOfRangeException(nameof(teamType), teamType, null);
            }
        }

        public List<string> GetAllLanguages()
        {
            Log.Step(nameof(RadarPage), "Get All Languages from Language dropdown");
            Wait.UntilElementClickable(LanguageDropDown).Click();
            Wait.UntilJavaScriptReady();
            var getLanguageAllValue = Driver.GetTextFromAllElements(AllLanguagesList).ToList();
            Wait.UntilElementClickable(LanguageDropDown).Click();
            return getLanguageAllValue;
        }

        public void SelectLanguage(string languageName)
        {
            Log.Step(nameof(RadarPage), $"Select {languageName} from language dropdown");
            SelectItem(LanguageDropDown, PreferredLanguage(languageName));
        }
        public List<string> GetHeaderJumpLinksNamesTextList()
        {
            Log.Step(nameof(RadarPage), "Get header jump links text");
            return Wait.UntilAllElementsLocated(HeaderJumpLinksNamesTextList).Select(e => e.GetElementAttribute("title")).ToList().OrderBy(a => a).ToList();
        }
        public List<string> GetRadarViewDropdownValuesTextList()
        {
            Log.Step(nameof(RadarPage), "Get radar view dropdowns all values");
            Wait.UntilElementClickable(RadarViewDropdownDetailOption).Click();
            Wait.HardWait(1000);//wait till all dropdown values here
            return Driver.GetTextFromAllElements(RadarViewDropDownValuesTextList).ToList();
        }

        public List<string> GetAllRadarActionIconsTitleTextList()
        {
            Log.Step(nameof(RadarPage), "Get all radar action icons title text");
            return Driver.GetAttributeFromAllElements(ActionIconsTitleTextList, "title").ToList();
        }
        public string GetAnalyticsHeaderTitleText()
        {
            Log.Step(nameof(RadarPage), "Get analytics header title text");
            return Wait.UntilElementVisible(AnalyticsHeaderTitleText).GetText();
        }
        public List<string> GetTopFiveCompetenciesTitleText()
        {
            Log.Step(nameof(RadarPage), "Get top 5 competencies title text");
            return Driver.GetTextFromAllElements(TopFiveCompetenciesTitleText).ToList();
        }
        public List<string> GetLowestFiveCompetenciesTitleText()
        {
            Log.Step(nameof(RadarPage), "Get lowest 5 competencies title text");
            return Driver.GetTextFromAllElements(LowestFiveCompetenciesTitleText).ToList();
        }
        public List<string> GetFiveHighestConsensusCompetenciesTitleText()
        {
            Log.Step(nameof(RadarPage), "Get 5 highest consensus competencies title text");
            return Driver.GetTextFromAllElements(FiveHighestConsensusCompetenciesTitleText).ToList();
        }
        public List<string> GetFiveHighestConsensusCompetenciesTooltipDetailsText()
        {
            Log.Step(nameof(RadarPage), "Get five highest consensus competencies tooltip details text");
            Driver.JavaScriptScrollToElementCenter(AnalyticsHeaderTitleText);
            Driver.MoveToElement(FiveHighestConsensusCompetenciesTooltipButton);
            var fiveHighestConsensusCompetenciesToolTipDetails = Wait.UntilElementVisible(FiveHighestConsensusCompetenciesTooltipDescriptionText).GetText();
            return new List<string>(fiveHighestConsensusCompetenciesToolTipDetails.Split(new[] { "\r\n" }, StringSplitOptions.None));
        }
        public string GetFiveHighestConsensusCompetenciesTooltipSupportText()
        {
            Log.Step(nameof(RadarPage), "Get five highest consensus competencies tooltip support text");
            Driver.JavaScriptScrollToElementCenter(AnalyticsHeaderTitleText);
            Driver.MoveToElement(FiveHighestConsensusCompetenciesTooltipButton);
            var fiveHighestConsensusCompetenciesToolTipDetails = Wait.UntilElementVisible(FiveHighestConsensusCompetenciesTooltipSupportText).GetText();
            return fiveHighestConsensusCompetenciesToolTipDetails;
        }
        public List<string> GetLowestFiveConsensusCompetenciesTitleText()
        {
            Log.Step(nameof(RadarPage), "Get 5 lowest consensus competencies title text");
            return Driver.GetTextFromAllElements(FiveLowestConsensusCompetenciesTitleText).ToList();
        }
        public List<string> GetFiveLowestConsensusCompetenciesTooltipDetailsText()
        {
            Log.Step(nameof(RadarPage), "Get tooltip five lowest consensus competencies details text");
            Driver.JavaScriptScrollToElementCenter(AnalyticsHeaderTitleText);
            Driver.MoveToElement(FiveLowestConsensusCompetenciesTooltipButton);
            var fiveLowestConsensusCompetenciesToolTipDetails = Wait.UntilElementVisible(FiveLowestConsensusCompetenciesTooltipDescriptionText).GetText();
            return new List<string>(fiveLowestConsensusCompetenciesToolTipDetails.Split(new[] { "\r\n" }, StringSplitOptions.None));
        }
        public bool IsRadarIconOnHeaderPresent()
        {
            return Driver.IsElementPresent(RadarIconOnHeader);
        }
        public void NavigateToTeamRadarPageForProd(string env, int teamId, int assessmentId)
        {
            NavigateToUrl($"https://{env}.agilityinsights.ai/teams/{teamId}/assessments/{assessmentId}/radar");
        }

        //Select language on Radar page
        public enum RadarLanguage
        {
            [Description("ara")]
            Arabic,
            [Description("chi")]
            Chinese,
            [Description("eng")]
            English,
            [Description("fre")]
            French,
            [Description("ger")]
            German,
            [Description("hun")]
            Hungarian,
            [Description("jpn")]
            Japanese,
            [Description("kor")]
            Korean,
            [Description("pol")]
            Polish,
            [Description("por")]
            Portuguese,
            [Description("spa")]
            Spanish,
            [Description("tur")]
            Turkish
        }
    }
}