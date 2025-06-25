using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.DataObjects;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Common.Create;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Common.Edit;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.MultiTeam.Edit;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Teams.MultiTeam
{
    [TestClass]
    [TestCategory("Teams"), TestCategory("MultiTeam")]
    public class EditMultiTeamTests : BaseTest
    {
        private static bool _classInitFailed;
        private static AddTeamWithMemberRequest _multiTeam;
        private static AddTeamWithMemberRequest _team;
        private static int _multiTeamId;

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            try
            {
                _team = TeamFactory.GetNormalTeam("SubTeam");
                _multiTeam = TeamFactory.GetMultiTeam("EditMT");

                var setup = new SetupTeardownApi(TestEnvironment);
                var teamResponse = setup.CreateTeam(_team).GetAwaiter().GetResult();
                var multiTeamResponse = setup.CreateTeam(_multiTeam).GetAwaiter().GetResult();
                setup.AddSubteams(multiTeamResponse.Uid, new List<Guid> { teamResponse.Uid }).GetAwaiter().GetResult(); 
                _multiTeamId = new SetupTeardownApi(TestEnvironment).GetCompanyHierarchy(Company.Id).GetTeamByName(multiTeamResponse.Name).TeamId;
            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }

        
        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("TeamAdmin2"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public void MultiTeam_EditProfile()
        {
            VerifySetup(_classInitFailed);
            
            var login = new LoginPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var editTeamBasePage = new EditTeamBasePage(Driver, Log);
            var editMtProfilePage = new EditMtProfilePage(Driver, Log);

            Driver.NavigateToPage(ApplicationUrl);

            login.LoginToApplication(User.Username, User.Password);

            dashBoardPage.GridTeamView();
            dashBoardPage.SearchTeam(_multiTeam.Name);

            var teamId = dashBoardPage.GetTeamIdFromLink(_multiTeam.Name);
            dashBoardPage.ClickTeamEditButton(_multiTeam.Name);

            editTeamBasePage.GoToTeamProfileTab();

            string today = DateTime.Now.AddMonths(1).ToString("MMMM yyyy", CultureInfo.InvariantCulture);

            MultiTeamInfo multiTeamInfo = new MultiTeamInfo()
            {
                TeamName = "MultiTeamEdited" + RandomDataUtil.GetTeamName(),
                TeamType = "Product Line Team",
                Department = "Test Department Edited",
                DateEstablished = today,
                AgileAdoptionDate = today,
                Description = "Test Description Edited",
                TeamBio = $"{RandomDataUtil.GetTeamBio()} Edited",
                ImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\TestData\\1.jpg")
            };

            editMtProfilePage.EnterMultiTeamInfo(multiTeamInfo);
            _multiTeam.Name = multiTeamInfo.TeamName;
            multiTeamInfo.ImagePath = editMtProfilePage.GetTeamImage();

            editMtProfilePage.ClickUpdateTeamProfileButton();

            editTeamBasePage.GoToDashboard();
            dashBoardPage.GridTeamView();

            editTeamBasePage.NavigateToPage(teamId);

            editTeamBasePage.GoToTeamProfileTab();

            MultiTeamInfo actualMultiTeamInfo = editMtProfilePage.GetMultiTeamInfo();
            Assert.AreEqual(multiTeamInfo.TeamName, actualMultiTeamInfo.TeamName, "Team Name doesn't match");
            Assert.AreEqual(multiTeamInfo.TeamType, actualMultiTeamInfo.TeamType, "Team type doesn't match");
            Assert.AreEqual(multiTeamInfo.ImagePath, actualMultiTeamInfo.ImagePath, "Image Path doesn't match");
            Assert.AreEqual(multiTeamInfo.AgileAdoptionDate, actualMultiTeamInfo.AgileAdoptionDate, "Agile Adoption Date doesn't match");
            Assert.AreEqual(multiTeamInfo.DateEstablished, actualMultiTeamInfo.DateEstablished, "Date Established doesn't match");
            Assert.AreEqual(multiTeamInfo.Department, actualMultiTeamInfo.Department, "Department doesn't match");
            Assert.AreEqual(multiTeamInfo.Description, actualMultiTeamInfo.Description, "Description doesn't match");
            Assert.AreEqual(multiTeamInfo.TeamBio, actualMultiTeamInfo.TeamBio, "Team BIO doesn't match");
        }


        [TestMethod]
        [TestCategory("TeamAdmin2"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public void MultiTeam_EditSubTeams()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var editEtMtSubTeamBasePage = new EditEtMtSubTeamBasePage(Driver, Log);

            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(User.Username, User.Password);

            Log.Info($"Navigate to 'Edit multi team' page and Click on 'Sub-Teams' tab then add sub team - {SharedConstants.Team}");
            editEtMtSubTeamBasePage.NavigateToPage("multiteam",_multiTeamId);
            editEtMtSubTeamBasePage.SelectSubTeamViaSearchBox(SharedConstants.Team);
            editEtMtSubTeamBasePage.ClickUpdateSubTeamButton();
            Assert.That.ListContains(editEtMtSubTeamBasePage.GetSelectedSubTeamList(), SharedConstants.Team, $"List does not contain - {SharedConstants.Team}");
            Assert.AreEqual(2, editEtMtSubTeamBasePage.GetSelectedSubTeamList().Count, "Sub team count doesn't match");

            Log.Info($"Navigate to Team dashboard and verify {_multiTeam.Name} has sub team - {SharedConstants.Team}");
            dashBoardPage.NavigateToPage(Company.Id);
            dashBoardPage.SearchTeam(_multiTeam.Name);
            Assert.AreEqual("2", dashBoardPage.GetCellValue(1, "Number of Sub Teams"), "Sub team count doesn't match");
            var dashboardSubTeamName = dashBoardPage.GetCellValue(1, "Sub Teams");
            Assert.IsTrue(dashboardSubTeamName.Contains(SharedConstants.Team), $"{SharedConstants.Team} - Sub team does not present");

            Log.Info($"Navigate to 'Edit multi team' page and Click on 'Sub-Teams' tab then remove - {SharedConstants.Team}");
            editEtMtSubTeamBasePage.NavigateToPage("multiteam", _multiTeamId);
            editEtMtSubTeamBasePage.RemoveSubTeam(SharedConstants.Team);
            editEtMtSubTeamBasePage.ClickUpdateSubTeamButton();
            Assert.That.ListNotContains(editEtMtSubTeamBasePage.GetSelectedSubTeamList(), SharedConstants.Team, $"List contains - {SharedConstants.Team}");
            Assert.AreEqual(1, editEtMtSubTeamBasePage.GetSelectedSubTeamList().Count, "Sub team count doesn't match");

            Log.Info($"Navigate to Team dashboard and verify {_multiTeam.Name} has no sub team - {SharedConstants.Team}");
            dashBoardPage.NavigateToPage(Company.Id);
            var updatedDashboardSubTeamName = dashBoardPage.GetCellValue(1, "Sub Teams");
            Assert.IsFalse(updatedDashboardSubTeamName.Contains(SharedConstants.Team), $"{SharedConstants.Team} - Sub team present");
            Assert.AreEqual("1", dashBoardPage.GetCellValue(1, "Number of Sub Teams"), "Sub team count doesn't match");
        }


        [TestMethod]
        [TestCategory("TeamAdmin2"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public void MultiTeam_EditGrowthTeamMembers()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var editTeamBasePage = new EditTeamBasePage(Driver, Log);
            var editMtTeamMemberPage = new EditMtTeamMemberPage(Driver, Log);
            var addTeamMemberPage = new AddTeamMemberPage(Driver, Log);

            Driver.NavigateToPage(ApplicationUrl);

            login.LoginToApplication(User.Username, User.Password);

            var teamId = dashBoardPage.GetTeamIdFromLink(_multiTeam.Name);
            editTeamBasePage.NavigateToPage(teamId);

            editTeamBasePage.GoToGrowthTeamTab();

            addTeamMemberPage.ClickAddNewTeamMemberButton();
            
            var teamMemberInfo = new TeamMemberInfo
            {
                FirstName = "Member",
                LastName = "Name",
                Email = "memberemail" + CSharpHelpers.RandomNumber() + "@sharklasers.com",
                Role = "Architect"
            };
            
            addTeamMemberPage.EnterTeamMemberInfo(teamMemberInfo);
            addTeamMemberPage.ClickSaveAndCloseButton();
            
            TeamMemberInfo actualTeamMember = addTeamMemberPage.GetTeamMemberInfoFromGrid(1);
            Assert.AreEqual(teamMemberInfo.FirstName, actualTeamMember.FirstName, "Firstname doesn't match");
            Assert.AreEqual(teamMemberInfo.LastName, actualTeamMember.LastName, "Lastname doesn't match");
            Assert.AreEqual(teamMemberInfo.Email, actualTeamMember.Email, "Email doesn't match");
            Assert.AreEqual(teamMemberInfo.Role, actualTeamMember.Role, "Role doesn't match");

            teamMemberInfo = new TeamMemberInfo
            {
                FirstName = "MemberEdited",
                LastName = $"{SharedConstants.TeamMemberLastName}Edited",
                Email = "memberemailedited" + CSharpHelpers.RandomNumber() + "@sharklasers.com",
                Role = "Sponsor"
            };

            editMtTeamMemberPage.ClickTeamMemberEditButton(1);
            editMtTeamMemberPage.EnterTeamMemberInfo(teamMemberInfo);
            editMtTeamMemberPage.ClickUpdateButton();

            TeamMemberInfo actualTeamMember2 = addTeamMemberPage.GetTeamMemberInfoFromGrid(1);
            Assert.AreEqual(teamMemberInfo.FirstName, actualTeamMember2.FirstName, "Firstname doesn't match");
            Assert.AreEqual(teamMemberInfo.LastName, actualTeamMember2.LastName, "Lastname doesn't match");
            Assert.AreEqual(teamMemberInfo.Email, actualTeamMember2.Email, "Email doesn't match");
            Assert.AreEqual(teamMemberInfo.Role, actualTeamMember2.Role, "Role doesn't match");
        }


        [TestMethod]
        [TestCategory("TeamAdmin2"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public void MultiTeam_EditStakeHolders()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var editTeamBasePage = new EditTeamBasePage(Driver, Log);
            var editMtStakeHolderPage = new EditMtStakeHolderPage(Driver, Log);
            var addStakeHolderPage = new AddStakeHolderPage(Driver, Log);

            Driver.NavigateToPage(ApplicationUrl);

            login.LoginToApplication(User.Username, User.Password);

            var teamId = dashBoardPage.GetTeamIdFromLink(_multiTeam.Name);
            editTeamBasePage.NavigateToPage(teamId);

            editTeamBasePage.GoToStakeHoldersTab();

            addStakeHolderPage.ClickAddNewStakeHolderButton();
            
            var stakeHolderInfo = new StakeHolderInfo
            {
                FirstName = "Stake",
                LastName = SharedConstants.TeamMemberLastName,
                Email = "stakeemail" + CSharpHelpers.RandomNumber() + "@sharklasers.com",
                Role = "Sponsor"
            };
            
            addStakeHolderPage.EnterStakeHolderInfo(stakeHolderInfo);
            addStakeHolderPage.ClickSaveAndCloseButton();

            var actualStakeHolder = addStakeHolderPage.GetStakeHolderInfoFromGrid(1);
            Assert.AreEqual(stakeHolderInfo.FirstName, actualStakeHolder.FirstName, "Firstname doesn't match");
            Assert.AreEqual(stakeHolderInfo.LastName, actualStakeHolder.LastName, "Lastname doesn't match");
            Assert.AreEqual(stakeHolderInfo.Email, actualStakeHolder.Email, "Email doesn't match");
            Assert.AreEqual(stakeHolderInfo.Role, actualStakeHolder.Role, "Role doesn't match");

            stakeHolderInfo = new StakeHolderInfo
            {
                FirstName = "StakeEdited",
                LastName = $"{SharedConstants.TeamMemberLastName}Edited",
                Email = "stakeemailedited" + CSharpHelpers.RandomNumber() + "@sharklasers.com",
                Role = "Manager"
            };

            editMtStakeHolderPage.ClickStakeHolderEditButton(1);
            editMtStakeHolderPage.EnterStakeHolderInfo(stakeHolderInfo);
            editMtStakeHolderPage.ClickUpdateButton();

            StakeHolderInfo actualStakeHolderEdited = addStakeHolderPage.GetStakeHolderInfoFromGrid(1);
            Assert.AreEqual(stakeHolderInfo.FirstName, actualStakeHolderEdited.FirstName, "Firstname doesn't match");
            Assert.AreEqual(stakeHolderInfo.LastName, actualStakeHolderEdited.LastName, "Lastname doesn't match");
            Assert.AreEqual(stakeHolderInfo.Email, actualStakeHolderEdited.Email, "Email doesn't match");
            Assert.AreEqual(stakeHolderInfo.Role, actualStakeHolderEdited.Role, "Role doesn't match");
        }

    }
}
