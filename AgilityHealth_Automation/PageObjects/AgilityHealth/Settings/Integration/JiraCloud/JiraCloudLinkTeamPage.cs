using AgilityHealth_Automation.Utilities;
using AtCommon.Dtos.Integrations.Custom.JiraIntegrations.JiraCloudIntegration;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.Integration.JiraCloud
{
    public class JiraCloudLinkTeamPage : LinkTeamBasePage
    {
        public JiraCloudLinkTeamPage(IWebDriver driver, ILogger log = null) : base(driver, log)
        {
        }

        //Jira Cloud Credentials
        private readonly By ApiTokenTextBox = By.Id("apiToken");
        private readonly By ApiTokenValidationText = By.Id("apiToken-helper-text");
        private readonly By EmailTextBox = By.Id("email");
        private readonly By EmailValidationText = By.Id("email-helper-text");

        //Jira Cloud Credentials                
        public string GetApiTokenMandatoryValidationMessage()
        {
            Log.Step(nameof(JiraCloudLinkTeamPage), "Get the mandatory field validation message for 'Api Token' text field.");
            Wait.UntilElementClickable(ApiTokenTextBox).SendKeys(Keys.Tab);
            return Wait.UntilElementVisible(ApiTokenValidationText).GetText();
        }

        public void ClickOnEmailTextBox()
        {
            Log.Step(nameof(JiraCloudLinkTeamPage), "Click on the 'Email' text box.");
            Wait.UntilElementClickable(EmailTextBox).Click();
        }

        public string GetEmailMandatoryValidationMessage()
        {
            Log.Step(nameof(JiraCloudLinkTeamPage), "Get the mandatory field validation message for 'Email' text field.");
            ClickOnEmailTextBox();
            Wait.UntilElementClickable(EmailTextBox).SendKeys(Keys.Tab);
            return Wait.UntilElementVisible(EmailValidationText).GetText();
        }

        public void EnterApiToken(string apiToken)
        {
            Log.Step(nameof(JiraCloudLinkTeamPage), "Enter the api token.");
            Wait.UntilElementVisible(ApiTokenTextBox).SetText(apiToken);
        }

        public void EnterEmail(string email)
        {
            Log.Step(nameof(JiraCloudLinkTeamPage), "Enter the email.");
            Wait.UntilElementVisible(EmailTextBox).SetText(email);
        }

        public void EnterJiraCloudCredentials(JiraCloudCredential jiraCloudCredential)
        {
            Log.Step(nameof(JiraCloudLinkTeamPage), "Enter the 'Jira Cloud Credentials'.");
            EnterInstanceName(jiraCloudCredential.InstanceName);
            EnterApiToken(jiraCloudCredential.ApiToken);
            EnterEmail(jiraCloudCredential.Email);
            EnterUrl(jiraCloudCredential.ServerUrl);
            ClickOnAuthenticationPopupOkButton();
        }
    }
}