using System;
using System.Collections.Generic;
using System.IO;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.DataObjects;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Common.Create;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Common.Edit;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.EnterpriseTeam;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.MultiTeam.Edit;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Teams.EnterpriseTeam
{
    [TestClass]
    [TestCategory("Teams"), TestCategory("EnterpriseTeam")]
    public class EditEnterpriseTeamTests : BaseTest
    {
        private static bool _classInitFailed;
        private static AddTeamWithMemberRequest _multiTeam;
        private static AddTeamWithMemberRequest _enterpriseTeam1;
        private static AddTeamWithMemberRequest _enterpriseTeam2;
        private static int _enterpriseId;

        [ClassInitialize]
        public static void CreateEnterpriseTeam(TestContext _)
        {
            try
            {
                _multiTeam = TeamFactory.GetMultiTeam("MTSubTeam");
                _enterpriseTeam1 = TeamFactory.GetEnterpriseTeam("EditET");
                _enterpriseTeam2 = TeamFactory.GetEnterpriseTeam("EditET");

                var setup = new SetupTeardownApi(TestEnvironment);
                var multiTeamResponse = setup.CreateTeam(_multiTeam).GetAwaiter().GetResult();
                var enterpriseTeamResponse = setup.CreateTeam(_enterpriseTeam1).GetAwaiter().GetResult();
                setup.CreateTeam(_enterpriseTeam2).GetAwaiter().GetResult();
                setup.AddSubteams(enterpriseTeamResponse.Uid, new List<Guid> { multiTeamResponse.Uid }).GetAwaiter().GetResult();
                _enterpriseId = setup.GetCompanyHierarchy(Company.Id).GetTeamByName(enterpriseTeamResponse.Name).TeamId;
            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public void EnterpriseTeam_EditProfile()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var editTeamBasePage = new EditTeamBasePage(Driver, Log);
            var editEtProfilePage = new EditEtProfilePage(Driver, Log);

            Driver.NavigateToPage(ApplicationUrl);

            login.LoginToApplication(User.Username, User.Password);

            dashBoardPage.GridTeamView();
            dashBoardPage.SearchTeam(_enterpriseTeam1.Name);

            var teamId = dashBoardPage.GetTeamIdFromLink(_enterpriseTeam1.Name);
            dashBoardPage.ClickTeamEditButton(_enterpriseTeam1.Name);

            editTeamBasePage.GoToTeamProfileTab();

            var enterpriseTeamInfo = new EnterpriseTeamInfo()
            {
                TeamName = "EnterpriseTeamEdited" + RandomDataUtil.GetTeamName(),
                TeamType = "Portfolio Team",
                ExternalIdentifier = (User.IsCompanyAdmin()) ? "external identifier" : "",
                Department = "Test Department Edited",
                DateEstablished = DateTime.Now,
                AgileAdoptionDate = DateTime.Now,
                Description = "Test Description Edited",
                TeamBio = $"{RandomDataUtil.GetTeamBio()} Edited",
                ImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\TestData\\1.jpg")
            };

            editEtProfilePage.EnterEnterpriseTeamInfo(enterpriseTeamInfo);
            enterpriseTeamInfo.ImagePath = editEtProfilePage.GetTeamImage();

            editEtProfilePage.ClickUpdateTeamProfileButton();

            editTeamBasePage.GoToDashboard();
            dashBoardPage.GridTeamView();

            editTeamBasePage.NavigateToPage(teamId);

            editTeamBasePage.GoToTeamProfileTab();

            var actualEnterpriseTeamInfo = editEtProfilePage.GetEnterpriseTeamInfo();
            Assert.AreEqual(enterpriseTeamInfo.TeamName, actualEnterpriseTeamInfo.TeamName, "Team Name doesn't match");
            Assert.AreEqual(enterpriseTeamInfo.TeamType, actualEnterpriseTeamInfo.TeamType, "Team type doesn't match");
            Assert.AreEqual(enterpriseTeamInfo.AgileAdoptionDate.ToString("MMMM yyyy"), actualEnterpriseTeamInfo.AgileAdoptionDate.ToString("MMMM yyyy"), "Agile Adoption Date doesn't match");
            Assert.AreEqual(enterpriseTeamInfo.DateEstablished.ToString("MMMM yyyy"), actualEnterpriseTeamInfo.DateEstablished.ToString("MMMM yyyy"), "Date Established doesn't match");
            Assert.AreEqual(enterpriseTeamInfo.Department, actualEnterpriseTeamInfo.Department, "Department doesn't match");
            Assert.AreEqual(User.IsCompanyAdmin() ? enterpriseTeamInfo.ExternalIdentifier : _enterpriseTeam1.ExternalIdentifier, actualEnterpriseTeamInfo.ExternalIdentifier, "External Identifier doesn't match");
            Assert.AreEqual(enterpriseTeamInfo.Description, actualEnterpriseTeamInfo.Description, "Description doesn't match");
            Assert.AreEqual(enterpriseTeamInfo.ImagePath, actualEnterpriseTeamInfo.ImagePath, "Image Path doesn't match");
            Assert.AreEqual(enterpriseTeamInfo.TeamBio, actualEnterpriseTeamInfo.TeamBio, "Team BIO doesn't match");

        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public void EnterpriseTeam_EditSubTeams()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var editEtMtSubTeamBasePage = new EditEtMtSubTeamBasePage(Driver, Log);

            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(User.Username, User.Password);

            Log.Info($"Navigate to 'Edit enterprise team' page and Click on 'Sub-Teams' tab then add sub team - {Constants.MultiTeamForGi}");
            editEtMtSubTeamBasePage.NavigateToPage("enterprise", _enterpriseId);
            editEtMtSubTeamBasePage.SelectSubTeamViaSearchBox(Constants.MultiTeamForGi);
            editEtMtSubTeamBasePage.ClickUpdateSubTeamButton();
            Assert.That.ListContains(editEtMtSubTeamBasePage.GetSelectedSubTeamList(), Constants.MultiTeamForGi, $"List does not contain - {Constants.MultiTeamForGi}");
            Assert.AreEqual(2, editEtMtSubTeamBasePage.GetSelectedSubTeamList().Count, "Sub team count doesn't match");

            Log.Info($"Navigate to Team dashboard and verify {_enterpriseTeam1.Name} has sub team - {Constants.MultiTeamForGi}");
            dashBoardPage.NavigateToPage(Company.Id);
            dashBoardPage.SearchTeam(_enterpriseTeam1.Name);
            Assert.AreEqual("2", dashBoardPage.GetCellValue(1, "Number of Sub Teams"), "Sub team count doesn't match");
            var dashboardMultiTeamName = dashBoardPage.GetCellValue(1, "Multi Teams");
            Assert.IsTrue(dashboardMultiTeamName.Contains(Constants.MultiTeamForGi), $" {Constants.MultiTeamForGi} - Sub team doesn't present");

            Log.Info($"Navigate to 'Edit enterprise team' page and Click on 'Sub-Teams' tab then remove - {Constants.MultiTeamForGi}");
            editEtMtSubTeamBasePage.NavigateToPage("enterprise", _enterpriseId);
            editEtMtSubTeamBasePage.RemoveSubTeam(Constants.MultiTeamForGi);
            editEtMtSubTeamBasePage.ClickUpdateSubTeamButton();
            Assert.That.ListNotContains(editEtMtSubTeamBasePage.GetSelectedSubTeamList(), Constants.MultiTeamForGi, $"List contains - {Constants.MultiTeamForGi}");
            Assert.AreEqual(1, editEtMtSubTeamBasePage.GetSelectedSubTeamList().Count, "Sub team count doesn't match");

            Log.Info($"Navigate to Team dashboard and verify {_enterpriseTeam1.Name} has no sub team - {Constants.MultiTeamForGi}");
            dashBoardPage.NavigateToPage(Company.Id);
            var updatedDashboardMultiTeamName = dashBoardPage.GetCellValue(1, "Multi Teams");
            Assert.IsFalse(updatedDashboardMultiTeamName.Contains(Constants.MultiTeamForGi), $"{Constants.MultiTeamForGi} - Sub team present");
            Assert.AreEqual("1", dashBoardPage.GetCellValue(1, "Number of Sub Teams"), "Sub team count doesn't match");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public void EnterpriseTeam_EditTeamMembers()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var editTeamBasePage = new EditTeamBasePage(Driver, Log);
            var editMtTeamMemberPage = new EditMtTeamMemberPage(Driver, Log);
            var addTeamMemberPage = new AddTeamMemberPage(Driver, Log);

            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(User.Username, User.Password);

            Log.Info($"Navigate to the 'Edit Team Members' page for enterprise team - {_enterpriseTeam1.Name} and add new team member");
            editTeamBasePage.NavigateToPage(_enterpriseId.ToString());
            editTeamBasePage.GoToGrowthTeamTab();
            addTeamMemberPage.ClickAddNewTeamMemberButton();
            var teamMemberInfo = new TeamMemberInfo
            {
                FirstName = "Member",
                LastName = "Name",
                Email = "memberemail" + CSharpHelpers.RandomNumber() + "@sharklasers.com",
            };
            addTeamMemberPage.EnterTeamMemberInfo(teamMemberInfo);
            addTeamMemberPage.ClickSaveAndCloseButton();

            Log.Info("Verify that member is successfully added");
            var userImagePath = addTeamMemberPage.GetAvatarFromMembersGrid(teamMemberInfo.Email);
            Assert.IsTrue(addTeamMemberPage.DoesMemberExist(userImagePath, teamMemberInfo.FirstName, teamMemberInfo.LastName, teamMemberInfo.Email), $"{teamMemberInfo.FirstName} member does not exit");

            Log.Info("Edit the team member");
            var teamMemberEdited = new TeamMemberInfo
            {
                FirstName = "MemberEdited",
                LastName = $"{SharedConstants.TeamMemberLastName}Edited",
                Email = "memberemailedited" + CSharpHelpers.RandomNumber() + "@sharklasers.com",
            };
            editMtTeamMemberPage.ClickTeamMemberEditButton(1);
            editMtTeamMemberPage.EnterTeamMemberInfo(teamMemberEdited);
            editMtTeamMemberPage.ClickUpdateButton();

            Log.Info("Verify that member is successfully edited");
            userImagePath = addTeamMemberPage.GetAvatarFromMembersGrid(teamMemberEdited.Email);
            Assert.IsTrue(addTeamMemberPage.DoesMemberExist(userImagePath, teamMemberEdited.FirstName, teamMemberEdited.LastName, teamMemberEdited.Email), $"{teamMemberEdited.FirstName} member does not exit");
        }

        [TestMethod]
        //For the enterprise type, no specific roles category exists, So need to comment related code for stakeholders.
        //This case will be reactivated once Dev. team resolves the issue.
        //[TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public void EnterpriseTeam_EditStakeHolders()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var editTeamBasePage = new EditTeamBasePage(Driver, Log);
            var editMtStakeHolderPage = new EditMtStakeHolderPage(Driver, Log);
            var addStakeHolderPage = new AddStakeHolderPage(Driver, Log);
            var addTeamMemberPage = new AddTeamMemberPage(Driver, Log);

            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(User.Username, User.Password);

            Log.Info($"Navigate to the stakeholder page for enterprise team - {_enterpriseTeam1.Name} and add stakeholder");

            editTeamBasePage.NavigateToPage(_enterpriseId.ToString());
            editTeamBasePage.GoToStakeHoldersTab();
            addStakeHolderPage.ClickAddNewStakeHolderButton();
            var stakeHolderInfo = new StakeHolderInfo
            {
                FirstName = "Stake",
                LastName = SharedConstants.TeamMemberLastName,
                Email = "stakeemail" + CSharpHelpers.RandomNumber() + "@sharklasers.com",
                Role = SharedConstants.StakeholderRole
            };

            addStakeHolderPage.EnterStakeHolderInfo(stakeHolderInfo);
            addStakeHolderPage.ClickSaveAndCloseButton();

            Log.Info("Verify that stakeholder is successfully added");
            var userImagePath = addTeamMemberPage.GetAvatarFromMembersGrid(stakeHolderInfo.Email);
            Assert.IsTrue(addTeamMemberPage.DoesMemberExist(userImagePath, stakeHolderInfo.FirstName, stakeHolderInfo.LastName, stakeHolderInfo.Email), $"{stakeHolderInfo.FirstName} is not exit");

            Log.Info("Edit the stakeholder");
            var stakeHolderEdited = new StakeHolderInfo
            {
                FirstName = "StakeEdited",
                LastName = $"{SharedConstants.TeamMemberLastName}Edited",
                Email = "stakeemailedited" + CSharpHelpers.RandomNumber() + "@sharklasers.com",
                Role = SharedConstants.StakeholderRole
            };

            editMtStakeHolderPage.ClickStakeHolderEditButton(1);
            editMtStakeHolderPage.EnterStakeHolderInfo(stakeHolderEdited);
            editMtStakeHolderPage.ClickUpdateButton();

            Log.Info("Verify that stakeholder is successfully edited");
            userImagePath = addTeamMemberPage.GetAvatarFromMembersGrid(stakeHolderEdited.Email);
            Assert.IsTrue(addTeamMemberPage.DoesMemberExist(userImagePath, stakeHolderEdited.FirstName, stakeHolderEdited.LastName, stakeHolderEdited.Email), $"{stakeHolderEdited.FirstName} is not exit");

        }
    }
}
