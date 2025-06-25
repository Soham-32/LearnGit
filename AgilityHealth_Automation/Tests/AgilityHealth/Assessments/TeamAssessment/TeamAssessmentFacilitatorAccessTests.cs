using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Edit;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Common;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Assessment.AssessmentList;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.SetUpTearDown;
using AtCommon.Api;
using AtCommon.Dtos;
using AtCommon.Dtos.Assessments.Team.Custom;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Assessments.TeamAssessment
{

    [TestClass]
    [TestCategory("TeamAssessment"), TestCategory("Assessments")]
    public class TeamAssessmentFacilitatorAccessTests : BaseTest
    {
        private static bool _classInitFailed;
        private static TeamResponse _teamResponse;
        private static int _teamId;
        private static AddTeamWithMemberRequest _team;
        private static TeamAssessmentInfo _teamAssessmentWithFacilitator;
        private static TeamAssessmentInfo _teamAssessmentWithoutFacilitator;
        private static User FacilitatorUser => TestEnvironment.UserConfig.GetUserByDescription("business line admin");

        [ClassInitialize]
        public static void ClassSetup(TestContext testContext)
        {
            try
            {
                var setup = new SetUpMethods(testContext, TestEnvironment);
                var setupApiMethods = new SetupTeardownApi(TestEnvironment);

                _team = TeamFactory.GetNormalTeam("Team");
                _teamResponse = setupApiMethods.CreateTeam(_team).GetAwaiter().GetResult();
                _teamId = new SetupTeardownApi(TestEnvironment).GetCompanyHierarchy(Company.Id).GetTeamByName(_teamResponse.Name).TeamId;

                _teamAssessmentWithFacilitator = new TeamAssessmentInfo
                {
                    AssessmentType = SharedConstants.TeamAssessmentType,
                    AssessmentName = RandomDataUtil.GetAssessmentName(),
                    Facilitator = FacilitatorUser.FirstName + " " + FacilitatorUser.LastName,
                    FacilitationDate = DateTime.Today.AddDays(1),
                    StartDate = DateTime.Today
                };
                _teamAssessmentWithoutFacilitator = new TeamAssessmentInfo
                {
                    AssessmentType = SharedConstants.TeamAssessmentType,
                    AssessmentName = RandomDataUtil.GetAssessmentName(),
                    StartDate = DateTime.Today
                };

                setup.AddTeamAssessment(_teamId, _teamAssessmentWithFacilitator);
                setup.AddTeamAssessment(_teamId, _teamAssessmentWithoutFacilitator);
            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }


        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("KnownDefect")] // Bug : 40148 
        [TestCategory("CompanyAdmin")]
        public void TeamAssessment_Facilitator_Access()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var teamAssessmentEdit = new TeamAssessmentEditPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);
            var teamDashboard = new TeamDashboardPage(Driver, Log);
            var assessmentDashboardListTabPage = new AssessmentDashboardListTabPage(Driver, Log);

            login.NavigateToPage();

            login.LoginToApplication(FacilitatorUser.Username, FacilitatorUser.Password);

            Log.Info($"Verify that {_team.Name} team is displayed on Team Dashboard");
            Assert.IsTrue(teamDashboard.DoesTeamDisplay(_team.Name), $"{_team.Name} is not present on team Dashboard");

            Log.Info($"Goto assessment dashboard and verify that {_teamAssessmentWithFacilitator.AssessmentName} assessment is displayed on assessment dashboard");
            teamDashboard.ClickAssessmentDashBoard();
            assessmentDashboardListTabPage.FilterBySearchTerm(_teamAssessmentWithFacilitator.AssessmentName);
            Assert.IsTrue(assessmentDashboardListTabPage.IsAssessmentDisplayed(_teamAssessmentWithFacilitator.AssessmentName), $"Assessment - {_teamAssessmentWithFacilitator.AssessmentName} is not present");

            Log.Info($"Verify that {_teamAssessmentWithoutFacilitator.AssessmentName} assessment is not displayed on assessment dashboard");
            assessmentDashboardListTabPage.FilterBySearchTerm(_teamAssessmentWithoutFacilitator.AssessmentName);
            Assert.IsFalse(assessmentDashboardListTabPage.IsAssessmentDisplayed(_teamAssessmentWithoutFacilitator.AssessmentName), $"Assessment - {_teamAssessmentWithoutFacilitator.AssessmentName} is present");

            Log.Info($"Goto team assessment page and verify that {_teamAssessmentWithFacilitator.AssessmentName} assessment exists inside team & {_teamAssessmentWithoutFacilitator.AssessmentName} assessment does not exist inside team");
            teamAssessmentDashboard.NavigateToPage(_teamId);
            Assert.IsTrue(teamAssessmentDashboard.DoesAssessmentExist(_teamAssessmentWithFacilitator.AssessmentName), $"Assessment - {_teamAssessmentWithFacilitator.AssessmentName} is not present in the team Dashboard");
            Assert.IsFalse(teamAssessmentDashboard.DoesAssessmentExist(_teamAssessmentWithoutFacilitator.AssessmentName), $"Assessment - {_teamAssessmentWithoutFacilitator.AssessmentName} is not present in the team Dashboard");

            Log.Info($"Logout as Facilitator - {FacilitatorUser.FullName} and login as Company Admin -{User.FullName}");
            topNav.LogOut();
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Goto edit assessment page and deselect facilitator");
            teamAssessmentDashboard.NavigateToPage(_teamId);
            teamAssessmentDashboard.SelectRadarLink(_teamAssessmentWithFacilitator.AssessmentName, "Edit");
            teamAssessmentEdit.ClickEditDetailButton();
            teamAssessmentEdit.DeselectFacilitator(_teamAssessmentWithFacilitator.Facilitator);
            teamAssessmentEdit.EditPopup_ClickUpdateButton();

            Log.Info($"Logout as a  Company Admin - {User.FullName} and Login as a Facilitator - {FacilitatorUser.FullName}");
            topNav.LogOut();
            login.LoginToApplication(FacilitatorUser.Username, FacilitatorUser.Password);

            Log.Info($"Verify that {_team.Name} team is not displayed on team dashboard");
            Assert.IsFalse(teamDashboard.DoesTeamDisplay(_team.Name), $"{_team.Name} is present on Team Dashboard");

            Log.Info($"Goto assessment dashboard and verify that {_teamAssessmentWithFacilitator.AssessmentName} assessment is not displayed on assessment dashboard");
            teamDashboard.ClickAssessmentDashBoard();
            assessmentDashboardListTabPage.FilterBySearchTerm(_teamAssessmentWithFacilitator.AssessmentName);
            Assert.IsFalse(assessmentDashboardListTabPage.IsAssessmentDisplayed(_teamAssessmentWithFacilitator.AssessmentName), $"Assessment - {_teamAssessmentWithFacilitator.AssessmentName} is present");

            Log.Info($"Verify that {_teamAssessmentWithoutFacilitator.AssessmentName} assessment is not displayed on assessment dashboard");
            assessmentDashboardListTabPage.FilterBySearchTerm(_teamAssessmentWithoutFacilitator.AssessmentName);
            Assert.IsFalse(assessmentDashboardListTabPage.IsAssessmentDisplayed(_teamAssessmentWithoutFacilitator.AssessmentName), $"Assessment - {_teamAssessmentWithoutFacilitator.AssessmentName} is present");
        }
    }
}
