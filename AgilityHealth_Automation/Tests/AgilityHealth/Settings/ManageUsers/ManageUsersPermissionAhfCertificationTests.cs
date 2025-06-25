using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.DataObjects;
using AgilityHealth_Automation.ObjectFactories;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Common;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.ManageUsers;
using AgilityHealth_Automation.Utilities;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace AgilityHealth_Automation.Tests.AgilityHealth.Settings.ManageUsers
{
    [TestClass]
    [TestCategory("ManageUser"), TestCategory("Settings")]
    [TestCategory("CompanyAdmin")]
    public class ManageUsersPermissionAhfCertificationTests : BaseTest
    {
        private const string SectionName = "Users";
        private const string FeatureName = "Can Add AgilityInsights Facilitator Certification";

        [TestMethod]
        public void ManageUsers_Permission_Ahf_Certification_Via_TeamAdmin()
        {
            Permission_Ahf_Certification(UserType.TeamAdmin);
        }
        [TestMethod]
        public void ManageUsers_Permission_Ahf_Certification_Via_OrgLeader()
        {
            Permission_Ahf_Certification(UserType.OrganizationalLeader);
        }
        [TestMethod]
        public void ManageUsers_Permission_Ahf_Certification_Via_BlAdmin()
        {
            Permission_Ahf_Certification(UserType.BusinessLineAdmin);
        }

        private void Permission_Ahf_Certification(UserType user)
        {
            var login = new LoginPage(Driver, Log);
            var manageUserPage = new ManageUserPage(Driver, Log, user);
            var loginPage = new LoginPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);
            var userInfo = new AhUser();

            Log.Info($"Login as '{User.Username}' and navigate to 'Manage Users' tab");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            manageUserPage.NavigateToPage(Company.Id);
            manageUserPage.SelectTab();

            Log.Info($"Turn on '{FeatureName}' feature.");
            manageUserPage.EditPermission(SectionName, FeatureName);

            Log.Info($"Create '{user}' and logout as a 'CompanyAdmin'.");
            manageUserPage.ClickOnAddNewUserButton();

            switch (user)
            {
                case UserType.TeamAdmin:
                    {
                        userInfo = UserFactory.GetUserInfo(SharedConstants.Team);

                        manageUserPage.EnterUserInfo(userInfo, user);
                        break;
                    }
                case UserType.OrganizationalLeader:
                    {
                        userInfo = UserFactory.GetUserInfo(SharedConstants.MultiTeam);

                        manageUserPage.EnterUserInfo(userInfo, user);
                        break;
                    }
                case UserType.BusinessLineAdmin:
                    {
                        userInfo = UserFactory.GetUserInfo(SharedConstants.TeamTag);

                        manageUserPage.EnterUserInfo(userInfo, user);
                        break;
                    }

            }

            manageUserPage.ClickSaveAndCloseButton();
            topNav.LogOut();

            Log.Info($"Login as '{user}' and navigate to 'Manage Users' tab.");
            Driver.NavigateToPage(GmailUtil.GetUserActivationLink("Create Your AgilityInsights Account", userInfo.Email, "Inbox"));
            loginPage.SetUserPassword(SharedConstants.CommonPassword);
            manageUserPage.NavigateToPage(Company.Id);
            manageUserPage.SelectTab();

            Log.Info("Click on 'Add New' button & Verify that 'Ah Trainer', 'Active AHF' & 'Certifications' fields should be displayed");
            manageUserPage.ClickOnAddNewUserButton();
            Assert.IsTrue(manageUserPage.IsAhTrainerDisplayed(), "'AH Trainer' field is not displayed");
            Assert.IsTrue(manageUserPage.IsActiveAhfDisplayed(), "'Active AHF' field is not displayed");
            Assert.IsTrue(manageUserPage.IsCertificationDisplayed(), "'Certification' field is not displayed");
            manageUserPage.ClickCancelButton();

            Log.Info("Click on 'Edit' icon & Verify 'Ah Trainer', 'Active AHF' & 'Certifications' fields should be displayed.");
            manageUserPage.ClickOnEditUserIcon(userInfo.Email);
            Assert.IsTrue(manageUserPage.IsAhTrainerDisplayed(), "'AH Trainer' field is not displayed");
            Assert.IsTrue(manageUserPage.IsActiveAhfDisplayed(), "'Active AHF' field is not displayed");
            Assert.IsTrue(manageUserPage.IsCertificationDisplayed(), "'Certification' field is not displayed");
            manageUserPage.ClickCancelButton();

            Log.Info($"Logout as '{user}' & Login as 'CompanyAdmin' then navigate to 'Manage Users' tab.");
            topNav.LogOut();
            login.LoginToApplication(User.Username, User.Password);
            manageUserPage.NavigateToPage(Company.Id);

            manageUserPage.SelectTab();

            Log.Info($"Turn off '{FeatureName}' feature");
            manageUserPage.EditPermission(SectionName, FeatureName, false);

            Log.Info($"Logout as 'CompanyAdmin' & Login as '{user}' then navigate to 'Manage Users' tab.");
            topNav.LogOut();
            login.LoginToApplication(userInfo.Email, SharedConstants.CommonPassword);
            manageUserPage.NavigateToPage(Company.Id);
            manageUserPage.SelectTab();

            Log.Info("Click on 'Add New' button & Verify that 'Ah Trainer', 'Active AHF' & 'Certifications' fields should not be displayed.");
            manageUserPage.ClickOnAddNewUserButton();
            Assert.IsFalse(manageUserPage.IsAhTrainerDisplayed(), "'AH Trainer' field is displayed");
            Assert.IsFalse(manageUserPage.IsActiveAhfDisplayed(), "'Active AHF' field is displayed");
            Assert.IsFalse(manageUserPage.IsCertificationDisplayed(), "'Certification' field is displayed");
            manageUserPage.ClickCancelButton();

            Log.Info("Click on 'Edit' icon & Verify 'Ah Trainer', 'Active AHF' & 'Certifications' fields should not be displayed.");
            manageUserPage.ClickOnEditUserIcon(userInfo.Email);
            Assert.IsFalse(manageUserPage.IsAhTrainerDisplayed(), "'AH Trainer' field is displayed");
            Assert.IsFalse(manageUserPage.IsActiveAhfDisplayed(), "'Active AHF' field is displayed");
            Assert.IsFalse(manageUserPage.IsCertificationDisplayed(), "'Certification' field is displayed");
        }
    }
}
