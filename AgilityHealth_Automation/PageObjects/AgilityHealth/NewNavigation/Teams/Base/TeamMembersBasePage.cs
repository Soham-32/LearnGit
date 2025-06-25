using System.Threading;
using AgilityHealth_Automation.Utilities;
using AtCommon.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.NewNavigation.Teams.Base
{
    public class TeamMembersBasePage : CommonGridBasePage
    {
        public CommonAddEditMembersPopupBasePage PopupBase { get; set; }
        public CommonSelectTeamMemberFromDirectoryPopupBasePage SelectTeamMemberFromDictionaryPopupBase { get; set; }

        public TeamMembersBasePage(IWebDriver driver, ILogger log) : base(driver, log)
        {
            PopupBase = new CommonAddEditMembersPopupBasePage(driver, log);
            SelectTeamMemberFromDictionaryPopupBase = new CommonSelectTeamMemberFromDirectoryPopupBasePage(driver, log);
        }

        #region Locators

        #region Add New Member dropdown button and dropdown options
        private readonly By AddTeamMembersLeadersDropdown = By.XPath("//div[@id='TeamMembersGrid']//ul[@id='ddMenuBulkOperations']");
        private readonly By AddNewTeamMembersOrLeadersOption = By.XPath("//div[@id='TeamMembersGrid']//ul//li/a[@id='add-teammember']");
        private readonly By TeamMembersOrLeadersFromDirectoryButton = By.XPath("//div[@id='TeamMembersGrid']//ul//li/a[@onclick='ShowCompanyLookUpDialogtm()']");
        #endregion

        #region Team Member grid
        private static By TeamMemberEditButton(string email) => By.XPath($"//div[@id='TeamMembersGrid']/table/tbody/tr/td[text()='{email}']//following-sibling::td/ul/li[contains(@class,'k-first')]");
        private static By TeamMemberDropdownButton(string email) => By.XPath($"//div[@id='TeamMembersGrid']/table/tbody/tr/td[text()='{email}']//following-sibling::td/ul/li[contains(@class,'k-last')]");
        private static By TeamMemberTeamAccessAddButton(string email) => By.XPath($"//td[text()='{email.ToLower()}']/following-sibling::td//a/i");
        private static By TeamMemberSuccessfullyTeamAccessIcon(string email) => By.XPath($"//td[text()='{email.ToLower()}']/following-sibling::td/div/img[@class='img-is-teammember']");
        private static By TeamMemberCheckbox(string email) => By.CssSelector($"[name='memberId'][value='{email}']");
        private readonly By TeamMemberDeleteMenuItem = By.CssSelector("li[id^='menu_Delete']");
        
        #endregion

        #region Team Member stepper
        private readonly By TeamMemberQuickLinkCopyIcon = By.Id("btnCopyToClipboardTeamTM");

        private readonly By RemoveSelectedButton = By.Id("rem-t-member-btn");
        #endregion

        #endregion



        #region Methods

        #region Add New Member dropdown button
        public void ClickOnAddTeamMembersOrLeadersDropdown()
        {
            Log.Step(nameof(TeamMembersBasePage), "Click on 'Add Team Member' Dropdown");
            Wait.UntilElementVisible(AddTeamMembersLeadersDropdown);
            Thread.Sleep(500);// Wait till 'Add Team Member' dropdown options are Visible
            Wait.UntilElementClickable(AddTeamMembersLeadersDropdown).Click();
        }
        public void ClickOnAddNewTeamMembersOrLeadersOption()
        {
            Log.Step(nameof(TeamMembersBasePage), "Click on 'Add New Team Member/Leader' option");
            Wait.UntilElementVisible(AddNewTeamMembersOrLeadersOption);
            Thread.Sleep(500);// Wait till 'Add New Team Member/Leader' options are Visible
            Wait.UntilElementClickable(AddNewTeamMembersOrLeadersOption).Click();
        }
        public void OpenAddTeamMembersOrLeadersPopup()
        {
            Log.Step(nameof(TeamMembersBasePage), "Click on 'Add Team Members/Leaders' dropdown then 'Add New Team Member/Leader' option");
            ClickOnAddTeamMembersOrLeadersDropdown();
            ClickOnAddNewTeamMembersOrLeadersOption();
        }
        #endregion


        #region Select Team Member(s)/Leader(s)/Stakeholder(s) From Directory
        public void OpenSelectTeamMembersOrLeadersFromDirectoryPopup()
        {
            Log.Step(nameof(TeamMembersBasePage), "Click on 'Add Team Member/Leader' Dropdown then 'Team Member/Leader Directory' option");
            ClickOnAddTeamMembersOrLeadersDropdown();
            ClickOnTeamMembersOrLeadersFromDirectoryOption();
            Wait.HardWait(2000); //Need to wait until popup loaded successfully
        }
        public void ClickOnTeamMembersOrLeadersFromDirectoryOption()
        {
            Log.Step(nameof(TeamMembersBasePage), "Click on 'Team Member/Leader From Directory' option");
            Wait.UntilElementClickable(TeamMembersOrLeadersFromDirectoryButton).Click();
        }
        #endregion

        #region Team member grid
        public void ClickTeamMemberEditButton(string email)
        {
            Log.Step(nameof(TeamMembersBasePage), "Click on 'Team Member Edit' button");
            Wait.UntilElementVisible(TeamMemberEditButton(email));
            Driver.JavaScriptScrollToElement(TeamMemberEditButton(email));
            Thread.Sleep(500);//Wait till 'Team Member Edit' button is displayed
            Wait.UntilElementClickable(TeamMemberEditButton(email)).Click();
            Wait.UntilJavaScriptReady();
        }
        public void ClickOnTeamMemberTeamAccessButton(string email)
        {
            Log.Step(nameof(TeamMembersBasePage), $"Click on 'Add' button for 'Team Access' column for team member {email}");
            Driver.JavaScriptScrollToElement(TeamMemberTeamAccessAddButton(email));
            Wait.UntilElementClickable(TeamMemberTeamAccessAddButton(email)).Click();
            Wait.UntilJavaScriptReady();
        }

        public bool IsTeamMemberSuccessfullyTeamAccessIconDisplayed(string email)
        {
            return Driver.IsElementDisplayed(TeamMemberSuccessfullyTeamAccessIcon(email));
        }

        public void ClickOnTeamMemberCheckbox(string email)
        {
            Log.Step(nameof(TeamMembersBasePage), "Click on 'Team Member' checkbox");
            Wait.UntilElementVisible(TeamMemberCheckbox(email)).Click();
        }

        public void DeleteTeamMember(string email)
        {
            Log.Step(nameof(TeamMembersBasePage), $"Delete a team member with email {email}");
            Wait.UntilElementVisible(TeamMemberDropdownButton(email)).Click();
            Wait.UntilElementVisible(TeamMemberDeleteMenuItem).Click();
            Driver.AcceptAlert();
            Wait.UntilJavaScriptReady();
        }

        #endregion

        #region Team Member stepper

        public string GetCopiedQuickLinkText()
        {
            Log.Step(nameof(TeamMembersBasePage), "Click on 'Copy' icon");
            Wait.UntilElementClickable(TeamMemberQuickLinkCopyIcon).Click();
            var copiedLink = CSharpHelpers.GetClipboard();
            return copiedLink;
        }

        public void RemoveSelectedTeamMember()
        {
            Log.Step(nameof(TeamMembersBasePage), "Click on 'Removed Selected' button, then confirm removing");
            Wait.UntilElementVisible(RemoveSelectedButton).Click();
            Driver.AcceptAlert();
            Wait.UntilJavaScriptReady();
        }
        #endregion

        #endregion


    }
}
