using System.Collections.Generic;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AtCommon.Api;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Teams.NTierTeam
{
    [TestClass]
    [TestCategory("NTier"), TestCategory("Teams")]
    public class NTierAddNTierSmokeTest : NTierBaseTest
    {
        private static AddTeamWithMemberRequest _eTeam;

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            //create enterprise team
            _eTeam = TeamFactory.GetEnterpriseTeam("EnterpriseTeamSmoke_");
            new SetupTeardownApi(TestEnvironment).CreateTeam(_eTeam, NTierUser).GetAwaiter().GetResult();
        }

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("Smoke")]
        [TestCategory("Sanity")]
        [TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("TeamAdmin2")]

        public void NTier_CreateSmokeTest()
        {
            var login = new LoginPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(NTierUser.Username, NTierUser.Password);
            dashBoardPage.CloseDeploymentPopup();

            //Add N-Tier Team
            var nTierTeamName = "N-TierTeamSmoke_" + RandomDataUtil.GetTeamName();
            NTier_CreateNTierTeam(nTierTeamName, new List<string> { _eTeam.Name });

            dashBoardPage.GridTeamView();
            dashBoardPage.SearchTeam(nTierTeamName);

            Log.Info("Assert: Verify that team info displays correctly in grid view");
            Assert.AreEqual(nTierTeamName, dashBoardPage.GetCellValue(1, "Team Name"), "Team Name doesn't match");
            Assert.AreEqual(nTierTeamName, dashBoardPage.GetCellValue(1, "Work Type"), "Team Type doesn't match");
        }
    }
}