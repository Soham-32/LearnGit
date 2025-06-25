using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;
using System.Collections.Generic;
using System.Linq;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.NewNavigation.Teams.Base
{
    public class SubTeamBasePage : CommonGridBasePage
    {
        public SubTeamBasePage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }
        #region Locators

        #region Sub Team stepper
        private readonly By SubTeamStepperTitleText = By.XPath("//div[@class='createTeamContent']//h3");
        private readonly By SubTeamStepperInfoText = By.XPath("//div[@class='page-title-description hidden-xs']/div");
        #endregion

        #region Unassigned Sub Team
        public readonly By SearchTextBox = By.Id("filterBox");
        private static By UnassignedSubTeam(string subTeam) => By.XPath($"//div[@id='SubAllteamsGrid']//td[@class='chkbxAllGrid gridItem'][text()='{subTeam}']");
        private readonly By UnassignedSubTeamList = By.XPath("//div[@id='SubAllteamsGrid']//td[@class='chkbxAllGrid gridItem']");
        #endregion

        #region Assigned Sub Team
        private readonly By AssignedSubTeamList = By.XPath("//div[@id='SubSelectedteamsGrid']/div[@class='k-grid-content']//td[@class='chkbxAllGrid gridItem']");
        private static By AssignedSubTeam(string subTeam) => By.XPath($"//div[@id='SubSelectedteamsGrid']//td[@class='chkbxAllGrid gridItem'][text()='{subTeam}']");
        #endregion

        #endregion


        #region Methods

        #region Sub Team stepper
        public string GetSubTeamStepperTitle()
        {
            Log.Step(nameof(SubTeamBasePage), "On add Sub-Team stepper, get Sub-Team stepper title");
            return Wait.UntilElementVisible(SubTeamStepperTitleText).GetText();
        }
        public string GetSubTeamStepperInfo()
        {
            Log.Step(nameof(SubTeamBasePage), "On Sub-Team stepper, get Sub-Team stepper info");
            return Wait.UntilElementVisible(SubTeamStepperInfoText).GetText();
        }
        #endregion

        #region Unassigned Sub Team
        public void SearchSubTeams(string subTeam)
        {
            Log.Step(nameof(SubTeamBasePage), $"On Add Sub-Team stepper/tab, search {subTeam}");
            Wait.UntilElementExists(SearchTextBox).SetText(subTeam);
            Wait.UntilJavaScriptReady();
        }
        public void RemoveSearchedText()
        {
            Log.Step(nameof(SubTeamBasePage), "Remove the searched text from search box");
            Wait.UntilElementExists(SearchTextBox).Click();
            Wait.UntilElementClickable(SearchTextBox).SendKeys(Keys.Control + "a");
            Wait.UntilElementClickable(SearchTextBox).SendKeys(Keys.Delete);
        }
        public void AssignSubTeam(string subTeam)
        {
            Log.Step(nameof(SubTeamBasePage), $"On Add Sub-Team page, select {subTeam}");
            SearchSubTeams(subTeam);
            Wait.UntilJavaScriptReady();
            Wait.UntilElementClickable(UnassignedSubTeam(subTeam)).Click();
        }
        public bool IsSubTeamDisplayedOnUnassignedTeamsList(string subTeam)
        {
            Log.Step(nameof(SubTeamBasePage), $"On Add Sub-Team stepper/tab, verify that {subTeam} is displayed");
            return Driver.IsElementDisplayed(UnassignedSubTeam(subTeam));
        }
        public List<string> GetUnassignedSubTeamList()
        { 
            Wait.UntilJavaScriptReady();
            return Wait.UntilAllElementsLocated(UnassignedSubTeamList).Select(a => a.GetText()).ToList();
        }
        #endregion

        #region Assigned Sub Team
        public void UnAssignSubTeam(string subTeam)
        {
            Log.Step(nameof(SubTeamBasePage), $"On Add Sub-Team stepper/tab, Select the {subTeam}");
            Wait.UntilElementClickable(AssignedSubTeam(subTeam)).Click();
        }
        public bool IsSubTeamsDisplayedOnAssignedTeamsList(string subTeam)
        {
            Log.Step(nameof(SubTeamBasePage), $"On Add Sub-Team stepper/tab, verify that {subTeam} is displayed");
            return Driver.IsElementDisplayed(AssignedSubTeam(subTeam));
        }
        public List<string> GetAssignedSubTeamList()
        {
            return Wait.UntilAllElementsLocated(AssignedSubTeamList).Select(a => a.GetText()).ToList();
        }
        #endregion

        #endregion
    }
}