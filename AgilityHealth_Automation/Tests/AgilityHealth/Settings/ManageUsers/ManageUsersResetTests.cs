using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Common;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.ManageUsers;
using AtCommon.Dtos;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Settings.ManageUsers
{
    [TestClass]
    [TestCategory("ManageUser"), TestCategory("Settings")]
    public class ManageUsersResetTests : BaseTest
    {
        private static readonly User ResetUser = TestEnvironment.UserConfig.GetUserByDescription("password reset");

        [TestMethod]
        [TestCategory("KnownDefect")] // Bug : 51053
        [TestCategory("SiteAdmin")]
        public void ManageUsers_ResetPassword()
        {

            var login = new LoginPage(Driver, Log);
            var manageUserPage = new ManageUserPage(Driver, Log, UserType.Member);
            var passwordReset = new PasswordResetPage(Driver, Log);
            var teamsDashboard = new TeamDashboardPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            
            manageUserPage.NavigateToPage(Company.Id);
            manageUserPage.SelectTab();

            manageUserPage.FilterGrid(ResetUser.Username);
            manageUserPage.ClickResetPasswordButton(ResetUser.Username);

            topNav.LogOut();

            var resetLink = GmailUtil.GetPasswordResetLink(ResetUser.Username);
            var newPassword = CSharpHelpers.Random8Number().ToString();
            passwordReset.NavigateToUrl(resetLink);
            passwordReset.SubmitPassword(newPassword);

            Assert.IsTrue(teamsDashboard.DoesTeamsListExist(),
                "Teams Dashboard is not displayed after resetting password.");
        }

        [TestMethod]
        [TestCategory("KnownDefect")] // Bug : 51053
        [TestCategory("SiteAdmin")]
        public void ManageUsers_ResetPassword_SearchPopup()
        {
            var login = new LoginPage(Driver, Log);
            var manageUserPage = new ManageUserPage(Driver, Log, UserType.PartnerAdmin);
            var passwordReset = new PasswordResetPage(Driver, Log);
            var teamsDashboard = new TeamDashboardPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);
            var userSearch = new UserSearchPopup(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            
            manageUserPage.NavigateToPage(Company.Id);
            manageUserPage.Search(ResetUser.Username);
            
            userSearch.ClickResetPasswordButton(ResetUser.Username);

            topNav.LogOut();

            var resetLink = GmailUtil.GetPasswordResetLink(ResetUser.Username);
            var newPassword = CSharpHelpers.Random8Number().ToString();
            passwordReset.NavigateToUrl(resetLink);
            passwordReset.SubmitPassword(newPassword);

            Assert.IsTrue(teamsDashboard.DoesTeamsListExist(),
                "Teams Dashboard is not displayed after resetting password.");
        }

    }
}