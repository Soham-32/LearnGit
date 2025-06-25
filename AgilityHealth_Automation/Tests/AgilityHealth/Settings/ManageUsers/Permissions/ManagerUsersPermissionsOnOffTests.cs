using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.ManageUsers;
using AgilityHealth_Automation.Utilities;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Settings.ManageUsers.Permissions
{
    [TestClass]
    [TestCategory("ManageUser"), TestCategory("Settings"), TestCategory("ManageUsersPermissions")]
    [TestCategory("CompanyAdmin")]
    public class ManagerUsersPermissionsOnOffTests : BaseTest
    {
        [TestMethod]
        public void ManageUser_All_Permissions_OnOff_OrgLeader()
        {
            Verify_All_Permissions_OnOff(UserType.OrganizationalLeader);
        }
        [TestMethod]
        public void ManageUser_All_Permissions_OnOff_BusinessLineAdmin()
        {
            Verify_All_Permissions_OnOff(UserType.BusinessLineAdmin);
        }
        [TestMethod]
        public void ManageUser_All_Permissions_OnOff_TeamAdmin()
        {
            Verify_All_Permissions_OnOff(UserType.TeamAdmin);
        }
        public void Verify_All_Permissions_OnOff(UserType user)
        {
            var loginPage = new LoginPage(Driver, Log);
            var manageUserPage = new ManageUserPage(Driver, Log, user);

            Log.Info($"Navigate to the login page and login as {User.FullName}");
             loginPage.NavigateToPage();
            loginPage.LoginToApplication(User.Username, User.Password);

            Log.Info("Navigate to 'Manage Users' page and select the Tab");
            manageUserPage.NavigateToPage(Company.Id);
            manageUserPage.SelectTab();

            Log.Info("Deselect all permissions and refresh the page then verify that all permissions should be deselected");
            manageUserPage.SelectDeselectAllPermissions(false);
            Driver.RefreshPage();
            manageUserPage.SelectTab();
            manageUserPage.ExpandAllPermissionSections();
            Assert.IsFalse(manageUserPage.AreAllPermissionsSelected(), "All permissions are selected");

            Log.Info("Select all permissions and refresh the page then verify that all permissions should be selected");
            manageUserPage.PermissionPopupClickOnCloseIcon();

            Driver.Back();
            manageUserPage.NavigateToPage(Company.Id);
            manageUserPage.SelectTab();

            manageUserPage.SelectDeselectAllPermissions();
            Driver.RefreshPage();
            manageUserPage.SelectTab();
            manageUserPage.ExpandAllPermissionSections();
            Assert.IsTrue(manageUserPage.AreAllPermissionsSelected(), "All permissions are not selected");
        }
    }
}