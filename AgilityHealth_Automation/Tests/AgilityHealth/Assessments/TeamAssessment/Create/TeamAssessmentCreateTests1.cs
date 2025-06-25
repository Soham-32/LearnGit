using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Create;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos.Assessments.Team.Custom;
using AtCommon.Dtos.Companies;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Assessments.TeamAssessment.Create
{
    [TestClass]
    [TestCategory("TeamAssessment"), TestCategory("Assessments")]
    public class TeamAssessmentCreateTests1 : BaseTest
    {

        private static TeamHierarchyResponse _team;

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            _team = new SetupTeardownApi(TestEnvironment).GetCompanyHierarchy(Company.Id)
                .GetTeamByName(SharedConstants.Team);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public void TeamAssessment_Create_SaveAsDraft()
        {
            _team.CheckForNull($"<{nameof(_team)}> is null. Aborting test.");

            Log.Info("Test: Verify that user is able to save assessment as draft");

            var login = new LoginPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var assessmentProfile = new AssessmentProfilePage(Driver, Log);
            var selectTeamMembers = new SelectTeamMembersPage(Driver, Log);
            var selectStakeHolder = new SelectStakeHolderPage(Driver, Log);
            var reviewAndLaunch = new ReviewAndLaunchPage(Driver, Log);

            Log.Info($"Login as {User.Username} and navigate to 'Assessment Dashboard' page.");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            teamAssessmentDashboard.NavigateToPage(_team.TeamId);
            teamAssessmentDashboard.AddAnAssessment("Team");

            var teamAssessmentInfo = new TeamAssessmentInfo
            {
                AssessmentType = SharedConstants.TeamAssessmentType,
                AssessmentName = RandomDataUtil.GetAssessmentName(),
                LeadershipReadOutDate = DateTime.Today.AddDays(1).AddHours(10)
            };
            assessmentProfile.FillDataForAssessmentProfile(teamAssessmentInfo);
            assessmentProfile.ClickOnNextSelectTeamMemberButton();

            selectTeamMembers.SelectAllTeamMembers();
            selectTeamMembers.ClickOnNextSelectStakeholdersButton();

            selectStakeHolder.SelectAllStakeholders();
            selectStakeHolder.ClickOnReviewAndFinishButton();

            reviewAndLaunch.SelectSendToTeamMember();
            reviewAndLaunch.ClickOnSaveAsDraft();

            Log.Info("Verify that Assessment is saved as Draft on Dashboard ");
            var data = teamAssessmentDashboard.GetAssessmentStatus(teamAssessmentInfo.AssessmentName);
            Assert.AreEqual("Draft", data[0], "Assessment status is incorrect");
            Assert.AreEqual("disc dark-grey", data[1], "Assessment indicator is incorrect");

            teamAssessmentDashboard.ClickOnRadar(teamAssessmentInfo.AssessmentName);

            Assert.IsTrue(Driver.GetCurrentUrl().Contains("/review"),
                $"Not navigated to Assessment Review page i.e. Assessment is NOT saved as Draft only. Navigated to {Driver.GetCurrentUrl()}");
            Assert.IsTrue(reviewAndLaunch.IsSendToTeamMemberRadioButtonSelected(), "'SendToTeamMembers' Radio Button Is Not Selected");
            Assert.IsFalse(reviewAndLaunch.IsSendToStakeHolderRadioButtonSelected(), "'SendToStakeholders' Radio Button Is Selected");
            Assert.IsFalse(reviewAndLaunch.IsSendToEveryoneRadioButtonSelected(), "'SendToEveryone' Radio Button Is Selected");
            Assert.IsTrue(reviewAndLaunch.IsSaveAsDraftVisible(), "'Save As Draft' button is NOT present");
            var expectedDateTime = teamAssessmentInfo.LeadershipReadOutDate.ToString("M/d/yyyy h:mm tt");
            var actualDateTime = reviewAndLaunch.GetLeadershipReadoutDateAndTime();
            Assert.AreEqual(expectedDateTime, actualDateTime, "Leadership Readout Date doesn't match");
        }
    }
}