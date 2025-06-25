using System;
using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.IndividualAssessment.BatchEdit;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
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
    public class BatchEditSendReminderTest : IndividualAssessmentBaseTest
    {
        private static bool _classInitFailed;
        private static TeamResponse _team;
        private static CreateReviewerRequest _reviewer;
        private static IndividualAssessmentResponse _assessment;
        private static IndividualAssessmentResponse _assessmentResponse;
        private static CreateIndividualAssessmentRequest _assessmentRequest;

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            try
            {
                var setup = new SetupTeardownApi(TestEnvironment);

                _team = GetTeamForBatchEdit(setup, "BatchEditSendReminder");

                _reviewer = MemberFactory.GetReviewer();
                var reviewerResponse = setup.CreateReviewer(_reviewer).GetAwaiter().GetResult();

                var individualDataResponse = GetIndividualAssessment(setup, _team, "BatchEditSendReminder_");
                _assessmentRequest = individualDataResponse.Item2;
                _assessmentResponse = individualDataResponse.Item3;
                _assessmentRequest.Members = _team.Members.Select(m => m.ToAddIndividualMemberRequest()).ToList();
                _assessmentRequest.Members.First().Reviewers.Add(reviewerResponse.ToAddIndividualMemberRequest());
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
        public void BatchEdit_SendReminder()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var dashboardPage = new TeamDashboardPage(Driver, Log);
            var batchEditAssessmentPage = new BatchEditAssessmentPage(Driver, Log);
            var batchEditParticipantReviewerPage = new BatchEditParticipantReviewerPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            var teamId = dashboardPage.GetTeamIdFromLink(_team.Name);
            batchEditAssessmentPage.NavigateToPage(Company.Id, _team.Uid, _assessment.BatchId, teamId);
            batchEditAssessmentPage.WaitForAssessmentDataLoaded();
            batchEditAssessmentPage.ClickParticipantsReviewersTab();
            batchEditParticipantReviewerPage.SendReminder();

            //Assert twice to make sure reviewer receives 2 emails (one for original email and one for reminder email)
            Assert.IsTrue(GmailUtil.DoesMemberEmailExist(SharedConstants.IaEmailParticipantSubject, _assessmentRequest.Members.First().Email, "Inbox"),
                $"Could not find email with subject <{SharedConstants.IaEmailParticipantSubject}> sent to {_assessmentRequest.Members.First().Email}");
            Assert.IsTrue(GmailUtil.DoesMemberEmailExist(SharedConstants.IaEmailParticipantSubject, _assessmentRequest.Members.First().Email, "Inbox"),
                $"Could not find email with subject <{SharedConstants.IaEmailParticipantSubject}> sent to {_assessmentRequest.Members.First().Email}");
            Assert.IsTrue(GmailUtil.DoesMemberEmailExist(SharedConstants.IaEmailReviewerSubject, _reviewer.Email, "Inbox"),
                $"Could not find email with subject <{SharedConstants.IaEmailReviewerSubject}> sent to {_reviewer.Email}");
            Assert.IsTrue(GmailUtil.DoesMemberEmailExist(SharedConstants.IaEmailReviewerSubject, _reviewer.Email, "Inbox"),
                $"Could not find email with subject <{SharedConstants.IaEmailReviewerSubject}> sent to {_reviewer.Email}");
        }
    }
}
