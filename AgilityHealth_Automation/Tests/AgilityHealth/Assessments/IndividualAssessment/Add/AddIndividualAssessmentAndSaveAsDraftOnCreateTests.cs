using System;
using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.IndividualAssessment;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.IndividualAssessment.BatchEdit;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.IndividualAssessment.Create;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AtCommon.Api;
using AtCommon.Dtos.IndividualAssessments;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Assessments.IndividualAssessment.Add
{
    [TestClass]
    [TestCategory("TalentDevelopment"), TestCategory("Assessments")]
    public class AddIndividualAssessmentAndSaveAsDraftOnCreateTests : IndividualAssessmentBaseTest
    {
        private static bool _classInitFailed;
        private static TeamResponse _team;

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            try
            {
                var setup = new SetupTeardownApi(TestEnvironment);
                _team = GetTeamForIndividualAssessment(setup, $"IA{CSharpHelpers.RandomNumber()}", 2);
            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }

        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        [TestCategory("Critical")]
        public void IndividualAssessment_SaveAsDraftOnCreate()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var dashboardPage = new TeamDashboardPage(Driver, Log);
            var individualAssessmentDashboard = new IndividualAssessmentDashboardPage(Driver, Log);
            var createIndividualAssessment1 = new CreateIndividualAssessment1CreateAssessmentPage(Driver, Log);
            var createIndividualAssessment2 = new CreateIndividualAssessment2AddReviewersPage(Driver, Log);
            var createIndividualAssessment3 = new CreateIndividualAssessment3InviteViewersPage(Driver, Log);
            var createIndividualAssessment4 = new CreateIndividualAssessment4AddReviewAndPublishPage(Driver, Log);
            var batchEditAssessmentPage = new BatchEditAssessmentPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            var teamId = dashboardPage.GetTeamIdFromLink(_team.Name);
            createIndividualAssessment1.NavigateToPage(Company.Id, _team.Uid, teamId);

            var assessment = IndividualAssessmentFactory.GetUiIndividualAssessment($"SaveDraftIA_{Guid.NewGuid()}");
            assessment.Members = _team.Members.Select(m => m.ToAddIndividualMemberRequest()).ToList();
            createIndividualAssessment1.WaitUntilLoaded();
            createIndividualAssessment1.FillInIndividualAssessmentInfo(assessment, SharedConstants.IndividualAssessmentType);

            createIndividualAssessment1.ClickNextButton();
            createIndividualAssessment2.WaitUntilLoaded();

            createIndividualAssessment2.ClickNextButton();
            createIndividualAssessment3.WaitUntilLoaded();

            createIndividualAssessment3.ClickNextButton();
            createIndividualAssessment4.ClickSaveAsDraftButton();

            Log.Info("Verify that draft Rollup & Participant radar is displayed on dashboard.");

            Assert.IsTrue(individualAssessmentDashboard.IsRadarPresent(assessment.AssessmentName + " - Roll up"),
                $"Draft roll up radar doesn't exists with name : {assessment.AssessmentName} - Roll up");
            foreach (var member in assessment.Members)
            {
                Assert.AreEqual("Draft", individualAssessmentDashboard.GetAssessmentStatus($"{assessment.AssessmentName} - {member.FullName()}"), "Assessment status doesn't match");
                Assert.AreEqual("disc dark-grey", individualAssessmentDashboard.GetAssessmentIndicator($"{assessment.AssessmentName} - {member.FullName()}"), "Assessment indicator doesn't match");
                Assert.IsTrue(individualAssessmentDashboard.IsRadarPresent(assessment.AssessmentName + " - " + member.FirstName + " " + member.LastName),
                    $"Draft participant radar doesn't exists with name : {assessment.AssessmentName + " - " + member.FirstName + " " + member.LastName}");
            }

            Log.Info("Click on radar edit icon & publish the draft assessment");
            individualAssessmentDashboard.SelectRadarLink(assessment.AssessmentName + " - Roll up", "Edit");
            batchEditAssessmentPage.WaitForAssessmentDataLoaded();
            batchEditAssessmentPage.ClickPublishButton();

            Log.Info("Verify that Rollup & Participant radar is displayed on dashboard after user publish the draft assessment.");
            Assert.IsTrue(individualAssessmentDashboard.IsRadarPresent(assessment.AssessmentName + " - Roll up"),
                $"Roll up radar doesn't exists with name : {assessment.AssessmentName} - Roll up");
            foreach (var member in assessment.Members)
            {
                Assert.AreEqual("Open", individualAssessmentDashboard.GetAssessmentStatus(assessment.AssessmentName + " - " + member.FirstName + " " + member.LastName), "Assessment has not been published");
                Assert.AreEqual("disc green", individualAssessmentDashboard.GetAssessmentIndicator($"{assessment.AssessmentName} - {member.FullName()}"), "Assessment indicator doesn't match");
                Assert.IsTrue(individualAssessmentDashboard.IsRadarPresent(assessment.AssessmentName + " - " + member.FirstName + " " + member.LastName),
                    $"Participant radar doesn't exists with name : {assessment.AssessmentName + " - " + member.FirstName + " " + member.LastName}");
            }
        }
    }
}