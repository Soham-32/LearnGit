using AgilityHealth_Automation.Enum.NewNavigation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.NewNavigation.Teams.MultiTeam
{
    [TestClass]
    [TestCategory("Teams"), TestCategory("Multi-Team"), TestCategory("NewNavigation")]
    public class AddMultiTeamAddLeadersStepperTests3 : NewNavBaseTest
    {
        [TestMethod]
        [TestCategory("KnownDefect")] // Bug : 48260
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public void MultiTeam_Verify_Add_Leaders_ViaQuickLink()
        {
            Verify_Team_AddTeamMembersLeadersStepper_AddTeamMembersLeaders_ViaQuickLink(TeamType.MultiTeam);
        }
    }
}
