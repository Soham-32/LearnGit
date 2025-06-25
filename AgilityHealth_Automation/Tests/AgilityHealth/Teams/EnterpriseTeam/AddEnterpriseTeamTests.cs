using System;
using System.IO;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.DataObjects;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Common.Create;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Common.Edit;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.EnterpriseTeam;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.MultiTeam.Create;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Team.Create;
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
    public class AddEnterpriseTeamTests : BaseTest
    {
        public static bool ClassInitFailed;
        private static AddTeamWithMemberRequest _enterpriseTeam;
        private static int _enterpriseTeamId;
        [ClassInitialize]
        public static void CreateEnterpriseTeam(TestContext _)
        {
            try
            {
                _enterpriseTeam = TeamFactory.GetEnterpriseTeam("EditET");
                var setup = new SetupTeardownApi(TestEnvironment);
                var enterpriseTeamResponse = setup.CreateTeam(_enterpriseTeam).GetAwaiter().GetResult();
                _enterpriseTeamId = new SetupTeardownApi(TestEnvironment).GetCompanyHierarchy(Company.Id).GetTeamByName(enterpriseTeamResponse.Name).TeamId;
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
        public void EnterpriseTeam_AddNewTeam()
        {
            var login = new LoginPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var createEnterpriseTeamPage = new CreateEnterpriseTeamPage(Driver, Log);
            var addMtSubTeamPage = new AddMtSubTeamPage(Driver, Log);
            var addTeamMemberPage = new AddTeamMemberPage(Driver, Log);
            var addStakeHolderPage = new AddStakeHolderPage(Driver, Log);
            var finishAndReviewPage = new FinishAndReviewPage(Driver, Log);

            Driver.NavigateToPage(ApplicationUrl);

            login.LoginToApplication(User.Username, User.Password);

            dashBoardPage.ClickAddATeamButton();

            dashBoardPage.SelectTeamType(TeamType.EnterpriseTeam);

            dashBoardPage.ClickAddTeamButton();

            var enterpriseTeamInfo = new EnterpriseTeamInfo
            {
                TeamName = "ET_" + RandomDataUtil.GetTeamName(),
                TeamType = "Portfolio Team",
                ExternalIdentifier = "",
                Department = "IT",
                DateEstablished = DateTime.Now,
                AgileAdoptionDate = DateTime.Now,
                Description = "test description for enterprise team",
                TeamBio = $"{RandomDataUtil.GetTeamBio()} for enterprise team",
                ImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\TestData\\2.jpg")
            };

            if (User.IsCompanyAdmin())
                enterpriseTeamInfo.ExternalIdentifier = "external identifier";

            createEnterpriseTeamPage.EnterEnterpriseTeamInfo(enterpriseTeamInfo);
            var teamImageSource = createEnterpriseTeamPage.GetTeamImageSource();
            enterpriseTeamInfo.ImagePath = teamImageSource;

            createEnterpriseTeamPage.GoToAddSubteams();

            addMtSubTeamPage.SelectSubTeam(SharedConstants.MultiTeam);

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

            addTeamMemberPage.ClickContinueToAddStakeHolder();

            //For the enterprise type, no specific roles category exists, So need to comment related code for stakeholders.
            //This case will be reactivated once Dev. team resolves the issue.

            //addStakeHolderPage.ClickAddNewStakeHolderButton();

            //var stakeHolderInfo = new StakeHolderInfo
            //{
            //    FirstName = "Stake",
            //    LastName = SharedConstants.TeamMemberLastName,
            //    Email = "stakeemail" + CSharpHelpers.RandomNumber() + "@sharklasers.com",
            //    Role = SharedConstants.StakeholderRole
            //};

            //addStakeHolderPage.EnterStakeHolderInfo(stakeHolderInfo);
            //addStakeHolderPage.ClickSaveAndCloseButton();

            //var actualStakeHolder = addStakeHolderPage.GetStakeHolderInfoFromGrid(1);
            //Assert.AreEqual(stakeHolderInfo.FirstName, actualStakeHolder.FirstName, "Firstname doesn't match");
            //Assert.AreEqual(stakeHolderInfo.LastName, actualStakeHolder.LastName, "Lastname doesn't match");
            //Assert.AreEqual(stakeHolderInfo.Email, actualStakeHolder.Email, "Email doesn't match");
            //Assert.AreEqual(stakeHolderInfo.Role, actualStakeHolder.Role, "Email doesn't match");

            addStakeHolderPage.ClickReviewAndFinishButton();

            var actualInfo = finishAndReviewPage.GetEnterpriseTeamInfo();
            Assert.AreEqual(enterpriseTeamInfo.AgileAdoptionDate.ToString("MMMM yyyy"), actualInfo.AgileAdoptionDate.ToString("MMMM yyyy"), "Agile Adoption Date doesn't match");
            Assert.AreEqual(enterpriseTeamInfo.DateEstablished.ToString("MMMM yyyy"), actualInfo.DateEstablished.ToString("MMMM yyyy"), "Date Established doesn't match");
            Assert.AreEqual(enterpriseTeamInfo.Department, actualInfo.Department, "Department doesn't match");
            Assert.AreEqual(enterpriseTeamInfo.ExternalIdentifier, actualInfo.ExternalIdentifier, "External Identifier doesn't match");
            Assert.AreEqual(enterpriseTeamInfo.Description, actualInfo.Description, "Description doesn't match");
            Assert.AreEqual(enterpriseTeamInfo.ImagePath, actualInfo.ImagePath, "Image Path doesn't match");
            Assert.AreEqual(enterpriseTeamInfo.TeamBio, actualInfo.TeamBio, "Team BIO doesn't match");
            Assert.AreEqual(enterpriseTeamInfo.TeamName, actualInfo.TeamName, "Team Name doesn't match");

            Assert.IsTrue(finishAndReviewPage.GetSubTeamsText().Contains(SharedConstants.MultiTeam), "Failure !! Sub-team: " + SharedConstants.MultiTeam + " does not display in Finish and Review page");

            var actualTeamMember2 = finishAndReviewPage.GetTeamMemberFromGrid(1);
            Assert.AreEqual(teamMemberInfo.FirstName, actualTeamMember2.FirstName, "Firstname doesn't match");
            Assert.AreEqual(teamMemberInfo.LastName, actualTeamMember2.LastName, "Lastname doesn't match");
            Assert.AreEqual(teamMemberInfo.Email, actualTeamMember2.Email, "Email doesn't match");

            //var actualStakeHolder2 = finishAndReviewPage.GetStakeHolderFromGrid(1);
            //Assert.AreEqual(stakeHolderInfo.FirstName, actualStakeHolder2.FirstName, "Firstname doesn't match");
            //Assert.AreEqual(stakeHolderInfo.LastName, actualStakeHolder2.LastName, "Lastname doesn't match");
            //Assert.AreEqual(stakeHolderInfo.Email, actualStakeHolder2.Email, "Email doesn't match");
            //Assert.AreEqual(stakeHolderInfo.Role, actualStakeHolder2.Role, "Role doesn't match");

            finishAndReviewPage.ClickOnGoToTeamDashboard();

            dashBoardPage.GridTeamView();

            dashBoardPage.SearchTeam(enterpriseTeamInfo.TeamName);

            //Assert.AreEqual(enterpriseTeamInfo.ImagePath, dashBoardPage.GetAvatarSourceFromGrid(1), "Image Path doesn't match");
            Assert.AreEqual(enterpriseTeamInfo.TeamName, dashBoardPage.GetCellValue(1, "Team Name"), "Team Name doesn't match");
            Assert.AreEqual(enterpriseTeamInfo.TeamType, dashBoardPage.GetCellValue(1, "Work Type"), "Team Type doesn't match");
            Assert.AreEqual($"{SharedConstants.TeamTag}, {enterpriseTeamInfo.TeamType}", dashBoardPage.GetCellValue(1, "Team Tags"), "Team Type doesn't match");
        }

        [TestMethod]
        [TestCategory("TeamAdmin2"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public void EnterpriseTeam_AddNewTeam_NoSubTeams()
        {
            var login = new LoginPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var createEnterpriseTeamPage = new CreateEnterpriseTeamPage(Driver, Log);
            var addMtSubTeamPage = new AddMtSubTeamPage(Driver, Log);
            var addTeamMemberPage = new AddTeamMemberPage(Driver, Log);
            var addStakeHolderPage = new AddStakeHolderPage(Driver, Log);
            var finishAndReviewPage = new FinishAndReviewPage(Driver, Log);

            Driver.NavigateToPage(ApplicationUrl);

            login.LoginToApplication(User.Username, User.Password);

            createEnterpriseTeamPage.NavigateToPage(Company.Id.ToString());

            var enterpriseTeamInfo = new EnterpriseTeamInfo
            {
                TeamName = "ET_" + RandomDataUtil.GetTeamName(),
                TeamType = "Portfolio Team"
            };

            createEnterpriseTeamPage.EnterEnterpriseTeamInfo(enterpriseTeamInfo);

            createEnterpriseTeamPage.GoToAddSubteams();

            addMtSubTeamPage.ClickAddSubTeamButton();

            addTeamMemberPage.ClickContinueToAddStakeHolder();

            addStakeHolderPage.ClickReviewAndFinishButton();

            var actualInfo = finishAndReviewPage.GetEnterpriseTeamInfo();

            Assert.AreEqual(enterpriseTeamInfo.TeamName, actualInfo.TeamName, "Team Name doesn't match");
            Assert.AreEqual(enterpriseTeamInfo.TeamType, actualInfo.TeamType, "Team Name doesn't match");

            Assert.AreEqual("There are no sub-teams in this team. Click here to add some sub-teams.", finishAndReviewPage.GetNoSubTeamText());

            dashBoardPage.NavigateToPage(Company.Id);

            dashBoardPage.GridTeamView();

            dashBoardPage.SearchTeam(enterpriseTeamInfo.TeamName);

            Assert.AreEqual(enterpriseTeamInfo.TeamName, dashBoardPage.GetCellValue(1, "Team Name"), "Team Name doesn't match");
            Assert.AreEqual(enterpriseTeamInfo.TeamType, dashBoardPage.GetCellValue(1, "Work Type"), "Team Type doesn't match");
            Assert.AreEqual("0", dashBoardPage.GetCellValue(1, "Number of Sub Teams"), "Sub Team count doesn't match");
        }

        [TestMethod]
        [TestCategory("TeamAdmin2"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public void EnterpriseTeam_ReviewFinishPage_VerifyEditNavigation()
        {
            VerifySetup(ClassInitFailed);

            var login = new LoginPage(Driver, Log);
            var addTeamMemberPage = new AddTeamMemberPage(Driver, Log);
            var addMtSubTeamPage = new AddMtSubTeamPage(Driver, Log);
            var addStakeHolderPage = new AddStakeHolderPage(Driver, Log);
            var editProfileBasePage = new EditProfileBasePage(Driver, Log);
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
            finishAndReviewPage.NavigateToPage(_enterpriseTeamId.ToString());

            Log.Info($"On Review & Finish page, Click on 'Edit' link for '{teamProfile}' and verify page title and page url");
            finishAndReviewPage.ClickOnEditLink(teamProfile);
            Assert.AreEqual($"Edit {_enterpriseTeam.Name}", editProfileBasePage.GetTeamProfilePageTitle(), "Edit team profile title is not matched");
            Assert.AreEqual(Driver.GetCurrentUrl(), $"{BaseTest.ApplicationUrl}/teams/profile/{_enterpriseTeamId}", "Edit team profile page url is not matched");

            editProfileBasePage.ClickUpdateTeamProfileButton();
            addMtSubTeamPage.ClickAddSubTeamButton();
            addTeamMemberPage.ClickContinueToAddStakeHolder();
            addStakeHolderPage.ClickReviewAndFinishButton();

            Log.Info($"On Review & Finish page, Click on 'Edit' link for '{subTeams}' and verify page title and page url");
            finishAndReviewPage.ClickOnEditLink(subTeams);
            Assert.AreEqual("Add Sub-Teams", editTeamBasePage.GetPageHeaderTitle(), "Edit sub teams title is not matched");
            Assert.AreEqual(Driver.GetCurrentUrl(), $"{BaseTest.ApplicationUrl}/enterprise/{_enterpriseTeamId}/subteams", "Edit sub teams page url is not matched");

            addMtSubTeamPage.ClickAddSubTeamButton();
            addTeamMemberPage.ClickContinueToAddStakeHolder();
            addStakeHolderPage.ClickReviewAndFinishButton();

            Log.Info($"On Review & Finish page, Click on 'Edit' link for '{teamMembers}' and verify page title and page url");
            finishAndReviewPage.ClickOnEditLink(teamMembers);
            Assert.AreEqual("Add Growth Team", editTeamBasePage.GetPageHeaderTitle(), "Edit Growth team title is not matched");
            Assert.AreEqual(Driver.GetCurrentUrl(), $"{BaseTest.ApplicationUrl}/teammembers/{_enterpriseTeamId}", "Edit team members page url is not matched");

            addTeamMemberPage.ClickContinueToAddStakeHolder();
            addStakeHolderPage.ClickReviewAndFinishButton();

            Log.Info($"On Review & Finish page, Click on 'Edit' link for '{stakeholder}' and verify page title and page url");
            finishAndReviewPage.ClickOnEditLink(stakeholder);
            Assert.AreEqual("Add Stakeholders", editTeamBasePage.GetPageHeaderTitle(), "Edit stakeholder title is not matched");
            Assert.AreEqual(Driver.GetCurrentUrl(), $"{BaseTest.ApplicationUrl}/stakeholders/{_enterpriseTeamId}", "Edit stakeholder page url is not matched");
        }

        [TestMethod]
        [TestCategory("TeamAdmin2"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public void EnterpriseTeam_ReviewFinishPage_VerifyClickHereLinkNavigation()
        {
            VerifySetup(ClassInitFailed);

            var login = new LoginPage(Driver, Log);
            var addTeamMemberPage = new AddTeamMemberPage(Driver, Log);
            var addMtSubTeamPage = new AddMtSubTeamPage(Driver, Log);
            var addStakeHolderPage = new AddStakeHolderPage(Driver, Log);
            var finishAndReviewPage = new FinishAndReviewPage(Driver, Log);
            var editTeamBasePage = new EditTeamBasePage(Driver, Log);

            const string teamMembers = "team members";
            const string stakeholder = "stakeholders";
            const string subTeams = "sub-teams";

            Log.Info($"Login as {User.FullName}");
            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Navigate to the 'Review & Finish' page");
            finishAndReviewPage.NavigateToPage(_enterpriseTeamId.ToString());

            Log.Info($"On 'Review & Finish' page, click here link should be present for {subTeams}");
            Assert.IsTrue(finishAndReviewPage.IsClickHereLinkPresent(subTeams));

            Log.Info($"Click on the 'click here' link for {subTeams} and verify the page title and page url");
            finishAndReviewPage.ClickOnClickHereLink(subTeams);
            Assert.AreEqual("Add Sub-Teams", editTeamBasePage.GetPageHeaderTitle(), "Edit sub-teams title is not matched");
            Assert.AreEqual(Driver.GetCurrentUrl(), $"{BaseTest.ApplicationUrl}/enterprise/{_enterpriseTeamId}/subteams", "Edit sub-teams page url is not matched");

            addMtSubTeamPage.ClickAddSubTeamButton();
            addTeamMemberPage.ClickContinueToAddStakeHolder();
            addStakeHolderPage.ClickReviewAndFinishButton();

            Log.Info($"On 'Review & Finish' page, click here link should be present for {teamMembers}");
            Assert.IsTrue(finishAndReviewPage.IsClickHereLinkPresent(teamMembers));

            Log.Info($"Click on the 'click here' link for {teamMembers} and verify the page title and page url");
            finishAndReviewPage.ClickOnClickHereLink(teamMembers);
            Assert.AreEqual("Add Growth Team", editTeamBasePage.GetPageHeaderTitle(), "Edit Growth team title is not matched");
            Assert.AreEqual(Driver.GetCurrentUrl(), $"{BaseTest.ApplicationUrl}/teammembers/{_enterpriseTeamId}", "Edit team members page url is not matched");

            addTeamMemberPage.ClickContinueToAddStakeHolder();
            addStakeHolderPage.ClickReviewAndFinishButton();

            Log.Info($"On 'Review & Finish' page, click here link should be present for {stakeholder}");
            Assert.IsTrue(finishAndReviewPage.IsClickHereLinkPresent(stakeholder));

            Log.Info($"Click on the 'click here' link for {stakeholder} and verify the page title and page url");
            finishAndReviewPage.ClickOnClickHereLink(stakeholder);
            Assert.AreEqual("Add Stakeholders", editTeamBasePage.GetPageHeaderTitle(), "Edit stakeholders title is not matched");
            Assert.AreEqual(Driver.GetCurrentUrl(), $"{BaseTest.ApplicationUrl}/stakeholders/{_enterpriseTeamId}", "Edit stakeholders page url is not matched");
        }
    }
}
