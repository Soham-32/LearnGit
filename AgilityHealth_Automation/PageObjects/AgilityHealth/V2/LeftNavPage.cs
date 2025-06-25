using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.V2
{
    public class LeftNavPage : BasePage
    {
        public LeftNavPage(IWebDriver driver, ILogger log) : base(driver, log) { }

        //Tree Section
        private readonly By CompanyDropdown = By.XPath("//div[@id='CompanySelect']//select");
        private readonly By CompanyList = By.XPath("*//div[@id='CompanySelect']//select//option");
        private static By DynamicTeamLink(string teamName) => By.XPath($"//div[@title='{teamName}']//parent::a");
        private static By DynamicTeamExpandButton(string teamName) => By.XPath($"//div[@title='{teamName}']/../../following-sibling::div//button");
        private readonly By AllTeamNames = By.CssSelector(".sidebar__list a div");
        private static By ChildTeams(string parentTeam) => By.XPath($"//div[@title = '{parentTeam}']//ancestor::li[1]//li//a/div");
        private readonly By TeamList = By.CssSelector(".company__list");
        private static By SubTeamsOfParentTeam(string parentTeam) => By.XPath($"//*[@id = 'sidebarWrapper']//div[text() = '{parentTeam}']/../following-sibling::div//ul");
        private static By SpecificParentTeam(string parentTeam) =>
            By.XPath($"//*[@id = 'sidebarWrapper']//div[text() = '{parentTeam}'] | //*[@id = 'sidebarWrapper']//div//font[text() = '{parentTeam}']");
        private readonly By CompanyItem = By.XPath("(//div[contains(@class, 'MuiListItem-secondaryAction')]/a)[1]");
        private readonly By FirstTeamItem = By.XPath("(//div[contains(@class, 'MuiListItem-secondaryAction')]/a)[2]");


        public void WaitUntilLoaded(int timeOut = 60)
        {
            new SeleniumWait(Driver, timeOut).UntilElementVisible(TeamList);
        }

        //Tree Section
        public void SelectCompany(string companyName)
        {
            Log.Step(nameof(LeftNavPage), $"Select company <{companyName}> in the dropdown");
            Wait.UntilJavaScriptReady();
            Wait.UntilElementClickable(CompanyDropdown).SelectDropdownValueByVisibleText(companyName);
            Wait.UntilJavaScriptReady();
            Wait.UntilElementClickable(CompanyDropdown); //Dropdown field disappears on selecting value and displayed again, so waiting for it to be clickable again
        }

        public List<string> GetCompanyList()
        {
            Log.Step(nameof(LeftNavPage), "Get company name list from v2 left navigation dropdown");
            return Wait.UntilAllElementsLocated(CompanyList).Select(company => company.GetText()).ToList();
        }
        public void ClickOnTeamName(string teamName)
        {
            Log.Step(nameof(LeftNavPage), $"Click on team name <{teamName}>");
            Wait.UntilElementExists(DynamicTeamLink(teamName)).Click();
        }

        public bool IsTeamDisabled(string teamName)
        {
            Log.Step(nameof(LeftNavPage), $"Verify That, <{teamName}> Is Disabled on left nav?");
            return Wait.UntilElementVisible(DynamicTeamLink(teamName)).GetAttribute("style").Contains("pointer-events: none; color: rgb(170, 170, 170);");
        }

        public void ScrollToTeam(string teamName)
        {
            Log.Step(nameof(LeftNavPage), $"Scroll to team - {teamName} ");
            Driver.JavaScriptScrollToElement(DynamicTeamLink(teamName));
        }

        public void ClickOnTeamExpandButton(string teamName)
        {
            Log.Step(nameof(LeftNavPage), $"Expand {teamName} to view child team");
            Driver.JavaScriptScrollToElement(SpecificParentTeam(teamName), false);
            Wait.UntilElementExists(DynamicTeamExpandButton(teamName)).Click();
            Wait.UntilJavaScriptReady();
        }

        public List<string> GetAllTeamVisibleNames()
        {
            Log.Step(nameof(LeftNavPage), "Get all the teams from the left navigation that are visible");
            return Wait.UntilAllElementsLocated(AllTeamNames).Select(e => e.GetText().Trim()).ToList();
        }

        public void ScrollToParentTeamHeadingToViewSubteams(string parentTeam)
        {
            Wait.UntilJavaScriptReady();
            Log.Step(nameof(LeftNavPage), $"Scroll to view all {parentTeam} teams");
            Driver.JavaScriptScrollToElement(SubTeamsOfParentTeam(parentTeam));
            Wait.UntilJavaScriptReady();
        }

        public List<string> GetTeamNamesOfSpecificParent(string parent)
        {
            Log.Step(nameof(LeftNavPage), $"Get all the teams under {parent} parent header from the left navigation that are visible");
            return Wait.UntilAllElementsLocated(ChildTeams(parent)).Select(e => e.GetText().Trim()).ToList();
        }

        public IList<string> GetChildTeamName(string parentTeam)
        {
            var elements = Wait.UntilAllElementsLocated(ChildTeams(parentTeam));
            return elements.Select(team => team.GetText()).ToList();
        }

        public bool IsFirstTeamItemSelected()
        {
            return Wait.UntilElementVisible(FirstTeamItem).GetAttribute("class").Contains("active");
        }

        public bool IsCompanyItemSelected()
        {
            return Wait.UntilElementVisible(CompanyItem).GetAttribute("class").Contains("active");
        }

        public bool IsCompanyItemDisabled()
        {
            return Wait.UntilElementVisible(CompanyItem).GetAttribute("style").Contains("pointer-events: none");
        }
    }
}
