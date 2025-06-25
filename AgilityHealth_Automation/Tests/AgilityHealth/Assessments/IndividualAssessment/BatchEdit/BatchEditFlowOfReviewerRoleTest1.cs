using System;
using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.IndividualAssessment.BatchEdit;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Survey;
using AtCommon.Api;
using AtCommon.Dtos.Assessments;
using AtCommon.Dtos.IndividualAssessments;
using AtCommon.Dtos.Reports;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Assessments.IndividualAssessment.BatchEdit
{
    [TestClass]
    [TestCategory("TalentDevelopment"), TestCategory("Assessments")]
    public class BatchEditFlowOfReviewerRoleTest1 : IndividualAssessmentBaseTest
    {
        private static bool _classInitFailed;
        private static ReviewerResponse _reviewer;
        private static TeamResponse _team;
        private static IndividualAssessmentResponse _assessment;
        private static IndividualAssessmentResponse _assessmentResponse;
        private static CreateIndividualAssessmentRequest _assessmentRequest;

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            try
            {
                var setup = new SetupTeardownApi(TestEnvironment);
                var reviewerRequest = MemberFactory.GetReviewer();
                _reviewer = setup.CreateReviewer(reviewerRequest).GetAwaiter().GetResult();
                
                _team = GetTeamForBatchEdit(setup, "BatchEditReviewerRole");
                var individualDataResponse = GetIndividualAssessment(setup, _team, "BatchEditReviewerRoleIA_");
                _assessmentRequest = individualDataResponse.Item2;
                _assessmentResponse = individualDataResponse.Item3;
                _assessmentRequest.Members = _team.Members.Select(m => m.ToAddIndividualMemberRequest()).ToList();
                _assessmentRequest.Members.First().Reviewers.Add(_reviewer.ToAddIndividualMemberRequest());
                _assessmentRequest.BatchId = _assessmentResponse.BatchId;

                _assessment = setup.CreateIndividualAssessment(_assessmentRequest, SharedConstants.IndividualAssessmentType)
                    .GetAwaiter().GetResult();
            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        public void BatchEdit_ReviewerRole_Single()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var surveyPage = new SurveyPage(Driver, Log);
            var batchEditAssessmentPage = new BatchEditAssessmentPage(Driver, Log);
            var batchEditParticipantReviewerPage = new BatchEditParticipantReviewerPage(Driver, Log);

            var surveyLink = GmailUtil.GetSurveyLink(SharedConstants.IaEmailReviewerSubject,
                _assessmentRequest.Members.First().Reviewers.First().Email, "unread", _assessmentRequest.PointOfContact);
            surveyPage.NavigateToUrl(surveyLink);

            surveyPage.SelectReviewerRole(new List<string>{"Reviewer"});
            surveyPage.ConfirmIdentity();
            surveyPage.ClickStartSurveyButton();

            surveyPage.SubmitRandomSurvey();

            surveyPage.ClickNextButton();
            surveyPage.ClickNextButton();
            surveyPage.ClickNextButton();
            surveyPage.ClickFinishButton();
            
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            batchEditAssessmentPage.NavigateToPage(Company.Id, _team.Uid, _assessment.BatchId);
            batchEditAssessmentPage.WaitForAssessmentDataLoaded();
            batchEditAssessmentPage.ClickParticipantsReviewersTab();
            batchEditParticipantReviewerPage.ExpandCollapseParticipantsAndReviewersWithReviewerEmail(_assessmentRequest.Members.First().Reviewers.First().Email);
            var role = batchEditParticipantReviewerPage.GetReviewerRole(_assessmentRequest.Members.First().Reviewers.First().Email);

            Log.Info("Verify reviewer role from assessment shows");
            Assert.AreEqual("Reviewer", role, "Expected and actual role shown on the batch edit page do not match");
        }

    }
}