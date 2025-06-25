using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.Utilities;
using AtCommon.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Team.Create
{
    public class TeamInfo
    {
        public TeamInfo()
        {
            Tags = new List<KeyValuePair<string, string>>();
        }
        public string TeamName { get; set; }
        public string WorkType { get; set; }
        public string PreferredLanguage { get; set; }
        public string Methodology { get; set; }
        public string ExternalIdentifier { get; set; }
        public string Department { get; set; }
        public string DateEstablished { get; set; }
        public string AgileAdoptionDate { get; set; }
        public string Description { get; set; }
        public string TeamBio { get; set; }
        public string ImagePath { get; set; }
        public List<KeyValuePair<string, string>> Tags { get; set; }
    }

    internal class CreateTeamPage : BasePage
    {
        public CreateTeamPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        //Create Team
        private readonly By TeamName = By.Id("TeamName");

        private readonly By WorkType = By.CssSelector("span[aria-owns='WorkType_listbox']");
        private readonly By PreferredLanguage = By.CssSelector("span[aria-owns='IsoLanguageCode_listbox']");
        private readonly By Methodology = By.CssSelector("span[aria-owns='Methodology_listbox']");
        private readonly By ExternalIdentifierTextBox = By.Id("ExternalIdentifier");

        private readonly By ExternalIdentifierListBox =
            By.CssSelector("span[aria-owns='ExternalIdentifierCategory_listbox']");
        private readonly By CreateTeamAndAddTeamMembersBtn = By.CssSelector("input.green-btn.done");
        private readonly By DepartmentTextbox = By.Id("Department");
        private readonly By DateEstablishedTextbox = By.Id("TeamFormedDate");
        private readonly By AgileAdoptionDateTextbox = By.Id("AgileAdoptionDate");
        private readonly By DescriptionTextbox = By.Id("Description");
        private readonly By TeamBioTextbox = By.Id("Biography");
        private readonly By ImagePathField = By.Id("file");
        private readonly By UploadPhotoField = By.ClassName("team-photo");
        private readonly By TeamImagePreview = By.Id("preview");
        private readonly By ImageUploadDone = By.XPath("//strong[contains(@class,'k-upload-status-total')][contains(.,'Done')]");
        private readonly By LicenseKeyListBox = By.CssSelector("span[aria-owns='CompanyLicenseId_listbox']");

        private static By WorkTypeItem(string workType) => By.XPath($"//ul[@id='WorkType_listbox']/li[text()='{workType}']");
        private static By PreferredLanguageItem(string preferredLanguage) => By.XPath($"//ul[@id='IsoLanguageCode_listbox']/li[text()='{preferredLanguage}']");
        private readonly By AllLanguagesList = By.XPath("//ul[@id='IsoLanguageCode_listbox']/li[@role='option']");

        private static By MethodologyItem(string methodology) =>
            By.XPath($"//ul[@id='Methodology_listbox']/li[text()='{methodology}']");

        private static By LicenseKeyItem(string key) =>
            By.XPath($"//ul[@id='CompanyLicenseId_listbox']/li[text()='{key}']");

        private readonly By BusinessLinesListBox = By.XPath("//label[contains(@for,'TagAdminTags') and contains(text(), 'Business Lines')]/..//div[contains(@class, 'k-floatwrap')]");
        private static By BusinessLinesListItem(string item) => By.XPath($"//ul[contains(@id, 'TagAdminTags')]/li[text() = '{item}'] | //ul[contains(@id, 'TagAdminTags')]/li//font[text() = '{item}']");
        private readonly By LicenseKeyValidation = By.Id("LicenseKeyValidate");

        private readonly By ExternalIdentifierValidationMessage = By.XPath("//span[@id='ExternalIdentifierValidate']");

        private readonly By WorkTypeList = By.XPath("//ul[@id='WorkType_listbox']//li");
        private readonly By TeamTagStrategicObjectivesField = By.XPath("//label[@for='TagAdminTags_0__Values']/..//div[@class='k-widget k-multiselect k-header']");
        private readonly By TeamTagStrategicObjectivesList = By.XPath("//ul[@id='TagAdminTags_0__Values_listbox']//li");
        private readonly By TeamTagCoachingField = By.XPath("//label[@for='TagAdminTags_1__Values']/..//div[@class='k-widget k-multiselect k-header']");
        private readonly By TeamTagCoachingList = By.XPath("//ul[@id='TagAdminTags_1__Values_listbox']//li");
        private readonly By TeamTagBusinessLineField = By.XPath("//label[@for='TagAdminTags_2__Values']/..//div[@class='k-widget k-multiselect k-header']");
        private readonly By TeamTagBusinessLineList = By.XPath("//ul[@id='TagAdminTags_2__Values_listbox']//li");

        //Upgrade your subscription pop up
        private readonly By UpgradeYourSubscriptionPopupHeaderText = By.XPath("//h1[@class='upgrade-subscription-title']");

        private readonly By TagsSection = By.XPath("//div[@class='row']//h4[contains(text(),'Team Tags')]");

        public readonly By SelectedTag =
            By.XPath("//div//ul[@class='k-reset']//li//span[not (@class)]");

        public void EnterTeamInfo(TeamInfo teamInfo)
        {
            Log.Step(GetType().Name, "On Add A Team page, enter team info");
            Wait.UntilElementClickable(TeamName).SetText(teamInfo.TeamName);

            SelectWorkType(teamInfo.WorkType);

            if (!string.IsNullOrEmpty(teamInfo.Methodology))
            {
                SelectMethodology(teamInfo.Methodology);
            }
            if (!string.IsNullOrEmpty(teamInfo.PreferredLanguage))
            {
                SelectPreferredLanguage(teamInfo.PreferredLanguage);
            }
            if (!string.IsNullOrEmpty(teamInfo.ExternalIdentifier))
            {
                Wait.UntilElementClickable(ExternalIdentifierTextBox).SetText(teamInfo.ExternalIdentifier);
            }
            if (!string.IsNullOrEmpty(teamInfo.Department))
            {
                Wait.UntilElementClickable(DepartmentTextbox).SetText(teamInfo.Department);
            }
            if (!string.IsNullOrEmpty(teamInfo.DateEstablished))
            {
                Wait.UntilElementClickable(DateEstablishedTextbox).SetText(teamInfo.DateEstablished);
            }
            if (!string.IsNullOrEmpty(teamInfo.AgileAdoptionDate))
            {
                Wait.UntilElementClickable(AgileAdoptionDateTextbox).SetText(teamInfo.AgileAdoptionDate);
            }
            if (!string.IsNullOrEmpty(teamInfo.Description))
            {
                Wait.UntilElementClickable(DescriptionTextbox).SetText(teamInfo.Description);
            }
            if (!string.IsNullOrEmpty(teamInfo.TeamBio))
            {
                Wait.UntilElementClickable(TeamBioTextbox).SetText(teamInfo.TeamBio);
            }
            if (!string.IsNullOrEmpty(teamInfo.ImagePath))
            {
                if (Driver.IsInternetExplorer())
                {
                    Wait.UntilElementExists(UploadPhotoField).Click();
                    Wait.UntilJavaScriptReady();
                    var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\AutoIt\\Teams\\TeamFileUpload.exe");
                    var commandLineArguments = teamInfo.ImagePath.Replace(" ", "*");
                    CSharpHelpers.RunExternalExe(filePath, commandLineArguments);
                }
                else
                {
                    Wait.UntilElementExists(ImagePathField).SendKeys(teamInfo.ImagePath);
                }
                Wait.UntilElementExists(ImageUploadDone);
            }

            foreach (var tag in teamInfo.Tags)
            {
                switch (tag.Key)
                {
                    case "Business Lines":
                        SelectItem(BusinessLinesListBox, BusinessLinesListItem(tag.Value));
                        break;
                    default:
                        throw new Exception($"'{tag.Key}' is not a recognized Tag type");
                }

            }
        }

        public void SelectWorkType(string workType)
        {
            SelectItem(WorkType, WorkTypeItem(workType));
        }
        public void SelectPreferredLanguage(string preferredLanguage)
        {
            SelectItem(PreferredLanguage, PreferredLanguageItem(preferredLanguage));
        }
        public List<string> GetAllLanguages()
        {
            Log.Step(nameof(CreateTeamPage), "Get All Languages from Language dropdown");
            Wait.UntilElementClickable(PreferredLanguage).Click();
            Wait.UntilJavaScriptReady();
            var getHeaderLanguageAllValue = Driver.GetTextFromAllElements(AllLanguagesList).ToList();
            Wait.UntilElementClickable(PreferredLanguage).Click();
            return getHeaderLanguageAllValue;
        }

        public void SelectMethodology(string methodology)
        {
            SelectItem(Methodology, MethodologyItem(methodology));
        }

        public void SelectLicenseKey(string key)
        {
            SelectItem(LicenseKeyListBox, LicenseKeyItem(key));
        }

        public bool DoesLicenseKeyBoxExist()
        {
            return Driver.IsElementDisplayed(LicenseKeyListBox);
        }

        public bool IsPreferredLanguageDisplayed()
        {
            return Driver.IsElementDisplayed(PreferredLanguage);
        }

        public void ClickCreateTeamAndAddTeamMembers()
        {
            Log.Step(GetType().Name, "On Add A Team page, click Create Team & Add Team Member button");
            Wait.UntilElementClickable(CreateTeamAndAddTeamMembersBtn).Click();
        }

        public string GetTeamImage()
        {
            return Wait.UntilElementExists(TeamImagePreview).GetElementAttribute("src");
        }

        public bool IsExternalIdentifierTextboxVisible()
        {
            return Wait.UntilElementExists(ExternalIdentifierTextBox).Displayed;
        }

        public bool IsExternalIdentifierListBoxVisible()
        {
            return Driver.IsElementPresent(ExternalIdentifierListBox) && Wait.UntilElementExists(ExternalIdentifierListBox).Displayed;
        }
        public string GetExternalIdentifierValidationMessage()
        {
            return Wait.UntilElementExists(ExternalIdentifierValidationMessage).GetText();
        }
        public bool IsLicenseKeyValidationVisible()
        {
            return Driver.IsElementDisplayed(LicenseKeyValidation);
        }
        public bool IsUploadPhotoFieldDisplayed()
        {
            return Driver.IsElementDisplayed(UploadPhotoField);
        }
        public List<string> GetWorkTypeDropdownList()
        {
            Log.Step(GetType().Name, "Get team's work type list");
            Wait.UntilElementClickable(WorkType).Click();
            Wait.UntilJavaScriptReady();
            return Wait.UntilAllElementsLocated(WorkTypeList).Select(row => row.GetText()).ToList();
        }
        public List<string> GetStrategicObjectivesDropdownList()
        {
            Log.Step(GetType().Name, "Get team's strategic objectives list");
            Wait.UntilElementClickable(TeamTagStrategicObjectivesField).Click();
            Wait.UntilJavaScriptReady();
            return Wait.UntilAllElementsLocated(TeamTagStrategicObjectivesList).Select(row => row.GetText()).ToList();
        }
        public List<string> GetCoachingDropdownList()
        {
            Log.Step(GetType().Name, "Get team's coaching list");
            Wait.UntilElementClickable(TeamTagCoachingField).Click();
            Wait.UntilJavaScriptReady();
            return Wait.UntilAllElementsLocated(TeamTagCoachingList).Select(row => row.GetText()).ToList();
        }
        public List<string> GetBusinessLineDropdownList()
        {
            Log.Step(GetType().Name, "Get team's business line list");
            Wait.UntilElementClickable(TeamTagBusinessLineField).Click();
            Wait.UntilJavaScriptReady();
            return Wait.UntilAllElementsLocated(TeamTagBusinessLineList).Select(row => row.GetText()).ToList();
        }

        public string GetUpgradeYourSubscriptionPopupHeaderText()
        {
            Log.Step(GetType().Name, "Get 'Upgrade Your Subscription' header text");
            return Wait.UntilElementVisible(UpgradeYourSubscriptionPopupHeaderText).GetText();
        }

        public bool IsTagsSectionDisplayed()
        {
            Log.Info("Is 'Team Tags' section displayed ?");
            return Driver.IsElementDisplayed(TagsSection);
        }

        public string GetSelectedTag()
        {
            return Wait.UntilElementVisible(SelectedTag).GetText();
        }
        public void NavigateToPage(string companyId)
        {
            Log.Step(GetType().Name, $"Navigate to company id {companyId}");
            NavigateToUrl($"{BaseTest.ApplicationUrl}/teams/{companyId}/create");
        }
    }
}