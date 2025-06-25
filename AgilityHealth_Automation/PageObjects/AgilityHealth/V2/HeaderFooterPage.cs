using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.V2
{
    public class HeaderFooterPage : BasePage
    {
        public HeaderFooterPage(IWebDriver driver, ILogger log) : base(driver, log) { }


        //Top Section
        private readonly By GrowthPortalLink = By.CssSelector("button[label='Growth Portal']");
        private readonly By ReportsLink = By.CssSelector("button[label='Reports']");
        private readonly By SupportCenterLink = By.CssSelector("button[label='Support Center']");
        private readonly By SettingsLink = By.CssSelector("button[label='Settings']");
        private readonly By BusinessOutcome= By.CssSelector("button[label='Business Outcomes']");
        private readonly By InsightsButton= By.CssSelector("button[label='Insights']");
        private readonly By PulseAssessmentsButton = By.CssSelector("button[label='Assessments']");
        private readonly By GrowthPlanLink = By.CssSelector("button[label='Growth Plan']");
        private readonly By StopImpersonatingLink = By.XPath("//li/a/span[text()='Stop Impersonating']");

        //Top section
        public void ClickOnGrowthPortalLink()
        {
            Log.Step(nameof(HeaderFooterPage), "Click on 'Growth Portal' button");
            Wait.UntilElementClickable(GrowthPortalLink).Click();
            Wait.UntilJavaScriptReady();
        }

        public void ClickOnReportsLink()
        {
            Log.Step(nameof(HeaderFooterPage), "Click on 'Reports' button");
            Wait.UntilElementClickable(ReportsLink).Click();
            Wait.UntilJavaScriptReady();
        }

        public void ClickOnSupportCenterLink()
        {
            Log.Step(nameof(HeaderFooterPage), "Click on 'Support Center' button");
            Wait.UntilElementClickable(SupportCenterLink).Click();
            Wait.UntilJavaScriptReady();
        }

        public void ClickOnSettingsLink()
        {
            Log.Step(nameof(HeaderFooterPage), "Click on 'Settings' button");
            Wait.UntilElementClickable(SettingsLink).Click();
            Wait.UntilJavaScriptReady();
        }

        public void ClickOnBusinessOutComeLink()
        {
            Log.Step(nameof(HeaderFooterPage), "Click on 'Business Outcomes' button");
            Wait.UntilElementClickable(BusinessOutcome).Click();
            Wait.UntilJavaScriptReady();
        }

        public void ClickInsightsButton()
        {
            Log.Step(nameof(HeaderFooterPage), "Click on 'Insights' button");
            Wait.UntilElementClickable(InsightsButton).Click();
        }

        public void ClickAssessmentsButton()
        {
            Log.Step(nameof(HeaderFooterPage), "Click on 'Assessments' button");
            Wait.UntilElementClickable(PulseAssessmentsButton).Click();
        }
        public bool IsPulseAssessmentButtonIsDisplayed()
        {
            Log.Step(nameof(HeaderFooterPage), "Is 'Pulse Assessments' button displayed?");
            return Driver.IsElementDisplayed(PulseAssessmentsButton);
        }

        public void SignOut()
        {
            Log.Step(nameof(HeaderFooterPage), "Sign out of the application");
            Driver.ExecuteJavaScript("document.getElementById('logoutForm').submit()");
        }

        public bool IsGrowthPlanLinkPresent()
        {
            Log.Step(nameof(HeaderFooterPage),"Is Growth Plan link present ?");
            return Driver.IsElementDisplayed(GrowthPlanLink,10);
        }

        public void ClickOnGrowthPlanLink()
        {
            Log.Step(nameof(HeaderFooterPage), "Click on Growth Plan Link");
            Wait.UntilElementClickable(GrowthPlanLink).Click();
        }

        public bool DoesInsightsButtonDisplay()
        {
            return Driver.IsElementDisplayed(InsightsButton);
        }

        public void ClickOnStopImpersonateLink()
        {
            Log.Step(nameof(HeaderFooterPage), "Click on Stop Impersonating Link");
            Wait.UntilElementVisible(StopImpersonatingLink).Click();
        }
    }
}
