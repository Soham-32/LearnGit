using System.Collections.Generic;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.ManageUsers
{
    internal class UserSearchPopup : BasePage
    {
        public UserSearchPopup(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        // Locators
        private readonly By SearchTextbox = By.Id("txtSearchAllUserspopup");
        private readonly By GoButton = By.Id("btnGoSearchUserpopup");
        private readonly By CloseButton = By.Id("btnGoSearchUserClose");
        private static By CellsByColumnName(string columnName) => By.XPath(
            $"//*[@id = 'SearchUserGrid']//table/tbody/tr/td[count(//*[@id='SearchUserGrid']//thead//th[@data-title='{columnName}']/preceding-sibling::th)+1]");
        private static By ResetPasswordButton(string email) =>
            By.XPath($"//*[@id = 'SearchUserGrid']//td[text() = '{email.ToLower()}']/..//input[@class='resetPasswordBtn' and @style = 'visibility: visible'] | //*[@id = 'SearchUserGrid']//font[contains(normalize-space(),'{email.ToLower()}')]/ancestor::td/..//input[contains(@class,'resetPasswordBtn') and @style = 'visibility: visible']");
        protected readonly By SendingEmailPopupTitle = By.Id("email_sending_dialog_wnd_title");

        // Methods
        public void Search(string searchTerm)
        {
            Wait.UntilJavaScriptReady();
            Wait.UntilElementClickable(SearchTextbox).SetText(searchTerm);
            Wait.UntilElementClickable(GoButton).Click();
            Wait.UntilJavaScriptReady();
        }

        public void ClickCloseButton()
        {
            Wait.UntilElementVisible(CloseButton).Click();
            Wait.UntilElementInvisible(CloseButton);
        }

        public IList<string> GetColumnValues(string columnName)
        {
            return Driver.GetTextFromAllElements(CellsByColumnName(columnName));
        }

        public void ClickResetPasswordButton(string email)
        {
            Wait.UntilElementClickable(ResetPasswordButton(email)).Click();
            Wait.UntilElementVisible(SendingEmailPopupTitle);
            Wait.UntilElementInvisible(SendingEmailPopupTitle);
        }

        public bool IsResetPasswordButtonPresent(string email)
        {
            return Driver.IsElementDisplayed(ResetPasswordButton(email));
        }

    }
}