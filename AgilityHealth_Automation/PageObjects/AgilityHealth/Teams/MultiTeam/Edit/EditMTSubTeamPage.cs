using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.MultiTeam.Edit
{
    internal class EditMTSubTeamPage : BasePage
    {


        public EditMTSubTeamPage(IWebDriver driver, ILogger log) : base(driver, log) { }

        private readonly By UpdateSubTeamButton = By.Id("btnAddSubTeams");
        private readonly By SubTeamCheckBoxes = By.XPath("//div[@id='SubAllteamsGrid']//td[@class='chkbxAllGrid']");
        private static By SubTeamCheckBox(string subTeamName) => By.XPath(
            $"//input[@class='chkbxAllGrid subteams']/../following-sibling::td[@class='chkbxAllGrid'][text()='{subTeamName}']");

        private readonly By SelectedTeamCheckBoxes =
            By.XPath("//div[@id='SubSelectedteamsGrid']//input[@class='chkbxAllGrid subteams']");
        
        public void RemoveSubTeam(string subTeamName) => Wait.UntilElementClickable(SubTeamCheckBox(subTeamName)).Click();

        public void AddRandomSubTeam(int numberOfSubTeam)
        {
            Log.Step(nameof(EditMTSubTeamPage), $"On Edit Team page, Sub-Teams tab, add {numberOfSubTeam} random sub-team");
            var elements = Wait.UntilAllElementsLocated(SubTeamCheckBoxes);

            for (var i = 0; i < numberOfSubTeam; i++)
            {
                elements[0].Click();
                elements = Wait.UntilAllElementsLocated(SubTeamCheckBoxes);
            }
        }

        public void ClickUpdateSubTeamButton()
        {
            Log.Step(nameof(EditMTSubTeamPage), "On Edit Team page, Sub-Teams tab, click Update Sub-Teams button");
            Wait.UntilElementClickable(UpdateSubTeamButton).Click();
        }

        public int TotalSelectedSubTeam() => Driver.GetElementCount(SelectedTeamCheckBoxes);
    }
}
