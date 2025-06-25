using System;
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
    public class NTierAddNTierWithGoiTest : NTierBaseTest
    {
        public static string NTierTeamName = "N-TierGOI_" + RandomDataUtil.GetTeamName();
        private static AddTeamWithMemberRequest _team;
        private static AddTeamWithMemberRequest _mTeam;
        private static AddTeamWithMemberRequest _eTeam;

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            var setup = new SetupTeardownApi(TestEnvironment);

            //create normal team
            _team = TeamFactory.GetGoiTeam("GoiTeam_");
            var teamResponse = setup.CreateTeam(_team, NTierUser).GetAwaiter().GetResult();

            //create multiteam
            _mTeam = TeamFactory.GetMultiTeam("MultiTeamGoi_");
            var multiTeamResponse = setup.CreateTeam(_mTeam, NTierUser).GetAwaiter().GetResult();
            setup.AddSubteams(multiTeamResponse.Uid, new List<Guid> {teamResponse.Uid}, NTierUser).GetAwaiter().GetResult();
            
            //create enterprise team
            _eTeam = TeamFactory.GetEnterpriseTeam("EnterpriseTeamGOI_");
            var enterpriseTeamResponse = setup.CreateTeam(_eTeam, NTierUser).GetAwaiter().GetResult();
            setup.AddSubteams(enterpriseTeamResponse.Uid, new List<Guid> {multiTeamResponse.Uid}, NTierUser).GetAwaiter().GetResult();
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("TeamAdmin2")]
        public void NTier_AddNTierWithoutGOIinLeftNav()
        {
            var login = new LoginPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var insights = new InsightsDashboardPage(Driver, Log);
            var leftNav = new LeftNavPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(NTierUser.Username, NTierUser.Password);
            dashBoardPage.CloseDeploymentPopup();
            
            //Add N-Tier Team
            NTier_CreateNTierTeam(NTierTeamName, new List<string>{_eTeam.Name});

            dashBoardPage.GridTeamView();
            dashBoardPage.SearchTeam(NTierTeamName);

            Log.Info("Assert: Verify that team info displays correctly in grid view");
            Assert.AreEqual(NTierTeamName, dashBoardPage.GetCellValue(1, "Team Name"), "Team Name doesn't match");
            Assert.AreEqual(NTierTeamName, dashBoardPage.GetCellValue(1, "Work Type"), "Team Type doesn't match");

            //verify showing in Insights
            insights.NavigateToPage(Company.NtierId);
            leftNav.ClickOnTeamExpandButton("Unassigned");

            leftNav.ClickOnTeamExpandButton(NTierTeamName);
            leftNav.ClickOnTeamExpandButton(_eTeam.Name);
            leftNav.ClickOnTeamExpandButton(_mTeam.Name);
            var allTeamList = leftNav.GetAllTeamVisibleNames();
            
            Log.Info("Assert: Verify that the GOI Team does not show");
            Assert.That.ListNotContains(allTeamList, _team.Name.ToUpper(), $"{_team.Name} is present");
        }
    }
}