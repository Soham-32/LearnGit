using System;
using System.Collections.Generic;
using System.IO;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.DataObjects;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Company;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.ManageUsers;
using AgilityHealth_Automation.Utilities;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Settings.ManageUsers
{
    [TestClass]
    [TestCategory("ManageUser"), TestCategory("Settings")]
    public class ManageUserSaveAndAddNewUserTests : BaseTest
    {
        public static bool ClassInitFailed;
        public static List<string> ExpectedLanguageList = ManageRadarFactory.Languages();

        [TestMethod]
        [TestCategory("SiteAdmin")]
        public void ManageUser_SaveAndAddNew_PartnerAdmin()
        {
            var loginPage = new LoginPage(Driver, Log);
            var manageUserPagePartnerAdmin = new ManageUserPage(Driver, Log, UserType.PartnerAdmin);
            var companyDashboardPage = new CompanyDashboardPage(Driver, Log);

            Log.Info("Take a login as site admin and navigate to 'Manage User' tab");
            loginPage.NavigateToPage();
            loginPage.LoginToApplication(User.Username, User.Password);
            companyDashboardPage.WaitUntilLoaded();
            manageUserPagePartnerAdmin.NavigateToPage(Company.Id);

            Log.Info("Fill all the user details and click on 'Save And Add New' button");
            manageUserPagePartnerAdmin.ClickOnAddNewUserButton();
            var partnerAdminInfo = new PartnerAdminInfo
            {
                FirstName = RandomDataUtil.GetFirstName(),
                LastName = SharedConstants.TeamMemberLastName,
                Email = Constants.UserEmailPrefix + "_PA_" + CSharpHelpers.RandomNumber() + Constants.UserEmailDomain,
                PreferredLanguage = "Spanish",
                NotifyUser = true,
                Companies = new List<string>
                {
                    "Automation_BL (DO NOT USE)",
                    "Automation_CA (DO NOT USE)",
                    "Automation_M (DO NOT USE)",
                    "Automation_OL (DO NOT USE)"
                },
                ActiveAhf = true,
                AhTrainer = true,
                ImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\TestData\\1.jpg")
            };

            Log.Info("Verify all languages from 'Preferred Language' dropdown'");
            var actualLanguageList = manageUserPagePartnerAdmin.GetAllLanguages();
            Assert.That.ListsAreEqual(ExpectedLanguageList, actualLanguageList, "Language list doesn't match");

            manageUserPagePartnerAdmin.EnterPartnerAdminInfo(partnerAdminInfo);
            manageUserPagePartnerAdmin.AddCompanies(partnerAdminInfo.Companies);
            partnerAdminInfo.Companies.Add(User.CompanyName);
            manageUserPagePartnerAdmin.ClickSaveAndAddNewButton();

            Log.Info($"Verify that 'Add {UserType.PartnerAdmin}' popup should be opened and data should be saved");
            Assert.IsTrue(manageUserPagePartnerAdmin.IsUserPopupPresent(), $"{UserType.PartnerAdmin} popup is not opened");

            manageUserPagePartnerAdmin.ClickCancelButton();
            Assert.IsTrue(manageUserPagePartnerAdmin.IsUserDisplayed(partnerAdminInfo.Email),
                $"Failure !! Partner Admin with First name: {partnerAdminInfo.FirstName} and Last name: {partnerAdminInfo.LastName} is not deleted");

            Log.Info($"Edit {partnerAdminInfo.Email} user and verify all languages from 'Preferred Language' dropdown'");
            manageUserPagePartnerAdmin.ClickOnEditUserIcon(partnerAdminInfo.Email);
            actualLanguageList = manageUserPagePartnerAdmin.GetAllLanguages();
            Assert.That.ListsAreEqual(ExpectedLanguageList, actualLanguageList, "Language list doesn't match");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void ManageUser_SaveAndAddNew_CompanyAdmin()
        {
            var loginPage = new LoginPage(Driver, Log);
            var manageUserPageCompanyAdmin = new ManageUserPage(Driver, Log, UserType.CompanyAdmin);

            Log.Info("Take a login as Company admin and navigate to 'Manage User' tab");
            loginPage.NavigateToPage();
            loginPage.LoginToApplication(User.Username, User.Password);
            manageUserPageCompanyAdmin.NavigateToPage(Company.Id);

            Log.Info("Fill all the user details and click on 'Save And Add New' button");
            manageUserPageCompanyAdmin.ClickOnAddNewUserButton();
            var companyAdminInfo = new CompanyAdminInfo
            {
                FirstName = RandomDataUtil.GetFirstName(),
                LastName = SharedConstants.TeamMemberLastName,
                Email = Constants.UserEmailPrefix + "_CA_" + CSharpHelpers.RandomNumber() + Constants.UserEmailDomain,
                PreferredLanguage = "Spanish",
                NotifyUser = true,
                FeatureAdmin = false,
                ImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\TestData\\1.jpg"),
                ActiveAhf = true,
                AhTrainer = true
            };

            Log.Info("Verify all languages from 'Preferred Language' dropdown'");
            var actualLanguageList = manageUserPageCompanyAdmin.GetAllLanguages();
            Assert.That.ListsAreEqual(ExpectedLanguageList, actualLanguageList, "Language list doesn't match");

            manageUserPageCompanyAdmin.EnterCompanyAdminInfo(companyAdminInfo);
            manageUserPageCompanyAdmin.ClickSaveAndAddNewButton();

            Log.Info($"Verify that 'Add {UserType.CompanyAdmin}' popup should be opened and data should be saved");
            Assert.IsTrue(manageUserPageCompanyAdmin.IsUserPopupPresent(), $"{UserType.CompanyAdmin} popup is not opened");

            manageUserPageCompanyAdmin.ClickCancelButton();
            Assert.IsTrue(manageUserPageCompanyAdmin.IsUserDisplayed(companyAdminInfo.Email),
                $"Failure !! Company Admin with First name: {companyAdminInfo.FirstName} and Last name: {companyAdminInfo.LastName} is not deleted");

            Log.Info($"Edit {companyAdminInfo.Email} user and verify all languages from 'Preferred Language' dropdown'");
            manageUserPageCompanyAdmin.ClickOnEditUserIcon(companyAdminInfo.Email);
            actualLanguageList = manageUserPageCompanyAdmin.GetAllLanguages();
            Assert.That.ListsAreEqual(ExpectedLanguageList, actualLanguageList, "Language list doesn't match");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void ManageUser_SaveAndAddNew_OrganizationalLeader()
        {
            var loginPage = new LoginPage(Driver, Log);
            var manageUserPageOrganizationalLeaders = new ManageUserPage(Driver, Log, UserType.OrganizationalLeader);

            Log.Info("Take a login as organizational Leader and navigate to 'Manage User' tab");
            loginPage.NavigateToPage();
            loginPage.LoginToApplication(User.Username, User.Password);
            manageUserPageOrganizationalLeaders.NavigateToPage(Company.Id);
            manageUserPageOrganizationalLeaders.SelectTab();

            Log.Info("Fill all the user details and click on 'Save And Add New' button");
            manageUserPageOrganizationalLeaders.ClickOnAddNewUserButton();
            var organizationalLeadersInfo = new OrganizationalLeadersInfo
            {
                FirstName = RandomDataUtil.GetFirstName(),
                LastName = SharedConstants.TeamMemberLastName,
                Email = Constants.UserEmailPrefix + "_OL_" + CSharpHelpers.RandomNumber() + Constants.UserEmailDomain,
                NotifyUser = true,
                CanSeeTeamName = false,
                CanViewSubteams = false,
                Team = Constants.MultiTeamForBenchmarking,
                ImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\TestData\\1.jpg"),
                ActiveAhf = true,
                AhTrainer = true
            };

            Log.Info("Verify all languages from 'Preferred Language' dropdown'");
            var actualLanguageList = manageUserPageOrganizationalLeaders.GetAllLanguages();
            Assert.That.ListsAreEqual(ExpectedLanguageList, actualLanguageList, "Language list doesn't match");

            manageUserPageOrganizationalLeaders.EnterOlInfo(organizationalLeadersInfo);
            manageUserPageOrganizationalLeaders.ClickSaveAndAddNewButton();

            Log.Info($"Verify that 'Add {UserType.OrganizationalLeader}' popup should be opened and data should be saved");
            Assert.IsTrue(manageUserPageOrganizationalLeaders.IsUserPopupPresent(), $"{UserType.OrganizationalLeader} popup is not opened");

            manageUserPageOrganizationalLeaders.ClickCancelButton();
            Assert.IsTrue(manageUserPageOrganizationalLeaders.IsUserDisplayed(organizationalLeadersInfo.Email),
                $"Failure !! organizational Leader with First name: {organizationalLeadersInfo.FirstName} and Last name: {organizationalLeadersInfo.LastName} is not deleted");

            Log.Info($"Edit {organizationalLeadersInfo.Email} user and verify all languages from 'Preferred Language' dropdown'");
            manageUserPageOrganizationalLeaders.ClickOnEditUserIcon(organizationalLeadersInfo.Email);
            actualLanguageList = manageUserPageOrganizationalLeaders.GetAllLanguages();
            Assert.That.ListsAreEqual(ExpectedLanguageList, actualLanguageList, "Language list doesn't match");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void ManageUser_SaveAndAddNew_BusinessLineAdmin()
        {
            var loginPage = new LoginPage(Driver, Log);
            var manageUserPageBusinessLineAdmin = new ManageUserPage(Driver, Log, UserType.BusinessLineAdmin);

            Log.Info("Take a login as business Line Admin and navigate to 'Manage User' tab");
            loginPage.NavigateToPage();
            loginPage.LoginToApplication(User.Username, User.Password);
            manageUserPageBusinessLineAdmin.NavigateToPage(Company.Id);
            manageUserPageBusinessLineAdmin.SelectTab();

            Log.Info("Fill all the user details and click on 'Save And Add New' button");
            manageUserPageBusinessLineAdmin.ClickOnAddNewUserButton();
            var blAdminInfo = new BlAdminInfo
            {
                FirstName = RandomDataUtil.GetFirstName(),
                LastName = SharedConstants.TeamMemberLastName,
                Email = Constants.UserEmailPrefix + "_BL_" + CSharpHelpers.RandomNumber() + Constants.UserEmailDomain,
                PreferredLanguage = "German",
                NotifyUser = true,
                Tag = SharedConstants.TeamTag,
                ImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\TestData\\1.jpg"),
                ActiveAhf = true,
                AhTrainer = true
            };

            Log.Info("Verify all languages from 'Preferred Language' dropdown'");
            var actualLanguageList = manageUserPageBusinessLineAdmin.GetAllLanguages();
            Assert.That.ListsAreEqual(ExpectedLanguageList, actualLanguageList, "Language list doesn't match");

            manageUserPageBusinessLineAdmin.EnterBlAdminInfo(blAdminInfo);
            manageUserPageBusinessLineAdmin.ClickSaveAndAddNewButton();

            Log.Info($"Verify that 'Add {UserType.BusinessLineAdmin}' popup should be opened and data should be saved");
            Assert.IsTrue(manageUserPageBusinessLineAdmin.IsUserPopupPresent(), $"{UserType.BusinessLineAdmin} popup is not opened");

            manageUserPageBusinessLineAdmin.ClickCancelButton();
            Assert.IsTrue(manageUserPageBusinessLineAdmin.IsUserDisplayed(blAdminInfo.Email),
                $"Failure !! Business Line Admin with First name: {blAdminInfo.FirstName} and Last name: {blAdminInfo.LastName} is not deleted");

            Log.Info($"Edit {blAdminInfo.Email} user and verify all languages from 'Preferred Language' dropdown'");
            manageUserPageBusinessLineAdmin.ClickOnEditUserIcon(blAdminInfo.Email);
            actualLanguageList = manageUserPageBusinessLineAdmin.GetAllLanguages();
            Assert.That.ListsAreEqual(ExpectedLanguageList, actualLanguageList, "Language list doesn't match");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void ManageUser_SaveAndAddNew_TeamAdmin()
        {
            var loginPage = new LoginPage(Driver, Log);
            var manageUserPageTeamAdmins = new ManageUserPage(Driver, Log, UserType.TeamAdmin);

            Log.Info("Take a login as Team Admin and navigate to 'Manage User' tab");
            loginPage.NavigateToPage();
            loginPage.LoginToApplication(User.Username, User.Password);
            manageUserPageTeamAdmins.NavigateToPage(Company.Id);
            manageUserPageTeamAdmins.SelectTab();

            Log.Info("Fill all the user details and click on 'Save And Add New' button");
            manageUserPageTeamAdmins.ClickOnAddNewUserButton();
            var teamAdminInfo = new TeamAdminInfo
            {
                FirstName = RandomDataUtil.GetFirstName(),
                LastName = SharedConstants.TeamMemberLastName,
                Email = Constants.UserEmailPrefix + "_TA_" + CSharpHelpers.RandomNumber() + Constants.UserEmailDomain,
                PreferredLanguage = "Spanish",
                NotifyUser = true,
                Team = SharedConstants.Team,
                ImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\TestData\\1.jpg"),
                ActiveAhf = true,
                AhTrainer = true
            };

            Log.Info("Verify all languages from 'Preferred Language' dropdown'");
            var actualLanguageList = manageUserPageTeamAdmins.GetAllLanguages();
            Assert.That.ListsAreEqual(ExpectedLanguageList, actualLanguageList, "Language list doesn't match");

            manageUserPageTeamAdmins.EnterTeamAdminInfo(teamAdminInfo);
            manageUserPageTeamAdmins.ClickSaveAndAddNewButton();

            Log.Info($"Verify that 'Add {UserType.TeamAdmin}' popup should be opened and data should be saved");
            Assert.IsTrue(manageUserPageTeamAdmins.IsUserPopupPresent(), $"{UserType.TeamAdmin} popup is not opened");

            manageUserPageTeamAdmins.ClickCancelButton();
            Assert.IsTrue(manageUserPageTeamAdmins.IsUserDisplayed(teamAdminInfo.Email),
                $"Failure !! Team Admin with First name: {teamAdminInfo.FirstName} and Last name: {teamAdminInfo.LastName} is not deleted");

            Log.Info($"Edit {teamAdminInfo.Email} user and verify all languages from 'Preferred Language' dropdown'");
            manageUserPageTeamAdmins.ClickOnEditUserIcon(teamAdminInfo.Email);
            actualLanguageList = manageUserPageTeamAdmins.GetAllLanguages();
            Assert.That.ListsAreEqual(ExpectedLanguageList, actualLanguageList, "Language list doesn't match");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void ManageUser_SaveAndAddNew_Coaches()
        {
            var loginPage = new LoginPage(Driver, Log);
            var manageUserPageCoach = new ManageUserPage(Driver, Log, UserType.Coaches);

            Log.Info("Take a login as coach and navigate to 'Manage User' tab");
            loginPage.NavigateToPage();
            loginPage.LoginToApplication(User.Username, User.Password);
            manageUserPageCoach.NavigateToPage(Company.Id);
            manageUserPageCoach.SelectTab();

            Log.Info("Fill all the user details and click on 'Save And Add New' button");
            manageUserPageCoach.ClickOnAddNewUserButton();
            var coachInfo = new CoachInfo()
            {
                FirstName = RandomDataUtil.GetFirstName(),
                LastName = SharedConstants.TeamMemberLastName,
                Email = Constants.UserEmailPrefix + CSharpHelpers.RandomNumber() + Constants.UserEmailDomain,
                PreferredLanguage = "Japanese",
                LinkedIn = "https://linkedin.com/agilityhealth",
                Bio = "This is Bio",
                ImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\TestData\\1.jpg"),
                ActiveAhf = true,
                AhTrainer = true
            };

            Log.Info("Verify all languages from 'Preferred Language' dropdown'");
            var actualLanguageList = manageUserPageCoach.GetAllLanguages();
            Assert.That.ListsAreEqual(ExpectedLanguageList, actualLanguageList, "Language list doesn't match");
            manageUserPageCoach.EnterCoachInfo(coachInfo);
            manageUserPageCoach.ClickSaveAndAddNewButton();

            Log.Info($"Verify that 'Add {UserType.Coaches}' popup should be opened and data should be saved");
            Assert.IsTrue(manageUserPageCoach.IsUserPopupPresent(), $"{UserType.Coaches} popup is not opened");

            manageUserPageCoach.ClickCancelButton();
            Assert.IsTrue(manageUserPageCoach.IsUserDisplayed(coachInfo.Email),
                $"Failure !! coach Leader with First name: {coachInfo.FirstName} and Last name: {coachInfo.LastName} is not deleted");

            Log.Info($"Edit {coachInfo.Email} user and verify all languages from 'Preferred Language' dropdown'");
            manageUserPageCoach.ClickOnEditUserIcon(coachInfo.Email);
            actualLanguageList = manageUserPageCoach.GetAllLanguages();
            Assert.That.ListsAreEqual(ExpectedLanguageList, actualLanguageList, "Language list doesn't match");
        }

    }
}