using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;
using System.Collections.Generic;
using System.Linq;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.NewNavigation.Teams.Base
{
    public class ReviewBasePage : CommonGridBasePage
    {
        public ReviewBasePage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }
        #region Locators

        #region Team Profile
        private readonly By TeamProfileSection = By.Id("viewprofile");
        private readonly By TeamProfileEditButton = By.XPath("//div[@id='viewprofile']/a");
        #endregion

        #region Team Tags
        private readonly By CategoriesList = By.XPath("//div[@id='collapseOne']//label");
        private readonly By TeamTagsSection = By.XPath("//div[@id='headingOne']/h4");
        private readonly By TeamTagsEditButton = By.XPath("//span[text()='Team Tags']/following-sibling::span/a");
        private static By TagList(string item) => By.XPath($"//label[contains(text(),'{item}')]//ancestor::div[@class='row rowpadtag']/following-sibling::div[1]//span");
        #endregion

        #region Sub Teams
        private readonly By SubTeamTeamNamesList = By.XPath("//table[@id='subteams']//td[2]");
        private readonly By SubTeamTeamTagList = By.XPath("//table[@id='subteams']//td[3]/span");
        private readonly By SubTeamsEditButton = By.XPath("//div[@id='headingFour']/h4/a");
        private readonly By SubTeamsSection = By.XPath("//div[@id='headingFour']/h4");
        #endregion

        #region Leaders
        private readonly By LeadersEditButton = By.XPath("//div[@id='headingFive']/h4/a");
        private readonly By LeadersSection = By.XPath("//div[@id='headingFive']/h4");
        #endregion

        #endregion


        #region Methods

        #region Team Profile
        public string GetTeamProfileText()
        {
            return Wait.UntilElementVisible(TeamProfileSection).GetText();
        }

        public void ClickOnTeamProfileEditButton()
        {
            Log.Step(nameof(ReviewBasePage), "Click on team profile 'Edit' button");
            Wait.UntilElementClickable(TeamProfileEditButton).Click();
        }
        #endregion

        #region Team Tags
        public List<KeyValuePair<string, string>> GetTeamTagsText()
        {
            var categoryList = Driver.GetTextFromAllElements(CategoriesList).ToList();
            return (from category in categoryList let tagList = Driver.GetTextFromAllElements(TagList(category)).ToList() from tag in tagList select new KeyValuePair<string, string>(category, tag)).ToList();
        }

        public void ClickOnTeamTagsEditButton()
        {
            Log.Step(nameof(ReviewBasePage), "On Review & Finish page, Click on the 'Edit' link for Team Tags");
            Wait.UntilElementVisible(TeamTagsEditButton);
            Driver.JavaScriptScrollToElement(TeamTagsEditButton);
            Wait.HardWait(5000);  //Wait till 'Team Tag' Edit button is displayed
            Wait.UntilElementClickable(TeamTagsEditButton).Click();
            Wait.UntilJavaScriptReady();
        }

        public void ClickOnTeamTagsSectionHeader()
        {
            Log.Step(nameof(ReviewBasePage), "Click on 'Team Tag' header section");
            Driver.JavaScriptScrollToElement(TeamTagsSection);
            Wait.HardWait(3000);//need to wait to scroll to Team Tag section
            Wait.UntilElementClickable(TeamTagsSection).Click();
        }

        public bool IsTeamTagsSectionExpanded()
        {
            return bool.Parse(Wait.UntilElementExists(TeamTagsSection).GetAttribute("aria-expanded"));
        }
        #endregion

        #region Sub Teams
        public void ClickOnTeamSubTeamsEditButton()
        {
            Log.Step(nameof(ReviewBasePage), "On Review & Finish page, Click on the 'Edit' link for 'Sub-Teams'");
            Driver.JavaScriptScrollToElement(SubTeamsEditButton);
            Wait.HardWait(1000); //need to wait to load the stepper
            Wait.UntilElementClickable(SubTeamsEditButton).Click();
            Wait.UntilJavaScriptReady();
        }
        public void ClickOnSubTeamsSection()
        {
            Log.Step(nameof(ReviewBasePage), "Click on 'Sub Teams' header section");
            Driver.JavaScriptScrollToElement(SubTeamsSection);
            Wait.HardWait(3000);//need to wait to scroll to Sub Teams section
            Wait.UntilElementClickable(SubTeamsSection).Click();
            Wait.UntilJavaScriptReady();
        }
        public bool IsMultiSubTeamsSectionExpanded()
        {
            return bool.Parse(Wait.UntilElementExists(SubTeamsSection).GetAttribute("aria-expanded"));
        }
        public IList<string> GetSubTeamsTextList()
        {
            return Driver.GetTextFromAllElements(SubTeamTeamNamesList);
        }
        public List<string> GetSubTeamsTagList()
        {
            return Driver.GetTextFromAllElements(SubTeamTeamTagList).ToList();
        }
        #endregion

        #region Leaders
        public void ClickOnLeadersEditButton()
        {
            Log.Step(nameof(ReviewBasePage), "On Review & Finish page, Click on the 'Edit' link for 'Leaders'");
            Driver.JavaScriptScrollToElement(LeadersEditButton);
            Wait.HardWait(1000); //need to wait to load the stepper
            Wait.UntilElementClickable(LeadersEditButton).Click();
            Wait.UntilJavaScriptReady();
        }
        public void ClickOnLeadersSection()
        {
            Log.Step(nameof(ReviewBasePage), "Click on 'Leaders' header section");
            Driver.JavaScriptScrollToElement(LeadersSection);
            Wait.HardWait(3000);//need to wait to scroll to Leaders section
            Wait.UntilElementClickable(LeadersSection).Click();
            Wait.UntilJavaScriptReady();
        }
        public bool IsLeadersSectionExpanded()
        {
            return bool.Parse(Wait.UntilElementExists(LeadersSection).GetAttribute("aria-expanded"));
        }
        #endregion

        #endregion
    }
}
