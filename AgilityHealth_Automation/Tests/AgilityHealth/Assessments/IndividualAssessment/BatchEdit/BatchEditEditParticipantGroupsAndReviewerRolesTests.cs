using System;
using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.IndividualAssessment.BatchEdit;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Survey;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Team.Edit;
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
    [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
    [TestCategory("TalentDevelopment"), TestCategory("Assessments")]
    public class BatchEditEditParticipantGroupsAndReviewerRolesTests : IndividualAssessmentBaseTest
    {
        private static bool _classInitFailed;
        private static TeamResponse _team;
        private static CreateReviewerRequest _reviewer;
        private static IndividualAssessmentResponse _assessment;
        private static CreateIndividualAssessmentRequest _assessmentRequest;

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            try
            {
                var setup = new SetupTeardownApi(TestEnvironment);
                _team = GetTeamForBatchEdit(setup, "BatchEdit",2);
                _reviewer = MemberFactory.GetReviewer();

                var reviewerResponse = setup.CreateReviewer(_reviewer).GetAwaiter().GetResult();
                var individualDataResponse = GetIndividualAssessment(setup, _team, "BatchEdit_");
                _assessmentRequest = individualDataResponse.Item2;
                 var assessmentResponse = individualDataResponse.Item3;

                _assessmentRequest.Members = _team.Members.Select(m => m.ToAddIndividualMemberRequest()).ToList();
                _assessmentRequest.Members.First().Reviewers.Add(reviewerResponse.ToAddIndividualMemberRequest());
                _assessmentRequest.BatchId = assessmentResponse.BatchId;

                _assessment = setup.CreateIndividualAssessment(_assessmentRequest, SharedConstants.IndividualAssessmentType).GetAwaiter().GetResult();
            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }

        [TestMethod]
        public void BatchEdit_EditMemberParticipantGroup()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var surveyPage = new SurveyPage(Driver, Log);
            var batchEditAssessmentPage = new BatchEditAssessmentPage(Driver, Log);
            var batchEditParticipantReviewerPage = new BatchEditParticipantReviewerPage(Driver, Log);
            var dashboardPage = new TeamDashboardPage(Driver, Log);
            var participantEmail = _assessmentRequest.Members.First().Email;

            var participantGroupList = new List<string>
            {
                "Support",
                "Business"
            };

            Log.Info($"Fill the survey of {participantEmail} ");
            var surveyLink = GmailUtil.GetSurveyLink(SharedConstants.IaEmailParticipantSubject, participantEmail, "unread", _assessmentRequest.PointOfContact);
            surveyPage.NavigateToUrl(surveyLink);
            surveyPage.ConfirmIdentity();
            surveyPage.ClickStartSurveyButton();
            surveyPage.SubmitRandomSurvey();
            surveyPage.ClickNextButton();
            surveyPage.ClickNextButton();
            surveyPage.ClickNextButton();
            surveyPage.ClickFinishButton();

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            var teamId = dashboardPage.GetTeamIdFromLink(_team.Name);
            batchEditAssessmentPage.NavigateToPage(Company.Id, _team.Uid, _assessment.BatchId, teamId);
            batchEditAssessmentPage.WaitForAssessmentDataLoaded();
            batchEditAssessmentPage.ClickParticipantsReviewersTab();

            Log.Info("Edit the 'Participant Group'");
            batchEditParticipantReviewerPage.EditParticipant(participantEmail);
            batchEditParticipantReviewerPage.UpdateParticipantGroup(participantGroupList);

            Log.Info($"Assert: Verify that edited participant group {participantGroupList} displays in participants list");
            var updatedParticipantGroup = batchEditParticipantReviewerPage.GetParticipantGroup(participantEmail).StringToList();
            Assert.That.ListsAreEqual(updatedParticipantGroup, participantGroupList, "Added participant group are not matching");

            Log.Info($"Remove {participantGroupList.First()} from the participant group");
            batchEditParticipantReviewerPage.EditParticipant(participantEmail);
            batchEditParticipantReviewerPage.RemoveParticipantGroup(participantGroupList.First());
            Assert.IsFalse(batchEditParticipantReviewerPage.IsParticipantGroupDisplayed(participantEmail,participantGroupList.First()), $"{participantGroupList.First()} does exist");
            Assert.IsTrue(batchEditParticipantReviewerPage.IsParticipantGroupDisplayed(participantEmail, participantGroupList.Last()), $"{participantGroupList.First()} does exist");
        }

        [TestMethod]
        public void BatchEdit_EditReviewerRoles()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var surveyPage = new SurveyPage(Driver, Log);
            var batchEditAssessmentPage = new BatchEditAssessmentPage(Driver, Log);
            var batchEditParticipantReviewerPage = new BatchEditParticipantReviewerPage(Driver, Log);
            var dashboardPage = new TeamDashboardPage(Driver, Log);

            var roles = new List<string>
            {
                "Reviewer"
            };

            Log.Info($"Fill the survey of {_reviewer.Email}");
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

            var teamId = dashboardPage.GetTeamIdFromLink(_team.Name);
            batchEditAssessmentPage.NavigateToPage(Company.Id, _team.Uid, _assessment.BatchId, teamId);
            batchEditAssessmentPage.WaitForAssessmentDataLoaded();
            batchEditAssessmentPage.ClickParticipantsReviewersTab();
            batchEditParticipantReviewerPage.ExpandCollapseParticipantsAndReviewersWithReviewerEmail(_reviewer.Email);

            Log.Info($"Remove {roles} from the reviewer");
            batchEditParticipantReviewerPage.ClickOnEditReviewerButton(_reviewer.Email);
            batchEditParticipantReviewerPage.RemoveRoleFromReviewer(roles.First());
            Assert.IsFalse(batchEditParticipantReviewerPage.DoesRoleOfReviewerDisplay(roles.First()), $"{roles.First()} does exist");

            Log.Info($"Add {roles} for reviewer");
            batchEditParticipantReviewerPage.ClickOnEditReviewerButton(_reviewer.Email);

            roles.Add("Customer");

            batchEditParticipantReviewerPage.EditRoles(roles);
            var updatedRole = batchEditParticipantReviewerPage.GetReviewerRole(_reviewer.Email).StringToList();
            Assert.That.ListsAreEqual(updatedRole, roles, "Added Roles do not match");
        }

        [TestMethod]
        public void BatchEdit_EditMemberParticipantGroup_VerifyAtTeamLevel()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var batchEditAssessmentPage = new BatchEditAssessmentPage(Driver, Log);
            var batchEditParticipantReviewerPage = new BatchEditParticipantReviewerPage(Driver, Log);
            var dashboardPage = new TeamDashboardPage(Driver, Log);
            var editTeamMemberPage = new EditTeamTeamMemberPage(Driver, Log);
            var participantEmail = _assessmentRequest.Members.Last().Email;

            var participantGroupList = new List<string>
            {
                "Support",
                "Business"
            };

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            var teamId = dashboardPage.GetTeamIdFromLink(_team.Name);
            Log.Info("Go to BatchEdit Assessment and Edit the 'Participant' to add participant group with tags ");
            batchEditAssessmentPage.NavigateToPage(Company.Id, _team.Uid, _assessment.BatchId, teamId);
            batchEditAssessmentPage.WaitForAssessmentDataLoaded();

            batchEditAssessmentPage.ClickParticipantsReviewersTab();
            batchEditParticipantReviewerPage.EditParticipant(participantEmail);
            batchEditParticipantReviewerPage.UpdateParticipantGroup(participantGroupList);

            Log.Info($"Assert: Verify that edited participant group {participantGroupList} displays in participants list");
            var updatedParticipantGroup = batchEditParticipantReviewerPage.GetParticipantGroup(participantEmail).StringToList();
            Assert.That.ListsAreEqual(updatedParticipantGroup, participantGroupList, "Added participant group are not matching");

            Log.Info("Go to Team dashboard, edit the team and team member");
            dashboardPage.NavigateToPage(Company.Id);
            dashboardPage.GridTeamView();

            editTeamMemberPage.NavigateToPage(teamId.ToInt());

            Log.Info($"Assert: Verify that edited participant group {participantGroupList} displays in Team Member's list");
            var addedParticipantGroup = editTeamMemberPage.GetTeamMemberParticipantGroup(participantEmail.ToLower()).StringToList();
            Assert.That.ListsAreEqual(participantGroupList, addedParticipantGroup, "Added participant group are not matching");

            Log.Info("Go to BatchEdit Assessment and Edit the 'Participant' to remove one participant group with tags ");
            batchEditParticipantReviewerPage.NavigateToPage(Company.Id, _team.Uid, _assessment.BatchId, teamId);
            batchEditAssessmentPage.WaitForAssessmentDataLoaded();
            batchEditAssessmentPage.ClickParticipantsReviewersTab();

            batchEditParticipantReviewerPage.EditParticipant(participantEmail);
            batchEditParticipantReviewerPage.RemoveParticipantGroup(participantGroupList.First());

            Log.Info($"Assert: Verify that participant group {participantGroupList.First()} removed from Team Member's list");
            editTeamMemberPage.NavigateToPage(teamId.ToInt());
            Assert.IsFalse(editTeamMemberPage.DoesParticipantGroupExist(participantEmail.ToLower(), participantGroupList.First()), "Participant Group does exist");
            Assert.IsTrue(editTeamMemberPage.DoesParticipantGroupExist(participantEmail.ToLower(),participantGroupList.Last()),"Participant Group does not exist");
        }
    }
}