using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.DataObjects;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Create;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Survey;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Common.Create;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Common.Members;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using AtCommon.Dtos.Assessments.Team.Custom;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Assessments.TeamAssessment.Create
{
    [TestClass]
    [TestCategory("TeamAssessment"), TestCategory("Assessments")]
    public class TeamAssessmentCreateReviewAndLaunchPageTests1 : BaseTest
    {
        public static bool ClassInitFailed;
        private static int _teamId;
        private static TeamResponse _teamResponse;

        private static readonly TeamAssessmentInfo TeamAssessment = new TeamAssessmentInfo
        {
            AssessmentType = SharedConstants.TeamAssessmentType,
            AssessmentName = RandomDataUtil.GetAssessmentName(),
            LeadershipReadOutDate = DateTime.Today.AddDays(1)

        };
        private static readonly TeamMemberInfo TeamMemberInfo = new TeamMemberInfo
        {
            FirstName = RandomDataUtil.GetFirstName(),
            LastName = SharedConstants.TeamMemberLastName,
            Email = Constants.UserEmailPrefix + "member1" + TeamMemberInfo.FirstName + Constants.UserEmailDomain,
        };
        private static readonly StakeHolderInfo StakeholderInfo = new StakeHolderInfo
        {
            FirstName = RandomDataUtil.GetFirstName(),
            LastName = SharedConstants.TeamMemberLastName,
            Email = Constants.UserEmailPrefix + "stakeholder1" + StakeholderInfo.FirstName + Constants.UserEmailDomain,
            Role = SharedConstants.StakeholderRole
        };


        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            try
            {
                var setup = new SetupTeardownApi(TestEnvironment);
                var team = TeamFactory.GetNormalTeam("Team");
                _teamResponse = setup.CreateTeam(team).GetAwaiter().GetResult();
                _teamId = new SetupTeardownApi(TestEnvironment).GetCompanyHierarchy(Company.Id).GetTeamByName(_teamResponse.Name).TeamId;

            }
            catch (Exception)
            {
                ClassInitFailed = true;
                throw;
            }
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public void TeamAssessment_Create_ReviewAndLaunch_Edit_Profile_TeamMember_Stakeholder_Preview_Delete_Assessment()
        {
            VerifySetup(ClassInitFailed);

            var login = new LoginPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var addTeamMemberPage = new AddTeamMemberPage(Driver, Log);
            var addStakeHolderPage = new AddStakeHolderPage(Driver, Log);
            var teamMemberCommon = new TeamMemberCommon(Driver, Log);
            var stakeholderCommon = new StakeHolderCommon(Driver, Log);
            var assessmentProfile = new AssessmentProfilePage(Driver, Log);
            var selectTeamMembers = new SelectTeamMembersPage(Driver, Log);
            var selectStakeHolder = new SelectStakeHolderPage(Driver, Log);
            var reviewAndLaunch = new ReviewAndLaunchPage(Driver, Log);
            var surveyPage = new SurveyPage(Driver, Log);

            Log.Info($"Login as {User.Username} and navigate to 'Assessment Dashboard' page.");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            teamAssessmentDashboard.NavigateToPage(_teamId);
            teamAssessmentDashboard.AddAnAssessment("Team");
            assessmentProfile.FillDataForAssessmentProfile(TeamAssessment);
            assessmentProfile.ClickOnNextSelectTeamMemberButton();

            Log.Info("Add team member and go to stakeholder page");
            teamMemberCommon.ClickAddNewTeamMemberButton();
            addTeamMemberPage.EnterTeamMemberInfo(TeamMemberInfo);
            addTeamMemberPage.ClickSaveAndCloseButton();
            selectTeamMembers.SelectTeamMemberByName(TeamMemberInfo.FirstName + " " + TeamMemberInfo.LastName);
            selectTeamMembers.ClickOnNextSelectStakeholdersButton();

            Log.Info("Add stakeholder and go to review & finish page");
            stakeholderCommon.ClickAddNewStakeHolderButton();
            addStakeHolderPage.EnterStakeHolderInfo(StakeholderInfo);
            teamMemberCommon.ClickSaveAndCloseButton();
            selectStakeHolder.SelectStakeHolderByName(StakeholderInfo.FirstName + " " + StakeholderInfo.LastName);
            selectStakeHolder.ClickOnReviewAndFinishButton();

            var expectedDateTime = TeamAssessment.LeadershipReadOutDate.ToString("M/d/yyyy hh:mm tt");
            var actualDateTime = reviewAndLaunch.GetLeadershipReadoutDateAndTime();
            Assert.AreEqual(expectedDateTime, actualDateTime, "Leadership Readout Date doesn't match");

            Log.Info("On review & finish page, Click on the edit button of 'Assessment Profile'");
            var actualProfileDetails = reviewAndLaunch.GetAssessmentName();
            reviewAndLaunch.ClickOnAssessmentProfileEditButton();
            Assert.IsTrue(reviewAndLaunch.IsEditAssessmentDetailsPopupTitleDisplayed(), "'Edit Assessment details' popup is not displayed");

            var editedAssessmentProfileDetails = new TeamAssessmentInfo
            {
                AssessmentName = TeamAssessment.AssessmentName + "updated"
            };

            Log.Info("Fill the assessment profile details and click on the 'Cancel' button");
            reviewAndLaunch.FillNameForAssessmentProfile(editedAssessmentProfileDetails);
            reviewAndLaunch.EditAssessmentDetailsPopupClickOnCancelButton();
            Assert.AreNotEqual(editedAssessmentProfileDetails.AssessmentName, actualProfileDetails, "'Assessment Name' is matched");

            Log.Info("Fill the assessment profile details and click on the 'Update' button");
            reviewAndLaunch.ClickOnAssessmentProfileEditButton();
            reviewAndLaunch.FillNameForAssessmentProfile(editedAssessmentProfileDetails);
            reviewAndLaunch.EditAssessmentDetailsPopupClickOnUpdateButton();

            Log.Info("Get the assessment profile details");
            var updatedActualProfileDetails = reviewAndLaunch.GetAssessmentName();
            Assert.AreEqual(editedAssessmentProfileDetails.AssessmentName, updatedActualProfileDetails, "'Assessment Name' is not matched");
           
            var updatedLeadershipDateTime = new TeamAssessmentInfo
            {
                LeadershipReadOutDate = DateTime.Today.AddDays(2)
            };
            Log.Info("Verify Leadership Readout Date");
            reviewAndLaunch.UpdateLeadershipReadoutDateTime(updatedLeadershipDateTime);
            var expectedUpdatedDateTime = updatedLeadershipDateTime.LeadershipReadOutDate.ToString("M/d/yyyy h:mm tt");
            var actualUpdatedDateTime = reviewAndLaunch.GetLeadershipReadoutDateAndTime();
            Assert.AreEqual(expectedUpdatedDateTime,actualUpdatedDateTime, "Leadership Readout Date doesn't match");

            Log.Info("Click on the edit button of 'Team Members'");
            reviewAndLaunch.ClickOnTeamMembersEditButton();
            Assert.AreEqual($"Add Team Members for {_teamResponse.Name}", selectTeamMembers.GetPageHeaderTitle(), "Edit team members title is not matched");

            selectTeamMembers.ClickOnNextSelectStakeholdersButton();
            selectStakeHolder.ClickOnReviewAndFinishButton();

            Log.Info("Click on the edit button of 'Stakeholders'");
            reviewAndLaunch.ClickOnStakeholdersEditButton();
            Assert.AreEqual($"Add Stakeholders for {_teamResponse.Name}", selectStakeHolder.GetPageHeaderTitle(), "Edit stakeholder title is not matched");

            Log.Info("Navigate to 'Preview Assessment' page and verify 'Company Name' and 'Team Name'");
            selectStakeHolder.ClickOnReviewAndFinishButton();
            reviewAndLaunch.ClickOnPreviewAssessmentButton();
            Driver.SwitchToLastWindow();
            Assert.AreEqual(User.CompanyName, surveyPage.GetHeaderCompanyName(), "'Company Name' is not matched");
            Assert.AreEqual(_teamResponse.Name, surveyPage.GetHeaderTeamName(), "'Team Name' is not matched");

            Log.Info("On survey Page, Select the role");
            surveyPage.SelectHeaderRoleTag(new List<string> { "Stakeholder" });
            Assert.IsTrue(surveyPage.IsSurveyFinishStepDisplayed(), "'Finish' step is not displayed");

            Driver.SwitchToFirstWindow();

            Log.Info("On Review & Finish Page, Click on the 'Delete' button");
            reviewAndLaunch.ClickOnDeleteAssessmentButton();
            Assert.IsTrue(reviewAndLaunch.IsDeleteAssessmentPopupTitleDisplayed(), "'Delete Assessment' popup title is not displayed");

            Log.Info("Click on the 'Cancel' button");
            reviewAndLaunch.DeleteAssessmentPopupClickOnCancelButton();
            Assert.AreEqual(editedAssessmentProfileDetails.AssessmentName, updatedActualProfileDetails, "Assessment Name is not matched");

            Log.Info("Click on 'Remove' button of 'Delete Assessment' popup and verify added assessment is deleted");
            reviewAndLaunch.ClickOnDeleteAssessmentButtonAndChooseRemoveOption();
            Assert.IsFalse(teamAssessmentDashboard.DoesAssessmentExist(TeamAssessment.AssessmentName), $"Assessment <{TeamAssessment.AssessmentName}> still shows on the Assessment Dashboard");
        }
    }
}