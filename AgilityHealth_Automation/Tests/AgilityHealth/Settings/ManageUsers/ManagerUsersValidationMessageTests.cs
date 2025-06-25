using System.Collections.Generic;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.ManageUsers;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Settings.ManageUsers
{
    [TestClass]
    [TestCategory("ManageUser"), TestCategory("Settings")]
    public class ManagerUsersValidationMessageTests : BaseTest
    {
        public static bool ClassInitFailed;
        private static readonly List<string> ExpectedList = new List<string>()
            {"FirstName", "LastName", "Email"};

        [TestMethod]
        [TestCategory("SiteAdmin")]
        public void ManageUser_All_Mandatory_Fields_PartnerAdmin()
        {
            Verify_Validation_Message(UserType.PartnerAdmin);
        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void ManageUser_All_Mandatory_Fields_CompanyAdmin()
        {
            Verify_Validation_Message(UserType.CompanyAdmin);
        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void ManageUser_All_Mandatory_Fields_OrganizationalLeaders()
        {
            Verify_Validation_Message(UserType.OrganizationalLeader);
        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void ManageUser_All_Mandatory_Fields_BusinessLineAdmins()
        {
            Verify_Validation_Message(UserType.BusinessLineAdmin);
        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void ManageUser_All_Mandatory_Fields_TeamAdmins()
        {
            Verify_Validation_Message(UserType.TeamAdmin);
        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void ManageUser_All_Mandatory_Fields_Coaches()
        {
            Verify_Validation_Message(UserType.Coaches);
        }

        private void Verify_Validation_Message(UserType userType)
        {
            var loginPage = new LoginPage(Driver, Log);
            var manageUserPage = new ManageUserPage(Driver, Log, userType);

            Log.Info("Take a login as Company admin and navigate to 'Manage User' tab");
            loginPage.NavigateToPage();
            loginPage.LoginToApplication(User.Username, User.Password);
            manageUserPage.NavigateToPage(Company.Id);
            manageUserPage.SelectTab();

            Log.Info("Click on the 'Save And Close' button and Verify that validation message");
            manageUserPage.ClickOnAddNewUserButton();
            manageUserPage.ClickSaveAndCloseButton();
            foreach (var expectedField in ExpectedList)
            {
                Assert.IsTrue(manageUserPage.IsFieldValidationMessageDisplayed(expectedField), $"Validation message is not displayed for Manage Users '{expectedField}' field");
            }
            manageUserPage.ClickCancelButton();

            Log.Info($"Click on the 'Save And Add New' button and Verify that validation message");
            manageUserPage.ClickOnAddNewUserButton();
            manageUserPage.ClickSaveAndAddNewButton();
            foreach (var expectedField in ExpectedList)
            {
                Assert.IsTrue(manageUserPage.IsFieldValidationMessageDisplayed(expectedField), $"Validation message is not displayed for Manage Users '{expectedField}' field");
            }
        }
    }
}