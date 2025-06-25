using System;
using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.IndividualAssessment.Create;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Team.Edit;
using AgilityHealth_Automation.Utilities;
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
    public class AddIndividualAssessmentCreateNewParticipantTest : IndividualAssessmentBaseTest
    {
        private static bool _classInitFailed;
        private static CreateReviewerRequest _reviewer;
        private static TeamResponse _team;
        private static string _participant;

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
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        public void IndividualAssessment_CreateNewParticipant()
        {
            VerifySetup(_classInitFailed);
            
            var login = new LoginPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var createIndividualAssessment1 = new CreateIndividualAssessment1CreateAssessmentPage(Driver, Log);
            var createIndividualAssessment2 = new CreateIndividualAssessment2AddReviewersPage(Driver, Log);
            var createIndividualAssessment3 = new CreateIndividualAssessment3InviteViewersPage(Driver, Log);
            var createIndividualAssessment4 = new CreateIndividualAssessment4AddReviewAndPublishPage(Driver, Log);
            var editTeamMemberPage = new EditTeamTeamMemberPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            dashBoardPage.GridTeamView();
            var teamId = dashBoardPage.GetTeamIdFromLink(_team.Name).ToInt();
            createIndividualAssessment1.NavigateToPage(Company.Id, _team.Uid);

            var assessment = IndividualAssessmentFactory.GetUiIndividualAssessment($"EditCreateIA_{Guid.NewGuid()}");
            assessment.Members = _team.Members.Select(m => m.ToAddIndividualMemberRequest()).ToList();
            
            createIndividualAssessment1.WaitUntilLoaded();
            createIndividualAssessment1.FillInIndividualAssessmentInfo(assessment, SharedConstants.IndividualAssessmentType);
            createIndividualAssessment1.ClickNextButton();

            createIndividualAssessment2.WaitUntilLoaded();
            createIndividualAssessment2.CreateNewParticipant();

            _participant = User.IsOrganizationalLeader() ? Constants.StakeholderEmail1 : Constants.TeamMemberEmail1;
            
            createIndividualAssessment2.SelectNewParticipant(_participant);
            createIndividualAssessment2.ClickCreateParticipantButton();

            Log.Info("Assert: Verify that new add participant displays in reviewers list");
            Assert.IsTrue(createIndividualAssessment2.DoesParticipantDisplay(_participant), $"New added participant {_participant} should display in reviewers list");

            createIndividualAssessment2.ClickNextButton();
            createIndividualAssessment3.ClickNextButton();
            createIndividualAssessment4.WaitUntilLoaded();
            createIndividualAssessment4.ClickPublishBottomButton();

            editTeamMemberPage.NavigateToPage(teamId);

            Assert.IsTrue(editTeamMemberPage.IsTeamMemberDisplayed(_participant), "New added participant should be added automatically to the team member list");
        }
    }
}