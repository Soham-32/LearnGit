using System.Collections.Generic;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Insights;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.PageObjects.AgilityHealth.V2;
using AtCommon.Api;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Teams.NTierTeam
{
    [TestClass]
    [TestCategory("Teams"), TestCategory("NTier")]
    public class NTierAddUnassignedNTierTest : NTierBaseTest
    {
        public static string NTierTeamName1 = "N-Tier1forUnassignedTest_" + RandomDataUtil.GetTeamName();
        public static string NTierTeamName2 = "N-Tier2forUnassignedTest_" + RandomDataUtil.GetTeamName();
        public static string NTierTeamNameUnassigned = "Unassigned N-Tier_" + RandomDataUtil.GetTeamName();
        private static AddTeamWithMemberRequest _eTeam;

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            var setup = new SetupTeardownApi(TestEnvironment);

            //create enterprise team
            _eTeam = TeamFactory.GetEnterpriseTeam("EnterpriseTeamUnassigned_");
            setup.CreateTeam(_eTeam, NTierUser).GetAwaiter().GetResult();
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("TeamAdmin2")]
        public void NTier_AddUnassignedNTier()
        {
            var login = new LoginPage(Driver, Log);
            var insights = new InsightsDashboardPage(Driver, Log);
            var leftNav = new LeftNavPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(NTierUser.Username, NTierUser.Password);
            dashBoardPage.CloseDeploymentPopup();
            
            //Add NTier Team
            NTier_CreateNTierTeam(NTierTeamName1, new List<string>{_eTeam.Name});
            NTier_CreateNTierTeam(NTierTeamName2, new List<string>{NTierTeamName1});
            NTier_CreateNTierTeam(NTierTeamNameUnassigned, new List<string>());

            //verify showing in Insights
            var parentTeamName = "Unassigned";
            insights.NavigateToPage(Company.NtierId);
            leftNav.WaitUntilLoaded();
            var visibleTeamNames = leftNav.GetAllTeamVisibleNames();
            leftNav.ClickOnTeamExpandButton(parentTeamName);
            leftNav.ScrollToTeam(NTierTeamNameUnassigned);
            var unassignedTeamList = leftNav.GetTeamNamesOfSpecificParent(parentTeamName);

            //assert
            Log.Info("Assert: Verify unassigned N-Tier team shows up under unassigned in the left navigation on Insights");
            Assert.IsTrue(unassignedTeamList.Exists(team => team == NTierTeamNameUnassigned.ToUpper()));
            Assert.IsFalse(visibleTeamNames.Exists(team => team == NTierTeamNameUnassigned.ToUpper()));
        }
    }
}