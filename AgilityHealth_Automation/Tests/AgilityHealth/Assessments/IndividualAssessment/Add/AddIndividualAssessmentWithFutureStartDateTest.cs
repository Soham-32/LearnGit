using System;
using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.IndividualAssessment;
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
    public class AddIndividualAssessmentWithFutureStartDateTest : IndividualAssessmentBaseTest
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
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        public void IndividualAssessment_FutureStartDate()
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

            var assessment = IndividualAssessmentFactory.GetUiIndividualAssessment($"SaveDraftIA_{Guid.NewGuid()}");
            assessment.Members = _team.Members.Select(m => m.ToAddIndividualMemberRequest()).ToList();
            assessment.Start = DateTime.Today.AddDays(3);

            createIndividualAssessment1.WaitUntilLoaded();
            createIndividualAssessment1.FillInIndividualAssessmentInfo(assessment, SharedConstants.IndividualAssessmentType);

            createIndividualAssessment1.ClickNextButton();
            createIndividualAssessment2.WaitUntilLoaded();

            createIndividualAssessment2.ClickNextButton();
            createIndividualAssessment3.WaitUntilLoaded();

            createIndividualAssessment3.ClickNextButton();
            createIndividualAssessment4.ClickPublishTopButton();

            Log.Info("Assert: Verify that Assessment showing as Pending on Dashboard ");
            foreach (var member in assessment.Members)
            {
                Assert.AreEqual("Pending",
                    individualAssessmentDashboard.GetAssessmentStatus(
                        $"{assessment.AssessmentName} - {member.FullName()}"),
                    "Assessment status doesn't match");
                Assert.AreEqual("disc orange",
                    individualAssessmentDashboard.GetAssessmentIndicator(
                        $"{assessment.AssessmentName} - {member.FullName()}"),
                    "Assessment indicator doesn't match");
            }
        }
    }
}