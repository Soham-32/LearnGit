using System;
using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.IndividualAssessment.Edit;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Survey;
using AtCommon.Api;
using AtCommon.Dtos.Assessments;
using AtCommon.Dtos.IndividualAssessments;
using AtCommon.Dtos.Reports;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Assessments.IndividualAssessment.Edit
{
    [TestClass]
    [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
    [TestCategory("TalentDevelopment"), TestCategory("Assessments")]
    public class EditClosedIndividualAssessmentEditReviewerWithRoleTests : IndividualAssessmentBaseTest
    {
        private static CreateIndividualAssessmentRequest _assessmentRequest;
        private static IndividualAssessmentResponse _assessment;
        private static ReviewerResponse _reviewer;
        private static TeamResponse _team;
        private static bool _classInitFailed;
        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            try
            {
                var setup = new SetupTeardownApi(TestEnvironment);

                _team = GetTeamForBatchEdit(setup, "IA");
                var individualDataResponse = GetIndividualAssessment(setup, _team, "ReviewRoleEditIASection_");
                _assessmentRequest = individualDataResponse.Item2;
                _assessmentRequest.Members = _team.Members.Select(m => m.ToAddIndividualMemberRequest()).ToList();

                _reviewer = setup.CreateReviewer(MemberFactory.GetReviewer()).GetAwaiter().GetResult();
                _assessmentRequest.Members.First().Reviewers.Add(_reviewer.ToAddIndividualMemberRequest());

                _assessment = setup.CreateIndividualAssessment(_assessmentRequest, SharedConstants.IndividualAssessmentType).GetAwaiter().GetResult();
            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }

        [TestMethod]
        public void ClosedIndividualAssessment_EditReviewerRoles()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var iaEditPage = new IaEdit(Driver, Log);
            var surveyPage = new SurveyPage(Driver, Log);
            var roles = new List<string>
            {
                "Reviewer"
            };

            surveyPage.NavigateToUrl(GmailUtil.GetSurveyLink(
            SharedConstants.IaEmailReviewerSubject, _reviewer.Email, "inbox"));
            surveyPage.SelectReviewerRole(roles);
            surveyPage.ConfirmIdentity();
            surveyPage.ClickStartSurveyButton();
            surveyPage.SubmitSurvey(7);
            surveyPage.ClickNextButton();
            surveyPage.ClickNextButton();
            surveyPage.ClickNextButton();
            surveyPage.ClickFinishButton();

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            var assessmentUid = _assessment.AssessmentList.FirstOrDefault().CheckForNull().AssessmentUid;
            iaEditPage.NavigateToPage(Company.Id, _team.Uid, assessmentUid);

            Log.Info($"Remove {roles} from the reviewer");
            iaEditPage.ClickOnEditReviewer(_reviewer.Email);
            iaEditPage.RemoveRoleFromReviewer(roles.First());
            Assert.IsFalse(iaEditPage.DoesRoleOfReviewerDisplay(roles.First()), $"{roles.First()} does exist");

            Log.Info($"Add {roles} for reviewer");
            iaEditPage.ClickOnEditReviewer(_reviewer.Email);

            roles.Add("Customer");

            iaEditPage.EditRoles(roles);
            var updatedRole = iaEditPage.GetRolesOfReviewer(_reviewer.Email).StringToList();
            Assert.That.ListsAreEqual(updatedRole, roles, "Added Roles do not match");
        }
    }
}