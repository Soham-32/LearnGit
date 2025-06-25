using System;
using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.IndividualAssessment.Edit;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Survey;
using AtCommon.Api;
using AtCommon.Dtos.Assessments;
using AtCommon.Dtos.Reports;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Assessments.IndividualAssessment.Edit
{
    [TestClass]
    [TestCategory("TalentDevelopment"), TestCategory("Assessments")]
    public class EditIndividualAssessmentAccessLinkTests : IndividualAssessmentBaseTest
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
             
                var reviewerRequest = MemberFactory.GetReviewer();
                _reviewer = setup.CreateReviewer(reviewerRequest).GetAwaiter().GetResult();
                
                _team = GetTeamForBatchEdit(setup, "BatchEditViewer");
                var individualDataResponse = GetIndividualAssessment(setup, _team, "BatchEditIAViewer_");
                var assessment = individualDataResponse.Item2;
                assessment.Members = _team.Members.Select(m => m.ToAddIndividualMemberRequest()).ToList();
                assessment.Members.First().Reviewers.Add(_reviewer.ToAddIndividualMemberRequest());

                _assessment = setup.CreateIndividualAssessment(assessment, SharedConstants.IndividualAssessmentType)
                    .GetAwaiter().GetResult();
                assessment.BatchId = _assessment.BatchId;
                _assessment = setup.CreateIndividualAssessment(assessment, SharedConstants.IndividualAssessmentType)
                    .GetAwaiter().GetResult();
            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }


        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        public void IndividualAssessment_Edit_AccessLink()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var iaEditPage = new IaEdit(Driver, Log);
            var surveyPage = new SurveyPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            var assessmentUid = _assessment.AssessmentList.FirstOrDefault().CheckForNull("assessmentUid is null.").AssessmentUid;
            iaEditPage.NavigateToPage(Company.Id, _team.Uid, assessmentUid);

            iaEditPage.ClickOnReviewerAccessLinkIcon(_reviewer.Email);

            Log.Info("Verify that a tooltip displays");
            Assert.AreEqual("Link copied to clipboard", iaEditPage.GetAccessLinkTooltip(), $"Access link tooltip should display with correct message. Expected: 'Link copied to clipboard' - Actual '{iaEditPage.GetAccessLinkTooltip()}'");

            iaEditPage.NavigateToUrl(CSharpHelpers.GetClipboard());
            surveyPage.SelectReviewerRole(new List<string>{"Reviewer"});

            Log.Info("Verify that user gets navigated to correct survey screen");
            Assert.AreEqual("Assessment - AH", Driver.Title, $"Survey screen should display. Expected: Assessment AH - Actual: {Driver.Title}");
            Assert.AreEqual($"Hello, {_reviewer.FullName()}", surveyPage.GetSurveyIdentity(), $"Survey screen should display for the correct user. Expected Hello, {_reviewer.FullName()} - Actual: {surveyPage.GetSurveyIdentity()}");
        }
    }
}
