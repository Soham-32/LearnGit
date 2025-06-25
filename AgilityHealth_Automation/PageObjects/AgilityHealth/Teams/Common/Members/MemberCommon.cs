using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.Utilities;
using AtCommon.Dtos.Teams;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Common.Members
{
    public class MemberCommon : BasePage
    {
        public MemberCommon(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        private readonly By AddFromDirectoryButton = By.CssSelector("span[onclick='ShowCompanyLookUpDialog()']");
        private static By TeamMemberEmail(string emailOrName) => By.XPath($"//table[@class='persondata-table']/tbody//td[count(//table[@class='persondata-table']//th[@data-title='Email']/preceding-sibling::th)+1][text()='{emailOrName}'] | //table[@class='persondata-table']/tbody//td[count(//table[@class='persondata-table']//th[@data-title='Email']/preceding-sibling::th)+1]//*[text()='{emailOrName}']");
        private readonly By UploadMembersButton = By.Id("uploadBtn");
        private By MemberFromGrid(string imagePath, string firstName, string lastName, string email) =>
                    By.XPath($"//table/tbody//td/img[@src='{imagePath}']/../following-sibling::td[text()='{firstName}']/following-sibling::td[text()='{lastName}']/following-sibling::td[text()='{email}']");
        private By MemberAvatarImageFromGrid(string email) => By.XPath($"//table/tbody//td[text()='{email}']/preceding-sibling::td//img");

        //Company Lookup Popup
        private readonly By CompanyLookupPopupTeamsTab = By.XPath("//a[text()='Teams'] | //a/font/font[text()='Teams']");
        private static By CompanyLookupMemberOrTeamCheckBox(string name) => By.XPath($"//td[text()='{name}']//preceding-sibling::td//input | //td//font[text()='{name}']//..//..//preceding-sibling::td//input");

        //Company Lookup Popup Members Tab
        private readonly By CompanyLookUpMembersTabSearchTextbox = By.Id("filterMembers");
        private readonly By CompanyLookupMembersTabAddSelectedTeamMembersButton = By.Id("btnAddMember");

        //Company Lookup Popup Teams Tab
        private readonly By CompanyLookUpTeamsTabSearchTextbox = By.Id("filterTeams");
        private readonly By CompanyLookupTeamsTabAddSelectedTeamMembersButton = By.Id("btnAddTeam");

        //Upload Team Members & Stakeholders popup
        private readonly By UploadPopupSelectExcelFileButton = By.XPath("//input[@id='files']");
        private readonly By UploadPopupUploadingDoneIndicator = By.XPath("//span[@class='k-icon k-i-tick']");
        private readonly By UploadPopupUploadButton = By.XPath("//input[@id='uploadFile']");
        private readonly By UploadPopupUploadErrorMessage = By.Id("uploadError");
        private readonly By ImportCompletePopupCloseButton = By.XPath("//button[text()='Close'] | //button//font[text()='Close']");

        //add member popup
        internal readonly By FirstNameTextBox = By.Id("FirstName");
        internal readonly By LastNameTextBox = By.Id("LastName");
        internal readonly By EmailTextBox = By.Id("Email");
        private readonly By RoleListBox = By.XPath("//label[normalize-space()='Role' or @for='Role']/following-sibling::div//input");

        private static By RoleListItem(string role) => By.XPath(
            $"//ul[ @id='Role_listbox' or @id='Categories_0__SelectedTags_listbox']/li[text()='{role}'] | //ul[ @id='Role_listbox' or @id='Categories_0__SelectedTags_listbox']/li//font[text()='{role}']");
        internal readonly By UploadTeamMemberPhoto = By.Id("file");
        internal readonly By ResetPhotoLink = By.Id("reset");
        internal readonly By SaveAndCloseButton = By.XPath("//a[text()='Save and Close'] | //a//font[text()='Save and Close']");
        internal readonly By DeleteRoleSymbol = By.XPath("//label[normalize-space()='Role' or @for='Role']/following-sibling::div//li/span[@class='k-select']");
        internal readonly By EmailFieldValidationMessage = By.Id("Email_validationMessage");
        private readonly By AddNewMembersPopupCancelButton = By.XPath("//a[text()='Cancel']");
        private readonly By AddNewMembersPopupSaveAndAddNewButton = By.XPath("//a[text()='Save and Add New'] | //a//font[text()='Save and Add New']");
        private readonly By AddNewTeamMembersPopupTitle = By.XPath("//div//span[@role='heading'][text()='Add New Team Member'][not(@id='sessionWindow_wnd_title')] | //div//span[@role='heading']//font[text()='Add New Team Member'][not(@id='sessionWindow_wnd_title')]");
        private readonly By AddNewMembersPopupCloseIcon = By.XPath("//span[@class='k-icon k-i-close']");
        private readonly By PageHeaderTitle = By.XPath("//div[@class='txt fl-lt']//p");


        private readonly By MemberRoleField = By.XPath("//label[@for='Role' or normalize-space()='Role']/following-sibling::div[contains(@class,'k-widget k-multiselect k-header')]//div");
        private readonly By MemberTagRoleList = By.XPath("//ul[@id='Role_listbox' or @id='Categories_0__SelectedTags_listbox']//li");
        private readonly By TeamMemberTagParticipantGroupField = By.XPath("//label[@for='ParticipantGroup' or normalize-space()='Participant Group']/following-sibling::div[contains(@class,'k-widget k-multiselect k-header')]//div");
        private readonly By TeamMemberTagParticipantGroupList = By.XPath("//ul[@id='ParticipantGroup_listbox' or @id='Categories_3__SelectedTags_listbox' or @id='Categories_1__SelectedTags_listbox']//li");
        // Invalid Team Member & Stakeholder popup
        private readonly By InvalidMemberPopupUpdateButton = By.XPath("//div[@id='promptToFixUserWindow']//button[@onclick='updateUserRecord()']");
        private readonly By InvalidMemberPopupSkipTeamMemberButton = By.XPath("//div[@id='promptToFixUserWindow']//button[@onclick='skipUser()']");
        private readonly By InvalidMemberPopupErrorMessage = By.XPath("//div[@id='promptToFixUserWindow']//span[@id='promptError']");
        private static By InvalidMemberPopupUserDetailsTextbox(string userDetailField) => By.Id($"prompt{userDetailField}");

        // Blank Excel Import
        private readonly  By BlankExcelImportPopupMessage = By.Id("blankFileImportWindow");

        public string GetPageHeaderTitle()
        {
            Log.Step(GetType().Name, "On Edit Team page, get page header title");
            return Wait.UntilElementVisible(PageHeaderTitle).GetText();
        }
        public void ClickOnUploadButton()
        {
            Log.Step(nameof(MemberCommon), "Click on 'Upload Team Members' or 'Upload Stakeholder' button");
            Wait.UntilElementClickable(UploadMembersButton).Click();
        }

        public void UploadPopupSelectExcelFile(string filePath)
        {
            Log.Step(nameof(MemberCommon), "Upload the excel file.");
            Wait.UntilElementExists(UploadPopupSelectExcelFileButton).SendKeys(filePath);
        }

        public void UploadPopupWaitUntillExcelFileUploadedSuccessfully()
        {
            Log.Step(nameof(MemberCommon), "Verify that excel file is uploaded successfully.");
            Wait.UntilElementInvisible(UploadPopupUploadErrorMessage);
            Wait.UntilElementVisible(UploadPopupUploadingDoneIndicator);
        }

        public void UploadPopupClickOnUploadButton()
        {
            Log.Step(nameof(MemberCommon), "Click on 'Upload' button");
            Wait.UntilElementClickable(UploadPopupUploadButton).Click();
            Wait.UntilJavaScriptReady();
        }

        public void ImportCompletePopupClickOnCloseButton()
        {
            Log.Step(nameof(MemberCommon), "Click on 'Close' button");
            Wait.UntilElementClickable(ImportCompletePopupCloseButton).Click();
            Wait.UntilJavaScriptReady();
        }

        public void UploadMembersFromExcelFile(string filePath)
        {
            Log.Step(nameof(MemberCommon), "Upload Team Members's or Stakeholders's excel file ");
            ClickOnUploadButton();
            UploadPopupSelectExcelFile(filePath);
            UploadPopupWaitUntillExcelFileUploadedSuccessfully();
            UploadPopupClickOnUploadButton();
        }
        public bool DoesMemberExist(string imagePath, string firstName, string lastName, string email)
        {
            Log.Step(nameof(MemberCommon), "Verify that member or stakeholder is exist on grid.");
            return Driver.IsElementDisplayed(MemberFromGrid(imagePath, firstName, lastName, email));
        }
        public string GetAvatarFromMembersGrid(string email) => Wait.UntilElementVisible(MemberAvatarImageFromGrid(email)).GetElementAttribute("src").Replace("https://atqa.agilityinsights.ai", "");

        public void ClickAddFromDirectoryButton()
        {
            Log.Step(GetType().Name, "Click and add from directory button");
            Wait.UntilJavaScriptReady();
            Wait.UntilElementClickable(AddFromDirectoryButton).Click();
            Wait.UntilJavaScriptReady();
        }

        //Company Lookup Popup
        public void CompanyLookupSelectMemberOrTeam(string memberEmailOrTeamName)
        {
            Log.Step(GetType().Name, $"Select member or team : {memberEmailOrTeamName}");
            Wait.UntilElementClickable(CompanyLookupMemberOrTeamCheckBox(memberEmailOrTeamName)).Check();
            Wait.UntilJavaScriptReady();
        }

        public void CompanyLookupClickOnTeamsTab()
        {
            Log.Step(GetType().Name, "Click on teams tab");
            Wait.UntilElementClickable(CompanyLookupPopupTeamsTab).Click();
        }

        public void CompanyLookupAddMembersFromDirectory(List<string> emailAddresses)
        {
            Log.Step(GetType().Name, "Add members from directory");
            ClickAddFromDirectoryButton();
            foreach (var email in emailAddresses)
            {
                CompanyLookupSearchMembers(email);
                CompanyLookupSelectMemberOrTeam(email);
            }
            CompanyLookupMembersTabClickOnAddSelectedTeamMembersButton();
            Wait.UntilLoadingDone();
        }

        public void CompanyLookupAddTeamsFromDirectory(List<string> teamNameList)
        {
            Log.Step(GetType().Name, "Click on add from directory, Click on teams tab, Select teams and click on Add");
            ClickAddFromDirectoryButton();
            CompanyLookupClickOnTeamsTab();
            foreach (var teamName in teamNameList)
            {
                CompanyLookupSearchTeams(teamName);
                CompanyLookupSelectMemberOrTeam(teamName);
            }
            CompanyLookupTeamsTabClickOnAddSelectedTeamsButton();
            Wait.UntilLoadingDone();
        }

        //Members
        public void CompanyLookupSearchMembers(string email)
        {
            Log.Step(GetType().Name, $"Search {email} Member");
            Wait.UntilElementClickable(CompanyLookUpMembersTabSearchTextbox).SetText(email);
        }

        public void CompanyLookupMembersTabClickOnAddSelectedTeamMembersButton()
        {
            Log.Step(GetType().Name, "Click on 'Add Selected Team Members' button for members");
            Wait.UntilElementClickable(CompanyLookupMembersTabAddSelectedTeamMembersButton).Click();
            Wait.UntilJavaScriptReady();
        }

        //Teams
        public void CompanyLookupSearchTeams(string teamName)
        {
            Log.Step(GetType().Name, $"Search {teamName} teams");
            Wait.UntilElementClickable(CompanyLookUpTeamsTabSearchTextbox).SetText(teamName);
        }
        public void CompanyLookupTeamsTabClickOnAddSelectedTeamsButton()
        {
            Log.Step(GetType().Name, $"Click on 'Add Selected Team Members' button for teams");
            Wait.UntilElementClickable(CompanyLookupTeamsTabAddSelectedTeamMembersButton).Click();
            Wait.UntilJavaScriptReady();
        }

        public bool IsTeamMemberDisplayed(string email, int timeout =20)
        {
            return Driver.IsElementDisplayed(TeamMemberEmail(email.ToLower()), timeout);
        }

        public bool IsAddFromDirectoryButtonPresent()
        {
            return Wait.InCase(AddFromDirectoryButton) != null && Wait.InCase(AddFromDirectoryButton).Displayed;
        }

        public void SelectRole(string role)
        {
            Log.Step(GetType().Name, $"Select role from RoleList");
            SelectItem(RoleListBox, RoleListItem(role));
        }

        public void UploadPhoto(string filePath)
        {
            Log.Step(GetType().Name, $"Upload photo");
            Wait.UntilElementExists(UploadTeamMemberPhoto).SendKeys(filePath);
        }

        public void ClickSaveAndCloseButton()
        {
            Log.Step(GetType().Name, "On the popup, click Save and Close button");
            Wait.UntilElementVisible(SaveAndCloseButton);
            Wait.UntilElementClickable(SaveAndCloseButton).Click();
            Wait.UntilElementNotExist(SaveAndCloseButton);
            Wait.HardWait(2000); // Wait until popup disappeared.
        }
        public void AddNewMembersPopupClickOnCancelButton()
        {
            Log.Step(GetType().Name, "Click on 'Cancel' button");
            Wait.UntilElementClickable(AddNewMembersPopupCancelButton).Click();
        }

        public void AddNewMembersPopupClickOnSaveAndAddNewButton()
        {
            Log.Step(GetType().Name, "Click on 'Save and add New' button");
            Wait.UntilElementClickable(AddNewMembersPopupSaveAndAddNewButton).Click();
            Wait.UntilJavaScriptReady();
        }
        public void AddNewMembersPopupClickOnCloseIcon()
        {
            Log.Step(GetType().Name, "Click on 'Close' icon ");
            Wait.UntilElementClickable(AddNewMembersPopupCloseIcon).Click();
            Wait.UntilJavaScriptReady();
        }
        public bool IsAddNewTeamMembersPopupDisplayed()
        {
            Wait.UntilJavaScriptReady();
            return Driver.IsElementDisplayed(AddNewTeamMembersPopupTitle);
        }

        public void DeleteRole()
        {
            Log.Step(GetType().Name, "Delete Role");
            if (Driver.IsElementPresent(DeleteRoleSymbol))
            {
                Wait.UntilJavaScriptReady();
                Wait.UntilElementClickable(DeleteRoleSymbol).Click();
            }
        }

        public void HideEmailValidationMessage()
        {
            Log.Step(GetType().Name, "Hide email availibility message");
            if (Wait.InCase(EmailFieldValidationMessage) != null)
            {
                Driver.JavaScriptSetAttribute(Wait.UntilElementVisible(EmailFieldValidationMessage), "style", "display: none;");
            }
        }

        public List<string> GetMemberRoleDropdownList()
        {
            Log.Step(GetType().Name, "Get memeber's role list");
            Wait.UntilJavaScriptReady();
            Wait.UntilElementClickable(MemberRoleField).Click();
            Wait.UntilJavaScriptReady();
            var getMemberRoleList = Wait.UntilAllElementsLocated(MemberTagRoleList).Select(row => row.GetText()).ToList();
            Wait.UntilElementClickable(FirstNameTextBox).Click();
            return getMemberRoleList;
        }
        public List<string> GetTeamMemberParticipantGroupDropdownList()
        {
            Log.Step(GetType().Name, "Get memeber's participant group list");
            Wait.UntilElementClickable(TeamMemberTagParticipantGroupField).Click();
            Wait.UntilJavaScriptReady();
            return Wait.UntilAllElementsLocated(TeamMemberTagParticipantGroupList).Select(row => row.GetText()).ToList();

        }

        // Invalid Team Member & Stakeholder popup
        public void InvalidMemberPopupClickOnUpdateButton()
        {
            Log.Step(nameof(MemberCommon), "On invalid member popup, Click on 'Update' button");
            Wait.UntilElementClickable(InvalidMemberPopupUpdateButton).Click();
        }
        public void InvalidMemberPopupClickOnSkipTeamMemberButton()
        {
            Log.Step(nameof(MemberCommon), "On invalid member popup, Click on 'Skip Team Member' button");
            Wait.UntilElementClickable(InvalidMemberPopupSkipTeamMemberButton).Click();
        }
        public string InvalidMemberPopupGetErrorMessage()
        {
            Log.Step(GetType().Name, "On invalid member popup, get error message");
            return Wait.UntilElementVisible(InvalidMemberPopupErrorMessage).GetText();
        }

        public void FillMemberInfo(AddMemberRequest memberInfo)
        {
            Wait.UntilElementExists(InvalidMemberPopupUserDetailsTextbox("Email")).SetText(memberInfo.Email);
            Wait.UntilElementExists(InvalidMemberPopupUserDetailsTextbox("FirstName")).SetText(memberInfo.FirstName);
            Wait.UntilElementExists(InvalidMemberPopupUserDetailsTextbox("LastName")).SetText(memberInfo.LastName);
        }

        // Blank Excel Import
        public string BlankExcelImportPopupGetPopupMessage()
        {
            Log.Step(GetType().Name, "On blank excel import popup, get popup message");
            return Wait.UntilElementVisible(BlankExcelImportPopupMessage).GetText();
        }
    }
}
