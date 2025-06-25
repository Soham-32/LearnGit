using System;
using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
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
    public class EditIndividualAssessmentResendAssessmentTests : IndividualAssessmentBaseTest
    {
        private static bool _classInitFailed;
        private static ReviewerResponse _reviewer;
        private static TeamResponse _team;
        private static IndividualAssessmentResponse _assessment;

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            try
            {
                var setup = new SetupTeardownApi(TestEnvironment);
                
                var reviewerRequest = MemberFactory.GetReviewer();
                _reviewer = setup.CreateReviewer(reviewerRequest).GetAwaiter().GetResult();
                
                _team = GetTeamForBatchEdit(setup, "EditIA");
                var individualDataResponse = GetIndividualAssessment(setup, _team, "ReviewerIA_");
                var assessment = individualDataResponse.Item2;
                assessment.Members = _team.Members.Select(m => m.ToAddIndividualMemberRequest()).ToList();
                assessment.Members.First().Reviewers.Add(_reviewer.ToAddIndividualMemberRequest());

                _assessment = setup.CreateIndividualAssessment(assessment, SharedConstants.IndividualAssessmentType).GetAwaiter().GetResult();
                assessment.BatchId = _assessment.BatchId;
                _assessment = setup.CreateIndividualAssessment(assessment, SharedConstants.IndividualAssessmentType).GetAwaiter().GetResult();

            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }


        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        public void IndividualAssessment_Edit_ResendAssessment()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var iaEditPage = new IaEdit(Driver, Log);
            
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            var assessmentUid = _assessment.AssessmentList.FirstOrDefault().CheckForNull("assessmentUid is null.").AssessmentUid;
            iaEditPage.NavigateToPage(Company.Id, _team.Uid, assessmentUid);

            iaEditPage.ResendAssessment();

            Assert.AreEqual("Reviewer email has been sent", iaEditPage.GetToasterMessage(), "Toaster message for reviewer is not showing properly");

            //Assert twice to make sure reviewer receives 2 emails (one for original email and one for resent email)
            Assert.IsTrue(GmailUtil.DoesMemberEmailExist(SharedConstants.IaEmailReviewerSubject, _reviewer.Email, "Inbox"),
                $"Could not find email with subject <{SharedConstants.IaEmailReviewerSubject}> sent to {_reviewer.Email}");
            Assert.IsTrue(GmailUtil.DoesMemberEmailExist(SharedConstants.IaEmailReviewerSubject, _reviewer.Email, "Inbox"),
                $"Could not find email with subject <{SharedConstants.IaEmailReviewerSubject}> sent to {_reviewer.Email}");
        }
    }
}
