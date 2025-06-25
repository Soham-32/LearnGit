using System;
using System.Collections.Generic;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.MultiTeam.Create;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.N_Tier;
using AtCommon.Api;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Teams.NTierTeam
{
    [TestClass]
    [TestCategory("Teams"), TestCategory("NTier")]
    public class NTierAddNTierWithEtTest : NTierBaseTest
    {
        public static string NTierTeamName = "N-Tier Team ET Only Showing_" + RandomDataUtil.GetTeamName();
        private static AddTeamWithMemberRequest _team;
        private static AddTeamWithMemberRequest _mTeam;
        private static List<AddTeamWithMemberRequest> _eTeam;

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            var setup = new SetupTeardownApi(TestEnvironment);

            //create normal team
            _team = TeamFactory.GetNormalTeam("Normal Team for NTier ET Only_");
            var teamResponse = setup.CreateTeam(_team, NTierUser).GetAwaiter().GetResult();

            //create multiteam
            _mTeam = TeamFactory.GetMultiTeam("MultiTeam for NTier ET Only_");
            var multiTeamResponse = setup.CreateTeam(_mTeam, NTierUser).GetAwaiter().GetResult();
            setup.AddSubteams(multiTeamResponse.Uid, new List<Guid> { teamResponse.Uid }, NTierUser).GetAwaiter().GetResult();

            //create enterprise team
            _eTeam = new List<AddTeamWithMemberRequest>
            {
                TeamFactory.GetEnterpriseTeam("Enterprise Team for NTier ET Only 1_"),
                TeamFactory.GetEnterpriseTeam("Enterprise Team for NTier ET Only 2_")
            };

            var enterpriseTeamResponse = setup.CreateTeam(_eTeam[0], NTierUser).GetAwaiter().GetResult();
            setup.AddSubteams(enterpriseTeamResponse.Uid, new List<Guid> {multiTeamResponse.Uid}, NTierUser).GetAwaiter().GetResult();

            setup.CreateTeam(_eTeam[1], NTierUser).GetAwaiter().GetResult();

        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("TeamAdmin2")]
        public void NTier_NTierTeamWithEnterpriseTeamOnlyShowing()
        {
            var login = new LoginPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var addMtSubTeamPage = new AddMtSubTeamPage(Driver, Log);
            var createNTierPage = new CreateNTierPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(NTierUser.Username, NTierUser.Password);
            dashBoardPage.CloseDeploymentPopup();
            
            //Add N-Tier Team
            dashBoardPage.ClickAddATeamButton();
            dashBoardPage.SelectTeamType(TeamType.NTier);
            dashBoardPage.ClickAddTeamButton();

            createNTierPage.InputNTierTeamName(NTierTeamName);
            createNTierPage.ClickCreateButton();

            Log.Info("Assert: Verify only enterprise and n-tier (if any) teams are showing");
            var etAndNtRows = createNTierPage.GetListOfSubteam();
            foreach(var team in etAndNtRows)
            {
                Assert.IsTrue(team.Contains("Enterprise") || team.Contains("N-Tier"), $"<{team}> does not contain the term 'N-Tier' or 'Enterprise'");
            }

            //add subteam
            addMtSubTeamPage.SelectSubTeam(_eTeam[0].Name);

            Log.Info("Assert: Verify only enterprise teams are showing after an enterprise team has been selected");
            var etRows = createNTierPage.GetListOfSubteam();
            foreach(var team in etRows)
            {
                Assert.IsTrue(team.Contains("Enterprise"));
            }
        }
    }
}