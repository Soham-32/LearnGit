using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Account
{
    internal class PasswordResetPage : BasePage
    {
        public PasswordResetPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        // Locators
        private readonly By PasswordTextbox1 = By.Id("Password");
        private readonly By PasswordTextbox2 = By.Id("ConfirmPassword");
        private readonly By ResetPasswordButton = By.XPath("//input[@value = 'Reset Password']");


        // Methods
        public void SubmitPassword(string newPassword)
        {
            Wait.UntilElementClickable(PasswordTextbox1).SetText(newPassword);
            Wait.UntilElementClickable(PasswordTextbox2).SetText(newPassword);
            Wait.UntilElementClickable(ResetPasswordButton).Click();
        }



    }
}