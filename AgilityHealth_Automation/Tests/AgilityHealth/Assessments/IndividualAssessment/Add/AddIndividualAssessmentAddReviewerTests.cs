using System;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.IndividualAssessment.Common;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.IndividualAssessment.Create;
using AtCommon.Api;
using AtCommon.Dtos.Assessments;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Assessments.IndividualAssessment.Add
{
    [TestClass]
    [TestCategory("TalentDevelopment"), TestCategory("Assessments")]
    public class AddReviewerIndividualAssessmentTest : IndividualAssessmentBaseTest
    {
        private static bool _classInitFailed;
        private static CreateReviewerRequest _reviewer;
        private static TeamResponse _team;

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
           

            try
            {
                var setup = new SetupTeardownApi(TestEnvironment);
                _team = GetTeamForIndividualAssessment(setup, "IA");
                
                _reviewer = MemberFactory.GetReviewer();
                setup.CreateReviewer(_reviewer).GetAwaiter().GetResult();
            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        public void IndividualAssessment_AddReviewers()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var createIndividualAssessment1 = new CreateIndividualAssessment1CreateAssessmentPage(Driver, Log);
            var createIndividualAssessment2 = new CreateIndividualAssessment2AddReviewersPage(Driver, Log);
            var createIndividualAssessment3 = new CreateIndividualAssessment3InviteViewersPage(Driver, Log);
            var createIndividualAssessment4 = new CreateIndividualAssessment4AddReviewAndPublishPage(Driver, Log);
            var addReviewerModal = new AddReviewerModal(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            createIndividualAssessment1.NavigateToPage(Company.Id, _team.Uid);

            var assessment = IndividualAssessmentFactory.GetUiIndividualAssessment($"ReviewerIA_{Guid.NewGuid()}");

            createIndividualAssessment1.WaitUntilLoaded();
            createIndividualAssessment1.FillInIndividualAssessmentInfo(assessment, SharedConstants.IndividualAssessmentType);

            createIndividualAssessment1.ClickNextButton();

            createIndividualAssessment2.WaitUntilLoaded();
            createIndividualAssessment2.ExpandCollapseParticipantsAndReviewers();
            var reviewEmail = _team.Members[0].Email;
            createIndividualAssessment2.ClickAddReviewer(reviewEmail);
            addReviewerModal.WaitUntilLoaded();
            addReviewerModal.AddReviewersBySearchingInModal(_reviewer);

            Assert.AreEqual(_team.Members[0].FullName() + " (1)", createIndividualAssessment2.GetParticipantNameByEmail(reviewEmail), "Participant name should be displayed properly with count of reviewer");

            createIndividualAssessment2.ClickNextButton();

            createIndividualAssessment3.WaitUntilLoaded();
            createIndividualAssessment3.ClickNextButton();

            //publish
            createIndividualAssessment4.WaitUntilLoaded();
            createIndividualAssessment4.ClickPublishBottomButton();

            Log.Info("Assert: Verify that email for email was sent to the reviewer");
            const string reviewerSubject = SharedConstants.IaEmailReviewerSubject;

            Assert.IsTrue(GmailUtil.DoesMemberEmailExist(reviewerSubject, _reviewer.Email, "Inbox"),
                $"Could not find email with subject <{reviewerSubject}> sent to {_reviewer.Email}");
        }
    }
}
