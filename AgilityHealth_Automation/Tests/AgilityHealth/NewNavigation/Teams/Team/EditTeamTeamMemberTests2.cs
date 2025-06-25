using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.NewNavigation.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.NewNavigation.Teams.Edit;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace AgilityHealth_Automation.Tests.AgilityHealth.NewNavigation.Teams.Team
{
    [TestClass]
    [TestCategory("Team"), TestCategory("NewNavigation")]
    public class EditTeamTeamMemberTests2 : NewNavBaseTest
    {
        private static bool _classInitFailed;
        private static AddTeamWithMemberRequest _team;

        [ClassInitialize]
        public static void ClassSetUp(TestContext _)
        {
            try
            {
                _team = TeamFactory.GetNormalTeam("TeamMemberAccess");
                _team.Members.Add(MemberFactory.GetTeamMember());

                var setup = new SetupTeardownApi(TestEnvironment);
                setup.CreateTeam(_team).GetAwaiter().GetResult();
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
        public void Team_TeamMemberAccess()
        {
            VerifySetup(_classInitFailed);

            var loginPage = new LoginPage(Driver, Log);
            var teamDashboardPage = new TeamDashboardPage(Driver, Log);
            var teamMembersTabPage = new TeamMembersTabPage(Driver, Log);

            Log.Info("Navigate to the application and login");
            Driver.NavigateToPage(ApplicationUrl);
            loginPage.LoginToApplication(User.Username, User.Password);
            SwitchToNewNav();

            Log.Info("Switch to grid view");
            teamDashboardPage.SwitchToGridView();

            Log.Info("Enter team name and search");
            teamDashboardPage.SearchTeam(_team.Name);

            Log.Info("Click on Edit button");
            teamDashboardPage.ClickOnEditTeamButton(_team.Name);

            Log.Info("Click on Team Members tab");
            teamMembersTabPage.ClickOnTeamMembersTab();

            Log.Info("Click on team member 'Team Access' button and verify member should get account setup email");
            teamMembersTabPage.ClickOnTeamMemberTeamAccessButton(_team.Members[0].Email);
            Assert.IsTrue(teamMembersTabPage.IsTeamMemberSuccessfullyTeamAccessIconDisplayed(_team.Members[0].Email), "Team member doesn't have access");
            Assert.IsTrue(GmailUtil.DoesMemberEmailExist(SharedConstants.MemberAccountCreateEmailSubject, _team.Members[0].Email, "Auto_TeamAccess", "", 40), "Member is not receiving email");

            Log.Info("Setup member's account, Verify user navigated to 'Team Dashboard' page and team should be displayed");
            Driver.NavigateToPage(GmailUtil.GetUserCreateAccountLink(SharedConstants.MemberAccountCreateEmailSubject, _team.Members[0].Email, "Auto_TeamAccess"));
            loginPage.SetUserPassword(SharedConstants.CommonPassword);

            Log.Info("Switch to new navigation and search the team");
            teamDashboardPage.NavigateToPage(Company.Id);
            teamDashboardPage.SwitchToGridView();
            teamDashboardPage.SearchTeam(_team.Name);
            Assert.IsTrue(teamDashboardPage.IsTeamDisplayed(_team.Name), "Team is not displayed");
        }
    }
}
