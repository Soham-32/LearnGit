using System.Collections.Generic;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.IndividualAssessment.Common;
using AgilityHealth_Automation.Utilities;
using AtCommon.Utilities;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.IndividualAssessment.BatchEdit
{
    public class BatchEditViewersPage : BatchEditBase
    {
        public BatchEditViewersPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        private readonly By IndividualAndAggregateViewersListTextBox =
            By.XPath("//*[@automation-id='individualViewers']//input[not(@name = 'individualViewers')]");
        private readonly By AggregateViewersListTextBox =
            By.XPath("//*[@automation-id='aggregateViewers']//input[not(@name = 'aggregateViewers')]");
        private readonly By AddIndividualAndAggregateViewersFieldBox = AutomationId.Equals("individualViewers");
        private readonly By AddAggregateViewersFieldBox = AutomationId.Equals("aggregateViewers");

        private static By EmailDropDownList(string email) => By.XPath($"//div[contains(@id,'react-select') and text()='{email}']");
        private static By IndividualAndAggregateEmailInput(string email) => By.XPath($"//*[@automation-id='individualViewers']//descendant::div[text()='{email}']");
        private static By AggregateEmailInput(string email) => By.XPath($"//*[@automation-id='aggregateViewers']//descendant::div[text()='{email}']");

        public void WaitUntilViewersPageLoaded()
        {
            Log.Step(nameof(BatchEditViewersPage), "Wait for the viewers page to be loaded");
            Wait.UntilElementVisible(AddIndividualAndAggregateViewersFieldBox);
            Wait.UntilElementVisible(AddAggregateViewersFieldBox);
        }

        public void InputIndividualAndAggregateEmail(string email)
        {
            Log.Step(nameof(BatchEditViewersPage),
                $"Select email <{email}> in Individual and Aggregate dropdown");
            SelectViewer(email, IndividualAndAggregateViewersListTextBox);
            Wait.UntilElementVisible(IndividualAndAggregateEmailInput(email));
        }

        public void InputAggregateEmail(string email)
        {
            Log.Step(nameof(BatchEditViewersPage),
                $"Select email(s) <{email}>in Aggregate dropdown");
            SelectViewer(email, AggregateViewersListTextBox);
            Wait.UntilElementVisible(AggregateEmailInput(email));
        }

        private void SelectViewer(string email, By locator)
        {
            Log.Step(nameof(BatchEditViewersPage), "Select reviewer");
            var element = Wait.UntilElementClickable(locator);
            new Actions(Driver).MoveToElement(element).Click().SendKeys(element, email).Build().Perform();
            Wait.UntilElementClickable(EmailDropDownList(email)).Click();
        }

        public void DeleteIndividualAndAggregateEmail()
        {
            Log.Step(nameof(BatchEditViewersPage), $"Delete the email of the individual/aggregate viewer");
            Wait.UntilElementClickable(IndividualAndAggregateViewersListTextBox).ClearTextbox();
        }

        public List<string> GetIndividualAggregateViewer()
        {
            return Wait.UntilElementVisible(AddIndividualAndAggregateViewersFieldBox).GetText().SplitLines();
        }

        public List<string> GetAggregateViewer()
        {
            return Wait.UntilElementVisible(AddAggregateViewersFieldBox).GetText().SplitLines();
        }
    }
}
