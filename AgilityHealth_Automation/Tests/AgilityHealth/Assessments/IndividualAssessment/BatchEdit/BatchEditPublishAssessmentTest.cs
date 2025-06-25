using System;
using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.IndividualAssessment;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.IndividualAssessment.BatchEdit;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AtCommon.Api;
using AtCommon.Dtos.Assessments;
using AtCommon.Dtos.IndividualAssessments;
using AtCommon.Dtos.Reports;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Assessments.IndividualAssessment.BatchEdit
{
    [TestClass]
    [TestCategory("TalentDevelopment"), TestCategory("Assessments")]
    public class BatchEditPublishAssessmentTest : IndividualAssessmentBaseTest
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
                
                _team = GetTeamForBatchEdit(setup, "BatchEditPublish");
                var individualDataResponse = IndividualAssessmentFactory.GetDraftIndividualAssessment(
                    Company.Id, User.CompanyName, _team.Uid, "BatchEditPublishIA_");
                
                var reviewerRequest = MemberFactory.GetReviewer();
                _reviewer = setup.CreateReviewer(reviewerRequest).GetAwaiter().GetResult();
                individualDataResponse.Members = _team.Members.Select(m => m.ToAddIndividualMemberRequest()).ToList();
                individualDataResponse.Members.First().Reviewers.Add(_reviewer.ToAddIndividualMemberRequest());

                _assessment = setup.CreateIndividualAssessment(individualDataResponse, SharedConstants.IndividualAssessmentType)
                    .GetAwaiter().GetResult();
            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        public void BatchEdit_Publish_SaveDraft()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var batchEditAssessmentPage = new BatchEditAssessmentPage(Driver, Log);
            var assessmentDashboardPage = new IndividualAssessmentDashboardPage(Driver, Log);
            var dashboardPage = new TeamDashboardPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            var teamId = dashboardPage.GetTeamIdFromLink(_team.Name).ToInt();
            teamAssessmentDashboard.NavigateToPage(teamId);

            var radarName = $"{_assessment.AssessmentName} - {_assessment.Participants.First().FullName()}";
            var statusDraft = assessmentDashboardPage.GetAssessmentStatus(radarName);

            //assert
            Log.Info("Assert: Verify that the assessment has been saved as a draft");
            Assert.AreEqual("Draft", statusDraft, "Assessment has not been drafted");

            batchEditAssessmentPage.NavigateToPage(Company.Id, _team.Uid, _assessment.BatchId, teamId.ToString());
            batchEditAssessmentPage.WaitForAssessmentDataLoaded();

            //assert

            batchEditAssessmentPage.ClickPublishButton();

            var statusPublished = assessmentDashboardPage.GetAssessmentStatus(radarName);

            //assert
            Log.Info("Assert: Verify that the assessment has been published");
            Assert.AreEqual("Open", statusPublished, "Assessment has not been published");
        }
    }
}