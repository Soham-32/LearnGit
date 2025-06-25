using AgilityHealth_Automation.Enum.NewNavigation;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace AgilityHealth_Automation.Tests.AgilityHealth.NewNavigation.Teams.Team
{
    [TestClass]
    [TestCategory("Teams"), TestCategory("Team"), TestCategory("NewNavigation")]
    public class AddTeamTeamMembersStepperTests2 : NewNavBaseTest
    {
        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        [TestCategory("KnownDefect")]//Test bug:49568
        public void Team_FullWizard_Verify_Add_TeamMembers_ViaQuickLink()
        {
            Verify_Team_AddTeamMembersLeadersStepper_AddTeamMembersLeaders_ViaQuickLink(TeamType.FullWizardTeam);
        }
    }
}
