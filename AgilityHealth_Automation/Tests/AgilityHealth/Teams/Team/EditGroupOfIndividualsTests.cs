using System;
using System.Globalization;
using System.IO;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.DataObjects;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Common.Create;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Common.Edit;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Team.Create;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Team.Edit;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Teams.Team
{
    [TestClass]
    [TestCategory("Teams"), TestCategory("Team")]
    public class EditGroupOfIndividualsTests : BaseTest
    {
        private static bool _classInitFailed;
        private static AddTeamWithMemberRequest _team;

        [ClassInitialize]
        public static void CreateGoiTeam(TestContext _)
        {
            try
            {
                _team = TeamFactory.GetGoiTeam("EditGoiTeam");
                _team.Members.Add(MemberFactory.GetTeamMember());

                var setup = new SetupTeardownApi(TestEnvironment);
                setup.CreateTeam(_team).GetAwaiter().GetResult();
            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }

        }

        [TestMethod]
        [TestCategory("TeamAdmin2"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public void GOI_EditTeamProfile()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var editTeamProfilePage = new EditTeamProfilePage(Driver, Log);

            Driver.NavigateToPage(ApplicationUrl);

            login.LoginToApplication(User.Username, User.Password);

            dashBoardPage.GridTeamView();

            dashBoardPage.SearchTeam(_team.Name);

            dashBoardPage.ClickTeamEditButton(_team.Name);

            var today = DateTime.Now.AddMonths(1).ToString("MMMM yyyy", CultureInfo.InvariantCulture);
            var teamInfo = new TeamInfo()
            {
                TeamName = "Edit_" + RandomDataUtil.GetTeamName(),
                WorkType = "Group Of Individuals",
                PreferredLanguage = "English",
                Methodology = "Waterfall",
                Department = "Test Edited Department",
                DateEstablished = today,
                AgileAdoptionDate = today,
                Description = "Test Edited Description",
                TeamBio = $"{RandomDataUtil.GetTeamBio()} edited",
                ImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\TestData\\2.jpg")
            };

            editTeamProfilePage.EnterTeamInfo(teamInfo);
            teamInfo.ImagePath= editTeamProfilePage.GetTeamImage();

            editTeamProfilePage.ClickUpdateTeamProfileButton();
            _team.Name = teamInfo.TeamName;

            TeamInfo actualTeamInfo = editTeamProfilePage.GetTeamInfo();

            Assert.AreEqual(teamInfo.AgileAdoptionDate, actualTeamInfo.AgileAdoptionDate, "Agile Adoption Date doesn't match");
            Assert.AreEqual(teamInfo.DateEstablished, actualTeamInfo.DateEstablished, "Date Established doesn't match");
            Assert.AreEqual(teamInfo.Department, actualTeamInfo.Department, "Department doesn't match");
            Assert.AreEqual(teamInfo.Description, actualTeamInfo.Description, "Description doesn't match");
            Assert.AreEqual(teamInfo.Methodology, actualTeamInfo.Methodology, "Methodology doesn't match");
            Assert.AreEqual(teamInfo.TeamBio, actualTeamInfo.TeamBio, "Team BIO doesn't match");
            Assert.AreEqual(teamInfo.TeamName, actualTeamInfo.TeamName, "Team Name doesn't match");
            Assert.AreEqual(teamInfo.WorkType, actualTeamInfo.WorkType, "Work Type doesn't match");
            Assert.AreEqual(teamInfo.PreferredLanguage, actualTeamInfo.PreferredLanguage, "Preferred Language doesn't match");
            Assert.AreEqual(teamInfo.ImagePath, actualTeamInfo.ImagePath, "Image Path doesn't match");
        }

        [TestMethod]
        [TestCategory("TeamAdmin2"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public void GOI_EditTeamMembers()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var editTeamBasePage = new EditTeamBasePage(Driver, Log);
            var editTeamTeamMemberPage = new EditTeamTeamMemberPage(Driver, Log);

            Driver.NavigateToPage(ApplicationUrl);

            login.LoginToApplication(User.Username, User.Password);

            dashBoardPage.GridTeamView();

            var teamId = dashBoardPage.GetTeamIdFromLink(_team.Name);
            editTeamBasePage.NavigateToPage(teamId);

            editTeamBasePage.GoToTeamMembersTab();

            TeamMemberInfo teamMemberInfo = new TeamMemberInfo
            {
                FirstName = "MemberEdited",
                LastName = $"{SharedConstants.TeamMemberLastName}Edited",
                Email = "memberemailedtied" + CSharpHelpers.RandomNumber() + "@sharklasers.com",
                Role = "Individual",
                ParticipantGroup = "Support"
            };

            editTeamTeamMemberPage.ClickTeamMemberEditButton(1);
            editTeamTeamMemberPage.EnterTeamMemberInfo(teamMemberInfo,"Edit");
            editTeamTeamMemberPage.ClickUpdateButton();

            TeamMemberInfo actualTeamMember = editTeamTeamMemberPage.GetTeamMemberInfoFromGrid(1);
            Assert.AreEqual(teamMemberInfo.FirstName, actualTeamMember.FirstName, "Firstname doesn't match");
            Assert.AreEqual(teamMemberInfo.LastName, actualTeamMember.LastName, "Lastname doesn't match");
            Assert.AreEqual(teamMemberInfo.Email, actualTeamMember.Email, "Email doesn't match");
            Assert.IsTrue(teamMemberInfo.Role.Contains(teamMemberInfo.Role), "Role doesn't match");
            Assert.IsTrue(teamMemberInfo.ParticipantGroup.Contains(teamMemberInfo.ParticipantGroup), "ParticipantGroup doesn't match");

        }

        [TestMethod]
        [TestCategory("TeamAdmin2"), TestCategory("CompanyAdmin")]
        public void GOI_EditStakeHolders()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var editTeamBasePage = new EditTeamBasePage(Driver, Log);
            var editTeamStakeHolderPage = new EditTeamStakeHolderPage(Driver, Log);
            var addStakeHolderPage = new AddStakeHolderPage(Driver, Log);

            Driver.NavigateToPage(ApplicationUrl);

            login.LoginToApplication(User.Username, User.Password);

            dashBoardPage.GridTeamView();

            var teamId = dashBoardPage.GetTeamIdFromLink(_team.Name);
            editTeamBasePage.NavigateToPage(teamId);

            editTeamBasePage.GoToStakeHoldersTab();

            StakeHolderInfo stakeHolderInfo = new StakeHolderInfo
            {
                FirstName = "Stake",
                LastName = SharedConstants.TeamMemberLastName,
                Email = "s" + CSharpHelpers.RandomNumber() + "@s.com",
                Role = "Sponsor"
            };

            editTeamStakeHolderPage.ClickAddNewStakeHolderButton();
            addStakeHolderPage.EnterStakeHolderInfo(stakeHolderInfo,"Edit");
            addStakeHolderPage.ClickSaveAndCloseButton();

            StakeHolderInfo actualStakeHolder = editTeamStakeHolderPage.GetStakeHolderInfoFromGrid(1);
            Assert.AreEqual(stakeHolderInfo.FirstName, actualStakeHolder.FirstName, "Firstname doesn't match");
            Assert.AreEqual(stakeHolderInfo.LastName, actualStakeHolder.LastName, "Lastname doesn't match");
            Assert.AreEqual(stakeHolderInfo.Email, actualStakeHolder.Email, "Email doesn't match");
            Assert.AreEqual(stakeHolderInfo.Role, actualStakeHolder.Role, "Role doesn't match");

            StakeHolderInfo stakeHolderInfo2 = new StakeHolderInfo
            {
                FirstName = "StakeEdited",
                LastName = $"{SharedConstants.TeamMemberLastName}Edited",
                Email = "stakeemailedited" + CSharpHelpers.RandomNumber() + "@sharklasers.com",
                Role = "Executive"
            };

            editTeamStakeHolderPage.ClickStakeHolderEditButton(1);
            editTeamStakeHolderPage.EnterStakeHolderInfo(stakeHolderInfo2,"Edit");
            editTeamStakeHolderPage.ClickUpdateButton();

            StakeHolderInfo actualStakeHolder2 = editTeamStakeHolderPage.GetStakeHolderInfoFromGrid(1);
            Assert.AreEqual(stakeHolderInfo2.FirstName, actualStakeHolder2.FirstName, "Firstname doesn't match");
            Assert.AreEqual(stakeHolderInfo2.LastName, actualStakeHolder2.LastName, "Lastname doesn't match");
            Assert.AreEqual(stakeHolderInfo2.Email, actualStakeHolder2.Email, "Email doesn't match");
            Assert.AreEqual(stakeHolderInfo2.Role, actualStakeHolder2.Role, "Role doesn't match");

        }
    }
}
