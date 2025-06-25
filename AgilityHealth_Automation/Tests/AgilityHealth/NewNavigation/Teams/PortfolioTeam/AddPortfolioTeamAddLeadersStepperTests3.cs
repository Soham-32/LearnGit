using AgilityHealth_Automation.Enum.NewNavigation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.NewNavigation.Teams.PortfolioTeam
{
    [TestClass]
    [TestCategory("Teams"), TestCategory("Multi-Team"), TestCategory("NewNavigation")]
    public class AddPortfolioTeamAddLeadersStepperTests3 : NewNavBaseTest
    {
        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        [TestCategory("KnownDefect")]//Test Bug 49568
        public void PortfolioTeam_Verify_Add_TeamMembers_ViaQuickLink()
        {
            Verify_Team_AddTeamMembersLeadersStepper_AddTeamMembersLeaders_ViaQuickLink(TeamType.PortfolioTeam);
        }
    }
}
