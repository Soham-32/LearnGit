using System;
using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.IndividualAssessment.Edit;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Survey;
using AtCommon.Api;
using AtCommon.Dtos.Reports;
using AtCommon.Dtos.Teams;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Assessments.IndividualAssessment.Edit
{
    [TestClass]
    [TestCategory("TalentDevelopment"), TestCategory("Assessments")]
    public class EditIndividualAssessmentTakeAssessmentButton : IndividualAssessmentBaseTest
    {
        private static bool _classInitFailed;
        private static TeamResponse _team;
        private static IndividualAssessmentResponse _assessment;

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            try
            {
                var setup = new SetupTeardownApi(TestEnvironment);
               
                _team = GetTeamForBatchEdit(setup, "EditIA");
                var individualDataResponse = GetIndividualAssessment(setup, _team, "TakeIA_");
                var assessmentRequest = individualDataResponse.Item2;
                assessmentRequest.Members = _team.Members.Select(m => m.ToAddIndividualMemberRequest()).ToList();

                _assessment = setup.CreateIndividualAssessment(assessmentRequest, SharedConstants.IndividualAssessmentType).GetAwaiter().GetResult();
            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        public void IndividualAssessment_Edit_TakeAssessment()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var iaEditPage = new IaEdit(Driver, Log);
            var surveyPage = new SurveyPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            var assessmentUid = _assessment.AssessmentList.FirstOrDefault().CheckForNull("assessmentUid is null.").AssessmentUid;
            iaEditPage.NavigateToPage(Company.Id, _team.Uid, assessmentUid);

            Log.Info("Assert: Verify that 'Take Assessment' button is active");
            var enabledButton = iaEditPage.IsTakeAssessmentButtonEnabled();
            Assert.IsTrue(enabledButton, "Take assessment button is enabled");

            iaEditPage.ClickTakeAssessment();

            surveyPage.ConfirmIdentity();
            surveyPage.ClickStartSurveyButton();
            surveyPage.SubmitSurvey(7);
            surveyPage.ClickNextButton();
            surveyPage.ClickNextButton();
            surveyPage.ClickNextButton();
            surveyPage.ClickFinishButton();

            iaEditPage.NavigateToPage(Company.Id, _team.Uid, assessmentUid);

            Log.Info("Assert: Verify that 'Take Assessment' button is disabled");
            var disabledButton = iaEditPage.IsTakeAssessmentButtonEnabled();
            var takenTime = iaEditPage.TakeAssessmentTakenTitle();

            Assert.IsFalse(disabledButton, "Take assessment button is enabled");
            Assert.IsTrue(takenTime.Contains("Taken"), "Taken assessment does not have time");
        }
    }
}