using System.Text.RegularExpressions;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Create;
using AgilityHealth_Automation.Utilities;
using AtCommon.Dtos.IndividualAssessments;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.IndividualAssessment.Create
{
    internal class IaProfilePage : BasePage
    {
        public IaProfilePage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        private readonly By IaProfileIntro = By.ClassName("intro");
        private readonly By AssessmentName = By.Id("name");
        private readonly By PointOfContact = By.Id("poc");
        private readonly By PointOfContactEmail = By.Id("pocemail");
        private readonly By NextButton = By.Id("nextButton");
        private readonly By SurveyTypeListbox = By.Id("SurveyTypeName");

        private By DynamicTeamMemberCheckBox(string teamMemberName) => By.XPath(
            $"//div[@id='TeamMembersGrid']/table/tbody//td[text()='{teamMemberName}']/preceding-sibling::td/input");
        

        public void EnterIndividualAssessmentProfile(CreateIndividualAssessmentRequest individualAssessmentRollUp, string assessmentType)
        {
            Log.Step(nameof(AssessmentProfilePage), "Enter data for individual assessment profile");
            Wait.UntilElementVisible(AssessmentName).SetText(individualAssessmentRollUp.AssessmentName);
            Wait.UntilElementVisible(PointOfContact).SetText(individualAssessmentRollUp.PointOfContact);
            Wait.UntilElementVisible(PointOfContactEmail).SetText(individualAssessmentRollUp.PointOfContactEmail);
            Wait.UntilElementClickable(SurveyTypeListbox).SelectDropdownValueByVisibleText(assessmentType);

            Driver.JavaScriptScroll("0", "1000");
            foreach (var reviewee in individualAssessmentRollUp.Members)
            {
                SelectIndividualAssessment(reviewee.FullName());
            }

        }

        public void ClickNextButton()
        {
            Log.Step(nameof(AssessmentProfilePage), "Click on Next button");
            Wait.UntilElementClickable(NextButton).Click();
            Wait.UntilJavaScriptReady();
        }

        public void SelectIndividualAssessment(string teamMemberName)
        {
            Wait.UntilElementClickable(DynamicTeamMemberCheckBox(teamMemberName)).Click();
            Wait.UntilJavaScriptReady();
        }

        public string GetIntroText()
        {
            var introText = Wait.UntilElementVisible(IaProfileIntro).GetText();
            introText = Regex.Replace(introText, @"\r\n?|\n", "");
            return introText;
        }
    }
}