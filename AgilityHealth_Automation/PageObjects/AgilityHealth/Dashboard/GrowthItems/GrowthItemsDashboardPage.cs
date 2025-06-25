using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.GrowthItems
{
    public class GrowthItemsDashboardPage : BasePage
    {
        public GrowthItemsDashboardPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        private readonly By ToggleView = By.Id("toggle-header");
        private readonly By KanbanTag = By.Id("pTagKanBan");
        internal readonly By KanbanBoardDiv = By.Id("mainDiv");
        private readonly By AssessmentTypeToggleButton = By.Id("toggle-header-TeamInd");
        private static By GiEditButtonFromGrid(string title) => By.XPath($"//div[@id='GrowthPlan']//table/tbody/tr/td[contains(normalize-space(), '{title}')]/following-sibling::td//span[text()='Edit']");

        private static By CopyLinkWithAppliedFiltersButton => By.Id("ClipboardFilters");
        private static By SelectedCategoryFilterText => By.XPath("//span[@aria-owns='categories_listbox']//span//span");
        private static By SelectedRadarTypeFilterText => By.XPath("//span[@aria-owns='surveytype_listbox']//span//span");

        private readonly By GrowthItemsDashboardTitle = By.XPath("//h1[contains(text(),'Growth Items')]");

        //AI Insights
        private readonly By AiSummarizationButton = By.XPath("//div[@class='ai-widget']//*[text()='AI Summarization']");
        private readonly By RegenerateButton = By.Id("regenerate");
        private readonly By ResponseOfThePrompt = By.Id("insights_content");
        private readonly By AiSummarizationPopUp = By.Id("insights_window");
        private readonly By SummarizeByTeamCheckbox = By.Id("genai-teams-checkbox");
        private readonly By BackToPromptsLink = By.XPath("//div[text()='< Back to Prompts']");
        private readonly By CopyLink = By.Id("copy_text");
        private readonly By ReadOurDataPrivacyPolicyLink = By.CssSelector("a.insights_privacy");
        private static By AiSummarizationPrompt(string title) => By.XPath($"//div[@class='ai-prompt'][text()=\"{title}\"]"); //Cannot add single quote because there is single quotes in the titles.
        private static By PromptInResponseWindow(string title) => By.XPath($"//div[@class='insights_content_title'][text()=\"{title}\"]");
        public void ChangeViewWidget(GrowthItemWidget widget)
        {
            Log.Step(nameof(GrowthItemsDashboardPage), $"Change widget view to <{widget:G}>");
            if (widget == GrowthItemWidget.Kanban)
            {
                if (Wait.InCase(KanbanTag).Displayed) return;
                Wait.UntilElementVisible(ToggleView);
                Wait.UntilElementClickable(ToggleView).Click();
                Wait.UntilElementVisible(KanbanBoardDiv);
            }
        }
        public void ChangeAssessmentTypeView(AssessmentWidget widget)
        {
            Log.Step(nameof(GrowthItemsDashboardPage), $"Change widget view to <{widget:G}>");
            if (widget != AssessmentWidget.Individual) return;
            Wait.UntilElementVisible(AssessmentTypeToggleButton);
            Wait.UntilElementClickable(AssessmentTypeToggleButton).Click();
        }

        public void ClickOnEditGrowthItem(string title)
        {
            Log.Step(nameof(GrowthItemsDashboardPage), $"Click on Edit button for <{title}>");
            Wait.UntilElementClickable(GiEditButtonFromGrid(title)).Click();
            Wait.UntilJavaScriptReady();
        }
        public bool IsEditGrowthItemButtonDisplayed(string title) => Driver.IsElementDisplayed(GiEditButtonFromGrid(title));
        public bool IsCopyLinkWithAppliedFiltersButtonDisplayed()
        {
            return Driver.IsElementDisplayed(CopyLinkWithAppliedFiltersButton);
        }

        public void ClickOnCopyLinkWithAppliedFiltersButton()
        {
            Log.Step(GetType().Name, "Click on 'Copy Link with Applied Filters' button");
            Wait.UntilElementVisible(CopyLinkWithAppliedFiltersButton);
            Wait.UntilElementClickable(CopyLinkWithAppliedFiltersButton).Click();
            Wait.UntilJavaScriptReady();
        }
        public string GetSelectedCategoryFilterText()
        {
            Log.Step(GetType().Name, "Get selected category filter text");
            Wait.UntilJavaScriptReady();
            return Wait.UntilElementExists(SelectedCategoryFilterText).GetText();
        }
        public string GetSelectedRadarTypeFilterText()
        {
            Log.Step(GetType().Name, "Get selected radar type filter text");
            Wait.UntilJavaScriptReady();
            return Wait.UntilElementExists(SelectedRadarTypeFilterText).GetText();
        }
        public string GetGrowthItemsDashboardTitle()
        {
            return Wait.UntilElementVisible(GrowthItemsDashboardTitle).GetText();
        }
        public void NavigateToCopiedUrl(string url)
        {
            Log.Step(GetType().Name, $"Navigating to copied URL: <{url}>");
            Driver.SwitchTo().NewWindow(WindowType.Tab);
            Wait.UntilJavaScriptReady();
            Driver.NavigateToPage(url);
        }
        //AI Insights
        public bool IsAiSummarizationButtonDisplayed()
        {
            Wait.HardWait(5000); //page rendering
            return Driver.IsElementDisplayed(AiSummarizationButton);
        }
        public void NavigateToPage(int companyId)
        {
            NavigateToUrl($"{BaseTest.ApplicationUrl}/company/{companyId}/growthitems");
        }

        public void NavigateToGrowthItemsPageForProd(string env, int companyId)
        {
            NavigateToUrl($"https://{env}.agilityinsights.ai/company/{companyId}/growthitems");
        }

        public void ClickOnAiSummarizationButton()
        {
            Log.Step(GetType().Name, "Click on AI-Summarization toggle button.");
            Wait.UntilElementClickable(AiSummarizationButton).Click();
        }

        public bool IsAiSummarizationPopUpDisplayed()
        {
            Wait.UntilJavaScriptReady();
            return Driver.IsElementDisplayed(AiSummarizationPopUp,10);
        }

        public void ClickOnSummarizeByTeamCheckbox()
        {
            Log.Step(GetType().Name, "Click on 'Summarize By Team' checkbox.");
            Wait.UntilElementClickable(SummarizeByTeamCheckbox).Click();
        }
        public bool IsSummarizeByTeamCheckboxChecked()
        {
            return Wait.UntilElementClickable(SummarizeByTeamCheckbox).Selected;
        }

        public void ClickOnThePrompt(string prompt)
        {
            Log.Step(GetType().Name, $"Click on the prompt: {prompt}");
            Wait.UntilElementClickable(AiSummarizationPrompt(prompt)).Click();
            Wait.HardWait(5000);
        }
        public bool IsRegenerateButtonDisplayed()
        {
            return Driver.IsElementDisplayed(RegenerateButton);
        }

        public void ClickOnBackToPromptsLink()
        {
            Log.Step(GetType().Name, "Click on 'Back to Prompts' link.");
            Wait.UntilElementClickable(BackToPromptsLink).Click();
            Wait.HardWait(1000);
        }

        public bool IsPromptResponseDisplayed(string prompt)
        {
            return Driver.IsElementDisplayed(PromptInResponseWindow(prompt), 10) && Driver.IsElementDisplayed(ResponseOfThePrompt, 10);
        }

        public bool IsCopyLinkDisplayed()
        {
            Wait.HardWait(6000); //window frame takes time to load
            Wait.UntilJavaScriptReady();
            return Driver.IsElementDisplayed(CopyLink, 10);
        }

        public bool IsReadOurDataAndPrivacyPolicyLinkDisplayed()
        {
            return Driver.IsElementDisplayed(ReadOurDataPrivacyPolicyLink);
        }

        //Key-customers-verification
        #region
        public bool IsKanbanBoardDisplayed() => Driver.IsElementDisplayed(KanbanBoardDiv);
        #endregion

    }

    public enum GrowthItemWidget
    {
        Grid,
        Kanban
    }
    public enum AssessmentWidget
    {
        Team,
        Individual
    }
}