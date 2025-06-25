using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Survey
{
    internal class PersonalAssessmentLinkPage : BasePage
    {
        public PersonalAssessmentLinkPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        // Locators
        private readonly By EmailTextbox = By.Id("emailId");
        private readonly By SubmitButton = By.Id("submit");

        // Email Sent Popup for founded email
        private readonly By CheckingAssessmentPopup = By.Id("checkingAssessmentWindow");
        private readonly By EmailFoundPopupCloseButton = By.Id("closeFound");
        private readonly By EmailFoundPopupMessage = By.Id("contactFoundMessage");

        // Email Sent Popup for not founded email
        private readonly By EmailNotFoundPopupCloseButton = By.Id("closeNotFound");
        private readonly By EmailNotFoundPopupMessage = By.Id("contactNotFoundMessage");

        // Getting validation message 
        private readonly By ValidationMessageForInvalidEmail = By.ClassName("enterValidEmlMsg");
        private readonly By ValidationMessageForBlankEmail = By.ClassName("needEmailAddr");

        // Methods
        public void EnterEmail(string emailAddress)
        {
            Log.Step(nameof(PersonalAssessmentLinkPage), "Enter email address and click the 'Submit' button");
            Wait.UntilElementClickable(EmailTextbox).SetText(emailAddress);
            Wait.UntilElementClickable(SubmitButton).Click();
            Wait.UntilElementInvisible(CheckingAssessmentPopup);
        }

        // Email Found Popup
        public void ClickCloseButtonOnEmailFoundPopup()
        {
            Log.Step(nameof(PersonalAssessmentLinkPage), "Click the 'Close' button on the Email Found Popup");
            Wait.UntilElementClickable(EmailFoundPopupCloseButton).Click();
            Wait.UntilElementInvisible(EmailFoundPopupCloseButton);
        }

        public string GetEmailFoundPopupText()
        {
            Log.Step(nameof(PersonalAssessmentLinkPage), "Get email found popup message");
            return Wait.UntilElementVisible(EmailFoundPopupMessage).GetText();
        }

        // Email Not Found Popup 
        public void ClickCloseButtonOnEmailSentPopupNotFound()
        {
            Log.Step(nameof(PersonalAssessmentLinkPage), "Click the 'Close' button on the Email Sent Popup Not Found");
            Wait.UntilElementClickable(EmailNotFoundPopupCloseButton).Click();
            Wait.UntilElementInvisible(EmailNotFoundPopupCloseButton);
        }

        public string GetEmailNotFoundPopupText()
        {
            Log.Step(nameof(PersonalAssessmentLinkPage), "Get email not found popup message for not found");
            return Wait.UntilElementVisible(EmailNotFoundPopupMessage).GetText();
        }

        // Getting validation message 
        public string GetValidationMessageForInvalidEmailText()
        {
            Log.Step(nameof(PersonalAssessmentLinkPage), "Get error validation message for invalid email");
            return Wait.UntilElementVisible(ValidationMessageForInvalidEmail).GetText();
        }
        public string GetValidationMessageForBlankEmailText()
        {
            Log.Step(nameof(PersonalAssessmentLinkPage), "Get error validation message for blank email");
            return Wait.UntilElementVisible(ValidationMessageForBlankEmail).GetText();
        }
    }
}