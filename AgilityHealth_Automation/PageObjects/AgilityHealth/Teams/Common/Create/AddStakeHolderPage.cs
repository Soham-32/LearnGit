using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Common.Members;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Common.Create
{

    public class AddStakeHolderPage : StakeHolderCommon
    {

        public AddStakeHolderPage(IWebDriver driver, ILogger log) : base(driver, log) { }

        
        private readonly By ContinueToStakeHolderButton = By.Id("add_stakeholder");
        

        public void ClickReviewAndFinishButton()
        {
            Log.Step(nameof(AddStakeHolderPage), "On Add Stakeholder page, click 'Review and Finish' button");
            Wait.UntilElementClickable(ContinueToStakeHolderButton).Click();
        }

    }
}