using System;
using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.IndividualAssessment.Common;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.IndividualAssessment.Create;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.IndividualAssessment.Edit;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AtCommon.Api;
using AtCommon.Dtos;
using AtCommon.Dtos.Assessments;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Assessments.IndividualAssessment.Add
{
    [TestClass]
    [TestCategory("TalentDevelopment"), TestCategory("Assessments")]
    public class AddOwnReviewerIndividualAssessmentTests : IndividualAssessmentBaseTest
    {
        private static bool _classInitFailed;
        private static TeamResponse _team;
        private static CreateReviewerRequest _reviewer;
        private static User _member;

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            _member = TestEnvironment.UserConfig.GetUserByDescription("member");

            try
            {
                var setup = new SetupTeardownApi(TestEnvironment);
                _team = GetTeamForIndividualAssessment(setup, "IA", 1, null,_member);
                
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
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        public void IndividualAssessment_AddOwnReviewer()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var createIndividualAssessment1 = new CreateIndividualAssessment1CreateAssessmentPage(Driver, Log);
            var createIndividualAssessment2 = new CreateIndividualAssessment2AddReviewersPage(Driver, Log);
            var createIndividualAssessment3 = new CreateIndividualAssessment3InviteViewersPage(Driver, Log);
            var createIndividualAssessment4 = new CreateIndividualAssessment4AddReviewAndPublishPage(Driver, Log);
            var iaEditPage = new IaEdit(Driver, Log);
            var addReviewerModal = new AddReviewerModal(Driver, Log);
            var dashboardPage = new TeamDashboardPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            var teamId = dashboardPage.GetTeamIdFromLink(_team.Name);

            createIndividualAssessment1.NavigateToPage(Company.Id, _team.Uid, teamId);

            var assessment = IndividualAssessmentFactory.GetUiIndividualAssessment($"AddReviewerIA_{Guid.NewGuid()}", true);
            assessment.Members = _team.Members.Select(m => m.ToAddIndividualMemberRequest()).ToList();
            assessment.End = DateTime.Today.AddDays(3);
            
            createIndividualAssessment1.WaitUntilLoaded();
            createIndividualAssessment1.FillInIndividualAssessmentInfo(assessment, SharedConstants.IndividualAssessmentType);

            createIndividualAssessment1.ClickNextButton();
            
            createIndividualAssessment2.WaitUntilLoaded();
            createIndividualAssessment2.CheckToInviteOwnReviewers(assessment);
            createIndividualAssessment2.ClickNextButton();

            createIndividualAssessment3.WaitUntilLoaded();
            createIndividualAssessment3.ClickNextButton();

            //publish
            createIndividualAssessment4.ClickPublishBottomButton();

            Log.Info("Assert: Verify that email for email was sent to the participant");
            const string expectedSubjectParticipant = SharedConstants.IaEmailParticipantSubject;
            var filterLabel = "";

            foreach (var participant in assessment.Members)
            {
                filterLabel = participant.Email != _member.Username ? "Inbox" : "";
                Assert.IsTrue(GmailUtil.DoesMemberEmailExist(expectedSubjectParticipant, participant.Email, filterLabel),
                    $"Could not find email with subject <{expectedSubjectParticipant}> sent to {participant.Email}");
            }

            var loginLink = GmailUtil.GetLoginLink(expectedSubjectParticipant, _member.Username, filterLabel, assessment.PointOfContactEmail);
            login.NavigateToUrl(loginLink);

            login.LoginToApplication(_member.Username, _member.Password);

            //assert this is correct assessment
            Assert.AreEqual(assessment.AssessmentName.ToLower(), iaEditPage.GetAssessmentName().ToLower(), "Assessment name does not match.");
            
            iaEditPage.ClickAddReviewerButton();
            addReviewerModal.WaitUntilLoaded();
            addReviewerModal.AddReviewersBySearchingInModal(_reviewer);
            
            Log.Info("Assert: Verify email sent to reviewer");
            Assert.IsTrue(GmailUtil.DoesMemberEmailExist(SharedConstants.IaEmailReviewerSubject, 
                    _reviewer.Email, "Inbox"),
                    $"Could not find email with subject <{SharedConstants.IaEmailReviewerSubject}> sent to {_reviewer.Email}");
        }
    }
}