using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard
{
    class TeamAssessmentDashboardPage : AssessmentDashboardCommon
    {
        public TeamAssessmentDashboardPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }
        private readonly By SwimLane = By.XPath("//div[@class='dashboard_2 active_class swimlane'] | //div[@class='dashboard_1 active_class swimlane']");
        private readonly By HeadingText = By.Id("headingText");
        private readonly By DashboardList = By.XPath("//div[@id='add-tab-padding']/a[not(@suiveryid)]");
        private readonly By AddAnAssessmentText = By.XPath("//section[@id='team-assessments']//div//h5[text()='Add An Assessment']");
        private readonly By AssessmentEditLinks = By.CssSelector("a[title='Edit']");
        private readonly By GrowthItemsTab = By.XPath("//a[text()='Growth Items']");
        private readonly By RadarLinks =
            By.XPath("//section[@id='team-assessments']//div//h5[text() != 'Add An Assessment']/preceding-sibling::a");
        private readonly By ActiveTeamAssessment = By.XPath("//main[@id='main']/div[3]/a[@href='#']");
        private readonly By ParticipantRadarTab = By.XPath("//div[@id='add-tab-padding']/a[@href='#']");
        private readonly By EditFirstRadar = By.XPath("(//div[@class='profiles']//div[@class='links ease button-bar']//a[@title='Edit'])[1]");
        private readonly By FirstRadar = By.XPath("(//section[@id='team-assessments']//div//*[contains(@class,'assessments-logo')])[1]");
        private static By DynamicRadar(string radarName) =>
            By.XPath($"//section[@id='team-assessments']//div//*[text()='{radarName}']//ancestor::div[@class='updates']/a | //*[text()='{radarName}']//ancestor::div[@class='updates']/a |  //section[@id='team-assessments']//div//h5//font[text()='{radarName}']//..//..//ancestor::div[@class='updates']//a");

        private static By DynamicRadarStatus(string radarName) => By.XPath(
            $"//section[@id='team-assessments']//div//h5[contains(normalize-space(),'{radarName}')]/following-sibling::div[@class='status']");

        private static By DynamicRadarIndicator(string radarName) => By.XPath(
            $"//section[@id='team-assessments']//div//h5[text()='{radarName}']/following-sibling::div[@class='status']/div[contains(@class, 'indicator')]/div | //section[@id='team-assessments']//div//h5//font[text()='{radarName}']//..//../following-sibling::div[@class='status']/div[contains(@class, 'indicator')]/div");

        private static By DynamicRadarName(string radarName) =>
            By.XPath($"//section[@id='team-assessments']//div//h5[text()='{radarName}']");

        private static By DynamicAssessmentDate(string assessmentName) => By.XPath(
            $"//section[@id='team-assessments']//div//h5[text()='{assessmentName}']/following-sibling::div[@class='status']//div[@class='date']");

        private static By DynamicAssessmentParticipantTeamMember(string assessmentName) => By.XPath(
            $"//h5[text()='{assessmentName}']/following-sibling::div[@class='light participants'][1] | //h5//font[text()='{assessmentName}']/..//../following-sibling::div[@class='light participants'][1]");
        private static By DynamicAssessmentTeamCount(string assessmentName) => By.XPath(
            $"//h5[contains(normalize-space(),'{assessmentName}')]/following-sibling::div[@class='light participants'][2]");
        private static By DynamicAssessmentParticipantStakeholders(string assessmentName) => By.XPath(
            $"//section[@id='team-assessments']//div//h5[text()='{assessmentName}']/following-sibling::div[@class='light participants'][2]");
        private readonly By AssessmentHeaderTitle = By.XPath("//div[@class='pg-title']//h1");

        private static By DynamicRadarLink(string radar, string link) =>
            By.XPath(
                $"//section[@id='team-assessments']//div//h5[contains(normalize-space(),'{radar}')]/preceding-sibling::div[contains(@class,'links ease')]/a[@title='{link}']");

        private static By DynamicEtMtRadarName(string radar) =>
            By.XPath($"//div[@id='mtListView']//h5[text()='{radar}']/preceding-sibling::div/a");

        private static By DynamicEditAssessmentLink(string assessmentName) => By.XPath($"h5[text()='{assessmentName}']/following-sibling::div/a[@title='Edit'] | //h5[text()='{assessmentName}']/preceding-sibling::div//a[@title='Edit'] | //font[text()='{assessmentName}']//ancestor::h5//preceding-sibling::div//a[@title = 'Edit'] | //h5[text()='{assessmentName}']/parent::*//a[@title='Edit']");
        
        //Pulse Assessments

        private static By PulseRadarName(string radarName) =>
            By.XPath($"//section[@id='team-assessments-pulse']//h5[contains(normalize-space(),'{radarName}')]");

        private static By PulseRadar(string radarName) =>
            By.XPath($"//*[text()='{radarName}']//ancestor::div[@class='updates']//a[contains(@class,'assessments-logo')]");

        private static By PulseEditLink(string radar) =>
            By.XPath($"//*[text()='{radar}']//ancestor::div[@class='updates']//a[@title='Edit']");

        private static By PulseRadarStatus(string radarName, string status) => By.XPath(
            $"//h5[normalize-space(text())='{radarName}']/following-sibling::div[@class='status' and @title='{status}'] |//h5[normalize-space(text())='{radarName}']/ancestor::div[contains(@class,'updates')]/following-sibling::div[@class='status' and @title='{status}']");

        // return the By locator for the radar link so it can be used in multiple methods
        public bool IsParticipantRadarTabDisplayed()
        {
            return Wait.InCase(ParticipantRadarTab) != null;
        }
        private By GetRadarLink(string radarName)
        {
            return DynamicRadar(radarName);
        }

        public string GetHeadingText()
        {
            Log.Step(nameof(TeamAssessmentDashboardPage), "Get Heading text");
            return Wait.UntilElementVisible(HeadingText).GetText();
        }
        public List<string> GetDashboardsList()
        {
            Log.Step(nameof(TeamAssessmentDashboardPage), "Get All dashboards list");
            return Driver.GetTextFromAllElements(DashboardList).ToList();
        }

        public void ClickOnRadar(string radar)
        {
            Log.Step(nameof(TeamAssessmentDashboardPage), $"Click on radar <{radar}>");
            Driver.JavaScriptScrollToElement(DynamicRadar(radar), false);
            Wait.UntilElementClickable(DynamicRadar(radar)).ClickOn(Driver);
            Wait.UntilJavaScriptReady();
        }
        public void ClickOnFirstRadarFromAssessmentDashboard()
        {
            Log.Step(nameof(TeamAssessmentDashboardPage), "Click on first radar from team assessment dashboard");
            Wait.UntilElementClickable(FirstRadar).Click();
            Wait.UntilJavaScriptReady();
        }
        public void EditFirstRadarFromAssessmentDashboard()
        {
            Log.Step(nameof(TeamAssessmentDashboardPage), "Edit first radar from team assessment dashboard");
            Wait.UntilElementClickable(EditFirstRadar).Click();
            Wait.UntilJavaScriptReady();
        }

        public bool IsAssessmentRadarGrayedOut(string assessmentName)
        {
            Log.Step(nameof(TeamAssessmentDashboardPage), $"Is {assessmentName} assessment radar is grayed out?");
            Wait.UntilElementVisible(DynamicRadar(assessmentName));
            Driver.JavaScriptScrollToElement(DynamicRadar(assessmentName), false);
            return Wait.UntilElementVisible(DynamicRadar(assessmentName)).GetAttribute("class").Equals("assessment-logo-grayed");
        }

        public void NavigateToAssessmentRadarPageByIndex(int index)
        {
            Log.Step(nameof(TeamAssessmentDashboardPage), $"Navigate to assessment radar by index <{index}>");
            var href = Wait.UntilAllElementsLocated(RadarLinks)[index].GetAttribute("href");
            Driver.NavigateToPage(href);
        }

        public string GetRadarEditHref(string assessmentName)
        {
            return Wait.UntilElementExists(GetRadarLink(assessmentName)).GetAttribute("href").Replace("/radar", "/edit");
        }

        public string[] GetAssessmentStatus(string assessmentName)
        {

            var status = Wait.UntilElementExists(DynamicRadarStatus(assessmentName)).GetElementAttribute("title");
            var indicator = Wait.UntilElementExists(DynamicRadarIndicator(assessmentName)).GetElementAttribute("class");
            string[] data = { status, indicator };

            return data;
        }
        public bool IsAssessmentActive()
        {
            return Wait.InCase(ActiveTeamAssessment) != null;
        }
        public bool IsAssessmentPresent(string assessmentName)
        {
            return Wait.InCase(DynamicRadarName(assessmentName)) != null;
        }
        public bool IsAnyAssessmentsDisplayed()
        {
            return !Driver.IsElementDisplayed(AddAnAssessmentText);
        }
        public bool IsSwimLaneDisplayed()
        {
            return Driver.IsElementDisplayed(SwimLane);
        }

        public string GetAssessmentParticipantTeamMembers(string assessmentName)
        {
            return Wait.UntilElementExists(DynamicAssessmentParticipantTeamMember(assessmentName)).GetAttribute("textContent");
        }

        public string GetAssessmentParticipantStakeholders(string assessmentName)
        {
            return Wait.UntilElementExists(DynamicAssessmentParticipantStakeholders(assessmentName)).GetAttribute("textContent");
        }

        public string GetAssessmentDate(string assessmentName)
        {
            return Wait.UntilElementExists(DynamicAssessmentDate(assessmentName)).GetText();
        }


        public string GetAssessmentDashboardTitle()
        {
            return Wait.UntilElementVisible(AssessmentHeaderTitle).GetText();
        }



        public void SelectRadarLink(string radar, string link)
        {
            Log.Step(nameof(TeamAssessmentDashboardPage), $"Select link <{link}> on radar <{radar}>");
            var radarElement = Wait.UntilElementVisible(GetRadarLink(radar));
            if (Driver.IsInternetExplorer())
            {
                Driver.NavigateToPage(radarElement.GetAttribute("href").Replace("/radar", "/" + link));
            }
            else
            {
                Driver.JavaScriptScrollToElement(radarElement);
                Driver.MoveToElement(radarElement);
                Wait.HardWait(4000);//Edit link takes to be enabled
                Wait.UntilElementClickable(DynamicRadarLink(radar, link)).Click();
                Wait.HardWait(2000);//Edit page takes time to load
            }

        }


        public bool DoesAssessmentExist(string assessmentName)
        {
            return Driver.GetElementCount(DynamicRadarName(assessmentName)) > 0;
        }

        public void ClickMtEtRadar(string radar)
        {
            Log.Step(nameof(TeamAssessmentDashboardPage), $"Click MT/ET radar <{radar}>");
            Wait.UntilElementClickable(DynamicEtMtRadarName(radar)).ClickOn(Driver);
        }


        public List<string> GetAssessmentEditLinks()
        {
            var links = Wait.UntilAllElementsLocated(AssessmentEditLinks);

            return links.Select(element => element.GetAttribute("href")).ToList();
        }

        public bool IsAssessmentEditIconDisplayed(string assessmentName)
        {
            var radarElement = Wait.UntilElementVisible(GetRadarLink(assessmentName));
            Driver.JavaScriptScrollToElementCenter(GetRadarLink(assessmentName));
            Driver.MoveToElement(radarElement);
            return Driver.IsElementDisplayed(DynamicEditAssessmentLink(assessmentName));
        }
        public void ClickOnTheEditIcon(string assessmentName)
        {
            Log.Step(nameof(TeamAssessmentDashboardPage), $"Click on the 'Edit' icon {assessmentName}");
            var radarElement = Wait.UntilElementVisible(GetRadarLink(assessmentName));
            Driver.JavaScriptScrollToElement(radarElement);
            Driver.MoveToElement(radarElement);
            Wait.UntilElementClickable(DynamicEditAssessmentLink(assessmentName)).Click();
        }
        public void SelectGrowthItemsTab()
        {
            Log.Step(nameof(TeamAssessmentDashboardPage), "Select Growth Items tab");
            Wait.UntilElementClickable(GrowthItemsTab).Click();
            Wait.UntilJavaScriptReady();
        }


        // PulseAssessmentV2
        private static By GetPulseRadarLink(string radarName)
        {
            return PulseRadar(radarName);
        }

        public bool IsPulseAssessmentDisplayed(string assessmentName)
        {
            return Driver.IsElementDisplayed(PulseRadarName(assessmentName));
        }

        public void ClickOnPulseEditLink(string radar)
        {
            Log.Step(nameof(TeamAssessmentDashboardPage), $"Click on Pulse Edit radar <{radar}>");
            var radarElement = Wait.UntilElementVisible(GetPulseRadarLink(radar));

            Driver.JavaScriptScrollToElement(radarElement, false);
            Driver.MoveToElement(radarElement);
            Wait.UntilJavaScriptReady();
            Wait.UntilElementClickable(PulseEditLink(radar)).Click();
            Wait.HardWait(21000);// Edit pulse page is taking time to load the page.
        }

        public bool IsPulseStatusDisplayed(string radarName, string status)
        {
            return Driver.IsElementDisplayed(PulseRadarStatus(radarName, status));
        }

        public void ClickOnPulseRadar(string radarName)
        {
            Log.Step(nameof(TeamAssessmentDashboardPage), $"Click on pulse radar <{radarName}>");
            Driver.JavaScriptScrollToElement(PulseRadar(radarName), false);
            Wait.UntilElementClickable(PulseRadar(radarName)).Click();
        }

        public string GetPulseAssessmentTeamMemberCompletedInfo(string assessmentName)
        {
            Log.Step(nameof(TeamAssessmentDashboardPage), "Get Team Member Completed info");
            return Wait.UntilElementVisible(DynamicAssessmentParticipantTeamMember(assessmentName)).GetText();
        }

        public string GetPulseAssessmentTeamCompletedInfo(string assessmentName)
        {
            Log.Step(nameof(TeamAssessmentDashboardPage), "Get team completed info");
            return Wait.UntilElementVisible(DynamicAssessmentTeamCount(assessmentName)).GetText();
        }
        public void NavigateToTeamAssessmentPage(string env, int teamId)
        {
            NavigateToUrl($"https://{env}.agilityinsights.ai/teams/{teamId}/assessments");
        }
        public void NavigateToParticipantRadarPage(string env, int teamId, int assessmentId)
        {
            NavigateToUrl($"https://{env}.agilityinsights.ai/teams/{teamId}/assessments/{assessmentId}/radar");
        }
        public void NavigateToRollUpRadarPage(string env, int assessmentId)
        {
            NavigateToUrl($"https://{env}.agilityinsights.ai/assessments/{assessmentId}/multiindividualradar");
        }
    }
}