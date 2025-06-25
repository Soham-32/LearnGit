using System;
using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.IndividualAssessment;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.IndividualAssessment.Edit;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Survey;
using AtCommon.Api;
using AtCommon.Dtos.Assessments;
using AtCommon.Dtos.IndividualAssessments;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Assessments.IndividualAssessment.Edit
{
    [TestClass]
    [TestCategory("TalentDevelopment"), TestCategory("Assessments")]
    public class EditIndividualAssessmentVerifyReviewerRoleFromAssessmentTest1 : IndividualAssessmentBaseTest
    {
        private static bool _classInitFailed;
        private static TeamResponse _team;
        private static CreateIndividualAssessmentRequest _assessmentRequest;

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            try
            {
                var setup = new SetupTeardownApi(TestEnvironment);
                var admin = User.IsMember() ? TestEnvironment.UserConfig.GetUserByDescription("user 3") : User;

                _team = GetTeamForBatchEdit(setup, "IA");
                var individualDataResponse = GetIndividualAssessment(setup, _team, "ReviewRoleEditIASection_");
                _assessmentRequest = individualDataResponse.Item2;
                _assessmentRequest.Members = _team.Members.Select(m => m.ToAddIndividualMemberRequest()).ToList();

                var reviewer = setup.CreateReviewer(MemberFactory.GetReviewer(), admin).GetAwaiter().GetResult();
                _assessmentRequest.Members.First().Reviewers.Add(reviewer.ToAddIndividualMemberRequest());

                var assessment = setup.CreateIndividualAssessment(_assessmentRequest, SharedConstants.IndividualAssessmentType, admin)
                    .GetAwaiter().GetResult();
                _assessmentRequest.BatchId = assessment.BatchId;
                setup.CreateIndividualAssessment(_assessmentRequest, SharedConstants.IndividualAssessmentType, admin)
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
        public void IndividualAssessment_Edit_ReviewerRole_Single()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var dashboardPage = new TeamDashboardPage(Driver, Log);
            var iaDashboardPage = new IndividualAssessmentDashboardPage(Driver, Log);
            var surveyPage = new SurveyPage(Driver, Log);
            var iaEditPage = new IaEdit(Driver, Log);

            var surveyLink = GmailUtil.GetSurveyLink(SharedConstants.IaEmailReviewerSubject,
                _assessmentRequest.Members.First().Reviewers.First().Email, "unread", _assessmentRequest.PointOfContact);
            surveyPage.NavigateToUrl(surveyLink);

            surveyPage.SelectReviewerRole(new List<string>{"Reviewer"});
            surveyPage.ConfirmIdentity();
            surveyPage.ClickStartSurveyButton();

            surveyPage.SubmitRandomSurvey();

            surveyPage.ClickNextButton();
            surveyPage.ClickNextButton();
            surveyPage.ClickNextButton();
            surveyPage.ClickFinishButton();

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            var teamId = dashboardPage.GetTeamIdFromLink(_team.Name).ToInt();
            iaDashboardPage.NavigateToPage(teamId);
            iaDashboardPage.ClickOnAssessmentType(SharedConstants.IndividualAssessmentType);

            iaDashboardPage.SelectRadarLink($"{_assessmentRequest.AssessmentName} - {_assessmentRequest.Members.FirstOrDefault().FullName()}", "Edit");

            var role = iaEditPage.GetReviewerRole();
            Log.Info("Verify reviewer role from assessment shows");
            Assert.AreEqual("Reviewer", role, "Expected and actual role shown on the individual assessment edit page do not match");
        }

    }
}