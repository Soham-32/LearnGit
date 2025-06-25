using System;
using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.IndividualAssessment.Common;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.IndividualAssessment.Edit;
using AtCommon.Api;
using AtCommon.Dtos.Assessments;
using AtCommon.Dtos.Reports;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Assessments.IndividualAssessment.Edit
{
    [TestClass]
    [TestCategory("TalentDevelopment"), TestCategory("Assessments")]
    public class EditIndividualAssessmentAddReviewerTests : IndividualAssessmentBaseTest
    {
        private static CreateReviewerRequest _reviewer;
        private static TeamResponse _team;
        private static IndividualAssessmentResponse _assessment;
        private static bool _classInitFailed;

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            try
            {
                var setup = new SetupTeardownApi(TestEnvironment);
              
                _reviewer = MemberFactory.GetReviewer();
                setup.CreateReviewer(_reviewer).GetAwaiter().GetResult();

                _team = GetTeamForBatchEdit(setup, "EditIA");
                var individualDataResponse = GetIndividualAssessment(setup, _team, "EditIASection_");
                var assessmentRequest = individualDataResponse.Item2;
                assessmentRequest.Members = _team.Members.Select(m => m.ToAddIndividualMemberRequest()).ToList();

                _assessment = setup.CreateIndividualAssessment(
                    assessmentRequest, SharedConstants.IndividualAssessmentType).GetAwaiter().GetResult();
            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        public void IndividualAssessment_Edit_AddExistingReviewer()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var iaEditPage = new IaEdit(Driver, Log);
            var addReviewerModal = new AddReviewerModal(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            var assessmentUid = _assessment.AssessmentList.FirstOrDefault().CheckForNull("assessmentUid is null.").AssessmentUid;
            iaEditPage.NavigateToPage(Company.Id, _team.Uid, assessmentUid);

            iaEditPage.ClickAddReviewerButton();

            addReviewerModal.AddReviewersBySearchingInModal(_reviewer);

            Log.Info("Assert: Verify chosen reviewer shows up on page");
            Assert.IsTrue(iaEditPage.DoesReviewerExist(_reviewer.Email), "Chosen reviewer does not match what is on the screen");

            Log.Info("Assert: Verify that email for survey was sent to the reviewer");
            const string reviewerSubject = SharedConstants.IaEmailReviewerSubject;

            Assert.IsTrue(GmailUtil.DoesMemberEmailExist(reviewerSubject, _reviewer.Email, "Inbox"),
                $"Could not find email with subject <{reviewerSubject}> sent to {_reviewer.Email}");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        public void IndividualAssessment_Edit_AddNewReviewer()
        {
            _assessment.CheckForNull($"{nameof(_assessment)} is null. Aborting test.");

            var login = new LoginPage(Driver, Log);
            var iaEditPage = new IaEdit(Driver, Log);
            var addReviewerModal = new AddReviewerModal(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            var assessmentUid = _assessment.AssessmentList.FirstOrDefault().CheckForNull("assessmentUid is null.").AssessmentUid;
            iaEditPage.NavigateToPage(Company.Id, _team.Uid, assessmentUid);

            iaEditPage.ClickAddReviewerButton();

            var newReviewer = MemberFactory.GetReviewer();
            newReviewer.FirstName = $"NewReviewer{Guid.NewGuid()}";
            addReviewerModal.CreateNewReviewer(newReviewer, false);

            addReviewerModal.AddReviewersByScrollingInModal(newReviewer);
            iaEditPage.WaitForPageLoad();

            Log.Info("Assert: Verify chosen reviewer shows up on page");
            Assert.IsTrue(iaEditPage.DoesReviewerExist(newReviewer.Email), "Chosen reviewer does not match what is on the screen");

            Log.Info("Assert: Verify that email for survey was sent to the reviewer");
            const string reviewerSubject = SharedConstants.IaEmailReviewerSubject;

            Assert.IsTrue(GmailUtil.DoesMemberEmailExist(reviewerSubject, newReviewer.Email, "Inbox"),
                $"Could not find email with subject <{reviewerSubject}> sent to {newReviewer.Email}");
        }
    }
}
