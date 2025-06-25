using System;
using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.IndividualAssessment.BatchEdit;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Survey;
using AgilityHealth_Automation.Utilities;
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
    public class BatchEditAccessLinkTests : IndividualAssessmentBaseTest
    {
        private static bool _classInitFailed;
        private static TeamResponse _team;
        private static CreateReviewerRequest _reviewer;
        private static IndividualAssessmentResponse _assessment;
        private static IndividualAssessmentResponse _assessmentResponse;
        private static CreateIndividualAssessmentRequest _assessmentRequest;

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            try
            {
                var setup = new SetupTeardownApi(TestEnvironment);
                
                _team = GetTeamForBatchEdit(setup, "BatchEditAccessLink");
                _reviewer = MemberFactory.GetReviewer();
                var reviewerResponse = setup.CreateReviewer(_reviewer).GetAwaiter().GetResult();
                var individualDataResponse = GetIndividualAssessment(setup, _team, "BatchEditAccessLink_");
                _assessmentRequest = individualDataResponse.Item2;
                _assessmentResponse = individualDataResponse.Item3;
                _assessmentRequest.Members = _team.Members.Select(m => m.ToAddIndividualMemberRequest()).ToList();
                _assessmentRequest.Members.First().Reviewers.Add(reviewerResponse.ToAddIndividualMemberRequest());
                _assessmentRequest.BatchId = _assessmentResponse.BatchId;

                _assessment = setup.CreateIndividualAssessment(_assessmentRequest, SharedConstants.IndividualAssessmentType)
                    .GetAwaiter().GetResult();
            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public void BatchEdit_AccessLink()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var dashboardPage = new TeamDashboardPage(Driver, Log);
            var batchEditAssessmentPage = new BatchEditAssessmentPage(Driver, Log);
            var batchEditParticipantReviewerPage = new BatchEditParticipantReviewerPage(Driver, Log);
            var surveyPage = new SurveyPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            var teamId = dashboardPage.GetTeamIdFromLink(_team.Name);
            batchEditAssessmentPage.NavigateToPage(Company.Id, _team.Uid, _assessment.BatchId, teamId);
            batchEditAssessmentPage.WaitForAssessmentDataLoaded();
            batchEditAssessmentPage.ClickParticipantsReviewersTab();
            batchEditParticipantReviewerPage.ExpandCollapseParticipantsAndReviewersWithReviewerEmail(_reviewer.Email);
            batchEditParticipantReviewerPage.ClickOnReviewerAccessLinkButton(_reviewer.Email);

            Assert.AreEqual("Link copied to clipboard", batchEditParticipantReviewerPage.GetAccessLinkTooltip(), $"Access link tooltip should display with correct message. Expected: 'Link copied to clipboard' - Actual '{batchEditParticipantReviewerPage.GetAccessLinkTooltip()}'");

            Driver.NavigateToPage(CSharpHelpers.GetClipboard());
            surveyPage.SelectReviewerRole(new List<string> { "Reviewer" });

            Log.Info("Verify that user gets navigated to correct survey screen");
            Assert.AreEqual("Assessment - AH", Driver.Title, $"Survey screen should display. Expected: Assessment AH - Actual: {Driver.Title}");
            Assert.AreEqual($"Hello, {_reviewer.FullName()}", surveyPage.GetSurveyIdentity(), $"Survey screen should display for the correct user. Expected Hello, {_reviewer.FullName()} - Actual: {surveyPage.GetSurveyIdentity()}");
        }
    }
}
