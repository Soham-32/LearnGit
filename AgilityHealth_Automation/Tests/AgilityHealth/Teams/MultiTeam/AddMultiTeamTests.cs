using System;
using System.Globalization;
using System.IO;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.DataObjects;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Common.Create;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Common.Edit;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.MultiTeam.Create;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Team.Create;
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
    public class AddMultiTeamTests : BaseTest
    {
        public static bool ClassInitFailed;
        private static AddTeamWithMemberRequest _multiTeam;
        private static int _multiTeamId;

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            try
            {
                _multiTeam = TeamFactory.GetMultiTeam("EditMT");

                var setup = new SetupTeardownApi(TestEnvironment);
                var multiTeamResponse = setup.CreateTeam(_multiTeam).GetAwaiter().GetResult();
                _multiTeamId = new SetupTeardownApi(TestEnvironment).GetCompanyHierarchy(Company.Id).GetTeamByName(multiTeamResponse.Name).TeamId;
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
        public void MultiTeam_AddNewTeam()
        {
            var login = new LoginPage(Driver, Log);

            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var createMultiTeamPage = new CreateMultiTeamPage(Driver, Log);
            var addTeamMemberPage = new AddTeamMemberPage(Driver, Log);
            var addStakeHolderPage = new AddStakeHolderPage(Driver, Log);
            var finishAndReviewPage = new FinishAndReviewPage(Driver, Log);
            var addMtSubTeamPage = new AddMtSubTeamPage(Driver, Log);

            Driver.NavigateToPage(ApplicationUrl);

            login.LoginToApplication(User.Username, User.Password);

            dashBoardPage.ClickAddATeamButton();

            dashBoardPage.SelectTeamType(TeamType.MultiTeam);

            dashBoardPage.ClickAddTeamButton();

            var today = DateTime.Now.ToString("MMMM yyyy", CultureInfo.InvariantCulture);

            var multiTeamInfo = new MultiTeamInfo()
            {
                TeamName = "MultiTeam" + RandomDataUtil.GetTeamName() + CSharpHelpers.RandomNumber(),
                TeamType = "Program Team",
                AssessmentType = SharedConstants.TeamAssessmentType,
                Department = "Test Department",
                DateEstablished = today,
                AgileAdoptionDate = today,
                Description = "Test Description",
                TeamBio = RandomDataUtil.GetTeamBio(),
                ImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\TestData\\2.jpg")
            };

            createMultiTeamPage.EnterMultiTeamInfo(multiTeamInfo);
            var teamImagePath = createMultiTeamPage.GetTeamImage();
            multiTeamInfo.ImagePath = teamImagePath;

            createMultiTeamPage.ClickCreateTeamAndAddSubTeam();

            addMtSubTeamPage.SelectSubTeam(SharedConstants.Team);

            addMtSubTeamPage.ClickAddSubTeamButton();

            addTeamMemberPage.ClickAddNewTeamMemberButton();

            var teamMemberInfo = new TeamMemberInfo
            {
                FirstName = "Member",
                LastName = SharedConstants.TeamMemberLastName,
                Email = "memberemail" + CSharpHelpers.RandomNumber() + "@sharklasers.com",
                Role = "Architect"
            };

            addTeamMemberPage.EnterTeamMemberInfo(teamMemberInfo);
            addTeamMemberPage.ClickSaveAndCloseButton();

            var actualTeamMember = addTeamMemberPage.GetTeamMemberInfoFromGrid(1);
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

            addStakeHolderPage.ClickReviewAndFinishButton();

            var actualInfo = finishAndReviewPage.GetMultiTeamInfo();
            Assert.AreEqual(multiTeamInfo.AgileAdoptionDate, actualInfo.AgileAdoptionDate, "Agile Adoption Date doesn't match");
            Assert.AreEqual(multiTeamInfo.DateEstablished, actualInfo.DateEstablished, "Date Established doesn't match");
            Assert.AreEqual(multiTeamInfo.Department, actualInfo.Department, "Department doesn't match");
            Assert.AreEqual(multiTeamInfo.Description, actualInfo.Description, "Description doesn't match");
            Assert.AreEqual(multiTeamInfo.TeamBio, actualInfo.TeamBio, "Team BIO doesn't match");
            Assert.AreEqual(multiTeamInfo.TeamName, actualInfo.TeamName, "Team Name doesn't match");
            Assert.AreEqual(multiTeamInfo.ImagePath, actualInfo.ImagePath, "Image Path doesn't match");

            Assert.IsTrue(finishAndReviewPage.GetSubTeamsText().Contains(SharedConstants.Team), "Failure !! Sub-team: " + SharedConstants.Team + " does not display in Finish and Review page");

            var actualTeamMember2 = finishAndReviewPage.GetTeamMemberFromGrid(1);
            Assert.AreEqual(teamMemberInfo.FirstName, actualTeamMember2.FirstName, "Firstname doesn't match");
            Assert.AreEqual(teamMemberInfo.LastName, actualTeamMember2.LastName, "Lastname doesn't match");
            Assert.AreEqual(teamMemberInfo.Email, actualTeamMember2.Email, "Email doesn't match");
            Assert.AreEqual(teamMemberInfo.Role, actualTeamMember2.Role, "Role doesn't match");

            var actualStakeHolder2 = finishAndReviewPage.GetStakeHolderFromGrid(1);
            Assert.AreEqual(stakeHolderInfo.FirstName, actualStakeHolder2.FirstName, "Firstname doesn't match");
            Assert.AreEqual(stakeHolderInfo.LastName, actualStakeHolder2.LastName, "Lastname doesn't match");
            Assert.AreEqual(stakeHolderInfo.Email, actualStakeHolder2.Email, "Email doesn't match");
            Assert.AreEqual(stakeHolderInfo.Role, actualStakeHolder2.Role, "Role doesn't match");

            finishAndReviewPage.ClickOnGoToTeamDashboard();

            dashBoardPage.GridTeamView();

            dashBoardPage.SearchTeam(multiTeamInfo.TeamName);

            //Assert.AreEqual(teamImagePath, dashBoardPage.GetAvatarSourceFromGrid(1), "Image is incorrect");
            Assert.AreEqual(multiTeamInfo.TeamName, dashBoardPage.GetCellValue(1, "Team Name"), "Name is incorrect");
            Assert.AreEqual(multiTeamInfo.TeamType, dashBoardPage.GetCellValue(1, "Work Type"), "Team Type is incorrect");
            Assert.IsTrue(dashBoardPage.GetCellValue(1, "Team Tags").Contains(SharedConstants.TeamTag), "Tags are incorrect");
        }

        [TestMethod]
        [TestCategory("TeamAdmin2"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public void MultiTeam_AddNewTeam_NoSubTeams()
        {
            var login = new LoginPage(Driver, Log);

            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var createMultiTeamPage = new CreateMultiTeamPage(Driver, Log);
            var addTeamMemberPage = new AddTeamMemberPage(Driver, Log);
            var addStakeHolderPage = new AddStakeHolderPage(Driver, Log);
            var finishAndReviewPage = new FinishAndReviewPage(Driver, Log);
            var addMtSubTeamPage = new AddMtSubTeamPage(Driver, Log);

            Driver.NavigateToPage(ApplicationUrl);

            login.LoginToApplication(User.Username, User.Password);

            createMultiTeamPage.NavigateToPage(Company.Id.ToString());

            var multiTeamInfo = new MultiTeamInfo()
            {
                TeamName = "MultiTeam" + RandomDataUtil.GetTeamName(),
                TeamType = "Program Team"
            };

            createMultiTeamPage.EnterMultiTeamInfo(multiTeamInfo);

            createMultiTeamPage.ClickCreateTeamAndAddSubTeam();

            addMtSubTeamPage.ClickAddSubTeamButton();

            addTeamMemberPage.ClickContinueToAddStakeHolder();

            addStakeHolderPage.ClickReviewAndFinishButton();

            var actualInfo = finishAndReviewPage.GetMultiTeamInfo();

            Assert.AreEqual(multiTeamInfo.TeamName, actualInfo.TeamName, "Team Name doesn't match");

            Assert.AreEqual("There are no sub-teams in this team. Click here to add some sub-teams.", finishAndReviewPage.GetNoSubTeamText());

            dashBoardPage.NavigateToPage(Company.Id);

            dashBoardPage.GridTeamView();

            dashBoardPage.SearchTeam(multiTeamInfo.TeamName);

            Assert.AreEqual(multiTeamInfo.TeamName, dashBoardPage.GetCellValue(1, "Team Name"), "Name is incorrect");
            Assert.AreEqual(multiTeamInfo.TeamType, dashBoardPage.GetCellValue(1, "Work Type"), "Team Type is incorrect");
            Assert.AreEqual("0", dashBoardPage.GetCellValue(1, "Number of Sub Teams"));
        }

        [TestMethod]
        [TestCategory("TeamAdmin2"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public void MultiTeam_AddNewGoiTeam()
        {
            var login = new LoginPage(Driver, Log);

            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var createMultiTeamPage = new CreateMultiTeamPage(Driver, Log);
            var addTeamMemberPage = new AddTeamMemberPage(Driver, Log);
            var addStakeHolderPage = new AddStakeHolderPage(Driver, Log);
            var finishAndReviewPage = new FinishAndReviewPage(Driver, Log);
            var addMtSubTeamPage = new AddMtSubTeamPage(Driver, Log);

            Driver.NavigateToPage(ApplicationUrl);

            login.LoginToApplication(User.Username, User.Password);

            createMultiTeamPage.NavigateToPage(Company.Id.ToString());

            var today = DateTime.Now.ToString("MMMM yyyy", CultureInfo.InvariantCulture);

            var multiTeamInfo = new MultiTeamInfo()
            {
                TeamName = "MultiTeam" + RandomDataUtil.GetTeamName(),
                TeamType = "Group Of Individuals",
                AssessmentType = SharedConstants.TeamAssessmentType,
                Department = "Test Department",
                DateEstablished = today,
                AgileAdoptionDate = today,
                Description = "Test Description",
                TeamBio = RandomDataUtil.GetTeamBio(),
                ImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\TestData\\2.jpg")
            };

            createMultiTeamPage.EnterMultiTeamInfo(multiTeamInfo);
            var teamImagePath = createMultiTeamPage.GetTeamImage();
            multiTeamInfo.ImagePath = teamImagePath;

            createMultiTeamPage.ClickCreateTeamAndAddSubTeam();

            addMtSubTeamPage.SelectSubTeam(Constants.GoiTeam);
            addMtSubTeamPage.SelectSubTeam(Constants.GoiTeam2);

            addMtSubTeamPage.ClickAddSubTeamButton();

            addTeamMemberPage.ClickAddNewTeamMemberButton();

            var teamMemberInfo = new TeamMemberInfo
            {
                FirstName = "Member",
                LastName = SharedConstants.TeamMemberLastName,
                Email = "memberemail" + CSharpHelpers.RandomNumber() + "@sharklasers.com"
            };

            addTeamMemberPage.EnterTeamMemberInfo(teamMemberInfo);
            addTeamMemberPage.ClickSaveAndCloseButton();

            var actualTeamMember = addTeamMemberPage.GetTeamMemberInfoFromGrid(1);
            Assert.AreEqual(teamMemberInfo.FirstName, actualTeamMember.FirstName, "Firstname doesn't match");
            Assert.AreEqual(teamMemberInfo.LastName, actualTeamMember.LastName, "Lastname doesn't match");
            Assert.AreEqual(teamMemberInfo.Email, actualTeamMember.Email, "Email doesn't match");
            Assert.AreEqual("", actualTeamMember.Role, "Role doesn't match");

            addTeamMemberPage.ClickContinueToAddStakeHolder();

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

            addStakeHolderPage.ClickReviewAndFinishButton();

            var actualInfo = finishAndReviewPage.GetMultiTeamInfo();
            Assert.AreEqual(multiTeamInfo.AgileAdoptionDate, actualInfo.AgileAdoptionDate, "Agile Adoption Date doesn't match");
            Assert.AreEqual(multiTeamInfo.DateEstablished, actualInfo.DateEstablished, "Date Established doesn't match");
            Assert.AreEqual(multiTeamInfo.Department, actualInfo.Department, "Department doesn't match");
            Assert.AreEqual(multiTeamInfo.Description, actualInfo.Description, "Description doesn't match");
            Assert.AreEqual(multiTeamInfo.TeamBio, actualInfo.TeamBio, "Team BIO doesn't match");
            Assert.AreEqual(multiTeamInfo.TeamName, actualInfo.TeamName, "Team Name doesn't match");
            Assert.AreEqual(multiTeamInfo.ImagePath, actualInfo.ImagePath, "Image Path doesn't match");

            Assert.IsTrue(finishAndReviewPage.GetSubTeamsText().Contains(Constants.GoiTeam), "Failure !! Sub-team: " + Constants.GoiTeam + " does not display in Finish and Review page");
            Assert.IsTrue(finishAndReviewPage.GetSubTeamsText().Contains(Constants.GoiTeam2), "Failure !! Sub-team: " + Constants.GoiTeam2 + " does not display in Finish and Review page");

            var actualTeamMember2 = finishAndReviewPage.GetTeamMemberFromGrid(1);
            Assert.AreEqual(teamMemberInfo.FirstName, actualTeamMember2.FirstName, "Firstname doesn't match");
            Assert.AreEqual(teamMemberInfo.LastName, actualTeamMember2.LastName, "Lastname doesn't match");
            Assert.AreEqual(teamMemberInfo.Email, actualTeamMember2.Email, "Email doesn't match");

            var actualStakeHolder2 = finishAndReviewPage.GetStakeHolderFromGrid(1);
            Assert.AreEqual(stakeHolderInfo.FirstName, actualStakeHolder2.FirstName, "Firstname doesn't match");
            Assert.AreEqual(stakeHolderInfo.LastName, actualStakeHolder2.LastName, "Lastname doesn't match");
            Assert.AreEqual(stakeHolderInfo.Email, actualStakeHolder2.Email, "Email doesn't match");
            Assert.AreEqual(stakeHolderInfo.Role, actualStakeHolder2.Role, "Role doesn't match");

            dashBoardPage.NavigateToPage(Company.Id);

            dashBoardPage.GridTeamView();

            dashBoardPage.SearchTeam(multiTeamInfo.TeamName);

            Assert.AreEqual(teamImagePath, dashBoardPage.GetAvatarSourceFromGrid(1), "Image is incorrect");
            Assert.AreEqual(multiTeamInfo.TeamName, dashBoardPage.GetCellValue(1, "Team Name"), "Name is incorrect");
            Assert.AreEqual(multiTeamInfo.TeamType, dashBoardPage.GetCellValue(1, "Work Type"), "Team Type is incorrect");
            Assert.IsTrue(dashBoardPage.GetCellValue(1, "Team Tags").Contains(SharedConstants.TeamTag), "Tags are incorrect");
        }

        [TestMethod]
        [TestCategory("TeamAdmin2"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public void MultiTeam_ReviewFinishPage_VerifyEditNavigation()
        {
            VerifySetup(ClassInitFailed);

            var login = new LoginPage(Driver, Log);
            var editProfileBasePage = new EditProfileBasePage(Driver, Log);
            var addMtSubTeamPage = new AddMtSubTeamPage(Driver, Log);
            var addTeamMemberPage = new AddTeamMemberPage(Driver, Log);
            var addStakeHolderPage = new AddStakeHolderPage(Driver, Log);
            var finishAndReviewPage = new FinishAndReviewPage(Driver, Log);
            var editTeamBasePage = new EditTeamBasePage(Driver, Log);

            const string teamProfile = "Team Profile";
            const string teamMembers = "Growth Team";
            const string stakeholder = "Stakeholders";
            const string subTeams = "Sub-Teams";

            Log.Info($"Login as {User.FullName}");
            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Navigate to the 'Review & Finish' page");
            finishAndReviewPage.NavigateToPage(_multiTeamId.ToString());

            Log.Info($"On Review & Finish page, Click on 'Edit' link for '{teamProfile}' and verify page title and page url");
            finishAndReviewPage.ClickOnEditLink(teamProfile);
            Assert.AreEqual($"Edit {_multiTeam.Name}", editProfileBasePage.GetTeamProfilePageTitle(), "Edit team profile title is not matched");
            Assert.AreEqual(Driver.GetCurrentUrl(), $"{BaseTest.ApplicationUrl}/teams/profile/{_multiTeamId}", "Edit team profile page url is not matched");

            editProfileBasePage.ClickUpdateTeamProfileButton();
            addMtSubTeamPage.ClickAddSubTeamButton();
            addTeamMemberPage.ClickContinueToAddStakeHolder();
            addStakeHolderPage.ClickReviewAndFinishButton();

            Log.Info($"On Review & Finish page, Click on 'Edit' link for '{subTeams}' and verify page title and page url");
            finishAndReviewPage.ClickOnEditLink(subTeams);
            Assert.AreEqual("Add Sub-Teams", editTeamBasePage.GetPageHeaderTitle(), "Edit sub teams title is not matched");
            Assert.AreEqual(Driver.GetCurrentUrl(), $"{BaseTest.ApplicationUrl}/multiteam/{_multiTeamId}/subteams", "Edit sub teams page url is not matched");

            addMtSubTeamPage.ClickAddSubTeamButton();
            addTeamMemberPage.ClickContinueToAddStakeHolder();
            addStakeHolderPage.ClickReviewAndFinishButton();

            Log.Info($"On Review & Finish page, Click on 'Edit' link for '{teamMembers}' and verify page title and page url");
            finishAndReviewPage.ClickOnEditLink(teamMembers);
            Assert.AreEqual("Add Growth Team", editTeamBasePage.GetPageHeaderTitle(), "Edit Growth team title is not matched");
            Assert.AreEqual(Driver.GetCurrentUrl(), $"{BaseTest.ApplicationUrl}/teammembers/{_multiTeamId}", "Edit Growth team page url is not matched");

            addTeamMemberPage.ClickContinueToAddStakeHolder();
            addStakeHolderPage.ClickReviewAndFinishButton();

            Log.Info($"On Review & Finish page, Click on 'Edit' link for '{stakeholder}' and verify page title and page url");
            finishAndReviewPage.ClickOnEditLink(stakeholder);
            Assert.AreEqual("Add Stakeholders", editTeamBasePage.GetPageHeaderTitle(), "Edit stakeholder title is not matched");
            Assert.AreEqual(Driver.GetCurrentUrl(), $"{BaseTest.ApplicationUrl}/stakeholders/{_multiTeamId}", "Edit stakeholder page url is not matched");
        }

        [TestMethod]
        [TestCategory("TeamAdmin2"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public void MultiTeam_ReviewFinishPage_VerifyClickHereLinkNavigation()
        {
            VerifySetup(ClassInitFailed);

            var login = new LoginPage(Driver, Log);
            var finishAndReviewPage = new FinishAndReviewPage(Driver, Log);
            var addTeamMemberPage = new AddTeamMemberPage(Driver, Log);
            var addStakeHolderPage = new AddStakeHolderPage(Driver, Log);
            var editTeamBasePage = new EditTeamBasePage(Driver, Log);
            var addMtSubTeamPage = new AddMtSubTeamPage(Driver, Log);

            const string teamMembers = "team members";
            const string stakeholder = "stakeholders";
            const string subTeams = "sub-teams";

            Log.Info($"Login as {User.FullName}");
            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Navigate to the 'Review & Finish' page");
            finishAndReviewPage.NavigateToPage(_multiTeamId.ToString());

            Log.Info($"On 'Review & Finish' page, click here link should be present for {subTeams}");
            Assert.IsTrue(finishAndReviewPage.IsClickHereLinkPresent(subTeams));

            Log.Info($"Click on the 'click here' link for {subTeams} and verify the page title and page url");
            finishAndReviewPage.ClickOnClickHereLink(subTeams);
            Assert.AreEqual("Add Sub-Teams", editTeamBasePage.GetPageHeaderTitle(), "Edit sub-teams title is not matched");
            Assert.AreEqual(Driver.GetCurrentUrl(), $"{BaseTest.ApplicationUrl}/multiteam/{_multiTeamId}/subteams", "Edit sub-teams page url is not matched");

            addMtSubTeamPage.ClickAddSubTeamButton();
            addTeamMemberPage.ClickContinueToAddStakeHolder();
            addStakeHolderPage.ClickReviewAndFinishButton();

            Log.Info($"On 'Review & Finish' page, click here link should be present for {teamMembers}");
            Assert.IsTrue(finishAndReviewPage.IsClickHereLinkPresent(teamMembers));

            Log.Info($"Click on the 'click here' link for {teamMembers} and verify the page title and page url");
            finishAndReviewPage.ClickOnClickHereLink(teamMembers);
            Assert.AreEqual("Add Growth Team", editTeamBasePage.GetPageHeaderTitle(), "Edit team members title is not matched");
            Assert.AreEqual(Driver.GetCurrentUrl(), $"{BaseTest.ApplicationUrl}/teammembers/{_multiTeamId}", "Edit team members page url is not matched");

            addTeamMemberPage.ClickContinueToAddStakeHolder();
            addStakeHolderPage.ClickReviewAndFinishButton();

            Log.Info($"On 'Review & Finish' page, click here link should be present for {stakeholder}");
            Assert.IsTrue(finishAndReviewPage.IsClickHereLinkPresent(stakeholder));

            Log.Info($"Click on the 'click here' link for {stakeholder} and verify the page title and page url");
            finishAndReviewPage.ClickOnClickHereLink(stakeholder);
            Assert.AreEqual("Add Stakeholders", editTeamBasePage.GetPageHeaderTitle(), "Edit stakeholders title is not matched");
            Assert.AreEqual(Driver.GetCurrentUrl(), $"{BaseTest.ApplicationUrl}/stakeholders/{_multiTeamId}", "Edit stakeholders page url is not matched");

        }
    }
}
