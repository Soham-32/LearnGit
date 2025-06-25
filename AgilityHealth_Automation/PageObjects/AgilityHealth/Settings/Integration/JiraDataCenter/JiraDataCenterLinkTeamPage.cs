using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;
using AtCommon.Dtos.Integrations.Custom.JiraIntegrations.JiraDataCenterIntegration;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.Integration.JiraDataCenter
{
    public class JiraDataCenterLinkTeamPage : LinkTeamBasePage
    {
        public JiraDataCenterLinkTeamPage(IWebDriver driver, ILogger log = null) : base(driver, log)
        {
        }

        //Jira Data Center Credentials         
        private readonly By UserNameTextBox = By.Id("userName");
        private readonly By UserNameValidationText = By.Id("userName-helper-text");
        private readonly By PasswordTextBox = By.Id("passwordName");
        private readonly By PasswordValidationText = By.Id("passwordName-helper-text");

        //Jira Data Center Credentials         
        public string GetUserNameMandatoryValidationMessage()
        {
            Log.Step(nameof(JiraDataCenterLinkTeamPage), "Get the mandatory field validation message for the 'User Name' text field.");
            Wait.UntilElementClickable(UserNameTextBox).SendKeys(Keys.Tab);
            return Wait.UntilElementVisible(UserNameValidationText).GetText();
        }

        public string GetPasswordMandatoryValidationMessage()
        {
            Log.Step(nameof(JiraDataCenterLinkTeamPage), "Get the mandatory field validation message for the 'Password' text field.");
            Wait.UntilElementClickable(PasswordTextBox).SendKeys(Keys.Tab);
            return Wait.UntilElementVisible(PasswordValidationText).GetText();
        }

        public void EnterUserName(string userName)
        {
            Log.Step(nameof(JiraDataCenterLinkTeamPage), "Enter the 'User Name'.");
            Wait.UntilElementVisible(UserNameTextBox).SetText(userName);
        }

        public void EnterPassword(string password)
        {
            Log.Step(nameof(JiraDataCenterLinkTeamPage), "Enter the 'Password'.");
            Wait.UntilElementVisible(PasswordTextBox).SetText(password);
        }

        public void EnterJiraDataCenterCredentials(JiraDataCenterCredential jiraDataCenterCredential)
        {
            Log.Step(nameof(JiraDataCenterLinkTeamPage), "Enter the 'Jira Data Center Credentials'");
            EnterInstanceName(jiraDataCenterCredential.InstanceName);
            EnterUserName(jiraDataCenterCredential.UserName);
            EnterPassword(jiraDataCenterCredential.Password);
            EnterUrl(jiraDataCenterCredential.ServerUrl);
            ClickOnAuthenticationPopupOkButton();
        }
    }
}
