using System;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Common.Members;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Create
{
    internal class SelectTeamMembersPage : MemberCommon
    {
        public SelectTeamMembersPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        private readonly By SelectAllCheckbox = By.ClassName("masterContact");
        private readonly By NextSelectStakeholdersButton = By.Id("btnTeamMembers");
        private readonly By TeamMemberLoadIcon = By.XPath("//div[@class='k-loading-image']");
        private static By TeamMemberCheckbox(int rowIndex) => 
            By.XPath($"//table[@class='persondata-table']//tr[{rowIndex}]/td/input");

        private readonly By TableRows = By.XPath("//table[@class='persondata-table']//tr");
        private static By TeamMemberName(int rowIndex) =>
            By.XPath($"//table[contains(@class,'persondata-table')]//tr[{rowIndex}]/td[2]");

        public void WaitForTeamMembersPageToLoad()
        {
            Wait.UntilElementNotExist(TeamMemberLoadIcon);
        }

        public void SelectAllTeamMembers()
        {
            Log.Step(nameof(SelectTeamMembersPage), "Select All Team Members");
            Driver.MoveToElement(Wait.UntilElementClickable(SelectAllCheckbox)).Check();
        }

        public void ClickOnNextSelectStakeholdersButton()
        {
            Log.Step(nameof(SelectTeamMembersPage), "Click on Next, Select Stakeholder button");
            Wait.UntilElementClickable(NextSelectStakeholdersButton).Click();
            Wait.UntilJavaScriptReady();
        }

        public void SelectTeamMemberByName(string name)
        {
            Log.Step(nameof(SelectTeamMembersPage), $"Select team member name {name}");
            int rowIndex = 0;
            int rowCount = Driver.GetElementCount(TableRows) - 1;
            for (int i = 1; i <= rowCount; i++)
            {
                string actualName = Wait.UntilElementVisible(TeamMemberName(i)).Text; 
                if (actualName.Equals(name))
                {
                    rowIndex = i;
                    break;
                }
            }

            if(rowIndex == 0)
            {
                throw new Exception($"The team member '{name}' was not found in the table");
            }

            Wait.UntilElementClickable(TeamMemberCheckbox(rowIndex)).Check();

        }
       
    }
}