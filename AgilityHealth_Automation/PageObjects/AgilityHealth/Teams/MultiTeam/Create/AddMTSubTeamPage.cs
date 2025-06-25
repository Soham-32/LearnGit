using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.MultiTeam.Create
{
    internal class AddMtSubTeamPage : BasePage
    {
        public AddMtSubTeamPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        private readonly By AddSubTeamButton = By.Id("btnAddSubTeams");
        private readonly By FilterTextbox = By.Id("filterBox");
        private static By SubTeamItem(string item) => By.XPath($"//td[contains(@class,'chkbxAllGrid')][text()='{item}'] | //td[contains(@class,'chkbxAllGrid')]//font[text()='{item}']");

        public void SelectSubTeam(string subTeam)
        {
            Log.Step(nameof(AddMtSubTeamPage), $"On Add Sub-Team page, select {subTeam}");
            Wait.UntilElementClickable(FilterTextbox).SetText(subTeam);
            Wait.UntilJavaScriptReady();
            Wait.UntilElementClickable(SubTeamItem(subTeam)).Click();
        }

        public void ClickAddSubTeamButton()
        {
            Log.Step(nameof(AddMtSubTeamPage), "Click Add Team Member button");
            Wait.UntilElementClickable(AddSubTeamButton).Click();
        }
        public bool IsSubTeamPresent(string subTeam)
        {
            Log.Step(nameof(AddMtSubTeamPage), $"Is {subTeam} Present ?");
            return Driver.IsElementPresent(SubTeamItem(subTeam));
        }
    }
}