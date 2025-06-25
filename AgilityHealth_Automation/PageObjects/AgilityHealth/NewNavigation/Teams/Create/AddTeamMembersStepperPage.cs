using AgilityHealth_Automation.PageObjects.AgilityHealth.NewNavigation.Teams.Base;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.NewNavigation.Teams.Create
{
    public class AddTeamMembersStepperPage : TeamMembersBasePage
    {
        public AddTeamMembersStepperPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }
        #region Locators

        #region Team Member stepper
        // 'Continue' button
        private readonly By ContinueToStakeholderButton = By.XPath("//div[contains(@class,'wizard-setup')]//a[text()='Continue to Stakeholders ']");
        #endregion

        #region Leader Stepper
        private readonly By ContinueToReviewButton = By.XPath("//div[@class='contents']//following-sibling::a[normalize-space()='Continue to Review']");
        #endregion

        #endregion


        #region Methods

        #region Team Member stepper
        // 'Continue' button
        public void ClickOnContinueToStakeholderButton()
        {
            Log.Step(nameof(TeamMembersBasePage), "Click on 'Continue To Stakeholder' button");
            Wait.UntilElementEnabled(ContinueToStakeholderButton);
            Wait.UntilJavaScriptReady();
            Driver.JavaScriptScrollToElement(ContinueToStakeholderButton);
            Wait.HardWait(1000);//Wait till 'Continue To Stakeholder' button is displayed
            Wait.UntilElementClickable(ContinueToStakeholderButton).Click();
        }
        #endregion

        #region Stakeholder/Leader Stepper
        public void ClickOnContinueToReviewButton()
        {
            Log.Step(nameof(StakeholdersBasePage), "Click on 'Continue To Review' button");
            Wait.UntilElementVisible(ContinueToReviewButton);
            Wait.HardWait(1000);//Wait till 'Continue To Review' button is displayed  
            Driver.JavaScriptScrollToElement(ContinueToReviewButton);
            Wait.UntilElementClickable(ContinueToReviewButton).Click();
            Wait.UntilJavaScriptReady();
            Wait.HardWait(1000); //Need to wait until popup get closed
        }
    }
        #endregion

        #endregion
}