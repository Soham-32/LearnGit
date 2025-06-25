using System;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Common.Members;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Create
{
    internal class SelectStakeHolderPage : StakeHolderCommon
    {
        public SelectStakeHolderPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        private readonly By StakeholderLoadIcon = By.XPath("//div[@class='k-loading-image']");
        private readonly By ReviewAndFinishButton = By.XPath("//input[@value='Review and Finish']");
        private readonly By SelectAllCheckbox = By.CssSelector("input.masterContact");
        
        private static By StakeHolderCheckbox(int rowIndex) => By.XPath($"//table[@class='persondata-table']//tr[{rowIndex}]/td/input");

        private readonly By TableRows = By.XPath("//table[@class='persondata-table']//tbody/tr");
        private static By StakeHolderName(int rowIndex) => By.XPath($"//table[@class='persondata-table']//tbody/tr[{rowIndex}]/td[2]");

        public void WaitForStakeholdersPageToLoad()
        {
            Wait.UntilElementNotExist(StakeholderLoadIcon);
        }
        public void SelectAllStakeholders()
        {
            Log.Step(nameof(SelectStakeHolderPage), "Select All Stakeholders");
            Wait.UntilElementClickable(SelectAllCheckbox);
            Driver.JavaScriptScrollToElement(Wait.UntilElementVisible(SelectAllCheckbox)).ClickOn(Driver);
        }

        public void ClickOnReviewAndFinishButton()
        {
            Log.Step(nameof(SelectStakeHolderPage), "Click on Reviewer and Finish button");
            Wait.UntilElementClickable(ReviewAndFinishButton);
            Driver.JavaScriptScrollToElement(Wait.UntilElementVisible(ReviewAndFinishButton)).ClickOn(Driver);
            Wait.UntilJavaScriptReady();
        }

        internal void SelectStakeHolderByName(string name)
        {
            Log.Step(nameof(SelectStakeHolderPage), $"Select Stakeholder with name <{name}>");
            var rowIndex = 0;
            var rowCount = Driver.GetElementCount(TableRows);
            for (var i = 1; i <= rowCount; i++)
            {
                var actualName = Wait.UntilElementVisible(StakeHolderName(i)).Text;
                if (!actualName.Equals(name)) continue;
                rowIndex = i;
                break;
            }

            if (rowIndex == 0)
            {
                throw new Exception($"The StakeHolder '{name}' was not found in the table");
            }

            Wait.UntilElementClickable(StakeHolderCheckbox(rowIndex)).Check();
        }
    }
}