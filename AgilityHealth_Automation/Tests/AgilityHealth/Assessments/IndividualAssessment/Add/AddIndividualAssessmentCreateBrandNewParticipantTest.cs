using System;
using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.IndividualAssessment.Create;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
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
    public class AddIndividualAssessmentCreateBrandNewParticipantTest : IndividualAssessmentBaseTest
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
        [TestCategory("KnownDefectAsOL")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        [TestCategory("Critical")]
        public void IndividualAssessment_CreateBrandNewParticipant()
        {
            VerifySetup(_classInitFailed);
            
            var login = new LoginPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var createIndividualAssessment1 = new CreateIndividualAssessment1CreateAssessmentPage(Driver, Log);
            var createIndividualAssessment2 = new CreateIndividualAssessment2AddReviewersPage(Driver, Log);
            const string participantGroups = "Technical";

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            dashBoardPage.GridTeamView();
            createIndividualAssessment1.NavigateToPage(Company.Id, _team.Uid);

            var assessment = IndividualAssessmentFactory.GetUiIndividualAssessment($"EditCreateIA_{Guid.NewGuid()}");
            assessment.Members = _team.Members.Select(m => m.ToAddIndividualMemberRequest()).ToList();
            
            createIndividualAssessment1.WaitUntilLoaded();
            createIndividualAssessment1.FillInIndividualAssessmentInfo(assessment, SharedConstants.IndividualAssessmentType);
            createIndividualAssessment1.ClickNextButton();

            createIndividualAssessment2.WaitUntilLoaded();
            createIndividualAssessment2.CreateNewParticipant();

            var participant = MemberFactory.GetValidIndividualPostTeamMember();

            createIndividualAssessment2.CreateNewParticipant(participant, participantGroups);

            Log.Info("Assert: Verify that new add participant displays in participants list");
            Assert.IsTrue(createIndividualAssessment2.DoesParticipantDisplay(participant.Email), $"Newly added participant {participant.Email} should display in participants list");
            Assert.IsTrue(createIndividualAssessment2.IsParticipantGroupDisplayed(participant.Email, participantGroups), $"Newly added participant {participant.Email} and participant group {participantGroups} should display in participants list");
        }
    }
}