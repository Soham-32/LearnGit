using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Survey;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.ManageRadar
{
    internal class PreviewAssessmentPage : SurveyPage
    {
        public PreviewAssessmentPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        private readonly By AllOpenEndedText = By.XPath("//p[@class='openEndedQuestion']");
        private static By LeftNavDimension(string dimension) => By.XPath($"//ul[@id='menu']//li[normalize-space()='{dimension}']");
        private readonly By WelcomeButton = By.Id("welcomeButton");

        private static By QuestionTooltipIcon(string competency) => By.XPath($"//*[contains(text(),'{competency}')]/ancestor::li//span[@data-role='tooltip']");
        private readonly By QuestionTooltipMessage = By.XPath("//div[@role='tooltip' and @id]");
        private static By NA_Text(string competency) => By.XPath($"//*[contains(text(),'{competency}')]//ancestor::li//td[@class='tooltipNA']"); 
        private static By ProgressAnswerText(string competency) => By.XPath($"//*[contains(text(),'{competency}')]//ancestor::li//div[contains(@class,'progressAnswerStyle')]");

        public void ClickOnWelcomeButton()
        {
            Log.Step(nameof(PreviewAssessmentPage), "Click on 'Welcome dimension'");
            Wait.UntilElementClickable(WelcomeButton).Click();
        }

        public void ClickOnDimension(string dimension)
        {
            Log.Step(nameof(PreviewAssessmentPage), $"Click on '{dimension} dimension'");
            Wait.UntilElementClickable(LeftNavDimension(dimension)).Click();
        }

        public List<string> GetDimensionList()
        {
            Log.Step(nameof(PreviewAssessmentPage), "Get Dimensions List");
            var dimensionList = GetLeftNavDimensionsList().ToList();
            dimensionList.Remove(dimensionList.LastOrDefault());
            return dimensionList;
        }

        public Dictionary<string, IList<string>> GetAssessmentDetailsForDimension(string dimensionName)
        {
            Log.Step(nameof(PreviewAssessmentPage), "Get Lists of SubDimension, Competency and Questions ");
            ClickStartSurveyButton();
            for (var i = 0; i < GetDimensionList().Count; i++)
            {
                ClickNextButton();
            }

            ClickOnWelcomeButton();
            Wait.UntilJavaScriptReady();
            ClickOnDimension(dimensionName);
            Wait.UntilJavaScriptReady();

            //sub Dimension
            var subDimensionDimensionList = GetAllSubDimensionsHeaderTextList();
            var subDimensionList = subDimensionDimensionList.Select(subDimension => (subDimension.Replace($"{dimensionName} - ", ""))).ToList();

            //question
            var questionList = new List<string>();
            foreach (var subDimension in subDimensionDimensionList)
            {
                var subDimensionQuestionList = GetAllQuestionsBySubDimensionList(subDimension).ToList();
                questionList.AddRange(subDimensionQuestionList);
                subDimensionQuestionList.Clear();
            }

            //competency
            var competenciesList = questionList.Select(question => question.Split(' ')).Select(s2 => s2.LastOrDefault()).ToList();
            if (competenciesList.FirstOrDefault()== "の質問テキストはここにあります")
            {
                competenciesList = questionList.Select(question => question.Split(' ')).Select(s2 => s2.FirstOrDefault()).ToList();
            }
            else if (competenciesList.FirstOrDefault().Contains("Kompetencja"))
            {
                competenciesList =  questionList.Select(question => question.Split(' ')).Select(s2 => System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(s2.LastOrDefault()) ).ToList();
            }
            else if(competenciesList.FirstOrDefault() == "9-10")
            {
                competenciesList = questionList.Select(question => question.Split(':')).Select(s2 => s2.FirstOrDefault()).ToList();
            }
            else if(competenciesList.FirstOrDefault() == "|飞行：9-10")
            {
                competenciesList = questionList.Select(question => question.Split('\r')).Select(s2 => s2.First())
                    .ToList();
            }
            
            Driver.RefreshPage();
            return new Dictionary<string, IList<string>>
            {
                { "SubDimensions",subDimensionList},
                { "Questions",questionList},
                { "Competencies",competenciesList},
            };
        }

        public string GetCompetencyTooltipMessage(string competency)
        {
            Log.Step(nameof(PreviewAssessmentPage), $"Get {competency} competency tooltip message");
            Driver.MoveToElement(QuestionTooltipIcon(competency));
            return string.Join("", Driver.GetTextFromAllElements(QuestionTooltipMessage).ToList()).Replace("\r\n\r\n","").Replace("\r\n","");
        }

        public string GetNA_Text(string competency)
        {
            Log.Step(nameof(PreviewAssessmentPage), $"Get {competency} N/A text");
            return Wait.UntilElementVisible(NA_Text(competency)).Text;
        }

        public string GetProgressAnswerText(string competency)
        {
            Log.Step(nameof(PreviewAssessmentPage), $"Get {competency} Progress answer text");
            return Wait.UntilElementVisible(ProgressAnswerText(competency)).Text;
        }

        public List<string> GetOpenEndedList()
        {
            Log.Step(nameof(PreviewAssessmentPage), "Get OpenEnded List");
            ClickStartSurveyButton();
            for (var i = 0; i < GetDimensionList().Count; i++)
            {
                ClickNextButton();
            }
            var openEndedList = Driver.GetTextFromAllElements(AllOpenEndedText).ToList();
            Driver.RefreshPage();
            return openEndedList;
        }
    }
}