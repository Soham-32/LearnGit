using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.DataObjects;
using AgilityHealth_Automation.Utilities;
using AtCommon.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.ManageUsers
{
    internal class ManageUserPage : BasePage
    {
        private readonly string UserTabId;
        private readonly string UserTabName;

        public ManageUserPage(IWebDriver driver, ILogger log, UserType userType) : base(driver, log)
        {
            UserTabId = UserTabIdList[userType.ToString()];
            UserTabName = UserTabNameList[userType.ToString()];
        }

        private static readonly Dictionary<string, string> UserTabIdList = new Dictionary<string, string>
        {
            {"CompanyAdmin" , "companyAdminGrid"},
            {"OrganizationalLeader" , "OrganizationalLeaderGrid"},
            {"BusinessLineAdmin" , "TagAdminGrid"},
            {"TeamAdmin" , "TeamAdminGrid"},
            {"PartnerAdmin" , "PartnerAdminGrid"},
            {"Coaches" , "CoachGrid"},
            {"Member" , "IndividualGrid"}
        };
        private static readonly Dictionary<string, string> UserTabNameList = new Dictionary<string, string>
        {
            {"CompanyAdmin" , "Company Admins"},
            {"OrganizationalLeader" , "Organizational Leaders"},
            {"BusinessLineAdmin" , "Business Line Admins" },
            {"TeamAdmin" , "Team Admins"},
            {"PartnerAdmin" , "Partner Admins"},
            {"Coaches" , "Coaches"},
            {"Member" , "Individuals"}
        };

        //Title
        private readonly By ManageUserPageTitle = By.XPath("//h2[text()='Manage Users']");

        // Manage User Tab
        private By UserTab => By.XPath($"//div[@id='UserPanel']/ul/li/a[text()='{UserTabName}']");
        private readonly By SearchTextbox = By.Id("txtSearchAllUsers");
        private readonly By GoButton = By.Id("btnGoSearchUser");
        private readonly By EditIcon = By.XPath("//a[@id='mainSearchEdit']");
        private readonly By CloseButton = By.Id("btnGoSearchUserClose");
        private static By ImpersonateButton(string email) => By.XPath($"//td//form//input[@value='{email}']//following-sibling::button[contains(@class,'Impersonatebtn')]");
        private By AddNewUserButton => By.XPath($"*//div[@id='{UserTabId}']//div//a[contains(normalize-space(),'Add New')] | *//div[@id='{UserTabId}']//div//a/span[@class='k-icon k-add']");

        private readonly By SearchFilterTextbox = By.XPath("//div[@id='TeamAdminGrid']//div//input[@id='teamAdminFilterBox']");
        private By EditUserIcon(string email) => By.XPath($"//div[@id='{UserTabId}']/table/tbody//td[text()='{email}']/following-sibling::td/a[contains(@class,'k-grid-edit')]/span");
        private By DeleteUserIcon(string email) => By.XPath($"//div[@id='{UserTabId}']/table/tbody//td[text()='{email}']/following-sibling::td/a[contains(@class,'k-grid-delete')]/span");
        private By UserEmail(string email) => By.XPath($"//div[@id='{UserTabId}']/table/tbody//td[text()='{email}']");
        private By UserCreatedDate(string imagePath, string firstName, string lastName, string email, string addedDate) =>
            By.XPath($"//div[@id='{UserTabId}']/table/tbody//td/img[@src='{imagePath}']/../following-sibling::td[text()='{firstName}']/following-sibling::td[text()='{lastName}']/following-sibling::td[text()='{email}']/following-sibling::td[contains(normalize-space(),'{addedDate}')]");
        private By LastLogin(string firstName, string lastName, string email) =>
            By.XPath($"//div[@id='{UserTabId}']/table/tbody//td[text()='{firstName}']/following-sibling::td[text()='{lastName}']/following-sibling::td[text()='{email}']/following-sibling::td[not(@style='display:none')][2]");
        private By AvatarImage(string email) => By.XPath($"//div[@id='{UserTabId}']/table/tbody//td[text()='{email}']/preceding-sibling::td//img");
        private By UserAddedDate(string email) => By.XPath($"//div[@id='{UserTabId}']//table//tbody//tr/td[text()='{email}']/following-sibling::td[not(@style='display:none')][1] | //div[@id='{UserTabId}']//table//tbody//tr/td[text()='{email}']/following-sibling::td[not(@style='display:none')][1]/font");
        private static By SendEmailIcon(string email) => By.XPath($"//td[contains(text(),'{email}')]/following-sibling::td/a[@title='Send Email']");

        // Add User Popup
        private readonly By FirstNameTextbox = By.Id("FirstName");
        private readonly By LastNameTextbox = By.Id("LastName");
        private readonly By EmailTextbox = By.Id("Email");
        private readonly By PreferredLanguageDropDown = By.XPath("//label[contains(normalize-space(),'Preferred Language')]//parent::div//following-sibling::div//span[contains(@class,'k-input')]");
        private readonly By AllLanguagesList = By.XPath("//ul[@id='IsoLanguageCode_listbox']//li[@role='option']");
        private readonly By UploadPhotoField = By.ClassName("avatar-photo");
        private static By SelectPreferredLanguage(string language) => By.XPath($"//div//ul[@id='IsoLanguageCode_listbox']//li[text()='{language}'] | //div//ul[@id='IsoLanguageCode_listbox']//li//font[text()='{language}']");
        private readonly By NotifyUserCheckbox = By.Id("NotifyUser");
        private readonly By FeatureAdminCheckbox = By.Id("FeatureAdmin");
        private readonly By Photo = By.Id("Avatar");
        private readonly By SaveAndCloseButton = By.XPath("//a[contains(text(), 'Save and Close')] | //font[contains(text(), 'Save and Close')]/ancestor::a");
        private readonly By SaveAndAddNewButton = By.XPath("//a[contains(text(), 'Save and Add New')] | //font[text()='Save and Add New']");
        private readonly By CancelButton = By.CssSelector(".k-edit-buttons .k-grid-cancel");
        private readonly By MtFilterBox = By.Id("multiteamFilterBox1");
        private readonly By MtSelector = By.CssSelector("#gridAllMultiTeams .chkbxAllMultiGrid");
        private readonly By CanSeeTeamNamesLeader = By.Id("CanSeeTeamNamesLeader");
        private readonly By CanSeeSubTeamsLeader = By.Id("CanSeeSubTeamsLeader");
        private readonly By TagFilterBox = By.Id("TagFilterBox1");
        private readonly By TagSelector = By.CssSelector("#gridAllTags .chkbxAllGrid");
        private readonly By TeamFilterBox = By.Id("teamAdminFilterBox1");
        private readonly By CompanyFilterBox = By.Id("teamFilterBox1");
        private readonly By TeamSelector = By.CssSelector("#gridAllTeams .chkbxAllGrid");
        private readonly By CompanySelector = By.CssSelector("#gridAllCompanies .chkbxAllGrid");
        private readonly By LinkedInSelector = By.Id("LinkedIn");
        private readonly By BioSelector = By.Id("Bio");
        private readonly By DoneUploadingLabel = By.XPath("//strong[contains(@class,'k-upload-status-total')][contains(.,'Done')]");
        private readonly By AhfCheckbox = By.Id("activeAhf");
        private readonly By AhfValue = By.XPath("//input[@name='ActiveAHF' and @data-bind='value:ActiveAHF']");
        private readonly By AhTrainerCheckbox = By.Id("AHTrainer");
        private readonly By CertificationTextBox = By.XPath("//select[@id='Certifications']//parent::div//div");
        private static By SelectedCompanyCheckbox(string companyName) => By.XPath($"//div[@id = 'gridSelectedCompanies']//td[text() = ' {companyName}']/input");
        private static By UserPopupTitle => By.XPath("//div[@class='k-edit-form-container']//ancestor::div//div[@class='k-window-titlebar k-header']//span[contains(normalize-space(),'Add')]");
        private static By FieldValidationMessage(string fieldName) => By.Id($"{fieldName}_validationMessage");

        //Edit User Permission
        private static By EditPermissionPopUpEditSectionsExpandButton(string sectionName) => By.XPath($"//p[normalize-space()='Section: {sectionName}']//a[@class='k-icon k-i-expand']");
        private static By EditPermissionPopUpPermissionCheckbox(string feature) => By.XPath($"//span[contains(normalize-space(),'{feature}')]//parent::td//preceding-sibling::td//input[@class='chkPermission']");
        private readonly By EditPermissionButton = By.XPath("//div[contains(@id,'Grid')]//a[normalize-space()='Permissions']");
        private readonly By EditPermissionPopUpUpdateButton = By.Id("updatePermissions");
        private readonly By EditPermissionPopUpConfirmationCloseButton = By.Id("permissionClose");
        private readonly By EditPermissionPopUpCloseButton = By.XPath("//span[contains(text(),'Permissions')]//parent::div//div//a//span[text()='Close'] | //*[contains(text(),'Permissions')]//..//..//parent::div//div//a//span[@class = 'k-icon k-i-close']");
        private static By AllPermissionSectionsExpandButton => By.XPath("//p[contains(@class,'k-reset')]");
        private static By AllPermissionCheckboxes => By.XPath("//span//parent::td//preceding-sibling::td//input[@class='chkPermission' and not(@disabled)]");
        private static By ExpandSectionIcon(string sectionName) => By.XPath($"//p[text()='{sectionName}']//a[@class='k-icon k-i-expand']");
        private static By CollapseSectionIcon(string sectionName) => By.XPath($"//p[text()='{sectionName}']//a[@class='k-icon k-i-collapse']");
        private readonly By PermissionPopUpCloseIcon = By.XPath("//span[@id='permission_dialog_window_wnd_title']/..//div//a//span[@role='presentation']");

        //ManageIndividualsTab
        private static By ResetPasswordButton(string email) => By.XPath($"//div[@id = 'IndividualGrid']//td[text() = '{email.ToLower()}']/..//input[@class='resetPasswordBtn notranslate']");
        private readonly By FilterTextbox = By.Id("individualAdminFilterBox");
        protected readonly By SendingEmailPopupTitle = By.Id("email_sending_dialog_wnd_title");

        // Manage User Tab
        public void SelectTab()
        {
            Log.Step(nameof(ManageUserPage), "Select user tab");
            Wait.UntilJavaScriptReady();
            Wait.UntilJavaScriptReady();
            Wait.UntilElementClickable(UserTab).Click();
            Wait.UntilJavaScriptReady();
        }
        public bool IsUserTabDisplayed()
        {
            Log.Step(nameof(ManageUserPage), "Is user tab displayed?");
            return Driver.IsElementDisplayed(UserTab);
        }
        public void Search(string searchTerm)
        {
            Wait.UntilElementClickable(SearchTextbox).SetText(searchTerm);
            Wait.UntilElementClickable(GoButton).Click();
            Wait.UntilJavaScriptReady();
        }
        public void ClickOnCloseButton()
        {
            Log.Step(nameof(ManageUserPage),"Click on close button.");
            Wait.UntilElementClickable(CloseButton).Click();
        }

        public void ChangeAhfCheckboxValue(string searchText, bool value)
        {
            Log.Step(nameof(ManageUserPage), $"Active/InActive AHF checkbox based on value = {value}");
            Search(searchText);
            Wait.UntilElementClickable(EditIcon).Click();
            Wait.UntilJavaScriptReady();
            if (value == (Wait.UntilElementExists(AhfValue).GetElementAttribute("value") == "false"))
            {
                Wait.UntilElementClickable(AhfCheckbox).Click();
            }
            Wait.UntilElementClickable(SaveAndCloseButton).Click();
        }
        public void ClickOnImpersonateButton(string email)
        {
            Log.Step(nameof(ManageUserPage), "Click on Impersonate button");
            Wait.UntilElementClickable(ImpersonateButton(email)).Click();
        }
        public bool IsImpersonateButtonPresent(string email)
        {
            return Driver.IsElementPresent(ImpersonateButton(email));
        }

        public void ClickOnAddNewUserButton()
        {
            Log.Step(nameof(ManageUserPage), "Click on 'Add New' button");
            Wait.UntilJavaScriptReady();
            Wait.UntilElementVisible(AddNewUserButton);
            Wait.UntilElementClickable(AddNewUserButton).Click();
            Wait.UntilJavaScriptReady();
        }
        public void SearchUserOnUserTab(string searchUser)
        {
            Log.Step(nameof(ManageUserPage), $"Search '{searchUser}' user on tab");
            Wait.UntilElementClickable(SearchFilterTextbox).SetText(searchUser);
            Wait.UntilJavaScriptReady();
        }
        public void ClearSearchUserBox()
        {
            Log.Step(nameof(ManageUserPage), "Clear user search box.");
            Wait.UntilElementClickable(SearchFilterTextbox).Clear();
        }
        public void ClickOnEditUserIcon(string email)
        {
            Log.Step(nameof(ManageUserPage), "Click on Edit Company Admin icon");
            Wait.UntilJavaScriptReady(); // wait for the table to load
            Wait.UntilElementVisible(EditUserIcon(email));
            Wait.UntilElementClickable(EditUserIcon(email)).Click();
            Wait.UntilJavaScriptReady();
        }

        public void ClickOnEditUserIconFromGlobalSearch()
        {
            Log.Step(nameof(ManageUserPage), "Click on edit user icon from global search.");
            Wait.UntilElementVisible(EditIcon);
            Wait.UntilElementClickable(EditIcon).Click();
            Wait.HardWait(3000); // Wait until page load
        }
        public string ClickOnDeleteUserIcon(string email)
        {
            Log.Step(nameof(ManageUserPage), "Click on Edit Company Admin icon");
            Wait.UntilJavaScriptReady(); // wait for the table to load
            Wait.UntilElementClickable(DeleteUserIcon(email)).Click();
            var alertMessage = Driver.GetAlertMessage();
            Driver.AcceptAlert();
            return alertMessage;
        }
        public bool IsEditUserPageDisplayed()
        {
            Log.Step(nameof(ManageUserPage),"Is edit user page displayed?");
            return Driver.IsElementDisplayed(EmailTextbox);
        }
        
        public bool IsUserDisplayed(string email)
        {
            return Driver.IsElementDisplayed(UserEmail(email));
        }
        public bool IsUserExist(string imagePath, string firstName, string lastName, string email, string addedDate)
        {
            Wait.UntilJavaScriptReady();
            return Driver.IsElementDisplayed(UserCreatedDate(imagePath, firstName, lastName, email, addedDate));
        }
        public string GetLastLogin(string firstName, string lastName, string email)
        {

            return Wait.UntilElementClickable(LastLogin(firstName, lastName, email)).GetText();
        }
        public string GetAvatar(string email) => Wait.UntilElementVisible(AvatarImage(email)).GetElementAttribute("src");
        public string GetUserAddedDateFromGrid(string email)
        {
            Wait.HardWait(2000); //Need to wait until date is visible
            return Wait.UntilElementVisible(UserAddedDate(email)).GetText();
        }

        public void ClickOnSendEmailIcon(string email)
        {
            Log.Step(nameof(ManageUserPage), "Click on the 'Send Email' icon");
            Wait.UntilElementClickable(SendEmailIcon(email)).Click();
        }

        public void NavigateToPage(int companyId)
        {
            NavigateToUrl($"{BaseTest.ApplicationUrl}/user/{companyId}", ManageUserPageTitle);
        }

        // Add User Popup
        public void CheckUncheckAhFCheckboxValue(bool value)
        {
            Log.Step(nameof(ManageUserPage), $"Active/InActive AHF checkbox based on value = {value}");
            Wait.UntilJavaScriptReady();
            Wait.UntilElementVisible(AhfCheckbox).Check(value);
        }
        public void CheckUncheckAhTrainerCheckboxValue(bool value)
        {
            Log.Step(nameof(ManageUserPage), $"Active/InActive AHF checkbox based on value = {value}");
            Wait.UntilJavaScriptReady();
            Wait.UntilElementVisible(AhTrainerCheckbox).Check(value);
        }
        public bool IsAhTrainerCheckboxSelected()
        {
            Log.Step(nameof(ManageUserPage), "Is Ah Trainer checkbox selected?");
            return Driver.IsElementSelected(AhTrainerCheckbox);
        }
        public bool IsAhfCheckboxSelected()
        {
            Log.Step(nameof(ManageUserPage), "Is Ah Trainer checkbox selected?");
            return Driver.IsElementSelected(AhfCheckbox);
        }
        public bool IsAhTrainerDisplayed()
        {
            return Driver.IsElementDisplayed(AhTrainerCheckbox);
        }
        public bool IsActiveAhfDisplayed()
        {
            return Driver.IsElementDisplayed(AhfCheckbox);
        }
        public bool IsCertificationDisplayed()
        {
            return Driver.IsElementDisplayed(CertificationTextBox);
        }
        public string GetSelectedPreferredLanguage()
        {
            return Wait.UntilElementVisible(PreferredLanguageDropDown).GetText();
        }
        public bool IsPreferredLanguageDisplayed()
        {
            return Driver.IsElementDisplayed(PreferredLanguageDropDown);
        }
        public List<string> GetAllLanguages()
        {
            Log.Step(nameof(ManageUserPage), "Get All Languages from Language dropdown");
            Wait.UntilElementClickable(PreferredLanguageDropDown).Click();
            Wait.UntilJavaScriptReady();
            var getHeaderLanguageAllValue = Driver.GetTextFromAllElements(AllLanguagesList).ToList();
            Wait.UntilElementClickable(PreferredLanguageDropDown).Click();
            return getHeaderLanguageAllValue;
        }
        public void ClickSaveAndCloseButton()
        {
            Log.Step(nameof(ManageUserPage), "Click on 'Save and Close' button");
            Wait.UntilJavaScriptReady();
            Wait.UntilElementVisible(SaveAndCloseButton).Click();
            Wait.UntilJavaScriptReady();
        }
        public void ClickSaveAndAddNewButton()
        {
            Log.Step(nameof(ManageUserPage), "Click on 'Save and Add New' button");
            Wait.UntilJavaScriptReady();
            Wait.UntilElementVisible(SaveAndAddNewButton).Click();
            Wait.HardWait(1000);     // Wait until 'Add New User' 
        }
        public void ClickCancelButton()
        {
            Log.Step(nameof(ManageUserPage), "Click on 'Cancel' button");
            Wait.UntilElementVisible(CancelButton).Click();
            Wait.UntilJavaScriptReady();
        }
        public void UploadPhoto(string filePath)
        {
            Wait.UntilJavaScriptReady();
            Wait.UntilElementExists(Photo).SetText(filePath, false);
            Wait.HardWait(3000);  // Wait until image load
            Wait.UntilElementExists(DoneUploadingLabel);
        }
        public bool IsUserPopupPresent()    
        {
            return Driver.IsElementDisplayed(UserPopupTitle);
        }
        public bool IsFieldValidationMessageDisplayed(string fieldName)
        {
            return Driver.IsElementDisplayed(FieldValidationMessage(fieldName));
        }

        public bool IsUploadPhotoFieldDisplayed()
        {
            return Driver.IsElementDisplayed(UploadPhotoField);
        }
        //Edit User Permission
        public void EditPermission(string sectionName, string featureName, bool featureEnabled = true)
        {
            Log.Step(nameof(ManageUserPage), $"Edit permission for {sectionName} and {featureName}, and setting value : {featureEnabled}");
            Wait.UntilAllElementsLocated(EditPermissionButton).First(e => e.Displayed).Click();
            if (!Driver.IsElementDisplayed(CollapseSectionIcon($"Section: {sectionName}")))
            {
                Wait.UntilElementClickable(EditPermissionPopUpEditSectionsExpandButton(sectionName)).Click();
            }
            Wait.UntilElementVisible(EditPermissionPopUpPermissionCheckbox(featureName)).Check(featureEnabled);
            Wait.UntilElementClickable(EditPermissionPopUpUpdateButton).Click();
            Wait.UntilAllElementsLocated(EditPermissionPopUpConfirmationCloseButton).First(e => Wait.UntilElementVisible(e) != null).Click();
            Wait.UntilElementClickable(EditPermissionPopUpCloseButton).Click();
        }

        public void SelectDeselectAllPermissions(bool featureEnabled = true)
        {
            Log.Step(nameof(ManageUserPage), "Select/Deselect all permission");
            ExpandAllPermissionSections();
            var elements = Wait.UntilAllElementsLocated(AllPermissionCheckboxes);
            foreach (var element in elements)
            {
                Driver.JavaScriptScrollToElement(element);
                element.Check(featureEnabled);
                Wait.UntilJavaScriptReady();
            }
            Wait.UntilElementClickable(EditPermissionPopUpUpdateButton).Click();
            Wait.UntilAllElementsLocated(EditPermissionPopUpConfirmationCloseButton).First(e => Wait.UntilElementVisible(e) != null).Click();
            Wait.UntilElementClickable(EditPermissionPopUpCloseButton).Click();
        }
        public bool AreAllPermissionsSelected()
        {
            Log.Step(GetType().Name, "Is all permission checkbox selected?");
            return Wait.UntilAllElementsLocated(AllPermissionCheckboxes).All(a => a.Selected);
        }
        public void ExpandAllPermissionSections()
        {
            Log.Step(nameof(ManageUserPage), "Expand all permission sections");
            Wait.UntilJavaScriptReady();

            // Open Permission popup
            Wait.UntilAllElementsLocated(EditPermissionButton).First(e => e.Displayed).Click();
            Wait.UntilJavaScriptReady();

            // Getting all permission's section name list and expand every section
            var getAllPermissionSectionNames = Wait.UntilAllElementsLocated(AllPermissionSectionsExpandButton).Select(e => e.GetText()).ToList();
            foreach (var sectionName in getAllPermissionSectionNames.Where(sectionName => !Driver.IsElementPresent(CollapseSectionIcon(sectionName))))
            {
                Wait.UntilElementVisible(ExpandSectionIcon(sectionName)).Click();
            }
        }
        public void PermissionPopupClickOnCloseIcon()
        {
            Log.Step(nameof(ManageUserPage), "Click on 'Close' icon");
            Wait.UntilElementVisible(PermissionPopUpCloseIcon).Click();
        }

        // Company Admin
        public void EnterCompanyAdminInfo(CompanyAdminInfo companyAdminInfo)
        {
            Log.Step(nameof(ManageUserPage), "Enter Company Admin info");
            Wait.UntilElementVisible(FirstNameTextbox).SetText(companyAdminInfo.FirstName);
            Wait.UntilElementVisible(LastNameTextbox).SetText(companyAdminInfo.LastName);
            Wait.UntilElementVisible(EmailTextbox).SetText(companyAdminInfo.Email);
            if (!string.IsNullOrEmpty(companyAdminInfo.PreferredLanguage))
            {
                SelectItem(PreferredLanguageDropDown, SelectPreferredLanguage(companyAdminInfo.PreferredLanguage));
            }
            Wait.UntilElementVisible(NotifyUserCheckbox).Check(companyAdminInfo.NotifyUser);

            Wait.UntilElementVisible(FeatureAdminCheckbox).Check(companyAdminInfo.FeatureAdmin);
            CheckUncheckAhFCheckboxValue(companyAdminInfo.ActiveAhf);
            CheckUncheckAhTrainerCheckboxValue(companyAdminInfo.AhTrainer);

            if (string.IsNullOrEmpty(companyAdminInfo.ImagePath)) return;
            UploadPhoto(companyAdminInfo.ImagePath);

        }
        public void EditCompanyAdminInfo(CompanyAdminInfo companyAdminInfo)
        {
            Log.Step(nameof(ManageUserPage), "Edit Company Admin info");
            Wait.UntilElementVisible(FirstNameTextbox);
            Driver.ExecuteJavaScript("document.getElementById(\"FirstName\").value = \"\";");
            Wait.UntilJavaScriptReady();
            Wait.UntilElementVisible(FirstNameTextbox).SetText(companyAdminInfo.FirstName, false);
            Driver.ExecuteJavaScript("document.getElementById(\"LastName\").value = \"\";");
            Wait.UntilJavaScriptReady();
            Wait.UntilElementVisible(LastNameTextbox).SetText(companyAdminInfo.LastName, false);
            Driver.ExecuteJavaScript("document.getElementById(\"Email\").value = \"\";");
            Wait.UntilJavaScriptReady();
            Wait.UntilElementVisible(EmailTextbox).SetText(companyAdminInfo.Email, false);
            if (!string.IsNullOrEmpty(companyAdminInfo.PreferredLanguage))
            {
                SelectItem(PreferredLanguageDropDown, SelectPreferredLanguage(companyAdminInfo.PreferredLanguage));
            }
            if (string.IsNullOrEmpty(companyAdminInfo.ImagePath)) return;
            UploadPhoto(companyAdminInfo.ImagePath);
        }

        // BL Admin
        public void EnterBlAdminInfo(BlAdminInfo blAdminInfo, bool isBlAdmin = false)
        {
            Log.Step(nameof(ManageUserPage), "Enter BL Admin info");
            Wait.UntilElementVisible(EmailTextbox);
            Wait.UntilElementVisible(EmailTextbox).SetText(blAdminInfo.Email, false);
            Wait.UntilElementVisible(FirstNameTextbox).SetText(blAdminInfo.FirstName, false);
            Wait.UntilElementVisible(LastNameTextbox).SetText(blAdminInfo.LastName, false);
            if (!string.IsNullOrEmpty(blAdminInfo.PreferredLanguage))
            {
                SelectItem(PreferredLanguageDropDown, SelectPreferredLanguage(blAdminInfo.PreferredLanguage));
            }

            Wait.UntilElementClickable(NotifyUserCheckbox).Check(blAdminInfo.NotifyUser);


            if (!string.IsNullOrEmpty(blAdminInfo.ImagePath))
            {
                UploadPhoto(blAdminInfo.ImagePath);
            }

            Wait.UntilElementVisible(TagSelector);
            if (!string.IsNullOrEmpty(blAdminInfo.Tag))
            {
                SelectTag(blAdminInfo.Tag);
            }
            if (isBlAdmin) return;
            CheckUncheckAhFCheckboxValue(blAdminInfo.ActiveAhf);
            CheckUncheckAhTrainerCheckboxValue(blAdminInfo.AhTrainer);
        }
        public void SelectTag(string tag)
        {
            Wait.UntilElementVisible(TagFilterBox).SetText(tag);
            Wait.UntilJavaScriptReady();
            Wait.UntilElementVisible(TagSelector).Click();
            Wait.UntilJavaScriptReady();
        }
       
        public void EditBlAdminInfo(BlAdminInfo blAdminInfo, bool isBlAdmin = false)
        {
            Log.Step(nameof(ManageUserPage), "Edit BL Admin info");
            Wait.UntilElementVisible(EmailTextbox);
            Driver.ExecuteJavaScript("document.getElementById(\"Email\").value = \"\";");
            Wait.UntilJavaScriptReady();
            Wait.UntilElementVisible(EmailTextbox).SetText(blAdminInfo.Email, false);
            Driver.ExecuteJavaScript("document.getElementById(\"FirstName\").value = \"\";");
            Wait.UntilJavaScriptReady();
            Wait.UntilElementVisible(FirstNameTextbox).SetText(blAdminInfo.FirstName, false);
            Driver.ExecuteJavaScript("document.getElementById(\"LastName\").value = \"\";");
            Wait.UntilJavaScriptReady();
            Wait.UntilElementVisible(LastNameTextbox).SetText(blAdminInfo.LastName, false);
            if (!string.IsNullOrEmpty(blAdminInfo.PreferredLanguage))
            {
                SelectItem(PreferredLanguageDropDown, SelectPreferredLanguage(blAdminInfo.PreferredLanguage));
            }
            if (!string.IsNullOrEmpty(blAdminInfo.ImagePath))
            {
                UploadPhoto(blAdminInfo.ImagePath);
            }
            if (isBlAdmin) return;
            CheckUncheckAhFCheckboxValue(blAdminInfo.ActiveAhf);
            CheckUncheckAhTrainerCheckboxValue(blAdminInfo.AhTrainer);
        }
        // Team Admin
        public void EnterTeamAdminInfo(TeamAdminInfo teamAdminInfo)
        {
            Log.Step(nameof(ManageUserPage), "Enter Team Admin info");
            if (IsUserDisplayed(teamAdminInfo.Email))
            {
                ClickOnDeleteUserIcon(teamAdminInfo.Email);
            }
            Wait.UntilElementVisible(EmailTextbox);
            Wait.UntilElementVisible(EmailTextbox).SetText(teamAdminInfo.Email, false);
            Wait.UntilElementVisible(FirstNameTextbox).SetText(teamAdminInfo.FirstName, false);
            Wait.UntilElementVisible(LastNameTextbox).SetText(teamAdminInfo.LastName, false);

            if (!string.IsNullOrEmpty(teamAdminInfo.PreferredLanguage))
            {
                SelectItem(PreferredLanguageDropDown, SelectPreferredLanguage(teamAdminInfo.PreferredLanguage));
            }
            Wait.UntilElementClickable(NotifyUserCheckbox).Check(teamAdminInfo.NotifyUser);

            if (!string.IsNullOrEmpty(teamAdminInfo.ImagePath))
            {
                UploadPhoto(teamAdminInfo.ImagePath);
            }

            Wait.UntilElementVisible(TeamSelector);
            if (!string.IsNullOrEmpty(teamAdminInfo.Team))
            {
                SelectTeam(teamAdminInfo.Team);
            }

            CheckUncheckAhFCheckboxValue(teamAdminInfo.ActiveAhf);
            CheckUncheckAhTrainerCheckboxValue(teamAdminInfo.AhTrainer);
        }
        public void EditTeamAdminInfo(TeamAdminInfo teamAdminInfo)
        {
            Log.Step(nameof(ManageUserPage), "Edit Team Admin info");
            Wait.UntilElementVisible(EmailTextbox);
            Driver.ExecuteJavaScript("document.getElementById(\"Email\").value = \"\";");
            Wait.UntilJavaScriptReady();
            Wait.UntilElementVisible(EmailTextbox).SetText(teamAdminInfo.Email, false);
            Driver.ExecuteJavaScript("document.getElementById(\"FirstName\").value = \"\";");
            Wait.UntilJavaScriptReady();
            Wait.UntilElementVisible(FirstNameTextbox).SetText(teamAdminInfo.FirstName, false);
            Driver.ExecuteJavaScript("document.getElementById(\"LastName\").value = \"\";");
            Wait.UntilJavaScriptReady();
            Wait.UntilElementVisible(LastNameTextbox).SetText(teamAdminInfo.LastName, false);
            if (!string.IsNullOrEmpty(teamAdminInfo.PreferredLanguage))
            {
                SelectItem(PreferredLanguageDropDown, SelectPreferredLanguage(teamAdminInfo.PreferredLanguage));
            }
            if (string.IsNullOrEmpty(teamAdminInfo.ImagePath)) return;
            UploadPhoto(teamAdminInfo.ImagePath);
            CheckUncheckAhFCheckboxValue(teamAdminInfo.ActiveAhf);
            CheckUncheckAhTrainerCheckboxValue(teamAdminInfo.AhTrainer);

            Wait.UntilElementVisible(TeamSelector);
            if (!string.IsNullOrEmpty(teamAdminInfo.Team))
            {
                SelectTeam(teamAdminInfo.Team);
            }
        }
        public void SelectTeam(string teamName)
        {
            Wait.UntilElementVisible(TeamFilterBox).SetText(teamName);
            Wait.UntilJavaScriptReady();
            Wait.UntilElementVisible(TeamSelector).Click();
            Wait.UntilJavaScriptReady();
        }

        // Organizational Leaders
        public void EnterOlInfo(OrganizationalLeadersInfo organizationalLeadersInfo)
        {
            Log.Step(nameof(ManageUserPage), "Enter Org Leader info");
            if (IsUserDisplayed(organizationalLeadersInfo.Email))
            {
                ClickOnDeleteUserIcon(organizationalLeadersInfo.Email);
            }
            Wait.UntilElementVisible(EmailTextbox).SetText(organizationalLeadersInfo.Email, false);
            Wait.UntilElementVisible(FirstNameTextbox).SetText(organizationalLeadersInfo.FirstName, false);
            Wait.UntilElementVisible(LastNameTextbox).SetText(organizationalLeadersInfo.LastName, false);
            if (!string.IsNullOrEmpty(organizationalLeadersInfo.PreferredLanguage))
            {
                SelectItem(PreferredLanguageDropDown, SelectPreferredLanguage(organizationalLeadersInfo.PreferredLanguage));
            }
            if (organizationalLeadersInfo.NotifyUser)
            {
                Wait.UntilElementClickable(NotifyUserCheckbox).Check();
            }
            else
            {
                Wait.UntilElementClickable(NotifyUserCheckbox).Check(false);
            }

            if (organizationalLeadersInfo.CanSeeTeamName)
            {
                Wait.UntilElementClickable(CanSeeTeamNamesLeader).Check();

                if (organizationalLeadersInfo.CanViewSubteams)
                {
                    Wait.UntilElementClickable(CanSeeSubTeamsLeader).Check();
                }
                else
                {
                    Wait.UntilElementClickable(CanSeeSubTeamsLeader).Check(false);
                }
            }
            else
            {
                Wait.UntilElementClickable(CanSeeTeamNamesLeader).Check(false);
            }

            if (!string.IsNullOrEmpty(organizationalLeadersInfo.ImagePath))
            {
                UploadPhoto(organizationalLeadersInfo.ImagePath);
            }

            Wait.UntilElementVisible(MtSelector);
            SelectMtTeam(organizationalLeadersInfo.Team);

            CheckUncheckAhFCheckboxValue(organizationalLeadersInfo.ActiveAhf);
            CheckUncheckAhTrainerCheckboxValue(organizationalLeadersInfo.AhTrainer);
        }

        public void EnterUserInfo(AhUser userInfo, UserType user)
        {
            Log.Step(nameof(ManageUserPage), "Enter user info");
            if (IsUserDisplayed(userInfo.Email))
            {
                ClickOnDeleteUserIcon(userInfo.Email);
            }
            Wait.UntilElementVisible(EmailTextbox).SetText(userInfo.Email, false);
            Wait.UntilElementVisible(FirstNameTextbox).SetText(userInfo.FirstName, false);
            Wait.UntilElementVisible(LastNameTextbox).SetText(userInfo.LastName, false);
            if (userInfo.NotifyUser)
            {
                Wait.UntilElementClickable(NotifyUserCheckbox).Check();
            }
            else
            {
                Wait.UntilElementClickable(NotifyUserCheckbox).Check(false);
            }

            if (!string.IsNullOrEmpty(userInfo.FilterBy) && user == UserType.TeamAdmin)
            {
                Wait.UntilElementVisible(TeamSelector);
                SelectTeam(userInfo.FilterBy);
            }
            else if (!string.IsNullOrEmpty(userInfo.FilterBy) && user == UserType.OrganizationalLeader)
            {
                Wait.UntilElementVisible(MtSelector);
                SelectMtTeam(userInfo.FilterBy);
            }
            else if (!string.IsNullOrEmpty(userInfo.FilterBy) && user == UserType.BusinessLineAdmin)
            {
                Wait.UntilElementVisible(TagSelector);
                SelectTag(userInfo.FilterBy);
            }
        }
        public void EditOlInfo(OrganizationalLeadersInfo organizationalLeadersInfo)
        {
            Log.Step(nameof(ManageUserPage), "Edit Org Leader info");
            Wait.UntilElementVisible(EmailTextbox);
            Driver.ExecuteJavaScript("document.getElementById(\"Email\").value = \"\";");
            Wait.UntilJavaScriptReady();
            Wait.UntilElementVisible(EmailTextbox).SetText(organizationalLeadersInfo.Email, false);
            Driver.ExecuteJavaScript("document.getElementById(\"FirstName\").value = \"\";");
            Wait.UntilJavaScriptReady();
            Wait.UntilElementVisible(FirstNameTextbox).SetText(organizationalLeadersInfo.FirstName, false);
            Driver.ExecuteJavaScript("document.getElementById(\"LastName\").value = \"\";");
            Wait.UntilJavaScriptReady();
            Wait.UntilElementVisible(LastNameTextbox).SetText(organizationalLeadersInfo.LastName, false);
            if (!string.IsNullOrEmpty(organizationalLeadersInfo.PreferredLanguage))
            {
                SelectItem(PreferredLanguageDropDown, SelectPreferredLanguage(organizationalLeadersInfo.PreferredLanguage));
            }
            if (organizationalLeadersInfo.CanSeeTeamName)
            {
                Wait.UntilElementVisible(CanSeeTeamNamesLeader).Check();

                if (organizationalLeadersInfo.CanViewSubteams)
                {
                    Wait.UntilElementVisible(CanSeeSubTeamsLeader).Check();
                }
                else
                {
                    Wait.UntilElementVisible(CanSeeSubTeamsLeader).Check(false);
                }
            }
            else
            {
                Wait.UntilElementVisible(CanSeeSubTeamsLeader).Check(false);
                Wait.UntilElementVisible(CanSeeTeamNamesLeader).Check(false);
            }

            CheckUncheckAhFCheckboxValue(organizationalLeadersInfo.ActiveAhf);
            CheckUncheckAhTrainerCheckboxValue(organizationalLeadersInfo.AhTrainer);

            if (string.IsNullOrEmpty(organizationalLeadersInfo.ImagePath)) return;
            UploadPhoto(organizationalLeadersInfo.ImagePath);

        }
        public void SelectMtTeam(string team)
        {
            Wait.UntilElementVisible(MtFilterBox);
            Wait.UntilElementClickable(MtFilterBox).Clear();
            Wait.UntilJavaScriptReady();
            Wait.UntilElementClickable(MtFilterBox).SetText(team, false);
            Wait.UntilJavaScriptReady();
            Wait.UntilElementVisible(MtSelector).Click();
            Wait.UntilJavaScriptReady();
        }

        // Coach
        public void EnterCoachInfo(CoachInfo coachInfo)
        {
            Log.Step(nameof(ManageUserPage), "Enter Coach info");
            Wait.UntilElementVisible(FirstNameTextbox).SetText(coachInfo.FirstName);
            Wait.UntilElementVisible(LastNameTextbox).SetText(coachInfo.LastName);
            Wait.UntilElementVisible(EmailTextbox).SetText(coachInfo.Email);
            if (!string.IsNullOrEmpty(coachInfo.PreferredLanguage))
            {
                SelectItem(PreferredLanguageDropDown, SelectPreferredLanguage(coachInfo.PreferredLanguage));
            }
            Wait.UntilElementVisible(LinkedInSelector).SetText(coachInfo.LinkedIn);
            if (!string.IsNullOrEmpty(coachInfo.ImagePath))
            {
                UploadPhoto(coachInfo.ImagePath);
            }

            Wait.UntilElementVisible(BioSelector).SetText(coachInfo.Bio);

            CheckUncheckAhFCheckboxValue(coachInfo.ActiveAhf);
            CheckUncheckAhTrainerCheckboxValue(coachInfo.AhTrainer);
        }
        public void EditCoachInfo(CoachInfo coachInfo)
        {
            Log.Step(nameof(ManageUserPage), "Edit Coach info");
            Wait.UntilElementVisible(FirstNameTextbox);
            Driver.ExecuteJavaScript("document.getElementById(\"FirstName\").value = \"\";");
            Wait.UntilJavaScriptReady();
            Wait.UntilElementVisible(FirstNameTextbox).SetText(coachInfo.FirstName, false);
            Driver.ExecuteJavaScript("document.getElementById(\"LastName\").value = \"\";");
            Wait.UntilJavaScriptReady();
            Wait.UntilElementVisible(LastNameTextbox).SetText(coachInfo.LastName, false);
            Driver.ExecuteJavaScript("document.getElementById(\"Email\").value = \"\";");
            Wait.UntilJavaScriptReady();
            Wait.UntilElementVisible(EmailTextbox).SetText(coachInfo.Email, false);
            if (!string.IsNullOrEmpty(coachInfo.PreferredLanguage))
            {
                SelectItem(PreferredLanguageDropDown, SelectPreferredLanguage(coachInfo.PreferredLanguage));
            }
            if (string.IsNullOrEmpty(coachInfo.ImagePath)) return;
            UploadPhoto(coachInfo.ImagePath);

            CheckUncheckAhFCheckboxValue(coachInfo.ActiveAhf);
            CheckUncheckAhTrainerCheckboxValue(coachInfo.AhTrainer);
        }

        // Partner Admin
        public void EnterPartnerAdminInfo(PartnerAdminInfo partnerAdmin)
        {
            Log.Step(nameof(ManageUserPage), "Enter Part Admin info");
            Wait.UntilElementVisible(FirstNameTextbox).SetText(partnerAdmin.FirstName);
            Wait.UntilElementVisible(LastNameTextbox).SetText(partnerAdmin.LastName);
            Wait.UntilElementVisible(EmailTextbox).SetText(partnerAdmin.Email);
            if (!string.IsNullOrEmpty(partnerAdmin.PreferredLanguage))
            {
                SelectItem(PreferredLanguageDropDown, SelectPreferredLanguage(partnerAdmin.PreferredLanguage));
            }
            if (!string.IsNullOrEmpty(partnerAdmin.LinkedIn))
            {
                Wait.UntilElementVisible(LinkedInSelector).SetText(partnerAdmin.LinkedIn);
            }

            if (partnerAdmin.NotifyUser)
            {
                Wait.UntilElementVisible(NotifyUserCheckbox).Check();
            }
            else
            {
                Wait.UntilElementVisible(NotifyUserCheckbox).Check(false);
            }

            CheckUncheckAhFCheckboxValue(partnerAdmin.ActiveAhf);
            CheckUncheckAhTrainerCheckboxValue(partnerAdmin.AhTrainer);

            if (string.IsNullOrEmpty(partnerAdmin.ImagePath)) return;
            UploadPhoto(partnerAdmin.ImagePath);

        }
        public void AddCompanies(List<string> companies)
        {
            foreach (var company in companies)
            {
                SelectCompany(company);
            }
        }
        public void RemoveCompanies(List<string> companies)
        {
            foreach (var company in companies)
            {
                Wait.UntilElementClickable(SelectedCompanyCheckbox(company)).Check(false);
            }
        }
        public void SelectCompany(string companyName)
        {
            Wait.UntilElementVisible(CompanyFilterBox).SetText(companyName);
            Wait.UntilJavaScriptReady();
            Wait.UntilElementVisible(CompanySelector).Click();
            Wait.UntilJavaScriptReady();
        }
        public void EditPartnerAdminInfo(PartnerAdminInfo partnerAdmin)
        {
            Log.Step(nameof(ManageUserPage), "Edit Partner Admin info");
            if (!string.IsNullOrEmpty(partnerAdmin.FirstName))
            {
                Wait.UntilElementVisible(FirstNameTextbox).SetText(partnerAdmin.FirstName);
            }

            if (!string.IsNullOrEmpty(partnerAdmin.LastName))
            {
                Wait.UntilElementVisible(LastNameTextbox).SetText(partnerAdmin.LastName);
            }

            if (!string.IsNullOrEmpty(partnerAdmin.Email))
            {
                Wait.UntilElementVisible(EmailTextbox).SetText(partnerAdmin.Email);
            }

            if (!string.IsNullOrEmpty(partnerAdmin.PreferredLanguage))
            {
                SelectItem(PreferredLanguageDropDown, SelectPreferredLanguage(partnerAdmin.PreferredLanguage));
            }

            if (!string.IsNullOrEmpty(partnerAdmin.LinkedIn))
            {
                Wait.UntilElementVisible(LinkedInSelector).SetText(partnerAdmin.LinkedIn);
            }

            if (!string.IsNullOrEmpty(partnerAdmin.ImagePath))
            {
                UploadPhoto(partnerAdmin.ImagePath);
            }

            CheckUncheckAhTrainerCheckboxValue(partnerAdmin.AhTrainer);
            CheckUncheckAhFCheckboxValue(partnerAdmin.ActiveAhf);
            CheckUncheckAhTrainerCheckboxValue(partnerAdmin.AhTrainer);
            CheckUncheckAhFCheckboxValue(partnerAdmin.ActiveAhf);
        }

        //ManageIndividualsTab
        public void ClickResetPasswordButton(string email)
        {
            Wait.UntilElementClickable(ResetPasswordButton(email.ToLower())).Click();
            Wait.UntilElementVisible(SendingEmailPopupTitle);
            Wait.UntilElementInvisible(SendingEmailPopupTitle);
        }
        public void FilterGrid(string searchTerm)
        {
            Wait.UntilElementClickable(FilterTextbox).SendKeys(searchTerm + Keys.Enter);
            Wait.UntilJavaScriptReady();
            Wait.UntilJavaScriptReady();
        }
    }
}