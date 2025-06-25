using System.Collections.Generic;
using System.Text.RegularExpressions;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.IndividualAssessment.Create
{
    internal class IaReviewAndFinishPage : BasePage
    {
        public IaReviewAndFinishPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        private readonly By IaFinishIntro = By.ClassName("intro");
        private readonly By PublishButton = By.Id("publishBtn");
        private readonly By SaveAsDraftButton = By.Id("draftButton");
        private readonly By AssessmentNameText = By.CssSelector("div.content h5");
        private readonly By StartDate = By.Id("StartDate");
        private readonly By EndDate = By.Id("EndDate");
        private readonly By GiveVisiblityCheckbox = By.Id("CanViewRollUpAndIndividualRadars");
        private readonly By GiveVisibilityListBox = By.XPath("//*[@id='TeamIndividualTeamMembers']/preceding-sibling::div");
        private By GiveVisibilityListItem(string item) => By.XPath($"//ul[@id='TeamIndividualTeamMembers_listbox']/li[text() = '{item}']");
        private readonly By AllOwRevieweeToViewResultsCheckbox = By.Id("CanViewResults");

        private By DynamicTeamMemberCheckBox(string reviewee, string assessmentType, int numberOfReviewer) =>
            By.XPath(
                $"//table/tbody//td[text()='{reviewee}']/following-sibling::td[contains(text(),'{assessmentType}')]/following-sibling::td/span[text()='{numberOfReviewer}']");

        private readonly By CoachesGrid = By.Id("coaches-grid");
        private readonly By DeleteButton = By.Id("delete_assessment_link");
        private readonly By RemoveButton = By.Id("remove_assessment");
        

        public string GetAssessmentName()
        {
            return Wait.UntilElementVisible(AssessmentNameText).GetText();
        }

        public string GetStartDate()
        {
            return Wait.UntilElementExists(StartDate).GetElementAttribute("value");
        }

        public string GetEndDate()
        {
            return Wait.UntilElementExists(EndDate).GetElementAttribute("value");
        }

        public void PublishAssessment()
        {
            Log.Step(nameof(IaReviewAndFinishPage), "Click on Publish button");
            Wait.UntilElementClickable(PublishButton).ClickOn(Driver);
            Wait.UntilJavaScriptReady();
        }

        public void SaveAsDraft()
        {
            Log.Step(nameof(IaReviewAndFinishPage), "Click on Save as Draft button");
            Wait.UntilElementClickable(SaveAsDraftButton).Click();
            Wait.UntilJavaScriptReady();
        }

        public bool DoesIndividualAssessmentExist(string reviewee, string assessmentType, int numberOfReviewer)
        {
            return Wait.InCase(DynamicTeamMemberCheckBox(reviewee, assessmentType, numberOfReviewer)) != null;
        }

        public bool DoesIntroTextDisplayCorrectly(string expected)
        {
            var introText = Wait.UntilElementVisible(IaFinishIntro).GetText();
            introText = Regex.Replace(introText, @"\r\n?|\n", "");
            return expected.Trim().Equals(introText);
        }

        public void GiveVisibilityToTeamMembers(List<string> teamMembers)
        {
            foreach (var member in teamMembers)
            {
                Wait.UntilElementClickable(GiveVisiblityCheckbox).Click();
                SelectItem(GiveVisibilityListBox, GiveVisibilityListItem(member));
            }
        }

        public bool IsCoachesGridDisplayed()
        {
            return Wait.InCase(CoachesGrid)?.Displayed ?? false;
        }

        public void DeleteAssessment()
        {
            Log.Step(nameof(IaReviewAndFinishPage), "Delete assessment");
            Wait.UntilElementClickable(DeleteButton).Click();
            Wait.UntilJavaScriptReady();
            Wait.UntilElementClickable(RemoveButton).Click();
            Wait.UntilJavaScriptReady();
        }

        public void CheckAllowRevieweeToViewResults()
        {
            Wait.UntilElementClickable(AllOwRevieweeToViewResultsCheckbox).Check();
        }
    }
}