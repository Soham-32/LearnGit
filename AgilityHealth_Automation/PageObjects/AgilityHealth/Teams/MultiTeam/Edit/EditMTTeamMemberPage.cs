using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.DataObjects;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.MultiTeam.Edit
{
    internal class EditMtTeamMemberPage : BasePage
    {

        public EditMtTeamMemberPage(IWebDriver driver, ILogger log) : base(driver, log) { }


        private readonly By DeleteRoleSymbol = By.XPath("//label[normalize-space()='Role' or @for='Role']/following-sibling::div//li/span[@class='k-select']");
        private readonly By FirstNameTextBox = By.Id("FirstName");
        private readonly By LastNameTextBox = By.Id("LastName");
        private readonly By EmailTextBox = By.Id("Email");
        private readonly By RoleListBox = By.XPath("//label[normalize-space()='Role' or @for='Role']/following-sibling::div//input");
        private static By RoleListItem(string role) => By.XPath($"//ul[@id='Categories_0__SelectedTags_listbox']/li[text()='{role}'] | //ul[@id='Categories_0__SelectedTags_listbox']/li//font[text()='{role}']");
        private readonly By UpdateButton = By.CssSelector("a.k-grid-update");
        private readonly By UploadTeamMemberPhoto = By.Id("file");
        private static By TeamMemberEditButton(int index) => By.XPath($"//div[@id='TeamMembersGrid']/table/tbody/tr[{index}]/td[8]/a[contains(@class,'k-grid-edit')]");

        public void DeleteRole()
        {
            Log.Step(GetType().Name, "Delete Role");
            if (Driver.IsElementPresent(DeleteRoleSymbol))
            {
                Wait.UntilJavaScriptReady();
                Wait.UntilElementClickable(DeleteRoleSymbol).Click();
            }
        }

        public void EnterTeamMemberInfo(TeamMemberInfo memberInfo)
        {
            Wait.UntilElementClickable(FirstNameTextBox).SetText(memberInfo.FirstName);
            Wait.UntilElementClickable(LastNameTextBox).SetText(memberInfo.LastName);
            Wait.UntilElementClickable(EmailTextBox).SetText(memberInfo.Email);

            if (!string.IsNullOrEmpty(memberInfo.Role))
            {
                DeleteRole();
                SelectRole(memberInfo.Role);
            }

            if (!string.IsNullOrEmpty(memberInfo.ImagePath))
            {
                UploadPhoto(memberInfo.ImagePath);
            }
        }

        public void SelectRole(string role)
        {
            SelectItem(RoleListBox, RoleListItem(role));
        }

        public void UploadPhoto(string filePath) => Wait.UntilElementClickable(UploadTeamMemberPhoto).SendKeys(filePath);

        public void ClickUpdateButton()
        {
            Wait.UntilElementClickable(UpdateButton).Click();
            if (Driver.IsElementDisplayed(UpdateButton))
            {
                Wait.UntilElementClickable(UpdateButton).Click();
            }
            Wait.UntilElementNotExist(UpdateButton);
        }

        public void ClickTeamMemberEditButton(int index) => Wait.UntilElementClickable(TeamMemberEditButton(index)).Click();
    }
}
