using System;
using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.IndividualAssessment.Common;
using AgilityHealth_Automation.Utilities;
using AtCommon.Dtos.IndividualAssessments;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.IndividualAssessment.Create
{
    public class CreateIndividualAssessment4AddReviewAndPublishPage : CreateIndividualAssessmentBase
    {
        public CreateIndividualAssessment4AddReviewAndPublishPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        private readonly By AssessmentType = By.Id("typeName");
        private readonly By AssessmentStartDateTime = By.Id("startDate");
        private readonly By PointOfContact = By.Id("pointOfContact");
        private readonly By AssessmentName = By.Id("name");
        private readonly By AssessmentEndDateTime = By.Id("endDate");
        private readonly By PointOfContactEmail = By.Id("pointOfContactEmail");

        private static By Reviewer(string email) =>
            By.XPath($"//div[@id='panelParticipants']//div//p[contains(normalize-space(),'{email}')]");
        private readonly By IndividualViewers = AutomationId.StartsWith("individualViewerEmail_");
        private readonly By AggregateViewers = AutomationId.StartsWith("aggregateViewerEmail_");
        private readonly By PublishTopButton = AutomationId.Equals("publishBtn_top");
        private readonly By PublishBottomButton = AutomationId.Equals("publishBtn_bottom");
        private readonly By AssessmentEditIcon = By.XPath("//div[@automation-id='Assessment_title']/parent::div/following-sibling::div/button[@automation-id='editIcon']");

        public CreateIndividualAssessmentRequest GetAssessmentInfo()
        {
            var request = new CreateIndividualAssessmentRequest()
            {
                PointOfContact = Wait.UntilElementVisible(PointOfContact).GetText(),
                PointOfContactEmail = Wait.UntilElementVisible(PointOfContactEmail).GetText(),
                AssessmentName = Wait.UntilElementVisible(AssessmentName).GetText()
            };
            var start = Wait.UntilElementVisible(AssessmentStartDateTime).GetText();
            var end = Wait.UntilElementVisible(AssessmentEndDateTime).GetText();

            //removing timezone, so it is is parseable 
            request.Start = DateTime.Parse(start.Remove(start.IndexOf("m", StringComparison.Ordinal) + 1));
            request.End = DateTime.Parse(end.Remove(end.IndexOf("m", StringComparison.Ordinal) + 1));

            return request;
        }

        public string GetAssessmentType()
        {
            return Wait.UntilElementVisible(AssessmentType).GetText();
        }

        public void ClickPublishTopButton()
        {
            Log.Step(nameof(CreateIndividualAssessment4AddReviewAndPublishPage), "Click the top 'Publish' button");
            Driver.JavaScriptScrollToElement(Wait.UntilElementVisible(PublishTopButton), false).Click();
            Wait.UntilElementNotExist(PublishTopButton);
        }

        public void ClickPublishBottomButton()
        {
            Log.Step(nameof(CreateIndividualAssessment4AddReviewAndPublishPage), "Click the bottom 'Publish' button");
            Wait.UntilElementClickable(PublishBottomButton).Click();
            Wait.UntilElementNotExist(PublishBottomButton);
            Wait.UntilJavaScriptReady();
        }

        public void WaitUntilLoaded()
        {
            Log.Step(nameof(CreateIndividualAssessment4AddReviewAndPublishPage), "Wait for page to load");
            Wait.UntilElementVisible(PublishBottomButton);
            Wait.UntilJavaScriptReady();
        }

        public List<string> GetIndividualViewers() => Driver.GetTextFromAllElements(IndividualViewers).ToList();

        public List<string> GetAggregateViewers() => Driver.GetTextFromAllElements(AggregateViewers).ToList();

        public bool HasReviewer(string email)
        {
            return Driver.IsElementPresent(Reviewer(email));
        }

        public void ClickAssessmentEditIcon()
        {
            Log.Step(nameof(CreateIndividualAssessment4AddReviewAndPublishPage), "Click the 'Edit' button in the Assessment Section");
            Wait.UntilElementVisible(AssessmentEditIcon).Click();
        }

        public bool DoesPublishButtonDisplay()
        {
            var publishButton = Wait.InCase(PublishBottomButton);
            return publishButton != null && publishButton.Displayed;
        }
    }
}
