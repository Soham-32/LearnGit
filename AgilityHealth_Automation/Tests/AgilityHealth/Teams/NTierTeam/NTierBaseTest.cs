using System.Collections.Generic;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.ObjectFactories;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Common.Create;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.MultiTeam.Create;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.N_Tier;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Team.Create;
using AtCommon.Dtos;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Teams.NTierTeam
{
    public class NTierBaseTest : BaseTest
    {
        protected static readonly User NTierUser = TestEnvironment.UserConfig.GetUserByDescription("ntier user");

        public void NTier_CreateNTierTeam(string nTierTeamName, List<string> subTeam)
        {
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var finishAndReviewPage = new FinishAndReviewPage(Driver, Log);
            var addMtSubTeamPage = new AddMtSubTeamPage(Driver, Log);
            var createNTierPage = new CreateNTierPage(Driver, Log);
            var addTeamMemberPage = new AddTeamMemberPage(Driver, Log);
            var addStakeHolderPage = new AddStakeHolderPage(Driver, Log);

            dashBoardPage.ClickAddATeamButton();
            dashBoardPage.SelectTeamType(TeamType.NTier);
            dashBoardPage.ClickAddTeamButton();
            createNTierPage.InputNTierTeamName(nTierTeamName);
            createNTierPage.ClickCreateButton();
            foreach(var team in subTeam)
            {
                addMtSubTeamPage.SelectSubTeam(team);
            }
            addMtSubTeamPage.ClickAddSubTeamButton();
            addTeamMemberPage.ClickAddNewTeamMemberButton();
            var teamMemberInfo = MemberFactory.GetTeamMemberInfo();
            addTeamMemberPage.EnterTeamMemberInfo(teamMemberInfo);
            addTeamMemberPage.ClickSaveAndCloseButton();
            addStakeHolderPage.ClickReviewAndFinishButton();
            finishAndReviewPage.ClickOnGoToTeamDashboard();
        }
    }
}