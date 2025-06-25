using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.IndividualAssessment.Common;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.IndividualAssessment.BatchEdit
{
    public class BatchEditAssessmentPage : BatchEditBase
    {
        public BatchEditAssessmentPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        private readonly By AssessmentNameInnerTextbox = By.XPath("//*[@automation-id='assessmentName']//input");
        private readonly By PointOfContactInnerTextbox = By.Id("pointOfContact");
        private readonly By PointOfContactEmailInnerTextbox = By.Id("pointOfContactEmail");
        private readonly By AssessmentStartInput = By.XPath("//div[@automation-id='assessmentStartDateTime']//div//input");
        private readonly By AssessmentEndInput = By.XPath("//div[@automation-id='assessmentEndDateTime']//div//input");

        public void WaitForAssessmentDataLoaded()
        {
            Log.Step(nameof(BatchEditAssessmentPage), "Wait for page to be loaded with assessment data");
            Wait.UntilAttributeNotEquals(AssessmentNameInnerTextbox, "textContent", "​");
        }

        public void InputAssessmentNameText(string assessmentName)
        {
            Log.Step(nameof(BatchEditAssessmentPage), "Input text in Assessment Name textbox");
            Wait.UntilElementVisible(AssessmentNameInnerTextbox);
            Wait.UntilElementVisible(PointOfContactInnerTextbox);
            Wait.UntilElementVisible(AssessmentStartInput);
            Wait.UntilElementClickable(AssessmentNameInnerTextbox).SendKeys(Keys.Control + "a");
            Wait.UntilElementClickable(AssessmentNameInnerTextbox).SendKeys(Keys.Delete);
            Wait.UntilElementClickable(AssessmentNameInnerTextbox).SetText(assessmentName, isReact: true);
        }

        public string GetAssessmentName()
        {
            Wait.HardWait(3000);
            return Wait.UntilElementExists(By.Id("assessmentName")).GetAttribute("value");
        }

        public string GetPointOfContact()
        {
            return Wait.UntilElementVisible(PointOfContactInnerTextbox).GetAttribute("value");
        }

        public string GetPointOfContactEmail()
        {
            return Wait.UntilElementVisible(PointOfContactEmailInnerTextbox).GetAttribute("value");
        }

        public string GetAssessmentStartDateTime()
        {
            return Wait.UntilElementVisible(AssessmentStartInput).GetAttribute("value");
        }

        public string GetAssessmentEndDateTime()
        {
            return Wait.UntilElementVisible(AssessmentEndInput).GetAttribute("value");
        }
    }
}
