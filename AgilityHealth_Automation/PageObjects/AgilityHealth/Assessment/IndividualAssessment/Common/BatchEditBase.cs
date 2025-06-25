using System;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.IndividualAssessment.Common
{
    public class BatchEditBase : IndividualAssessmentBase
    {
        public BatchEditBase(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }
        //Header
        private readonly By PublishButton = AutomationId.Equals("headerPublishBtn");
        private readonly By SaveButton = AutomationId.Equals("headerUpdateBtn");
        private readonly By CancelButton = AutomationId.Equals("headerCancelBtn");
        private readonly By TrashCanButton = AutomationId.Equals("headerTrashBtn");
        private readonly By XButton = AutomationId.Equals("headerCloseBtn");
        private readonly By BatchEditHeading = By.XPath("//div//h1[contains(text(),'Batch Edit')]");

        //Tabs
        private readonly By ViewersTab = By.Id("tabpanel-2");
        private readonly By ParticipantsReviewersTab = By.Id("tabpanel-1");
                
        public void ClickCancelButton()
        {
            Log.Step(GetType().Name, "Click cancel button in header");
            Wait.UntilElementClickable(CancelButton).Click();
        }
        public void ClickDeleteTrashButton()
        {
            Log.Step(GetType().Name, "Click trash button in header");
            Wait.UntilElementClickable(TrashCanButton).Click();
        }
        public void ClickXButton()
        {
            Log.Step(GetType().Name, "Click x button in header");
            Wait.UntilElementClickable(XButton).Click();
        }

        public void ClickSaveButton()
        {
            Log.Step(GetType().Name, "Click the save button in the header");
            Wait.UntilElementClickable(SaveButton).Click();
        }
        public void ClickPublishButton()
        {
            Log.Step(GetType().Name, "Click the publish button in the header");
            Wait.UntilElementClickable(PublishButton).Click();
        }

        public void ClickViewersTab()
        {
            Log.Step(GetType().Name, "Click the 'Viewers' tab");
            Wait.UntilElementClickable(ViewersTab).Click();
        }

        public void ClickParticipantsReviewersTab()
        {
            Log.Step(GetType().Name, "Click the 'Participants/Reviewers' tab");
            Wait.UntilElementClickable(ParticipantsReviewersTab).Click();
        }

        public bool IsBatchEditPagePresent()
        {
            Log.Step(GetType().Name, "Return whether user still on batch edit page");
            return Wait.InCase(BatchEditHeading) != null;
        }

        public void NavigateToPage(int companyId, Guid teamUid, int batchId, string teamId = null)
        {
            var url =
                $"{BaseTest.ApplicationUrl}/v2/batch-edit-assessments/company/{companyId}/team/{teamUid}/batch/{batchId}";
            if (teamId != null)
                url = url.AddQueryParameter("teamId", teamId);
            NavigateToUrl(url);
        }
    }
}