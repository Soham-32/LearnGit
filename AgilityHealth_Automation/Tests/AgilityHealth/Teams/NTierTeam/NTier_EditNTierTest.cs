using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using AgilityHealth_Automation.DataObjects;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Common.Edit;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.N_Tier;
using AtCommon.Api;
using AtCommon.Dtos;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Teams.NTierTeam
{
    [TestClass]
    [TestCategory("Teams"), TestCategory("NTier")]
    public class NTierEditNTierTest : NTierBaseTest
    {
        private static AddTeamWithMemberRequest _eTeam;
        private static readonly UserConfig NTierAhfUserConfig = new UserConfig("CA");
        private static User NTierAhfUser => NTierAhfUserConfig.GetUserByDescription("ntier ahf user");

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            //create enterprise team
            _eTeam = TeamFactory.GetEnterpriseTeam("EnterpriseTeamSmoke_");
            new SetupTeardownApi(TestEnvironment).CreateTeam(_eTeam, NTierUser).GetAwaiter().GetResult();
        }

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin")]
        public void NTier_EditTeamTestAsCompanyAdmin()
        {
            NTier_EditTeamTest(NTierUser);
        }

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin")]
        public void NTier_EditTeamTestAsCompanyFeatureAdmin()
        {
            NTier_EditTeamTest(NTierAhfUser);

        }
        public void NTier_EditTeamTest(User user)
        {
            var login = new LoginPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var editNTierProfilePage = new EditNTierProfilePage(Driver, Log);
            var editTeamBasePage = new EditTeamBasePage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(user.Username, user.Password);
            dashBoardPage.CloseDeploymentPopup();

            //Add N-Tier Team
            var nTierTeamName = "N-TierTeam_" + RandomDataUtil.GetTeamName();
            NTier_CreateNTierTeam(nTierTeamName, new List<string> { _eTeam.Name });

            dashBoardPage.GridTeamView();
            dashBoardPage.SearchTeam(nTierTeamName);

            Log.Info("Assert: Verify that team info displays correctly in grid view");
            Assert.AreEqual(nTierTeamName, dashBoardPage.GetCellValue(1, "Team Name"), "Team Name doesn't match");
            Assert.AreEqual(nTierTeamName, dashBoardPage.GetCellValue(1, "Work Type"), "Team Type doesn't match");

            //Edit N-Tier Team 
            dashBoardPage.ClickTeamEditButton(nTierTeamName);

            var today = DateTime.UtcNow.AddMonths(1).ToString("MMMM yyyy", CultureInfo.InvariantCulture);
            var nTierTeamInfo = new NTierTeamInfo()
            {
                TeamName = "N-TierTeamEdited" + RandomDataUtil.GetTeamName(),
                Department = "Test Edited Department",
                ExternalIdentifier = "Edited External Identifier",
                DateEstablished = today,
                AgileAdoptionDate = today,
                Description = "Test Edited Description",
                TeamBio = RandomDataUtil.GetTeamBio(),
                ImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\TestData\\2.jpg")
            };

            editNTierProfilePage.EnterNTierTeamInfo(nTierTeamInfo);
            nTierTeamInfo.ImagePath = editNTierProfilePage.GetTeamImage();
            editNTierProfilePage.ClickUpdateTeamProfileButton();

            editTeamBasePage.GoToDashboard();

            dashBoardPage.GridTeamView();
            dashBoardPage.SearchTeam(nTierTeamInfo.TeamName);
            dashBoardPage.ClickTeamEditButton(nTierTeamInfo.TeamName);

            var actualMultiTeamInfo = editNTierProfilePage.GetNTierTeamInfo();
            Assert.AreEqual(nTierTeamInfo.TeamName, actualMultiTeamInfo.TeamName, "Team Name doesn't match");
            Assert.AreEqual(nTierTeamInfo.ExternalIdentifier, actualMultiTeamInfo.ExternalIdentifier, "External Identifier doesn't match");
            Assert.AreEqual(nTierTeamInfo.ImagePath, actualMultiTeamInfo.ImagePath, "Image Path doesn't match");
            Assert.AreEqual(nTierTeamInfo.AgileAdoptionDate, actualMultiTeamInfo.AgileAdoptionDate, "Agile Adoption Date doesn't match");
            Assert.AreEqual(nTierTeamInfo.DateEstablished, actualMultiTeamInfo.DateEstablished, "Date Established doesn't match");
            Assert.AreEqual(nTierTeamInfo.Department, actualMultiTeamInfo.Department, "Department doesn't match");
            Assert.AreEqual(nTierTeamInfo.Description, actualMultiTeamInfo.Description, "Description doesn't match");
            Assert.AreEqual(nTierTeamInfo.TeamBio, actualMultiTeamInfo.TeamBio, "Team BIO doesn't match");
        }
    }
}