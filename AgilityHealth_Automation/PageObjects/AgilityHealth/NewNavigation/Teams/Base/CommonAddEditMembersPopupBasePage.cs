using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;
using AgilityHealth_Automation.DataObjects.NewNavigation.Teams;
using System.Collections.Generic;
using System.Threading;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.NewNavigation.Teams.Base
{
    public class CommonAddEditMembersPopupBasePage : TeamBasePage
    {

        public CommonAddEditMembersPopupBasePage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        #region Locators

        #region Add New Team Member/Stakeholders/Leaders popup
        private readonly By FirstNameTextBox = By.Id("FirstName");
        private readonly By LastNameTextBox = By.Id("LastName");
        private readonly By EmailTextBox = By.Id("Email");
        private readonly By RoleListBox = By.XPath("//label[normalize-space()='Role' or @for='Role']/following-sibling::div/div");
        private readonly By ParticipantGroupListBox = By.XPath("//label[normalize-space()='Participant Group']/following-sibling::div");
        private readonly By CreateAndAddNewButton = By.Id("btnSaveAndNew");
        private readonly By CreateAndCloseButton = By.XPath("//a[text()='Create & Close']");
        private readonly By UpdateButton = By.XPath("//a[text()='Update']");
        private static By RoleListItem(string role) => By.XPath($"//ul[ @id='Role_listbox' or @id='Categories_0__SelectedTags_listbox']/li[text()='{role}']");
        private static By SelectedRoleRemoveIcon(string roleItem) => By.XPath($"//label[normalize-space()='Role' or @for='Role']/following-sibling::div//li/span[text()='{roleItem}']//following-sibling::span");
        private static By ParticipantGroupListItem(string participantGroup) => By.XPath($"//ul[@id='ParticipantGroup_listbox' or @id='Categories_3__SelectedTags_listbox' or @id='Categories_1__SelectedTags_listbox']/li[text()='{participantGroup}']");
        private static By SelectedParticipantGroupRemoveIcon(string participantGroupItem) => By.XPath($"//label[normalize-space()='Participant Group']/following-sibling::div//li/span[text()='{participantGroupItem}']//following-sibling::span");

        //Validation
        private static By FieldValidationMessage(string fieldName) => By.XPath($"//div[@id='{fieldName}_validationMessage']");
        private readonly By EmailExistValidationMessage = By.CssSelector(".field-validation-error[data-valmsg-for='Email']>li");
        #endregion

        #endregion




        #region Methods

        #region Add New Team Member/Stakeholders/Leaders popup

        public void ClickOnFirstName()
        {
            Log.Step(nameof(CommonAddEditMembersPopupBasePage), "Click on 'First Name' label");
            Wait.UntilElementClickable(FirstNameTextBox).Click();
        }

        #region Add Data

        public void EnterTeamMemberOrLeadersInfo(Member teamMemberStakeholderInfo, bool removeRoleParticipantGroup = false, List<string> selectedRoleList = null, List<string> selectedParticipantGroupList = null)
        {
            Log.Step(nameof(CommonAddEditMembersPopupBasePage), "Enter Team member/Stakeholder info");
            EnterFirstName(teamMemberStakeholderInfo.FirstName);
            EnterLastName(teamMemberStakeholderInfo.LastName);
            EnterEmail(teamMemberStakeholderInfo.Email);

            if (!Driver.IsElementDisplayed(RoleListBox)) return;
            if (!teamMemberStakeholderInfo.Role.Equals(null))
            {
                if (removeRoleParticipantGroup)
                {
                    RemoveRoles(selectedRoleList);
                }
                foreach (var role in teamMemberStakeholderInfo.Role)
                {
                    SelectRole(role);
                }
            }
            if (!Driver.IsElementDisplayed(ParticipantGroupListBox)) return;
            if (teamMemberStakeholderInfo.ParticipantGroup.Equals(null)) return;
            if (removeRoleParticipantGroup)
            {
                RemoveParticipantGroups(selectedParticipantGroupList);
            }
            foreach (var participantGroup in teamMemberStakeholderInfo.ParticipantGroup)
            {
                SelectParticipantGroup(participantGroup);
            }
        }

        public void EnterFirstName(string firstName)
        {
            Log.Step(nameof(CommonAddEditMembersPopupBasePage), $"Enter {firstName} into 'FirstName' textbox");
            Wait.UntilElementVisible(FirstNameTextBox);
            Wait.HardWait(1000);//wait till first name text box is displayed
            Wait.UntilElementClickable(FirstNameTextBox).SetText(firstName);
            Wait.UntilJavaScriptReady();
        }
        public void EnterLastName(string lastname)
        {
            Log.Step(nameof(CommonAddEditMembersPopupBasePage), $"Enter {lastname} into 'LastName' textbox");
            Wait.UntilElementVisible(LastNameTextBox);
            Wait.HardWait(1000);//wait till first name text box is displayed
            Wait.UntilElementClickable(LastNameTextBox).SetText(lastname);
            Wait.UntilJavaScriptReady();
        }
        public void EnterEmail(string email)
        {
            Log.Step(nameof(CommonAddEditMembersPopupBasePage), $"Enter {email} into 'Email' textbox");
            Wait.UntilElementVisible(EmailTextBox);
            Wait.HardWait(1000);//wait till first name text box is displayed
            Wait.UntilElementClickable(EmailTextBox).SetText(email);
            Wait.UntilJavaScriptReady();
        }
        public void SelectRole(string role)
        {
            Log.Step(nameof(TeamMembersBasePage), $"Select {role} from 'Role' selection box");
            Wait.UntilElementVisible(RoleListBox);
            if (Driver.IsElementDisplayed(RoleListBox))
            {
                SelectItem(RoleListBox, RoleListItem(role));
            }
        }
        public void SelectParticipantGroup(string participantGroup)
        {
            Log.Step(nameof(CommonAddEditMembersPopupBasePage), $"Select {participantGroup} from 'Participant Group' selection box");
            SelectItem(ParticipantGroupListBox, ParticipantGroupListItem(participantGroup));
        }

        #endregion

        #region Remove Data

        public void RemoveFirstName()
        {
            Log.Step(nameof(CommonAddEditMembersPopupBasePage), "Remove text from first name");
            Wait.UntilElementClickable(FirstNameTextBox).ClearTextbox();
        }
        public void RemoveLastName()
        {
            Log.Step(nameof(CommonAddEditMembersPopupBasePage), "Remove text from last name");
            Wait.UntilElementClickable(LastNameTextBox).ClearTextbox();
        }
        public void RemoveEmail()
        {
            Log.Step(nameof(CommonAddEditMembersPopupBasePage), "Remove text from email");
            Wait.UntilElementClickable(EmailTextBox).ClearTextbox();
        }
        public void RemoveRoles(List<string> roles)
        {
            foreach (var role in roles)
            {
                Log.Step(nameof(CommonAddEditMembersPopupBasePage), $"Remove selected {role} from 'Role' selection box");
                Wait.UntilElementClickable(SelectedRoleRemoveIcon(role)).Click();
            }
        }
        public void RemoveParticipantGroups(List<string> participantGroups)
        {
            Wait.UntilJavaScriptReady();
            foreach (var participantGroup in participantGroups)
            {
                Log.Step(nameof(CommonAddEditMembersPopupBasePage), $"Remove selected {participantGroup} from 'Participant Group' selection box");
                Wait.UntilElementClickable(SelectedParticipantGroupRemoveIcon(participantGroup)).Click();
            }
        }

        #endregion


        public void ClickOnCreateAndAddNewButton()
        {
            Log.Step(nameof(CommonAddEditMembersPopupBasePage), "Click on 'Create And Add New' button");
            Wait.UntilElementEnabled(CreateAndAddNewButton);
            Driver.JavaScriptScrollToElement(CreateAndAddNewButton);
            Thread.Sleep(2000); //Wait till 'Create And Add New' button is enabled
            Wait.UntilElementClickable(CreateAndAddNewButton).Click();
            Wait.UntilJavaScriptReady();
                
        }
        public void ClickOnCreateAndCloseButton()
        {
            Log.Step(nameof(CommonAddEditMembersPopupBasePage), "Click on 'Create And Close' button");
            Wait.UntilElementEnabled(CreateAndCloseButton);
            Driver.JavaScriptScrollToElement(CreateAndCloseButton);
            Thread.Sleep(3000);//Wait till 'Create And Close' button is enabled
            Wait.UntilElementClickable(CreateAndCloseButton).Click();
            Wait.UntilJavaScriptReady();
        }
        public void ClickOnUpdateButton()
        {
            Log.Step(nameof(CommonAddEditMembersPopupBasePage), "Click on 'Update' button");
            Wait.UntilElementVisible(UpdateButton);
            Wait.UntilJavaScriptReady();
            Driver.JavaScriptScrollToElement(UpdateButton);
            Thread.Sleep(500); //Wait till 'Update' button is enabled
            Wait.UntilElementClickable(UpdateButton).Click();
        }

        public bool IsCreateAndAddNewButtonEnable()
        {
            return Wait.UntilElementExists(CreateAndAddNewButton).GetAttribute("style").Contains("auto");
        }
        public bool IsCreateAndCloseButtonEnable()
        {
            return Wait.UntilElementExists(CreateAndCloseButton).GetAttribute("style").Contains("auto");
        }
        public bool IsUpdateButtonEnable()
        {
            return Wait.UntilElementExists(UpdateButton).GetAttribute("style").Contains("auto");
        }


        //Validation Message
        public string GetFieldValidationMessage(string fieldName)
        {
            return Wait.UntilElementVisible(FieldValidationMessage(fieldName)).GetText();
        }

        public string GetEmailExistValidationMessage()
        {
            return Wait.UntilElementVisible(EmailExistValidationMessage).GetText();
        }

        #endregion

        #endregion

    }
}