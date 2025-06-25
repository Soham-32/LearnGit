using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.IndividualAssessment.Create;
using AgilityHealth_Automation.Utilities;
using AtCommon.Dtos.Assessments;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.IndividualAssessment.Common
{
    public class AddReviewerModal : BasePage
    {
        public AddReviewerModal(IWebDriver driver, ILogger log) : base(driver, log)
        {

        }

        private readonly By ReviewerModalTitle = By.XPath("//*[@id='form-dialog-title']//h6[contains(normalize-space(),'Add/Remove Reviewers')]");
        private readonly By AddReviewersTextBox = By.XPath("//div[text()='Search by Name or Email']/following-sibling::div//div//input[@type='text']");
        private readonly By AddSelectedReviewerButton = AutomationId.Equals("addSelectedReviewersButton");
        private static By AddReviewersCheckBox(string firstName, string lastName) => AutomationId.Equals($"reviewerCheckbox-Reviewer Name-{firstName}-{lastName}");
        private readonly By CreateNewReviewerIcon = AutomationId.Equals("createNewReviewerBtn");
        private readonly By CreateNewReviewerTitle = By.XPath("//h1[contains(normalize-space(),'Create New Reviewer')]");

        private readonly By NewReviewerEmailTextBox = By.XPath("//input[@id='emailNewParticipantTextField']");
        private readonly By NewReviewerFirstNameTextBox = By.XPath("//input[@id='firstNameNewParticipantTextField']");
        private readonly By NewReviewerLastNameTextBox = By.XPath("//input[@id='lastNameNewParticipantTextField']");
        private readonly By CreateNewReviewerButton = AutomationId.Equals("createParticipantBtn");
        private readonly By MemberExistsTitle = By.XPath("//p[text()='Member Exists']");
        private readonly By UseExistingButton = AutomationId.Equals("createAddAnotherParticipantBtn");
        private readonly By ReviewerRoleDropdown = By.CssSelector("[automation-id='companyRoles'] > div");
        private readonly By SaveButton = AutomationId.Equals("addSelectedReviewersButton");
        private static By CreatedReviewerEmail(string email) => By.XPath($"//div/p[text()='{email}']");
        private static By ReviewerRoleItem(string item) => By.XPath($"//span[@automation-id='companyRoles']//div[contains(@id, 'react-select')][contains(normalize-space(),'{item}')]");

        public void WaitUntilLoaded()
        {
            Log.Step(nameof(AddReviewerModal), "Wait until the modal loads");
            Wait.UntilElementVisible(ReviewerModalTitle);
            Wait.UntilJavaScriptReady();
        }

        public void AddReviewersBySearchingInModal(CreateReviewerRequest reviewer, bool check = true)
        {
            Log.Step(nameof(CreateIndividualAssessment4AddReviewAndPublishPage),
                $"Search for the Reviewer, then check the checkbox next to <{reviewer.FullName()}>");
            WaitUntilLoaded();
            Wait.HardWait(5000);// Wait required to Load search model completely
            Wait.UntilElementClickable(AddReviewersTextBox).SetText(reviewer.FirstName);
            Wait.HardWait(5000); // Wait required once the firstname is entered in the textbox 
            Wait.UntilElementClickable(AddReviewersCheckBox(reviewer.FirstName, reviewer.LastName)).Check(check);
            Wait.UntilElementClickable(AddSelectedReviewerButton).Click();
        }

        public void AddReviewersByScrollingInModal(CreateReviewerRequest reviewer)
        {
            Log.Step(nameof(CreateIndividualAssessment4AddReviewAndPublishPage),
                $"Scroll to the Reviewer, then check the checkbox next to <{reviewer.FullName()}>");
            WaitUntilLoaded();
            Wait.UntilJavaScriptReady();
            Wait.UntilElementClickable(AddSelectedReviewerButton).Click();
        }

        public void CreateNewReviewer(CreateReviewerRequest reviewer, bool setRole = true)
        {
            Log.Step(nameof(CreateIndividualAssessment4AddReviewAndPublishPage),
                "Click the 'Create New Reviewer' button, enter new reviewer's info in the popup, and click the 'Create' button");
            Wait.UntilElementClickable(CreateNewReviewerIcon).Click();
            Wait.UntilElementVisible(CreateNewReviewerTitle);
            Wait.UntilJavaScriptReady();
            Wait.UntilElementClickable(NewReviewerEmailTextBox).SetText(reviewer.Email);
            Wait.UntilElementClickable(NewReviewerFirstNameTextBox).SetText(reviewer.FirstName);
            Wait.UntilElementClickable(NewReviewerLastNameTextBox).SetText(reviewer.LastName);

            if (setRole)
            {
                if (reviewer.RoleTags[0].Tags != null)
                {
                    foreach (var reviewerRole in reviewer.RoleTags[0].Tags)
                    {
                        SelectItem(ReviewerRoleDropdown, ReviewerRoleItem(reviewerRole.Name));
                    }
                }
            }

            Wait.UntilElementClickable(CreateNewReviewerButton).Click();
        }

        public void ClickOnSaveButton(CreateReviewerRequest reviewer)
        {
            Log.Step(nameof(AddReviewerModal), "Click on Save button");
            Wait.UntilElementExists(CreatedReviewerEmail(reviewer.Email));
            Wait.UntilElementClickable(SaveButton).Click();
            Wait.HardWait(2000);// Page is taking time to load data for reviewer.
        }

        public void MemberExistsModal_UpdateMember()
        {
            Log.Step(nameof(CreateIndividualAssessment4AddReviewAndPublishPage),
                "Click the 'Update' button on the Member Exists popup");
            Wait.UntilElementVisible(MemberExistsTitle);
            Wait.UntilJavaScriptReady();
            Wait.UntilElementClickable(CreateNewReviewerButton).Click();
        }

        public void MemberExistsModal_UseExisting()
        {
            Log.Step(nameof(CreateIndividualAssessment4AddReviewAndPublishPage),
                "Click the 'Use Existing' button on the Member Exists popup");
            Wait.UntilElementVisible(MemberExistsTitle);
            Wait.UntilJavaScriptReady();
            Wait.UntilElementClickable(UseExistingButton).Click();
        }

        public bool IsReviewerChecked(string firstname, string lastname)
        {
            Log.Step(nameof(AddReviewerModal), "Verify that the reviewer is still checked in the modal");
            Wait.UntilJavaScriptReady();
            var element = Wait.UntilElementClickable(AddReviewersCheckBox(firstname, lastname));
            return element.GetElementAttribute("class").Contains("Mui-checked");
        }
    }
}
