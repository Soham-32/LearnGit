using AgilityHealth_Automation.Enum.NewNavigation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.NewNavigation.Teams.PortfolioTeam
{
    [TestClass]
    [TestCategory("Teams"), TestCategory("Multi-Team"), TestCategory("NewNavigation")]
    public class AddPortfolioTeamAddLeadersStepperTests4 : NewNavBaseTest
    {
        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("KnownDefect")] // Bug Id : 45959
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public void PortfolioTeam_Verify_Leader_CanAccessTeam_Successfully()
        {
            Verify_Team_AddTeamMembersLeadersStepper_TeamMemberLeader_CanAccessTeam_Successfully(TeamType.PortfolioTeam);
        }
    }
}
