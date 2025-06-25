using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.IndividualAssessment.Common;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.IndividualAssessment.Create
{
    public class CreateIndividualAssessment3InviteViewersPage : CreateIndividualAssessmentBase
    {
        public CreateIndividualAssessment3InviteViewersPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        private readonly By AddIndividualAndAggregateViewersFieldBox = AutomationId.Equals("individualViewers");
        private readonly By AddAggregateViewersFieldBox = AutomationId.Equals("aggregateViewers");

        private readonly By AggregateViewersInFieldBox = By.XPath(
            "//*[@automation-id='aggregateViewers']//div[contains(@class, 'multiValue')]/div[text()]");
        private static By EmailDropDownList(string email) => By.XPath($"//div[contains(@id,'react-select') and text()='{email}']");

        public void InputIndividualAndAggregateEmail(string email)
        {
            Log.Step(nameof(CreateIndividualAssessment3InviteViewersPage),
                $"Select email <{email}> in Individual and Aggregate dropdown");

            for (int i = 0; i < 3; i++)
            {
                Wait.UntilElementClickable(AddIndividualAndAggregateViewersFieldBox);
                SelectViewer(email, AddIndividualAndAggregateViewersFieldBox);

                if (Driver.IsElementDisplayed(AddIndividualAndAggregateViewersFieldBox)) break;
            }
        }

        public void InputAggregateEmail(string email)
        {
            Log.Step(nameof(CreateIndividualAssessment3InviteViewersPage),
                $"Select email(s) <{email}>in Aggregate dropdown");
            
            for(int i = 0; i < 3; i++)
            {
                Wait.UntilElementClickable(AddAggregateViewersFieldBox);
                SelectViewer(email, AddAggregateViewersFieldBox);

                if (Driver.IsElementDisplayed(AggregateViewersInFieldBox)) break;
            }
            
        }

        private void SelectViewer(string email, By locator)
        {
            Log.Step(nameof(CreateIndividualAssessment3InviteViewersPage),
                $"Select reviewer with email {email}");
            var element = Wait.UntilElementClickable(locator);
            new Actions(Driver).MoveToElement(element).Click().SendKeys(element, email).Build().Perform();
            Wait.UntilElementClickable(EmailDropDownList(email)).Click();
        }

        public void WaitUntilLoaded()
        {
            Log.Step(nameof(CreateIndividualAssessment3InviteViewersPage), "Wait for page to load");
            Wait.UntilJavaScriptReady();
            Wait.UntilElementVisible(AddAggregateViewersFieldBox);
        }
    }
}
