using System;
using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.IndividualAssessment.Edit;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Survey;
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
    public class EditIndividualAssessmentDoneReviewWithCheckMark : IndividualAssessmentBaseTest
    {
        private static bool _classInitFailed;
        private static TeamResponse _team;
        private static CreateReviewerRequest _reviewer;
        private static IndividualAssessmentResponse _assessment;

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            try
            {
                var setup = new SetupTeardownApi(TestEnvironment);
                
                _reviewer = MemberFactory.GetReviewer();
                var reviewerResponse = setup.CreateReviewer(_reviewer).GetAwaiter().GetResult();
              
                _team = GetTeamForBatchEdit(setup, "BatchEditViewer");
                var individualDataResponse = GetIndividualAssessment(setup, _team, "BatchEditIAViewer_");
                var assessmentRequest = individualDataResponse.Item2;
                assessmentRequest.Members = _team.Members.Select(m => m.ToAddIndividualMemberRequest()).ToList();
                assessmentRequest.Members.First().Reviewers.Add(reviewerResponse.ToAddIndividualMemberRequest());
            
                _assessment = setup.CreateIndividualAssessment(assessmentRequest, SharedConstants.IndividualAssessmentType)
                    .GetAwaiter().GetResult();
                assessmentRequest.BatchId = _assessment.BatchId;
                _assessment = setup.CreateIndividualAssessment(assessmentRequest, SharedConstants.IndividualAssessmentType)
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
        public void IndividualAssessment_Edit_DoneReviewWithCheckmarkAndLock()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var iaEditPage = new IaEdit(Driver, Log);
            var surveyPage = new SurveyPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            var assessmentUid = _assessment.AssessmentList.FirstOrDefault().CheckForNull("assessmentUid is null.").AssessmentUid;
            iaEditPage.NavigateToPage(Company.Id, _team.Uid, assessmentUid);
            
            var checkMark = iaEditPage.GetCompletedTitle(_reviewer.Email);
            Assert.AreNotEqual("Completed", checkMark, "A check mark is present. The survey has already been taken");

            surveyPage.NavigateToUrl(GmailUtil.GetSurveyLink(
                SharedConstants.IaEmailReviewerSubject, _reviewer.Email, "inbox"));
            surveyPage.SelectReviewerRole(new List<string>{"Reviewer"});
            surveyPage.ConfirmIdentity();
            surveyPage.ClickStartSurveyButton();
            surveyPage.SubmitSurvey(7);
            surveyPage.ClickNextButton();
            surveyPage.ClickNextButton();
            surveyPage.ClickNextButton();
            surveyPage.ClickFinishButton();

            iaEditPage.NavigateToPage(Company.Id, _team.Uid, assessmentUid);

            Log.Info("Assert: Verify check mark shows and lock icon is in correct status under reviewer section");
            checkMark = iaEditPage.GetCompletedTitle(_reviewer.Email);
            var lockIcon = iaEditPage.GetTypeOfLockIcon(_reviewer.Email);

            Assert.AreEqual("Completed", checkMark, "A check mark is not present. The survey has not been taken");

            if (User.IsCompanyAdmin() || User.IsSiteAdmin() || User.IsPartnerAdmin())
            {
                Assert.AreEqual("openLockBtn", lockIcon, "Lock icon should not be locked.");
            }
            else
            {
                Assert.AreEqual("closeLockBtn", lockIcon, "Lock icon should not be open.");
            }
        }
    }
}