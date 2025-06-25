using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;
using System.Collections.Generic;
using System.Linq;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Account
{
    public class ProfileInfo
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PreferredLanguage { get; set; }
        public string PhoneNumber { get; set; }
        public string MyBrand { get; set; }
        public string MySkill { get; set; }
        public string LinkedIn { get; set; }
        public string ValuePro { get; set; }
        public string PhotoPath { get; set; }
    }

    internal class AccountSettingsPage : BasePage
    {
        public AccountSettingsPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        #region Elements

        #region My Profile

        private readonly By FirstNameTextbox = By.Id("FirstName");
        private readonly By LastNameTextbox = By.Id("LastName");
        private readonly By PreferredLanguage = By.XPath("//span[@aria-owns='IsoLanguageCode_listbox']//span[@class='k-input']");
        private readonly By PhoneNumberTextbox = By.Id("PhoneNumber");
        private readonly By BrandInfoTextbox = By.Id("BrandInformation");
        private readonly By MySkillDropdown = By.CssSelector("input[aria-owns='MySkills_taglist MySkills_listbox']");
        private readonly By LinkedInTextbox = By.Id("LinkedIn");
        private readonly By ValuePropInfoTextbox = By.Id("ValuePropInformation");
        private readonly By PhotoInput = By.Id("file");
        private readonly By UpdateButton = By.Id("update_PersonalSection_client");
        private readonly By ResetLink = By.CssSelector("#reset a");
        private readonly By UploadPhotoField = By.ClassName("team-photo");
        private readonly By UploadingIndicator =
            By.XPath("//strong[contains(@class,'k-upload-status-total')][contains(.,'Done')]");
        private readonly By PageTitle = By.XPath("//div[@class='pg-title']//h2");

        private By DynamicSkillItem(string item) => By.XPath($"//ul[@id='MySkills_listbox']/li[text()='{item}']");
        private By DynamicTab(string item) => By.XPath($"//li[@role='tab']/a[text()='{item}']");
        private By DynamicAvailableTeam(string team) => By.XPath($"//div[@id='UnSelectedTeams']//td[text() ='{team}']");
        private By DynamicSelectedTeam(string team) => By.XPath($"//div[@id='SelectedTeams']//td[text() ='{team}']");

        private By DynamicPreferredLanguage(string language) => By.XPath($"//ul[@id='IsoLanguageCode_listbox']/li[text()='{language}']");
        private readonly By AllLanguagesList = By.XPath("//ul[@id='IsoLanguageCode_listbox']/li[@role='option']");

        //Key-customer verification
        #region Elements
        private readonly By MyProfileSectionText = By.XPath("//div[@id = 'pnlPersonalSection'] //h5");
        private readonly By TeamNameColumnText = By.XPath("//th[@data-title = 'Team Name']//a");
        #endregion

        #endregion

        #region My Notification

        private readonly By FilterAvailable = By.Id("filterUnSelected");
        private readonly By UpdateNotificationButton = By.CssSelector(".green-btn[value='Update']");
        private readonly By AllSelectedTeams = By.CssSelector("#SelectedTeams table tbody tr");

        #endregion

        #region Manage Password

        private readonly By PasswordPanel = By.Id("pnlPasswordSection");
        private readonly By CurrentPasswordTextbox = By.Id("OldPassword");
        private readonly By NewPasswordTextbox = By.Id("NewPassword");
        private readonly By ConfirmPasswordTextbox = By.Id("ConfirmPassword");
        private readonly By UpdatePasswordButton = By.Id("update_Password_client");
        private readonly By ErrorMessageLabel = By.Id("errorMessage");

        #endregion

        #endregion


        #region Methods

        #region My Profile

        public string GetFirstName()
        {
            return Wait.UntilElementVisible(FirstNameTextbox).GetElementAttribute("value");
        }

        public string GetPhoneNumber()
        {
            return Wait.UntilElementVisible(PhoneNumberTextbox).GetElementAttribute("value");
        }

        public string GetBrand()
        {
            return Wait.UntilElementVisible(BrandInfoTextbox).GetText();
        }

        public string GetPreferredLanguage()
        {
            return Wait.UntilElementVisible(PreferredLanguage).GetText();
        }

        public bool IsPreferredLanguageDisplayed()
        {
            return Driver.IsElementDisplayed(PreferredLanguage);
        }
        public bool IsUploadPhotoFieldDisplayed()
        {
            return Driver.IsElementDisplayed(UploadPhotoField);
        }
        public string GetLinkedIn()
        {
            return Wait.UntilElementVisible(LinkedInTextbox).GetElementAttribute("value");
        }

        public string GetValuePro()
        {
            return Wait.UntilElementVisible(ValuePropInfoTextbox).GetText();
        }

        public void ClickOnResetLink()
        {
            Wait.UntilElementClickable(ResetLink).Click();
        }

        public string GetErrorMessage()
        {
            return Wait.UntilElementVisible(ErrorMessageLabel).GetText();
        }

        public string GetPageTitle()
        {
            return Wait.UntilElementVisible(PageTitle).GetText();
        }

        public void EnterProfileInfo(ProfileInfo profileInfo)
        {
            Log.Step(nameof(AccountSettingsPage), "Enter profile info");
            Wait.UntilElementClickable(FirstNameTextbox).SetText(profileInfo.FirstName);
            Wait.UntilElementClickable(LastNameTextbox).SetText(profileInfo.LastName);
            SelectLanguage(profileInfo.PreferredLanguage);
            Wait.UntilElementClickable(PhoneNumberTextbox).SetText(profileInfo.PhoneNumber);
            Wait.UntilElementClickable(BrandInfoTextbox).SetText(profileInfo.MyBrand);
            //Broken currently, need to talk with Kyle on this issue.Can't add skill except Master data.Bug raised: Bug 7448: Settings > Tags > Skills - Don't see option for adding Skills
            // SelectSkill(profileInfo.MySkill);
            Wait.UntilElementClickable(LinkedInTextbox).SetText(profileInfo.LinkedIn);
            Wait.UntilElementClickable(ValuePropInfoTextbox).SetText(profileInfo.ValuePro);
            if (profileInfo.PhotoPath.Length > 0)
                UploadProfilePhoto(profileInfo.PhotoPath);
        }

        public void SelectSkill(string skill)
        {
            Wait.UntilElementClickable(MySkillDropdown).Click();
            Wait.UntilElementClickable(DynamicSkillItem(skill)).Click();
        }
        public void SelectLanguage(string language)
        {
            SelectItem(PreferredLanguage, DynamicPreferredLanguage(language));
        }

        public List<string> GetAllLanguages()
        {
            Log.Step(nameof(AccountSettingsPage), "Get All Languages from Language dropdown");
            Wait.UntilElementClickable(PreferredLanguage).Click();
            Wait.UntilJavaScriptReady();
            var getLanguageAllValue = Driver.GetTextFromAllElements(AllLanguagesList).ToList();
            Wait.UntilElementClickable(PreferredLanguage).Click();
            return getLanguageAllValue;
        }

        public void UploadProfilePhoto(string photoPath)
        {
            Wait.UntilJavaScriptReady();
            Driver.ExecuteJavaScript("$(\".team-photo ~ div:nth-child(4)\").show()");
            Wait.UntilElementExists(PhotoInput).SetText(photoPath, false);
            Wait.UntilElementVisible(UploadingIndicator);
            Wait.UntilJavaScriptReady();
            Driver.ExecuteJavaScript("$(\".team-photo ~ div:nth-child(4)\").hide()");
        }

        public void ClickUpdateButton()
        {
            Log.Step(nameof(AccountSettingsPage), "Click on Update button");
            Wait.UntilElementClickable(UpdateButton).Click();
        }

        //Key-customers verification
        #region Elements
        public string GetMyProfileSectionText()
        {
            Log.Step(nameof(AccountSettingsPage), "Get the My Profile section text");
            return Wait.UntilElementVisible(MyProfileSectionText).GetText();
        }

        public string GetMyTeamNameColumnText()
        {
            Log.Step(nameof(AccountSettingsPage), "Get the Team Name column text on My Notifications tab");
            return Wait.UntilElementVisible(TeamNameColumnText).GetText();
        }
        #endregion

        #endregion

        #region Common

        public void SelectTab(string tab)
        {
            Log.Step(nameof(AccountSettingsPage), $"Select tab <{tab}>");
            Wait.UntilElementClickable(DynamicTab(tab)).Click();
            Wait.UntilJavaScriptReady();
        }

        #endregion

        #region My Notifcation

        public void SelectAvailableTeam(string team)
        {
            Log.Step(nameof(AccountSettingsPage), $"Select available team <{team}>");
            Wait.UntilElementClickable(FilterAvailable).SetText(team);
            Wait.UntilElementClickable(DynamicAvailableTeam(team)).Click();
        }

        public bool DoesSelectedTeamDisplay(string team)
        {
            return Wait.InCase(DynamicSelectedTeam(team)) != null;
        }

        public void ClickUpdateNotificationButton()
        {
            Log.Step(nameof(AccountSettingsPage), "Click on Update Notification button");
            Wait.UntilElementClickable(UpdateNotificationButton).Click();
            Wait.UntilJavaScriptReady();
        }

        public void SelectSelectedTeam(string team)
        {
            Log.Step(nameof(AccountSettingsPage), $"Select selected team <{team}>");
            Wait.UntilElementClickable(DynamicSelectedTeam(team)).Click();
        }

        public void DeSelectAllSelectedTeam()
        {
            Log.Step(nameof(AccountSettingsPage), "Deselect all selected team");
            var allSelectedTeams = Wait.InCases(AllSelectedTeams);
            while (allSelectedTeams.Count != 0)
            {
                Wait.UntilElementClickable(allSelectedTeams[0]).Click();
                allSelectedTeams = Wait.InCases(AllSelectedTeams);
                Wait.UntilJavaScriptReady();
            }
        }

        #endregion

        #region Manage Password

        public void ClickPasswordPanel()
        {
            Log.Step(nameof(AccountSettingsPage), "Click on Password panel button");
            Wait.UntilElementClickable(PasswordPanel).Click();
        }

        public void UpdatePassword(string oldPassword = null, string newPassword = null, string confirmPassword = null)
        {
            Log.Step(nameof(AccountSettingsPage), $"Update password with old password <{oldPassword}>, new password <{newPassword}> and confirm password <{confirmPassword}>");
            Wait.UntilElementClickable(CurrentPasswordTextbox).Click();
            if (!string.IsNullOrEmpty(oldPassword))
                Wait.UntilElementVisible(CurrentPasswordTextbox).SetText(oldPassword);
            if (!string.IsNullOrEmpty(newPassword))
                Wait.UntilElementVisible(NewPasswordTextbox).SetText(newPassword);
            if (!string.IsNullOrEmpty(confirmPassword))
                Wait.UntilElementVisible(ConfirmPasswordTextbox).SetText(confirmPassword);
        }

        public void ClickUpdatePassword()
        {
            Log.Step(nameof(AccountSettingsPage), "Click on Update Password button");
            Wait.UntilJavaScriptReady();
            Wait.UntilElementClickable(UpdatePasswordButton).Click();
            Wait.UntilJavaScriptReady();
        }

        #endregion

        #endregion
    }
}