using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.DataObjects.NewNavigation.Teams;
using AgilityHealth_Automation.ObjectFactories.NewNavigation.Teams;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Common;
using AgilityHealth_Automation.PageObjects.AgilityHealth.NewNavigation.Teams.Create;
using AgilityHealth_Automation.PageObjects.AgilityHealth.NewNavigation.Teams.Edit;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Survey.QuickLaunch;
using AgilityHealth_Automation.Utilities;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories.QuickLaunch;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using TeamDashboardPage = AgilityHealth_Automation.PageObjects.AgilityHealth.NewNavigation.Dashboard.TeamDashboardPage;
using TeamType = AgilityHealth_Automation.Enum.NewNavigation.TeamType;

namespace AgilityHealth_Automation.Tests.AgilityHealth.NewNavigation
{
    [TestClass]
    public class NewNavBaseTest : BaseTest
    {
        //Create Team Stepper
        protected const string CreateTeamStepperTitle = "Let'S Walk Through Creating A Team";
        protected const string CreateTeamStepperInfo = "Make sure to upload a team photo if you have one. Team tags will help you filter later.";

        //Sub Team Stepper
        protected const string SubTeamStepperTitle = "Let'S Add Sub-Teams";
        protected const string SubTeamStepperInfo = "Select the sub-teams from the list below.";

        //Validation message
        protected const string ValidationMessageForTeamMemberLeaderStakeholderPopup = "This value is required.";

        private Team TeamInfo = TeamsFactory.GetValidFullWizardTeamInfo();

        protected void SwitchToNewNav()
        {
            var teamDashboardPage = new TeamDashboardPage(Driver, Log);
            var header = new TopNavigation(Driver, Log);

            new SeleniumWait(Driver).HardWait(5000); //Wait until new nav page is loaded

            var newNavUrl = teamDashboardPage.GetTeamDashboardUrl(Company.Id);
            if (Driver.GetCurrentUrl() != newNavUrl)
            {
                header.ClickOnSwitchNewNavButton();
            }
        }

        public void Verify_Team_CreateTeamStepper_Title_InfoText_ContinueButton(TeamType teamType)
        {
            var login = new LoginPage(Driver, Log);
            var teamDashboardPage = new TeamDashboardPage(Driver, Log);
            var createTeamStepperPage = new CreateTeamStepperPage(Driver, Log);

            TeamInfo = teamType switch
            {
                TeamType.MultiTeam => MultiTeamsFactory.GetValidMultiTeamInfo(),
                TeamType.PortfolioTeam => PortfolioTeamsFactory.GetValidPortfolioTeamInfo(),
                TeamType.EnterpriseTeam => EnterpriseTeamsFactory.GetValidEnterpriseTeamInfo(),
                _ => TeamInfo
            };

            Log.Info("Login to the application and switch to new navigation then switch to grid view on team dashboard");
            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(User.Username, User.Password);
            SwitchToNewNav();
            teamDashboardPage.SwitchToGridView();


            Log.Info($"Click on 'Add a New Team' button and select team type to create a '{teamType}'");
            teamDashboardPage.ClickOnAddNewTeamButtonAndSelectTeamType(teamType);
            var actualCreateTeamStepperTitle = createTeamStepperPage.GetCreateTeamStepperTitle();
            Assert.AreEqual(CreateTeamStepperTitle, actualCreateTeamStepperTitle, "'Create Team' stepper header title is not matched");

            var actualCreateTeamStepperInfo = createTeamStepperPage.GetCreateTeamStepperInfo();
            Assert.AreEqual(CreateTeamStepperInfo, actualCreateTeamStepperInfo, "'Create Team' stepper info text is not matched");

            Log.Info("Verify default state of 'Continue To Add Sub Teams' or 'Continue To Add Team Members' button");
            Assert.IsFalse(createTeamStepperPage.IsContinueToAddSubTeamsOrTeamMembersButtonEnabled(), "'Continue To Add Sub Teams' or 'Continue To Add Team Members' button is enabled");

            Log.Info("Enter mandatory fields and verify 'Continue To Add Sub Teams' or 'Continue To Add Team Members' button");
            createTeamStepperPage.EnterTeamName(TeamInfo.TeamName);
            createTeamStepperPage.SelectWorkType(TeamInfo.WorkType);
            Assert.IsTrue(createTeamStepperPage.IsContinueToAddSubTeamsOrTeamMembersButtonEnabled(), "'Continue To Add Sub Teams' or 'Continue To Add Team Members' button is disabled");

            Log.Info("Remove team name and verify 'Continue To Add Sub Teams' or 'Continue To Add Team Members' button");
            createTeamStepperPage.RemoveTeamName();
            Assert.IsFalse(createTeamStepperPage.IsContinueToAddSubTeamsOrTeamMembersButtonEnabled(), "'Continue To Add Sub Teams' or 'Continue To Add Team Members' button is enabled");

            if (teamType != TeamType.EnterpriseTeam)
            {
                Log.Info("Remove 'Work Type' and verify 'Continue To Add Sub Teams' or 'Continue To Add Team Members' button");
                createTeamStepperPage.EnterTeamName(TeamInfo.TeamName);
                createTeamStepperPage.RemoveWorkType();
                Assert.IsFalse(createTeamStepperPage.IsContinueToAddSubTeamsOrTeamMembersButtonEnabled(), "'Continue To Add Sub Teams' or 'Continue To Add Team Members' button is enabled");
            }

        }

