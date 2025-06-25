using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.IndividualAssessment.Common;
using AgilityHealth_Automation.PageObjects.AgilityHealth.V2;
using AgilityHealth_Automation.Utilities;
using AtCommon.Dtos.IndividualAssessments;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.IndividualAssessment.Create
{
    public class CreateIndividualAssessment1CreateAssessmentPage : CreateIndividualAssessmentBase
    {
        public CreateIndividualAssessment1CreateAssessmentPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        private readonly By AssessmentTypeListBox = By.Id("mui-component-select-assessmentType");
        private static By AssessmentTypeListItem(string item) => By.Name(item);
        private readonly By AssessmentNameTextBox = By.Id("assessmentName");
        private readonly By PointOfContactTextBox = By.Id("pointOfContact");
        private readonly By PointOfContactEmailTextBox = By.Id("pointOfContactEmail");
        private const string StartDateId = "assessmentStartDateTime";
        private const string EndDateId = "assessmentEndDateTime";


        public void FillInIndividualAssessmentInfo(CreateIndividualAssessmentRequest request, string assessmentTypeName)
        {
            Log.Step(nameof(CreateIndividualAssessment1CreateAssessmentPage), "Fill in Assessment Profile info");
            SelectItem(AssessmentTypeListBox, AssessmentTypeListItem(assessmentTypeName));
            Wait.UntilElementClickable(AssessmentNameTextBox).ClearTextbox();
            Wait.UntilElementClickable(AssessmentNameTextBox).SetText(request.AssessmentName);
            var endCalendar = new V2Calendar(Driver, EndDateId);
            endCalendar.SetDateAndTime(request.End);
            var startCalendar = new V2Calendar(Driver, StartDateId);
            startCalendar.SetDateAndTime(request.Start);
            Wait.UntilElementClickable(PointOfContactTextBox).ClearTextbox();
            Wait.UntilElementClickable(PointOfContactTextBox).SetText(request.PointOfContact);
            Wait.UntilElementClickable(PointOfContactEmailTextBox).ClearTextbox();
            Wait.UntilElementClickable(PointOfContactEmailTextBox).SetText(request.PointOfContactEmail);
        }

        public void WaitUntilLoaded()
        {
            Log.Step(nameof(CreateIndividualAssessment1CreateAssessmentPage), "Wait for page to load");
            Wait.UntilElementClickable(AssessmentTypeListBox);
        }
    }
}
