using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.Utilities;
using AtCommon.Dtos.Companies;
using AtCommon.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Company.Base
{
    internal class SecurityBasePage : BasePage
    {
        public SecurityBasePage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        // Locators

        protected readonly By SessionTimeout = AutomationId.Equals("security-session-timeout", "input");
        protected readonly By MaxSessionLengthTextbox = AutomationId.Equals("max-session-length", "input");
        protected readonly By ForcePasswordUpdateTextbox = AutomationId.Equals("force-password-update", "input");
        protected readonly By RequireSecurityQuestionsCheckbox = By.Name("requireSecurityQuestions");
        protected readonly By TwoFactorCompanyAdminCheckbox = By.Name("twoFactorCompanyAdmin");
        protected readonly By TwoFactorOrgLeaderCheckbox = By.Name("twoFactorOrgLeader");
        protected readonly By TwoFactorBlAdminCheckbox = By.Name("twoFactorBuAdmin");
        protected readonly By TwoFactorTeamAdminCheckbox = By.Name("twoFactorTeamAdmin");
        protected readonly By SignOutUrlTextbox = AutomationId.Equals("sign-out-url", "input");
        protected readonly By AuditTrailRetentionPeriodTextbox =
            AutomationId.Equals("audit-trail-retention-period", "input");

        // Methods

        public void WaitUntilLoaded()
        {
            Wait.UntilElementVisible(SessionTimeout);
        }


        public void FillSecurityInfo(AddCompanyRequest company, bool maxSession = true)
        {
            Log.Step(GetType().Name, "Fill in Security Info");
            Wait.UntilElementClickable(SessionTimeout).SetText(company.SessionTimeout.ToString(), isReact:true);
            if (maxSession) // Due to Bug:38622 adding condition, once bug resolved we will remove this condition
            {
                Wait.UntilElementClickable(MaxSessionLengthTextbox).SetText(company.MaxSessionLength.ToString(), isReact: true);
            }
            Wait.UntilElementClickable(ForcePasswordUpdateTextbox).SetText(company.ForcePasswordUpdate.ToString(), isReact:true);
            Wait.UntilElementEnabled(RequireSecurityQuestionsCheckbox).Check(company.RequireSecurityQuestions);
            Wait.UntilElementEnabled(TwoFactorCompanyAdminCheckbox).Check(company.TwoFactorCompanyAdmin);
            Wait.UntilElementEnabled(TwoFactorOrgLeaderCheckbox).Check(company.TwoFactorOrgLeader);
            Wait.UntilElementEnabled(TwoFactorBlAdminCheckbox).Check(company.TwoFactorBuAdmin);
            Wait.UntilElementEnabled(TwoFactorTeamAdminCheckbox).Check(company.TwoFactorTeamAdmin);
            Wait.UntilElementClickable(SignOutUrlTextbox).SetText(company.LogoutUrl, isReact:true);
            Wait.UntilElementClickable(AuditTrailRetentionPeriodTextbox)
                .SetText(company.AuditTrailRetentionPeriod.ToString(), isReact:true);
        }


        public AddCompanyRequest GetSecurityInfo()
        {
            return new AddCompanyRequest
            {
                SessionTimeout = Wait.UntilElementClickable(SessionTimeout).GetText().ToInt(),
                MaxSessionLength = Wait.UntilElementClickable(MaxSessionLengthTextbox).GetText().ToInt(),
                ForcePasswordUpdate = Wait.UntilElementClickable(ForcePasswordUpdateTextbox).GetText().ToInt(),
                RequireSecurityQuestions = Wait.UntilElementExists(RequireSecurityQuestionsCheckbox).Selected,
                TwoFactorCompanyAdmin = Wait.UntilElementExists(TwoFactorCompanyAdminCheckbox).Selected,
                TwoFactorOrgLeader = Wait.UntilElementExists(TwoFactorOrgLeaderCheckbox).Selected,
                TwoFactorBuAdmin = Wait.UntilElementExists(TwoFactorBlAdminCheckbox).Selected,
                TwoFactorTeamAdmin = Wait.UntilElementExists(TwoFactorTeamAdminCheckbox).Selected,
                LogoutUrl = Wait.UntilElementClickable(SignOutUrlTextbox).GetText(),
                AuditTrailRetentionPeriod = Wait.UntilElementClickable(AuditTrailRetentionPeriodTextbox).GetText().ToInt()
            };
        }

    }
}