        public void Verify_Team_AddTeamMembersLeadersStepper_AddEditDelete_Successfully(TeamType teamType)
        {
            var login = new LoginPage(Driver, Log);
            var teamDashboardPage = new TeamDashboardPage(Driver, Log);
            var createTeamStepperPage = new CreateTeamStepperPage(Driver, Log);
            var addTeamSubTeamPage = new AddTeamSubTeamStepperPage(Driver, Log);
            var createTeamMemberLeaderStepperPage = new AddTeamMembersStepperPage(Driver, Log);

            TeamInfo = teamType switch
            {
                TeamType.MultiTeam => MultiTeamsFactory.GetValidMultiTeamInfo(),
                TeamType.PortfolioTeam => PortfolioTeamsFactory.GetValidPortfolioTeamInfo(),
                TeamType.EnterpriseTeam => EnterpriseTeamsFactory.GetValidEnterpriseTeamInfo(),
                _ => TeamInfo
            };

            var teamMemberInfo = TeamsFactory.GetTeamMemberInfo();
            var editTeamMemberInfo = TeamsFactory.GetEditTeamMemberInfo();
            var selectedRoleList = teamMemberInfo.Role;
            var selectedParticipantGroupList = teamMemberInfo.ParticipantGroup;

            Log.Info("Login to the application and switch to new navigation");
            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(User.Username, User.Password);
            SwitchToNewNav();

            Log.Info($"Click on 'Add a New Team' button and select '{teamType}' type to create a team");
            teamDashboardPage.SwitchToGridView();
            teamDashboardPage.ClickOnAddNewTeamButtonAndSelectTeamType(teamType);

            Log.Info("Enter team info and continue till 'Leaders' or 'Team Members' stepper");
            createTeamStepperPage.EnterTeamInfo(TeamInfo);
            createTeamStepperPage.ClickOnContinueToSubTeamOrTeamMemberButton();
            if (TeamType.MultiTeam == teamType | TeamType.PortfolioTeam == teamType | TeamType.EnterpriseTeam == teamType) { addTeamSubTeamPage.ClickOnContinueToAddLeadersButton(); }

            Log.Info("Create new team member or leader and verify");
            createTeamMemberLeaderStepperPage.OpenAddTeamMembersOrLeadersPopup();
            createTeamMemberLeaderStepperPage.PopupBase.EnterTeamMemberOrLeadersInfo(teamMemberInfo);
            createTeamMemberLeaderStepperPage.PopupBase.ClickOnCreateAndCloseButton();

            var actualLeaders = createTeamMemberLeaderStepperPage.GetTeamMemberOrStakeholderOrLeadersInfoFromGrid(teamMemberInfo.Email);
            Assert.AreEqual(teamMemberInfo.FirstName, actualLeaders.FirstName, "First name doesn't match");
            Assert.AreEqual(teamMemberInfo.LastName, actualLeaders.LastName, "Last name doesn't match");
            Assert.AreEqual(teamMemberInfo.Email, actualLeaders.Email, "Email doesn't match");
            if (TeamType.FullWizardTeam == teamType)
            {
                Assert.That.ListsAreEqual(teamMemberInfo.Role, actualLeaders.Role, "Role doesn't matched");
                Assert.That.ListsAreEqual(teamMemberInfo.ParticipantGroup, actualLeaders.ParticipantGroup, "Tags doesn't matched");
            }

            Log.Info("Edit team member or leader information and verify on grid");
            createTeamMemberLeaderStepperPage.ClickTeamMemberEditButton(teamMemberInfo.Email);

            createTeamMemberLeaderStepperPage.PopupBase.EnterTeamMemberOrLeadersInfo(editTeamMemberInfo, true, selectedRoleList, selectedParticipantGroupList);
            createTeamMemberLeaderStepperPage.PopupBase.ClickOnFirstName();
            createTeamMemberLeaderStepperPage.PopupBase.ClickOnUpdateButton();
            Assert.IsFalse(createTeamMemberLeaderStepperPage.IsTeamMemberDisplayed(teamMemberInfo.Email), "Old team member or leader is displayed");
            Assert.IsTrue(createTeamMemberLeaderStepperPage.IsTeamMemberDisplayed(editTeamMemberInfo.Email), "Edited team member or leader is not displayed");

            var actualEditedLeaders = createTeamMemberLeaderStepperPage.GetTeamMemberOrStakeholderOrLeadersInfoFromGrid(editTeamMemberInfo.Email);
            Assert.AreEqual(editTeamMemberInfo.FirstName, actualEditedLeaders.FirstName, "First name doesn't matched");
            Assert.AreEqual(editTeamMemberInfo.LastName, actualEditedLeaders.LastName, "Last name doesn't matched");
            Assert.AreEqual(editTeamMemberInfo.Email, actualEditedLeaders.Email, "Email doesn't matched");
            if (TeamType.FullWizardTeam == teamType)
            {
                Assert.That.ListsAreEqual(editTeamMemberInfo.Role, actualEditedLeaders.Role, "Role doesn't matched");
                Assert.That.ListsAreEqual(editTeamMemberInfo.ParticipantGroup, actualEditedLeaders.ParticipantGroup, "Tags doesn't matched");
            }

            Log.Info("Delete team member or leader and verify on grid");
            createTeamMemberLeaderStepperPage.ClickOnDeleteButton(editTeamMemberInfo.Email);
            Assert.IsFalse(createTeamMemberLeaderStepperPage.IsTeamMemberDisplayed(editTeamMemberInfo.Email), "team member or leader is displayed");
        }

        public void Verify_Team_AddTeamMembersLeadersStepper_ValidationMessage(TeamType teamType)
        {
            var login = new LoginPage(Driver, Log);
            var teamDashboardPage = new TeamDashboardPage(Driver, Log);
            var createTeamStepperPage = new CreateTeamStepperPage(Driver, Log);
            var addTeamSubTeamPage = new AddTeamSubTeamStepperPage(Driver, Log);
            var addTeamMemberLeadersStepperPage = new AddTeamMembersStepperPage(Driver, Log);

            TeamInfo = teamType switch
            {
                TeamType.MultiTeam => MultiTeamsFactory.GetValidMultiTeamInfo(),
                TeamType.PortfolioTeam => PortfolioTeamsFactory.GetValidPortfolioTeamInfo(),
                TeamType.EnterpriseTeam => EnterpriseTeamsFactory.GetValidEnterpriseTeamInfo(),
                _ => TeamInfo
            };

            var leadersInfo = TeamsFactory.GetTeamMemberInfo();
            var editLeadersInfo = TeamsFactory.GetEditTeamMemberInfo();

            Log.Info("Login to the application and switch to new navigation");
            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(User.Username, User.Password);
            SwitchToNewNav();

            Log.Info($"Click on 'Add a New Team' button and select {teamType} to create a team");
            teamDashboardPage.SwitchToGridView();
            teamDashboardPage.ClickOnAddNewTeamButtonAndSelectTeamType(teamType);

            Log.Info("Enter team info and continue till 'Team Members' or 'Leaders' stepper");
            createTeamStepperPage.EnterTeamInfo(TeamInfo);
            createTeamStepperPage.ClickOnContinueToSubTeamOrTeamMemberButton();
            if (TeamType.MultiTeam == teamType | TeamType.PortfolioTeam == teamType | TeamType.EnterpriseTeam == teamType) { addTeamSubTeamPage.ClickOnContinueToAddLeadersButton(); }

            Log.Info("Open 'Add New Team Member' or 'Add New Leaders' popup and verify 'Create And Add New' and 'Create And Close' button are disabled");
            addTeamMemberLeadersStepperPage.OpenAddTeamMembersOrLeadersPopup();
            Assert.IsFalse(addTeamMemberLeadersStepperPage.PopupBase.IsCreateAndAddNewButtonEnable(), "'Create And Add New' button is enabled");
            Assert.IsFalse(addTeamMemberLeadersStepperPage.PopupBase.IsCreateAndCloseButtonEnable(), "'Create And Close' button is enabled");

            Log.Info("Enter team members or leaders info and verify 'Create And Add New' and 'Create And Close' button are enabled");
            addTeamMemberLeadersStepperPage.PopupBase.EnterTeamMemberOrLeadersInfo(leadersInfo);
            Assert.IsTrue(addTeamMemberLeadersStepperPage.PopupBase.IsCreateAndAddNewButtonEnable(), "'Create And Add New' button is disabled");
            Assert.IsTrue(addTeamMemberLeadersStepperPage.PopupBase.IsCreateAndCloseButtonEnable(), "'Create And Close' button is disabled");

            Log.Info("Remove team member or leader info and verify the validation messages");
            addTeamMemberLeadersStepperPage.PopupBase.RemoveFirstName();
            addTeamMemberLeadersStepperPage.PopupBase.RemoveEmail();
            addTeamMemberLeadersStepperPage.PopupBase.RemoveLastName();
            addTeamMemberLeadersStepperPage.PopupBase.ClickOnFirstName();
            Assert.AreEqual(ValidationMessageForTeamMemberLeaderStakeholderPopup, addTeamMemberLeadersStepperPage.PopupBase.GetFieldValidationMessage("FirstName"), "Validation message for first name is not matched");
            Assert.AreEqual(ValidationMessageForTeamMemberLeaderStakeholderPopup, addTeamMemberLeadersStepperPage.PopupBase.GetFieldValidationMessage("LastName"), "Validation message for last name is not matched");
            Assert.AreEqual(ValidationMessageForTeamMemberLeaderStakeholderPopup, addTeamMemberLeadersStepperPage.PopupBase.GetFieldValidationMessage("Email"), "Validation message for email is not matched");

            Log.Info("Create team member or leader and open 'Edit Team Member' or 'Edit leader' popup via clicking on 'Edit' button for newly created team member or leader");
            addTeamMemberLeadersStepperPage.PopupBase.EnterFirstName(editLeadersInfo.FirstName);
            addTeamMemberLeadersStepperPage.PopupBase.EnterLastName(editLeadersInfo.LastName);
            addTeamMemberLeadersStepperPage.PopupBase.EnterEmail(editLeadersInfo.Email);
            Thread.Sleep(2000); //Wait until button is enabled
            addTeamMemberLeadersStepperPage.PopupBase.ClickOnFirstName();
            addTeamMemberLeadersStepperPage.PopupBase.ClickOnCreateAndCloseButton();
            addTeamMemberLeadersStepperPage.ClickTeamMemberEditButton(editLeadersInfo.Email);

            Log.Info("Remove email and verify the 'Update' button");
            addTeamMemberLeadersStepperPage.PopupBase.RemoveEmail();
            Assert.IsFalse(addTeamMemberLeadersStepperPage.PopupBase.IsUpdateButtonEnable(), "'Update' button is enabled");
        }

