using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.Utilities;
using AtCommon.Dtos.Teams;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Edit
{
    internal class EditRevieweePopup : BasePage
    {
        public EditRevieweePopup(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        private readonly By ClosePopupButton =
            By.XPath("//span[@id = 'edit_assessment_dialog_wnd_title']/following-sibling::div/a");
        private readonly By AddAdditionalReviewerButton = By.LinkText("Add Additional Reviewer");
        private readonly By FirstNameTextBox = By.Id("FirstName");
        private readonly By LastNameTextBox = By.Id("LastName");
        private readonly By EmailTextBox = By.Id("Email");
        private readonly By ReviewerUpdateButton = By.Id("update_0");
        private readonly By MemberExistsPopup = By.Id("member_exists_dialog");
        private readonly By MemberExistsUpdateButton = By.Id("update_member");


        public void AddAdditionalReviewer(AddMemberRequest reviewer)
        {
            Log.Step(nameof(EditRevieweePopup), $"Add additional reviewer with email {reviewer.Email}");
            Wait.UntilJavaScriptReady();
            Wait.UntilElementClickable(AddAdditionalReviewerButton).Click();
            Wait.UntilJavaScriptReady();

            Wait.UntilElementClickable(FirstNameTextBox).SetText(reviewer.FirstName);
            Wait.UntilElementClickable(LastNameTextBox).SetText(reviewer.LastName);
            Wait.UntilElementClickable(EmailTextBox).SetText(reviewer.Email);
            
            Wait.UntilElementClickable(ReviewerUpdateButton).Click();
            Wait.UntilJavaScriptReady();

            if (!Driver.IsElementDisplayed(MemberExistsPopup)) return;
            Wait.UntilElementClickable(MemberExistsUpdateButton).Click();
            Wait.UntilJavaScriptReady();
        }

        internal void ClosePopup()
        {
            Log.Step(nameof(EditRevieweePopup), "Click the 'Close' button on the popup");
            Driver.SwitchTo().ParentFrame();
            Wait.UntilElementClickable(ClosePopupButton).Click();
            Wait.UntilJavaScriptReady();
        }
    }
}
