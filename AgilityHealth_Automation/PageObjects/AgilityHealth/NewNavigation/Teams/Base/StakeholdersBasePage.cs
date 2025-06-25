using System.Threading;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.NewNavigation.Teams.Base
{
    public class StakeholdersBasePage : CommonGridBasePage
    {
        public CommonAddEditMembersPopupBasePage PopupBase { get; set; }

        public StakeholdersBasePage(IWebDriver driver, ILogger log) : base(driver, log)
        {
            PopupBase = new CommonAddEditMembersPopupBasePage(driver, log);
        }

        #region Locators

        #region Add New Stakeholder dropdown button and dropdown options
        private readonly By AddStakeholdersDropdown = By.XPath("//div[@id='StakeholdersGrid']//ul[@id='ddMenuBulkOperations']");
        private readonly By AddNewStakeHoldersOption = By.XPath("//div[@id='StakeholdersGrid']//ul//li/a[@id='add-stakeholder']");
        private readonly By StakeholdersFromDirectoryButton = By.XPath("//div[@id='StakeholdersGrid']//ul//li/a[@onclick='ShowCompanyLookUpDialogsh()']");
        #endregion

        #region Stakeholder Stepper

        private readonly By ValidationMessageForRole = By.Id("mySpan");
        private static By StakeholderEditButton(string email) => By.XPath($"//td[text()='{email}']/following-sibling::td//span[normalize-space()='Edit']");
        private readonly By DeleteMenuItem = By.CssSelector(".k-popup li[id^='menush_Delete']");
        private static By StakeholderDropdownButton(string email) => By.XPath($"//div[@id='StakeholdersGrid']/table/tbody/tr/td[text()='{email}']//following-sibling::td[3]/ul/li[contains(@class,'k-last')]");
        #endregion

        #region Stakeholder/Leader Stepper
        private readonly By ContinueToReviewButton = By.XPath("//div[@class='contents']//following-sibling::a[normalize-space()='Continue to Review']");
        #endregion

        #region Leaders Tab

        private readonly By LeadersTab = By.XPath("//a[text()='Leaders']/..");
        private static By LeadersActionDropdownButton(string email) => By.XPath($"//div[@id='TeamMembersGrid']/table/tbody/tr/td[text()='{email}']//following-sibling::td[3]/ul/li[contains(@class,'k-last')]");
        private readonly By LeaderDeleteMenuItem = By.CssSelector("li[id^='menu_Delete']");
        private static By LeaderCheckbox(string email) => By.CssSelector($"[name='memberId'][value='{email}']");
        private readonly By RemoveSelectedButton = By.Id("rem-t-member-btn");
        #endregion

        #endregion



        #region Methods

        #region Add New Stakeholder dropdown button
        public void ClickOnAddStakeholdersDropdown()
        {
            Log.Step(nameof(StakeholdersBasePage), "Click on 'Stakeholder' Dropdown");
            Wait.UntilElementClickable(AddStakeholdersDropdown).Click();
        }
        public void ClickOnAddNewStakeHolderOption()
        {
            Log.Step(nameof(StakeholdersBasePage), "Click on 'Add New Stakeholder' option");
            Wait.UntilElementClickable(AddNewStakeHoldersOption).Click();
        }
        public void OpenAddStakeholdersPopup()
        {
            Log.Step(nameof(StakeholdersBasePage), "Click on 'Add Team Stakeholders' dropdown then 'Add New Stakeholder' option");
            ClickOnAddStakeholdersDropdown();
            ClickOnAddNewStakeHolderOption();
        }
        #endregion

        #region Select Stakeholder(s) From Directory
        public void OpenSelectStakeholdersFromDirectoryPopup()
        {
            Log.Step(nameof(StakeholdersBasePage), "Click on 'Add Stakeholder' Dropdown then 'Stakeholder Directory' option");
            ClickOnAddStakeholdersDropdown();
            ClickOnStakeholdersFromDirectoryOption();
            Wait.HardWait(2000); //Need to wait until popup loaded successfully
        }
        public void ClickOnStakeholdersFromDirectoryOption()
        {
            Log.Step(nameof(StakeholdersBasePage), "Click on 'Stakeholder From Directory' option");
            Wait.UntilElementClickable(StakeholdersFromDirectoryButton).Click();
        }
        #endregion

        #region Stakeholder Stepper

        public void ClickOnStakeholderEditButton(string email)
        {
            Log.Step(nameof(StakeholdersBasePage), "Click on 'Stakeholder Edit' button");
            Wait.UntilElementVisible(StakeholderEditButton(email));
            Driver.JavaScriptScrollToElement(StakeholderEditButton(email));
            Thread.Sleep(500);// Wait till Edit button is enabled
            Wait.UntilElementClickable(StakeholderEditButton(email)).Click();
        }
        public bool IsValidationMessageDisplayed()
        {
            Thread.Sleep(1000);//Wait till 'Validation message' is displayed
            return Driver.IsElementDisplayed(ValidationMessageForRole);
        }

        public string GetValidationMessageForRole()
        {
            return Wait.UntilElementVisible(ValidationMessageForRole).GetText();
        }

        public void DeleteStakeholder(string email)
        {
            Log.Step(nameof(StakeholdersBasePage), $"Delete a stakeholder with {email}");
            Wait.UntilElementVisible(StakeholderDropdownButton(email)).Click();
            Wait.UntilElementVisible(DeleteMenuItem).Click();
            Driver.AcceptAlert();
            Wait.UntilJavaScriptReady();
        }
        #endregion

        #region Stakeholder/Leader Stepper
        public void ClickOnContinueToReviewButton()
        {
            Log.Step(nameof(StakeholdersBasePage), "Click on 'Continue To Review' button");
            var button = Wait.UntilElementVisible(ContinueToReviewButton);
            Driver.JavaScriptScrollToElement(button);
            Thread.Sleep(500); // Wait till 'Continue To Review' button is enabled
            Wait.UntilElementClickable(button).Click();
            Wait.UntilJavaScriptReady();
        }
        #endregion

        #region Leaders Tab

        public void ClickOnLeadersTab()
        {
            Log.Step(nameof(StakeholdersBasePage), "Click on 'Leaders' tab");
            Wait.UntilElementClickable(LeadersTab).Click();
        }

        public void ClickOnLeaderCheckbox(string email)
        {
            Log.Step(nameof(StakeholdersBasePage), "Click on 'Leader' checkbox");
            Wait.UntilElementVisible(LeaderCheckbox(email)).Click();
        }

        public void DeleteLeader(string email)
        {
            Log.Step(nameof(StakeholdersBasePage), $"Delete a team member with email {email}");
            Wait.UntilElementVisible(LeadersActionDropdownButton(email)).Click();
            Wait.UntilElementVisible(LeaderDeleteMenuItem).Click();
            Driver.AcceptAlert();
            Wait.UntilJavaScriptReady();
        }

        public void RemoveSelectedLeader()
        {
            Log.Step(nameof(StakeholdersBasePage), "Click on 'Removed Selected' button, then confirm removing");
            Wait.UntilElementVisible(RemoveSelectedButton).Click();
            Driver.AcceptAlert();
            Wait.UntilJavaScriptReady();
        }
        #endregion

        #endregion
    }
}
