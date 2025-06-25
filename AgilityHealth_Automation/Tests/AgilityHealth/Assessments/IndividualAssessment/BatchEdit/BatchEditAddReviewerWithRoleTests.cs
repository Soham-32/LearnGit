using System;
using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.IndividualAssessment.BatchEdit;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.IndividualAssessment.Common;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.IndividualAssessment.Create;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AtCommon.Api;
using AtCommon.Dtos.Assessments;
using AtCommon.Dtos.IndividualAssessments;
using AtCommon.Dtos.Reports;
using AtCommon.Dtos.Teams;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Assessments.IndividualAssessment.BatchEdit
{
    [TestClass]
    [TestCategory("TalentDevelopment"), TestCategory("Assessments")]
    public class BatchEditAddReviewerWithRoleTests : IndividualAssessmentBaseTest
    {
        private static bool _classInitFailed;
        private static TeamResponse _team;
        private static IndividualAssessmentResponse _assessment;
        private static CreateIndividualAssessmentRequest _assessmentResponse;

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            try
            {
                var setup = new SetupTeardownApi(TestEnvironment);

                _team = GetTeamForBatchEdit(setup, "BatchEdit");
                var individualDataResponse = GetIndividualAssessment(setup, _team, "BatchEdit_");
                _assessmentResponse = individualDataResponse.Item2;
                _assessmentResponse.Members = _team.Members.Select(m => m.ToAddIndividualMemberRequest()).ToList();
                _assessment = setup.CreateIndividualAssessment(_assessmentResponse, SharedConstants.IndividualAssessmentType)
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
        public void BatchEdit_Add_ReviewerWithRole()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var batchEditAssessmentPage = new BatchEditAssessmentPage(Driver, Log);
            var batchEditParticipantReviewerPage = new BatchEditParticipantReviewerPage(Driver, Log);
            var createIndividualAssessment2 = new CreateIndividualAssessment2AddReviewersPage(Driver, Log);
            var dashboardPage = new TeamDashboardPage(Driver, Log);
            var addReviewerModal = new AddReviewerModal(Driver, Log);


            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            var teamId = dashboardPage.GetTeamIdFromLink(_team.Name);

            batchEditAssessmentPage.NavigateToPage(Company.Id, _team.Uid, _assessment.BatchId, teamId);
            batchEditAssessmentPage.WaitForAssessmentDataLoaded();
            batchEditAssessmentPage.ClickParticipantsReviewersTab();
            batchEditParticipantReviewerPage.ExpandCollapseParticipantsAndReviewers();
            batchEditParticipantReviewerPage.ClickAddReviewer(_assessmentResponse.Members[0].Email);

            var newReviewer = new CreateReviewerRequest()
            {
                FirstName = $"NewReviewer{Guid.NewGuid()}",
                LastName = SharedConstants.TeamMemberLastName,
                Email = $"ah_automation+R{Guid.NewGuid():N}@test.com",
                RoleTags = new List<RoleResponse>
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
                }
            };

            addReviewerModal.CreateNewReviewer(newReviewer);
            addReviewerModal.AddReviewersByScrollingInModal(newReviewer);

            Log.Info("Assert: Verify that added reviewer with roles displays in reviewers list");
            Assert.IsTrue(createIndividualAssessment2.DoesReviewerDisplayReviewerScreen($"{newReviewer.FirstName} {newReviewer.LastName}", newReviewer.Email, newReviewer.RoleTags[0].Tags),
                $"Added reviewer {newReviewer.Email} with roles doesn't display in reviewers list");
        }
    }
}