        public void Verify_Team_AddTeamMembersLeadersStepper_AddTeamMembersLeaders_ViaQuickLink(TeamType teamType)
        {
            var driver = Driver;
            var login = new LoginPage(Driver, Log);
            var teamDashboardPage = new TeamDashboardPage(Driver, Log);
            var createTeamStepperPage = new CreateTeamStepperPage(Driver, Log);
            var addTeamSubTeamPage = new AddTeamSubTeamStepperPage(Driver, Log);
            var addTeamMemberLeaderStepperPage = new AddTeamMembersStepperPage(Driver, Log);
            var quickLaunchMemberAccessPage = new QuickLaunchMemberAccessPage(Driver, Log);

            var getValidQuickLaunchLeaderAccessInfo = QuickLaunchAssessmentFactory.GetValidQuickLaunchMemberAccessInfo();

            TeamInfo = teamType switch
            {
                TeamType.MultiTeam => MultiTeamsFactory.GetValidMultiTeamInfo(),
                TeamType.PortfolioTeam => PortfolioTeamsFactory.GetValidPortfolioTeamInfo(),
                TeamType.EnterpriseTeam => EnterpriseTeamsFactory.GetValidEnterpriseTeamInfo(),
                _ => TeamInfo
            };

            Log.Info("Login to the application and switch to new navigation");
            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(User.Username, User.Password);
            SwitchToNewNav();

            Log.Info($"Click on 'Add a New Team' button and select '{teamType}' type to create a multi team");
            teamDashboardPage.SwitchToGridView();
            teamDashboardPage.ClickOnAddNewTeamButtonAndSelectTeamType(teamType);

            Log.Info("Enter team info and click on 'Continue To Add Sub-Teams' or 'Continue to Team Members' button");
            createTeamStepperPage.EnterTeamInfo(TeamInfo);
            createTeamStepperPage.ClickOnContinueToSubTeamOrTeamMemberButton();
            if (TeamType.MultiTeam == teamType | TeamType.PortfolioTeam == teamType | TeamType.EnterpriseTeam == teamType) { addTeamSubTeamPage.ClickOnContinueToAddLeadersButton(); }

            lock (ClipboardLock)
            {
                Log.Info("Click on team member or leader copy link and paste it in new tab");
                var leaderCopyLink = addTeamMemberLeaderStepperPage.GetCopiedQuickLinkText();
                driver.SwitchTo().NewWindow(WindowType.Tab);
                driver.NavigateToPage(leaderCopyLink);
            }

            Log.Info("Enter team member or leader details and click on 'Submit' button");
            quickLaunchMemberAccessPage.EnterQuickLaunchAssessmentAccessInfo(getValidQuickLaunchLeaderAccessInfo);
            quickLaunchMemberAccessPage.ClickOnSubmitButton();

            Log.Info("Navigate to first window, Refresh the page and verify the team member and leader info from grid");
            Driver.SwitchToFirstWindow();
            Driver.RefreshPage();
            var actualTeamMemberInfo = addTeamMemberLeaderStepperPage.GetTeamMemberOrStakeholderOrLeadersInfoFromGrid(getValidQuickLaunchLeaderAccessInfo.Email);
            Assert.AreEqual(getValidQuickLaunchLeaderAccessInfo.FirstName, actualTeamMemberInfo.FirstName, "First Name doesn't match");
            Assert.AreEqual(getValidQuickLaunchLeaderAccessInfo.LastName, actualTeamMemberInfo.LastName, "Last Name doesn't match");
            Assert.AreEqual(getValidQuickLaunchLeaderAccessInfo.Email, actualTeamMemberInfo.Email, "Email doesn't match");
            if (TeamType.FullWizardTeam == teamType)
            {
                Assert.That.ListsAreEqual(getValidQuickLaunchLeaderAccessInfo.Roles, actualTeamMemberInfo.Role, "Role doesn't match");
                Assert.That.ListsAreEqual(getValidQuickLaunchLeaderAccessInfo.ParticipantGroups, actualTeamMemberInfo.ParticipantGroup, "Participant doesn't match");
            }
        }

