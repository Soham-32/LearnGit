using System;
using System.IO;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.DataObjects;
using AgilityHealth_Automation.Utilities;
using AtCommon.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Common.Members
{
    public class TeamMemberCommon : MemberCommon
    {
        public TeamMemberCommon(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        private readonly By AddTeamMemberButton = By.Id("add-team-member");
        private readonly By DownloadTemplateIcon = By.XPath("//a[contains(@title,'Download')]");
        private readonly By InviteTeamMemberLinkCopyIcon = By.Id("btnCopyToClipboardTeam");

        // Add member popup
        private readonly By EmploymentTypeSection = By.CssSelector("label[for='EmployeeType']");
        private readonly By AllocationTypeSection = By.CssSelector("label[for='AllocationType']");
        private readonly By ParticipantGroupListBox = By.XPath("//label[normalize-space()='Participant Group']/following-sibling::div//input");
        private static By ParticipantGroupListItem(string item) => By.XPath($"//ul[@id='ParticipantGroup_listbox' or @id='Categories_3__SelectedTags_listbox' or @id='Categories_1__SelectedTags_listbox']/li[text()='{item}']");
        private readonly By DeleteParticipantSymbol = By.XPath("//label[normalize-space()='Participant Group']/following-sibling::div//li/span[@class='k-select']");

        // Grid by Index
        private static By FirstNameFromGrid(int index) => By.XPath($"//div[@id='TeamMembersGrid']/table/tbody/tr[{index}]/td[2]");
        private static By LastNameFromGrid(int index) => By.XPath($"//div[@id='TeamMembersGrid']/table/tbody/tr[{index}]/td[3]");
        private static By EmailFromGrid(int index) => By.XPath($"//div[@id='TeamMembersGrid']/table/tbody/tr[{index}]/td[4]");
        private static By RoleFromGrid(int index) => By.XPath($"//div[@id='TeamMembersGrid']/table/tbody/tr[{index}]/td[5]");
        private static By ParticipantGroupFromGrid(int index) => By.XPath($"//div[@id='TeamMembersGrid']/table/tbody/tr[{index}]/td[6]");

        // Grid by Email
        private static By FirstNameFromGrid(string email) => By.XPath($"//td[text()='{email}']/preceding-sibling::td[2]");
        private static By LastNameFromGrid(string email) => By.XPath($"//td[text()='{email}']/preceding-sibling::td[1]");
        private static By EmailFromGrid(string email) => By.XPath($"//td[text()='{email}']");
        private static By RoleFromGrid(string email) => By.XPath($"//td[text()='{email}']/following-sibling::td[1]");

        private static By ParticipantGroupByName(string email, string participantGroup) => By.XPath($"//td[text()='{email}']//following-sibling::td[text()='{participantGroup}']");
        private static By ParticipantGroupFromGrid(string email) =>
            By.XPath($"//td[text()='{email}']//following-sibling::td[2]");

        private static By TeamMemberAccessAddButton(string email) => By.XPath(
            $"//*[@id='TeamMembersGrid']//td[contains(text(), '{email.ToLower()}')]//following-sibling::td//span[text()='Add']");
        private static By TeamMemberAccessedImage(string email) => By.XPath($"//*[@id='TeamMembersGrid']//td[contains(text(), '{email.ToLower()}')]//following-sibling::td//div//img[@class='img-is-teammember']");
        private static By TeamMemberEditButton(int index) => By.XPath($"//div[@id='TeamMembersGrid']/table/tbody/tr[{index}]/td[8]/a[contains(@class,'k-grid-edit')]");
        private static By TeamMemberDeleteButton(string firstName) => By.XPath($"//div[@id='TeamMembersGrid']/table/tbody//td[text()='{firstName}']/following-sibling::td/a[contains(@class,'k-grid-delete')]");

        public void ClickAddNewTeamMemberButton()
        {
            Log.Step(GetType().Name, "On Add Team Member page, click Add Team Member button");
            Wait.UntilJavaScriptReady();
            Wait.UntilElementClickable(AddTeamMemberButton).Click();
            Wait.UntilJavaScriptReady();
        }

        public void ClickOnDownloadTemplateIcon()
        {
            Log.Step(GetType().Name, "Click on Download Excel Template");
            Wait.UntilElementClickable(DownloadTemplateIcon).Click();
        }

        public bool IsInviteTeamMemberLinkCopyIconDisplayed()
        {
            return Driver.IsElementDisplayed(InviteTeamMemberLinkCopyIcon);
        }

        public void ClickOnInviteTeamMemberLinkCopyIcon()
        {
            Log.Step(GetType().Name, "Click on invite team member link copy icon");
            Wait.UntilElementClickable(InviteTeamMemberLinkCopyIcon).Click();
        }

        public void EnterTeamMemberInfo(TeamMemberInfo memberInfo, string operationType = "Add")
        {

            if (Driver.IsInternetExplorer())
            {
                if (operationType.Equals("Edit"))
                {
                    //Clearing fields
                    Wait.UntilElementClickable(EmailTextBox).SetText("");
                    Wait.UntilJavaScriptReady();
                    Wait.UntilElementClickable(LastNameTextBox).SetText("");
                    Wait.UntilJavaScriptReady();
                    Wait.UntilElementClickable(FirstNameTextBox).SetText("");
                    Wait.UntilJavaScriptReady();
                }
                else
                {
                    Wait.UntilElementClickable(FirstNameTextBox).SendKeys(Keys.Tab);
                }

                //Using external .exe to fill data. EXE is generated via AutoIT

                var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\AutoIt\\Teams\\TeamMemberStakeholderAddEdit.exe");
                var commandLineArguments = $"{operationType}*Team*Members" + " " + memberInfo.FirstName + " " + memberInfo.LastName + " " + memberInfo.Email;
                CSharpHelpers.RunExternalExe(filePath, commandLineArguments);
            }
            else
            {
                Wait.UntilElementClickable(EmailTextBox).SetText(memberInfo.Email);
                Wait.UntilJavaScriptReady();
                Wait.UntilElementVisible(FirstNameTextBox);
                Wait.UntilElementClickable(FirstNameTextBox).SetText(memberInfo.FirstName);
                Wait.UntilJavaScriptReady();
                Wait.UntilElementClickable(LastNameTextBox).SetText(memberInfo.LastName);
                Wait.UntilJavaScriptReady();
            }


            if (!string.IsNullOrEmpty(memberInfo.Role))
            {
                DeleteRole();
                SelectRole(memberInfo.Role);
            }

            if (!string.IsNullOrEmpty(memberInfo.ParticipantGroup))
            {
                DeleteParticipant();
                SelectParticipantGroup(memberInfo.ParticipantGroup);
            }

            if (!string.IsNullOrEmpty(memberInfo.ImagePath))
            {
                UploadPhoto(memberInfo.ImagePath);
            }
        }

        public void ClickOnTeamMemberTeamAccessButton(string email)
        {
            Log.Step(GetType().Name, $"Click on 'Add' button for 'Team Access' field for team member {email}");
            Wait.UntilElementClickable(TeamMemberAccessAddButton(email)).Click();
            Wait.UntilJavaScriptReady();
        }

        public bool IsTeamMemberHaveTeamAccess(string email)
        {
            return Driver.IsElementDisplayed(TeamMemberAccessedImage(email));
        }

        public TeamMemberInfo GetTeamMemberInfoFromGrid(int index)
        {
            Wait.HardWait(2000);//Takes time to load details
            TeamMemberInfo teamMemberInfo = new TeamMemberInfo
            {
                FirstName = Wait.UntilElementExists(FirstNameFromGrid(index)).Text,
                LastName = Wait.UntilElementExists(LastNameFromGrid(index)).Text,
                Email = Wait.UntilElementExists(EmailFromGrid(index)).Text,
                Role = Wait.UntilElementExists(RoleFromGrid(index)).Text,
                ParticipantGroup = Wait.UntilElementExists(ParticipantGroupFromGrid(index)).Text
            };

            return teamMemberInfo;

        }

        public TeamMemberInfo GetTeamMemberInfoFromGridByEmail(string email)
        {
            Log.Step(GetType().Name, $"Get the Team member information from grid for team member {email}");
            var teamMemberInfo = new TeamMemberInfo
            {
                FirstName = Wait.UntilElementExists(FirstNameFromGrid(email)).Text,
                LastName = Wait.UntilElementExists(LastNameFromGrid(email)).Text,
                Email = Wait.UntilElementExists(EmailFromGrid(email)).Text,
                Role = Wait.UntilElementExists(RoleFromGrid(email)).Text,
                ParticipantGroup = Wait.UntilElementExists(ParticipantGroupFromGrid(email)).Text
            };
            return teamMemberInfo;
        }

        public string GetTeamMemberParticipantGroup(string email)
        {
            Log.Step(nameof(TeamMemberCommon), "Get team member participant group");
            Wait.HardWait(2000); //It take time to load participant group
            return Wait.UntilElementExists(ParticipantGroupFromGrid(email)).GetText();
        }

        public bool DoesParticipantGroupExist(string email, string participantGroup)
        {
            return Driver.IsElementDisplayed(ParticipantGroupByName(email, participantGroup));
        }

        public void DeleteParticipant()
        {
            if (Driver.IsElementPresent(DeleteParticipantSymbol))
            {
                Wait.UntilJavaScriptReady();
                Wait.UntilElementClickable(DeleteParticipantSymbol).Click();
            }
        }

        public void SelectParticipantGroup(string participantGroup)
        {
            SelectItem(ParticipantGroupListBox, ParticipantGroupListItem(participantGroup));
        }

        public void ClickTeamMemberEditButton(int index)
        {
            Wait.UntilElementClickable(TeamMemberEditButton(index)).Click();
            Wait.UntilJavaScriptReady();
        }

        public bool IsAllocationTypeSectionPresent()
        {
            return Driver.IsElementPresent(AllocationTypeSection);
        }
        public bool IsEmploymentTypeSectionPresent()
        {
            return Driver.IsElementPresent(EmploymentTypeSection);
        }

        public void DeleteTeamMember(string memberEmail)
        {
            Log.Step(nameof(TeamMemberCommon), "Click on 'Delete' button ");
            Driver.JavaScriptScrollToElement(TeamMemberDeleteButton(memberEmail));
            Wait.UntilElementClickable(TeamMemberDeleteButton(memberEmail)).Click();
            Driver.AcceptAlert();
        }


        public void NavigateToPage(int teamId)
        {
            Log.Step(GetType().Name, $"Navigate to team id {teamId}");
            NavigateToUrl($"{BaseTest.ApplicationUrl}/teammembers/edit/{teamId}");
            Wait.HardWait(2000); // Need to wait till team members edit page display properly
        }

    }
}
