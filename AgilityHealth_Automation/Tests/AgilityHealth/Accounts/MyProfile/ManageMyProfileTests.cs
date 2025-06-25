using System;
using System.IO;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Common;
using AgilityHealth_Automation.Utilities;
using AtCommon.Dtos;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Accounts.MyProfile
{
    [TestClass]
    [TestCategory("Accounts"), TestCategory("MyProfile")]
    public class ManageMyProfileTests : BaseTest
    {
        private static User User2 => TestEnvironment.UserConfig.GetUserByDescription("user 2");

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("OrgLeader"), TestCategory("BLAdmin"), TestCategory("Member")]
        public void ManageMyProfile_UpdateMyProfile()
        {
            var login = new LoginPage(Driver, Log);
            var accountSettingsPage = new AccountSettingsPage(Driver, Log);
            var header = new TopNavigation(Driver, Log);

            Log.Info("Login to the application and Navigate to 'My Profile' page.");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            header.HoverOnNameRoleSection();
            header.ClickOnMyProfile();

            var profileInfo = new ProfileInfo()
            {
                FirstName = RandomDataUtil.GetFirstName(),
                LastName = RandomDataUtil.GetLastName(),
                PreferredLanguage = "Spanish",
                PhoneNumber = "0123456789",
                MyBrand = "Test Brand",
                MySkill = "ASP.Net",
                LinkedIn = "https://test.com",
                ValuePro = "Test Value",
                PhotoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\TestData\\1.jpg")
            };
            
            Log.Info("Verify all the languages in 'Preferred Language' Dropdown");
            var expectedLanguageList = ManageRadarFactory.Languages();
            var actualLanguageList = accountSettingsPage.GetAllLanguages();
            Assert.That.ListsAreEqual(expectedLanguageList, actualLanguageList, "Language list doesn't match");

            Log.Info("Enter Profile information and click on 'Update' button");
            accountSettingsPage.EnterProfileInfo(profileInfo);
            accountSettingsPage.ClickUpdateButton();

            Assert.AreEqual(profileInfo.FirstName, accountSettingsPage.GetFirstName(), "Profile Info > First name doesn't match");
            Assert.AreEqual(profileInfo.PhoneNumber, accountSettingsPage.GetPhoneNumber(), "Profile Info > phone number doesn't match");
            Assert.AreEqual(profileInfo.PreferredLanguage, accountSettingsPage.GetPreferredLanguage(), "Profile Info > Preferred Language doesn't match");
            Assert.AreEqual(profileInfo.MyBrand, accountSettingsPage.GetBrand(), "Profile Info > my brand info doesn't match");
            Assert.AreEqual(profileInfo.LinkedIn, accountSettingsPage.GetLinkedIn(), "Profile Info > LinkedIn info doesn't match");
            Assert.AreEqual(profileInfo.ValuePro, accountSettingsPage.GetValuePro(), "Profile Info > Value Pro info doesn't match");
            
            profileInfo = new ProfileInfo()
            {
                FirstName = User.FirstName,
                LastName = User.LastName,
                PreferredLanguage = "English",
                PhoneNumber = "",
                MyBrand = "",
                MySkill = "",
                LinkedIn = "",
                ValuePro = "",
                PhotoPath = ""
            };
            accountSettingsPage.EnterProfileInfo(profileInfo);
            accountSettingsPage.ClickUpdateButton();

        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public void ManageMyProfile_UpdateMyNotification()
        {
            var login = new LoginPage(Driver, Log);
            var accountSettingsPage = new AccountSettingsPage(Driver, Log);
            var header = new TopNavigation(Driver, Log);

            login.NavigateToPage();

            login.LoginToApplication(User.Username, User.Password);

            header.HoverOnNameRoleSection();

            header.ClickOnMyProfile();

            accountSettingsPage.SelectTab("My Notifications");

            accountSettingsPage.DeSelectAllSelectedTeam();

            accountSettingsPage.SelectAvailableTeam(Constants.GoiTeam);

            accountSettingsPage.ClickUpdateNotificationButton();

            Assert.IsTrue(accountSettingsPage.DoesSelectedTeamDisplay(Constants.GoiTeam), "Selected Team isn't displayed i.e. " + Constants.GoiTeam);

            accountSettingsPage.SelectSelectedTeam(Constants.GoiTeam);

            accountSettingsPage.ClickUpdateNotificationButton();

            Assert.IsFalse(accountSettingsPage.DoesSelectedTeamDisplay(Constants.GoiTeam), "Selected Team is still displayed i.e. " + Constants.GoiTeam);

        }

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("OrgLeader")]
        [TestCategory("BLAdmin"), TestCategory("Member")]
        public void ManageMyProfile_UpdatePassword()
        {
            var login = new LoginPage(Driver, Log);
            var accountSettingsPage = new AccountSettingsPage(Driver, Log);
            var header = new TopNavigation(Driver, Log);

            login.NavigateToPage();

            var newPassword = User2.NewPassword;
            var currentPassword = User2.Password;

            login.LoginToApplication(User2.Username, currentPassword);
            try
            {
                if (login.GetValidationErrorText().Equals(Constants.InvalidEmailAndPasswordValidationMessage))
                {
                    newPassword = User2.Password;
                    currentPassword = User2.NewPassword;
                    login.LoginToApplication(User2.Username, currentPassword);
                }
            }
            catch
            {
                // ignored
            }


            header.NavigateToMyProfilePage(Company.Id);

            accountSettingsPage.ClickPasswordPanel();

            accountSettingsPage.ClickUpdatePassword();

            Assert.AreEqual("Please fill New Password.", accountSettingsPage.GetErrorMessage(), "Error message is incorrect");

            accountSettingsPage.UpdatePassword(User2.Password, newPassword);
            accountSettingsPage.ClickUpdatePassword();

            Assert.AreEqual("Please fill Confirm Password.", accountSettingsPage.GetErrorMessage(), "Error message is incorrect");

            accountSettingsPage.UpdatePassword(currentPassword, newPassword, "123");
            accountSettingsPage.ClickUpdatePassword();

            Assert.AreEqual("The new password and confirmation password do not match.", accountSettingsPage.GetErrorMessage(), "Error message is incorrect");

            accountSettingsPage.UpdatePassword("12345", newPassword, newPassword);
            accountSettingsPage.ClickUpdatePassword();

            Assert.AreEqual("Your old password is incorrect.", accountSettingsPage.GetErrorMessage(), "Error message is incorrect");

            accountSettingsPage.UpdatePassword(currentPassword, newPassword, newPassword);
            accountSettingsPage.ClickUpdatePassword();

            Assert.AreEqual("Your password is successfully changed.", accountSettingsPage.GetErrorMessage(), "Success message is incorrect");

            header.HoverOnNameRoleSection();
            header.ClickOnSignOut();

            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(User2.Username, newPassword);
            header.NavigateToMyProfilePage(Company.Id);
            Assert.IsTrue(header.IsNameAndRoleSectionDisplayed(), "User isn't logged in successfully");
        }
    }
}
