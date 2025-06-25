using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.ManageTeamTags
{
    internal class ManageTagsPage : BasePage
    {
        public ManageTagsPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        //Company Selection
        private readonly By CompanyDropdown = By.XPath("//span[@class='k-icon k-i-arrow-s']");
        private readonly By ApplyButton = By.XPath("//span[text()='Apply']");
        private readonly By CompanySearchListBox = By.XPath("//input[@aria-owns='Companies_listbox']");

        //Tabs
        private readonly By TeamMembersTab = By.XPath("//a[text()='Team Members']");
        private readonly By StakeholdersTab = By.XPath("//a[text()='Stakeholders']");

        //Teams Tab
        private static By TeamsTag(string tagName, string parentTagName) => By.XPath($"//*[@id='TagContainerCompanyTeamCategory']//tr//td[text()='{tagName}']//following-sibling::td[text()='{parentTagName}']//parent::tr | //*[@id='TagContainerCompanyTeamCategory']//tr//td[text()='{tagName}']//following-sibling::td//font[text()='{parentTagName}']//ancestor::tr");
        private static By TeamsCategoryNameAndType(string categoryName, string type) => By.XPath($"//*[@id='TagCategoryCompanyTeamCategory']//tr//td[text()='{categoryName}']//following-sibling::td[text()='{type}']//parent::tr");

        //Team Members Tab
        private static By TeamMemberCategoryNameAndType(string categoryName, string type) => By.XPath($"//*[@id='TagCategoryCompanyTeamMemberCategory']//tr//td[text()='{categoryName}']//following-sibling::td[text()='{type}']//parent::tr");
        private static By TeamMemberTag(string tagName, string parentTagName) =>
            By.XPath(
                $"//*[@id='TagCompanyTeamMemberCategory']//tr//td[text()='{tagName}']//following-sibling::td//following-sibling::td//following-sibling::td[text()='{parentTagName}']//parent::tr");

        //Stakeholders Tab
        private static By StakeholderCategoryAndType(string categoryName, string type) => By.XPath($"//*[@id='TagCategoryCompanyStakeholderCategory']//tr//td[text()='{categoryName}']//following-sibling::td[text()='{type}']//parent::tr");

        private static By StakeholderTags(string tagName) => By.XPath($"//*[@Id='TagCompanyStakeholderCategory']//tr//td[text()='{tagName}']//parent::tr");


        //Company Selection
        public void SelectCompany(string companyName)
        {
            Log.Step(nameof(ManageTagsPage), "Select a company");
            Wait.UntilElementClickable(CompanyDropdown).Click();
            Wait.UntilElementVisible(CompanySearchListBox).SetText(companyName);
            Wait.UntilJavaScriptReady();
            Wait.UntilElementClickable(ApplyButton).Click();
        }

        //Tabs
        public void ClickOnTeamMembersTab()
        {
            Log.Step(nameof(ManageTagsPage), "Click On TeamMembers Tab");
            Wait.UntilElementClickable(TeamMembersTab).Click();
        }

        public void ClickOnStakeHoldersTab()
        {
            Log.Step(nameof(ManageTagsPage), "Click On Stakeholders Tab");
            Wait.UntilElementClickable(StakeholdersTab).Click();
        }

        //Teams Tab
        public bool IsTeamsCategoryPresent(string categoryName, string type)
        {
            Log.Step(nameof(ManageTagsPage), $"Verify '{categoryName}{type}' team category present or not");
            Wait.UntilElementVisible(TeamsCategoryNameAndType(categoryName, type));
            return Driver.IsElementDisplayed(TeamsCategoryNameAndType(categoryName, type));
        }
        public bool IsTeamTagPresent(string tagName, string parentTagName)
        {
            Log.Step(nameof(ManageTagsPage), $"Verify '{tagName}{parentTagName}' team tag is present or not");
            return Driver.IsElementDisplayed(TeamsTag(tagName, parentTagName));
        }
        public void ClickByTeamsCategoryNameAndType(string categoryName, string type)
        {
            Log.Step(nameof(ManageTagsPage), "Click By Team Category Name & Type");
            Wait.UntilElementClickable(TeamsCategoryNameAndType(categoryName, type)).Click();
            Wait.UntilJavaScriptReady();
        }

        //Team Members Tab
        public bool IsTeamMemberCategoryDisplayed(string categoryName, string type)
        {
            Log.Step(nameof(ManageTagsPage), $"Verify '{categoryName}{type}' team member category present or not");
            Wait.UntilElementVisible(TeamMemberCategoryNameAndType(categoryName, type));
            return Driver.IsElementDisplayed(TeamMemberCategoryNameAndType(categoryName, type));
        }
        public bool IsTeamMemberTagsPresents(string tagName, string parentTagName)
        {
            Log.Step(nameof(ManageTagsPage), $"Verify '{tagName}{parentTagName}' team member tag is present or not");
            return Driver.IsElementDisplayed(TeamMemberTag(tagName, parentTagName));
        }
        public void ClickByTeamMemberCategoryNameAndType(string categoryName, string type)
        {
            Log.Step(nameof(ManageTagsPage), "Click By Team Member Category Name & Type");
            Wait.UntilElementClickable(TeamMemberCategoryNameAndType(categoryName, type)).Click();
            Wait.UntilJavaScriptReady();
        }

        //Stakeholders Tab
        public bool IsStakeholderCategoryDisplayed(string categoryName, string type)
        {
            Log.Step(nameof(ManageTagsPage), $"Verify '{categoryName}{type}' stakeholder category present or not");
            Wait.UntilElementVisible(StakeholderCategoryAndType(categoryName, type));
            return Driver.IsElementDisplayed(StakeholderCategoryAndType(categoryName, type));
        }
        public bool IsStakeholderTagsPresents(string tagName)
        {
            Log.Step(nameof(ManageTagsPage), $"Verify '{tagName}' stakeholder tag is present or not");
            return Driver.IsElementDisplayed(StakeholderTags(tagName),15);     
        }
        public void ClickByStakeholderCategoryNameAndType(string categoryName, string type)
        {
            Log.Step(nameof(ManageTagsPage), "Click By Stakeholder Category Name & Type");
            Wait.UntilElementClickable(StakeholderCategoryAndType(categoryName, type)).Click();
            Wait.UntilJavaScriptReady();
        }

        //Common
        public void NavigateToPage(int companyId)
        {
            Log.Step(nameof(ManageTagsPage), "Navigate To Manage Tags Page");
            NavigateToUrl($"{BaseTest.ApplicationUrl}/tags/company/{companyId}");
        }
    }

}
