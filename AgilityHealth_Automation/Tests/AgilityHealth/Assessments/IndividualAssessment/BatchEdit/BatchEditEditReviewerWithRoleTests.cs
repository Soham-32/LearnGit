using System;
using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.IndividualAssessment.BatchEdit;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.IndividualAssessment.Edit;
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
    public class BatchEditEditReviewerWithRoleTests : IndividualAssessmentBaseTest
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

                _team = GetTeamForBatchEdit(setup, "BatchEdit");

                _reviewer = MemberFactory.GetReviewer();
                var reviewerResponse = setup.CreateReviewer(_reviewer).GetAwaiter().GetResult();

                var individualDataResponse = GetIndividualAssessment(setup, _team, "BatchEdit_");
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
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        public void BatchEdit_EditReviewerWithRole()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var dashboardPage = new TeamDashboardPage(Driver, Log);
            var batchEditAssessmentPage = new BatchEditAssessmentPage(Driver, Log);
            var batchEditParticipantReviewerPage = new BatchEditParticipantReviewerPage(Driver, Log);
            var iaEditPage = new IaEdit(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            var teamId = dashboardPage.GetTeamIdFromLink(_team.Name);
            batchEditAssessmentPage.NavigateToPage(Company.Id, _team.Uid, _assessment.BatchId, teamId);
            batchEditAssessmentPage.WaitForAssessmentDataLoaded();
            batchEditAssessmentPage.ClickParticipantsReviewersTab();
            batchEditParticipantReviewerPage.ExpandCollapseParticipantsAndReviewersWithReviewerEmail(_reviewer.Email);

            batchEditParticipantReviewerPage.EditReviewer(_reviewer.Email);

            var email = Guid.NewGuid() + "@example.com";
            var firstName = Guid.NewGuid().ToString();
            const string lastName = "Name";
            var roles = new List<RoleResponse>
            {
                new RoleResponse
                {
                    Tags = new List<TagRoleResponse>
                    {
                        new TagRoleResponse
                        {
                            Id = 1,
                            Name = "Customer"
                        },
                        new TagRoleResponse
                        {
                            Id = 1,
                            Name = "Reviewer"
                        }
                    }
                }
            };

            batchEditParticipantReviewerPage.UpdateMember(email, firstName, lastName, roles[0].Tags);

            Log.Info("Assert: Verify that edited reviewer with roles displays in reviewers list");
            Assert.IsTrue(batchEditParticipantReviewerPage.DoesReviewerDisplayReviewerScreen($"{firstName} {lastName}", email, roles[0].Tags),
                $"Edited reviewer {email} with roles doesn't display in reviewers list");

            var assessmentUid = _assessment.AssessmentList.FirstOrDefault().CheckForNull("assessmentUid is null.").AssessmentUid;
            batchEditParticipantReviewerPage.ClickSaveButton();
            iaEditPage.NavigateToPage(Company.Id, _team.Uid, assessmentUid);

            var role = iaEditPage.GetReviewerRole();
            Log.Info("Verify reviewer role shows correctly");
            Assert.AreEqual("Customer, Reviewer", role, "Expected and actual role shown on the individual assessment edit page do not match");
        }
    }
}
