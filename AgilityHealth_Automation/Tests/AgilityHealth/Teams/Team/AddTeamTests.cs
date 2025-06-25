using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.DataObjects;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Common.Create;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Common.Edit;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Team.Create;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Teams.Team
{
    [TestClass]
    [TestCategory("Teams"), TestCategory("Team")]
    public class AddTeamTests : BaseTest
    {
        public static string NormalTeamName = "Team_" + RandomDataUtil.GetTeamName();
        public static string GoiTeamName = "GOI_" + RandomDataUtil.GetTeamName();
        public static bool ClassInitFailed;
        private static int _teamId;

        private static AddTeamWithMemberRequest _team;
        [ClassInitialize]
        public static void ClassSetUp(TestContext _)
        {
            try
            {
                _team = TeamFactory.GetNormalTeam("EditTeam");
                var setup = new SetupTeardownApi(TestEnvironment);
                var teamResponse = setup.CreateTeam(_team).GetAwaiter().GetResult();
                _teamId = new SetupTeardownApi(TestEnvironment).GetCompanyHierarchy(Company.Id).GetTeamByName(teamResponse.Name).TeamId;

            }
            catch (Exception)
            {
                ClassInitFailed = true;
                throw;
            }

        }

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("Smoke")]
        [TestCategory("Sanity")]
        [TestCategory("TeamAdmin2"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public void Team_AddNormalTeam()
        {
            var login = new LoginPage(Driver, Log);
            var csharpHelpers = new CSharpHelpers();
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var createTeamPage = new CreateTeamPage(Driver, Log);
            var addTeamMemberPage = new AddTeamMemberPage(Driver, Log);
            var addStakeHolderPage = new AddStakeHolderPage(Driver, Log);
            var finishAndReviewPage = new FinishAndReviewPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);

            Driver.NavigateToPage(ApplicationUrl);

            login.LoginToApplication(User.Username, User.Password);

            dashBoardPage.ClickAddATeamButton();

            dashBoardPage.SelectTeamType(TeamType.Team);

            dashBoardPage.ClickAddTeamButton();

            var actualWorkTypeList = createTeamPage.GetWorkTypeDropdownList();
            var expectedWorkTypeList = csharpHelpers.SortListAscending(actualWorkTypeList).ToList();
            Assert.That.ListsAreEqual(expectedWorkTypeList, actualWorkTypeList);

            var actualStrategicObjectivesList = createTeamPage.GetStrategicObjectivesDropdownList();
            var expectedStrategicObjectivesList = csharpHelpers.SortListAscending(actualStrategicObjectivesList).ToList();
            Assert.That.ListsAreEqual(expectedStrategicObjectivesList, actualStrategicObjectivesList);

            var actualCoachingList = createTeamPage.GetCoachingDropdownList();
            var expectedCoachingList = csharpHelpers.SortListAscending(actualCoachingList).ToList();
            Assert.That.ListsAreEqual(expectedCoachingList, actualCoachingList);

            var actualBusinessLinesList = createTeamPage.GetBusinessLineDropdownList();
            var expectedBusinessLineList = csharpHelpers.SortListAscending(actualBusinessLinesList).ToList();
            Assert.That.ListsAreEqual(expectedBusinessLineList, actualBusinessLinesList);

            var today = DateTime.Now.ToString("MMMM yyyy", CultureInfo.InvariantCulture);
            var teamInfo = new TeamInfo()
            {
                TeamName = NormalTeamName,
                WorkType = SharedConstants.NewTeamWorkType,
                PreferredLanguage = "English",
                Methodology = "Scrum",
                Department = "Test Department",
                DateEstablished = today,
                AgileAdoptionDate = today,
                Description = "Test Description",
                TeamBio = RandomDataUtil.GetTeamBio(),
                ImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\TestData\\1.jpg"),
                Tags = new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>("Business Lines", SharedConstants.TeamTag) }
            };

            createTeamPage.EnterTeamInfo(teamInfo);
            teamInfo.ImagePath = createTeamPage.GetTeamImage();

            createTeamPage.ClickCreateTeamAndAddTeamMembers();

            addTeamMemberPage.ClickAddNewTeamMemberButton();

            var actualTeamMemberRoleList = addTeamMemberPage.GetMemberRoleDropdownList();
            var expectedTeamMemberRoleList = csharpHelpers.SortListAscending(actualTeamMemberRoleList).ToList();
            Assert.That.ListsAreEqual(expectedTeamMemberRoleList, actualTeamMemberRoleList);

            var actualTeamMemberParticipantGroupList = addTeamMemberPage.GetTeamMemberParticipantGroupDropdownList();
            var expectedTeamMemberParticipantGroupList = csharpHelpers.SortListAscending(actualTeamMemberParticipantGroupList).ToList();
            Assert.That.ListsAreEqual(expectedTeamMemberParticipantGroupList, actualTeamMemberParticipantGroupList);

            var teamMemberInfo = new TeamMemberInfo
            {
                FirstName = "Member",
                LastName = SharedConstants.TeamMemberLastName,
                Email = "m" + CSharpHelpers.RandomNumber() + "@s.com",
                Role = "Developer",
                ParticipantGroup = "Technical"
            };

            addTeamMemberPage.EnterTeamMemberInfo(teamMemberInfo);
            addTeamMemberPage.ClickSaveAndCloseButton();

            TeamMemberInfo actualTeamMember = addTeamMemberPage.GetTeamMemberInfoFromGrid(1);
            Assert.AreEqual(teamMemberInfo.FirstName, actualTeamMember.FirstName, "Firstname doesn't match");
            Assert.AreEqual(teamMemberInfo.LastName, actualTeamMember.LastName, "Lastname doesn't match");
            Assert.AreEqual(teamMemberInfo.Email, actualTeamMember.Email, "Email doesn't match");
            Assert.AreEqual(teamMemberInfo.Role, actualTeamMember.Role, "Role doesn't match");

            addTeamMemberPage.ClickContinueToAddStakeHolder();

            addStakeHolderPage.ClickAddNewStakeHolderButton();

            var stakeHolderInfo = new StakeHolderInfo
            {
                FirstName = "Stake",
                LastName = SharedConstants.TeamMemberLastName,
                Email = "s" + CSharpHelpers.RandomNumber() + "@s.com",
                Role = "Sponsor"
            };

            var actualStakeholderRoleList = addStakeHolderPage.GetMemberRoleDropdownList();
            var expectedStakeholderRoleList = csharpHelpers.SortListAscending(actualStakeholderRoleList).ToList();
            Assert.That.ListsAreEqual(expectedStakeholderRoleList, actualStakeholderRoleList);

            addStakeHolderPage.EnterStakeHolderInfo(stakeHolderInfo);
            addStakeHolderPage.ClickSaveAndCloseButton();

            var actualStakeHolder = addStakeHolderPage.GetStakeHolderInfoFromGrid(1);
            Assert.AreEqual(stakeHolderInfo.FirstName, actualStakeHolder.FirstName, "Firstname doesn't match");
            Assert.AreEqual(stakeHolderInfo.LastName, actualStakeHolder.LastName, "Lastname doesn't match");
            Assert.AreEqual(stakeHolderInfo.Email, actualStakeHolder.Email, "Email doesn't match");
            Assert.AreEqual(stakeHolderInfo.Role, actualStakeHolder.Role, "Role doesn't match");

            addStakeHolderPage.ClickReviewAndFinishButton();

            TeamInfo actualTeamInfo = finishAndReviewPage.GetTeamInfo();
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

            TeamMemberInfo member1 = finishAndReviewPage.GetTeamMemberFromGrid(1);
            Assert.AreEqual(teamMemberInfo.FirstName, member1.FirstName, "Firstname doesn't match");
            Assert.AreEqual(teamMemberInfo.LastName, member1.LastName, "Lastname doesn't match");
            Assert.AreEqual(teamMemberInfo.Email, member1.Email, "Email doesn't match");
            Assert.IsTrue(member1.Role.Contains(teamMemberInfo.Role), "Role doesn't match");
            Assert.IsTrue(member1.Role.Contains(teamMemberInfo.ParticipantGroup), "ParticipantGroup doesn't match");

            StakeHolderInfo stake1 = finishAndReviewPage.GetStakeHolderFromGrid(1);
            Assert.AreEqual(stakeHolderInfo.FirstName, stake1.FirstName, "Firstname doesn't match");
            Assert.AreEqual(stakeHolderInfo.LastName, stake1.LastName, "Lastname doesn't match");
            Assert.AreEqual(stakeHolderInfo.Email, stake1.Email, "Email doesn't match");
            Assert.AreEqual(stakeHolderInfo.Role, stake1.Role, "Role doesn't match");

            finishAndReviewPage.ClickOnGoToTeamDashboard();

            dashBoardPage.GridTeamView();

            dashBoardPage.SearchTeam(teamInfo.TeamName);

            //Assert.AreEqual(teamInfo.ImagePath, dashBoardPage.GetAvatarSourceFromGrid(1), "Image Path doesn't match");
            Assert.AreEqual(teamInfo.TeamName, dashBoardPage.GetCellValue(1, "Team Name"), "Team Name doesn't match");
            Assert.AreEqual(teamInfo.WorkType, dashBoardPage.GetCellValue(1, "Work Type"), "Work Type doesn't match");
            Assert.IsTrue(dashBoardPage.GetCellValue(1, "Team Tags").Contains(SharedConstants.TeamTag), "Team Tag doesn't match");

            Log.Info("Navigate to assessment dashboard and verify pulse radio button is displayed or not after creating the team");
            var teamId = int.Parse(dashBoardPage.GetTeamIdFromLink(teamInfo.TeamName));
            teamAssessmentDashboard.NavigateToPage(teamId);
            teamAssessmentDashboard.ClickOnAddAnAssessmentButton();
            Assert.IsTrue(teamAssessmentDashboard.IsAddAnAssessmentPulseRadioButtonDisplayed(), "Pulse radio button is displayed");
        }

        [TestMethod]
        [TestCategory("TeamAdmin2"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public void Team_AddGroupOfIndividualsTeam()
        {
            var login = new LoginPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var createTeamPage = new CreateTeamPage(Driver, Log);
            var addTeamMemberPage = new AddTeamMemberPage(Driver, Log);
            var addStakeHolderPage = new AddStakeHolderPage(Driver, Log);
            var finishAndReviewPage = new FinishAndReviewPage(Driver, Log);
            var assessmentDashboardPage = new TeamAssessmentDashboardPage(Driver, Log);

            Driver.NavigateToPage(ApplicationUrl);

            login.LoginToApplication(User.Username, User.Password);

            createTeamPage.NavigateToPage(Company.Id.ToString());

            var today = DateTime.Now.ToString("MMMM yyyy", CultureInfo.InvariantCulture);
            var teamInfo = new TeamInfo()
            {
                TeamName = GoiTeamName,
                WorkType = "Group Of Individuals",
                PreferredLanguage = "English",
                Methodology = "Scrum",
                Department = "Test Department",
                DateEstablished = today,
                AgileAdoptionDate = today,
                Description = "Test Description",
                TeamBio = RandomDataUtil.GetTeamBio(),
                ImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\TestData\\1.jpg"),
                Tags = new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>("Business Lines", SharedConstants.TeamTag) }
            };

            createTeamPage.EnterTeamInfo(teamInfo);
            teamInfo.ImagePath = createTeamPage.GetTeamImage();

            createTeamPage.ClickCreateTeamAndAddTeamMembers();

            addTeamMemberPage.ClickAddNewTeamMemberButton();

            var teamMemberInfo = new TeamMemberInfo
            {
                FirstName = "Member",
                LastName = SharedConstants.TeamMemberLastName,
                Email = "m" + CSharpHelpers.RandomNumber() + "@s.com",
                Role = "Individual",
                ParticipantGroup = "Technical"
            };

            addTeamMemberPage.EnterTeamMemberInfo(teamMemberInfo);
            addTeamMemberPage.ClickSaveAndCloseButton();

            TeamMemberInfo actualTeamMember = addTeamMemberPage.GetTeamMemberInfoFromGrid(1);
            Assert.AreEqual(teamMemberInfo.FirstName, actualTeamMember.FirstName, "Firstname doesn't match");
            Assert.AreEqual(teamMemberInfo.LastName, actualTeamMember.LastName, "Lastname doesn't match");
            Assert.AreEqual(teamMemberInfo.Email, actualTeamMember.Email, "Email doesn't match");
            Assert.AreEqual(teamMemberInfo.ParticipantGroup, actualTeamMember.ParticipantGroup, "Participant group doesn't match");

            addTeamMemberPage.ClickContinueToAddStakeHolder();

            addStakeHolderPage.ClickAddNewStakeHolderButton();

            var stakeHolderInfo = new StakeHolderInfo
            {
                FirstName = "Stake",
                LastName = SharedConstants.TeamMemberLastName,
                Email = "s" + CSharpHelpers.RandomNumber() + "@s.com",
                Role = "Sponsor"
            };

            addStakeHolderPage.EnterStakeHolderInfo(stakeHolderInfo);
            addStakeHolderPage.ClickSaveAndCloseButton();

            StakeHolderInfo actualStakeHolder = addStakeHolderPage.GetStakeHolderInfoFromGrid(1);
            Assert.AreEqual(stakeHolderInfo.FirstName, actualStakeHolder.FirstName, "Firstname doesn't match");
            Assert.AreEqual(stakeHolderInfo.LastName, actualStakeHolder.LastName, "Lastname doesn't match");
            Assert.AreEqual(stakeHolderInfo.Email, actualStakeHolder.Email, "Email doesn't match");
            Assert.AreEqual(stakeHolderInfo.Role, actualStakeHolder.Role, "Role doesn't match");

            addStakeHolderPage.ClickReviewAndFinishButton();

            TeamInfo actualTeamInfo = finishAndReviewPage.GetTeamInfo();
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

            TeamMemberInfo member1 = finishAndReviewPage.GetTeamMemberFromGrid(1);
            Assert.AreEqual(teamMemberInfo.FirstName, member1.FirstName, "Firstname doesn't match");
            Assert.AreEqual(teamMemberInfo.LastName, member1.LastName, "Lastname doesn't match");
            Assert.AreEqual(teamMemberInfo.Email, member1.Email, "Email doesn't match");
            Assert.IsTrue(member1.Role.Contains(teamMemberInfo.Role), "Role doesn't match");
            Assert.IsTrue(member1.Role.Contains(teamMemberInfo.ParticipantGroup), "ParticipantGroup doesn't match");

            StakeHolderInfo stake1 = finishAndReviewPage.GetStakeHolderFromGrid(1);
            Assert.AreEqual(stakeHolderInfo.FirstName, stake1.FirstName, "Firstname doesn't match");
            Assert.AreEqual(stakeHolderInfo.LastName, stake1.LastName, "Lastname doesn't match");
            Assert.AreEqual(stakeHolderInfo.Email, stake1.Email, "Email doesn't match");
            Assert.AreEqual(stakeHolderInfo.Role, stake1.Role, "Role doesn't match");

            finishAndReviewPage.ClickOnGoToAssessmentDashboard();

            Assert.AreEqual("handle ico ease on", assessmentDashboardPage.GetToggleClass());

            Driver.NavigateToPage(ApplicationUrl);

            dashBoardPage.GridTeamView();

            dashBoardPage.SearchTeam(teamInfo.TeamName);

            Assert.AreEqual(teamInfo.ImagePath, dashBoardPage.GetAvatarSourceFromGrid(1), "Image Path doesn't match");
            Assert.AreEqual(teamInfo.TeamName, dashBoardPage.GetCellValue(1, "Team Name"), "Team Name doesn't match");
            Assert.AreEqual(teamInfo.WorkType, dashBoardPage.GetCellValue(1, "Work Type"), "Work Type doesn't match");
            Assert.IsTrue(dashBoardPage.GetCellValue(1, "Team Tags").Contains(SharedConstants.TeamTag), "Team Tag doesn't match");
        }

        [TestMethod]
        [TestCategory("TeamAdmin2"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public void Team_ReviewFinishPage_VerifyEditNavigation()
        {
            VerifySetup(ClassInitFailed);
            var login = new LoginPage(Driver, Log);
            var finishAndReviewPage = new FinishAndReviewPage(Driver, Log);
            var editProfileBasePage = new EditProfileBasePage(Driver, Log);
            var addTeamMemberPage = new AddTeamMemberPage(Driver, Log);
            var addStakeHolderPage = new AddStakeHolderPage(Driver, Log);
            var editTeamBasePage = new EditTeamBasePage(Driver, Log);

            const string teamProfile = "Team Profile";
            const string teamMembers = "Team Members";
            const string stakeholder = "Stakeholders";

            Log.Info($"Login as {User.FullName}");
            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Navigate to the 'Review & Finish' page");
            finishAndReviewPage.NavigateToPage(_teamId.ToString());

            Log.Info($"On Review & Finish page, Click on 'Edit' link for '{teamProfile}' and verify page title and page url");
            finishAndReviewPage.ClickOnEditLink(teamProfile);
            Assert.AreEqual($"Edit {_team.Name}", editProfileBasePage.GetTeamProfilePageTitle(), "Edit team profile title is not matched");
            Assert.AreEqual(Driver.GetCurrentUrl(), $"{BaseTest.ApplicationUrl}/teams/profile/{_teamId}", "Edit team profile page url is not matched");

            editProfileBasePage.ClickUpdateTeamProfileButton();
            addTeamMemberPage.ClickContinueToAddStakeHolder();
            addStakeHolderPage.ClickReviewAndFinishButton();

            Log.Info($"On Review & Finish page, Click on 'Edit' link for '{teamMembers}' and verify page title and page url");
            finishAndReviewPage.ClickOnEditLink(teamMembers);
            Assert.AreEqual("Add Team Members", editTeamBasePage.GetPageHeaderTitle(), "Edit team members title is not matched");
            Assert.AreEqual(Driver.GetCurrentUrl(), $"{BaseTest.ApplicationUrl}/teammembers/{_teamId}", "Edit team members page url is not matched");

            addTeamMemberPage.ClickContinueToAddStakeHolder();
            addStakeHolderPage.ClickReviewAndFinishButton();

            Log.Info($"On Review & Finish page, Click on 'Edit' link for '{stakeholder}' and verify page title and page url");
            finishAndReviewPage.ClickOnEditLink(stakeholder);
            Assert.AreEqual("Add Stakeholders", editTeamBasePage.GetPageHeaderTitle(), "Edit stakeholder title is not matched");
            Assert.IsTrue(Driver.GetCurrentUrl().Contains($"/stakeholders/{_teamId}"), "Edit stakeholder page url is not matched");

        }

        [TestMethod]
        [TestCategory("TeamAdmin2"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public void Team_ReviewFinishPage_VerifyClickHereLinkNavigation()
        {
            VerifySetup(ClassInitFailed);

            var login = new LoginPage(Driver, Log);
            var finishAndReviewPage = new FinishAndReviewPage(Driver, Log);
            var addTeamMemberPage = new AddTeamMemberPage(Driver, Log);
            var addStakeHolderPage = new AddStakeHolderPage(Driver, Log);
            var editTeamBasePage = new EditTeamBasePage(Driver, Log);

            const string teamMembers = "team members";
            const string stakeholder = "stakeholders";

            Log.Info($"Login as {User.FullName}");
            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Navigate to the 'Review & Finish' page");
            finishAndReviewPage.NavigateToPage(_teamId.ToString());

            Log.Info($"On 'Review & Finish' page, click here link should be present for {teamMembers}");
            Assert.IsTrue(finishAndReviewPage.IsClickHereLinkPresent(teamMembers));

            Log.Info($"Click on the 'click here' link for {teamMembers} and verify the page title and page url");
            finishAndReviewPage.ClickOnClickHereLink(teamMembers);
            Assert.AreEqual("Add Team Members", editTeamBasePage.GetPageHeaderTitle(), "Edit team members title is not matched");
            Assert.AreEqual(Driver.GetCurrentUrl(), $"{BaseTest.ApplicationUrl}/teammembers/{_teamId}", "Edit team members page url is not matched");

            addTeamMemberPage.ClickContinueToAddStakeHolder();
            addStakeHolderPage.ClickReviewAndFinishButton();

            Log.Info($"On 'Review & Finish' page, click here link should be present for {stakeholder}");
            Assert.IsTrue(finishAndReviewPage.IsClickHereLinkPresent(stakeholder));

            Log.Info($"Click on the 'click here' link for {stakeholder} and verify the page title and page url");
            finishAndReviewPage.ClickOnClickHereLink(stakeholder);
            Assert.AreEqual("Add Stakeholders", editTeamBasePage.GetPageHeaderTitle(), "Edit stakeholders title is not matched");
            Assert.AreEqual(Driver.GetCurrentUrl(), $"{BaseTest.ApplicationUrl}/stakeholders/{_teamId}", "Edit stakeholders page url is not matched");
        }
    }
}