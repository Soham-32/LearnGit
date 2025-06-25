using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;
using System.Collections.Generic;
using System.Linq;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Common
{
    internal class TopNavigation : BasePage
    {

        public TopNavigation(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        #region Elements

        #region UserDetails And Dropdown
        private readonly By ProfileIcon = By.CssSelector("div.navbar-user-avatar");
        private readonly By NameAndRole = By.Id("userNameAndRole");
        private readonly By SwitchNewNavigation = By.CssSelector("#logoutForm a[href^='/unifiednav?navValue']");
        private readonly By SwitchCompaniesButton = By.CssSelector("#logoutForm a[href^='/usercompany']");
        private readonly By MyProfile = By.XPath("//ul[@id='profileMenu']//li/a[text()='My Profile']");
        private readonly By SignOut = By.CssSelector("#logoutForm li.logout a");
        private readonly By ReportsHeaderText = By.XPath("//h1[text()='How can we help you?']");
        #endregion

        //Key-Customer verification
        #region
        private readonly By LogoutSuccessfulText = By.XPath("//h1[text() = 'Logout Successful']");
        private readonly By AccessYourAccountText = By.XPath("//h1[text() = 'Access Your Account']");
        #endregion

        #region GlobalIcon
        //v1
        private readonly By GlobeIcon = By.Id("globeIcon");
        private readonly By LanguageDropdown = By.Id("languageDropdown");
        private readonly By ListOfLanguages = By.XPath("//select[@id='languageDropdown']/option");
        private static By SelectLanguage(string languageName) => By.XPath($"//select[@id='languageDropdown']/option[text()='{languageName}']");

        //v2
        private readonly By GlobeIconV2 = By.XPath("//*[local-name()='svg' and @data-icon='globe']");

        #endregion

        #region Links
        private readonly By SettingsLink = By.XPath("//*[contains(text(),'Settings')]");
        private readonly By GrowthPortalLink = By.CssSelector("a[href^='/growthportal']");
        private readonly By DashboardLink = By.CssSelector("i.default");
        private readonly By ReportsLink = By.LinkText("REPORTS");
        private readonly By BusinessOutcome = By.LinkText("BUSINESS OUTCOMES");
        private readonly By InsightsDashboardLink = By.CssSelector("a[href^='/V2/insights']");
        private readonly By OpenAssessments = By.Id("openAssessmentLink");
        private readonly By PulseAssessmentsLink = By.CssSelector("a[href='/V2/pulse-assessments']");
        private readonly By SupportCenterLink = By.XPath("//*[contains(text(),'Support Center')]");
        private readonly By GrowthPlanLink = By.XPath("//li/a/span[text()='Growth Plan']");
        #endregion

        #region Open Assessment Pop up
        private readonly By OpenAssessmentsPopUp = By.ClassName("open-assessment");
        private readonly By OpenAssessmentsPopUpTakeAssessmentLink = By.ClassName("takeassessment");
        #endregion

        #region Breadcrumbs

        private readonly By Breadcrumbs = By.CssSelector(".breadcrumbs");

        #endregion

        #endregion

        #region Methods

        #region UserDetails And Dropdown

        public void HoverOnProfileIcon()
        {
            Wait.UntilElementExists(ProfileIcon);
            Driver.MoveToElement(Wait.UntilElementClickable(ProfileIcon));
        }

        public void HoverOnNameRoleSection()
        {
            Log.Step(nameof(TopNavigation), "Hover on Name/Role section");
            Driver.MoveToElement(Wait.UntilElementClickable(NameAndRole));
        }

        public string GetUserNameAndRoleText()
        {
            Log.Step(nameof(TopNavigation), "Get Name/Role text");
            return Wait.UntilElementVisible(NameAndRole).GetText().Replace("\r\n", string.Empty);
        }

        public void ClickOnSwitchNewNavButton()
        {
            Log.Step(nameof(TopNavigation), "Click on 'Switch - New Nav' button");
            if (!Driver.IsElementDisplayed(NameAndRole, 10)) return;
            HoverOnNameRoleSection();
            Wait.UntilElementVisible(SwitchNewNavigation);
            Wait.UntilElementClickable(SwitchNewNavigation).Click();
        }

        public void ClickOnSwitchCompaniesButton()
        {
            Log.Step(nameof(TopNavigation), "Click on Switch Companies");
            HoverOnNameRoleSection();
            Wait.UntilElementVisible(SwitchCompaniesButton);
            Wait.UntilElementClickable(SwitchCompaniesButton).Click();
        }

        public void ClickOnMyProfile()
        {
            Log.Step(nameof(TopNavigation), "Click on my profile");
            CloseDeploymentPopup();
            Wait.UntilElementVisible(ProfileIcon).Click();
            Wait.UntilElementClickable(MyProfile).Click();
        }

        public void ClickOnSignOut()
        {
            Log.Step(nameof(TopNavigation), "Click on signout");
            Wait.UntilElementVisible(SignOut);
            Wait.UntilElementClickable(SignOut).Click();
        }

        public void LogOut()
        {
            Log.Step(nameof(TopNavigation), "Logout");
            Wait.HardWait(5000);// Wait until team dashboard load
            Driver.ExecuteJavaScript("document.getElementById('logoutForm').submit()");
        }

        //Key-Customer verification
        #region
        public bool IsLogoutSuccessfulTextPresent()
        {
            return Driver.IsElementDisplayed(LogoutSuccessfulText);
        }

        public bool IsAccessYourAccountTextPresent()
        {
            return Driver.IsElementDisplayed(AccessYourAccountText);
        }
        #endregion

        public bool IsNameAndRoleSectionDisplayed()
        {
            return Driver.IsElementPresent(NameAndRole) && Wait.UntilElementVisible(NameAndRole).Displayed;
        }

        public void NavigateToMyProfilePage(int companyId)
        {
            NavigateToUrl($"{BaseTest.ApplicationUrl}/account?companyId={companyId}");
        }

        public void NavigateToMyProfilePageForProd(string env, int companyId)
        {
            NavigateToUrl($"https://{env}.agilityinsights.ai/account?companyId={companyId}");
        }

        #endregion

        #region GlobalIcon
        //v1
        public void ClickOnGlobeIcon()
        {
            Log.Step(nameof(TopNavigation), "Click on 'Globe' icon");
            Wait.UntilElementClickable(GlobeIcon).Click();
        }

        public List<string> GetListOfLanguages(bool v2 = false)
        {
            Log.Step(nameof(TopNavigation), "Get list of the languages");
            switch (v2)
            {
                case true when !Driver.IsElementDisplayed(LanguageDropdown):
                    ClickOnGlobeIconV2();
                    break;
                case false when !Driver.IsElementDisplayed(LanguageDropdown):
                    ClickOnGlobeIcon();
                    break;
            }
            var languageList = Driver.GetTextFromAllElements(ListOfLanguages).ToList();
            return languageList;
        }

        public void SelectALanguage(string language, bool v2 = false)
        {
            Log.Step(nameof(TopNavigation), $"Select {language} Language from the Language dropdown");

            switch (v2)
            {
                case true when !Driver.IsElementDisplayed(LanguageDropdown):
                    ClickOnGlobeIconV2();
                    break;
                case false when !Driver.IsElementDisplayed(LanguageDropdown):
                    ClickOnGlobeIcon();
                    break;
            }
            SelectItem(LanguageDropdown, SelectLanguage(language));
        }

        //v2
        public void ClickOnGlobeIconV2()
        {
            Log.Step(nameof(TopNavigation), "Click on 'Globe' icon");
            Wait.UntilElementClickable(GlobeIconV2).Click();
        }

        #endregion

        #region Links

        public void ClickOnSupportCenterLink()
        {
            Log.Step(nameof(TopNavigation), "Click on Support Center link");
            Wait.UntilElementClickable(SupportCenterLink).Click();
            Wait.HardWait(2000);
        }
        public void ClickOnSettingsLink()
        {
            Log.Step(nameof(TopNavigation), "Click on Settings link");
            Wait.HardWait(2000);// need to wait until the page is loaded
            Wait.UntilElementClickable(SettingsLink).Click();
        }

        public void ClickOnGrowthPortalLink()
        {
            Log.Step(nameof(TopNavigation), "Click on Growth Portal link");
            Wait.UntilElementClickable(GrowthPortalLink).Click();
        }

        public void GoToDashboard()
        {
            Log.Step(nameof(TopNavigation), "Go to Dashboard");
            Wait.UntilElementClickable(DashboardLink).Click();
        }

        public void ClickOnReportsLink()
        {
            Log.Step(nameof(TopNavigation), "Click on Reports link");
            Wait.UntilElementClickable(ReportsLink).Click();
        }

        public bool IsReportsLinkPresent()
        {
            return Driver.IsElementPresent(ReportsLink, 1) && Wait.UntilElementExists(ReportsLink).Displayed;
        }

        public void ClickOnBusinessOutComeLink()
        {
            Log.Step(nameof(TopNavigation), "Click on 'Business Outcomes' link");
            Wait.UntilElementClickable(BusinessOutcome).Click();
            Driver.SwitchToLastWindow();
            Driver.Manage().Window.Maximize();
            Wait.UntilJavaScriptReady();
        }

        public bool IsBusinessOutComeLinkDisplayed()
        {
            return Driver.IsElementDisplayed(BusinessOutcome);
        }

        public void ClickOnGrowthPlanLink()
        {
            Log.Step(nameof(TopNavigation), "Click on 'Growth Plan' link");
            Wait.UntilElementClickable(GrowthPlanLink).Click();
            Driver.SwitchToLastWindow();
            Driver.Manage().Window.Maximize();
            Wait.UntilJavaScriptReady();
        }

        public bool IsGrowthPlanLinkDisplayed()
        {
            return Driver.IsElementDisplayed(GrowthPlanLink);
        }

        public void ClickOnOpenAssessmentsLink()
        {
            Wait.UntilElementClickable(OpenAssessments).Click();
            Wait.UntilJavaScriptReady();
        }

        public bool IsOpenAssessmentLinkPresent()
        {
            return Driver.IsElementPresent(OpenAssessments, 1) && Wait.UntilElementExists(OpenAssessments).Displayed;
        }


        public bool DoesGrowthPortalLinkDisplay()
        {
            return Driver.IsElementPresent(GrowthPortalLink, 1) && Wait.UntilElementExists(GrowthPortalLink).Displayed;
        }

        public void ClickOnInsightsDashboardLink()
        {
            Log.Step(nameof(TopNavigation), "Click on Insights Dashboard link");
            Wait.UntilJavaScriptReady();
            Wait.UntilElementClickable(InsightsDashboardLink).Click();
            Driver.SwitchToLastWindow();
            Wait.UntilJavaScriptReady();
        }

        public void ClickOnPulseAssessmentsLink()
        {
            Wait.UntilElementClickable(PulseAssessmentsLink).Click();
            Driver.SwitchToLastWindow();
        }
        public bool IsPulseAssessmentsLinkDisplayed()
        {
            return Driver.IsElementDisplayed(PulseAssessmentsLink);
        }
        public bool IsResourcesLinkDisplayed()
        {
            Wait.HardWait(10000);
            return Driver.IsElementDisplayed(ReportsHeaderText, 10);
        }

        public void NavigateToSupportCenterPage()
        {
            Wait.HardWait(5000);
            Driver.NavigateToPage("https://support.agilityinsights.ai/hc/en-us");
        }
        #endregion

        #region Open Assessment Pop up
        public bool IsOpenAssessmentPopUpPresent()
        {
            return Driver.IsElementPresent(OpenAssessmentsPopUp, 1) && Wait.UntilElementExists(OpenAssessmentsPopUp).Displayed;
        }

        public string GetOpenAssessmentPopUpText()
        {
            return Wait.UntilElementVisible(OpenAssessmentsPopUp).GetText();
        }

        public void ClickOnOpenAssessmentPopUpTakeAssessmentLink()
        {
            Log.Step(nameof(TopNavigation), "Click on Open Assessment Popup, Take Assessment link");
            Wait.UntilElementClickable(OpenAssessmentsPopUpTakeAssessmentLink).Click();
        }
        #endregion

        #region Breadcrumbs

        public string GetBreadcrumbText()
        {
            Wait.UntilJavaScriptReady();
            return Wait.UntilElementVisible(Breadcrumbs).GetText();
        }

        public bool DoesInsightsDashboardLinkDisplay()
        {
            return Driver.IsElementDisplayed(InsightsDashboardLink);
        }
        public bool DoesSupportCenterDashboardLinkDisplay()
        {
            return Driver.IsElementDisplayed(SupportCenterLink);
        }
        #endregion

        #endregion
    }
}