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
    [TestCategory("CompanyAdmin")]
    public class ManageUsersPermissionResetPasswordTests : BaseTest
    {
        private static User TaUser => TestEnvironment.UserConfig.GetUserByDescription("team admin");
        private static readonly User ResetTaUser = TestEnvironment.UserConfig.GetUserByDescription("team admin password reset");
        private static User OlUser => TestEnvironment.UserConfig.GetUserByDescription("org leader");
        private static readonly User ResetOlTaUser = TestEnvironment.UserConfig.GetUserByDescription("org leader password reset");
        private static User BlUser => TestEnvironment.UserConfig.GetUserByDescription("business line admin");
        private static readonly User ResetBlTaUser = TestEnvironment.UserConfig.GetUserByDescription("business line admin password reset");

        private const string SectionName = "Users";
        private const string FeatureName = "Can Reset Password";

        [TestMethod]
        public void ManageUsers_Permission_ResetPassword_Via_TeamAdmin()
        {
            var login = new LoginPage(Driver, Log);
            var manageUserPage = new ManageUserPage(Driver, Log, UserType.TeamAdmin);
            var passwordReset = new PasswordResetPage(Driver, Log);
            var teamsDashboard = new TeamDashboardPage(Driver, Log);
            var userSearch = new UserSearchPopup(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            manageUserPage.NavigateToPage(Company.Id);

            Log.Info("Edit Permission for 'Team Admins' users");
            manageUserPage.SelectTab();
            manageUserPage.EditPermission(SectionName, FeatureName, false);
            topNav.LogOut();

            Log.Info($"Login with existing TA user: {TaUser.Username} and verify that 'Reset Password' button not displayed for user: {ResetTaUser.Username}");
            login.LoginToApplication(TaUser.Username, TaUser.Password);
            manageUserPage.NavigateToPage(Company.Id);
            manageUserPage.Search(ResetTaUser.Username);
            Assert.IsFalse(userSearch.IsResetPasswordButtonPresent(ResetTaUser.Username), $"'Reset Password' button present for user {ResetTaUser.Username}");

            topNav.LogOut();

            Log.Info($"Login with Company Admin : {User.Username} and Set the permission for reset password");
            login.LoginToApplication(User.Username, User.Password);
            manageUserPage.NavigateToPage(Company.Id);
            manageUserPage.SelectTab();
            manageUserPage.EditPermission(SectionName, FeatureName);
            topNav.LogOut();

            Log.Info($"Login with existing TA user: {TaUser.Username} and verify that 'Reset Password' button displayed for user: {ResetTaUser.Username}");
            login.LoginToApplication(TaUser.Username, TaUser.Password);
            manageUserPage.NavigateToPage(Company.Id);
            manageUserPage.Search(ResetTaUser.Username);
            Assert.IsTrue(userSearch.IsResetPasswordButtonPresent(ResetTaUser.Username), $"'Reset Password' button is not present for {ResetTaUser.Username}");

            userSearch.ClickResetPasswordButton(ResetTaUser.Username);
            topNav.LogOut();

            Log.Info($"Set a new password for user: {ResetTaUser.Username}");
            var resetLink = GmailUtil.GetPasswordResetLink(ResetTaUser.Username);
            var newPassword = CSharpHelpers.Random8Number().ToString();
            passwordReset.NavigateToUrl(resetLink);
            passwordReset.SubmitPassword(newPassword);

            Assert.IsTrue(teamsDashboard.DoesTeamsListExist(),
                "Teams Dashboard is not displayed after resetting password.");
        }

        [TestMethod]
        public void ManageUsers_Permission_ResetPassword_Via_OrgLeader()
        {
            var login = new LoginPage(Driver, Log);
            var manageUserPage = new ManageUserPage(Driver, Log, UserType.OrganizationalLeader);
            var passwordReset = new PasswordResetPage(Driver, Log);
            var teamsDashboard = new TeamDashboardPage(Driver, Log);
            var userSearch = new UserSearchPopup(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            manageUserPage.NavigateToPage(Company.Id);

            Log.Info("Edit Permission for 'Organizational Leader' user");
            manageUserPage.SelectTab();
            manageUserPage.EditPermission(SectionName, FeatureName, false);
            topNav.LogOut();

            Log.Info($"Login with existing TA user: {OlUser.Username} and verify that 'Reset Password' button not displayed for user: {ResetOlTaUser.Username}");
            login.LoginToApplication(OlUser.Username, OlUser.Password);
            manageUserPage.NavigateToPage(Company.Id);
            manageUserPage.Search(ResetOlTaUser.Username);
            Assert.IsFalse(userSearch.IsResetPasswordButtonPresent(ResetOlTaUser.Username), $"'Reset Password' button is present for {ResetOlTaUser.Username}");
            topNav.LogOut();

            Log.Info($"Login with Company Admin : {User.Username} and Set the permission for reset password");
            login.LoginToApplication(User.Username, User.Password);
            manageUserPage.NavigateToPage(Company.Id);
            manageUserPage.SelectTab();
            manageUserPage.EditPermission(SectionName, FeatureName);
            topNav.LogOut();

            Log.Info($"Login with existing TA user: {OlUser.Username} and verify that 'Reset Password' button displayed for user: {ResetOlTaUser.Username}");
            login.LoginToApplication(OlUser.Username, OlUser.Password);
            manageUserPage.NavigateToPage(Company.Id);
            manageUserPage.Search(ResetOlTaUser.Username);
            Assert.IsTrue(userSearch.IsResetPasswordButtonPresent(ResetOlTaUser.Username), $"'Reset Password' button is not present for {ResetOlTaUser.Username}");
            userSearch.ClickResetPasswordButton(ResetOlTaUser.Username);
            topNav.LogOut();

            Log.Info($"Set a new password for user: {ResetOlTaUser.Username}");
            var resetLink = GmailUtil.GetPasswordResetLink(ResetOlTaUser.Username);
            var newPassword = CSharpHelpers.Random8Number().ToString();
            passwordReset.NavigateToUrl(resetLink);
            passwordReset.SubmitPassword(newPassword);

            Assert.IsTrue(teamsDashboard.DoesTeamsListExist(),
                "Teams Dashboard is not displayed after resetting password.");
        }

        [TestMethod]
        public void ManageUsers_Permission_ResetPassword_Via_BusinessLineAdmin()
        {

            var login = new LoginPage(Driver, Log);
            var manageUserPage = new ManageUserPage(Driver, Log, UserType.BusinessLineAdmin);
            var passwordReset = new PasswordResetPage(Driver, Log);
            var teamsDashboard = new TeamDashboardPage(Driver, Log);
            var userSearch = new UserSearchPopup(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            manageUserPage.NavigateToPage(Company.Id);

            Log.Info("Edit Permission for 'Business Line Admins' users");
            manageUserPage.SelectTab();
            manageUserPage.EditPermission(SectionName, FeatureName, false);
            topNav.LogOut();

            Log.Info($"Login with existing TA user: {BlUser.Username} and verify that 'Reset Password' button not displayed for user: {ResetBlTaUser.Username}");
            login.LoginToApplication(BlUser.Username, BlUser.Password);
            manageUserPage.NavigateToPage(Company.Id);
            manageUserPage.Search(ResetBlTaUser.Username);

            Assert.IsFalse(userSearch.IsResetPasswordButtonPresent(ResetBlTaUser.Username), $"'Reset Password' button is not present for {ResetBlTaUser.Username}");
            topNav.LogOut();

            Log.Info($"Login with Admin : {User.Username} and Set the permission for reset password");
            login.LoginToApplication(User.Username, User.Password);
            manageUserPage.NavigateToPage(Company.Id);
            manageUserPage.SelectTab();
            manageUserPage.EditPermission(SectionName, FeatureName);
            topNav.LogOut();

            Log.Info($"Login with existing TA user: {BlUser.Username} and verify that 'Reset Password' button displayed for user: {ResetBlTaUser.Username}");
            login.LoginToApplication(BlUser.Username, BlUser.Password);
            manageUserPage.NavigateToPage(Company.Id);
            manageUserPage.Search(ResetBlTaUser.Username);

            Assert.IsTrue(userSearch.IsResetPasswordButtonPresent(ResetBlTaUser.Username), $"'Reset Password' button is not present for {ResetBlTaUser.Username}");
            userSearch.ClickResetPasswordButton(ResetBlTaUser.Username);
            topNav.LogOut();

            Log.Info($"Set a new password for user: {ResetBlTaUser.Username}");
            var resetLink = GmailUtil.GetPasswordResetLink(ResetBlTaUser.Username);
            var newPassword = CSharpHelpers.Random8Number().ToString();
            passwordReset.NavigateToUrl(resetLink);
            passwordReset.SubmitPassword(newPassword);

            Assert.IsTrue(teamsDashboard.DoesTeamsListExist(),
                "Teams Dashboard is not displayed after resetting password.");
        }
    }
}