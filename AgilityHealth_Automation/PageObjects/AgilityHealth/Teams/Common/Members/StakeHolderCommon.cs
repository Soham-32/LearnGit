using System;
using System.IO;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.DataObjects;
using AgilityHealth_Automation.Utilities;
using AtCommon.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Common.Members
{
    public class StakeHolderCommon : MemberCommon
    {
        public StakeHolderCommon(IWebDriver driver, ILogger log) : base(driver, log) { }

        private readonly By AddNewStakeHolderButton = By.XPath("//a[contains(@id,'add-stakeholder')]");
        private readonly By AddNewStakeholderPopupTitle = By.XPath("//div//span[@role='heading'][text()='Add New Stakeholder' or text()= 'Add Stakeholder'][not(@id)] | //div//span[@role='heading']//font[text()='Add New Stakeholder' or text()= 'Add Stakeholder'][not(@id)]");

        // grid
        private static By FirstNameFromGrid(int index) => By.XPath($"//div[@id='StakeholdersGrid']/table/tbody/tr[{index}]/td[2]");
        private static By LastNameFromGrid(int index) => By.XPath($"//div[@id='StakeholdersGrid']/table/tbody/tr[{index}]/td[3]");
        private static By EmailFromGrid(int index) => By.XPath($"//div[@id='StakeholdersGrid']/table/tbody/tr[{index}]/td[4]");
        private static By RoleFromGrid(int index) => By.XPath($"//div[@id='StakeholdersGrid']/table/tbody/tr[{index}]/td[5]");
        private static By EditStakeHolderButtonFromGrid(int index) => By.XPath($"//div[@id='StakeholdersGrid']/table/tbody/tr[{index}]/td[6]/a[contains(@class,'k-grid-edit')]");

        public void ClickAddNewStakeHolderButton()
        {
            Log.Step(GetType().Name, "On Stakeholder tab, click on the 'Add Stakeholder' button");
            Wait.UntilJavaScriptReady();
            Wait.UntilElementClickable(AddNewStakeHolderButton).Click();
        }

        public void ClickStakeHolderEditButton(int index)
        {
            Log.Step(GetType().Name, $"On Stakeholder tab, click Edit button at index {index}");
            Wait.UntilElementClickable(EditStakeHolderButtonFromGrid(index)).Click();
        }

        public void EnterStakeHolderInfo(StakeHolderInfo stakeHolderInfo, string operationType = "Add")
        {
            Log.Step(GetType().Name, $"On Edit Stakeholder popup, enter Stakeholder info for {stakeHolderInfo.Email}");
            if (Driver.IsInternetExplorer())
            {
                if (operationType.Equals("Edit"))
                {
                    //Clearing fields
                    Wait.UntilElementClickable(EmailTextBox).SetText("");
                    Wait.UntilJavaScriptReady();
                    Wait.UntilElementClickable(LastNameTextBox).SetText("");
                    Wait.UntilJavaScriptReady();
                    Wait.UntilElementClickable(FirstNameTextBox).SetText("");
                    Wait.UntilJavaScriptReady();
                }
                else
                {
                    //Moving cursour to First name field
                    Wait.UntilElementClickable(FirstNameTextBox).SendKeys(Keys.Tab);
                }

                //Using external .exe to fill data. EXE is generated via AutoIT
                var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\AutoIt\\Teams\\TeamMemberStakeholderAddEdit.exe");
                var commandLineArguments = $"{operationType}*Stakeholders" + " " + stakeHolderInfo.FirstName + " " + stakeHolderInfo.LastName + " " + stakeHolderInfo.Email;
                CSharpHelpers.RunExternalExe(filePath, commandLineArguments);
            }
            else
            {
                Wait.UntilElementVisible(FirstNameTextBox);
                Wait.UntilElementClickable(FirstNameTextBox).SetText(stakeHolderInfo.FirstName);
                Wait.UntilJavaScriptReady();
                Wait.UntilElementClickable(LastNameTextBox).SetText(stakeHolderInfo.LastName);
                Wait.UntilJavaScriptReady();
                Wait.UntilElementClickable(EmailTextBox).SetText(stakeHolderInfo.Email);
                Wait.UntilJavaScriptReady();
            }

            //Hiding Email validation message if present. At times, it gets displayed eventhough we've entered email already.
            //Due to validation message, roles can't deleted or added.
            HideEmailValidationMessage();

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

        public StakeHolderInfo GetStakeHolderInfoFromGrid(int rowIndex)
        {
            Wait.HardWait(2000);//Takes time to load data
            var stakeHolderInfo = new StakeHolderInfo
            {
                FirstName = Wait.UntilElementVisible(FirstNameFromGrid(rowIndex)).Text,
                LastName = Wait.UntilElementVisible(LastNameFromGrid(rowIndex)).Text,
                Email = Wait.UntilElementVisible(EmailFromGrid(rowIndex)).Text,
                Role = Wait.UntilElementVisible(RoleFromGrid(rowIndex)).Text,
            };

            return stakeHolderInfo;
        }

        public bool IsAddNewStakeholderPopupDisplayed()
        {
            return Driver.IsElementDisplayed(AddNewStakeholderPopupTitle);
        }

        public void NavigateToPage(int teamId)
        {
            Log.Step(GetType().Name, $"Navigate to team id {teamId}");
            NavigateToUrl($"{BaseTest.ApplicationUrl}/stakeholders/edit/{teamId}");
        }
    }
}
