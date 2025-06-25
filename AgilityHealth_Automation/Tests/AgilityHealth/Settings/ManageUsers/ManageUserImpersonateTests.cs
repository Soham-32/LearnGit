using System;
using System.IO;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.DataObjects;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.ManageUsers;
using AgilityHealth_Automation.PageObjects.AgilityHealth.V2;
using AgilityHealth_Automation.Utilities;
using AtCommon.Dtos;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Settings.ManageUsers
{
    [TestClass]
    [TestCategory("ManageUser"), TestCategory("Settings")]
    public class ManageUserImpersonateTests : BaseTest
    {
        private static User User7 => TestEnvironment.UserConfig.GetUserByDescription("user 7");

        //[TestMethod]
        //[TestCategory("CompanyAdmin")]
        public void ManageUsers_Impersonate_As_OlAdmin_AhTrainer()
        {
            var login = new LoginPage(Driver, Log);
            var manageUserPage = new ManageUserPage(Driver, Log, UserType.OrganizationalLeader);
            var topNav = new HeaderFooterPage(Driver, Log);
            var teamDashboard = new TeamDashboardPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User7.Username, User7.Password);

            manageUserPage.NavigateToPage(Company.Id);
            manageUserPage.SelectTab();

            manageUserPage.ClickOnAddNewUserButton();

            var organizationalLeadersInfo = new OrganizationalLeadersInfo
            {
                FirstName = RandomDataUtil.GetFirstName(),
                LastName = SharedConstants.TeamMemberLastName,
                Email = Constants.UserEmailPrefix + "_OL_" + CSharpHelpers.RandomNumber() + Constants.UserEmailDomain,
                NotifyUser = true,
                CanSeeTeamName = true,
                CanViewSubteams = true,
                Team = Constants.MultiTeamForBenchmarking,
                ImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\TestData\\1.jpg"),
                AhTrainer = true
            };

            manageUserPage.EnterOlInfo(organizationalLeadersInfo);
            manageUserPage.ClickSaveAndCloseButton();
            manageUserPage.ClickOnEditUserIcon(organizationalLeadersInfo.Email);
            Assert.IsTrue(manageUserPage.IsAhTrainerCheckboxSelected(), "Ah Trainer Checkbox is not selected, when it should be selected");
            manageUserPage.ClickCancelButton();

            manageUserPage.ClickOnImpersonateButton(organizationalLeadersInfo.Email);
            Assert.IsTrue(teamDashboard.IsFacilitatorDashboardDisplayed(), "Facilitator Dashboard is not present");
            topNav.ClickOnStopImpersonateLink();

            manageUserPage.NavigateToPage(Company.Id);
            manageUserPage.SelectTab(); 
            manageUserPage.ClickOnEditUserIcon(organizationalLeadersInfo.Email);

            var organizationalLeadersInfoEdited = new OrganizationalLeadersInfo
            {
                FirstName = RandomDataUtil.GetFirstName(),
                LastName = $"{SharedConstants.TeamMemberLastName}Edited",
                Email = Constants.UserEmailPrefix + "_OL_edit" + CSharpHelpers.RandomNumber() + Constants.UserEmailDomain,
                CanSeeTeamName = false,
                CanViewSubteams = false
            };

            manageUserPage.EditOlInfo(organizationalLeadersInfoEdited);
            manageUserPage.ClickSaveAndCloseButton();

            manageUserPage.ClickOnEditUserIcon(organizationalLeadersInfoEdited.Email);
            Assert.IsFalse(manageUserPage.IsAhTrainerCheckboxSelected(), "Ah Trainer Checkbox is selected, when it should not be selected");
            manageUserPage.ClickCancelButton();

            manageUserPage.ClickOnImpersonateButton(organizationalLeadersInfoEdited.Email);
            Assert.IsFalse(teamDashboard.IsFacilitatorDashboardDisplayed(), "Facilitator Dashboard is not present");
            
            //Clean up
            try
            {
                topNav.ClickOnStopImpersonateLink();
            }
            catch
            {
                //Nothing
            }
        }
    }
}