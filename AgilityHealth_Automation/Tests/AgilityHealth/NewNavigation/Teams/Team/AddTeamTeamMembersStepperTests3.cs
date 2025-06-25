using AgilityHealth_Automation.Enum.NewNavigation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.NewNavigation.Teams.Team
{
    [TestClass]
    [TestCategory("Teams"), TestCategory("Team"), TestCategory("NewNavigation")]
    public class AddTeamTeamMembersStepperTests3 : NewNavBaseTest
    {
        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("KnownDefect")] // Bug Id : 45959
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public void Team_FullWizard_Verify_TeamMember_CanAccessTeam_Successfully()
        {
            Verify_Team_AddTeamMembersLeadersStepper_TeamMemberLeader_CanAccessTeam_Successfully(TeamType.FullWizardTeam);
        }
    }
}
