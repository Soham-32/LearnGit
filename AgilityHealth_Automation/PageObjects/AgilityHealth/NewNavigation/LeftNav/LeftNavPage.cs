using System.Collections.Generic;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.NewNavigation.LeftNav
{
    public class LeftNavPage : BasePage
    {
        public LeftNavPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        #region Locators
        private readonly By TxtSearchTeam = By.XPath("//div[@id='sidebarWrapper']//input[@placeholder='Search']");
        private readonly By LblHighlightedResult = By.XPath("//span[@style='background-color: yellow;']");
        private readonly By CompanyList = By.XPath("//div[@id='sidebarWrapper']//span[@role='combobox']");
        private static By CompanyListItem(string company) => By.XPath($"//ul[@id='ddlSelectCompany_listbox']/li[text()='{company}']");

        private static By ExpandIcon(string teamName) => By.XPath($"//span[text()='{teamName}']/ancestor::a/preceding-sibling::div//button");
        private static By CollapseIcon(string teamName) =>
            By.XPath($"//span[text()='{teamName}']/ancestor::a/preceding-sibling::div[@aria-expanded='true']//button");

        private static By TeamLabel(string teamName) => By.XPath($"//div[@id='sidebarWrapper']//span[text()='{teamName}']");
        #endregion

        #region Methods
        #region Search
        public void SearchTeam(string search)
        {
            Log.Step(nameof(LeftNavPage), $"On Left Nav, searching for {search}");
            Wait.HardWait(3000); // Wait till page load
            Wait.UntilElementVisible(TxtSearchTeam).Clear();
            Wait.UntilElementVisible(TxtSearchTeam).SetText(search).SendKeys(Keys.Enter);
            Wait.HardWait(5000); // Wait till searched text displayed 
        }

        public IList<string> GetHighlightedResult()
        {
            var lblHighlightedResultElements = Wait.UntilAllElementsLocated(LblHighlightedResult);
            var lblHighlightedValue = new List<string>();

            foreach (var element in lblHighlightedResultElements)
            {
                Driver.JavaScriptScrollToElement(element);
                lblHighlightedValue.Add(Wait.UntilElementVisible(element).GetText());
            }
            return lblHighlightedValue;
        }

        public void ExpandTeam(string teamName)
        {
            Log.Step(nameof(LeftNavPage), $"Expand team ${teamName}");
            Wait.UntilElementVisible(ExpandIcon(teamName)).Click();
            Wait.UntilJavaScriptReady();
        }

        public void CollapseTeam(string teamName)
        {
            Log.Step(nameof(LeftNavPage), $"Collapse team ${teamName}");
            Wait.UntilElementVisible(CollapseIcon(teamName)).Click();
            Wait.UntilJavaScriptReady();
        }

        public void ClickOnATeam(string teamName)
        {
            Log.Step(nameof(LeftNavPage), $"Click on a team name ${teamName}");
            Wait.UntilElementVisible(TeamLabel(teamName)).Click();
            Wait.UntilJavaScriptReady();
        }

        public bool DoesTeamDisplay(string teamName)
        {
            return Driver.IsElementDisplayed(TeamLabel(teamName));
        }
        #endregion

        #region Select company

        public void SelectCompany(string company)
        {
            Log.Step(nameof(LeftNavPage), $"On Left Nav, select a company with name {company}");
            SelectItem(CompanyList, CompanyListItem(company));
        }

        public bool IsCompanyDisabled()
        {
            return Wait.UntilElementVisible(CompanyList).GetAttribute("aria-disabled").Equals("true");
        }
        #endregion
        #endregion
    }
}