        public void Verify_Team_AddTeamMembersLeadersStepper_TeamMemberLeader_CanAccessTeam_Successfully(TeamType teamType)
        {
            var login = new LoginPage(Driver, Log);
            var header = new TopNavigation(Driver, Log);
            var teamDashboardPage = new TeamDashboardPage(Driver, Log);
            var createTeamStepperPage = new CreateTeamStepperPage(Driver, Log);
            var addTeamSubTeamPage = new AddTeamSubTeamStepperPage(Driver, Log);
            var addTeamMembersStepperPage = new AddTeamMembersStepperPage(Driver, Log);
            var leadersTabPage = new StakeholdersTabPage(Driver, Log);


            TeamInfo = teamType switch
            {
                TeamType.MultiTeam => MultiTeamsFactory.GetValidMultiTeamInfo(),
                TeamType.PortfolioTeam => PortfolioTeamsFactory.GetValidPortfolioTeamInfo(),
                TeamType.EnterpriseTeam => EnterpriseTeamsFactory.GetValidEnterpriseTeamInfo(),
                _ => TeamInfo
            };

            var teamMemberInfo = TeamsFactory.GetTeamMemberInfo();

            Log.Info("Login to the application and switch to new navigation");
            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(User.Username, User.Password);
            SwitchToNewNav();

            Log.Info($"Click on 'Add a New Team' button and select '{teamType}' type to create a team");
            teamDashboardPage.SwitchToGridView();
            teamDashboardPage.ClickOnAddNewTeamButtonAndSelectTeamType(teamType);

            Log.Info("Enter team info and continue till 'Leaders' or 'Team Members' stepper");
            createTeamStepperPage.EnterTeamInfo(TeamInfo);
            createTeamStepperPage.ClickOnContinueToSubTeamOrTeamMemberButton();
            if (TeamType.MultiTeam == teamType | TeamType.PortfolioTeam == teamType | TeamType.EnterpriseTeam == teamType) { addTeamSubTeamPage.ClickOnContinueToAddLeadersButton(); }

            Log.Info("Create new leader");
            addTeamMembersStepperPage.OpenAddTeamMembersOrLeadersPopup();
            addTeamMembersStepperPage.PopupBase.EnterTeamMemberOrLeadersInfo(teamMemberInfo);
            addTeamMembersStepperPage.PopupBase.ClickOnCreateAndCloseButton();

            Log.Info("Click on leader or team member 'Team Access' button and verify leader or team member should get account setup email");
            addTeamMembersStepperPage.ClickOnTeamMemberTeamAccessButton(teamMemberInfo.Email);
            Assert.IsTrue(addTeamMembersStepperPage.IsTeamMemberSuccessfullyTeamAccessIconDisplayed(teamMemberInfo.Email), "Leader or Team members doesn't have access");
            teamDashboardPage.NavigateToPage(Company.Id);
            header.LogOut();

            Assert.IsTrue(GmailUtil.DoesMemberEmailExist(SharedConstants.MemberAccountCreateEmailSubject, teamMemberInfo.Email, "Auto_TeamAccess", "", 40), "Member is not receiving email");

            Log.Info("Setup leader's or team member's account, Verify user navigated to 'Team Dashboard' page and team should be displayed");
            Driver.NavigateToPage(GmailUtil.GetUserCreateAccountLink(SharedConstants.MemberAccountCreateEmailSubject, teamMemberInfo.Email, "Auto_TeamAccess"));
            login.SetUserPassword(SharedConstants.CommonPassword);

            Log.Info("Switch to new navigation and search the team");
            SwitchToNewNav();
            teamDashboardPage.NavigateToPage(Company.Id);
            
            teamDashboardPage.SearchTeam(TeamInfo.TeamName);
            Assert.IsTrue(teamDashboardPage.IsTeamDisplayed(TeamInfo.TeamName), "Team is not displayed");

            Log.Info("Logout and Login as a CA user");
            teamDashboardPage.NavigateToPage(Company.Id);
            header.LogOut();
            login.LoginToApplication(User.Username, User.Password);

            Log.Info($"Search for the {teamType} and delete leader or team member and verify on tab");
            teamDashboardPage.SearchTeam(TeamInfo.TeamName);
            teamDashboardPage.ClickOnTeamName(TeamInfo.TeamName);
            Driver.SwitchTo().DefaultContent();

            leadersTabPage.ClickOnLeadersTab();
            teamDashboardPage.SwitchToIframeForNewNav();
            leadersTabPage.ClickOnDeleteButton(teamMemberInfo.Email);
            Assert.IsFalse(leadersTabPage.IsTeamMemberDisplayed(teamMemberInfo.Email), "leader or team member is displayed");

            Log.Info("Logout and Login as a leader or team member");
            teamDashboardPage.NavigateToPage(Company.Id);
            header.LogOut();
            login.LoginToApplication(teamMemberInfo.Email, SharedConstants.CommonPassword);

            Log.Info($"Verify that {teamType} should not be displayed on Team Dashboard tab");
            SwitchToNewNav();
            teamDashboardPage.NavigateToPage(Company.Id);
            
            teamDashboardPage.SearchTeam(TeamInfo.TeamName);
            Assert.IsFalse(teamDashboardPage.IsTeamDisplayed(TeamInfo.TeamName), "Team is still displayed");
        }

        public void Verify_Team_AddTeamMembersLeadersStepper_AddFromDirectory_AddMemberLeader_Successfully(TeamType teamType, TeamResponse teamResponse)
        {
            var login = new LoginPage(Driver, Log);
            var teamDashboardPage = new TeamDashboardPage(Driver, Log);
            var createTeamStepperPage = new CreateTeamStepperPage(Driver, Log);
            var addTeamSubTeamPage = new AddTeamSubTeamStepperPage(Driver, Log);
            var addLeadersTeamMembersStepperPage = new AddTeamMembersStepperPage(Driver, Log);

            TeamInfo = teamType switch
            {
                TeamType.MultiTeam => MultiTeamsFactory.GetValidMultiTeamInfo(),
                TeamType.PortfolioTeam => PortfolioTeamsFactory.GetValidPortfolioTeamInfo(),
                TeamType.EnterpriseTeam => EnterpriseTeamsFactory.GetValidEnterpriseTeamInfo(),
                _ => TeamInfo
            };

            Log.Info("Login to the application and switch to new navigation");
            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(User.Username, User.Password);
            SwitchToNewNav();

            Log.Info($"Click on 'Add a New Team' button and select '{teamType}' team type to create a team");
            teamDashboardPage.SwitchToGridView();
            teamDashboardPage.ClickOnAddNewTeamButtonAndSelectTeamType(teamType);

            Log.Info("Enter team info and continue till 'Leaders' or 'Team Members' stepper");
            createTeamStepperPage.EnterTeamInfo(TeamInfo);
            createTeamStepperPage.ClickOnContinueToSubTeamOrTeamMemberButton();
            if (TeamType.MultiTeam == teamType | TeamType.PortfolioTeam == teamType | TeamType.EnterpriseTeam == teamType) { addTeamSubTeamPage.ClickOnContinueToAddLeadersButton(); }

            Log.Info("Open directory pop up from add leader dropdown and add team's members from directory and verify the leaders on 'Leaders' stepper");
            addLeadersTeamMembersStepperPage.OpenSelectTeamMembersOrLeadersFromDirectoryPopup();
            var expectedMemberList = teamResponse.Members.Select(a => a.Email).ToList();
            addLeadersTeamMembersStepperPage.SelectTeamMemberFromDictionaryPopupBase.AddMembersFromDirectory(expectedMemberList);

            foreach (var expectedMember in expectedMemberList)
            {
                Assert.IsTrue(addLeadersTeamMembersStepperPage.IsTeamMemberDisplayed(expectedMember), $"Leader or Team Member: {expectedMember} is not added");
            }
        }

