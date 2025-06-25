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
    public class NTierAddNTierTeamWithNTierChildTest : NTierBaseTest
    {
        
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
        public void NTier_AddNTierChildToNTierTeam()
        {
            var login = new LoginPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var addMtSubTeamPage = new AddMtSubTeamPage(Driver, Log);
            var createNTierPage = new CreateNTierPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(NTierUser.Username, NTierUser.Password);
            dashBoardPage.CloseDeploymentPopup();
            
            //Add First NTier Team
            var nTierTeamNameChild = "N-TierChild_" + RandomDataUtil.GetTeamName();
            NTier_CreateNTierTeam(nTierTeamNameChild, new List<string>{_eTeam.Name});

            //Add Second NTier Team - up until selecting a subteam
            dashBoardPage.ClickAddATeamButton();
            dashBoardPage.SelectTeamType(TeamType.NTier);
            dashBoardPage.ClickAddTeamButton();
            var nTierTeamNameParent = "N-TierParent_" + RandomDataUtil.GetTeamName();
            createNTierPage.InputNTierTeamName(nTierTeamNameParent);
            createNTierPage.ClickCreateButton();
            addMtSubTeamPage.SelectSubTeam(nTierTeamNameChild);

            Log.Info("Assert: Verify that only N-Tier teams show up");
            var ntRows = createNTierPage.GetListOfSubteam();
            foreach (var team in ntRows)
            {
                Assert.IsTrue(team.Contains("N-Tier"), $"<{team}> does not contain the term 'N-Tier'");
            }

        }
    }
}