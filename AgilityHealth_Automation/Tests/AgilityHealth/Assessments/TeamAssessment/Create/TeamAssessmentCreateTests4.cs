using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Create;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos.Companies;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Assessments.TeamAssessment.Create
{
    [TestClass]
    [TestCategory("TeamAssessment"), TestCategory("Assessments")]
    public class TeamAssessmentCreateTests4 : BaseTest
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
        public void TeamAssessment_Create_SendToTeamMembers()
        {
            _team.CheckForNull($"<{nameof(_team)}> is null. Aborting test.");

            Log.Info("Test: User is able to send assessment to Team Members");

            var login = new LoginPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var assessmentProfile = new AssessmentProfilePage(Driver, Log);
            var selectTeamMembers = new SelectTeamMembersPage(Driver, Log);
            var selectStakeHolder = new SelectStakeHolderPage(Driver, Log);
            var reviewAndLaunch = new ReviewAndLaunchPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            teamAssessmentDashboard.NavigateToPage(_team.TeamId);

            teamAssessmentDashboard.AddAnAssessment("Team");

            var assessmentName = RandomDataUtil.GetAssessmentName();
            const string surveyType = SharedConstants.TeamAssessmentType;
            assessmentProfile.FillDataForAssessmentProfile(surveyType, assessmentName);
            assessmentProfile.ClickOnNextSelectTeamMemberButton();

            selectTeamMembers.SelectTeamMemberByName(Constants.TeamMemberName1);
            selectTeamMembers.SelectTeamMemberByName(Constants.TeamMemberName2);

            selectTeamMembers.ClickOnNextSelectStakeholdersButton();

            selectStakeHolder.ClickOnReviewAndFinishButton();

            Log.Info("Verify that user is able to see the emails of Team Members as selected");
            var expectedMembers = new Dictionary<string, string>
            {
                { Constants.TeamMemberName1, Constants.TeamMemberEmail1 },
                { Constants.TeamMemberName2, Constants.TeamMemberEmail2 }
            };

            var actualMembers = reviewAndLaunch.GetTeamMembers();

            foreach (var expected in expectedMembers)
            {
                Assert.IsTrue(actualMembers.ContainsKey(expected.Key),
                    $"Could not find team member with name {expected.Key}");
                Assert.AreEqual(expected.Value, actualMembers[expected.Key],
                    $"Email address does not match for <{expected.Key}>");
            }

            reviewAndLaunch.SelectSendToTeamMember();

            reviewAndLaunch.ClickOnPublish();

            Log.Info("Verifying where assessments are received by Team Members");
            var expectedSubject = SharedConstants.TeamAssessmentSubject(_team.Name, assessmentName);
            foreach (var teamMember in expectedMembers)
            {
                Assert.IsTrue(GmailUtil.DoesMemberEmailExist(expectedSubject, teamMember.Value),
                    $"Email NOT received <{expectedSubject}> sent to {teamMember.Value}");
            }
        }
    }
}