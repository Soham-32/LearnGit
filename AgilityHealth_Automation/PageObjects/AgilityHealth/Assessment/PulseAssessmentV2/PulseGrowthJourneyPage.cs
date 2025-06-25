using System;
using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.GrowthJourney;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.PulseAssessmentV2
{
    public class PulseGrowthJourneyPage : GrowthJourneyPage
    {
        public PulseGrowthJourneyPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        //Locators
        private readonly By PulseAssessmentName = By.XPath("//div[@class='pulse-header']");
        private static By PulseNameList(string pulseName) => By.XPath($"//div[@id='chart_container']//span[contains(text(), '{pulseName}')]");

        private readonly By PulseResultsCompetencyDot =
            By.CssSelector("div[id^='highcharts'] > svg > rect.highcharts-plot-border");

        private readonly By ParticipantCount = By.XPath("//span[contains(text(),'Participants')]/parent::div/span");

        //Pulse Analysis

        private readonly By PulseAnalysisSection = By.Id("pulse_compare_analysis_anchor");
        private static By SurveyedVsTotalParticipantCount(string teamName) =>
            By.XPath($"//table//tr/td[text()='{teamName}']//following-sibling::td[1] | //table//tr/td//font//font[text()='{teamName}']//ancestor::td//following-sibling::td[1]");

        //Methods
        public List<string> GetPulseAssessmentNames()
        {
            Log.Info("Get the pulse assessment names");
            return Wait.UntilAllElementsLocated(PulseAssessmentName).Select(a => a.GetText()).ToList();
        }

        public bool IsPulseResultsDisplayed(string pulseName)
        {
            return Driver.IsElementDisplayed(PulseNameList(pulseName));
        }

        public int GetTheCountOfSurveyedParticipants()
        {
            string a = null;
            for (var i = 0; i < 5; i++)
            {
                Driver.MoveToElement(Wait.UntilElementVisible(PulseResultsCompetencyDot));
                if (!Driver.IsElementPresent(ParticipantCount)) continue;
                a= Wait.UntilElementExists(ParticipantCount).GetText().Split(':').Last();
                break;
            }
            return Convert.ToInt32(a);
        }

        public string GetTheCountOfSurveyedVsTotalMembers(string teamName)
        {
            Driver.JavaScriptScrollToElement(PulseAnalysisSection);
            return Wait.UntilElementExists(SurveyedVsTotalParticipantCount(teamName)).GetText();
        }
    }
}
