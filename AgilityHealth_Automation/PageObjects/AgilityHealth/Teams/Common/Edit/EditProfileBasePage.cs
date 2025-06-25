using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Common.Edit
{
    public class EditProfileBasePage : BasePage
    {

        public EditProfileBasePage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        protected By PageHeaderTitle = By.CssSelector(".page-title-header span");
        protected By TeamNameTextbox = By.Id("TeamName");
        protected By WorkTypeTextbox = By.CssSelector("span[aria-owns='WorkTypeId_listbox'] span[class='k-input']");
        protected By PreferredLanguage = By.XPath("//span[@aria-owns='IsoLanguageCode_listbox']//span[@class='k-input']");
        protected By WorkTypeListItem(string item) => By.XPath($"//ul[@id='WorkTypeId_listbox']/li[text()='{item}'] | //ul[@id='WorkTypeId_listbox']/li//font[text()='{item}']");
        protected By DepartmentTextbox = By.Id("Department");
        protected By AgileAdoptionDateTextbox = By.Id("AgileAdoptionDate");
        protected const string AgileAdoptionDateId = "AgileAdoptionDate_dateview";
        protected By DateEstablishedTextbox = By.Id("TeamFormedDate");
        protected const string DateEstablishedId = "TeamFormedDate_dateview";
        protected By DescriptionTextbox = By.Id("Description");
        protected By BiographyTextbox = By.Id("Biography");
        protected By TeamImage = By.Id("preview");
        protected By ImagePathField = By.Id("file");
        protected By TeamImageClass = By.ClassName("team-photo");
        protected By UpdateTeamProfileButton = By.XPath("//input[@onclick='ClickDonePre()']");
        protected By ImageUploadDone = By.XPath("//strong[contains(@class,'k-upload-status-total')][contains(.,'Done')]");
        protected By AdvancedOptionsToggle = By.Id("openAdvancedOptions");
        protected By IntegrationOption = By.XPath("//h4[text()='Integrations']/following-sibling::div");
        protected By ExternalIdentifierTextbox = By.Id("ExternalIdentifier");
        protected static By PreferredLanguageItem(string preferredLanguage) => By.XPath($"//ul[@id='IsoLanguageCode_listbox']/li[contains(normalize-space(),'{preferredLanguage}')]");

        public void SelectWorkType(string workType)
        {
            SelectItem(WorkTypeTextbox, WorkTypeListItem(workType));
        }
        public void SelectPreferredLanguage(string preferredLanguage)
        {
            SelectItem(PreferredLanguage, PreferredLanguageItem(preferredLanguage));
        }
        public void ClickUpdateTeamProfileButton()
        {
            Log.Step(GetType().Name, "On Edit Team page, Team Profile tab, click Update Team Profile button");
            Wait.UntilElementClickable(UpdateTeamProfileButton).Click();
            Wait.UntilJavaScriptReady();
        }

        public string GetTeamImage()
        {
            return Wait.UntilElementExists(TeamImage).GetAttribute("src");
        }

        public void ClickAdvancedOptions()
        {
            Log.Step(GetType().Name, "On Edit Team page, expand Advanced option");
            Wait.UntilElementClickable(AdvancedOptionsToggle).Click();
            Wait.UntilJavaScriptReady();
        }

        public bool DoesIntegrationOptionDisplay()
        {
            var element = Wait.InCase(IntegrationOption);
            if (element != null)
            {
                return element.Displayed;
            }
            return false;
        }
        public string GetTeamProfilePageTitle()
        {
            Log.Step(GetType().Name, "On Edit Team page, get team profile page title");
            return Wait.UntilElementVisible(PageHeaderTitle).GetText();
        }

        public void NavigateToPage(int teamId)
        {
            Log.Step(GetType().Name, $"Navigate to team edit page, team id {teamId}");
            NavigateToUrl($"{BaseTest.ApplicationUrl}/teams/edit/{teamId}");
        }
    }
}
