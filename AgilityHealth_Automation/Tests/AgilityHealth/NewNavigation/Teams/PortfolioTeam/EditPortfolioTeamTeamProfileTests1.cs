using AgilityHealth_Automation.ObjectFactories.NewNavigation.Teams;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.NewNavigation.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.NewNavigation.Teams.Edit;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace AgilityHealth_Automation.Tests.AgilityHealth.NewNavigation.Teams.PortfolioTeam
{
    [TestClass]
    [TestCategory("PortfolioTeam"), TestCategory("NewNavigation")]
    public class EditPortfolioTeamTeamProfileTests1 : NewNavBaseTest
    {
        private static bool _classInitFailed;
        private static AddTeamWithMemberRequest _portfolioTeam;

        [ClassInitialize]
        public static void ClassSetUp(TestContext _)
        {
            try
            {
                _portfolioTeam = TeamFactory.GetEnterpriseTeam("EditedPortfolio");
                _portfolioTeam.Members.Add(MemberFactory.GetTeamMember());

                var setup = new SetupTeardownApi(TestEnvironment);
                setup.CreateTeam(_portfolioTeam).GetAwaiter().GetResult();
            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }

        }

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin")]
        [TestCategory("KnownDefect")] // Bug Id : 45521
        public void Portfolio_EditProfile()
        {
            VerifySetup(_classInitFailed);

            var loginPage = new LoginPage(Driver, Log);
            var teamDashboardPage = new TeamDashboardPage(Driver, Log);
            var teamProfileTabPage = new TeamProfileTabPage(Driver, Log);

            Log.Info("Navigate to the application and login");
            Driver.NavigateToPage(ApplicationUrl);
            loginPage.LoginToApplication(User.Username, User.Password);
            SwitchToNewNav();

            Log.Info("Enter team name and search");
            teamDashboardPage.SearchTeam(_portfolioTeam.Name);

            Log.Info("Click on Edit button");
            teamDashboardPage.ClickOnEditTeamButton(_portfolioTeam.Name);

            var teamInfo = PortfolioTeamsFactory.GetValidUpdatedPortfolioTeamInfo();

            Log.Info("Enter team info");
            teamProfileTabPage.EnterTeamInfo(teamInfo);
            teamInfo.ImagePath = teamProfileTabPage.GetTeamImage();

            Log.Info("Click on Update Team Profile button");
            teamProfileTabPage.ClickOnUpdateTeamProfile();

            Log.Info("Navigate back to team dashboard");
            teamDashboardPage.NavigateToPage(Company.Id);

            Log.Info("Enter team name and search");
            teamDashboardPage.SearchTeam(teamInfo.TeamName);

            Log.Info("Click on Edit button");
            teamDashboardPage.ClickOnEditTeamButton(teamInfo.TeamName);

            Log.Info("Verify that portfolio team gets edited properly");
            var editedTeamInfo = teamProfileTabPage.GetMultiTeamInfo();
            editedTeamInfo.ImagePath = teamProfileTabPage.GetTeamImage();

            Assert.AreEqual(teamInfo.DepartmentAndGroup, editedTeamInfo.DepartmentAndGroup, "Department and Group doesn't match");
            Assert.AreEqual(teamInfo.TeamBioOrBackground, editedTeamInfo.TeamBioOrBackground, "Team BIO or Background doesn't match");
            Assert.AreEqual(teamInfo.TeamName, editedTeamInfo.TeamName, "Team Name doesn't match");
            Assert.AreEqual(teamInfo.WorkType, editedTeamInfo.WorkType, "Work Type doesn't match");
            Assert.AreEqual(teamInfo.ImagePath, editedTeamInfo.ImagePath, "Image Path doesn't match");
        }
    }
}
