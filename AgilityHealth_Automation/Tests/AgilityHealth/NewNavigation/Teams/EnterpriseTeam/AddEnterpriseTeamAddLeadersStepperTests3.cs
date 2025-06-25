using AgilityHealth_Automation.Enum.NewNavigation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.NewNavigation.Teams.EnterpriseTeam
{
    [TestClass]
    [TestCategory("Teams"), TestCategory("EnterpriseTeam"), TestCategory("NewNavigation")]
    public class AddEnterpriseTeamAddLeadersStepperTests3 : NewNavBaseTest
    {
        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        [TestCategory("KnownDefect")]//Test Bug 49568
        public void EnterpriseTeamTeam_Verify_Add_TeamMembers_ViaQuickLink()
        {
            Verify_Team_AddTeamMembersLeadersStepper_AddTeamMembersLeaders_ViaQuickLink(TeamType.EnterpriseTeam);
        }
    }
}