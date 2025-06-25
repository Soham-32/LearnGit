using System;
using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.IndividualAssessment;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.IndividualAssessment.Create;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AtCommon.Api;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Assessments.IndividualAssessment.Add
{
    [TestClass]
    [TestCategory("TalentDevelopment"), TestCategory("Assessments")]
    public class PublishIndividualAssessmentTest : IndividualAssessmentBaseTest
    {
        private static bool _classInitFailed;
        private static TeamResponse _team;

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            try
            {
                var setup = new SetupTeardownApi(TestEnvironment);
                _team = GetTeamForIndividualAssessment(setup, "IA", 2);
            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("Smoke")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        public void IndividualAssessment_ReviewAndPublish()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var dashboardPage = new TeamDashboardPage(Driver, Log);
            var individualAssessmentDashboard = new IndividualAssessmentDashboardPage(Driver, Log);
            var createIndividualAssessment1 = new CreateIndividualAssessment1CreateAssessmentPage(Driver, Log);
            var createIndividualAssessment2 = new CreateIndividualAssessment2AddReviewersPage(Driver, Log);
            var createIndividualAssessment3 = new CreateIndividualAssessment3InviteViewersPage(Driver, Log);
            var createIndividualAssessment4 = new CreateIndividualAssessment4AddReviewAndPublishPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            var teamId = dashboardPage.GetTeamIdFromLink(_team.Name);
            createIndividualAssessment1.NavigateToPage(Company.Id, _team.Uid, teamId);

            var assessment = IndividualAssessmentFactory.GetUiIndividualAssessment($"PublishIA_{Guid.NewGuid()}");
            assessment.Members = _team.Members.Select(m => m.ToAddIndividualMemberRequest()).ToList();

            createIndividualAssessment1.WaitUntilLoaded();

            //fill out assessment
            createIndividualAssessment1.FillInIndividualAssessmentInfo(assessment, SharedConstants.IndividualAssessmentType);

            createIndividualAssessment1.ClickNextButton();
            
            createIndividualAssessment2.WaitUntilLoaded();
            createIndividualAssessment2.ClickNextButton();

            createIndividualAssessment3.WaitUntilLoaded();
            createIndividualAssessment3.ClickNextButton();

            //get info from page
            createIndividualAssessment4.WaitUntilLoaded();
            var actualAssessmentInfo = createIndividualAssessment4.GetAssessmentInfo();
            var actualAssessmentType = createIndividualAssessment4.GetAssessmentType();

            var timeDifferenceStart = assessment.Start.Subtract(actualAssessmentInfo.Start);
            var timeDifferenceEnd = assessment.End.Subtract(actualAssessmentInfo.End);

            //validate
            Log.Info("Assert: 'Review & Publish' shows correct info");
            Assert.AreEqual(assessment.AssessmentName, actualAssessmentInfo.AssessmentName, "Assessment names do not match");
            Assert.AreEqual(assessment.PointOfContact, actualAssessmentInfo.PointOfContact, "Point of contact names do not match");
            Assert.AreEqual(assessment.PointOfContactEmail, actualAssessmentInfo.PointOfContactEmail, "Point of contact emails do not match");
            Assert.IsTrue(timeDifferenceStart < TimeSpan.Parse("00:02:00") && timeDifferenceStart > TimeSpan.Parse("-00:02:00"), $"Start Date displays incorrectly - Expected:<{assessment.Start}>. Actual:<{actualAssessmentInfo.Start}>.");
            Assert.IsTrue(timeDifferenceEnd < TimeSpan.Parse("00:02:00") && timeDifferenceEnd > TimeSpan.Parse("-00:02:00"), $"Start Date displays incorrectly - Expected:<{assessment.Start}>. Actual:<{actualAssessmentInfo.Start}>.");
            Assert.AreEqual(SharedConstants.IndividualAssessmentType, actualAssessmentType, "Assessment types do not match");
            
            //publish
            createIndividualAssessment4.ClickPublishBottomButton();

            Log.Info("Assert: Verify assessment has a published status on assessment dashboard");
            foreach (var participant in assessment.Members)
            {
                var assessmentName = $"{assessment.AssessmentName} - {participant.FirstName} {participant.LastName}";
                Assert.AreEqual("Open", individualAssessmentDashboard.GetAssessmentStatus(
                        assessmentName),
                        $"Assessment status doesn't match for {participant.FirstName}");
                Assert.AreEqual("disc green",
                    individualAssessmentDashboard.GetAssessmentIndicator(
                        assessmentName),
                    $"Assessment indicator doesn't match for {participant.FirstName}");
                Assert.IsFalse(individualAssessmentDashboard.DoesDraftAssessmentDisplay(assessmentName), $"Draft assessment shouldn't display for {participant.FirstName}");
            }

            var emailBox = User.IsOrganizationalLeader() ? "" : "Inbox";
            Log.Info("Assert: Verify that email for email was sent to the participant");
            foreach (var participant in assessment.Members)
            {
                Assert.IsTrue(GmailUtil.DoesMemberEmailExist(
                        SharedConstants.IaEmailParticipantSubject, participant.Email, emailBox),
                    $"Could not find email with subject <{SharedConstants.IaEmailParticipantSubject}> sent to {participant.Email}");
            }
        }
    }
}
