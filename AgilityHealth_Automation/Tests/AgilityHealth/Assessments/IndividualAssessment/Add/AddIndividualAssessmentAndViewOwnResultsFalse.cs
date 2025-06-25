using System;
using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.IndividualAssessment.Create;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Common;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AtCommon.Api;
using AtCommon.Dtos;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Assessments.IndividualAssessment.Add
{
    [TestClass]
    [TestCategory("TalentDevelopment"), TestCategory("Assessments")]
    public class AddIndividualAssessmentAndViewOwnResultsFalse : IndividualAssessmentBaseTest
    {
        private static bool _classInitFailed;
        private static TeamResponse _team;
        private static User _member;

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            _member = TestEnvironment.UserConfig.GetUserByDescription("member");

            try
            {
                var setup = new SetupTeardownApi(TestEnvironment);
                _team = GetTeamForIndividualAssessment(setup, "IA", 1, null, _member);
            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }
           
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        public void IndividualAssessment_ViewOwnResults_False()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var dashboardPage = new TeamDashboardPage(Driver, Log);
            var createIndividualAssessment1 = new CreateIndividualAssessment1CreateAssessmentPage(Driver, Log);
            var createIndividualAssessment2 = new CreateIndividualAssessment2AddReviewersPage(Driver, Log);
            var createIndividualAssessment3 = new CreateIndividualAssessment3InviteViewersPage(Driver, Log);
            var createIndividualAssessment4 = new CreateIndividualAssessment4AddReviewAndPublishPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            var teamId = dashboardPage.GetTeamIdFromLink(_team.Name);
            
            createIndividualAssessment1.NavigateToPage(Company.Id, _team.Uid, teamId);

            var assessment = IndividualAssessmentFactory.GetUiIndividualAssessment($"ViewOwnResultsFalse_{Guid.NewGuid()}", false, false);
            assessment.Members = _team.Members.Select(m => m.ToAddIndividualMemberRequest()).ToList();

            createIndividualAssessment1.WaitUntilLoaded();
            createIndividualAssessment1.FillInIndividualAssessmentInfo(assessment, SharedConstants.IndividualAssessmentType);

            createIndividualAssessment1.ClickNextButton();
            createIndividualAssessment2.WaitUntilLoaded();
           
            createIndividualAssessment2.CheckboxForViewOwnResults(assessment);
            createIndividualAssessment2.ClickNextButton();

            createIndividualAssessment3.WaitUntilLoaded();
            createIndividualAssessment3.ClickNextButton();

            //publish
            createIndividualAssessment4.WaitUntilLoaded();
            createIndividualAssessment4.ClickPublishBottomButton();

            Log.Info("Assert: Verify that email for email was sent to the participant");
            foreach (var participant in assessment.Members)
            {
                var filterLabel = participant.Email != _member.Username ? "Inbox" : "";
                Assert.IsTrue(GmailUtil.DoesMemberEmailExist(SharedConstants.IaEmailParticipantSubject, participant.Email, filterLabel),
                    $"Could not find email with subject <{SharedConstants.IaEmailParticipantSubject}> sent to {participant.Email}");
            }

            dashboardPage.NavigateToPage(Company.Id);
            topNav.LogOut();

            login.LoginToApplication(_member.Username, _member.Password);

            dashboardPage.GridTeamView();

            Log.Info("Assert: Verify that participant is unable to access team");
            var teamExistence = dashboardPage.DoesTeamDisplay(_team.Name);

            Assert.IsFalse(teamExistence,"Participant is able to access team when they shouldn't be able to");
        }
    }
}