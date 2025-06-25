using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.DataObjects;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.MultiTeam.Edit
{
    internal class EditMtStakeHolderPage : BasePage
    {

        public EditMtStakeHolderPage(IWebDriver driver, ILogger log) : base(driver, log) { }

        private readonly By DeleteRoleSymbol = By.XPath("//ul[@id='Role_taglist']/li/span[@class='k-select']");

        private readonly By FirstNameTextBox = By.Id("FirstName");
        private readonly By LastNameTextBox = By.Id("LastName");
        private readonly By EmailTextBox = By.Id("Email");
        private readonly By RoleListBox = By.CssSelector("input[aria-owns='Role_taglist Role_listbox']");
        public readonly By UpdateButton = By.CssSelector("a.k-grid-update");
        public readonly By UploadTeamMemberPhoto = By.Id("file");
        private static By StakeholderEditButton(int index) => By.XPath(
            $"//div[@id='StakeholdersGrid']/table/tbody/tr[{index}]/td[6]/a[contains(@class,'k-grid-edit')]");
        private static By RoleListItem(string role) => By.XPath($"//ul[@id='Role_listbox']/li[text()='{role}'] | //ul[@id='Role_listbox']/li//font[text()='{role}']");

        public void DeleteRole()
        {
            if (Driver.IsElementPresent(DeleteRoleSymbol))
            {
                Wait.UntilElementClickable(DeleteRoleSymbol).Click();
            }
        }
        public void EnterStakeHolderInfo(StakeHolderInfo stakeHolderInfo)
        {
            Log.Step(nameof(EditMtStakeHolderPage), "On Edit MT Stakeholder popup, enter Stakeholder info");
            Wait.UntilElementClickable(EmailTextBox).SetText(stakeHolderInfo.Email);
            Wait.UntilJavaScriptReady();
            Wait.UntilElementClickable(FirstNameTextBox).SetText(stakeHolderInfo.FirstName);
            Wait.UntilJavaScriptReady();
            Wait.UntilElementClickable(LastNameTextBox).SetText(stakeHolderInfo.LastName);
            Wait.UntilJavaScriptReady();

            if (!string.IsNullOrEmpty(stakeHolderInfo.Role))
            {
                DeleteRole();
                SelectRole(stakeHolderInfo.Role);
            }

            if (!string.IsNullOrEmpty(stakeHolderInfo.ImagePath))
            {
                UploadPhoto(stakeHolderInfo.ImagePath);
            }
        }

        public void SelectRole(string role)
        {
            SelectItem(RoleListBox, RoleListItem(role));
        }

        public void UploadPhoto(string filePath)
        {
            Wait.UntilElementClickable(UploadTeamMemberPhoto).SendKeys(filePath);
        }

        public void ClickUpdateButton()
        {
            Log.Step(nameof(EditMtStakeHolderPage), "On Edit MT Stakeholder popup, click Update button");
            Wait.UntilElementClickable(UpdateButton).Click();
            Wait.UntilElementNotExist(UpdateButton);
        }

        public void ClickStakeHolderEditButton(int index)
        {
            Log.Step(nameof(EditMtStakeHolderPage), $"On Edit MT Stakeholder page, click edit Stakeholder button at index {index}");
            Wait.UntilElementClickable(StakeholderEditButton(index)).Click();
        }

    }
}
