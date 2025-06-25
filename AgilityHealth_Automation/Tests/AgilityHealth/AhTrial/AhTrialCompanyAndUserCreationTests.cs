using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Common;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Company;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams.QuickLaunch;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.ManageUsers;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.QuickLaunch;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos.AhTrial;
using AtCommon.Dtos.Companies;
using AtCommon.ObjectFactories;
using AtCommon.ObjectFactories.QuickLaunch;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace AgilityHealth_Automation.Tests.AgilityHealth.AhTrial
{
    [TestClass]
    [TestCategory("Critical")]
    [TestCategory("AhTrial")]
    public class AhTrialCompanyAndUserCreationTests : BaseTest
    {
        private static bool _classInitFailed;
        private static AhTrialCompanyRequest _ahTrialCompanyRequest;
        private static AhTrialBaseCompanyResponse _ahTrialBaseCompanyResponse;
        private static CompanyResponse _companyResponse;

        [ClassInitialize]
        public static void SetUp(TestContext _)
        {
            try
            {
                _ahTrialCompanyRequest = AhTrialFactory.GetValidAhTrialCompany();

                var setup = new SetupTeardownApi(TestEnvironment);
                _ahTrialBaseCompanyResponse = setup.CreateAhTrialCompany(_ahTrialCompanyRequest);
                _companyResponse = setup.GetCompany(Constants.AGlobalFinancialCompany).GetAwaiter().GetResult();
            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }

        [TestMethod]
        [TestCategory("SiteAdmin")]
        [TestCategory("KnownDefect")] //Bug Id: 47507
        public void AhTrial_Create_User_Successfully()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);
            var manageUserPage = new ManageUserPage(Driver, Log, UserType.CompanyAdmin);
            var manageUserPageTeamAdmin = new ManageUserPage(Driver, Log, UserType.TeamAdmin);
            var selectCompanyPage = new SelectCompanyPage(Driver);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);

            Log.Info("Login as SA");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Navigate to 'Manage User' page and verify user is created on 'Company Admins' page");
            manageUserPage.NavigateToPage(_ahTrialBaseCompanyResponse.Id);
            manageUserPage.SelectTab();
            Assert.IsTrue(manageUserPage.IsUserDisplayed(_ahTrialCompanyRequest.Email), "Failure !! Company Admin with First name: " + _ahTrialCompanyRequest.FirstName + " and Last name: " + _ahTrialCompanyRequest.LastName + " is not created");

            Log.Info("Logout and Login as a newly created CA user");
            topNav.LogOut();
            login.LoginToApplication(_ahTrialCompanyRequest.Email, _ahTrialCompanyRequest.Password);

            Log.Info($"Select {_ahTrialBaseCompanyResponse.Name} company and verify teams dashboard");
            selectCompanyPage.SelectCompany(_ahTrialBaseCompanyResponse.Name);
            var breadcrumbs = topNav.GetBreadcrumbText();
            Assert.IsTrue(breadcrumbs.Contains(_ahTrialBaseCompanyResponse.Name), $"The company name <{_ahTrialBaseCompanyResponse.Name}> was not found in the breadcrumbs <{breadcrumbs}.>");

            Log.Info("Verify user name and role at header");
            var expectedCaNameAndRoleText = topNav.GetUserNameAndRoleText();
            var actualCaNameAndRoleText = _ahTrialBaseCompanyResponse.CompanyFirstName + " " + _ahTrialBaseCompanyResponse.CompanyLastName + "Company Admin";
            Assert.AreEqual(expectedCaNameAndRoleText, actualCaNameAndRoleText, "User's name and role is matched");

            Log.Info($"Hover on user's name and click on 'Switch Companies' then select {Constants.AGlobalFinancialCompany} and verify teams");
            topNav.ClickOnSwitchCompaniesButton();
            selectCompanyPage.SelectCompany(Constants.AGlobalFinancialCompany);

            dashBoardPage.CloseWelcomePopup();
            var actualTeamNames = dashBoardPage.GetAllTeamsNames();
            var expectedTeamNames = new List<string>
            {
                "Arrested Development",
                "DevOps Diamondbacks- (DevOps)",
                "DevOps Program Team",
                "Diversification Divas (TeamHealth)",
                "Dons and Divas",
                "Exploration Station (TeamHealth)",
                "HR, Payroll and Training",
                "Invest Intel - (TeamHealth, SAFe Team & Tech Agility)",
                "Mike and the Mechanics (TeamHealth)",
                "SAFe & Sound -(SAFe 4.5 RTE, SAFe D/O)",
                "The Guardians (DevOps)",
                "The Other Guys (TeamHealth/DevOps)",
                "Training Wheels"
            };
            Assert.That.ListsAreEqual(expectedTeamNames, actualTeamNames, "Team names list is not matched");

            Log.Info("Verify user's name and role at header");
            var expectedTaNameAndRoleText = topNav.GetUserNameAndRoleText();
            var actualTaNameAndRoleText = _ahTrialBaseCompanyResponse.CompanyFirstName + " " + _ahTrialBaseCompanyResponse.CompanyLastName + "Team Admin";
            Assert.AreEqual(expectedTaNameAndRoleText, actualTaNameAndRoleText, "User's name and role are is matched");

            Log.Info($"Logout then login in as a SA, Navigate to 'Manage User' page and verify that user is added to {Constants.AGlobalFinancialCompany} as a 'Team Admins'");
            topNav.LogOut();
            login.LoginToApplication(User.Username, User.Password);
            manageUserPage.NavigateToPage(_companyResponse.Id);
            manageUserPageTeamAdmin.SelectTab();
            manageUserPageTeamAdmin.SearchUserOnUserTab(_ahTrialCompanyRequest.Email);
            Assert.IsTrue(manageUserPageTeamAdmin.IsUserDisplayed(_ahTrialCompanyRequest.Email), "Failure !! Company Admin with First name: " + _ahTrialCompanyRequest.FirstName + " and Last name: " + _ahTrialCompanyRequest.LastName + " is not created");
        }

        [TestMethod]
        [TestCategory("SiteAdmin")]
        [TestCategory("KnownDefect")] //Bug Id: 47507
        public void AhTrial_Create_Company_Successfully()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var companyDashboardPage = new CompanyDashboardPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);
            var quickLaunchAssessmentPage = new QuickLaunchAssessmentPage(Driver, Log);
            var quickLaunchTeamPage = new QuickLaunchTeamPage(Driver, Log);

            Log.Info("Login as SA and verify that Company dashboard is rendered");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Search for the created Company and verify that it is displayed");
            companyDashboardPage.Search(_ahTrialBaseCompanyResponse.Name);
            Assert.IsTrue(companyDashboardPage.IsCompanyPresent(_ahTrialBaseCompanyResponse.Name), $"Company {_ahTrialBaseCompanyResponse.Name} is not present");

            Log.Info("Validate the company Subscription shows as 'Trial'");
            Assert.AreEqual("Trial", companyDashboardPage.GetColumnValues("Subscript").ListToString(), "Subscript does not match");

            companyDashboardPage.ClickOnCompanyName(_ahTrialBaseCompanyResponse.Name);

            Log.Info("Verify that the user was taken to the Teams Dashboard");
            var breadcrumbs = topNav.GetBreadcrumbText();
            Assert.IsTrue(breadcrumbs.Contains(_ahTrialBaseCompanyResponse.Name), $"The company name <{_ahTrialBaseCompanyResponse.Name}> was not found in the breadcrumbs <{breadcrumbs}.>");

            Log.Info("Click on 'Assessment' from 'Quick Launch' then select 'Select Radar' dropdown and verify the radars list");
            quickLaunchAssessmentPage.ClickOnQuickLaunchOptionsLink(QuickLaunchAssessmentPage.QuickLaunchOptions.Assessment.ToString());
            quickLaunchAssessmentPage.ClickOnSelectRadarDropdown();
            var expectedAhTrialRadarList = QuickLaunchAssessmentFactory.GetExpectedAhTrialRadarList();
            var actualAhTrialRadarList = quickLaunchAssessmentPage.GetQuickLaunchAssessmentPopupRadarList();
            Assert.That.ListsAreEqual(expectedAhTrialRadarList, actualAhTrialRadarList, "Radar List doesn't match");

            Driver.RefreshPage();

            Log.Info("Click on 'Team' from 'Quick Launch' then select 'Select Work Type' dropdown and verify the work types list");
            quickLaunchAssessmentPage.ClickOnQuickLaunchOptionsLink(QuickLaunchAssessmentPage.QuickLaunchOptions.Team.ToString());
            quickLaunchTeamPage.ClickOnWorkTypeDropdown();
            var expectedAhTrialWorkTypeList = QuickLaunchAssessmentFactory.GetExpectedAhTrialWorkTypeList();
            var actualAhTrialWorkTypeList = quickLaunchTeamPage.GetWorkTypeList();
            foreach (var worktype in expectedAhTrialWorkTypeList)
            {
                Assert.That.ListContains(actualAhTrialWorkTypeList, worktype, "Work Type List doesn't match");
            }
        }
    }

}
