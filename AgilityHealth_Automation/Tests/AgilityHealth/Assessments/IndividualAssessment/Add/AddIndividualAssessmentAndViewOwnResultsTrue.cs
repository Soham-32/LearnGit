using System;
using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.IndividualAssessment;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.IndividualAssessment.Create;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
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
    public class AddIndividualAssessmentAndViewOwnResultsTrue : IndividualAssessmentBaseTest
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
        [TestCategory("Critical")]
        [TestCategory("KnownDefect")] //Bug Id: 53429
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        public void IndividualAssessment_ViewOwnResults_True()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var dashboardPage = new TeamDashboardPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var createIndividualAssessment1 = new CreateIndividualAssessment1CreateAssessmentPage(Driver, Log);
            var createIndividualAssessment2 = new CreateIndividualAssessment2AddReviewersPage(Driver, Log);
            var createIndividualAssessment3 = new CreateIndividualAssessment3InviteViewersPage(Driver, Log);
            var createIndividualAssessment4 = new CreateIndividualAssessment4AddReviewAndPublishPage(Driver, Log);
            var iAssessmentDashboard = new IndividualAssessmentDashboardPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            var teamId = dashboardPage.GetTeamIdFromLink(_team.Name);
            createIndividualAssessment1.NavigateToPage(Company.Id, _team.Uid, teamId);

            var assessment = IndividualAssessmentFactory.GetUiIndividualAssessment($"ViewOwnResultsTrue_{Guid.NewGuid()}");
            assessment.Members = _team.Members.Select(m => m.ToAddIndividualMemberRequest()).ToList();
            
            createIndividualAssessment1.WaitUntilLoaded();
            createIndividualAssessment1.FillInIndividualAssessmentInfo(assessment, SharedConstants.IndividualAssessmentType);

            createIndividualAssessment1.ClickNextButton();

            createIndividualAssessment2.WaitUntilLoaded();
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

            topNav.LogOut();

            login.LoginToApplication(_member.Username, _member.Password);

            teamAssessmentDashboard.NavigateToPage(teamId.ToInt());

            teamAssessmentDashboard.SwitchToIndividualAssessmentView();
            iAssessmentDashboard.WaitUntilLoaded();

            Log.Info("Assert: Verify able to access the assessment");
            var teamMember = _team.Members.First(t => t.Email == _member.Username).CheckForNull().FullName();
            Assert.IsTrue(iAssessmentDashboard.IsAssessmentPresent($"{assessment.AssessmentName} - {teamMember}"),
                $"Individual Assessment doesn't exist with name: <{assessment.AssessmentName} - {teamMember}>");
        }
    }
}