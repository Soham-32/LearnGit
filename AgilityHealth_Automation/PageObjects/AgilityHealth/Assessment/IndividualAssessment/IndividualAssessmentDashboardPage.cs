using System;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.IndividualAssessment
{

    internal class IndividualAssessmentDashboardPage : AssessmentDashboardCommon
    {
        public IndividualAssessmentDashboardPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        private readonly By AssessmentTypeListView = By.Id("mtListView");
        private readonly By GrowthJourneyTab = By.Id("growthJourneyTabIndividual");
        private readonly By IndividualAssessmentSurveys = By.Id("divIndividualAssessmentSurveys");

        private static By DynamicAssessmentType(string assessmentType) =>
            By.XPath($"//div[@id = 'mtListView']//h5[text() = '{assessmentType}']/preceding-sibling::div/a");

        private static By DynamicRadarItem(string item) =>
            By.XPath($"//section[@id='individual-assessments']//div//h5[.='{item}' or .//font[text()='{item}']]/ancestor::div[@class='updates']//a");

        private static By DynamicAssessmentStatus(string assessmentName) => By.XPath(
            $"//section[@id='individual-assessments']//div//h5[normalize-space(descendant::text())='{assessmentName}']/following-sibling::div[@class='status']");
        private static By DynamicDraftAssessment(string assessmentName) => By.XPath(
            $"//section[@id='individual-assessments']//div//h5[normalize-space(descendant::text())='{assessmentName}']/following-sibling::div[@class='status'][@title='Draft']\n");

        private static By DynamicAssessmentIndicator(string assessmentName) => By.XPath(
            $"//section[@id='individual-assessments']//div//h5[normalize-space(descendant::text())='{assessmentName}']/following-sibling::div[@class='status']/div[contains(@class, 'indicator')]/div");

        private static By DynamicAssessmentName(string assessmentSelector) =>
            By.XPath($"//section[@id='individual-assessments']//div//{assessmentSelector}");

        private static By DynamicAssessmentParticipant(string assessmentName) => By.XPath(
            $"//section[@id='individual-assessments']//div//h5[text()='{assessmentName}']/following-sibling::div[@class='light participants'][text()=' Participant']");

        private static By DynamicAssessmentReviewer(string assessmentName) => By.XPath(
            $"//section[@id='individual-assessments']//div//h5[contains(normalize-space(),'{assessmentName}')]/following-sibling::div[@class='light participants'][contains(normalize-space(),' Reviewer')]");

        private static By DynamicAssessmentDate(string assessmentName) => By.XPath(
            $"//section[@id='individual-assessments']//div//h5[text()='{assessmentName}']/following-sibling::div[@class='status']//div[@class='date']");

        private static By DynamicRadar(string radar) => By.XPath(
            $"//section[@id='individual-assessments']//div//h5[text()='{radar}']/preceding-sibling::a");

        private static By DynamicRadarLink(string radar, string link) => By.XPath(
            $"//section[@id='individual-assessments']//div//h5[text()='{radar}']/preceding-sibling::div[contains(@class,'links ease')]/a[@title='{link}']");

        private static By DynamicRadarEditLink(string radar) => By.XPath(
            $"//section[@id='individual-assessments']//div//h5[text()='{radar}']/preceding-sibling::div/a[@title='Edit']");
        private static By DynamicEditAssessmentLink(string assessmentName) => By.XPath($"//h5[text()='{assessmentName}']/preceding-sibling::div//a[@title='Edit']");

        public bool IsRadarPresent(string radar)
        {
            return Driver.IsElementDisplayed(DynamicRadarItem(radar));
        }
        public void ClickOnRadar(string radar)
        {
            Log.Step(nameof(IndividualAssessmentDashboardPage), $"Click on the radar <{radar}>");
            Driver.JavaScriptScrollToElement(Wait.UntilElementClickable(DynamicRadarItem(radar)));
            Wait.UntilElementClickable(DynamicRadarItem(radar)).Click();
        }

        public string GetAssessmentStatus(string assessmentName)
        {
            return Wait.UntilElementExists(DynamicAssessmentStatus(assessmentName)).GetElementAttribute("title");
        }

        public string GetAssessmentIndicator(string assessmentName)
        {
            return Wait.UntilElementExists(DynamicAssessmentIndicator(assessmentName)).GetElementAttribute("class");
        }

        public bool IsAssessmentPresent(string assessmentName)
        {
            var selector = $"h5[contains(normalize-space(),'{assessmentName}')]";
            if (assessmentName.ToLower().Contains("roll up"))
            {
                selector = $"h5[contains(normalize-space(),'{assessmentName}')]";
            }
            return Wait.InCase(DynamicAssessmentName(selector)) != null;
        }

        public string GetAssessmentParticipant(string assessmentName)
        {
            return Wait.UntilElementVisible(DynamicAssessmentParticipant(assessmentName))
                .GetElementAttribute("textContent");
        }

        public string GetAssessmentReviewer(string assessmentName)
        {
            return Wait.UntilElementVisible(DynamicAssessmentReviewer(assessmentName))
                .GetElementAttribute("textContent");
        }

        public string GetAssessmentDate(string assessmentName)
        {
            return Wait.UntilElementExists(DynamicAssessmentDate(assessmentName)).GetText();
        }

        public void SelectRadarLink(string radar, string link)
        {
            Log.Step(nameof(IndividualAssessmentDashboardPage), $"Click on the radar <{link}> link for <{radar}>");
            if (Driver.IsInternetExplorer())
            {
                Driver.NavigateToPage(Wait.UntilElementExists(DynamicRadarLink(radar, link)).GetAttribute("href"));
            }
            else
            {
                Driver.MoveToElement(Wait.UntilElementExists(DynamicRadar(radar)));
                Wait.UntilElementClickable(DynamicRadarLink(radar, link)).Click();
            }

            Wait.HardWait(2000);
        }


        public string EditBatch_GetLinkHref(string assessmentName)
        {
            var rollUpName = assessmentName.Split(new[] { " - " }, StringSplitOptions.None)[0] + " - Roll up";
            return Wait.UntilElementExists(DynamicRadarEditLink(rollUpName)).GetAttribute("href");
        }

        public void ClickOnAssessmentType(string assessmentType)
        {
            Wait.UntilJavaScriptReady();
            if (!Driver.IsElementPresent(AssessmentTypeListView)) return;

            Log.Step(nameof(IndividualAssessmentDashboardPage), $"Click on the Assessment type <{assessmentType}>");
            Wait.UntilElementVisible(DynamicAssessmentType(assessmentType)).Click();
        }

        public void ClickGrowthJourneyTab()
        {
            Log.Step(nameof(IndividualAssessmentDashboardPage), "Click on the 'Growth Journey' tab");
            Wait.UntilElementClickable(GrowthJourneyTab).Click();
        }

        public bool EditIndividual_IsLinkHrefPresent(string assessmentName, string individualName)
        {
            var name = assessmentName.Split(new[] { " - " }, StringSplitOptions.None)[0] + $" - {individualName}";

            return Driver.IsElementPresent(DynamicRadarEditLink(name));
        }
        public bool IsAssessmentEditIconDisplayed(string assessmentName)
        {
            return Driver.IsElementPresent(DynamicEditAssessmentLink(assessmentName));
        }

        public bool RadarIndividual_IsLinkHrefPresent(string assessmentName, string link)
        {
            Driver.MoveToElement(Wait.UntilElementExists(DynamicRadar(assessmentName)));
            return Driver.IsElementPresent(DynamicRadarLink(assessmentName, link));
        }

        public void WaitUntilLoaded()
        {
            Wait.UntilElementVisible(IndividualAssessmentSurveys);
            Wait.UntilJavaScriptReady();
        }

        public bool DoesDraftAssessmentDisplay(string assessmentName)
        {
            return Driver.IsElementPresent(DynamicDraftAssessment(assessmentName));
        }

        public new void NavigateToPage(int teamId)
        {
            NavigateToUrl($"{BaseTest.ApplicationUrl}/teams/{teamId}/assessments");
        }
    }
}