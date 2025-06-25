using AgilityHealth_Automation.Utilities;
using AtCommon.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Insights.StructuralAgility
{
    internal class StructuralAgilitySettingsPage : StructuralAgilityPage
    {
        public StructuralAgilitySettingsPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        // Locators
        private readonly By SaveButton = By.Id("Save__btn");
        private readonly By CancelButton = By.Id("Cancel__btn");
        private static By NumberOfTeamsToSupport(string workType, string role) =>
            By.XPath($"//table/tbody//td[text() = '{workType}']/following-sibling::td[text() = '{role}']/following-sibling::td//input | //table/tbody//td//font[text() = '{workType}']//..//..//following-sibling::td[.//font[text() = '{role}'] or text() = '{role}']/following-sibling::td//input");


        // Methods

        public int GetNumberOfTeamsToSupport(string workType, string role)
        {
            return Wait.UntilElementExists(NumberOfTeamsToSupport(workType, role)).GetText().ToInt();
        }

        public void SetNumberOfTeamsToSupport(string workType, string role, int number)
        {
            Log.Step(nameof(StructuralAgilitySettingsPage), $"Set the number of teams for work type <{workType}> and role <{role}> to be <{number}>");
            Wait.UntilElementExists(NumberOfTeamsToSupport(workType, role)).SetText(number.ToString(), isReact:true);
        }

        public void ClickSaveButton()
        {
            Log.Step(nameof(StructuralAgilitySettingsPage), "Click on the 'Save' button");
            Wait.UntilElementClickable(SaveButton).Click();
            WaitUntilWidgetsLoaded();
        }

        public void ClickCancelButton()
        {
            Log.Step(nameof(StructuralAgilitySettingsPage), "Click on the 'Cancel' button");
            Wait.UntilElementClickable(CancelButton).Click();
            WaitUntilWidgetsLoaded();
        }


    }
}