        public void Verify_Team_AddTeamMembersLeaders_AddFromDirectory_AddTeam_Successfully(TeamType teamType, TeamResponse teamResponse)
        {
            var login = new LoginPage(Driver, Log);
            var teamDashboardPage = new TeamDashboardPage(Driver, Log);
            var createMultiTeamStepperPage = new CreateTeamStepperPage(Driver, Log);
            var addMultiTeamSubTeamPage = new AddTeamSubTeamStepperPage(Driver, Log);
            var createLeadersStepperPage = new AddTeamMembersStepperPage(Driver, Log);

            TeamInfo = teamType switch
            {
                TeamType.MultiTeam => MultiTeamsFactory.GetValidMultiTeamInfo(),
                TeamType.PortfolioTeam => PortfolioTeamsFactory.GetValidPortfolioTeamInfo(),
                TeamType.EnterpriseTeam => EnterpriseTeamsFactory.GetValidEnterpriseTeamInfo(),
                _ => TeamInfo
            };

            Log.Info("Login to the application and switch to new navigation");
            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(User.Username, User.Password);
            SwitchToNewNav();

            Log.Info($"Click on 'Add a New Team' button and select '{teamType}' team type to create a team");
            teamDashboardPage.SwitchToGridView();
            teamDashboardPage.ClickOnAddNewTeamButtonAndSelectTeamType(teamType);

            Log.Info("Enter team info and continue till 'Leaders' or 'Team Members' stepper");
            createMultiTeamStepperPage.EnterTeamInfo(TeamInfo);
            createMultiTeamStepperPage.ClickOnContinueToSubTeamOrTeamMemberButton();
            if (TeamType.MultiTeam == teamType | TeamType.PortfolioTeam == teamType | TeamType.EnterpriseTeam == teamType) { addMultiTeamSubTeamPage.ClickOnContinueToAddLeadersButton(); }

            Log.Info("Open directory pop up from 'add leader' or 'add team member' dropdown and add team from directory and verify the members");
            createLeadersStepperPage.OpenSelectTeamMembersOrLeadersFromDirectoryPopup();
            createLeadersStepperPage.SelectTeamMemberFromDictionaryPopupBase.AddTeamsFromDirectory(new List<string> { teamResponse.Name });

            var expectedMemberList = teamResponse.Members.Select(a => a.Email).ToList();
            foreach (var expectedMember in expectedMemberList)
            {
                Assert.IsTrue(createLeadersStepperPage.IsTeamMemberDisplayed(expectedMember), $"Team's member : {expectedMember} is not added");
            }
        }

        public void Verify_PortfolioMultiTeam_AddSubTeamsSteppers_AddSubTeams(TeamType teamType)
        {
            var login = new LoginPage(Driver, Log);
            var teamDashboardPage = new TeamDashboardPage(Driver, Log);
            var createMultiTeamStepperPage = new CreateTeamStepperPage(Driver, Log);
            var addSubTeamsStepperPage = new AddTeamSubTeamStepperPage(Driver, Log);

            var teamInfo = MultiTeamsFactory.GetValidMultiTeamInfo();
            var sameLevelOfTeam = SharedConstants.MultiTeam;
            var subTeam = SharedConstants.Team;
            if (TeamType.PortfolioTeam == teamType)
            {
                teamInfo = PortfolioTeamsFactory.GetValidPortfolioTeamInfo();
                sameLevelOfTeam = SharedConstants.PortfolioTeam;
                subTeam = SharedConstants.MultiTeam;
            }
            if (TeamType.EnterpriseTeam == teamType)
            {
                teamInfo = EnterpriseTeamsFactory.GetValidEnterpriseTeamInfo();
                sameLevelOfTeam = SharedConstants.EnterpriseTeam;
                subTeam = SharedConstants.PortfolioTeam;
            }

            Log.Info("Login to the application and switch to new navigation");
            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(User.Username, User.Password);
            SwitchToNewNav();

            Log.Info($"Click on 'Add a New Team' button and select team type to create a '{teamType}'");
            teamDashboardPage.SwitchToGridView();
            teamDashboardPage.ClickOnAddNewTeamButtonAndSelectTeamType(teamType);

            Log.Info("Enter team info and click on 'Continue To Add Sub Teams' button");
            createMultiTeamStepperPage.EnterTeamInfo(teamInfo);
            createMultiTeamStepperPage.ClickOnContinueToSubTeamOrTeamMemberButton();
            Assert.AreEqual(SubTeamStepperTitle, addSubTeamsStepperPage.GetSubTeamStepperTitle(), "'Add Sub-Teams' stepper header title is not matched");
            Assert.AreEqual(SubTeamStepperInfo, addSubTeamsStepperPage.GetSubTeamStepperInfo(), "'Add Sub-Teams' stepper info text is not matched");

            Log.Info("Search the same team level and verify it which is not displayed if not Enteprise level");
            addSubTeamsStepperPage.SearchSubTeams(sameLevelOfTeam);
            if (TeamType.EnterpriseTeam == teamType)
            {
                Assert.IsTrue(addSubTeamsStepperPage.IsSubTeamDisplayedOnUnassignedTeamsList(sameLevelOfTeam), $"'{sameLevelOfTeam}' is not displayed on 'UnAssign Team' section");

                Log.Info("Assign the searched enterprise sub team name and verify");
                addSubTeamsStepperPage.AssignSubTeam(sameLevelOfTeam);
                Assert.IsFalse(addSubTeamsStepperPage.IsSubTeamDisplayedOnUnassignedTeamsList(SharedConstants.EnterpriseTeam), "Enterprise sub team is displayed on 'Team Name' table");
                Assert.IsTrue(addSubTeamsStepperPage.IsSubTeamsDisplayedOnAssignedTeamsList(SharedConstants.EnterpriseTeam), "Enterprise sub Team is not displayed on 'Seleted Team Names' table");

                Log.Info("Unassign enterprise 'Sub teams' then verify that unassigned enterprise 'Sub teams'");
                addSubTeamsStepperPage.UnAssignSubTeam(SharedConstants.EnterpriseTeam);
                Assert.IsFalse(addSubTeamsStepperPage.IsSubTeamsDisplayedOnAssignedTeamsList(SharedConstants.EnterpriseTeam), "Enterprise sub Team is displayed on 'Seleted Team Names' table");
                Assert.IsTrue(addSubTeamsStepperPage.IsSubTeamDisplayedOnUnassignedTeamsList(SharedConstants.EnterpriseTeam), "Enterprise sub team is not displayed on 'Team Name' table");
            }
            else
            {
                Assert.IsFalse(addSubTeamsStepperPage.IsSubTeamDisplayedOnUnassignedTeamsList(sameLevelOfTeam), $"'{sameLevelOfTeam}' is displayed on 'UnAssign Team' section");
            }

            Log.Info("Clear searched multi team and search team also verify");
            addSubTeamsStepperPage.RemoveSearchedText();
            addSubTeamsStepperPage.SearchSubTeams(subTeam);
            Assert.IsTrue(addSubTeamsStepperPage.IsSubTeamDisplayedOnUnassignedTeamsList(subTeam), "Sub team is not displayed on 'UnAssign Team' section");

            Log.Info("Assign the searched sub team name and verify");
            addSubTeamsStepperPage.AssignSubTeam(subTeam);
            Assert.IsFalse(addSubTeamsStepperPage.IsSubTeamDisplayedOnUnassignedTeamsList(subTeam), "Sub team is displayed on 'UnAssign Team' section");
            Assert.IsTrue(addSubTeamsStepperPage.IsSubTeamsDisplayedOnAssignedTeamsList(subTeam), "Sub Team is not displayed on 'Assign Team' section");

            Log.Info("UnAssign 'Sub teams' then verify that unassigned 'Sub teams'");
            addSubTeamsStepperPage.UnAssignSubTeam(subTeam);
            Assert.IsFalse(addSubTeamsStepperPage.IsSubTeamsDisplayedOnAssignedTeamsList(subTeam), "Sub Team is displayed on 'Assign Team' section");
            Assert.IsTrue(addSubTeamsStepperPage.IsSubTeamDisplayedOnUnassignedTeamsList(subTeam), "Sub team is not displayed on 'UnAssign Team' section");
        }

