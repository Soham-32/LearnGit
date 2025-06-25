using AgilityHealth_Automation.Enum.NewNavigation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.NewNavigation.Teams.EnterpriseTeam
{
    [TestClass]
    [TestCategory("Teams"), TestCategory("EnterpriseTeam"), TestCategory("NewNavigation")]
    public class AddEnterpriseTeamAddLeadersStepperTests4 : NewNavBaseTest
    {
        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        [TestCategory("KnownDefect")]//Bug:50142
        public void EnterpriseTeamTeam_Verify_Leader_CanAccessTeam_Successfully()
        {
            Verify_Team_AddTeamMembersLeadersStepper_TeamMemberLeader_CanAccessTeam_Successfully(TeamType.EnterpriseTeam);
        }
    }
}