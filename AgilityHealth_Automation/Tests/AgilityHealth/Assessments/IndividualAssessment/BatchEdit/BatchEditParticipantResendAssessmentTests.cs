using System;
using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.IndividualAssessment.BatchEdit;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AtCommon.Api;
using AtCommon.Dtos.IndividualAssessments;
using AtCommon.Dtos.Reports;
using AtCommon.Dtos.Teams;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Assessments.IndividualAssessment.BatchEdit
{
    [TestClass]
    [TestCategory("TalentDevelopment"), TestCategory("Assessments")]
    public class BatchEditParticipantResendAssessmentTests : IndividualAssessmentBaseTest
    {
        private static bool _classInitFailed;
        private static TeamResponse _team;
        private static IndividualAssessmentResponse _assessment;
        private static CreateIndividualAssessmentRequest _assessmentResponse;

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            try
            {
                var setup = new SetupTeardownApi(TestEnvironment);
                
                _team = GetTeamForBatchEdit(setup, "BatchEditResend", 2);
                var individualDataResponse = GetIndividualAssessment(setup, _team, "BatchEditResend_");
                _assessmentResponse = individualDataResponse.Item2;
                _assessmentResponse.Members = _team.Members.Select(m => m.ToAddIndividualMemberRequest()).ToList();
                _assessment = setup
                    .CreateIndividualAssessment(_assessmentResponse, SharedConstants.IndividualAssessmentType)
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
        public void BatchEdit_Participant_ResendAssessment()
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

            var participantEmail = _assessmentResponse.Members[0].Email;

            batchEditParticipantReviewerPage.ResendParticipantAssessment();

            Assert.AreEqual("Participant email has been sent", batchEditParticipantReviewerPage.GetToasterMessage(), "Toaster message for participant is not showing properly");

            //Assert twice to make sure reviewer receives 2 emails (one for original email and one for resent email)
            Assert.IsTrue(
                GmailUtil.DoesMemberEmailExist(SharedConstants.IaEmailParticipantSubject, participantEmail, "Inbox"),
                $"Could not find email with subject <{SharedConstants.IaEmailParticipantSubject}> sent to {participantEmail}");
            Assert.IsTrue(
                GmailUtil.DoesMemberEmailExist(SharedConstants.IaEmailParticipantSubject, participantEmail, "Inbox"),
                $"Could not find email with subject <{SharedConstants.IaEmailParticipantSubject}> sent to {participantEmail}");
        }
    }
}