        public void Verify_Team_ReviewStepper_Edit_TeamProfile(TeamType teamType)
        {
            var login = new LoginPage(Driver, Log);
            var teamDashboardPage = new TeamDashboardPage(Driver, Log);
            var createTeamStepperPage = new CreateTeamStepperPage(Driver, Log);
            var addSubTeamsStepperPage = new AddTeamSubTeamStepperPage(Driver, Log);
            var createTeamMembersStepperPage = new AddTeamMembersStepperPage(Driver, Log);
            var addStakeholderStepperPage = new AddStakeholdersStepperPage(Driver, Log);
            var reviewStepperPage = new ReviewStepperPage(Driver, Log);

            var teamInfo = TeamsFactory.GetValidFullWizardTeamInfo();
            var editTeamInfo = TeamsFactory.GetValidUpdatedFullWizardTeamInfo();
            switch (teamType)
            {
                case TeamType.MultiTeam:
                    teamInfo = MultiTeamsFactory.GetValidMultiTeamInfo();
                    editTeamInfo = MultiTeamsFactory.GetValidUpdatedMultiTeamInfo();
                    break;
                case TeamType.PortfolioTeam:
                    teamInfo = PortfolioTeamsFactory.GetValidPortfolioTeamInfo();
                    editTeamInfo = PortfolioTeamsFactory.GetValidUpdatedPortfolioTeamInfo();
                    break;
                case TeamType.QuickLaunchTeam:
                    break;
                case TeamType.FullWizardTeam:
                    break;
                case TeamType.EnterpriseTeam:
                    teamInfo = EnterpriseTeamsFactory.GetValidEnterpriseTeamInfo();
                    editTeamInfo = EnterpriseTeamsFactory.GetValidUpdatedEnterpriseTeamInfo();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(teamType), teamType, null);
            }

            Log.Info("Login to the application and switch to new navigation");
            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(User.Username, User.Password);
            SwitchToNewNav();

            Log.Info($"Click on 'Add a New Team' button and select '{teamType}' team type to create a team");
            teamDashboardPage.SwitchToGridView();
            teamDashboardPage.ClickOnAddNewTeamButtonAndSelectTeamType(teamType);

            Log.Info("Enter team info and continue till 'Review' stepper");
            createTeamStepperPage.EnterTeamInfo(teamInfo);
            createTeamStepperPage.ClickOnContinueToSubTeamOrTeamMemberButton();
            if (TeamType.MultiTeam == teamType | TeamType.PortfolioTeam == teamType | TeamType.EnterpriseTeam == teamType) { addSubTeamsStepperPage.ClickOnContinueToAddLeadersButton(); }
            else { createTeamMembersStepperPage.ClickOnContinueToStakeholderButton(); }
            addStakeholderStepperPage.ClickOnContinueToReviewButton();

            Log.Info("Get expected and actual 'Team Profile' text and validate");
            var expectedTeamProfileText = reviewStepperPage.GetExpectedTeamProfileText(teamInfo).ReplaceStringData();
            switch (teamType)
            {
                case TeamType.PortfolioTeam:
                    expectedTeamProfileText = reviewStepperPage.GetExpectedPortfolioTeamProfileText(teamInfo).ReplaceStringData();
                    break;
                case TeamType.MultiTeam:
                    expectedTeamProfileText = reviewStepperPage.GetExpectedMultiTeamProfileText(teamInfo).ReplaceStringData();
                    break;
                case TeamType.QuickLaunchTeam:
                    break;
                case TeamType.FullWizardTeam:
                    break;
                case TeamType.EnterpriseTeam:
                    expectedTeamProfileText = reviewStepperPage.GetExpectedEnterpriseTeamProfileText(teamInfo).ReplaceStringData();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(teamType), teamType, null);
            }

            var actualMultiTeamProfileText = reviewStepperPage.GetTeamProfileText().ReplaceStringData();
            Assert.AreEqual(expectedTeamProfileText, actualMultiTeamProfileText, "Team profile text doesn't match");

            Log.Info("Click on team profile 'Edit' button");
            reviewStepperPage.ClickOnTeamProfileEditButton();

            Log.Info("Edit team info and click on 'continue' button till 'Review' stepper");
            Driver.SwitchTo().DefaultContent();
            createTeamStepperPage.EnterTeamInfo(editTeamInfo);
            createTeamStepperPage.ClickOnContinueToSubTeamOrTeamMemberButton();
            if (TeamType.MultiTeam == teamType | TeamType.PortfolioTeam == teamType | TeamType.EnterpriseTeam == teamType) { addSubTeamsStepperPage.ClickOnContinueToAddLeadersButton(); }
            else { createTeamMembersStepperPage.ClickOnContinueToStakeholderButton(); }
            addStakeholderStepperPage.ClickOnContinueToReviewButton();


