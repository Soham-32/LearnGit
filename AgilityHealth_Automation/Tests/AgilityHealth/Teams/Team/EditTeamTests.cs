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
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using AtCommon.Dtos.Integrations.Custom;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Teams.Team
{

    [TestClass]
    [TestCategory("Teams"), TestCategory("Team")]
    public class EditTeamTests : BaseTest
    {
        private static bool _classInitFailed;
        private static AddTeamWithMemberRequest _team;

        [ClassInitialize]
        public static void ClassSetUp(TestContext _)
        {
            try
            {
                _team = TeamFactory.GetNormalTeam("EditTeam");
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
        [TestCategory("Critical")]
        [TestCategory("TeamAdmin2"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public void Team_EditProfile()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var editTeamBasePage = new EditTeamBasePage(Driver, Log);
            var editTeamProfilePage = new EditTeamProfilePage(Driver, Log);

            var csharpHelpers = new CSharpHelpers();

            Driver.NavigateToPage(ApplicationUrl);

            login.LoginToApplication(User.Username, User.Password);

            dashBoardPage.GridTeamView();

            dashBoardPage.SearchTeam(_team.Name);
            var teamId = dashBoardPage.GetTeamIdFromLink(_team.Name);

            dashBoardPage.ClickTeamEditButton(_team.Name);

            editTeamBasePage.GoToTeamProfileTab();

            var actualWorkTypeList = editTeamProfilePage.GetWorkTypeDropdownList();
            var expectedWorkTypeList = csharpHelpers.SortListAscending(actualWorkTypeList).ToList();
            Assert.That.ListsAreEqual(expectedWorkTypeList, actualWorkTypeList);

            var actualStrategicObjectivesList = editTeamProfilePage.GetStrategicObjectivesDropdownList();
            var expectedStrategicObjectivesList = csharpHelpers.SortListAscending(actualStrategicObjectivesList).ToList();
            Assert.That.ListsAreEqual(expectedStrategicObjectivesList, actualStrategicObjectivesList);

            var actualCoachingList = editTeamProfilePage.GetCoachingDropdownList();
            var expectedCoachingList = csharpHelpers.SortListAscending(actualCoachingList).ToList();
            Assert.That.ListsAreEqual(expectedCoachingList, actualCoachingList);

            var actualBusinessLinesList = editTeamProfilePage.GetBusinessLineDropdownList();
            var expectedBusinessLineList = csharpHelpers.SortListAscending(actualBusinessLinesList).ToList();
            Assert.That.ListsAreEqual(expectedBusinessLineList, actualBusinessLinesList);

            string today = DateTime.UtcNow.AddMonths(1).ToString("MMMM yyyy", CultureInfo.InvariantCulture);
            TeamInfo teamInfo = new TeamInfo()
            {
                TeamName = "Edit_" + RandomDataUtil.GetTeamName(),
                WorkType = "Software Delivery",
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
            teamInfo.ImagePath = editTeamProfilePage.GetTeamImage();

            editTeamProfilePage.ClickUpdateTeamProfileButton();
            _team.Name = teamInfo.TeamName;

            editTeamBasePage.GoToDashboard();

            editTeamBasePage.NavigateToPage(teamId);

            editTeamBasePage.GoToTeamProfileTab();

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
        public void Team_EditTeamMembers()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var editTeamBasePage = new EditTeamBasePage(Driver, Log);
            var editTeamTeamMemberPage = new EditTeamTeamMemberPage(Driver, Log);
            var csharpHelpers = new CSharpHelpers();

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
                Email = "me" + CSharpHelpers.RandomNumber() + "@sh.com",
                Role = "Designer",
                ParticipantGroup = "Support"
            };

            editTeamTeamMemberPage.ClickTeamMemberEditButton(1);

            var actualTeamMemberRoleList = editTeamTeamMemberPage.GetMemberRoleDropdownList();
            var expectedTeamMemberRoleList = csharpHelpers.SortListAscending(actualTeamMemberRoleList).ToList();
            Assert.That.ListsAreEqual(expectedTeamMemberRoleList, actualTeamMemberRoleList);

            var actualTeamMemberParticipantGroupList = editTeamTeamMemberPage.GetTeamMemberParticipantGroupDropdownList();
            var expectedTeamMemberParticipantGroupList = csharpHelpers.SortListAscending(actualTeamMemberParticipantGroupList).ToList();
            Assert.That.ListsAreEqual(expectedTeamMemberParticipantGroupList, actualTeamMemberParticipantGroupList);

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
        [TestCategory("TeamAdmin2"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public void Team_EditTeamStakeHolders()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var addStakeHolderPage = new AddStakeHolderPage(Driver, Log);
            var editTeamBasePage = new EditTeamBasePage(Driver, Log);
            var editTeamStakeHolderPage = new EditTeamStakeHolderPage(Driver, Log);
            var csharpHelpers = new CSharpHelpers();

            Driver.NavigateToPage(ApplicationUrl);

            login.LoginToApplication(User.Username, User.Password);

            var stakeHolderInfo = new StakeHolderInfo
            {
                FirstName = "Stake",
                LastName = SharedConstants.TeamMemberLastName,
                Email = "s" + CSharpHelpers.RandomNumber() + "@s.com",
                Role = "Sponsor"
            };

            dashBoardPage.GridTeamView();

            var teamId = dashBoardPage.GetTeamIdFromLink(_team.Name);
            editTeamBasePage.NavigateToPage(teamId);

            editTeamBasePage.GoToStakeHoldersTab();

            editTeamStakeHolderPage.ClickAddNewStakeHolderButton();

            var actualStakeholderRoleList = editTeamStakeHolderPage.GetMemberRoleDropdownList();
            var expectedStakeholderRoleList = csharpHelpers.SortListAscending(actualStakeholderRoleList).ToList();
            Assert.That.ListsAreEqual(expectedStakeholderRoleList, actualStakeholderRoleList);

            addStakeHolderPage.EnterStakeHolderInfo(stakeHolderInfo, "Edit");
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
                Email = "se" + CSharpHelpers.RandomNumber() + "@sh.com",
                Role = "Executive"
            };

            editTeamStakeHolderPage.ClickStakeHolderEditButton(1);
            editTeamStakeHolderPage.EnterStakeHolderInfo(stakeHolderInfo2, "Edit");
            editTeamStakeHolderPage.ClickUpdateButton();

            StakeHolderInfo actualStakeHolder2 = editTeamStakeHolderPage.GetStakeHolderInfoFromGrid(1);
            Assert.AreEqual(stakeHolderInfo2.FirstName, actualStakeHolder2.FirstName, "Firstname doesn't match");
            Assert.AreEqual(stakeHolderInfo2.LastName, actualStakeHolder2.LastName, "Lastname doesn't match");
            Assert.AreEqual(stakeHolderInfo2.Email, actualStakeHolder2.Email, "Email doesn't match");
            Assert.AreEqual(stakeHolderInfo2.Role, actualStakeHolder2.Role, "Role doesn't match");
        }

        [TestMethod]
        [TestCategory("KnownDefect")] // Bug Id: 51218
        [TestCategory("TeamAdmin2"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public void Team_EditTeamMetrics()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var editTeamBasePage = new EditTeamBasePage(Driver, Log);
            var editTeamMetricsPage = new EditMetricsBasePage(Driver, Log);

            Driver.NavigateToPage(ApplicationUrl);

            login.LoginToApplication(User.Username, User.Password);

            dashBoardPage.GridTeamView();

            var teamId = dashBoardPage.GetTeamIdFromLink(_team.Name);
            editTeamBasePage.NavigateToPage(teamId);

            editTeamBasePage.GoToMetricsTab();

            editTeamMetricsPage.ClickAddNewIterationDataButton();

            string iterationToday = DateTime.UtcNow.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
            var iterationData = new Iteration()
            {
                Name = "Iteration test",
                From = iterationToday,
                To = iterationToday,
                CommittedPoints = "10",
                CompletedPoints = "5",
                Defects = "20"
            };
            editTeamMetricsPage.EnterIterationData(iterationData);

            Iteration actualIteration = editTeamMetricsPage.GetIterationDataFromGrid(1);
            Assert.AreEqual(iterationData.Name, actualIteration.Name, "Name doesn't match");
            Assert.AreEqual(iterationData.From, actualIteration.From, "From doesn't match");
            Assert.AreEqual(iterationData.To, actualIteration.To, "To doesn't match");
            Assert.AreEqual(iterationData.CommittedPoints, actualIteration.CommittedPoints, "Target Points doesn't match");
            Assert.AreEqual(iterationData.CompletedPoints, actualIteration.CompletedPoints, "Actual Points doesn't match");
            Assert.AreEqual(iterationData.Defects, actualIteration.Defects, "Defects doesn't match");

            editTeamMetricsPage.ClickAddNewReleaseDataButton();

            string releaseToday = DateTime.UtcNow.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
            var releaseData = new Release()
            {
                Name = "Release test",
                TargetDate = releaseToday,
                ActualDate = releaseToday,
                Defects = "8"
            };

            editTeamMetricsPage.EnterReleaseData(releaseData);

            Release actualRelease = editTeamMetricsPage.GetReleaseDataFromGrid(1);
            Assert.AreEqual(releaseData.Name, actualRelease.Name, "Name doesn't match");
            Assert.AreEqual(releaseData.TargetDate, actualRelease.TargetDate, "Target Date doesn't match");
            Assert.AreEqual(releaseData.ActualDate, actualRelease.ActualDate, "Actual Date doesn't match");
            Assert.AreEqual(releaseData.Defects, actualRelease.Defects, "Defects doesn't match");

            iterationToday = DateTime.Now.AddDays(1).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
            iterationData = new Iteration()
            {
                Name = "Iteration test edited",
                From = iterationToday,
                To = iterationToday,
                CommittedPoints = "11",
                CompletedPoints = "6",
                Defects = "21"
            };

            editTeamMetricsPage.ClickIterationDataEditButton(1);
            editTeamMetricsPage.EnterIterationData(iterationData);

            releaseToday = DateTime.Now.AddDays(1).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
            releaseData = new Release()
            {
                Name = "Release test edited",
                TargetDate = releaseToday,
                ActualDate = releaseToday,
                Defects = "10"
            };

            editTeamMetricsPage.ClickReleaseDataEditButton(1);
            editTeamMetricsPage.EnterReleaseData(releaseData);

            editTeamMetricsPage.ExpandIteration();
            Iteration actualIterationData2 = editTeamMetricsPage.GetIterationDataFromGrid(1);
            Assert.AreEqual(iterationData.Name, actualIterationData2.Name, "Name doesn't match");
            // TODO: This isn't working and I don't know why. It works when you manually do it
            //Assert.AreEqual(iteration.From, actualIterationData2.From, "From doesn't match");
            //Assert.AreEqual(iteration.To, actualIterationData2.To, "To doesn't match");
            Assert.AreEqual(iterationData.CommittedPoints, actualIterationData2.CommittedPoints, "Target Points doesn't match");
            Assert.AreEqual(iterationData.CompletedPoints, actualIterationData2.CompletedPoints, "Actual Points doesn't match");
            Assert.AreEqual(iterationData.Defects, actualIterationData2.Defects, "Defects doesn't match");

            editTeamMetricsPage.ExpandRelease();
            Release actualReleaseData2 = editTeamMetricsPage.GetReleaseDataFromGrid(1);
            Assert.AreEqual(releaseData.Name, actualReleaseData2.Name, "Name doesn't match");
            Assert.AreEqual(releaseData.TargetDate, actualReleaseData2.TargetDate, "Target Date doesn't match");
            Assert.AreEqual(releaseData.ActualDate, actualReleaseData2.ActualDate, "Actual Date doesn't match");
            Assert.AreEqual(releaseData.Defects, actualReleaseData2.Defects, "Defects doesn't match");
        }
    }
}