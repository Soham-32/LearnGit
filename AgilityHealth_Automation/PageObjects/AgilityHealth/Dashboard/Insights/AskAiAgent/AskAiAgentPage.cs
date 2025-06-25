using System.Linq;
using OpenQA.Selenium;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using AgilityHealth_Automation.Utilities;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Insights.AskAiAgent
{
    public class AskAiAgentPage : InsightsDashboardPage
    {
        public AskAiAgentPage(IWebDriver driver, ILogger log = null) : base(driver, log)
        {
        }

        //Locators
        private readonly By AskAiLogoButton = By.XPath("//button/p[text()='Ask AI']");
        private readonly By WelcomeMessage = By.XPath("//p[contains(text(),'Welcome')]");
        private readonly By HistoryButton = By.XPath("//button[text()='History']");
        private readonly By HistoryPopupCloseIcon = By.CssSelector("svg[data-testid='CloseIcon']");
        private readonly By ExportPdfButton = By.XPath("//button[text()='Export Pdf']");
        private readonly By ClearChatButton = By.XPath("//button[text()='Clear Chat']");
        private readonly By SendButton = By.XPath("//button[text()='Send']");
        private readonly By HistoryPopupTitle = By.XPath("//span[contains(text(),'AskAI Default Questions')]");
        private readonly By TextareaForEnteringQuestion = By.XPath("//textarea[@placeholder='Enter your prompt here']");
        private readonly By DefaultQuestionsTitle = By.XPath("//p[text()='All Suggested Questions']");
        private readonly By DefaultQuestionsCheckbox = By.XPath("//p[text()='Default questions']/parent::div/span/input");
        private readonly By SimilarSuggestedQuestions = By.XPath("//div[contains(@class, 'MuiChip-clickableColorDefault')][@role='button']//span[contains(text(), '?')]");
        private static By QuestionsInHistory(string question)=> By.XPath($"//div[contains(@class,'roleAllocationGrid')]//td[text()='{question}']");
        private readonly By CollapseWindowIcon = By.CssSelector("svg[data-testid='NavigateBeforeIcon']");
        private readonly By ExpandWindowIcon = By.CssSelector("svg[data-testid='NavigateNextIcon']");


        //Methods
        public string GetUsernameDisplayedWithWelcomeMessage()
        {
            Log.Step(nameof(AskAiAgentTab), "Get username after extracting from the welcome message.");
            string welcomeText = Wait.UntilElementVisible(WelcomeMessage).GetText();
            return Regex.Match(welcomeText, @"Welcome,\s*(\w+)!").Groups[1].Value;
        }
        public void ClickOnAskAiLogoButton()
        {
            Log.Step(nameof(AskAiAgentTab), "Click on the AskAi logo button");
            Wait.UntilElementClickable(AskAiLogoButton).Click();
        }
        public void ClickOnExpandWindowIcon()
        {
            Log.Step(nameof(AskAiAgentTab), "Click on the Navigate before icon");
            Wait.UntilElementClickable(CollapseWindowIcon).Click();
        }
        public void ClickOnCollapseWindowIcon()
        {
            Log.Step(nameof(AskAiAgentTab), "Click on the Navigate next icon");
            Wait.UntilElementClickable(ExpandWindowIcon).Click();
        }
        public void ClickOnHistoryButton()
        {
            Log.Step(nameof(AskAiAgentTab), "Click on the History button");
            Wait.UntilElementClickable(HistoryButton).Click();
        }
        public void ClickOnHistoryPopupCloseButton()
        {
            Log.Step(nameof(AskAiAgentTab), "Click on the Close button of History popup");
            Wait.UntilElementClickable(HistoryPopupCloseIcon).Click();
            Wait.HardWait(1000); //Wait for the popup to be closed
        }
        public void ClickOnExportPdfButton()
        {
            Log.Step(nameof(AskAiAgentTab), "Click on the Export PDF button");
            Wait.UntilElementClickable(ExportPdfButton).Click();
        }
        public void ClickOnClearChatButton()
        {
            Log.Step(nameof(AskAiAgentTab), "Click on Clear chat button");
            Wait.UntilElementClickable(ClearChatButton).Click();
        }
        public void ClickOnSendButton()
        {
            Log.Step(nameof(AskAiAgentTab), "Click on the 'Send' button");
            Wait.UntilElementClickable(SendButton).Click();
            Wait.HardWait(5000); //Wait until the response is received
        }
        public void ClickOnDefaultQuestionsCheckbox()
        {
            Log.Step(nameof(AskAiAgentTab), "Click on the 'Default Questions' checkbox");
            Wait.UntilElementClickable(DefaultQuestionsCheckbox).Click();
        }
        public void EnterQuestionInTextarea(string prompt)
        {
            Log.Step(nameof(AskAiAgentTab), $"Enter the question {prompt} in the textarea");
            Wait.UntilElementClickable(TextareaForEnteringQuestion).SendKeys(prompt);
        }
        public bool IsHistoryPopupTitleDisplayed()
        {
            return Driver.IsElementDisplayed(HistoryPopupTitle);
        }
        public bool IsPromptSectionDisplayed()
        {
            return Driver.IsElementDisplayed(TextareaForEnteringQuestion);
        }
        public bool IsDefaultQuestionsTitleDisplayed()
        {
            return Driver.IsElementDisplayed(DefaultQuestionsTitle);
        }
        public bool IsQuestionDisplayedInHistory(string question)
        {
            return Driver.IsElementDisplayed(QuestionsInHistory(question));
        }
        public List<string> GetAllSuggestedQuestions()
        {
            Log.Step(nameof(AskAiAgentTab), "Get all the suggested questions from the textbox");
            return Driver.GetTextFromAllElements(SimilarSuggestedQuestions).ToList();
        }

        public bool IsSendButtonDisabled()
        {
            return Driver.FindElement(SendButton).GetAttribute("disabled") != null;   
        }
    }
}