            Log.Info("Get expected and actual edited 'Team Profile' text");
            var expectedEditedTeamProfileText = reviewStepperPage.GetExpectedTeamProfileText(editTeamInfo).ReplaceStringData();
            switch (teamType)
            {
                case TeamType.PortfolioTeam:
                    expectedEditedTeamProfileText = reviewStepperPage.GetExpectedPortfolioTeamProfileText(editTeamInfo).ReplaceStringData();
                    break;
                case TeamType.MultiTeam:
                    expectedEditedTeamProfileText = reviewStepperPage.GetExpectedMultiTeamProfileText(editTeamInfo).ReplaceStringData();
                    break;
                case TeamType.QuickLaunchTeam:
                    break;
                case TeamType.FullWizardTeam:
                    break;
                case TeamType.EnterpriseTeam:
                    expectedEditedTeamProfileText = reviewStepperPage.GetExpectedEnterpriseTeamProfileText(editTeamInfo).ReplaceStringData();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(teamType), teamType, null);
            }

            var actualEditedTeamProfileText = reviewStepperPage.GetTeamProfileText().ReplaceStringData();
            Assert.AreEqual(expectedEditedTeamProfileText, actualEditedTeamProfileText, "Team profile text doesn't match");
            Assert.AreNotEqual(expectedTeamProfileText, actualEditedTeamProfileText, "Team profile text is matched with old data");
        }

        public void Verify_Team_ReviewStepper_Edit_TeamTags(TeamType teamType)
        {
            var login = new LoginPage(Driver, Log);
            var teamDashboardPage = new TeamDashboardPage(Driver, Log);
            var createTeamStepperPage = new CreateTeamStepperPage(Driver, Log);
            var addSubTeamsStepperPage = new AddTeamSubTeamStepperPage(Driver, Log);
            var createTeamMembersStepperPage = new AddTeamMembersStepperPage(Driver, Log);
            var addStakeholderStepperPage = new AddStakeholdersStepperPage(Driver, Log);
            var reviewTeamStepperPage = new ReviewStepperPage(Driver, Log);

            var teamInfo = TeamsFactory.GetValidFullWizardTeamInfo();
            var editTeamInfo = TeamsFactory.GetValidUpdatedFullWizardTeamInfo();
            switch (teamType)
            {
                case TeamType.MultiTeam:
                    teamInfo = MultiTeamsFactory.GetValidMultiTeamInfo();
                    editTeamInfo = MultiTeamsFactory.GetValidUpdatedMultiTeamInfo();
                    break;
                case TeamType.PortfolioTeam:
                    teamInfo = PortfolioTeamsFactory.GetValidPortfolioTeamInfo();
                    editTeamInfo = PortfolioTeamsFactory.GetValidUpdatedPortfolioTeamInfo();
                    break;
                case TeamType.QuickLaunchTeam:
                    break;
                case TeamType.FullWizardTeam:
                    break;
                case TeamType.EnterpriseTeam:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(teamType), teamType, null);
            }

            Log.Info("Login to the application and switch to new navigation");
            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(User.Username, User.Password);
            SwitchToNewNav();

            Log.Info($"Click on 'Add a New Team' button and select '{teamType}' team type to create a team");
            teamDashboardPage.SwitchToGridView();
            teamDashboardPage.ClickOnAddNewTeamButtonAndSelectTeamType(teamType);

            Log.Info("Enter team info and continue till 'Review' stepper and verify");
            createTeamStepperPage.EnterTeamInfo(teamInfo);
            createTeamStepperPage.ClickOnContinueToSubTeamOrTeamMemberButton();
            if (TeamType.MultiTeam == teamType | TeamType.PortfolioTeam == teamType) { addSubTeamsStepperPage.ClickOnContinueToAddLeadersButton(); }
            else { createTeamMembersStepperPage.ClickOnContinueToStakeholderButton(); }
            addStakeholderStepperPage.ClickOnContinueToReviewButton();

            var actualTeamTagsText = reviewTeamStepperPage.GetTeamTagsText();
            CollectionAssert.AreEquivalent(teamInfo.Tags, actualTeamTagsText, "Tags are not matching");

            Log.Info("Click on team tag 'Edit' button");
            reviewTeamStepperPage.ClickOnTeamTagsEditButton();
            createTeamStepperPage.ClickOnSelectedTagsRemoveIcon(teamInfo.Tags[0].Key, teamInfo.Tags[0].Value);
            createTeamStepperPage.SelectTeamTags(editTeamInfo);

            Log.Info("Click on 'continue' button till 'Review' stepper and validate the Team tag info on 'Review' stepper");
            createTeamStepperPage.ClickOnContinueToSubTeamOrTeamMemberButton();
            if (TeamType.MultiTeam == teamType | TeamType.PortfolioTeam == teamType) { addSubTeamsStepperPage.ClickOnContinueToAddLeadersButton(); }
            else { createTeamMembersStepperPage.ClickOnContinueToStakeholderButton(); }
            addStakeholderStepperPage.ClickOnContinueToReviewButton();
            var actualEditedTeamTagsText = reviewTeamStepperPage.GetTeamTagsText();
            CollectionAssert.AreEquivalent(editTeamInfo.Tags, actualEditedTeamTagsText, "Tags are not matching");
        }

        public void Verify_Team_ReviewStepper_Edit_Leaders(TeamType teamType)
        {
            var login = new LoginPage(Driver, Log);
            var teamDashboardPage = new TeamDashboardPage(Driver, Log);
            var createTeamStepperPage = new CreateTeamStepperPage(Driver, Log);
            var addSubTeamsStepperPage = new AddTeamSubTeamStepperPage(Driver, Log);
            var createTeamMembersStepperPage = new AddTeamMembersStepperPage(Driver, Log);
            var addStakeholderStepperPage = new AddStakeholdersStepperPage(Driver, Log);
            var reviewStepperPage = new ReviewStepperPage(Driver, Log);

            TeamInfo = teamType switch
            {
                TeamType.MultiTeam => MultiTeamsFactory.GetValidMultiTeamInfo(),
                TeamType.PortfolioTeam => PortfolioTeamsFactory.GetValidPortfolioTeamInfo(),
                TeamType.EnterpriseTeam => EnterpriseTeamsFactory.GetValidEnterpriseTeamInfo(),
                _ => TeamInfo
            };

            var leadersInfo = TeamsFactory.GetTeamMemberInfo();
            var editLeaderInfo = TeamsFactory.GetEditTeamMemberInfo();

            Log.Info("Login to the application and switch to new navigation");
            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(User.Username, User.Password);
            SwitchToNewNav();

            Log.Info($"Click on 'Add a New Team' button and select '{teamType}' team type to create a team");
            teamDashboardPage.SwitchToGridView();
            teamDashboardPage.ClickOnAddNewTeamButtonAndSelectTeamType(teamType);

            Log.Info("Enter team info and continue till 'Review' stepper and verify");
            createTeamStepperPage.EnterTeamInfo(TeamInfo);
            createTeamStepperPage.ClickOnContinueToSubTeamOrTeamMemberButton();
            if (TeamType.MultiTeam == teamType | TeamType.PortfolioTeam == teamType | TeamType.EnterpriseTeam == teamType) { addSubTeamsStepperPage.ClickOnContinueToAddLeadersButton(); }

            Log.Info("'Click on 'Continue To Add Leaders' or 'Continue To Add Team Members' button and create new leader or team member");
            createTeamMembersStepperPage.OpenAddTeamMembersOrLeadersPopup();
            createTeamMembersStepperPage.PopupBase.EnterTeamMemberOrLeadersInfo(leadersInfo);
            createTeamMembersStepperPage.PopupBase.ClickOnCreateAndCloseButton();

            if (TeamType.FullWizardTeam == teamType)
            {
                createTeamMembersStepperPage.ClickOnContinueToStakeholderButton();
            }

            Log.Info("Continue to review stepper and verify leader or team member info");
            addStakeholderStepperPage.ClickOnContinueToReviewButton();
            var memberInfo = reviewStepperPage.GetTeamMemberOrStakeholderOrLeadersInfoFromGrid(leadersInfo.Email);
            Assert.AreEqual(leadersInfo.FirstName, memberInfo.FirstName, "First name doesn't match");
            Assert.AreEqual(leadersInfo.LastName, memberInfo.LastName, "Last name doesn't match");
            Assert.AreEqual(leadersInfo.Email, memberInfo.Email, "Email doesn't match");
            if (TeamType.FullWizardTeam == teamType)
            {
                foreach (var role in leadersInfo.Role)
                {
                    Assert.IsTrue(memberInfo.Role.Contains(role), "Role is not present");
                }
                foreach (var tag in leadersInfo.ParticipantGroup)
                {
                    Assert.IsTrue(memberInfo.ParticipantGroup.Contains(tag), "Tag is not present");
                }
            }

            Log.Info("Click on 'Edit' button on 'Leaders' or 'Team Member' section and update the leader or team member info also verify on grid");
            if (TeamType.MultiTeam == teamType | TeamType.PortfolioTeam == teamType | TeamType.EnterpriseTeam == teamType) { reviewStepperPage.ClickOnLeadersEditButton(); }
            else if (TeamType.FullWizardTeam == teamType)
            {
                reviewStepperPage.ClickOnTeamMembersEditButton();
            }

            addStakeholderStepperPage.ClickOnStakeholderEditButton(leadersInfo.Email);
            addStakeholderStepperPage.PopupBase.EnterTeamMemberOrLeadersInfo(editLeaderInfo);
            addStakeholderStepperPage.PopupBase.ClickOnFirstName();
            addStakeholderStepperPage.PopupBase.ClickOnUpdateButton();

            if (TeamType.FullWizardTeam == teamType)
            {
                createTeamMembersStepperPage.ClickOnContinueToStakeholderButton();
            }

            Log.Info("Click on 'Continue To Review' button and verify updated leader or team member info");
            addStakeholderStepperPage.ClickOnContinueToReviewButton();
            var actualUpdatedLeader = reviewStepperPage.GetTeamMemberOrStakeholderOrLeadersInfoFromGrid(editLeaderInfo.Email.ToLower());
            Assert.AreEqual(editLeaderInfo.FirstName, actualUpdatedLeader.FirstName, "First name doesn't match");
            Assert.AreEqual(editLeaderInfo.LastName, actualUpdatedLeader.LastName, "Last name doesn't match");
            Assert.AreEqual(editLeaderInfo.Email.ToLower(), actualUpdatedLeader.Email, "Email doesn't match");
        }

        public void Verify_Team_ReviewStepper_Edit_SubTeams(TeamType teamType)
        {
            var login = new LoginPage(Driver, Log);
            var teamDashboardPage = new TeamDashboardPage(Driver, Log);
            var createMultiTeamStepperPage = new CreateTeamStepperPage(Driver, Log);
            var addSubTeamsStepperPage = new AddTeamSubTeamStepperPage(Driver, Log);
            var addLeadersStepperPage = new AddStakeholdersStepperPage(Driver, Log);
            var reviewMultiTeamStepperPage = new ReviewStepperPage(Driver, Log);

            Log.Info("Login to the application and switch to new navigation");
            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(User.Username, User.Password);
            SwitchToNewNav();

            var teamInfo = MultiTeamsFactory.GetValidMultiTeamInfo();
            var subTeam = SharedConstants.Team;
            switch (teamType)
            {
                case TeamType.PortfolioTeam:
                    teamInfo = PortfolioTeamsFactory.GetValidPortfolioTeamInfo();
                    subTeam = SharedConstants.MultiTeam;
                    break;
                case TeamType.EnterpriseTeam:
                    teamInfo = EnterpriseTeamsFactory.GetValidEnterpriseTeamInfo();
                    subTeam = SharedConstants.PortfolioTeam;
                    break;
                case TeamType.QuickLaunchTeam:
                    break;
                case TeamType.FullWizardTeam:
                    break;
                case TeamType.MultiTeam:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(teamType), teamType, null);
            }

            Log.Info($"Click on 'Add a New Team' button and select team type to create a '{teamType}'");
            teamDashboardPage.SwitchToGridView();
            teamDashboardPage.ClickOnAddNewTeamButtonAndSelectTeamType(teamType);

            Log.Info("Enter team info and click on 'Continue To Add Sub Teams' button");
            createMultiTeamStepperPage.EnterTeamInfo(teamInfo);
            createMultiTeamStepperPage.ClickOnContinueToSubTeamOrTeamMemberButton();

            //Add Sub-Teams
            Log.Info("Assign sub team and verify");
            addSubTeamsStepperPage.AssignSubTeam(subTeam);

            Log.Info("Continue to review stepper and verify sub team");
            addSubTeamsStepperPage.ClickOnContinueToAddLeadersButton();
            addLeadersStepperPage.ClickOnContinueToReviewButton();
            Assert.IsTrue(reviewMultiTeamStepperPage.GetSubTeamsTextList().Contains(subTeam), "Failure !! Sub-team: " + subTeam + " does not display in Finish and Review page");

            Log.Info("Click on 'Edit' button on Sub team section and unAssign the sub team and verify");
            reviewMultiTeamStepperPage.ClickOnTeamSubTeamsEditButton();
            addSubTeamsStepperPage.UnAssignSubTeam(subTeam);
            Assert.IsFalse(addSubTeamsStepperPage.IsSubTeamsDisplayedOnAssignedTeamsList(subTeam), "Sub Team is displayed on 'Selected Team Names' table");
            Assert.IsTrue(addSubTeamsStepperPage.IsSubTeamDisplayedOnUnassignedTeamsList(subTeam), "Sub team is not displayed on 'Team Name' table");

            Log.Info("Continue to review stepper and verify sub team");
            addSubTeamsStepperPage.ClickOnContinueToAddLeadersButton();
            addLeadersStepperPage.ClickOnContinueToReviewButton();
            Assert.IsFalse(reviewMultiTeamStepperPage.GetSubTeamsTextList().Contains(subTeam), "Failure !! Sub-team: " + subTeam + " does not display in Finish and Review page");
        }

        public void Verify_Team_Success_Message_On_Every_Stepper(TeamType teamType)
        {
            var login = new LoginPage(Driver, Log);
            var teamDashboardPage = new TeamDashboardPage(Driver, Log);
            var createTeamStepperPage = new CreateTeamStepperPage(Driver, Log);
            var addSubTeamsStepperPage = new AddTeamSubTeamStepperPage(Driver, Log);
            var createTeamMembersStepperPage = new AddTeamMembersStepperPage(Driver, Log);
            var addStakeholderStepperPage = new AddStakeholdersStepperPage(Driver, Log);

            TeamInfo = teamType switch
            {
                TeamType.MultiTeam => MultiTeamsFactory.GetValidMultiTeamInfo(),
                TeamType.PortfolioTeam => PortfolioTeamsFactory.GetValidPortfolioTeamInfo(),
                TeamType.EnterpriseTeam => EnterpriseTeamsFactory.GetValidEnterpriseTeamInfo(),
                _ => TeamInfo
            };

            Log.Info("Login to the application and switch to new navigation");
            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(User.Username, User.Password);
            SwitchToNewNav();

            Log.Info($"Click on 'Add a New Team' button and select '{teamType}' team type to create a team");
            teamDashboardPage.SwitchToGridView();
            teamDashboardPage.ClickOnAddNewTeamButtonAndSelectTeamType(teamType);

            Log.Info("Enter team info and continue till 'Review' stepper and verify");
            createTeamStepperPage.EnterTeamInfo(TeamInfo);
            createTeamStepperPage.ClickOnContinueToSubTeamOrTeamMemberButton();
            Assert.AreEqual("Team created successfully", createTeamMembersStepperPage.GetSuccessMessage(), "Success Message does not matched");

            if (TeamType.MultiTeam == teamType | TeamType.PortfolioTeam == teamType | TeamType.EnterpriseTeam == teamType) { addSubTeamsStepperPage.ClickOnContinueToAddLeadersButton(); }

            if (TeamType.FullWizardTeam == teamType)
            {
                createTeamMembersStepperPage.ClickOnContinueToStakeholderButton();
                Assert.AreEqual("Changes saved", createTeamMembersStepperPage.GetSuccessMessage(), "Success Message does not matched");
            }

            Log.Info("Continue to review stepper and verify leader or team member info");
            addStakeholderStepperPage.ClickOnContinueToReviewButton();
            Assert.AreEqual("Changes saved", createTeamMembersStepperPage.GetSuccessMessage(), "Success Message does not matched");
        }

    }
}