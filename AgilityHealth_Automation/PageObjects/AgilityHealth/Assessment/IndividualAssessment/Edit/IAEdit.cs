using System;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.IndividualAssessment.Common;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos.IndividualAssessments;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.IndividualAssessment.Edit
{
    public class IaEdit : IndividualAssessmentBase
    {
        public IaEdit(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        //V2 - New Assessment Header
        private readonly By TakeAssessmentButton = By.XPath("//div//button[text()='TAKE ASSESSMENT']");
        private readonly By TakeAssessmentTakenTimeTitle = By.XPath("//div//button[text()='TAKE ASSESSMENT']/..");
        private readonly By ViewAssessmentResultsButton = By.XPath("//div//button[text()='View Assessment Results']");

        //V2 - New Assessment Section
        private readonly By AssessmentTitle = By.TagName("h1");
        private readonly By IaAssessmentType = By.Id("typeName");
        private readonly By IaStartDate = By.Id("startDate");
        private readonly By IaPointOfContact = By.Id("pointOfContact");
        private readonly By IaName = By.Id("name");
        private readonly By IaEndDate = By.Id("endDate");
        private readonly By IaPointOfContactEmail = By.Id("pointOfContactEmail");
        private static By ReviewerAccessLinkIcon(string email) => By.XPath($"//p[text()='{email}']//ancestor::div[contains(@class, 'MuiGrid-root')]//div//button[@automation-id='linkBtn']");
        private readonly By AddReviewerButton = AutomationId.Equals("addReviewerBtn");
        private readonly By ReviewerRole = AutomationId.Equals("reviewerRole");
        private static By ReviewerEmail(string email) => By.XPath($"//div[@automation-id='reviewerEmail']//p[contains(normalize-space(),'{email}')]");
        private static By ReviewerRoles(string email) => By.XPath($"//div[@automation-id='reviewerEmail']//p[contains(normalize-space(),'{email}')]/ancestor::div[@automation-id='reviewerEmail']/following-sibling::div[@automation-id='reviewerRole']//p");
        private static By EditReviewerButton(string email) => By.XPath($"//div[@automation-id='reviewerEmail']//p[contains(normalize-space(),'{email}')]/ancestor::div[@automation-id='reviewerEmail']/following-sibling::div//button[@automation-id='reviewerEditPencil']");
        //V2 - New Reviewers Section
        private static By CheckMarkIcon(string email) => By.XPath($"//div[@automation-id='reviewerEmail']//p[text()='{email}']/ancestor::div[contains(@class,'MuiGrid-container')]//div[@aria-label='Completed']");
        private static By LockIcon(string email) => By.XPath($"//p[text()='{email}']//ancestor::div[contains(@class, 'MuiGrid-root')]//div//button[@automation-id='openLockBtn']");

        public void ClickTakeAssessment()
        {
            Log.Step(nameof(IaEdit), "Click 'Take Assessment' button");
            Wait.UntilElementClickable(TakeAssessmentButton).Click();
        }

        public void ClickViewAssessmentResults()
        {
            Log.Step(nameof(IaEdit), "Click 'View Assessment Results' button");
            Wait.UntilElementClickable(ViewAssessmentResultsButton).Click();
        }

        public bool IsTakeAssessmentButtonEnabled()
        {
            Wait.UntilJavaScriptReady();
            return Wait.UntilElementExists(TakeAssessmentButton).Enabled;
        }

        public string TakeAssessmentTakenTitle()
        {
            return Wait.UntilElementVisible(TakeAssessmentTakenTimeTitle).GetAttribute("aria-label");
        }

        public string GetAssessmentType()
        {
            return Wait.UntilElementVisible(IaAssessmentType).GetText();
        }

        public string GetStartDate()
        {
            return Wait.UntilElementVisible(IaStartDate).GetText();
        }

        public string GetEndDate()
        {
            return Wait.UntilElementVisible(IaEndDate).GetText();
        }

        public string GetPoc()
        {
            return Wait.UntilElementVisible(IaPointOfContact).GetText();
        }

        public string GetPocEmail()
        {
            return Wait.UntilElementVisible(IaPointOfContactEmail).GetText();
        }

        public string GetAssessmentName()
        {
            return Wait.UntilElementVisible(IaName).GetText();
        }

        public void WaitForAssessmentTitle(string title)
        {
            Log.Step(nameof(IaEdit), $"Wait for assessment title to be {title}");
            Wait.UntilTextToBePresent(Wait.UntilElementVisible(AssessmentTitle), title);
        }
        public string GetAssessmentTitle()
        {
            Log.Step(nameof(IaEdit), "Get assessment title");
            return Wait.UntilElementVisible(AssessmentTitle).GetText();
        }

        public void ClickOnReviewerAccessLinkIcon(string email)
        {
            Log.Step(nameof(IaEdit), "Click 'Reviewer Access Link' button");
            Wait.UntilElementClickable(ReviewerAccessLinkIcon(email)).Click();
        }

        public void ClickOnEditReviewer(string email)
        {
            Log.Step(nameof(IaEdit), "Click 'Edit Reviewer' button");
            Wait.UntilElementClickable(EditReviewerButton(email)).Click();
        }
        public string GetRolesOfReviewer(string email)
        {
            Log.Step(nameof(IaEdit), "Get reviewer's role");
            Wait.HardWait(2000); //It take time to get reviewer roles
            return Wait.UntilElementVisible(ReviewerRoles(email)).GetText();
        }

        public void ClickAddReviewerButton()
        {
            Log.Step(nameof(IaEdit), "Click 'Add Reviewer' button");
            Wait.UntilElementVisible(AddReviewerButton).Click();
        }

        public CreateIndividualAssessmentRequest GetAssessmentInfo()
        {
            var request = new CreateIndividualAssessmentRequest()
            {
                PointOfContact = GetPoc(),
                PointOfContactEmail = GetPocEmail(),
                AssessmentName = GetAssessmentName()
            };
            var start = GetStartDate();
            var end = GetEndDate();

            //removing timezone, so it is is parseable 
            request.Start = DateTime.Parse(start.Remove(start.IndexOf("m", StringComparison.Ordinal) + 1));
            request.End = DateTime.Parse(end.Remove(end.IndexOf("m", StringComparison.Ordinal) + 1));

            return request;
        }

        public string GetCompletedTitle(string email)
        {
            return !Driver.IsElementDisplayed(CheckMarkIcon(email)) ? "" : Wait.UntilElementVisible(CheckMarkIcon(email)).GetAttribute("aria-label");
        }

        public string GetTypeOfLockIcon(string email)
        {
            return Wait.UntilElementExists(LockIcon(email)).GetAttribute("automation-id");
        }

        public bool DoesReviewerExist(string email)
        {
            return Driver.IsElementDisplayed(ReviewerEmail(email));
        }

        public bool DoesReviewerRoleExist(string email)
        {
            return Driver.IsElementDisplayed(ReviewerRoles(email));
        }

        public bool DoesAccessLinkIconDisplay(string email)
        {
            return Driver.IsElementDisplayed(ReviewerAccessLinkIcon(email));
        }

        public bool DoesCheckMarkIconDisplay(string email)
        {
            return Driver.IsElementDisplayed(CheckMarkIcon(email));
        }

        public bool DoesLockIconDisplay(string email)
        {
            return Driver.IsElementDisplayed(LockIcon(email));
        }

        public string GetReviewerRole()
        {
            Log.Step(nameof(IaEdit), "Get the selected role from the individual assessment edit page");
            return Wait.UntilElementVisible(ReviewerRole).GetText();
        }

        public void WaitForPageLoad()
        {
            Log.Step(nameof(IaEdit), "Wait for individual edit page to load");
            Wait.UntilElementExists(ReviewerRole);
        }
        public void NavigateToPage(int companyId, Guid teamUid, Guid assessmentUid, string teamId = null)
        {
            var url =
                $"{BaseTest.ApplicationUrl}/v2/edit-assessments/company/{companyId}/team/{teamUid}/assessment/{assessmentUid}";
            if (teamId != null)
                url = url.AddQueryParameter("teamId", teamId);
            NavigateToUrl(url);
            Wait.UntilElementVisible(AssessmentTitle);
            Wait.HardWait(2000); // Need to wait till page navigate to the IA page
        }
    }
}