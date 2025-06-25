using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;
using System.Collections.Generic;
using AtCommon.Dtos.Assessments.Team.Custom.QuickLaunch;
using System.Linq;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Survey.QuickLaunch
{
    internal class QuickLaunchMemberAccessPage : BasePage
    {
        public QuickLaunchMemberAccessPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        //header
        public readonly By AssessmentAccessPageTitleText = By.TagName("h1");
        public readonly By AssessmentAccessPageDescriptionList = By.XPath("//div[@class='create-team-container row']/div/form/preceding-sibling::div");

        //Form
        private readonly By FirstNameTextbox = By.Id("FirstName");
        private readonly By LastNameTextbox = By.Id("LastName");
        private readonly By CompanyEmailTextbox = By.Id("EmailAddress");
        private readonly By RolesDropDown = By.XPath("//ul[@id='SelectedRoles_taglist']/following-sibling::input");
        private static By MemberRole(string role) => By.XPath($"//ul[@id='SelectedRoles_listbox']//li[text()='{role}']");
        private readonly By ParticipantGroupsDropDown = By.XPath("//ul[@id='Categories_1__SelectedTagInts_taglist']/following-sibling::input");
        private readonly By SelectedRoleList = By.XPath("//ul[@id='SelectedRoles_taglist']/li/span[not(@class)]");
        private static By RemoveRoleXIcon(string role) => By.XPath($"//ul[@id='SelectedRoles_taglist']//span[contains(text(),'{role}')]/following-sibling::span/span");
        private readonly By SelectedParticipantGroupList = By.XPath("//ul[@id='Categories_1__SelectedTagInts_taglist']//span[not(@class)]");
        private static By RemoveParticipantGroupXIcon(string participantGroup) => By.XPath($"//ul[@id='Categories_1__SelectedTagInts_taglist']//span[contains(text(),'{participantGroup}')]/following-sibling::span//span");

        private static By MemberParticipantGroup(string participantGroup) => By.XPath($"//ul[@id='Categories_1__SelectedTagInts_listbox']//li[text()='{participantGroup}']");
        private readonly By SubmitButton = By.XPath("//input[@value='Submit']");
        private readonly By UserSuccessfullyAddedToTheTeamMessage = By.XPath("//div[contains(text(),'been added to the team!')]");

        //Tooltip
        private static By TooltipText(string fieldName) => By.XPath($"//label[contains(text(), '{fieldName}')]//following-sibling::img");

        //Validation
        private static By ValidationMessageText(string fieldName) => By.XPath($"//input[@id='{fieldName}']/following-sibling::span");
        private readonly By RoleValidationMessageText = By.XPath("//span[contains(text(), 'The Role field is required.')]");

        //Header
        public string GetAssessmentAccessPageTitleText()
        {
            Log.Step(nameof(QuickLaunchMemberAccessPage), "Get 'Assessment Access' page title text");
            return Wait.UntilElementVisible(AssessmentAccessPageTitleText).GetText();
        }
        public List<string> GetAssessmentAccessPageDescriptionList()
        {
            Log.Step(nameof(QuickLaunchMemberAccessPage), "Get the list of 'Assessment Access' page description");
            return Driver.GetTextFromAllElements(AssessmentAccessPageDescriptionList).ToList();
        }
        public bool IsSubmitButtonDisplayed()
        {
            return Wait.UntilElementVisible(SubmitButton).Displayed;
        }

        //Form
        public void EnterQuickLaunchAssessmentAccessInfo(QuickLaunchMemberAccess quickLaunchAssessmentAccess)
        {
            Log.Step(nameof(QuickLaunchMemberAccessPage), "Enter Quick Launch assessment access detail");
            EnterFirstName(quickLaunchAssessmentAccess.FirstName);
            EnterLastName(quickLaunchAssessmentAccess.LastName);
            EnterCompanyEmail(quickLaunchAssessmentAccess.Email);

            if (!Driver.IsElementDisplayed(RolesDropDown)) return;
            SelectRole(quickLaunchAssessmentAccess.Roles);
            if (!string.IsNullOrEmpty(quickLaunchAssessmentAccess.ParticipantGroups.ToString()))
                SelectParticipantGroup(quickLaunchAssessmentAccess.ParticipantGroups);
            Wait.UntilJavaScriptReady();
        }

        public void EnterFirstName(string firstName)
        {
            Log.Step(nameof(QuickLaunchMemberAccessPage), "Enter 'FirstName' in textbox");
            Wait.UntilElementEnabled(FirstNameTextbox).SetText(firstName);
        }
        public void EnterLastName(string lastName)
        {
            Log.Step(nameof(QuickLaunchMemberAccessPage), "Enter 'LastName' in textbox");
            Wait.UntilElementEnabled(LastNameTextbox).SetText(lastName);
        }
        public void EnterCompanyEmail(string email)
        {
            Log.Step(nameof(QuickLaunchMemberAccessPage), "Enter 'CompanyEmail' in textbox");
            Wait.UntilElementEnabled(CompanyEmailTextbox).SetText(email);
        }
        public void SelectRole(List<string> roleList)
        {
            Log.Step(nameof(QuickLaunchMemberAccessPage), $"Select Reviewer role <{roleList}>from the dropdown");
            foreach (var role in roleList)
            {
                SelectItem(RolesDropDown, MemberRole(role));
            }
        }
        public void SelectParticipantGroup(List<string> participantGroupList)
        {
            Log.Step(nameof(QuickLaunchMemberAccessPage), $"Select Reviewer role <{participantGroupList}> from the dropdown");
            foreach (var participantGroup in participantGroupList)
            {
                SelectItem(ParticipantGroupsDropDown, MemberParticipantGroup(participantGroup));
            }
        }

        public void RemoveRoleAndParticipantGroup(List<string> roles, List<string> participantGroups)
        {
            Log.Step(nameof(QuickLaunchMemberAccessPage), $"Remove Reviewer role <{roles}> and Participant Group <{participantGroups}> in the dropdown");
            foreach (var role in roles)
            {
                Wait.UntilElementClickable(RemoveRoleXIcon(role)).Click();
            }
            foreach (var participantGroup in participantGroups)
            {
                Wait.UntilElementClickable(RemoveParticipantGroupXIcon(participantGroup)).Click();
            }
        }
        public List<string> GetSelectedRoleList()
        {
            return Wait.UntilAllElementsLocated(SelectedRoleList).Select(e => e.GetText()).ToList();
        }
        public List<string> GetSelectedParticipantGroupList()
        {
            return Wait.UntilAllElementsLocated(SelectedParticipantGroupList).Select(e => e.GetText()).ToList();
        }

        public void ClickOnSubmitButton()
        {
            Log.Step(nameof(QuickLaunchMemberAccessPage), "Click on 'Submit' Button");
            Driver.JavaScriptScrollToElement(SubmitButton);
            Wait.UntilElementClickable(SubmitButton).Click();
        }

        public string GetUserSuccessfullyAddedToTheTeamMessage()
        {
            Log.Step(nameof(QuickLaunchMemberAccessPage), "Get 'Team Member Successfully Added To The Team' message");
            return Wait.UntilElementVisible(UserSuccessfullyAddedToTheTeamMessage).GetText();
        }

        //Tooltip
        public string GetTooltipMessage(string fieldName)
        {
            Log.Step(nameof(QuickLaunchMemberAccessPage), "Hover over the 'Role and Participant Groups' tooltip and get tooltip message");
            return Driver.MoveToElement(Wait.UntilElementVisible(TooltipText(fieldName))).GetAttribute("title");
        }

        //Validation
        public string GetValidationMessageText(string fieldName)
        {
            return Wait.UntilElementVisible(ValidationMessageText(fieldName)).GetText();
        }
        public string GetRoleValidationMessageText()
        {
            return Wait.UntilElementVisible(RoleValidationMessageText).GetText();
        }
    }
}
