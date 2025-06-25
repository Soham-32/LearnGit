using System;
using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.IndividualAssessment.Edit;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos.Assessments;
using AtCommon.Dtos.IndividualAssessments;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.IndividualAssessment.Common
{
    public class IndividualAssessmentBase : BasePage
    {
        public IndividualAssessmentBase(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        private readonly By PreviewAssessmentLink = AutomationId.Equals("previewRadarButton");
        private readonly By ConfirmationToaster = By.XPath("//div//h6[contains(text(),'email has been sent')]");
        private readonly By ConfirmDeleteDeleteButton = AutomationId.Equals("confirmDeleteDialogDeleteBtn");
        private readonly By PopupDeleteButton = AutomationId.Equals("confirmDeleteDialogDeleteBtn");
        private readonly By PopupCancelButton = AutomationId.Equals("confirmDeleteDialogCancelBtn");

        //Reviewer base
        private static By ReviewerAccessLinkButton(string email) => By.XPath($"//p[text()='{email}']//ancestor::div[contains(@class, 'MuiCollapse-entered')]//div//button[@automation-id='linkBtn']");
        private readonly By AccessLinkTooltip = By.XPath("//*[@role = 'presentation']//div//p");

        private readonly By LockIcon =
            By.XPath("//p[@automation-id='reviewerRoles']/../following-sibling::div//button/following-sibling::button");
        private static By ReviewerEmail(string email) => By.XPath($"//div[@automation-id='reviewerEmail']//p[contains(normalize-space(),'{email}')]");
        private static By EditReviewerButton(string email) => By.XPath($"//div[@automation-id='reviewerEmail']//p[contains(normalize-space(),'{email}')]//ancestor::div[@id='panelReviewersDetail']//button[@automation-id='reviewerEditPencil']");

        private static By AddReviewerButton(string email) => By.XPath(
            $"//*[@automation-id = 'participantExpansionPanelDetail-{email}']//button[@automation-id='participantAddReviewerBtn']");
        private static By CheckMarkIcon(string email) => By.XPath($"//div[@automation-id='reviewerEmail']//p[text()='{email}']/ancestor::div[contains(@class,'MuiGrid-container')]//div[@aria-label='Completed']");
        private readonly By ReviewerRoleDropdown = By.CssSelector("[automation-id='companyRoles'] > div");
        private static By ReviewerRoleItem(string item) => By.XPath($"//span[@automation-id='companyRoles']//div[contains(@id, 'react-select')][contains(normalize-space(),'{item}')]");
        private static By ReviewerNameAndEmail(string name, string email) => By.XPath($"//div[@automation-id='reviewerName' and normalize-space(.)='{name}']/following-sibling::div[@automation-id='reviewerEmail']//p[text()='{email}']");
        private static By ReviewerInReviewerScreen(string name, string email, string role) => By.XPath($"//div[@automation-id='reviewerName' and normalize-space(.)='{name}']/following-sibling::div[@automation-id='reviewerEmail']//p[text()='{email}']//ancestor::div[@id='panelReviewersDetail']//div//p[text()='{role}']");
        private static By DoesReviewerDisplayCreateIaAddReviewerList(string name, string email, string role) => By.XPath($"//div[@automation-id='reviewerName' and normalize-space(.)='{name}']/following-sibling::div[@automation-id='reviewerEmail']//p[text()='{email}']/ancestor::div[@id='panelReviewersDetail']//div//p[text()='{role}']");
        private static By DoesReviewerDisplayCreateIaPublishReviewerList(string name, string email, string role) => By.XPath($"//div[@automation-id='reviewerName' and normalize-space(.)='{name}']/following-sibling::div[@automation-id='reviewerEmail']//p[contains(normalize-space(),'{email}')]//ancestor::div[@id='panelReviewersDetail']//div//p[contains(normalize-space(),'{role}')]");
        private static By RemoveReviewerButton(string email) => By.XPath($"//div[@automation-id='reviewerEmail']//p[text()='{email}']//ancestor::div[@id='panelReviewersDetail']//button[@automation-id='removeReviewerBtn']");
        private static By RemoveRoleByName(string role) => By.XPath($"//span[@automation-id='companyRoles']//div[text()='{role}']//following-sibling::div");

        //Participant base
        private readonly By AddAdditionalParticipantsIcon = AutomationId.Equals("addParticipantsBtn");
        private readonly By NewParticipantEmail = By.CssSelector("span[automation-id='emailNewParticipantTextField']>div");
        private readonly By NewParticipantEmailItem = By.XPath("//input[@id='react-select-2-input']");
        private readonly By CreateParticipantButton = AutomationId.Equals("createParticipantBtn");
        private readonly By NewParticipantFirstName = By.Id("firstNameNewParticipantTextField");
        private readonly By NewParticipantLastName = By.Id("lastNameNewParticipantTextField");
        private readonly By ParticipantReopenAssessmentIcon = AutomationId.Equals("participantOpenLockBtn");
        private readonly By DialogOkButton = By.XPath("//div[@role='dialog']//div/button[text()='Ok']");
        private readonly By ResendAssessmentIcon = AutomationId.Equals("sendReviewerEmailBtn");
        private readonly By ParticipantResendAssessmentIcon = AutomationId.Equals("sendParticipantEmailBtn");
        private readonly By ReopenAssessmentIcon = AutomationId.Equals("openLockBtn");
        private readonly By SendReminderButton = AutomationId.Equals("emailParticipantsButton");
        private readonly By UploadExcelInput = By.CssSelector("button[automation-id='uploadExcelFileButton'] input");
        private readonly By UploadingIndicator = By.CssSelector("span[role='progressbar']");

        private readonly By DownLoadExcelFileButton = AutomationId.Equals("downloadExcelFileButton");
        private readonly By DownLoadUploadErrorFileButton = AutomationId.Equals("btnExportExcel");
        private readonly By ParticipantGroupDropdown = By.CssSelector("[automation-id='participantGroups'] > div");

        private static By ParticipantEmail(string email) => By.XPath($"//div[@automation-id='participantEmail']//p[contains(normalize-space(),'{email}')]");
        private static By ParticipantNameWithEmail(string email) => By.XPath($"//div[@automation-id='participantExpansionPanelSummary-{email}']//div[@automation-id='participantName']");
        private static By ParticipantNameAndEmail(string name, string email) => By.XPath($"//div[@automation-id='participantExpansionPanelSummary-{email}']//div[@automation-id='participantName' and p[contains(normalize-space(.),'{name}')]]");
        private static By ParticipantNameEmailParticipantGroups(string name, string email, string participantGroups) => By.XPath($"//div[@automation-id='participantExpansionPanelSummary-{email}']//div[@automation-id='participantName' and p[contains(normalize-space(.),'{name}')]]/following-sibling::div[@automation-id='participantGroup']//p[text()='{participantGroups}']");
        private static By RemoveParticipantButton(string email) => By.XPath($"//div[@automation-id='participantExpansionPanelSummary-{email}']//button[@automation-id='removeParticipantBtn']");
        private static By ParticipantAccessLinkButton(string email) => By.XPath($"//div[@automation-id='participantExpansionPanelSummary-{email}']//div//button[@automation-id='linkBtn']");
        private static By ParticipantGroup(string email) => By.XPath($"//div[@automation-id='participantExpansionPanelSummary-{email}']//div[@automation-id='participantGroup']/p");
        private static By ParticipantGroupByName(string email, string participantGroup) => By.XPath($"//*[text()='{email}']//ancestor::div//following-sibling::div//*[text()='{participantGroup}']");
        private static By RemoveParticipantGroups(string participantGroup) => By.XPath($"//span[@automation-id='participantGroups']//div[text()='{participantGroup}']//following-sibling::div");

        private static By EditParticipantButton(string email) => By.XPath($"//div[@automation-id='participantExpansionPanelSummary-{email}']//button[@automation-id='participantEditPencil']");
        private static By ParticipantCheckMarkIcon(string email) => By.XPath($"//div[@automation-id='participantEmail'][text()='{email}']/..//div[@title='Completed']");
        private static By ParticipantGroupItem(string item) => By.XPath($"//span[@automation-id='participantGroups']//div[contains(@id, 'react-select')][contains(normalize-space(),'{item}')]");
        private readonly By ParticipantReviewersHeaderExpandCollapse = AutomationId.Equals("toggleExpandAllParticipants");

        //Update participant/reviewer dialog
        private static readonly By EmailTextBox = By.Id("UpdateMemberEmailTextField");
        private static readonly By FirstNameTextBox = By.Id("UpdateMemberFirstNameTextField");
        private static readonly By LastNameTextBox = By.Id("UpdateMemberLastNameTextField");
        private static readonly By SaveButton = AutomationId.Equals("saveMemberBtn");

        public void NavigateToPage(int companyId, Guid teamUid, string teamId = null)
        {
            var url = $"{BaseTest.ApplicationUrl}/v2/individual-assessments/company/{companyId}/team/{teamUid}";
            if (teamId != null)
                url = url.AddQueryParameter("teamId", teamId);
            NavigateToUrl(url);
            Wait.HardWait(2000); //Wait untill NavigateToPage completed
        }

        public void ExpandCollapseParticipantsAndReviewersWithReviewerEmail(string email)
        {
            Log.Step(GetType().Name, "Expand the participant to reveal the reviewer(s) and the add reviewer button");
            Wait.UntilElementClickable(ParticipantReviewersHeaderExpandCollapse).Click();
            Wait.UntilElementVisible(ReviewerEmail(email));
        }

        public void ExpandCollapseParticipantsAndReviewers()
        {
            Log.Step(GetType().Name, "Expand the participant without a reviewer to reveal the add reviewer button");
            Wait.UntilElementClickable(ParticipantReviewersHeaderExpandCollapse).Click();
        }

        public void ClickOnPreviewAssessment()
        {
            Log.Step(GetType().Name, "Click on Preview Assessment link");
            Wait.UntilElementVisible(PreviewAssessmentLink).Click();
        }

        public string GetToasterMessage()
        {
            return Wait.UntilElementVisible(ConfirmationToaster).GetText();
        }

        //Reviewer base
        public bool DoesReviewerDisplay(string email)
        {
            Wait.UntilElementClickable(ParticipantReviewersHeaderExpandCollapse).Click();
            return Driver.IsElementDisplayed(ReviewerEmail(email));
        }

        public void ClickOnReviewerAccessLinkButton(string email)
        {
            Log.Step(GetType().Name, "Click 'Reviewer Access Link' button");
            Wait.UntilElementClickable(ReviewerAccessLinkButton(email)).Click();
        }

        public void EditReviewer(string reviewerEmail)
        {
            Log.Step(GetType().Name,
                $"Click the 'Edit' button for <{reviewerEmail}>");
            Driver.MoveToElement(ReviewerEmail(reviewerEmail));
            Wait.UntilElementClickable(EditReviewerButton(reviewerEmail)).Click();
        }

        public string GetAccessLinkTooltip()
        {
            return Wait.UntilElementVisible(AccessLinkTooltip).GetText();
        }

        public bool DoesAccessLinkButtonDisplay(string email)
        {
            return Driver.IsElementDisplayed(ReviewerAccessLinkButton(email));
        }

        public void ClickAddReviewer(string participantEmail)
        {
            Log.Step(GetType().Name,
                $"Click the 'Add Reviewer' button for <{participantEmail}>");
            Wait.UntilElementClickable(AddReviewerButton(participantEmail)).Click();
        }

        public string GetTypeOfLockIcon()
        {
            return Wait.UntilElementExists(LockIcon).GetAttribute("automation-id");
        }

        public bool DoesCompleteCheckMarkDisplay(string email)
        {
            return Driver.IsElementDisplayed(CheckMarkIcon(email));
        }

        public void WaitUntilCompleteMarkDisappear(string email)
        {
            Log.Step(GetType().Name, "Wait until complete mark disappeared");
            Wait.UntilElementNotExist(CheckMarkIcon(email));
        }

        public bool DoesReviewerDisplay(string name, string email)
        {
            return Driver.IsElementDisplayed(ReviewerNameAndEmail(name, email));
        }

        public bool DoesReviewerDisplayReviewerScreen(string name, string email, List<TagRoleResponse> roles)
        {
            return Driver.IsElementDisplayed(ReviewerInReviewerScreen(name, email, string.Join(", ", roles.Select(role => role.Name).ToArray())));
        }

        public bool DoesReviewerDisplayReviewerScreen(string name, string email, string roles)
        {
            return Driver.IsElementDisplayed(ReviewerInReviewerScreen(name, email, roles));
        }

        public bool DoesReviewerDisplayCreateIaAddReviewerList(string name, string email, List<TagRoleResponse> roles)
        {
            return Driver.IsElementDisplayed(DoesReviewerDisplayCreateIaAddReviewerList(name, email, string.Join(", ", roles.Select(role => role.Name).ToArray())));
        }

        public bool DoesReviewerDisplayCreateIaPublishReviewerList(string name, string email, List<TagRoleResponse> roles)
        {
            return Driver.IsElementDisplayed(DoesReviewerDisplayCreateIaPublishReviewerList(name, email, string.Join(", ", roles.Select(role => role.Name).ToArray())));
        }

        public void DeleteReviewer(string reviewerEmail)
        {
            Log.Step(GetType().Name, $"Click the 'Delete' button for <{reviewerEmail}>");

            var emailElement = Driver.FindElement(ReviewerEmail(reviewerEmail));
            ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].scrollIntoView({block: 'center'});", emailElement);
            Wait.UntilElementClickable(RemoveReviewerButton(reviewerEmail)).Click();
        }

        //Participant base
        public void CreateNewParticipant()
        {
            Log.Step(GetType().Name,
                "Click the 'Add additional Participants' button");
            Wait.UntilElementClickable(AddAdditionalParticipantsIcon).Click();
            Wait.UntilJavaScriptReady();
        }

        public void SelectNewParticipant(string email)
        {
            Log.Step(GetType().Name,
                $"Select new participant <{email}> in the dropdown");
            Wait.UntilElementClickable(NewParticipantEmail).Click();
            var element = Wait.UntilElementClickable(NewParticipantEmailItem);
            new Actions(Driver).MoveToElement(element).Click().SendKeys(element, email).Build().Perform();
            Wait.HardWait(1000);
            new Actions(Driver).SendKeys(Keys.Enter).Build().Perform();
        }

        public void ClickCreateParticipantButton()
        {
            Log.Step(GetType().Name,
                "Click the 'Select' button on Select New Participant popup");
            Wait.UntilElementClickable(CreateParticipantButton).Click();
        }

        public void ClickOnParticipantAccessLinkButton(string email)
        {
            Log.Step(GetType().Name, "Click 'Participant Access Link' button");
            Wait.UntilElementClickable(ParticipantAccessLinkButton(email)).Click();
        }

        public bool DoesParticipantDisplay(string email)
        {
            return Driver.IsElementDisplayed(ParticipantEmail(email));
        }

        public bool DoesParticipantDisplay(string name, string email)
        {
            return Driver.IsElementDisplayed(ParticipantNameAndEmail(name, email));
        }

        public bool DoesParticipantDisplay(string name, string email, List<string> participantGroups)
        {
            return Driver.IsElementDisplayed(ParticipantNameEmailParticipantGroups(name, email, string.Join(", ", participantGroups.ToArray())));
        }

        public void DeleteParticipant(string participantEmail)
        {
            Log.Step(GetType().Name,
                $"Click the 'Delete' button for <{participantEmail}>");
            Driver.JavaScriptScrollToElement(ParticipantEmail(participantEmail));
            Wait.UntilElementClickable(RemoveParticipantButton(participantEmail)).Click();
        }

        public void EditParticipant(string participantEmail)
        {
            Log.Step(GetType().Name,
                $"Click the 'Edit' button for <{participantEmail}>");
            Wait.UntilElementClickable(EditParticipantButton(participantEmail)).Click();
        }

        public string GetParticipantGroup(string email)
        {
            Log.Step(nameof(IaEdit), "Get participant group");
            return Wait.UntilElementVisible(ParticipantGroup(email)).GetText();
        }

        public bool IsParticipantGroupDisplayed(string email, string participantGroup)
        {
            return Driver.IsElementDisplayed(ParticipantGroupByName(email, participantGroup));
        }

        public void RemoveParticipantGroup(string participantGroup)
        {
            Log.Step(GetType().Name, "Remove Participant group");
            Wait.UntilElementClickable(RemoveParticipantGroups(participantGroup)).Click();
            Wait.UntilElementClickable(SaveButton).Click();
            Wait.UntilElementNotExist(SaveButton);
        }

        public void UpdateMember(string email, string firstName, string lastName, List<TagRoleResponse> roles = null)
        {
            Log.Step(GetType().Name, "Update member");
            Wait.UntilElementVisible(EmailTextBox).SetText(email, true, true);
            Wait.UntilElementVisible(FirstNameTextBox).SetText(firstName, true, true);
            Wait.UntilElementVisible(LastNameTextBox).SetText(lastName, true, true);
            if (roles != null)
            {
                foreach (var reviewerRole in roles)
                {
                    SelectItem(ReviewerRoleDropdown, ReviewerRoleItem(reviewerRole.Name));
                }
            }

            Wait.UntilElementClickable(SaveButton).Click();
            Wait.UntilElementNotExist(SaveButton);
        }

        public void UpdateParticipantGroup(List<string> participantGroups)
        {
            Log.Step(GetType().Name, "Update participant group");
            foreach (var participantGroup in participantGroups)
            {
                SelectItem(ParticipantGroupDropdown, ParticipantGroupItem(participantGroup));
            }

            Wait.UntilElementClickable(SaveButton).Click();
            Wait.UntilElementNotExist(SaveButton);
            Wait.HardWait(3000); // Need to wait till the participant group is saved completely.
        }

        public void EditRoles(List<string> roles)
        {
            Log.Step(GetType().Name, "Edit reviewer role");
            if (roles != null)
            {
                foreach (var reviewerRole in roles)
                {
                    SelectItem(ReviewerRoleDropdown, ReviewerRoleItem(reviewerRole));
                }
            }
            Wait.UntilElementClickable(SaveButton).Click();
            Wait.UntilElementNotExist(SaveButton);
            Wait.HardWait(5000);// Page is taking time to load data for reviewer.
        }
        public void RemoveRoleFromReviewer(string role)
        {
            Log.Step(GetType().Name, "Remove reviewer role");
            Wait.UntilElementClickable(RemoveRoleByName(role)).Click();
            Wait.UntilElementClickable(SaveButton).Click();
            Wait.UntilElementNotExist(SaveButton);
        }
        public bool DoesRoleOfReviewerDisplay(string role)
        {
            return Driver.IsElementDisplayed(RemoveRoleByName(role));
        }

        public void ReopenParticipantAssessment()
        {
            Log.Step(GetType().Name, "Click Participant - 'Reopen Assessment' button");
            Wait.UntilElementClickable(ParticipantReopenAssessmentIcon).Click();
            Wait.UntilElementClickable(DialogOkButton).Click();
        }

        public void ResendAssessment()
        {
            Log.Step(GetType().Name, "Click 'Resend Assessment' button");
            Wait.UntilElementClickable(ResendAssessmentIcon).Click();
            Wait.UntilElementClickable(DialogOkButton).Click();
        }

        public void ResendParticipantAssessment()
        {
            Log.Step(GetType().Name, "Click Participant - 'Resend Assessment' button");
            Wait.UntilElementClickable(ParticipantResendAssessmentIcon).Click();
            Wait.UntilElementClickable(DialogOkButton).Click();
        }

        public void ReopenAssessment()
        {
            Log.Step(GetType().Name, "Click 'Reopen Assessment' button");
            Wait.UntilElementClickable(ReopenAssessmentIcon).Click();
            Wait.UntilElementClickable(DialogOkButton).Click();
        }

        public void SendReminder()
        {
            Log.Step(GetType().Name, "Click on Send Reminder");
            Wait.UntilElementClickable(SendReminderButton).Click();
            Wait.UntilElementClickable(DialogOkButton).Click();
        }

        public bool DoesReopenAssessmentIconDisplay()
        {
            return Driver.IsElementDisplayed(ReopenAssessmentIcon);
        }

        public bool DoesParticipantReopenAssessmentIconDisplay()
        {
            return Driver.IsElementDisplayed(ParticipantReopenAssessmentIcon);
        }

        public void WaitUntilParticipantCompleteMarkDisappear(string email)
        {
            Log.Step(GetType().Name, "Wait until participant complete mark disappeared");
            Wait.UntilElementNotExist(ParticipantCheckMarkIcon(email));
        }

        public bool DoesResendIconDisplay()
        {
            return Driver.IsElementDisplayed(ResendAssessmentIcon);
        }

        public bool DoesParticipantCompleteCheckMarkDisplay(string email)
        {
            return Driver.IsElementDisplayed(ParticipantCheckMarkIcon(email));
        }

        public void AcceptDeleting()
        {
            Log.Step(GetType().Name, "Accept deleting");
            Wait.UntilElementClickable(ConfirmDeleteDeleteButton).Click();
        }

        public void WaitUntilAddReviewerButtonShows(string email)
        {
            Log.Step(GetType().Name, "Wait for Add Reviewers '+' icon to show after expanding participant");
            Wait.UntilElementClickable(AddReviewerButton(email));
        }

        public string GetParticipantNameByEmail(string email)
        {
            return Wait.UntilElementVisible(ParticipantNameWithEmail(email)).GetText();
        }

        public void CreateNewParticipant(IndividualAssessmentMemberRequest request, string participantGroups)
        {
            Log.Step(GetType().Name,
                $"Create new participant with email {request.Email}");
            SelectNewParticipant(request.Email);
            var firstNameElement = Wait.UntilElementClickable(NewParticipantFirstName);
            new Actions(Driver).MoveToElement(firstNameElement).SendKeys(firstNameElement, request.FirstName).Build().Perform();
            var lastNameElement = Wait.UntilElementClickable(NewParticipantLastName);
            new Actions(Driver).MoveToElement(lastNameElement).SendKeys(lastNameElement, request.LastName).Build().Perform();
            var participantGroup = Wait.UntilElementClickable(ParticipantGroupDropdown);
            new Actions(Driver).MoveToElement(participantGroup).SendKeys(participantGroup, participantGroups + Keys.Enter).Build().Perform();
            Wait.UntilElementClickable(SaveButton).Click();
            Wait.HardWait(2000);// Need to wait till get the roles
        }

        public void ImportExcelFile(string filePath)
        {
            Log.Step(GetType().Name,
                $"Import an excel file from {filePath}");
            Wait.UntilElementExists(UploadExcelInput).SendKeys(filePath);
            Wait.UntilElementNotExist(UploadingIndicator);
        }

        public void ClickOnDownloadExcelTemplate()
        {
            Log.Step(GetType().Name,
                "Click on Download Excel Template");
            Wait.UntilElementClickable(DownLoadExcelFileButton).Click();
        }

        public void ClickOnDownloadUploadError()
        {
            Log.Step(GetType().Name,
                "Click on Download Upload Error file");
            Wait.UntilElementClickable(DownLoadUploadErrorFileButton).Click();
        }

        public void ClickPopupDeleteButton()
        {
            Log.Step(GetType().Name, "Click the 'OK' button on the popup");
            Wait.UntilElementClickable(PopupDeleteButton).Click();
        }

        public void ClickPopupCancelButton()
        {
            Log.Step(GetType().Name, "Click the 'Cancel' button on the popup");
            Wait.UntilElementClickable(PopupCancelButton).Click();
        }
    }
}
