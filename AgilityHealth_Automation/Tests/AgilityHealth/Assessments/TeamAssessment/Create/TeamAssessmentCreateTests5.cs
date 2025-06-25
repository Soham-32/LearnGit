using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Create;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos;
using AtCommon.Dtos.Assessments.Team.Custom;
using AtCommon.Dtos.Companies;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Assessments.TeamAssessment.Create
{
    [TestClass]
    [TestCategory("TeamAssessment"), TestCategory("Assessments")]
    public class TeamAssessmentCreateTests5 : BaseTest
    {
        private static TeamHierarchyResponse _team;
        private static readonly User Facilitator = TestEnvironment.UserConfig.GetUserByDescription("facilitator");

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            _team = new SetupTeardownApi(TestEnvironment).GetCompanyHierarchy(Company.Id)
                .GetTeamByName(SharedConstants.Team);
        }

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public void TeamAssessment_Create_SendPostRetroSurvey_Enabled()
        {
            _team.CheckForNull($"<{nameof(_team)}> is null. Aborting test.");

            var login = new LoginPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var assessmentProfile = new AssessmentProfilePage(Driver, Log);
            var selectTeamMembers = new SelectTeamMembersPage(Driver, Log);
            var selectStakeHolder = new SelectStakeHolderPage(Driver, Log);
            var reviewAndLaunch = new ReviewAndLaunchPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            teamAssessmentDashboard.NavigateToPage(_team.TeamId);

            teamAssessmentDashboard.AddAnAssessment("team");

            var assessmentInfo = new TeamAssessmentInfo
            {
                AssessmentType = SharedConstants.TeamAssessmentType,
                AssessmentName = "AHFSurveyAssessment_" + RandomDataUtil.GetAssessmentName(),
                Facilitator = Facilitator.FullName
            };

            assessmentProfile.FillDataForAssessmentProfile(assessmentInfo);
            assessmentProfile.ClickOnNextSelectTeamMemberButton();

            selectTeamMembers.SelectTeamMemberByName(Constants.TeamMemberName1);

            selectTeamMembers.ClickOnNextSelectStakeholdersButton();

            selectStakeHolder.ClickOnReviewAndFinishButton();

            Log.Info("Validate that the checkbox is enabled and checked by default.");
            Assert.IsTrue(reviewAndLaunch.IsSendRetroSurveyCheckboxEnabled(),
                "The 'Send Post Retrospective Feedback assessment' checkbox should be enabled.");
        }
    }
}