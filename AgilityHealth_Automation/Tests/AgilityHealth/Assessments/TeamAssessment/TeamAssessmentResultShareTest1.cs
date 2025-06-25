using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Edit;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Common;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.ManageFeatures;
using AgilityHealth_Automation.SetUpTearDown;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos;
using AtCommon.Dtos.Assessments.Team.Custom;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Assessments.TeamAssessment
{
    [TestClass]
    [TestCategory("TeamAssessment"), TestCategory("Assessments")]
    public class TeamAssessmentResultShareTest1 : BaseTest
    {
        private static AddTeamWithMemberRequest _firstTeam;
        private static AddTeamWithMemberRequest _secondTeam;
        private static TeamResponse _createTeamResponseForFirstTeam;
        private static TeamResponse _createTeamResponseForSecondTeam;
        private static TeamHierarchyResponse _firstTeamId;
        private static TeamHierarchyResponse _secondTeamId;
        private static TeamAssessmentInfo _firstTeamAssessment;
        private static TeamAssessmentInfo _secondTeamAssessment;
        private static User MemberUser => TestEnvironment.UserConfig.GetUserByDescription("member");

        [ClassInitialize]
        public static void ClassSetup(TestContext testContext)
        {
            var setup = new SetupTeardownApi(TestEnvironment);
            var setupUi = new SetUpMethods(testContext, TestEnvironment);

            // Create a first team
            _firstTeam = TeamFactory.GetNormalTeam("Team");
            _firstTeam.Members.Add(new AddMemberRequest
            {
                FirstName = MemberUser.FirstName,
                LastName = MemberUser.LastName,
                Email = MemberUser.Username
            });
            _createTeamResponseForFirstTeam = setup.CreateTeam(_firstTeam).GetAwaiter().GetResult();
            _firstTeamId = new SetupTeardownApi(TestEnvironment).GetCompanyHierarchy(Company.Id).GetTeamByName(_createTeamResponseForFirstTeam.Name);

            setupUi.TeamMemberAccessAtTeamLevel(_firstTeamId.TeamId, (_createTeamResponseForFirstTeam.Members.First().Email));

            // Create a team assessment fot first team
            _firstTeamAssessment = new TeamAssessmentInfo
            {
                AssessmentType = SharedConstants.TeamAssessmentType,
                AssessmentName = $"TeamComments{Guid.NewGuid()}",
                TeamMembers = _firstTeam.Members.Select(a => a.FullName()).ToList()
            };

            setupUi.AddTeamAssessment(_firstTeamId.TeamId, _firstTeamAssessment);


            // Create a second team
            _secondTeam = TeamFactory.GetNormalTeam("Team");
            _secondTeam.Members.Add(new AddMemberRequest
            {
                FirstName = MemberUser.FirstName,
                LastName = MemberUser.LastName,
                Email = MemberUser.Username
            });
            _createTeamResponseForSecondTeam = setup.CreateTeam(_secondTeam).GetAwaiter().GetResult();
            _secondTeamId = new SetupTeardownApi(TestEnvironment).GetCompanyHierarchy(Company.Id).GetTeamByName(_createTeamResponseForSecondTeam.Name);

            setupUi.TeamMemberAccessAtTeamLevel(_secondTeamId.TeamId, (_createTeamResponseForSecondTeam.Members.First().Email));

            // Create a team assessment for second team
            _secondTeamAssessment = new TeamAssessmentInfo
            {
                AssessmentType = SharedConstants.TeamAssessmentType,
                AssessmentName = $"TeamComments{Guid.NewGuid()}",
                TeamMembers = _secondTeam.Members.Select(a => a.FullName()).ToList()
            };

            setupUi.AddTeamAssessment(_secondTeamId.TeamId, _secondTeamAssessment);

        }

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("KnownDefect")] // Bug id : 51053
        [TestCategory("CompanyAdmin")]
        public void TA_Result_Share_On_Off()
        {
            var login = new LoginPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var taEditPage = new TeamAssessmentEditPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            manageFeaturesPage.NavigateToPage(Company.Id);
            manageFeaturesPage.TurnOnEnableShareAssessmentResult();
            manageFeaturesPage.ClickUpdateButton();

            teamAssessmentDashboard.NavigateToPage(_firstTeamId.TeamId);
            teamAssessmentDashboard.SelectRadarLink(_firstTeamAssessment.AssessmentName, "Edit");
            taEditPage.StartSharingAssessmentResult();

            topNav.LogOut();
            login.LoginToApplication(MemberUser.Username, MemberUser.Password);

            teamAssessmentDashboard.NavigateToPage(_firstTeamId.TeamId);
            Assert.IsFalse(teamAssessmentDashboard.IsAssessmentRadarGrayedOut(_firstTeamAssessment.AssessmentName), $"'{_firstTeamAssessment.AssessmentName}' assessment radar is grayed out");

            teamAssessmentDashboard.ClickOnRadar(_firstTeamAssessment.AssessmentName);
            Assert.IsTrue(Driver.GetCurrentUrl().EndsWith("/radar"), $"User isn't navigated to '{_firstTeamAssessment.AssessmentName}' radar page - {Driver.GetCurrentUrl()}");

            topNav.LogOut();
            login.LoginToApplication(User.Username, User.Password);

            teamAssessmentDashboard.NavigateToPage(_firstTeamId.TeamId);
            teamAssessmentDashboard.SelectRadarLink(_firstTeamAssessment.AssessmentName, "Edit");

            taEditPage.StopSharingAssessmentResult();

            topNav.LogOut();
            login.LoginToApplication(MemberUser.Username, MemberUser.Password);

            teamAssessmentDashboard.NavigateToPage(_firstTeamId.TeamId);
            Assert.IsTrue(teamAssessmentDashboard.IsAssessmentRadarGrayedOut(_firstTeamAssessment.AssessmentName), $"'{_firstTeamAssessment.AssessmentName}' assessment radar is not grayed out");

            teamAssessmentDashboard.ClickOnRadar(_firstTeamAssessment.AssessmentName);
            Assert.IsFalse(Driver.GetCurrentUrl().EndsWith("/radar"), $"User is navigated to '{_firstTeamAssessment.AssessmentName}' radar page - {Driver.GetCurrentUrl()}");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void TA_Result_Share_MemberHavingTwoTeamAccess()
        {
            var login = new LoginPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var taEditPage = new TeamAssessmentEditPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            manageFeaturesPage.NavigateToPage(Company.Id);
            manageFeaturesPage.TurnOnEnableShareAssessmentResult();
            manageFeaturesPage.ClickUpdateButton();

            teamAssessmentDashboard.NavigateToPage(_secondTeamId.TeamId);
            teamAssessmentDashboard.SelectRadarLink(_secondTeamAssessment.AssessmentName, "Edit");
            taEditPage.StartSharingAssessmentResult();

            topNav.LogOut();
            login.LoginToApplication(MemberUser.Username, MemberUser.Password);

            teamAssessmentDashboard.NavigateToPage(_secondTeamId.TeamId);
            Assert.IsFalse(teamAssessmentDashboard.IsAssessmentRadarGrayedOut(_secondTeamAssessment.AssessmentName), $"{_secondTeamAssessment.AssessmentName}'s Radar logo is grayed out");

            teamAssessmentDashboard.NavigateToPage(_firstTeamId.TeamId);
            Assert.IsTrue(teamAssessmentDashboard.IsAssessmentRadarGrayedOut(_firstTeamAssessment.AssessmentName), $"{_firstTeamAssessment.AssessmentName}'s Radar logo is not grayed out");

        }
    }
}